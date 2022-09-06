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
      this.modelToolStrip_.Directory = directoryNode;
    }

    private void OnFileSelect_(IFileTreeNode<IModelFileBundle> fileNode) {
      var model = this.LoadModel_(Asserts.CastNonnull(fileNode.File));
      this.modelToolStrip_.Directory = fileNode.Parent;
      this.modelToolStrip_.Model = model;
      this.modelViewerGlPanel_.Model = model;
      this.modelTabs_.Model = model;
    }

    private IModel LoadModel_(IModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          BmdModelFileBundle bmdModelFileBundle
              => new BmdModelLoader().LoadModel(bmdModelFileBundle),
          CmbModelFileBundle cmbModelFileBundle
              => new CmbModelLoader().LoadModel(cmbModelFileBundle),
          DatModelFileBundle datModelFileBundle
              => new DatModelLoader().LoadModel(datModelFileBundle),
          GloModelFileBundle gloModelFileBundle
              => new GloModelLoader().LoadModel(gloModelFileBundle),
          ModModelFileBundle modModelFileBundle
              => new ModModelLoader().LoadModel(modModelFileBundle),
          ModlModelFileBundle modlModelFileBundle
              => new ModlModelLoader().LoadModel(modlModelFileBundle),
          OutModelFileBundle outModelFileBundle
              => new OutModelLoader().LoadModel(outModelFileBundle),
          VisModelFileBundle visModelFileBundle
              => new VisModelLoader().LoadModel(visModelFileBundle),
          XtdModelFileBundle xtdModelFileBundle
              => new XtdModelLoader().LoadModel(xtdModelFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };

    private void modelViewerGlPanel__Load(object sender, EventArgs e) { }
  }
}