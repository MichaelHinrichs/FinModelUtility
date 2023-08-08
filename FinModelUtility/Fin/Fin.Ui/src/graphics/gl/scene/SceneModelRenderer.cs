using System.Numerics;

using fin.config;
using fin.math.matrix;
using fin.model;
using fin.scene;
using fin.ui;
using fin.ui.graphics.gl.material;
using fin.ui.graphics.gl.model;

namespace fin.ui.graphics.gl.scene {
  public class SceneModelRenderer : IRenderable, IDisposable {
    private readonly ISceneModel sceneModel_;
    private readonly IModelRenderer modelRenderer_;
    private readonly IReadOnlyList<SceneModelRenderer> children_;

    public SceneModelRenderer(ISceneModel sceneModel) {
      this.sceneModel_ = sceneModel;

      var model = sceneModel.Model;
      this.modelRenderer_ =
          new ModelRendererV2(model,
                              sceneModel.BoneTransformManager);

      var hasNormals = false;
      foreach (var vertex in model.Skin.Vertices) {
        if (vertex is IReadOnlyNormalVertex { LocalNormal: { } }) {
          hasNormals = true;
          break;
        }
      }

      this.modelRenderer_.UseLighting = hasNormals;

      this.SkeletonRenderer =
          new SkeletonRenderer(model.Skeleton,
                               this.sceneModel_.BoneTransformManager) {
              Scale = this.sceneModel_.ViewerScale
          };

      this.children_ = sceneModel.Children
                                 .Select(child => new SceneModelRenderer(child))
                                 .ToArray();
    }

    ~SceneModelRenderer() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.modelRenderer_.Dispose();
      foreach (var child in this.children_) {
        child.Dispose();
      }
    }

    public ISkeletonRenderer SkeletonRenderer { get; }

    public void Render() {
      GlTransform.PushMatrix();

      var model = this.sceneModel_.Model;
      var skeleton = model.Skeleton;

      var rootBone = skeleton.Root;
      if (rootBone.FaceTowardsCamera) {
        var camera = Camera.Instance;
        var angle = camera.YawDegrees / 180f * MathF.PI;
        var rotateYaw =
            Quaternion.CreateFromYawPitchRoll(angle, 0, 0);

        var rotationBuffer = rotateYaw * rootBone.FaceTowardsCameraAdjustment;
        GlTransform.MultMatrix(SystemMatrixUtil.FromRotation(rotationBuffer));
      }

      var animation = this.sceneModel_.Animation;
      var animationPlaybackManager = this.sceneModel_.AnimationPlaybackManager;
      if (animation != null) {
        animationPlaybackManager.Tick();

        var frame = (float) animationPlaybackManager.Frame;
        this.sceneModel_.BoneTransformManager.CalculateMatrices(
            skeleton.Root,
            model.Skin.BoneWeights,
            (animation, frame),
            animationPlaybackManager.ShouldLoop);

        var hiddenMeshes = this.modelRenderer_.HiddenMeshes;

        hiddenMeshes.Clear();
        var defaultDisplayState = MeshDisplayState.VISIBLE;
        foreach (var (mesh, meshTracks) in animation.MeshTracks) {
          if (!meshTracks.DisplayStates.TryGetInterpolatedFrame(
                  frame,
                  out var displayState)) {
            displayState = defaultDisplayState;
          }

          if (displayState == MeshDisplayState.HIDDEN) {
            hiddenMeshes.Add(mesh);
          }
        }
      }

      this.modelRenderer_.Render();

      if (FinConfig.ShowSkeleton) {
        CommonShaderPrograms.TEXTURELESS_SHADER_PROGRAM.Use();
        this.SkeletonRenderer.Render();
      }

      foreach (var child in this.children_) {
        child.Render();
      }

      GlTransform.PopMatrix();
    }
  }
}