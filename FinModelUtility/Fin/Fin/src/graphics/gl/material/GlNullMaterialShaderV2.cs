using fin.math;
using fin.model;


namespace fin.graphics.gl.material {
  public class GlNullMaterialShaderSource : IGlMaterialShaderSource {
    public GlNullMaterialShaderSource(IModel model, bool useBoneMatrices) {
      this.VertexShaderSource =
          CommonShaderPrograms.GetVertexSrc(model, useBoneMatrices);
    }

    public string VertexShaderSource { get; }

    public string FragmentShaderSource => @"# version 130 

out vec4 fragColor;

in vec4 vertexColor0;

void main() {
  fragColor = vertexColor0;
}";
  }

  public class GlNullMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial?> {
    public GlNullMaterialShaderV2(
        IModel model,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, null, boneTransformManager, lighting) { }

    protected override void DisposeInternal() { }

    protected override IGlMaterialShaderSource GenerateShaderSource(
        IModel model,
        IReadOnlyMaterial? material)
      => new GlNullMaterialShaderSource(model, true);

    protected override void Setup(IReadOnlyMaterial? material,
                                  GlShaderProgram shaderProgram) { }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) { }
  }
}