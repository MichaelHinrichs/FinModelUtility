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

    public string Print(
        IFixedFunctionEquations<FixedFunctionSource> equations) {
      var sb = new StringBuilder();

      using var os = new StringWriter(sb);
      this.Print(os, equations);

      return sb.ToString();
    }

    public void Print(
        StringWriter os,
        IFixedFunctionEquations<FixedFunctionSource> equations) {
      os.WriteLine("# version 130");
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
      os.WriteLine("in vec3 vertexNormal;");
      os.WriteLine("in vec4 vertexColor0_;");
      os.WriteLine("in vec4 vertexColor1_;");
      for (var i = 0; i < 4; ++i) {
        os.WriteLine($"in vec2 uv{i};");
      }
      os.WriteLine();
      os.WriteLine("out vec4 fragColor;");
      os.WriteLine();
      os.WriteLine("void main() {");

      // TODO: Define inputs once as needed up here.
      os.WriteLine("vec3 diffuseLightNormal = normalize(vec3(.5, .5, -1));");
      os.WriteLine(
          "float diffuseLightAmount = max(-dot(vertexNormal, diffuseLightNormal), 0);");
      os.WriteLine("vec3 diffuseLightColor = vec3(.5, .5, .5);");
      os.WriteLine(
          "vec3 diffuseColor = diffuseLightAmount * diffuseLightColor;");
      os.WriteLine("vec4 vertexColor0 = vec4(diffuseColor, 1);");
      os.WriteLine();
      os.WriteLine("vec3 ambientLightColor = vec3(0, 0, 0);");
      os.WriteLine("vec4 vertexColor1 = vec4(ambientLightColor, 1);");
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

      os.Write("  fragColor = vec4(colorComponent, alphaComponent);");
      os.WriteLine();

      /*os.WriteLine(@"
  if (fragColor.a < .95) {
    discard;
  }");*/

      os.WriteLine("}");
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
          FixedFunctionSource.UNDEFINED => "vec3(1)",
          _ => throw new ArgumentOutOfRangeException()
      };
    }

    private string GetTextureValue_(int textureIndex) {
      var texture = this.textures_[textureIndex];

      // TODO: Handle special UV types
      var uvText = $"uv{texture.UvIndex}";

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