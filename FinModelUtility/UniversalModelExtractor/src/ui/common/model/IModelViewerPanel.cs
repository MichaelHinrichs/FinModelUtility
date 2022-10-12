using fin.animation.playback;
using fin.model;


namespace uni.ui.common.model {
  public interface IModelViewerPanel {
    public (IModelFileBundle, IModel)? ModelAndFileBundle { get; set; }
    public IAnimationPlaybackManager AnimationPlaybackManager { get; set; }
    public IAnimation? Animation { get; set; }
  }
}