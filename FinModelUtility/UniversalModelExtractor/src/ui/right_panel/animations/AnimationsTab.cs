
using fin.animation.playback;


namespace uni.ui.right_panel {
  public partial class AnimationsTab : UserControl {
    public AnimationsTab() {
      InitializeComponent();
    }

    public IAnimationPlaybackManager AnimationPlaybackManager =>
        this.animationPlaybackPanel_;
  }
}
