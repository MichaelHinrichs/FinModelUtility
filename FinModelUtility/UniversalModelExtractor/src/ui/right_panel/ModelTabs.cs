using fin.animation.playback;
using fin.model;


namespace uni.ui.right_panel {
  public partial class ModelTabs : UserControl {
    public ModelTabs() {
      InitializeComponent();
    }

    public IModel Model { get; set; }

    public IAnimationPlaybackManager AnimationPlaybackManager =>
        this.animationsTab_.AnimationPlaybackManager;
  }
}
