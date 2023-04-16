using fin.model;

namespace fin.gl.material {
  public class GlNullMaterialShaderSource : IGlMaterialShaderSource {
    public string VertexShaderSource => CommonShaderPrograms.VERTEX_SRC;

    public string FragmentShaderSource => @"# version 130 

out vec4 fragColor;

in vec4 vertexColor0;

void main() {
  fragColor = vertexColor0;
}";
  }

  public class GlNullMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial?> {
    public GlNullMaterialShaderV2(ILighting? lighting) :
        base(null, lighting) { }

    protected override void DisposeInternal() { }

    protected override IGlMaterialShaderSource GenerateShaderSource(
        IReadOnlyMaterial? material)
      => new GlNullMaterialShaderSource();

    protected override void Setup(IReadOnlyMaterial? material,
                                  GlShaderProgram shaderProgram) { }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) { }
  }
}