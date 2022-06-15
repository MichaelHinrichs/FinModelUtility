using fin.model;

using glo.api;

using mod.cli;

using uni.games;

using zar.api;


namespace uni.ui {
  public partial class UniversalModelExtractorForm : Form {
    public UniversalModelExtractorForm() {
      InitializeComponent();
    }

    private void UniversalModelExtractorForm_Load(object sender, EventArgs e) {
      this.modelFileTreeView_.Populate(
          new RootModelFileGatherer().GatherAllModelFiles());

      this.modelFileTreeView_.FileSelected += this.OnFileSelect_;
    }

    private void OnFileSelect_(IModelFileBundle modelFileBundle) {
      var model = this.LoadModel_(modelFileBundle);
      this.modelViewerGlPanel_.Model = model;
      this.modelTabs_.Model = model;
    }

    private IModel LoadModel_(IModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          GloModelFileBundle gloModelFileBundle => new GloModelLoader()
              .LoadModel(gloModelFileBundle),
          ModModelFileBundle modModelFileBundle => new ModModelLoader()
              .LoadModel(modModelFileBundle),
          ZarModelFileBundle zarModelFileBundle => new ZarModelLoader()
              .LoadModel(zarModelFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };
  }
}