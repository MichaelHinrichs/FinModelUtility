using fin.animation.playback;
using fin.model;


namespace uni.ui.right_panel {
  public partial class ModelTabs : UserControl {
    public ModelTabs() {
      InitializeComponent();
    }

    public IModel Model {
      set {
        this.animationsTab_.Model = value;
        this.texturesTab_.Model = value;
      }
    }

    public IAnimationPlaybackManager AnimationPlaybackManager =>
        this.animationsTab_.AnimationPlaybackManager;

    public event AnimationsTab.AnimationSelected OnAnimationSelected {
      add => this.animationsTab_.OnAnimationSelected += value;
      remove => this.animationsTab_.OnAnimationSelected -= value;
    }
  }
}
