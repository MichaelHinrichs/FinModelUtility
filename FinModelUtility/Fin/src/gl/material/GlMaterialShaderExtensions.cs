using fin.model;

namespace fin.gl.material {
  public static class GlMaterialShaderExtensions {
    public static IGlMaterialShaderSource ToShaderSource(
        this IReadOnlyMaterial material) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new GlFixedFunctionMaterialShaderSource(fixedFunctionMaterial);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new GlStandardMaterialShaderSource(standardMaterial);
      }

      return new GlSimpleMaterialShaderSource();
    }
  }
}