using bmd.exporter;

using fin.model;

using glo.api;

using mod.cli;

using modl.api;

using uni.games;

using cmb.api;

using dat.api;

using fin.util.asserts;

using hw.api;

using uni.ui.common;


namespace uni.ui {
  public partial class UniversalModelExtractorForm : Form {
    public UniversalModelExtractorForm() {
      InitializeComponent();

      this.modelViewerGlPanel_.AnimationPlaybackManager =
          this.modelTabs_.AnimationPlaybackManager;
      this.modelTabs_.OnAnimationSelected += animation =>
          this.modelViewerGlPanel_.Animation = animation;
    }

    private void UniversalModelExtractorForm_Load(object sender, EventArgs e) {
      this.modelFileTreeView_.Populate(
          new RootModelFileGatherer().GatherAllModelFiles());

      this.modelFileTreeView_.DirectorySelected += this.OnDirectorySelect_;
      this.modelFileTreeView_.FileSelected += this.OnFileSelect_;
    }

    private void OnDirectorySelect_(
        IFileTreeNode<IModelFileBundle> directoryNode) {
      this.modelToolStrip_.DirectoryNode = directoryNode;
    }

    private void OnFileSelect_(IFileTreeNode<IModelFileBundle> fileNode) {
      var model = this.LoadModel_(Asserts.CastNonnull(fileNode.File));
      this.modelToolStrip_.DirectoryNode = fileNode.Parent;
      this.modelToolStrip_.FileNodeAndModel = (fileNode, model);
      this.modelViewerGlPanel_.Model = model;
      this.modelTabs_.Model = model;
    }

    private IModel LoadModel_(IModelFileBundle modelFileBundle)
      => new GlobalModelLoader().LoadModel(modelFileBundle);

    private void modelViewerGlPanel__Load(object sender, EventArgs e) { }
  }
}