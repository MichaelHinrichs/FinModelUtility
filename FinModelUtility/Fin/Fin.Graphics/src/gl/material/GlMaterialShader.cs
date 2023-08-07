using fin.math;
using fin.model;


namespace fin.graphics.gl.material {
  public static class GlMaterialShader {
    public static IGlMaterialShader FromMaterial(
        IModel model,
        IMaterial? material,
        IBoneTransformManager? boneTransformManager = null,
        ILighting? lighting = null) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new GlFixedFunctionMaterialShaderV2(
            model,
            fixedFunctionMaterial,
            boneTransformManager,
            lighting);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new GlStandardMaterialShaderV2(model,
                                              standardMaterial,
                                              boneTransformManager,
                                              lighting);
      }

      if (material != null) {
        return new GlSimpleMaterialShaderV2(model,
                                            material,
                                            boneTransformManager,
                                            lighting);
      }

      return new GlNullMaterialShaderV2(model, boneTransformManager, lighting);
    }
  }
}