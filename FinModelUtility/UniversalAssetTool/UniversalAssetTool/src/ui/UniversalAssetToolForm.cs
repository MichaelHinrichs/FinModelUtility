using System.Diagnostics;

using fin.audio;
using fin.color;
using fin.data.queue;
using fin.exporter.assimp;
using fin.io;
using fin.io.bundles;
using fin.math.rotations;
using fin.model;
using fin.scene;
using fin.util.enumerables;
using fin.util.time;

using MathNet.Numerics;

using uni.config;
using uni.games;
using uni.ui.common;

namespace uni.ui;

public partial class UniversalAssetToolForm : Form {
  private IFileTreeNode<IFileBundle>? gameDirectory_;
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


    this.fpsCallback_ = TimedCallback.WithPeriod(() => {
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
    }, .25f);
 
    this.fileBundleTreeView_.DirectorySelected += this.OnDirectorySelect_;
    this.fileBundleTreeView_.FileSelected += this.OnFileBundleSelect_;
  }

  private void OnDirectorySelect_(IFileTreeNode<IFileBundle> directoryNode)
    => this.modelToolStrip_.DirectoryNode = directoryNode;

  private void OnFileBundleSelect_(IFileTreeNode<IFileBundle> fileNode) {
    switch (fileNode.File) {
      case IModelFileBundle modelFileBundle: {
        this.SelectModel_(fileNode, modelFileBundle);
        break;
      }
      case IAudioFileBundle audioFileBundle: {
        this.SelectAudio_(fileNode, audioFileBundle);
        break;
      }
      case ISceneFileBundle sceneFileBundle: {
        this.SelectScene_(fileNode, sceneFileBundle);
        break;
      }
    }
  }

  private void SelectScene_(IFileTreeNode<IFileBundle> fileNode,
                            ISceneFileBundle sceneFileBundle) {
    var scene = new GlobalSceneLoader().LoadScene(sceneFileBundle);
    this.UpdateScene_(fileNode, sceneFileBundle, scene);
  }

  private void SelectModel_(IFileTreeNode<IFileBundle> fileNode,
                            IModelFileBundle modelFileBundle) {
    var model = new GlobalModelLoader().LoadModel(modelFileBundle);

    var scene = new SceneImpl();
    var area = scene.AddArea();
    var obj = area.AddObject();
    var sceneModel = obj.AddSceneModel(model);

    // TODO: Need to be able to pass lighting into model from scene
    IReadOnlyList<ILight> lights = model.Lighting.Lights;
    if (lights.Count == 0) {
      model.Lighting.CreateLight()
                       .SetColor(FinColor.FromRgbFloats(1, 1, 1));
    }

    // Initializes first light if uninitialized
    var firstLight = lights[0];
    var normal = firstLight.Normal;
    if (normal.X.AlmostEqual(0) &&
        normal.Y.AlmostEqual(0) &&
        normal.Z.AlmostEqual(0)) {
      FinTrig.FromPitchYawDegrees(-45,
                                  45,
                                  out var normalX,
                                  out var normalY,
                                  out var normalZ);
      normal.X = normalX;
      normal.Y = normalY;
      normal.Z = normalZ;
    }

    if (Config.Instance.ViewerSettings.RotateLight) {
      var stopwatch = new FrameStopwatch();
      stopwatch.Start();

      obj.SetOnTickHandler(_ => {
        var time = stopwatch.Elapsed.TotalMilliseconds;
        var baseAngleInRadians = time / 400;

        var enabledCount = 0;
        foreach (var light in lights) {
          if (light.Enabled) {
            enabledCount++;
          }
        }

        var currentIndex = 0;
        foreach (var light in lights) {
          if (light.Enabled) {
            var angleInRadians = baseAngleInRadians +
                                 2 * MathF.PI *
                                 (1f * currentIndex / enabledCount);

            var normal = light.Normal;
            normal.X = (float) (.5f * Math.Cos(angleInRadians));
            normal.Y = (float) (.5f * Math.Sin(angleInRadians));
            normal.Z = (float) (.5f * Math.Cos(2 * angleInRadians));

            currentIndex++;
          }
        }
      });
    }

    this.UpdateScene_(fileNode, modelFileBundle, scene);
  }

  private void UpdateScene_(IFileTreeNode<IFileBundle> fileNode,
                            IFileBundle fileBundle,
                            IScene scene) {
    this.sceneViewerPanel_.FileBundleAndScene?.Item2.Dispose();
    this.sceneViewerPanel_.FileBundleAndScene = (fileBundle, scene);

    var model = this.sceneViewerPanel_.FirstSceneModel?.Model;
    this.modelTabs_.Model = (fileBundle, model);
    this.modelTabs_.AnimationPlaybackManager =
        this.sceneViewerPanel_.AnimationPlaybackManager;

    this.modelToolStrip_.DirectoryNode = fileNode.Parent;
    this.modelToolStrip_.FileNodeAndModel = (fileNode, model);
    this.exportAsToolStripMenuItem.Enabled = fileBundle is IModelFileBundle;

    if (Config.Instance.ViewerSettings.AutomaticallyPlayGameAudioForModel) {
      var gameDirectory = fileNode.Parent;
      while (gameDirectory?.Parent?.Parent != null) {
        gameDirectory = gameDirectory.Parent;
      }

      if (this.gameDirectory_ != gameDirectory) {
        var audioFileBundles = new List<IAudioFileBundle>();

        var nodeQueue =
            new FinQueue<IFileTreeNode<IFileBundle>?>(gameDirectory);
        while (nodeQueue.TryDequeue(out var node)) {
          if (node == null) {
            continue;
          }

          if (node.File is IAudioFileBundle audioFileBundle) {
            audioFileBundles.Add(audioFileBundle);
          }

          nodeQueue.Enqueue(node.Children);
        }

        this.audioPlayerPanel_.AudioFileBundles = audioFileBundles;
      }

      this.gameDirectory_ = gameDirectory;
    }
  }

  private void SelectAudio_(IFileTreeNode<IFileBundle> fileNode,
                            IAudioFileBundle audioFileBundle)
    => this.audioPlayerPanel_.AudioFileBundles = new[] {audioFileBundle};

  private void exportAsToolStripMenuItem_Click(object sender, EventArgs e) {
    var fileBundleAndScene = this.sceneViewerPanel_.FileBundleAndScene;
    if (fileBundleAndScene == null) {
      return;
    }

    var (fileBundle, scene) = fileBundleAndScene.Value;
    var modelFileBundle = fileBundle as IModelFileBundle;
    var model = this.sceneViewerPanel_.FirstSceneModel!.Model;

    var allSupportedExportFormats = AssimpUtil.SupportedExportFormats
                                              .OrderBy(ef => ef.Description)
                                              .ToArray();
    var mergedFormat =
        $"Model files|{string.Join(';', allSupportedExportFormats.Select(ef => $"*.{ef.FileExtension}"))}";
    var filter = string.Join('|', mergedFormat.Yield().Concat(allSupportedExportFormats.Select(
                                 ef => $"{ef.Description}|*.{ef.FileExtension}")));

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

  private void
      reportAnIssueToolStripMenuItem_Click(object sender, EventArgs e)
    => Process.Start("explorer",
                     "https://github.com/MeltyPlayer/FinModelUtility/issues/new");
}