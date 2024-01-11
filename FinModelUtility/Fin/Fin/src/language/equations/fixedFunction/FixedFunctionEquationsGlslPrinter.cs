using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using fin.model;
using fin.shaders.glsl;
using fin.util.asserts;

namespace fin.language.equations.fixedFunction {
  public class FixedFunctionEquationsGlslPrinter {
    public string Print(IReadOnlyFixedFunctionMaterial material) {
      var sb = new StringBuilder();

      using var os = new StringWriter(sb);
      this.Print(os, material);

      return sb.ToString();
    }

    public void Print(
        StringWriter os,
        IReadOnlyFixedFunctionMaterial material) {
      var equations = material.Equations;
      var registers = material.Registers;
      var textures = material.TextureSources;

      os.WriteLine("#version 400");
      os.WriteLine();

      var hasIndividualLights =
          Enumerable
              .Range(0, MaterialConstants.MAX_LIGHTS)
              .Select(
                  i => equations.DoOutputsDependOn(
                      new[] {
                          FixedFunctionSource.LIGHT_DIFFUSE_COLOR_0 + i,
                          FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_0 + i,
                          FixedFunctionSource.LIGHT_SPECULAR_COLOR_0 + i,
                          FixedFunctionSource.LIGHT_SPECULAR_ALPHA_0 + i
                      }))
              .ToArray();
      var dependsOnAnIndividualLight =
          hasIndividualLights.Any(value => value);
      var dependsOnMergedLights =
          equations.DoOutputsDependOn(
              new[] {
                  FixedFunctionSource.LIGHT_DIFFUSE_COLOR_MERGED,
                  FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_MERGED,
                  FixedFunctionSource.LIGHT_SPECULAR_COLOR_MERGED,
                  FixedFunctionSource.LIGHT_SPECULAR_ALPHA_MERGED,
              });
      var dependsOnLights = dependsOnMergedLights || dependsOnAnIndividualLight;

      var dependsOnAmbientLight = equations.DoOutputsDependOn(
          new[] {
              FixedFunctionSource.LIGHT_AMBIENT_COLOR,
              FixedFunctionSource.LIGHT_AMBIENT_ALPHA
          });

      // TODO: Optimize this if we only need ambient
      if (dependsOnLights || dependsOnAmbientLight) {
        os.WriteLine(GlslUtil.GetLightHeader(dependsOnAmbientLight));
        os.WriteLine($"uniform float {GlslConstants.UNIFORM_SHININESS_NAME};");
      }

      var dependsOnIndividualTextures =
          Enumerable
              .Range(0, MaterialConstants.MAX_TEXTURES)
              .Select(
                  i => equations
                      .DoOutputsDependOn(
                          new[] {
                              FixedFunctionSource.TEXTURE_COLOR_0 + i,
                              FixedFunctionSource.TEXTURE_ALPHA_0 + i
                          }))
              .ToArray();
      var dependsOnAnyTextures =
          dependsOnIndividualTextures.Any(value => value);

      if (dependsOnIndividualTextures
          .Select((dependsOnTexture, i) => (i, dependsOnTexture))
          .Where(tuple => tuple.dependsOnTexture)
          .Any(tuple => GlslUtil.RequiresFancyTextureData(
                   textures[tuple.i]))) {
        os.WriteLine(GlslUtil.GetTextureStruct());
      }

      var hadUniform = false;
      for (var t = 0; t < MaterialConstants.MAX_TEXTURES; ++t) {
        if (dependsOnIndividualTextures[t]) {
          hadUniform = true;
          os.WriteLine(
              $"uniform {GlslUtil.GetTypeOfTexture(textures[t])} texture{t};");
        }
      }

      foreach (var colorRegister in registers.ColorRegisters) {
        if (equations.DoOutputsDependOn(colorRegister)) {
          hadUniform = true;
          os.WriteLine($"uniform vec3 color_{colorRegister.Name};");
        }
      }

      foreach (var scalarRegister in registers.ScalarRegisters) {
        if (equations.DoOutputsDependOn(scalarRegister)) {
          hadUniform = true;
          os.WriteLine($"uniform float scalar_{scalarRegister.Name};");
        }
      }

      var hasWrittenLineBetweenUniformsAndIns = false;

      Action writeLineBetweenUniformsAndIns = () => {
        if (hadUniform && !hasWrittenLineBetweenUniformsAndIns) {
          hasWrittenLineBetweenUniformsAndIns = true;
          os.WriteLine();
        }
      };

      if (dependsOnAnyTextures && textures.Any(
              texture => texture?.UvType is UvType.SPHERICAL
                                            or UvType.LINEAR)) {
        writeLineBetweenUniformsAndIns();
        os.WriteLine("in vec2 normalUv;");
      }

      if (dependsOnLights) {
        writeLineBetweenUniformsAndIns();
        os.WriteLine("in vec3 vertexPosition;");
        os.WriteLine("in vec3 vertexNormal;");
      }

      for (var i = 0; i < MaterialConstants.MAX_COLORS; ++i) {
        if (equations.DoOutputsDependOn(new[] {
                FixedFunctionSource.VERTEX_COLOR_0 + i,
                FixedFunctionSource.VERTEX_ALPHA_0 + i
            })) {
          writeLineBetweenUniformsAndIns();
          os.WriteLine($"in vec4 vertexColor{i};");
        }
      }

      for (var i = 0; i < MaterialConstants.MAX_UVS; ++i) {
        if (textures.Any(texture => texture?.UvIndex == i)) {
          writeLineBetweenUniformsAndIns();
          os.WriteLine($"in vec2 uv{i};");
        }
      }

      os.WriteLine();
      os.WriteLine("out vec4 fragColor;");
      os.WriteLine();

      if (dependsOnLights) {
        os.WriteLine(
            $"""

             {GlslUtil.GetGetIndividualLightColorsFunction()}

             """);

        if (dependsOnMergedLights) {
          os.WriteLine($"""

                        {GlslUtil.GetGetMergedLightColorsFunction()}

                        """);
        }
      }

      os.WriteLine("void main() {");

      // Calculate lighting
      if (dependsOnLights) {
        os.WriteLine(
            $"""
               // Have to renormalize because the vertex normals can become distorted when interpolated.
               vec3 fragNormal = normalize(vertexNormal);

             """);
        // TODO: Optimize this if the shader depends on merged lighting as well as individual lights for some reason.
        if (dependsOnAnIndividualLight) {
          os.WriteLine(
              $$"""
                 vec4 individualLightDiffuseColors[{{MaterialConstants.MAX_LIGHTS}}];
                 vec4 individualLightSpecularColors[{{MaterialConstants.MAX_LIGHTS}}];
                 
                 for (int i = 0; i < {{MaterialConstants.MAX_LIGHTS}}; ++i) {
                   vec4 diffuseLightColor = vec4(0);
                   vec4 specularLightColor = vec4(0);
                   
                   getIndividualLightColors(lights[i], vertexPosition, fragNormal, {{GlslConstants.UNIFORM_SHININESS_NAME}}, diffuseLightColor, specularLightColor);
                   
                   individualLightDiffuseColors[i] = diffuseLightColor;
                   individualLightSpecularColors[i] = specularLightColor;
                 }
                 
               """);
        }
        if (dependsOnMergedLights) {
          os.WriteLine(
              $"""
                vec4 mergedLightDiffuseColor = vec4(0);
                vec4 mergedLightSpecularColor = vec4(0);
                getMergedLightColors(vertexPosition, fragNormal, {GlslConstants.UNIFORM_SHININESS_NAME}, mergedLightDiffuseColor, mergedLightSpecularColor);
                
              """);
        }
      }

      // TODO: Get tree of all values that this depends on, in case there needs to be other variables defined before.
      var outputColor =
          equations.ColorOutputs[FixedFunctionSource.OUTPUT_COLOR];

      os.Write("  vec3 colorComponent = ");
      this.PrintColorValue_(os, outputColor.ColorValue, textures);
      os.WriteLine(";");
      os.WriteLine();

      var outputAlpha =
          equations.ScalarOutputs[FixedFunctionSource.OUTPUT_ALPHA];

      os.Write("  float alphaComponent = ");
      this.PrintScalarValue_(os, outputAlpha.ScalarValue, textures);
      os.WriteLine(";");
      os.WriteLine();

      os.WriteLine("  fragColor = vec4(colorComponent, alphaComponent);");

      var alphaOpValue =
          this.DetermineAlphaOpValue_(
              material.AlphaOp,
              this.DetermineAlphaCompareType_(
                  material.AlphaCompareType0,
                  material.AlphaReference0),
              this.DetermineAlphaCompareType_(
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
              nameof(alphaCompareType),
              alphaCompareType,
              null)
      };

    private enum AlphaOpValue {
      ONLY_0_REQUIRED,
      ONLY_1_REQUIRED,
      BOTH_REQUIRED,
      ALWAYS_TRUE,
      ALWAYS_FALSE,
    }

    private AlphaOpValue DetermineAlphaOpValue_(
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

    private AlphaCompareValue DetermineAlphaCompareType_(
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
        IReadOnlyList<ITexture> textures,
        bool wrapExpressions = false) {
      if (value is IScalarExpression expression) {
        if (wrapExpressions) {
          os.Write("(");
        }

        this.PrintScalarExpression_(os, expression, textures);
        if (wrapExpressions) {
          os.Write(")");
        }
      } else if (value is IScalarTerm term) {
        this.PrintScalarTerm_(os, term, textures);
      } else if (value is IScalarFactor factor) {
        this.PrintScalarFactor_(os, factor, textures);
      } else {
        Asserts.Fail("Unsupported value type!");
      }
    }

    private void PrintScalarExpression_(
        StringWriter os,
        IScalarExpression expression,
        IReadOnlyList<ITexture> textures) {
      var terms = expression.Terms;

      for (var i = 0; i < terms.Count; ++i) {
        var term = terms[i];

        if (i > 0) {
          os.Write(" + ");
        }

        this.PrintScalarValue_(os, term, textures);
      }
    }

    private void PrintScalarTerm_(
        StringWriter os,
        IScalarTerm scalarTerm,
        IReadOnlyList<ITexture> textures) {
      var numerators = scalarTerm.NumeratorFactors;
      var denominators = scalarTerm.DenominatorFactors;

      if (numerators.Count > 0) {
        for (var i = 0; i < numerators.Count; ++i) {
          var numerator = numerators[i];

          if (i > 0) {
            os.Write("*");
          }

          this.PrintScalarValue_(os, numerator, textures, true);
        }
      } else {
        os.Write(1);
      }

      if (denominators != null) {
        for (var i = 0; i < denominators.Count; ++i) {
          var denominator = denominators[i];

          os.Write("/");

          this.PrintScalarValue_(os, denominator, textures, true);
        }
      }
    }

    private void PrintScalarFactor_(
        StringWriter os,
        IScalarFactor factor,
        IReadOnlyList<ITexture> textures) {
      if (factor is IScalarIdentifiedValue<FixedFunctionSource>
          identifiedValue) {
        this.PrintScalarIdentifiedValue_(os, identifiedValue, textures);
      } else if (factor is IScalarNamedValue namedValue) {
        this.PrintScalarNamedValue_(os, namedValue);
      } else if (factor is IScalarConstant constant) {
        this.PrintScalarConstant_(os, constant);
      } else if
          (factor is IColorNamedValueSwizzle<FixedFunctionSource>
           namedSwizzle) {
        this.PrintColorNamedValueSwizzle_(os, namedSwizzle, textures);
      } else if (factor is IColorValueSwizzle swizzle) {
        this.PrintColorValueSwizzle_(os, swizzle, textures);
      } else {
        Asserts.Fail("Unsupported factor type!");
      }
    }

    private void PrintScalarNamedValue_(
        StringWriter os,
        IScalarNamedValue namedValue)
      => os.Write($"scalar_{namedValue.Name}");

    private void PrintScalarIdentifiedValue_(
        StringWriter os,
        IScalarIdentifiedValue<FixedFunctionSource> identifiedValue,
        IReadOnlyList<ITexture> textures)
      => os.Write(this.GetScalarIdentifiedValue_(identifiedValue, textures));

    private string GetScalarIdentifiedValue_(
        IScalarIdentifiedValue<FixedFunctionSource> identifiedValue,
        IReadOnlyList<ITexture> textures) {
      var id = identifiedValue.Identifier;
      var isTextureAlpha = id is >= FixedFunctionSource.TEXTURE_ALPHA_0
                                 and <= FixedFunctionSource.TEXTURE_ALPHA_7;

      if (isTextureAlpha) {
        var textureIndex =
            (int) id - (int) FixedFunctionSource.TEXTURE_ALPHA_0;

        var textureText = this.GetTextureValue_(textureIndex, textures);
        var textureValueText = $"{textureText}.a";

        return textureValueText;
      }

      if (IsInRange_(id,
                     FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_0,
                     FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_7,
                     out var globalDiffuseAlphaIndex)) {
        return $"individualLightDiffuseColors[{globalDiffuseAlphaIndex}].a";
      }

      if (IsInRange_(id,
                     FixedFunctionSource.LIGHT_SPECULAR_ALPHA_0,
                     FixedFunctionSource.LIGHT_SPECULAR_ALPHA_7,
                     out var globalSpecularAlphaIndex)) {
        return $"individualLightSpecularColors[{globalSpecularAlphaIndex}].a";
      }

      return identifiedValue.Identifier switch {
          FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_MERGED => "mergedLightDiffuseColor.a",
          FixedFunctionSource.LIGHT_SPECULAR_ALPHA_MERGED => "mergedLightSpecularColor.a",

          FixedFunctionSource.LIGHT_AMBIENT_ALPHA => "ambientLightColor.a",

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

    private enum WrapType {
      NEVER,
      EXPRESSIONS,
      ALWAYS
    }

    private void PrintColorValue_(
        StringWriter os,
        IColorValue value,
        IReadOnlyList<ITexture> textures,
        WrapType wrapType = WrapType.NEVER) {
      var clamp = value.Clamp;

      if (clamp) {
        os.Write("clamp(");
      }

      if (value is IColorExpression expression) {
        var wrapExpressions =
            wrapType is WrapType.EXPRESSIONS or WrapType.ALWAYS;
        if (wrapExpressions) {
          os.Write("(");
        }

        this.PrintColorExpression_(os, expression, textures);
        if (wrapExpressions) {
          os.Write(")");
        }
      } else if (value is IColorTerm term) {
        var wrapTerms = wrapType == WrapType.ALWAYS;
        if (wrapTerms) {
          os.Write("(");
        }

        this.PrintColorTerm_(os, term, textures);
        if (wrapTerms) {
          os.Write(")");
        }
      } else if (value is IColorFactor factor) {
        var wrapFactors = wrapType == WrapType.ALWAYS;
        if (wrapFactors) {
          os.Write("(");
        }

        this.PrintColorFactor_(os, factor, textures);
        if (wrapFactors) {
          os.Write(")");
        }
      } else if (value is IColorValueTernaryOperator ternaryOperator) {
        this.PrintColorTernaryOperator_(os, ternaryOperator, textures);
      } else {
        Asserts.Fail("Unsupported value type!");
      }

      if (clamp) {
        os.Write(", 0, 1)");
      }
    }

    private void PrintColorExpression_(
        StringWriter os,
        IColorExpression expression,
        IReadOnlyList<ITexture> textures) {
      var terms = expression.Terms;

      for (var i = 0; i < terms.Count; ++i) {
        var term = terms[i];

        if (i > 0) {
          os.Write(" + ");
        }

        this.PrintColorValue_(os, term, textures);
      }
    }

    private void PrintColorTerm_(
        StringWriter os,
        IColorTerm scalarTerm,
        IReadOnlyList<ITexture> textures) {
      var numerators = scalarTerm.NumeratorFactors;
      var denominators = scalarTerm.DenominatorFactors;

      if (numerators.Count > 0) {
        for (var i = 0; i < numerators.Count; ++i) {
          var numerator = numerators[i];

          if (i > 0) {
            os.Write("*");
          }

          this.PrintColorValue_(os, numerator, textures, WrapType.EXPRESSIONS);
        }
      } else {
        os.Write(1);
      }

      if (denominators != null) {
        for (var i = 0; i < denominators.Count; ++i) {
          var denominator = denominators[i];

          os.Write("/");

          this.PrintColorValue_(os,
                                denominator,
                                textures,
                                WrapType.EXPRESSIONS);
        }
      }
    }

    private void PrintColorFactor_(
        StringWriter os,
        IColorFactor factor,
        IReadOnlyList<ITexture> textures) {
      if (factor is IColorIdentifiedValue<FixedFunctionSource>
          identifiedValue) {
        this.PrintColorIdentifiedValue_(os, identifiedValue, textures);
      } else if (factor is IColorNamedValue namedValue) {
        this.PrintColorNamedValue_(os, namedValue);
      } else {
        var useIntensity = factor.Intensity != null;

        if (!useIntensity) {
          var r = factor.R;
          var g = factor.G;
          var b = factor.B;

          os.Write("vec3(");
          this.PrintScalarValue_(os, r, textures);
          os.Write(",");
          this.PrintScalarValue_(os, g, textures);
          os.Write(",");
          this.PrintScalarValue_(os, b, textures);
          os.Write(")");
        } else {
          os.Write("vec3(");
          this.PrintScalarValue_(os, factor.Intensity!, textures);
          os.Write(")");
        }
      }
    }

    private void PrintColorIdentifiedValue_(
        StringWriter os,
        IColorIdentifiedValue<FixedFunctionSource> identifiedValue,
        IReadOnlyList<ITexture> textures)
      => os.Write(this.GetColorNamedValue_(identifiedValue, textures));

    private void PrintColorNamedValue_(
        StringWriter os,
        IColorNamedValue namedValue)
      => os.Write($"color_{namedValue.Name}");

    private string GetColorNamedValue_(
        IColorIdentifiedValue<FixedFunctionSource> identifiedValue,
        IReadOnlyList<ITexture> textures) {
      var id = identifiedValue.Identifier;
      var isTextureColor = id is >= FixedFunctionSource.TEXTURE_COLOR_0
                                 and <= FixedFunctionSource.TEXTURE_COLOR_7;
      var isTextureAlpha = id is >= FixedFunctionSource.TEXTURE_ALPHA_0
                                 and <= FixedFunctionSource.TEXTURE_ALPHA_7;

      if (isTextureColor || isTextureAlpha) {
        var textureIndex =
            isTextureColor
                ? (int) id - (int) FixedFunctionSource.TEXTURE_COLOR_0
                : (int) id - (int) FixedFunctionSource.TEXTURE_ALPHA_0;

        var textureText = this.GetTextureValue_(textureIndex, textures);
        var textureValueText = isTextureColor
            ? $"{textureText}.rgb"
            : $"vec3({textureText}.a)";

        return textureValueText;
      }

      if (IsInRange_(id,
                     FixedFunctionSource.LIGHT_DIFFUSE_COLOR_0,
                     FixedFunctionSource.LIGHT_DIFFUSE_COLOR_7,
                     out var globalDiffuseColorIndex)) {
        return $"individualLightDiffuseColors[{globalDiffuseColorIndex}].rgb";
      }

      if (IsInRange_(id,
                     FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_0,
                     FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_7,
                     out var globalDiffuseAlphaIndex)) {
        return $"individualLightDiffuseColors[{globalDiffuseAlphaIndex}].aaa";
      }

      if (IsInRange_(id,
                     FixedFunctionSource.LIGHT_SPECULAR_COLOR_0,
                     FixedFunctionSource.LIGHT_SPECULAR_COLOR_7,
                     out var globalSpecularColorIndex)) {
        return $"individualLightSpecularColors[{globalSpecularColorIndex}].rgb";
      }

      if (IsInRange_(id,
                     FixedFunctionSource.LIGHT_SPECULAR_ALPHA_0,
                     FixedFunctionSource.LIGHT_SPECULAR_ALPHA_7,
                     out var globalSpecularAlphaIndex)) {
        return $"individualLightSpecularColors[{globalSpecularAlphaIndex}].aaa";
      }

      return identifiedValue.Identifier switch {
          FixedFunctionSource.LIGHT_DIFFUSE_COLOR_MERGED => "mergedLightDiffuseColor.rgb",
          FixedFunctionSource.LIGHT_DIFFUSE_ALPHA_MERGED => "mergedLightDiffuseColor.aaa",
          FixedFunctionSource.LIGHT_SPECULAR_COLOR_MERGED => "mergedLightSpecularColor.rgb",
          FixedFunctionSource.LIGHT_SPECULAR_ALPHA_MERGED => "mergedLightSpecularColor.aaa",

          FixedFunctionSource.LIGHT_AMBIENT_COLOR => "ambientLightColor.rgb",
          FixedFunctionSource.LIGHT_AMBIENT_ALPHA => "ambientLightColor.aaa",

          FixedFunctionSource.VERTEX_COLOR_0 => "vertexColor0.rgb",
          FixedFunctionSource.VERTEX_COLOR_1 => "vertexColor1.rgb",

          FixedFunctionSource.VERTEX_ALPHA_0 => "vertexColor0.aaa",
          FixedFunctionSource.VERTEX_ALPHA_1 => "vertexColor1.aaa",

          FixedFunctionSource.UNDEFINED => "vec3(1)",
          _ => throw new ArgumentOutOfRangeException()
      };
    }

    private bool IsInRange_(FixedFunctionSource value,
                            FixedFunctionSource min,
                            FixedFunctionSource max,
                            out int relative) {
      relative = value - min;
      return value >= min && value <= max;
    }

    private string GetTextureValue_(int textureIndex,
                                    IReadOnlyList<ITexture> textures) {
      var texture = textures[textureIndex];
      var textureName = $"texture{textureIndex}";
      return texture.UvType switch {
          UvType.STANDARD
              => GlslUtil.ReadColorFromTexture(textureName,
                                               $"uv{texture.UvIndex}",
                                               texture),
          UvType.SPHERICAL
              => GlslUtil.ReadColorFromTexture(
                  textureName,
                  "normalUv",
                  uv => $"asin({uv}) / 3.14159 + 0.5",
                  texture),
          UvType.LINEAR
              => GlslUtil.ReadColorFromTexture(
                  textureName,
                  "normalUv",
                  uv => $"acos({uv}) / 3.14159",
                  texture),
      };
    }

    private void PrintColorTernaryOperator_(
        StringWriter os,
        IColorValueTernaryOperator ternaryOperator,
        IReadOnlyList<ITexture> textures) {
      os.Write('(');
      switch (ternaryOperator.ComparisonType) {
        case BoolComparisonType.EQUAL_TO: {
          os.Write("abs(");
          this.PrintScalarValue_(os, ternaryOperator.Lhs, textures);
          os.Write(" - ");
          this.PrintScalarValue_(os, ternaryOperator.Rhs, textures);
          os.Write(")");
          os.Write(" < ");
          os.Write("(1.0 / 255)");
          break;
        }
        case BoolComparisonType.GREATER_THAN: {
          this.PrintScalarValue_(os, ternaryOperator.Lhs, textures);
          os.Write(" > ");
          this.PrintScalarValue_(os, ternaryOperator.Rhs, textures);
          break;
        }
        default:
          throw new ArgumentOutOfRangeException(
              nameof(ternaryOperator.ComparisonType));
      }

      os.Write(" ? ");
      this.PrintColorValue_(os, ternaryOperator.TrueValue, textures);
      os.Write(" : ");
      this.PrintColorValue_(os, ternaryOperator.FalseValue, textures);
      os.Write(')');
    }

    private void PrintColorNamedValueSwizzle_(
        StringWriter os,
        IColorNamedValueSwizzle<FixedFunctionSource> swizzle,
        IReadOnlyList<ITexture> textures) {
      this.PrintColorIdentifiedValue_(os, swizzle.Source, textures);
      os.Write(".");
      os.Write(swizzle.SwizzleType switch {
          ColorSwizzle.R => 'r',
          ColorSwizzle.G => 'g',
          ColorSwizzle.B => 'b',
      });
    }

    private void PrintColorValueSwizzle_(
        StringWriter os,
        IColorValueSwizzle swizzle,
        IReadOnlyList<ITexture> textures) {
      this.PrintColorValue_(os, swizzle.Source, textures, WrapType.ALWAYS);
      os.Write(".");
      os.Write(swizzle.SwizzleType);
    }
  }
}