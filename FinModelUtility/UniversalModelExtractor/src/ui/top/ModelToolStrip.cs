using fin.data.queue;
using fin.io.bundles;
using fin.model;
using uni.config;
using uni.games;
using uni.ui.common;


namespace uni.ui.top {
  public partial class ModelToolStrip : UserControl {
    private IFileTreeNode<IFileBundle>? directoryNode_;
    private (IFileTreeNode<IFileBundle>, IModel)? fileNodeAndModel_;

    public ModelToolStrip() {
      InitializeComponent();

      var config = Config.Instance;

      var showBonesButton = this.showBonesButton_;
      showBonesButton.Checked = config.ShowSkeleton;
      showBonesButton.CheckedChanged += (_, e) => {
        Config.Instance.ShowSkeleton = showBonesButton.Checked;
      };

      var showGridButton = this.showGridButton_;
      showGridButton.Checked = config.ShowGrid;
      showGridButton.CheckedChanged += (_, e) => {
        Config.Instance.ShowGrid = showGridButton.Checked;
      };
    }

    public IFileTreeNode<IFileBundle>? DirectoryNode {
      set {
        var hasDirectory = value != null;
        this.directoryNode_ = value;

        var tooltipText = "Export all models in selected directory";
        var modelCount = 0;
        if (hasDirectory) {
          var subdirQueue = new FinQueue<IFileTreeNode<IFileBundle>>(value!);
          while (subdirQueue.TryDequeue(out var subdirNode)) {
            if (subdirNode.File is IModelFileBundle) {
              ++modelCount;
            } else {
              subdirQueue.Enqueue(subdirNode.Children);
            }
          }

          var totalText = this.GetTotalNodeText_(value!);
          tooltipText = modelCount == 1
                            ? $"Export {modelCount} model in '{totalText}'"
                            : $"Export all {modelCount} models in '{totalText}'";
        }

        this.exportAllModelsInSelectedDirectoryButton_.Enabled = modelCount > 0;
        this.exportAllModelsInSelectedDirectoryButton_.ToolTipText =
            tooltipText;
      }
    }

    public (IFileTreeNode<IFileBundle>, IModel?) FileNodeAndModel {
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
          new FinQueue<IFileTreeNode<IFileBundle>>(this.directoryNode_!);
      while (subdirQueue.TryDequeue(out var subdirNode)) {
        if (subdirNode.File is IModelFileBundle modelFileBundle) {
          allModelFileBundles.Add(modelFileBundle);
        } else {
          subdirQueue.Enqueue(subdirNode.Children);
        }
      }

      ExtractorUtil.ExtractAll(allModelFileBundles, new GlobalModelLoader());
    }

    private void exportSelectedModelButton__Click(object sender, EventArgs e) {
      if (this.fileNodeAndModel_ == null) {
        return;
      }

      var (fileNode, model) = this.fileNodeAndModel_.Value;
      ExtractorUtil.Extract(fileNode.File as IModelFileBundle, () => model);
    }

    private string GetTotalNodeText_(IFileTreeNode<IFileBundle> node) {
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