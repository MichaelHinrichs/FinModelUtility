using fin.model;

namespace fin.shaders.glsl {
  public class NullShaderSourceGlsl : IShaderSourceGlsl {
    public NullShaderSourceGlsl(IModel model, bool useBoneMatrices) {
      this.VertexShaderSource = GlslUtil.GetVertexSrc(model, useBoneMatrices);
    }

    public string VertexShaderSource { get; }

    public string FragmentShaderSource
      => """
         # version 400

         out vec4 fragColor;

         in vec4 vertexColor0;

         void main() {
           fragColor = vertexColor0;
         }
         """;
  }
}