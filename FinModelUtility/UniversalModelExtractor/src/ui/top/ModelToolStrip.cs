using fin.data.queue;
using fin.model;

using uni.games;
using uni.ui;
using uni.ui.common;


namespace uni.src.ui.top {
  public partial class ModelToolStrip : UserControl {
    private IFileTreeNode<IModelFileBundle>? directoryNode_;
    private (IFileTreeNode<IModelFileBundle>, IModel)? fileNodeAndModel_;

    public ModelToolStrip() {
      InitializeComponent();
    }

    public IFileTreeNode<IModelFileBundle>? DirectoryNode {
      set {
        var hasDirectory = value != null;
        this.exportAllModelsInSelectedDirectoryButton_.Enabled = hasDirectory;
        this.directoryNode_ = value;

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
        if (hasModel) {
          this.fileNodeAndModel_ = (fileNode, model!);
        } else {
          this.fileNodeAndModel_ = null;
        }

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
        EventArgs e) {
      var allModelFileBundles = new List<IModelFileBundle>();

      var subdirQueue =
          new FinQueue<IFileTreeNode<IModelFileBundle>>(this.directoryNode_!);
      while (subdirQueue.TryDequeue(out var subdirNode)) {
        var modelFileBundle = subdirNode.File;
        if (modelFileBundle != null) {
          allModelFileBundles.Add(modelFileBundle);
        } else {
          subdirQueue.Enqueue(subdirNode.Children);
        }
      }

      ExtractorUtil.ExtractAll(allModelFileBundles, new GlobalModelLoader());
    }

    private void
        exportSelectedModelButton__Click(object sender, EventArgs e) {
      var (fileNode, model) = this.fileNodeAndModel_.Value;
      ExtractorUtil.Extract(fileNode.File!, () => model);
    }

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