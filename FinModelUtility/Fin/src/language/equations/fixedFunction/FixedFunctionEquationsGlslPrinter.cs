using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using fin.model;
using fin.util.asserts;


namespace fin.language.equations.fixedFunction {
  public class FixedFunctionEquationsGlslPrinter {
    private readonly IReadOnlyList<ITexture?> textures_;

    public FixedFunctionEquationsGlslPrinter(IReadOnlyList<ITexture> textures) {
      this.textures_ = textures;
    }

    public string Print(IFixedFunctionMaterial material) {
      var sb = new StringBuilder();

      using var os = new StringWriter(sb);
      this.Print(os, material);

      return sb.ToString();
    }

    public void Print(
        StringWriter os,
        IFixedFunctionMaterial material) {
      var equations = material.Equations;

      os.WriteLine("# version 330");
      os.WriteLine();
      for (var t = 0; t < 8; ++t) {
        if (equations.ColorInputs.ContainsKey(
                FixedFunctionSource.TEXTURE_COLOR_0 + t) ||
            equations.ColorInputs.ContainsKey(
                FixedFunctionSource.TEXTURE_ALPHA_0 + t) ||
            equations.ScalarInputs.ContainsKey(
                FixedFunctionSource.TEXTURE_ALPHA_0 + t)) {
          os.WriteLine($"uniform sampler2D texture{t};");
        }
      }
      os.WriteLine();
      os.WriteLine("in vec2 normalUv;");
      os.WriteLine("in vec3 vertexNormal;");
      os.WriteLine("in vec4 vertexColor0;");
      os.WriteLine("in vec4 vertexColor1;");
      for (var i = 0; i < 4; ++i) {
        os.WriteLine($"in vec2 uv{i};");
      }
      os.WriteLine();
      os.WriteLine("out vec4 fragColor;");
      os.WriteLine();
      os.WriteLine("void main() {");

      // TODO: Define inputs once as needed up here.
      os.WriteLine(@"  vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));
  float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);

  float ambientLightAmount = .3;
  
  float lightAmount = min(ambientLightAmount + diffuseLightAmount, 1);
  vec3 lightColor = vec3(.5, .5, .5);
  
  vec4 diffuseLightingColor = vec4(lightAmount * lightColor, 1);");
      os.WriteLine();
      os.WriteLine("  vec4 ambientLightingColor = vec4(0, 0, 0, 1);");
      os.WriteLine();

      // TODO: Get tree of all values that this depends on, in case there needs to be other variables defined before.
      var outputColor =
          equations.ColorOutputs[FixedFunctionSource.OUTPUT_COLOR];

      os.Write("  vec3 colorComponent = ");
      this.PrintColorValue_(os, outputColor.ColorValue);
      os.WriteLine(";");
      os.WriteLine();

      var outputAlpha =
          equations.ScalarOutputs[FixedFunctionSource.OUTPUT_ALPHA];

      os.Write("  float alphaComponent = ");
      this.PrintScalarValue_(os, outputAlpha.ScalarValue);
      os.WriteLine(";");
      os.WriteLine();

      os.WriteLine("  fragColor = vec4(colorComponent, alphaComponent);");

      var alphaOpValue =
          DetermineAlphaOpValue(
              material.AlphaOp,
              DetermineAlphaCompareType(
                  material.AlphaCompareType0,
                  material.AlphaReference0),
              DetermineAlphaCompareType(
                  material.AlphaCompareType1,
                  material.AlphaReference1));

      if (alphaOpValue != AlphaOpValue.ALWAYS_TRUE) {
        os.WriteLine();

        var alphaCompareText0 =
            GetAlphaCompareText_(material.AlphaCompareType0,
                                 material.AlphaReference0);
        var alphaCompareText1 =
            GetAlphaCompareText_(material.AlphaCompareType1,
                                 material.AlphaReference1);

        switch (alphaOpValue) {
          case AlphaOpValue.ONLY_0_REQUIRED: {
            os.WriteLine($@"  if (!({alphaCompareText0})) {{
    discard;
  }}");
            break;
          }
          case AlphaOpValue.ONLY_1_REQUIRED: {
            os.WriteLine($@"  if (!({alphaCompareText1})) {{
    discard;
  }}");
            break;
          }
          case AlphaOpValue.BOTH_REQUIRED: {
            switch (material.AlphaOp) {
              case AlphaOp.And: {
                os.Write(
                    $"  if (!({alphaCompareText0} && {alphaCompareText1})");
                break;
              }
              case AlphaOp.Or: {
                os.Write(
                    $"  if (!({alphaCompareText0} || {alphaCompareText1})");
                break;
              }
              case AlphaOp.XOR: {
                os.WriteLine($"  bool a = {alphaCompareText0};");
                os.WriteLine($"  bool b = {alphaCompareText1};");
                os.Write(
                    $"  if (!(any(bvec2(all(bvec2(!a, b)), all(bvec2(a, !b)))))");
                break;
              }
              case AlphaOp.XNOR: {
                os.WriteLine($"  bool a = {alphaCompareText0};");
                os.WriteLine($"  bool b = {alphaCompareText1};");
                os.Write(
                    "  if (!(any(bvec2(all(bvec2(!a, !b)), all(bvec2(a, b)))))");
                break;
              }
              default: throw new ArgumentOutOfRangeException();
            }
            os.WriteLine(@") {
    discard;
  }");
            break;
          }
          case AlphaOpValue.ALWAYS_FALSE: {
            os.WriteLine("  discard;");
            break;
          }
          default: throw new ArgumentOutOfRangeException();
        }
      }

