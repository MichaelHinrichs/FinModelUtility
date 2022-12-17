using fin.animation.playback;
using fin.gl.model;
using fin.io.bundles;
using fin.model;


namespace uni.ui.common.model {
  public partial class ModelViewerPanel : UserControl, IModelViewerPanel {
    public ModelViewerPanel() {
      this.InitializeComponent();
    }

    public (IFileBundle, IModel)? FileBundleAndModel {
      get => this.impl_.FileBundleAndModel;
      set {
        var fileBundle = value?.Item1;
        if (fileBundle != null) {
          this.groupBox_.Text = fileBundle.FullPath;
        } else {
          this.groupBox_.Text = "(Select a model)";
        }

        this.impl_.FileBundleAndModel = value;
      }
    }

    public IAnimationPlaybackManager AnimationPlaybackManager {
      get => this.impl_.AnimationPlaybackManager;
      set => this.impl_.AnimationPlaybackManager = value;
    }

    public IAnimation? Animation {
      get => this.impl_.Animation;
      set => this.impl_.Animation = value;
    }

    public ISkeletonRenderer? SkeletonRenderer => this.impl_.SkeletonRenderer;
  }
}