using System.Diagnostics;

using fin.audio.io;
using fin.color;
using fin.data.queue;
using fin.model.io.exporters.assimp;
using fin.io;
using fin.io.bundles;
using fin.model;
using fin.model.io;
using fin.scene;
using fin.schema.vector;
using fin.util.asserts;
using fin.util.enumerables;
using fin.util.time;

using MathNet.Numerics;

using uni.cli;
using uni.config;
using uni.games;
using uni.ui.common.fileTreeView;

using fin.model.impl;

namespace uni.ui;

public partial class UniversalAssetToolForm : Form {
  private IFileTreeNode? gameDirectory_;
  private TimedCallback fpsCallback_;

  public UniversalAssetToolForm() {
    this.InitializeComponent();

    this.modelTabs_.OnAnimationSelected += animation =>
        this.sceneViewerPanel_.Animation = animation;
    this.modelTabs_.OnBoneSelected += bone => {
      var skeletonRenderer = this.sceneViewerPanel_.SkeletonRenderer;
      if (skeletonRenderer != null) {
        skeletonRenderer.SelectedBone = bone;
      }
    };

    this.modelToolStrip_.Progress.ProgressChanged +=
        (_, currentProgress) => {
          var fractionalProgress = currentProgress.Item1;
          this.cancellableProgressBar_.Value =
              (int) Math.Round(fractionalProgress * 100);

          var modelFileBundle = currentProgress.Item2;
          if (modelFileBundle == null) {
            if (fractionalProgress.AlmostEqual(0, .00001)) {
              this.cancellableProgressBar_.Text = "Nothing to report";
            } else if (fractionalProgress.AlmostEqual(1, .00001)) {
              this.cancellableProgressBar_.Text = "Done!";
            }
          } else {
            this.cancellableProgressBar_.Text =
                $"Extracting {modelFileBundle.DisplayFullPath}...";
          }
        };
    this.cancellableProgressBar_.Clicked += (sender, args)
        => this.modelToolStrip_.CancellationToken?.Cancel();
  }

  private void UniversalAssetToolForm_Load(object sender, EventArgs e) {
    this.fileBundleTreeView_.Populate(
        new RootFileGatherer().GatherAllFiles());


    this.fpsCallback_ =
        TimedCallback.WithPeriod(
            () => {
              if (!this.Created || this.IsDisposed) {
                return;
              }

              try {
                this.Invoke(() => {
                  var frameTime = this.sceneViewerPanel_.FrameTime;
                  var fps = (frameTime == TimeSpan.Zero)
                      ? 0
                      : 1 / frameTime.TotalSeconds;
                  this.Text = $"Universal Asset Tool ({fps:0.0} fps)";
                });
              } catch {
                // ignored, throws after window is closed
              }
            },
            .25f);

    this.fileBundleTreeView_.DirectorySelected += this.OnDirectorySelect_;
    this.fileBundleTreeView_.FileSelected += this.OnFileBundleSelect_;
  }

  private void OnDirectorySelect_(IFileTreeParentNode directoryNode)
    => this.modelToolStrip_.DirectoryNode = directoryNode;

  private void OnFileBundleSelect_(IFileTreeLeafNode fileNode) {
    switch (fileNode.File.FileBundle) {
      case IModelFileBundle modelFileBundle: {
        this.SelectModel_(fileNode, modelFileBundle);
        break;
      }
      case IAudioFileBundle audioFileBundle: {
        this.SelectAudio_(audioFileBundle);
        break;
      }
      case ISceneFileBundle sceneFileBundle: {
        this.SelectScene_(fileNode, sceneFileBundle);
        break;
      }
      default:
        throw new NotImplementedException();
    }
  }

  private void SelectScene_(IFileTreeLeafNode fileNode,
                            ISceneFileBundle sceneFileBundle) {
    var scene = new GlobalSceneImporter().ImportScene(sceneFileBundle,
      out var lighting);
    this.UpdateScene_(fileNode, sceneFileBundle, scene, lighting);
  }

