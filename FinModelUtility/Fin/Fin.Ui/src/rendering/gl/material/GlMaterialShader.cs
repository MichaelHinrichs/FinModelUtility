using fin.math;
using fin.model;

namespace fin.ui.rendering.gl.material {
  public static class GlMaterialShader {
    public static IGlMaterialShader FromMaterial(
        IModel model,
        IMaterial? material,
        IBoneTransformManager? boneTransformManager = null,
        ILighting? lighting = null) {
      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        return new GlFixedFunctionMaterialShader(
            model,
            fixedFunctionMaterial,
            boneTransformManager,
            lighting);
      }

      if (material is IStandardMaterial standardMaterial) {
        return new GlStandardMaterialShader(model,
                                              standardMaterial,
                                              boneTransformManager,
                                              lighting);
      }

      if (material != null) {
        return new GlSimpleMaterialShader(model,
                                            material,
                                            boneTransformManager,
                                            lighting);
      }

      return new GlNullMaterialShader(model, boneTransformManager, lighting);
    }
  }
}