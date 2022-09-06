using fin.model;

using uni.ui.common;


namespace uni.src.ui.top {
  public partial class ModelToolStrip : UserControl {
    public ModelToolStrip() {
      InitializeComponent();
    }

    public IFileTreeNode<IModelFileBundle>? DirectoryNode {
      set {
        var hasDirectory = value != null;
        this.exportAllModelsInSelectedDirectoryButton_.Enabled = hasDirectory;

        var tooltipText = "Export all models in selected directory";
        if (hasDirectory) {
          var totalText = this.GetTotalNodeText_(value!);
          tooltipText = $"Export all models in '{totalText}'";
        }
        this.exportAllModelsInSelectedDirectoryButton_.ToolTipText =
            tooltipText;
      }
    }

    public (IFileTreeNode<IModelFileBundle>, IModel?) FileNodeAndModel {
      set {
        var (fileNode, model) = value;

        var hasModel = model != null;
        this.exportSelectedModelButton_.Enabled = hasModel;

        var tooltipText = "Export selected model";
        if (hasModel) {
          var totalText = this.GetTotalNodeText_(fileNode);
          tooltipText = $"Export '{totalText}'";
        }
        this.exportSelectedModelButton_.ToolTipText = tooltipText;
      }
    }

    private void exportAllModelsInSelectedDirectoryButton__Click(
        object sender,
        EventArgs e) { }

    private void
        exportSelectedModelButton__Click(object sender, EventArgs e) { }

    private string GetTotalNodeText_(IFileTreeNode<IModelFileBundle> node) {
      var totalText = "";
      var directory = node;
      while (true) {
        if (totalText.Length > 0) {
          totalText = "/" + totalText;
        }
        totalText = directory.Text + totalText;

        directory = directory.Parent;
        if (directory?.Parent == null) {
          break;
        }
      }
      return totalText;
    }
  }
}