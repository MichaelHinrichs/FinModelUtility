using System.Numerics;

using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.ui.rendering.gl.material {
  public class GlColorMaterialShader : BGlMaterialShader<IColorMaterial> {
    private readonly IColorMaterial material_;
    private IShaderUniform<Vector4> diffuseLightColorUniform_;

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
                                  GlShaderProgram shaderProgram) {
      this.diffuseLightColorUniform_ =
          shaderProgram.GetUniformVec4("diffuseColor");
    }

    protected override void PassUniformsAndBindTextures(GlShaderProgram impl) {
      this.diffuseLightColorUniform_.SetAndMaybeMarkDirty(
          new Vector4(this.material_.Color.R / 255f,
                      this.material_.Color.G / 255f,
                      this.material_.Color.B / 255f,
                      this.material_.Color.A / 255f));
    }
  }
}