  private void SelectModel_(IFileTreeLeafNode fileNode,
                            IModelFileBundle modelFileBundle)
    => this.UpdateModel_(
        fileNode,
        modelFileBundle,
        new GlobalModelImporter().ImportModel(modelFileBundle));

  private void UpdateModel_(IFileTreeLeafNode? fileNode,
                            IModelFileBundle modelFileBundle,
                            IModel model) {
    var scene = new SceneImpl();
    var area = scene.AddArea();
    var obj = area.AddObject();
    obj.AddSceneModel(model);

    this.UpdateScene_(fileNode,
                      modelFileBundle,
                      scene,
                      this.CreateDefaultLightingForScene_(scene));
  }

  private void UpdateScene_(IFileTreeLeafNode? fileNode,
                            IFileBundle fileBundle,
                            IScene scene,
                            ILighting? lighting) {
    this.sceneViewerPanel_.FileBundleAndSceneAndLighting?.Item2.Dispose();
    this.sceneViewerPanel_.FileBundleAndSceneAndLighting =
        (fileBundle, scene, lighting);

    var model = this.sceneViewerPanel_.FirstSceneModel?.Model;
    this.modelTabs_.Model = (fileBundle, model);
    this.modelTabs_.AnimationPlaybackManager =
        this.sceneViewerPanel_.AnimationPlaybackManager;

    this.modelToolStrip_.DirectoryNode = fileNode?.Parent;
    this.modelToolStrip_.FileNodeAndModel = (fileNode, model);
    this.exportAsToolStripMenuItem.Enabled = fileBundle is IModelFileBundle;

    if (Config.Instance.ViewerSettings.AutomaticallyPlayGameAudioForModel) {
      var gameDirectory = fileNode.Parent;
      while (gameDirectory?.Parent?.Parent != null) {
        gameDirectory = gameDirectory.Parent;
      }

      if (this.gameDirectory_ != gameDirectory) {
        this.audioPlayerPanel_.AudioFileBundles =
            gameDirectory
                .GetFilesOfType<IAudioFileBundle>(true)
                .Select(bundle => bundle.TypedFileBundle)
                .ToArray();
      }

      this.gameDirectory_ = gameDirectory;
    }
  }

  private ILighting? CreateDefaultLightingForScene_(IScene scene) {
    var needsLights = false;
    var neededLightIndices = new HashSet<int>();

    var sceneModelQueue = new FinQueue<ISceneModel>(
        scene.Areas.SelectMany(
            area => area.Objects.SelectMany(obj => obj.Models)));
    while (sceneModelQueue.TryDequeue(out var sceneModel)) {
      sceneModelQueue.Enqueue(sceneModel.Children);

      var finModel = sceneModel.Model;

      var hasNormals = finModel.Skin.Vertices.Any(
          vertex => vertex is IReadOnlyNormalVertex { LocalNormal: { } });
      if (!hasNormals) {
        continue;
      }

      foreach (var finMaterial in finModel.MaterialManager.All) {
        if (finMaterial.IgnoreLights) {
          continue;
        }

        needsLights = true;

        if (finMaterial is not IFixedFunctionMaterial
            finFixedFunctionMaterial) {
          continue;
        }

        var equations = finFixedFunctionMaterial.Equations;
        for (var i = 0; i < 8; ++i) {
          if (equations.HasInput(FixedFunctionSource.LIGHT_0_ALPHA + i) ||
              equations.HasInput(FixedFunctionSource.LIGHT_0_COLOR + i)) {
            neededLightIndices.Add(i);
          }
        }
      }
    }

    if (!needsLights) {
      return null;
    }

    if (neededLightIndices.Count == 0) {
      neededLightIndices.Add(0);
    }

    var enabledCount = neededLightIndices.Count;
    var lightColors = enabledCount == 1
        ? new[] { Color.White }
        : new[] {
            Color.Pink,
            Color.LightBlue,
            Color.DarkSeaGreen,
            Color.PaleGoldenrod,
            Color.Lavender,
            Color.Bisque
        };

    var maxLightIndex = neededLightIndices.Max();
    var currentIndex = 0;
    var lighting = new LightingImpl();
    for (var i = 0; i <= maxLightIndex; ++i) {
      var light = lighting.CreateLight();
      if (!(light.Enabled = neededLightIndices.Contains(i))) {
        continue;
      }

      light.SetColor(FinColor.FromSystemColor(lightColors[currentIndex]));

      var angleInRadians = 2 * MathF.PI *
          (1f * currentIndex) / enabledCount;

      light.SetNormal(new Vector3f {
          X = (float) (.5f * Math.Cos(angleInRadians)),
          Y = (float) (.5f * Math.Sin(angleInRadians)),
          Z = -.5f,
      });
    }

    return lighting;
  }

