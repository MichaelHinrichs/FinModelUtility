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
        config.ShowSkeleton = showBonesButton.Checked;
      };

      var showGridButton = this.showGridButton_;
      showGridButton.Checked = config.ShowGrid;
      showGridButton.CheckedChanged += (_, e) => {
        config.ShowGrid = showGridButton.Checked;
      };

      var automaticallyPlayMusicButton = this.automaticallyPlayMusicButton_;
      automaticallyPlayMusicButton.Checked =
          config.AutomaticallyPlayGameAudioForModel;
      automaticallyPlayMusicButton.CheckedChanged += (_, e) => {
        config.AutomaticallyPlayGameAudioForModel = showGridButton.Checked;
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

      var extractorPromptChoice =
          ExtractorUtil.PromptIfModelFileBundlesAlreadyExtracted(
              allModelFileBundles, Config.Instance.ExportedFormats);
      if (extractorPromptChoice != ExtractorUtil.ExtractorPromptChoice.CANCEL) {
        ExtractorUtil.ExtractAll(allModelFileBundles,
                                 new GlobalModelLoader(),
                                 Config.Instance.ExportedFormats,
                                 extractorPromptChoice == ExtractorUtil
                                     .ExtractorPromptChoice.OVERWRITE_EXISTING);
      }
    }

    private void exportSelectedModelButton__Click(object sender, EventArgs e) {
      if (this.fileNodeAndModel_ == null) {
        return;
      }

      var (fileNode, model) = this.fileNodeAndModel_.Value;
      var modelFileBundle = fileNode.File as IModelFileBundle;
      var extractorPromptChoice =
          ExtractorUtil.PromptIfModelFileBundlesAlreadyExtracted(
              new[] { modelFileBundle }, Config.Instance.ExportedFormats);
      if (extractorPromptChoice != ExtractorUtil.ExtractorPromptChoice.CANCEL) {
        ExtractorUtil.Extract(modelFileBundle,
                              () => model,
                              Config.Instance.ExportedFormats,
                              extractorPromptChoice == ExtractorUtil
                                  .ExtractorPromptChoice.OVERWRITE_EXISTING);
      }
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