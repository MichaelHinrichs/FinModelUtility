using fin.io.bundles;
using System.Diagnostics;
using fin.model;
using uni.games;
using uni.ui.common;


namespace uni.ui;

public partial class UniversalModelExtractorForm : Form {
  public UniversalModelExtractorForm() {
    this.InitializeComponent();

    this.modelViewerGlPanel_.AnimationPlaybackManager =
        this.modelTabs_.AnimationPlaybackManager;
    this.modelTabs_.OnAnimationSelected += animation =>
        this.modelViewerGlPanel_.Animation = animation;
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
    var model = (fileNode.File is IModelFileBundle modelFileBundle)
                    ? this.LoadModel_(modelFileBundle)
                    : null;

    this.modelToolStrip_.DirectoryNode = fileNode.Parent;
    this.modelToolStrip_.FileNodeAndModel = (fileNode, model);
    this.modelViewerGlPanel_.Model = model;
    this.modelTabs_.Model = model;
  }

  private IModel LoadModel_(IModelFileBundle modelFileBundle) {
    return new GlobalModelLoader().LoadModel(modelFileBundle);
  }

  private void modelViewerGlPanel__Load(object sender, EventArgs e) { }

  private void gitHubToolStripMenuItem_Click(object sender, EventArgs e) {
    Process.Start("explorer",
                  "https://github.com/MeltyPlayer/FinModelUtility");
  }
}