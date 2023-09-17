using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class GlColorMaterialShader : BGlMaterialShader<IColorMaterial> {
    private readonly IColorMaterial material_;

    public GlColorMaterialShader(
        IModel model,
        IColorMaterial colorMaterial,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, colorMaterial, boneTransformManager, lighting) {
      this.material_ = colorMaterial;
    }

    protected override void DisposeInternal() { }

    protected override void Setup(IColorMaterial material,
                                  GlShaderProgram shaderProgram) { }

    protected override void PassUniformsAndBindTextures(GlShaderProgram impl) {
      var diffuseTextureLocation = impl.GetUniformLocation("diffuseColor");
      GL.Uniform4(diffuseTextureLocation, this.material_.Color);
    }
  }
}