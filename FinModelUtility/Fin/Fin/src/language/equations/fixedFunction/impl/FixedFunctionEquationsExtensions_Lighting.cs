using fin.model;

namespace fin.language.equations.fixedFunction {
  public static partial class FixedFunctionEquationsExtensions {
    public static IColorValue GetMergedLightDiffuseColor(
        this IFixedFunctionEquations<FixedFunctionSource> equations) {
      IColorValue? merged = null;

      for (var i = 0; i < MaterialConstants.MAX_LIGHTS; ++i) {
        var lightSrc = FixedFunctionSource.LIGHT_DIFFUSE_COLOR_0 + i;
        var newInput = equations.CreateOrGetColorInput(lightSrc);

        if (merged == null) {
          merged = newInput;
        } else {
          merged = merged.Add(newInput);
        }
      }

      return merged!;
    }

    public static IScalarValue GetMergedLightDiffuseAlpha(
        this IFixedFunctionEquations<FixedFunctionSource> equations) {
      IScalarValue? merged = null;

      for (var i = 0; i < MaterialConstants.MAX_LIGHTS; ++i) {
        var lightSrc = FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_0 + i;
        var newInput = equations.CreateOrGetScalarInput(lightSrc);

        if (merged == null) {
          merged = newInput;
        } else {
          merged = merged.Add(newInput);
        }
      }

      return merged!;
    }


    public static IColorValue GetMergedLightSpecularColor(
        this IFixedFunctionEquations<FixedFunctionSource> equations) {
      IColorValue? merged = null;

      for (var i = 0; i < MaterialConstants.MAX_LIGHTS; ++i) {
        var lightSrc = FixedFunctionSource.LIGHT_SPECULAR_COLOR_0 + i;
        var newInput = equations.CreateOrGetColorInput(lightSrc);

        if (merged == null) {
          merged = newInput;
        } else {
          merged = merged.Add(newInput);
        }
      }

      return merged!;
    }

    public static IScalarValue GetMergedLightSpecularAlpha(
        this IFixedFunctionEquations<FixedFunctionSource> equations) {
      IScalarValue? merged = null;

      for (var i = 0; i < MaterialConstants.MAX_LIGHTS; ++i) {
        var lightSrc = FixedFunctionSource.LIGHT_SPECULAR_ALPHA_0 + i;
        var newInput = equations.CreateOrGetScalarInput(lightSrc);

        if (merged == null) {
          merged = newInput;
        } else {
          merged = merged.Add(newInput);
        }
      }

      return merged!;
    }
  }
}