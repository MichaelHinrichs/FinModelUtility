using fin.model;

namespace fin.shaders.glsl {
  public static class GlslMaterialExtensions {
    public static IShaderSourceGlsl ToShaderSource(
        this IReadOnlyMaterial material,
        IModel model, 
        bool useBoneMatrices) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new FixedFunctionShaderSourceGlsl(
            model,
            fixedFunctionMaterial,
            useBoneMatrices);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new StandardShaderSourceGlsl(
            model,
            standardMaterial,
            useBoneMatrices);
      }

      return new SimpleShaderSourceGlsl(model, material, useBoneMatrices);
    }
  }
}