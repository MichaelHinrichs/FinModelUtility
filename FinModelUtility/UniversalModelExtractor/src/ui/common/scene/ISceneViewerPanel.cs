using fin.animation.playback;
using fin.gl.model;
using fin.io.bundles;
using fin.model;
using fin.scene;


namespace uni.ui.common.scene {
  public interface ISceneViewerPanel {
    (IFileBundle, IScene)? FileBundleAndScene { get; set; }
    ISceneModel? FirstSceneModel { get; }
    IAnimationPlaybackManager? AnimationPlaybackManager { get; }
    IAnimation? Animation { get; set; }
    ISkeletonRenderer? SkeletonRenderer { get; }
  }
}