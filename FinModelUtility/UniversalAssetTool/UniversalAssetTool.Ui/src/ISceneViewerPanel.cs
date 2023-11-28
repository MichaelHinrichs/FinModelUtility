using fin.animation;
using fin.io.bundles;
using fin.model;
using fin.scene;
using fin.ui.rendering.gl.model;

namespace uni.ui {
  public interface ISceneViewerPanel {
    (IFileBundle, IScene, ILighting?)? FileBundleAndSceneAndLighting {
      get;
      set;
    }

    ISceneModel? FirstSceneModel { get; }
    IAnimationPlaybackManager? AnimationPlaybackManager { get; }
    IModelAnimation? Animation { get; set; }
    ISkeletonRenderer? SkeletonRenderer { get; }

    TimeSpan FrameTime { get; }
  }
}