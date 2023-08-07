using fin.model;

namespace fin.graphics.gl.material {
  public static class GlMaterialShaderExtensions {
    public static IGlMaterialShaderSource ToShaderSource(
        this IReadOnlyMaterial material,
        IModel model, 
        bool useBoneMatrices) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new GlFixedFunctionMaterialShaderSource(
            model,
            fixedFunctionMaterial,
            useBoneMatrices);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new GlStandardMaterialShaderSource(
            model,
            standardMaterial,
            useBoneMatrices);
      }

      return new GlSimpleMaterialShaderSource(model, material, useBoneMatrices);
    }
  }
}