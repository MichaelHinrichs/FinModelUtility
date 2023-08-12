using fin.math;
using fin.model;
using fin.shaders.glsl;


namespace fin.ui.rendering.gl.material {
  public class GlNullMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial?> {
    public GlNullMaterialShaderV2(
        IModel model,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, null, boneTransformManager, lighting) { }

    protected override void DisposeInternal() { }

    protected override IShaderSourceGlsl GenerateShaderSource(
        IModel model,
        IReadOnlyMaterial? material)
      => new NullShaderSourceGlsl(model, true);

    protected override void Setup(IReadOnlyMaterial? material,
                                  GlShaderProgram shaderProgram) { }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) { }
  }
}