using fin.language.equations.fixedFunction;
using fin.model;

namespace fin.shaders.glsl {
  public class FixedFunctionShaderSourceGlsl : IShaderSourceGlsl {
    public FixedFunctionShaderSourceGlsl(
        IModel model,
        IFixedFunctionMaterial material,
        bool useBoneMatrices) {
      this.VertexShaderSource = GlslUtil.GetVertexSrc(model, useBoneMatrices);
      this.FragmentShaderSource =
          new FixedFunctionEquationsGlslPrinter(material.TextureSources)
              .Print(material);
    }

    public string VertexShaderSource { get; }
    public string FragmentShaderSource { get; }
  }
}
