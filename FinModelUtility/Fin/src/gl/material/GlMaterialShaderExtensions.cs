using fin.model;

namespace fin.gl.material {
  public static class GlMaterialShaderExtensions {
    public static IGlMaterialShaderSource ToShaderSource(
        this IReadOnlyMaterial material,
        IModel model) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new GlFixedFunctionMaterialShaderSource(model, fixedFunctionMaterial);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new GlStandardMaterialShaderSource(model, standardMaterial);
      }

      return new GlSimpleMaterialShaderSource(model);
    }
  }
}