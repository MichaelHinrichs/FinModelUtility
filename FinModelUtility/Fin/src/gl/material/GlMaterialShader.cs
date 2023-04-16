using fin.model;

namespace fin.gl.material {
  public static class GlMaterialShader {
    public static IGlMaterialShader FromMaterial(
        IMaterial? material,
        ILighting? lighting = null) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new GlFixedFunctionMaterialShaderV2(
            fixedFunctionMaterial,
            lighting);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new GlStandardMaterialShaderV2(standardMaterial, lighting);
      }

      if (material != null) {
        return new GlSimpleMaterialShaderV2(material, lighting);
      }

      return new GlNullMaterialShaderV2(lighting);
    }
  }
}