  private void SelectAudio_(IAudioFileBundle audioFileBundle)
    => this.audioPlayerPanel_.AudioFileBundles = new[] { audioFileBundle };

  private void importToolstripMenuItem_Click(object sender, EventArgs e) {
    var plugins = PluginUtil.Plugins;
    var supportedExtensions =
        plugins.SelectMany(plugin => plugin.FileExtensions).ToHashSet();

    var dialog = new OpenFileDialog {
        CheckFileExists = true,
        Multiselect = true,
        Title = "Select asset(s) for a single model",
        Filter = $"All supported plugin extensions|{string.Join(';',
          supportedExtensions
              .Select(extension => $"*{extension}"))}",
    };
    dialog.FileOk += (o, args) => {
      var inputFiles =
          dialog.FileNames.Select(
              fileName => (IReadOnlySystemFile) new FinFile(fileName));

      var bestMatch =
          plugins.FirstOrDefault(plugin => plugin.SupportsFiles(inputFiles));
      if (bestMatch == null) {
        // TODO: Show an error dialog
        return;
      }

      var finModel =
          bestMatch.ImportModel(inputFiles, out var modelFileBundle);
      this.UpdateModel_(null, modelFileBundle, finModel);
    };

    dialog.ShowDialog();
  }

  private void exportAsToolStripMenuItem_Click(object sender, EventArgs e) {
    var fileBundleAndScene =
        this.sceneViewerPanel_.FileBundleAndSceneAndLighting;
    if (fileBundleAndScene == null) {
      return;
    }

    var (fileBundle, _, _) = fileBundleAndScene.Value;
    var modelFileBundle =
        Asserts.AsA<IAnnotatedFileBundle<IModelFileBundle>>(fileBundle);
    var model = this.sceneViewerPanel_.FirstSceneModel!.Model;

    var allSupportedExportFormats = AssimpUtil.SupportedExportFormats
                                              .OrderBy(ef => ef.Description)
                                              .ToArray();
    var mergedFormat =
        $"Model files|{string.Join(';', allSupportedExportFormats.Select(ef => $"*.{ef.FileExtension}"))}";
    var filter = string.Join('|',
                             mergedFormat.Yield()
                                         .Concat(
                                             allSupportedExportFormats.Select(
                                                 ef
                                                     => $"{ef.Description}|*.{ef.FileExtension}")));

    var fbxIndex = allSupportedExportFormats.Select(ef => ef.FormatId)
                                            .IndexOfOrNegativeOne("fbx");

    var saveFileDialog = new SaveFileDialog();
    saveFileDialog.Filter = filter;
    saveFileDialog.FilterIndex = 2 + fbxIndex;
    saveFileDialog.OverwritePrompt = true;

    var result = saveFileDialog.ShowDialog();
    if (result == DialogResult.OK) {
      var outputFile = new FinFile(saveFileDialog.FileName);
      ExtractorUtil.Extract(modelFileBundle,
                            () => model,
                            outputFile.AssertGetParent(),
                            new[] { outputFile.FileType },
                            true,
                            outputFile.NameWithoutExtension);
    }
  }

  private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    => this.Close();

  private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
    => Process.Start("explorer",
                     "https://github.com/MeltyPlayer/FinModelUtility");

  private void reportAnIssueToolStripMenuItem_Click(
      object sender,
      EventArgs e)
    => Process.Start("explorer",
                     "https://github.com/MeltyPlayer/FinModelUtility/issues/new");
}