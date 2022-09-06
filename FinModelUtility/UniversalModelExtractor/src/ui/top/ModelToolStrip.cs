using fin.model;


namespace uni.src.ui.top {
  public partial class ModelToolStrip : UserControl {
    public ModelToolStrip() {
      InitializeComponent();
    }

    public IModel? Model {
      set {
        var hasModel = value != null;
        this.exportSelectedModelButton_.Enabled = hasModel;
        this.exportAllModelsInSelectedDirectoryButton_.Enabled = hasModel;
      }
    }

    private void exportAllModelsInSelectedDirectoryButton__Click(object sender, EventArgs e) {

    }

    private void exportSelectedModelButton__Click(object sender, EventArgs e) {

    }
  }
}
