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
        this.exportAllModelsButton_.Enabled = hasModel;
      }
    }
  }
}