      os.WriteLine("}");
    }

    private string GetAlphaCompareText_(
        AlphaCompareType alphaCompareType,
        float reference)
      => alphaCompareType switch {
          AlphaCompareType.Never   => "false",
          AlphaCompareType.Less    => $"fragColor.a < {reference}",
          AlphaCompareType.Equal   => $"fragColor.a == {reference}",
          AlphaCompareType.LEqual  => $"fragColor.a <= {reference}",
          AlphaCompareType.Greater => $"fragColor.a > {reference}",
          AlphaCompareType.NEqual  => $"fragColor.a != {reference}",
          AlphaCompareType.GEqual  => $"fragColor.a >= {reference}",
          AlphaCompareType.Always  => "true",
          _ => throw new ArgumentOutOfRangeException(
                   nameof(alphaCompareType), alphaCompareType, null)
      };

    private enum AlphaOpValue {
      ONLY_0_REQUIRED,
      ONLY_1_REQUIRED,
      BOTH_REQUIRED,
      ALWAYS_TRUE,
      ALWAYS_FALSE,
    }

    private AlphaOpValue DetermineAlphaOpValue(
        AlphaOp alphaOp,
        AlphaCompareValue compareValue0,
        AlphaCompareValue compareValue1) {
      var is0False = compareValue0 == AlphaCompareValue.ALWAYS_FALSE;
      var is0True = compareValue0 == AlphaCompareValue.ALWAYS_TRUE;
      var is1False = compareValue1 == AlphaCompareValue.ALWAYS_FALSE;
      var is1True = compareValue1 == AlphaCompareValue.ALWAYS_TRUE;

      if (alphaOp == AlphaOp.And) {
        if (is0False || is1False) {
          return AlphaOpValue.ALWAYS_FALSE;
        }

        if (is0True && is1True) {
          return AlphaOpValue.ALWAYS_TRUE;
        }
        if (is0True) {
          return AlphaOpValue.ONLY_1_REQUIRED;
        }
        if (is1True) {
          return AlphaOpValue.ONLY_0_REQUIRED;
        }
        return AlphaOpValue.BOTH_REQUIRED;
      }

      if (alphaOp == AlphaOp.Or) {
        if (is0True || is1True) {
          return AlphaOpValue.ALWAYS_TRUE;
        }

        if (is0False && is1False) {
          return AlphaOpValue.ALWAYS_FALSE;
        }
        if (is0False) {
          return AlphaOpValue.ONLY_1_REQUIRED;
        }
        if (is1False) {
          return AlphaOpValue.ONLY_0_REQUIRED;
        }
        return AlphaOpValue.BOTH_REQUIRED;
      }

      return AlphaOpValue.BOTH_REQUIRED;
    }

    private enum AlphaCompareValue {
      INDETERMINATE,
      ALWAYS_TRUE,
      ALWAYS_FALSE,
    }

    private AlphaCompareValue DetermineAlphaCompareType(
        AlphaCompareType compareType,
        float reference) {
      var isReference0 = Math.Abs(reference - 0) < .001;
      var isReference1 = Math.Abs(reference - 1) < .001;

      if (compareType == AlphaCompareType.Always ||
          (compareType == AlphaCompareType.GEqual && isReference0) ||
          (compareType == AlphaCompareType.LEqual && isReference1)) {
        return AlphaCompareValue.ALWAYS_TRUE;
      }

      if (compareType == AlphaCompareType.Never ||
          (compareType == AlphaCompareType.Greater && isReference1) ||
          (compareType == AlphaCompareType.Less && isReference0)) {
        return AlphaCompareValue.ALWAYS_FALSE;
      }

      return AlphaCompareValue.INDETERMINATE;
    }

    private void PrintScalarValue_(
        StringWriter os,
        IScalarValue value,
        bool wrapExpressions = false) {
      if (value is IScalarExpression expression) {
        if (wrapExpressions) {
          os.Write("(");
        }
        this.PrintScalarExpression_(os, expression);
        if (wrapExpressions) {
          os.Write(")");
        }
      } else if (value is IScalarTerm term) {
        this.PrintScalarTerm_(os, term);
      } else if (value is IScalarFactor factor) {
        this.PrintScalarFactor_(os, factor);
      } else {
        Asserts.Fail("Unsupported value type!");
      }
    }

    private void PrintScalarExpression_(
        StringWriter os,
        IScalarExpression expression) {
      var terms = expression.Terms;

      for (var i = 0; i < terms.Count; ++i) {
        var term = terms[i];

        if (i > 0) {
          os.Write(" + ");
        }
        this.PrintScalarValue_(os, term);
      }
    }

    private void PrintScalarTerm_(
        StringWriter os,
        IScalarTerm scalarTerm) {
      var numerators = scalarTerm.NumeratorFactors;
      var denominators = scalarTerm.DenominatorFactors;

      if (numerators.Count > 0) {
        for (var i = 0; i < numerators.Count; ++i) {
          var numerator = numerators[i];

          if (i > 0) {
            os.Write("*");
          }

          this.PrintScalarValue_(os, numerator, true);
        }
      } else {
        os.Write(1);
      }

      if (denominators != null) {
        for (var i = 0; i < denominators.Count; ++i) {
          var denominator = denominators[i];

          os.Write("/");

          this.PrintScalarValue_(os, denominator, true);
        }
      }
    }

    private void PrintScalarFactor_(
        StringWriter os,
        IScalarFactor factor) {
      if (factor is IScalarNamedValue<FixedFunctionSource> namedValue) {
        this.PrintScalarNamedValue_(os, namedValue);
      } else if (factor is IScalarConstant constant) {
        this.PrintScalarConstant_(os, constant);
      } else if
          (factor is IColorNamedValueSwizzle<FixedFunctionSource> swizzle) {
        this.PrintColorNamedValueSwizzle_(os, swizzle);
      } else {
        Asserts.Fail("Unsupported factor type!");
      }
    }

    private void PrintScalarNamedValue_(
        StringWriter os,
        IScalarNamedValue<FixedFunctionSource> namedValue)
      => os.Write(this.GetScalarNamedValue_(namedValue));

    private string GetScalarNamedValue_(
        IScalarNamedValue<FixedFunctionSource> namedValue) {
      var id = namedValue.Identifier;
      var isTextureAlpha = id is >= FixedFunctionSource.TEXTURE_ALPHA_0
                                 and <= FixedFunctionSource.TEXTURE_ALPHA_7;

      if (isTextureAlpha) {
        var textureIndex =
            (int) id - (int) FixedFunctionSource.TEXTURE_ALPHA_0;

        var textureText = this.GetTextureValue_(textureIndex);
        var textureValueText = $"{textureText}.a";

        return textureValueText;
      }

      return namedValue.Identifier switch {
          FixedFunctionSource.VERTEX_ALPHA_0 => "vertexColor0.a",
          FixedFunctionSource.VERTEX_ALPHA_1 => "vertexColor1.a",

          FixedFunctionSource.DIFFUSE_LIGHTING_ALPHA
              => "diffuseLightingColor.a",
          FixedFunctionSource.AMBIENT_LIGHTING_ALPHA
              => "ambientLightingColor.a",

          FixedFunctionSource.UNDEFINED => "1",
          _ => throw new ArgumentOutOfRangeException()
      };
    }

    private void PrintScalarConstant_(
        StringWriter os,
        IScalarConstant constant)
      => os.Write(constant.Value);


    private void PrintColorValue_(
        StringWriter os,
        IColorValue value,
        bool wrapExpressions = false) {
      var clamp = value.Clamp;

      if (clamp) {
        os.Write("clamp(");
      }

      if (value is IColorExpression expression) {
        if (wrapExpressions) {
          os.Write("(");
        }
        this.PrintColorExpression_(os, expression);
        if (wrapExpressions) {
          os.Write(")");
        }
      } else if (value is IColorTerm term) {
        this.PrintColorTerm_(os, term);
      } else if (value is IColorFactor factor) {
        this.PrintColorFactor_(os, factor);
      } else {
        Asserts.Fail("Unsupported value type!");
      }

      if (clamp) {
        os.Write(", 0, 1)");
      }
    }

    private void PrintColorExpression_(
        StringWriter os,
        IColorExpression expression) {
      var terms = expression.Terms;

      for (var i = 0; i < terms.Count; ++i) {
        var term = terms[i];

        if (i > 0) {
          os.Write(" + ");
        }
        this.PrintColorValue_(os, term);
      }
    }

    private void PrintColorTerm_(
        StringWriter os,
        IColorTerm scalarTerm) {
      var numerators = scalarTerm.NumeratorFactors;
      var denominators = scalarTerm.DenominatorFactors;

      if (numerators.Count > 0) {
        for (var i = 0; i < numerators.Count; ++i) {
          var numerator = numerators[i];

          if (i > 0) {
            os.Write("*");
          }

          this.PrintColorValue_(os, numerator, true);
        }
      } else {
        os.Write(1);
      }

      if (denominators != null) {
        for (var i = 0; i < denominators.Count; ++i) {
          var denominator = denominators[i];

          os.Write("/");

          this.PrintColorValue_(os, denominator, true);
        }
      }
    }

    private void PrintColorFactor_(
        StringWriter os,
        IColorFactor factor) {
      if (factor is IColorNamedValue<FixedFunctionSource> namedValue) {
        this.PrintColorNamedValue_(os, namedValue);
      } else {
        var useIntensity = factor.Intensity != null;

        IScalarValue r;
        IScalarValue g;
        IScalarValue b;

        if (!useIntensity) {
          r = factor.R;
          g = factor.G;
          b = factor.B;
        } else {
          r = g = b = factor.Intensity!;
        }

        os.Write("vec3(");
        this.PrintScalarValue_(os, r);
        os.Write(",");
        this.PrintScalarValue_(os, g);
        os.Write(",");
        this.PrintScalarValue_(os, b);
        os.Write(")");
      }
    }

    private void PrintColorNamedValue_(
        StringWriter os,
        IColorNamedValue<FixedFunctionSource> namedValue)
      => os.Write(this.GetColorNamedValue_(namedValue));

    private string GetColorNamedValue_(
        IColorNamedValue<FixedFunctionSource> namedValue) {
      var id = namedValue.Identifier;
      var isTextureColor = id is >= FixedFunctionSource.TEXTURE_COLOR_0
                                 and <= FixedFunctionSource.TEXTURE_COLOR_7;
      var isTextureAlpha = id is >= FixedFunctionSource.TEXTURE_ALPHA_0
                                 and <= FixedFunctionSource.TEXTURE_ALPHA_7;

      if (isTextureColor || isTextureAlpha) {
        var textureIndex =
            isTextureColor
                ? (int) id - (int) FixedFunctionSource.TEXTURE_COLOR_0
                : (int) id - (int) FixedFunctionSource.TEXTURE_ALPHA_0;

        var textureText = this.GetTextureValue_(textureIndex);
        var textureValueText = isTextureColor
                                   ? $"{textureText}.rgb"
                                   : $"vec3({textureText}.a)";

        return textureValueText;
      }

      return namedValue.Identifier switch {
          FixedFunctionSource.VERTEX_COLOR_0 => "vertexColor0.rgb",
          FixedFunctionSource.VERTEX_COLOR_1 => "vertexColor1.rgb",

          FixedFunctionSource.VERTEX_ALPHA_0 => "vertexColor0.aaa",
          FixedFunctionSource.VERTEX_ALPHA_1 => "vertexColor1.aaa",

          FixedFunctionSource.DIFFUSE_LIGHTING_COLOR
              => "diffuseLightingColor.rgb",
          FixedFunctionSource.DIFFUSE_LIGHTING_ALPHA
              => "diffuseLightingColor.aaa",

          FixedFunctionSource.AMBIENT_LIGHTING_COLOR
              => "ambientLightingColor.rgb",
          FixedFunctionSource.AMBIENT_LIGHTING_ALPHA
              => "ambientLightingColor.aaa",

          FixedFunctionSource.UNDEFINED => "vec3(1)",
          _ => throw new ArgumentOutOfRangeException()
      };
    }

    private string GetTextureValue_(int textureIndex) {
      var texture = this.textures_[textureIndex];

      var uvText = texture?.UvType switch {
          UvType.NORMAL    => $"uv{texture.UvIndex}",
          UvType.SPHERICAL => "asin(normalUv) / 3.14159 + 0.5",
          UvType.LINEAR    => "acos(normalUv) / 3.14159",
          _                => throw new ArgumentOutOfRangeException()
      };

      var textureText = $"texture(texture{textureIndex}, {uvText})";
      return textureText;
    }

    private void PrintColorNamedValueSwizzle_(
        StringWriter os,
        IColorNamedValueSwizzle<FixedFunctionSource> swizzle) {
      this.PrintColorNamedValue_(os, swizzle.Source);
      os.Write(".");
      os.Write(swizzle.SwizzleType switch {
          ColorSwizzle.R => 'r',
          ColorSwizzle.G => 'g',
          ColorSwizzle.B => 'b',
      });
    }
  }
}