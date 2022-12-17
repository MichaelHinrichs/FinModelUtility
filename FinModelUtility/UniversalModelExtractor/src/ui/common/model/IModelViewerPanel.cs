using fin.animation.playback;
using fin.io.bundles;
using fin.model;


namespace uni.ui.common.model {
  public interface IModelViewerPanel {
    public (IFileBundle, IModel)? FileBundleAndModel { get; set; }
    public IAnimationPlaybackManager AnimationPlaybackManager { get; set; }
    public IAnimation? Animation { get; set; }
  }
}