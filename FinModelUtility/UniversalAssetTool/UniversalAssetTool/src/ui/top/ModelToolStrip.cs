using fin.data.queue;
using fin.io.bundles;
using fin.model;
using fin.model.io;
using fin.util.progress;

using MathNet.Numerics;

using uni.config;
using uni.games;
using uni.ui.common;

using static uni.games.ExtractorUtil;

namespace uni.ui.top {
  public partial class ModelToolStrip : UserControl {
    private IFileTreeNode<IFileBundle>? directoryNode_;
    private (IFileTreeNode<IFileBundle>, IModel)? fileNodeAndModel_;

    private bool hasModelsInDirectory_;
    private bool isModelSelected_;

    public ModelToolStrip() {
      InitializeComponent();

      var config = Config.Instance;
      var viewerSettings = config.ViewerSettings;

      var showBonesButton = this.showBonesButton_;
      showBonesButton.Checked = viewerSettings.ShowSkeleton;
      showBonesButton.CheckedChanged += (_, e) => {
        viewerSettings.ShowSkeleton = showBonesButton.Checked;
      };

      var showGridButton = this.showGridButton_;
      showGridButton.Checked = viewerSettings.ShowGrid;
      showGridButton.CheckedChanged += (_, e) => {
        viewerSettings.ShowGrid = showGridButton.Checked;
      };

      var automaticallyPlayMusicButton = this.automaticallyPlayMusicButton_;
      automaticallyPlayMusicButton.Checked =
          viewerSettings.AutomaticallyPlayGameAudioForModel;
      automaticallyPlayMusicButton.CheckedChanged += (_, e) => {
        viewerSettings.AutomaticallyPlayGameAudioForModel = showGridButton.Checked;
      };

      this.Progress.ProgressChanged += (_, e) => {
        this.AttemptToUpdateExportSelectedModelButtonEnabledState_();
        this.AttemptToUpdateExportAllModelsInSelectedDirectoryButtonEnabledState_();
      };
    }

    public MemoryProgress<(float, IModelFileBundle?)> Progress { get; } =
      new((0, null));

    public CancellationTokenSource? CancellationToken { get; private set; }

    public bool IsStarted => this.CancellationToken != null;
    public bool IsInProgress
      => this.IsStarted && !this.Progress.Current.Item1.AlmostEqual(1, .000001);

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

        this.hasModelsInDirectory_ = modelCount > 0;
        this.AttemptToUpdateExportAllModelsInSelectedDirectoryButtonEnabledState_();
        this.exportAllModelsInSelectedDirectoryButton_.ToolTipText =
            tooltipText;
      }
    }

    public (IFileTreeNode<IFileBundle>, IModel?) FileNodeAndModel {
      set {
        var (fileNode, model) = value;

        this.isModelSelected_ = fileNode.File is IModelFileBundle;
        if (this.isModelSelected_) {
          this.fileNodeAndModel_ = (fileNode, model!);
        } else {
          this.fileNodeAndModel_ = null;
        }

        var tooltipText = "Export selected model";
        if (this.isModelSelected_) {
          var totalText = this.GetTotalNodeText_(fileNode);
          tooltipText = $"Export '{totalText}'";
        }

        this.AttemptToUpdateExportSelectedModelButtonEnabledState_();
        this.exportSelectedModelButton_.ToolTipText = tooltipText;
      }
    }

    public void AttemptToUpdateExportSelectedModelButtonEnabledState_() {
      this.exportSelectedModelButton_.Enabled =
          !this.IsInProgress && this.isModelSelected_;
    }

    public void AttemptToUpdateExportAllModelsInSelectedDirectoryButtonEnabledState_() {
      this.exportAllModelsInSelectedDirectoryButton_.Enabled =
          !this.IsInProgress && this.hasModelsInDirectory_;
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

      this.StartExportingModelsInBackground_(allModelFileBundles);
    }

    private void exportSelectedModelButton__Click(object sender, EventArgs e) {
      if (this.fileNodeAndModel_ == null) {
        return;
      }

      var (fileNode, _) = this.fileNodeAndModel_.Value;
      var modelFileBundle = fileNode.File as IModelFileBundle;
      this.StartExportingModelsInBackground_(new[] { modelFileBundle});
    }

    private void StartExportingModelsInBackground_(
        IReadOnlyList<IModelFileBundle> modelFileBundles) {
      var extractorPromptChoice =
          ExtractorUtil.PromptIfModelFileBundlesAlreadyExtracted(
              modelFileBundles,
              Config.Instance.ExporterSettings.ExportedFormats);
      if (extractorPromptChoice != ExtractorPromptChoice.CANCEL) {
        this.CancellationToken = new CancellationTokenSource();

        Task.Run(() => {
          ExtractorUtil.ExtractAll(modelFileBundles,
                                   new GlobalModelImporter(),
                                   this.Progress,
                                   this.CancellationToken,
                                   Config.Instance.ExporterSettings.ExportedFormats,
                                   extractorPromptChoice == ExtractorPromptChoice.OVERWRITE_EXISTING);
        });
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