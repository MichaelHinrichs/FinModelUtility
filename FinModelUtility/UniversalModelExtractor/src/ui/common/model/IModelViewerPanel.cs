using fin.animation.playback;
using fin.gl.model;
using fin.io.bundles;
using fin.model;
using fin.scene;


namespace uni.ui.common.model {
  public interface IModelViewerPanel {
    public (IFileBundle, IScene)? FileBundleAndScene { get; set; }
    public IAnimationPlaybackManager? AnimationPlaybackManager { get; }
    public IAnimation? Animation { get; set; }
    public ISkeletonRenderer? SkeletonRenderer { get; }
  }
}