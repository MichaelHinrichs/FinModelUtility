using bmd.exporter;

using fin.model;

using glo.api;

using mod.cli;

using modl.api;

using uni.games;

using cmb.api;

using dat.api;


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

      this.modelFileTreeView_.FileSelected += this.OnFileSelect_;
    }

    private void OnFileSelect_(IModelFileBundle modelFileBundle) {
      var model = this.LoadModel_(modelFileBundle);
      this.modelToolStrip1.Model = model;
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
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };

    private void modelViewerGlPanel__Load(object sender, EventArgs e) { }
  }
}