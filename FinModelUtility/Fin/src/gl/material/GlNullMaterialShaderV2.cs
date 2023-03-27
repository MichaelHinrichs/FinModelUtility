using fin.model;

namespace fin.gl.material {
  public class GlNullMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial?> {
    public GlNullMaterialShaderV2() : base(null) { }

    protected override void DisposeInternal() { }

    protected override GlShaderProgram GenerateShaderProgram(
        IReadOnlyMaterial? _) => GlShaderProgram.FromShaders(
        CommonShaderPrograms.VERTEX_SRC,
        @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor0;

void main() {
    fragColor = vertexColor0;
}");

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) { }
  }
}