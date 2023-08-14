using fin.animation.playback;
using fin.ui.rendering.gl.model;
using fin.io.bundles;
using fin.model;
using fin.scene;


namespace uni.ui.common.scene {
  public interface ISceneViewerPanel {
    (IFileBundle, IScene)? FileBundleAndScene { get; set; }
    ISceneModel? FirstSceneModel { get; }
    IAnimationPlaybackManager? AnimationPlaybackManager { get; }
    IModelAnimation? Animation { get; set; }
    ISkeletonRenderer? SkeletonRenderer { get; }

    TimeSpan FrameTime { get; }
  }
}