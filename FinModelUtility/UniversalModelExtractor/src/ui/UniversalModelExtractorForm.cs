using fin.audio;
using fin.data.queue;
using fin.io.bundles;
using System.Diagnostics;

using fin.color;
using fin.model;
using fin.scene;
using fin.schema.color;
using fin.schema.vector;

using uni.config;
using uni.games;
using uni.ui.common;


namespace uni.ui;

public partial class UniversalModelExtractorForm : Form {
  private IFileTreeNode<IFileBundle>? gameDirectory_;

  public UniversalModelExtractorForm() {
    this.InitializeComponent();

    this.modelTabs_.OnAnimationSelected += animation =>
        this.sceneViewerPanel_.Animation = animation;
    this.modelTabs_.OnBoneSelected += bone => {
      var skeletonRenderer = this.sceneViewerPanel_.SkeletonRenderer;
      if (skeletonRenderer != null) {
        skeletonRenderer.SelectedBone = bone;
      }
    };
  }

  private void UniversalModelExtractorForm_Load(object sender, EventArgs e) {
    this.fileBundleTreeView_.Populate(
        new RootModelFileGatherer().GatherAllModelFiles());

    this.fileBundleTreeView_.DirectorySelected += this.OnDirectorySelect_;
    this.fileBundleTreeView_.FileSelected += this.OnFileBundleSelect_;
  }

  private void OnDirectorySelect_(IFileTreeNode<IFileBundle> directoryNode) {
    this.modelToolStrip_.DirectoryNode = directoryNode;
  }

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

    var start = DateTime.Now;
    obj.SetOnTickHandler(_ => {
      var current = DateTime.Now;
      var elapsed = current - start;
      
      var time = elapsed.TotalMilliseconds;
      var baseAngleInRadians = time / 300;

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
          normal.Z = -1;

          currentIndex++;
        }
      }

    });

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

    if (Config.Instance.AutomaticallyPlayGameAudioForModel) {
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
                            IAudioFileBundle audioFileBundle) {
    this.audioPlayerPanel_.AudioFileBundles = new[] {audioFileBundle};
  }

  private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
    this.Close();
  }

  private void gitHubToolStripMenuItem_Click(object sender, EventArgs e) {
    Process.Start("explorer",
                  "https://github.com/MeltyPlayer/FinModelUtility");
  }

  private void
      reportAnIssueToolStripMenuItem_Click(object sender, EventArgs e) {
    Process.Start("explorer",
                  "https://github.com/MeltyPlayer/FinModelUtility/issues/new");
  }
}