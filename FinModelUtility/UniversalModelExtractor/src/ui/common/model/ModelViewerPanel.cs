using fin.animation.playback;
using fin.model;
using uni.ui.gl;


namespace uni.ui.common.model {
  public partial class ModelViewerPanel : UserControl, IModelViewerPanel {
    public ModelViewerPanel() {
      this.InitializeComponent();
    }

    public (IModelFileBundle, IModel)? ModelAndFileBundle {
      get => this.impl_.ModelAndFileBundle;
      set {
        var modelFileBundle = value?.Item1;
        if (modelFileBundle != null) {
          this.groupBox_.Text = modelFileBundle.FullPath;
        } else {
          this.groupBox_.Text = "(Select a model)";
        }

        this.impl_.ModelAndFileBundle = value;
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

    public SkeletonRenderer? SkeletonRenderer => this.impl_.SkeletonRenderer;
  }
}