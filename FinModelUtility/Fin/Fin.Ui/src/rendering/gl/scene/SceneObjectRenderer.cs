using fin.math.matrix;
using fin.scene;

namespace fin.ui.rendering.gl.scene {
  public class SceneObjectRenderer : IRenderable, IDisposable {
    private readonly ISceneObject sceneObject_;

    public SceneObjectRenderer(ISceneObject sceneObject) {
      this.sceneObject_ = sceneObject;
      this.ModelRenderers
          = sceneObject
            .Models
            .Select(model => new SceneModelRenderer(model))
            .ToArray();
    }

    ~SceneObjectRenderer() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var modelRenderer in this.ModelRenderers) {
        modelRenderer.Dispose();
      }
    }

    public IReadOnlyList<SceneModelRenderer> ModelRenderers { get; }

    public void Render() {
      GlTransform.PushMatrix();

      GlTransform.MultMatrix(
          SystemMatrixUtil.FromTrs(this.sceneObject_.Position,
                                   this.sceneObject_.Rotation,
                                   this.sceneObject_.Scale));

      foreach (var model in this.ModelRenderers) {
        model.Render();
      }

      GlTransform.PopMatrix();
    }
  }
}