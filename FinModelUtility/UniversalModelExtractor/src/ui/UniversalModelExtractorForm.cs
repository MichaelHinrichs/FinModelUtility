using fin.audio;
using fin.data.queue;
using fin.io.bundles;
using System.Diagnostics;
using fin.model;
using uni.config;
using uni.games;
using uni.ui.common;


namespace uni.ui;

public partial class UniversalModelExtractorForm : Form {
  private IFileTreeNode<IFileBundle>? gameDirectory_;

  public UniversalModelExtractorForm() {
    this.InitializeComponent();

    this.modelViewerPanel_.AnimationPlaybackManager =
        this.modelTabs_.AnimationPlaybackManager;
    this.modelTabs_.OnAnimationSelected += animation =>
        this.modelViewerPanel_.Animation = animation;
    this.modelTabs_.OnBoneSelected += bone => {
      var skeletonRenderer = this.modelViewerPanel_.SkeletonRenderer;
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
    }
  }

  private void SelectModel_(IFileTreeNode<IFileBundle> fileNode,
                            IModelFileBundle modelFileBundle) {
    var model = new GlobalModelLoader().LoadModel(modelFileBundle);

    this.modelToolStrip_.DirectoryNode = fileNode.Parent;
    this.modelToolStrip_.FileNodeAndModel = (fileNode, model);
    this.modelViewerPanel_.ModelAndFileBundle = (modelFileBundle, model);
    this.modelTabs_.Model = model;

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
    this.audioPlayerPanel_.AudioFileBundles = new[] { audioFileBundle };
  }

  private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
    this.Close();
  }

  private void gitHubToolStripMenuItem_Click(object sender, EventArgs e) {
    Process.Start("explorer",
                  "https://github.com/MeltyPlayer/FinModelUtility");
  }

  private void reportAnIssueToolStripMenuItem_Click(object sender, EventArgs e) {
    Process.Start("explorer",
      "https://github.com/MeltyPlayer/FinModelUtility/issues/new");
  }
}