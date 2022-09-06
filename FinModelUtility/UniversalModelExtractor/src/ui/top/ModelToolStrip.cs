using fin.model;

using uni.ui.common;


namespace uni.src.ui.top {
  public partial class ModelToolStrip : UserControl {
    public ModelToolStrip() {
      InitializeComponent();
    }

    public IFileTreeNode<IModelFileBundle>? Directory {
      set {
        var hasDirectory = value != null;
        this.exportAllModelsInSelectedDirectoryButton_.Enabled = hasDirectory;
      }
    }

    public IModel? Model {
      set {
        var hasModel = value != null;
        this.exportSelectedModelButton_.Enabled = hasModel;
      }
    }

    private void exportAllModelsInSelectedDirectoryButton__Click(object sender, EventArgs e) {

    }

    private void exportSelectedModelButton__Click(object sender, EventArgs e) {

    }
  }
}
