using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Linq;

using fin.model;

using bmd.GCN;

using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
using fin.util.asserts;

using Microsoft.Xna.Framework.Graphics.PackedVector;

using BlendFactor = fin.model.BlendFactor;
using LogicOp = fin.model.LogicOp;


namespace bmd.exporter {
  using static bmd.GCN.BMD.MAT3Section.TevStageProps;

  using TevOrder = BMD.MAT3Section.TevOrder;
  using TevStage = BMD.MAT3Section.TevStageProps;
  using TextureMatrixInfo = BMD.MAT3Section.TextureMatrixInfo;

  /// <summary>
  ///   BMD material, one of the common formats for the GameCube.
  ///
  ///   For more info:
  ///   http://www.amnoid.de/gc/tev.html
  /// </summary>
  public class BmdFixedFunctionMaterial {
    private const bool STRICT = false;

    public BmdFixedFunctionMaterial(
        IMaterialManager materialManager,
        int materialEntryIndex,
        BMD bmd,
        IList<BmdTexture> textures) {
      var materialEntry = bmd.MAT3.MaterialEntries[materialEntryIndex];
      var materialName = bmd.MAT3.StringTable[materialEntryIndex];

      var material = materialManager.AddFixedFunctionMaterial();
      material.Name = materialName;
      material.CullingMode =
          bmd.MAT3.CullModes[materialEntry.CullModeIndex] switch {
              BMD.CullMode.None  => CullingMode.SHOW_BOTH,
              BMD.CullMode.Front => CullingMode.SHOW_BACK_ONLY,
              BMD.CullMode.Back  => CullingMode.SHOW_FRONT_ONLY,
              BMD.CullMode.All   => CullingMode.SHOW_NEITHER,
              _                  => throw new ArgumentOutOfRangeException(),
          };

      this.Material = material;

      var colorConstants = new List<Color>();

      // TODO: Need to use material entry indices

      var equations = material.Equations;

      var colorZero = equations.CreateColorConstant(0);

      var scZero = equations.CreateScalarConstant(0);
      var scOne = equations.CreateScalarConstant(1);
      var scTwo = equations.CreateScalarConstant(2);
      var scFour = equations.CreateScalarConstant(4);
      var scHalf = equations.CreateScalarConstant(.5);
      var scMinusHalf = equations.CreateScalarConstant(-.5);

      var colorFixedFunctionOps = new ColorFixedFunctionOps(equations);
      var scalarFixedFunctionOps = new ScalarFixedFunctionOps(equations);

      var valueManager = new ValueManager(equations);

      // TODO: Where are color constants set inside the materials?
      // TODO: Need to support registers
      // TODO: Need to support multiple vertex colors
      // TODO: Colors should just be RGB in the fixed function library
      // TODO: Seems like only texture 1 is used, is this accurate?

      // TODO: This might need to be TevKonstColorIndexes
      valueManager.SetConstColors(
          materialEntry.TevColorIndexes.Select(
                           tevColorIndex => bmd.MAT3.ColorS10[tevColorIndex])
                       .ToArray());

      for (var i = 0; i < materialEntry.TevStageInfoIndexes.Length; ++i) {
        var tevStageIndex = materialEntry.TevStageInfoIndexes[i];
        if (tevStageIndex == -1) {
          continue;
        }

        var tevStage = bmd.MAT3.TevStages[tevStageIndex];

        var tevOrderIndex = materialEntry.TevOrderInfoIndexes[i];
        var tevOrder = bmd.MAT3.Tevorders[tevOrderIndex];

        // Updates which texture is referred to by TEXC
        var texStageIndex = tevOrder.TexMap;
        if (texStageIndex == -1) {
          valueManager.UpdateTextureColor(null);
        } else {
          var texStage = materialEntry.TextureIndexes[texStageIndex];
          var textureIndex = bmd.MAT3.TextureIndices[texStage];

          var bmdTexture = textures[textureIndex];

          // TODO: Share texture definitions between materials?
          var texture = materialManager.CreateTexture(bmdTexture.Image);

          texture.Name = bmdTexture.Name;
          texture.WrapModeU = bmdTexture.WrapModeS;
          texture.WrapModeV = bmdTexture.WrapModeT;
          texture.ColorType = bmdTexture.ColorType;

          var texCoordIndex = tevOrder.TexcoordID;
          texture.UvIndex = texCoordIndex;

          valueManager.UpdateTextureColor(texCoordIndex);
          material.SetTextureSource(texCoordIndex, texture);
        }

        // Updates which color is referred to by RASC
        var colorChannel = tevOrder.ChannelID;
        valueManager.UpdateRascColor(colorChannel);

        // TODO: This might need to be TevKonstColorIndexes
        var konstIndex =
            materialEntry.TevColorIndexes[tevStage.color_constant_sel];
        var konstColor = bmd.MAT3.ColorS10[konstIndex];
        valueManager.UpdateKonstColor(konstIndex, konstColor);
        colorConstants.Add(konstColor);

        // Set up color logic
        {
          var colorA = valueManager.GetColor(tevStage.color_a);
          var colorB = valueManager.GetColor(tevStage.color_b);
          var colorC = valueManager.GetColor(tevStage.color_c);
          var colorD = valueManager.GetColor(tevStage.color_d);

          IColorValue? colorValue = null;

          // TODO: Switch this to an enum
          var colorOp = tevStage.color_op;
          switch (colorOp) {
            // ADD: out = a*(1 - c) + b*c + d
            case TevOp.GX_TEV_ADD:
            case TevOp.GX_TEV_SUB: {
              var bias = tevStage.color_bias switch {
                  TevBias.GX_TB_ZERO    => null,
                  TevBias.GX_TB_ADDHALF => scHalf,
                  TevBias.GX_TB_SUBHALF => scMinusHalf,
                  _ => throw new ArgumentOutOfRangeException(
                           "Unsupported color bias!")
              };

              var scale = tevStage.color_scale switch {
                  TevScale.GX_CS_SCALE_1  => scOne,
                  TevScale.GX_CS_SCALE_2  => scTwo,
                  TevScale.GX_CS_SCALE_4  => scFour,
                  TevScale.GX_CS_DIVIDE_2 => scHalf,
                  _ => throw new ArgumentOutOfRangeException(
                           "Unsupported color scale!")
              };

              colorValue =
                  colorFixedFunctionOps.AddOrSubtractOp(
                      colorOp == TevOp.GX_TEV_ADD,
                      colorA,
                      colorB,
                      colorC,
                      colorD,
                      bias,
                      scale
                  );

              colorValue ??= colorZero;
              colorValue.Clamp = tevStage.color_clamp;

              break;
            }

            default: {
              if (BmdFixedFunctionMaterial.STRICT) {
                throw new NotImplementedException();
              } else {
                colorValue = colorZero;
              }
              break;
            }
          }

          valueManager.UpdateColorRegister(tevStage.color_regid, colorValue);

          var colorAText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(colorA);
          var colorBText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(colorB);
          var colorCText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(colorC);
          var colorDText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(colorD);

          var colorValueText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(colorValue);

          ;
        }

        // Set up alpha logic
        {
          var alphaA = valueManager.GetAlpha(tevStage.alpha_a);
          var alphaB = valueManager.GetAlpha(tevStage.alpha_b);
          var alphaC = valueManager.GetAlpha(tevStage.alpha_c);
          var alphaD = valueManager.GetAlpha(tevStage.alpha_d);

          IScalarValue? alphaValue = null;

          // TODO: Switch this to an enum
          var alphaOp = tevStage.alpha_op;
          switch (alphaOp) {
            // ADD: out = a*(1 - c) + b*c + d
            case TevOp.GX_TEV_ADD:
            case TevOp.GX_TEV_SUB: {
              var bias = tevStage.alpha_bias switch {
                  TevBias.GX_TB_ZERO    => null,
                  TevBias.GX_TB_ADDHALF => scHalf,
                  TevBias.GX_TB_SUBHALF => scMinusHalf,
                  _ => throw new ArgumentOutOfRangeException(
                           "Unsupported alpha bias!")
              };

              var scale = tevStage.alpha_scale switch {
                  TevScale.GX_CS_SCALE_1  => scOne,
                  TevScale.GX_CS_SCALE_2  => scTwo,
                  TevScale.GX_CS_SCALE_4  => scFour,
                  TevScale.GX_CS_DIVIDE_2 => scHalf,
                  _ => throw new ArgumentOutOfRangeException(
                           "Unsupported alpha scale!")
              };

              alphaValue =
                  scalarFixedFunctionOps.AddOrSubtractOp(
                      alphaOp == TevOp.GX_TEV_ADD,
                      alphaA,
                      alphaB,
                      alphaC,
                      alphaD,
                      bias,
                      scale
                  );

              alphaValue ??= scZero;
              //alphaValue.Clamp = tevStage.alpha_clamp;

              break;
            }

            default: {
              if (BmdFixedFunctionMaterial.STRICT) {
                throw new NotImplementedException();
              } else {
                alphaValue = scZero;
              }
              break;
            }
          }

          valueManager.UpdateAlphaRegister(tevStage.alpha_regid, alphaValue);

          var alphaAText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(alphaA);
          var alphaBText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(alphaB);
          var alphaCText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(alphaC);
          var alphaDText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(alphaD);

          var alphaValueText =
              new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
                  .Print(alphaValue);

          ;
        }
      }

      equations.CreateColorOutput(
          FixedFunctionSource.OUTPUT_COLOR,
          valueManager.GetColor(GxCc.GX_CC_CPREV));

      equations.CreateScalarOutput(
          FixedFunctionSource.OUTPUT_ALPHA,
          valueManager.GetAlpha(GxCc.GX_CC_APREV));

      // TODO: Set up compiled texture
      // TODO: If only a const color, create a texture for that


      var sb = new StringBuilder();

      {
        using var os = new StringWriter(sb);

        // TODO: Print textures, colors

        new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
            .Print(os, equations);
      }

      var output = sb.ToString();

      var colorTextureCount =
          material.Textures.Count(
              texture => texture.ColorType == ColorType.COLOR);

      // TODO: This is a bad assumption!
      if (colorTextureCount == 0 && colorConstants.Count > 0) {
        var colorConstant = colorConstants.Last();

        var intensityTexture = material.Textures
                                       .FirstOrDefault(
                                           texture => texture.ColorType ==
                                             ColorType.INTENSITY);
        if (intensityTexture != null) {
          return;
        }

        var colorBitmap = new Bitmap(1, 1);
        colorBitmap.SetPixel(0, 0, colorConstant);

        var colorTexture = materialManager.CreateTexture(colorBitmap);
        material.CompiledTexture = colorTexture;
      }
    }

    public IMaterial Material { get; }

    private class ValueManager {
      private readonly IColorValue colorUndefined_;
      private readonly IScalarValue alphaUndefined_;

      private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;


      private readonly Dictionary<TevOrder.ColorChannel, IColorValue>
          colorChannelsColors_ = new();

      private readonly Dictionary<TevStage.GxCc, IColorValue>
          colorValues_ = new();

      private readonly Dictionary<TevStage.GxCc, IScalarValue>
          alphaValues_ = new();

      public ValueManager(
          IFixedFunctionEquations<FixedFunctionSource> equations) {
        this.equations_ = equations;

        var colorZero = equations.CreateColorConstant(0);
        var colorOne = equations.CreateColorConstant(1);

        this.colorValues_[TevStage.GxCc.GX_CC_ZERO] = colorZero;
        this.colorValues_[TevStage.GxCc.GX_CC_ONE] = colorOne;

        this.colorUndefined_ =
            equations.CreateColorInput(FixedFunctionSource.UNDEFINED,
                                       colorZero);
        this.alphaUndefined_ =
            equations.CreateScalarInput(FixedFunctionSource.UNDEFINED,
                                        equations.CreateScalarConstant(0));
      }

      private readonly Dictionary<ColorRegister, IColorValue>
          colorRegisterColors_ = new();

      public void UpdateColorRegister(
          ColorRegister colorRegister,
          IColorValue colorValue) {
        var source = colorRegister switch {
            ColorRegister.GX_TEVPREV => TevStage.GxCc.GX_CC_CPREV,
            ColorRegister.GX_TEVREG0 => TevStage.GxCc.GX_CC_C0,
            ColorRegister.GX_TEVREG1 => TevStage.GxCc.GX_CC_C1,
            ColorRegister.GX_TEVREG2 => TevStage.GxCc.GX_CC_C2,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(colorRegister),
                     colorRegister,
                     null)
        };

        this.colorValues_[source] = colorValue;
      }

      public void UpdateAlphaRegister(
          ColorRegister alphaRegister,
          IScalarValue alphaValue) {
        var source = alphaRegister switch {
            ColorRegister.GX_TEVPREV => TevStage.GxCc.GX_CC_APREV,
            ColorRegister.GX_TEVREG0 => TevStage.GxCc.GX_CC_A0,
            ColorRegister.GX_TEVREG1 => TevStage.GxCc.GX_CC_A1,
            ColorRegister.GX_TEVREG2 => TevStage.GxCc.GX_CC_A2,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(alphaRegister),
                     alphaRegister,
                     null)
        };

        this.alphaValues_[source] = alphaValue;
      }

      /*public void UpdateTextureColor(
          IColorNamedValue<FixedFunctionSource>? colorValue) {
        if (colorValue != null) {
          this.colorValues_[TevStage.GxCc.GX_CC_TEXC] = colorValue;
        } else {
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_TEXC);
        }
      }*/

      private readonly IColorValue?[] textureColors_ = new IColorValue?[8];
      private readonly IColorValue?[] textureAlphas_ = new IColorValue?[8];
      private int? textureIndex_ = null;

      public void UpdateTextureColor(int? index) {
        if (this.textureIndex_ != index) {
          this.textureIndex_ = index;
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_TEXC);
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_TEXA);
        }

        if (index != null) {
          Asserts.True(index >= 0 && index < 8);
        }
      }

      private IColorValue GetTextureColorChannel_() {
        var indexOrNull = this.textureIndex_;
        Asserts.Nonnull(indexOrNull);

        var index = indexOrNull.Value;
        Asserts.True(index >= 0 && index < 8);

        var texture = this.textureColors_[index];
        if (texture == null) {
          this.textureColors_[index] =
              texture = this.equations_.CreateColorInput(
                  (FixedFunctionSource) (FixedFunctionSource.TEXTURE_COLOR_0 +
                                         index),
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[TevStage.GxCc.GX_CC_TEXC] = texture;
      }

      private IColorValue GetTextureAlphaChannel_() {
        var indexOrNull = this.textureIndex_;
        Asserts.Nonnull(indexOrNull);

        var index = indexOrNull.Value;
        Asserts.True(index >= 0 && index < 8);

        var texture = this.textureAlphas_[index];
        if (texture == null) {
          this.textureAlphas_[index] =
              texture = this.equations_.CreateColorInput(
                  (FixedFunctionSource) (FixedFunctionSource.TEXTURE_ALPHA_0 +
                                         index),
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[TevStage.GxCc.GX_CC_TEXA] = texture;
      }


      private TevOrder.ColorChannel? colorChannel_;

      public void UpdateRascColor(TevOrder.ColorChannel? colorChannel) {
        if (this.colorChannel_ != colorChannel) {
          this.colorChannel_ = colorChannel;
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_RASC);
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_RASA);
        }
      }

      private IColorValue GetVertexColorChannel_(TevStage.GxCc colorSource) {
        var channelOrNull = this.colorChannel_;
        Asserts.Nonnull(channelOrNull);

        var channel = channelOrNull.Value;

        if (!this.colorChannelsColors_.TryGetValue(channel, out var color)) {
          var source = colorSource switch {
              TevStage.GxCc.GX_CC_RASC => channel switch {
                  TevOrder.ColorChannel.GX_COLOR0A0 => FixedFunctionSource
                      .VERTEX_COLOR_0,
                  TevOrder.ColorChannel.GX_COLOR1A1 => FixedFunctionSource
                      .VERTEX_COLOR_1,
                  _ => throw new NotImplementedException()
              },
              TevStage.GxCc.GX_CC_RASA => channel switch {
                  TevOrder.ColorChannel.GX_COLOR0A0 => FixedFunctionSource
                      .VERTEX_ALPHA_0,
                  TevOrder.ColorChannel.GX_COLOR1A1 => FixedFunctionSource
                      .VERTEX_ALPHA_1,
                  _ => throw new NotImplementedException()
              },
              _ => throw new NotImplementedException()
          };

          this.colorChannelsColors_[channel] =
              color = this.equations_.CreateColorInput(
                  source,
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[TevStage.GxCc.GX_CC_RASC] = color;
      }

      private int? konstIndex_ = null;
      private Color? konstColor_ = null;

      private IList<Color> constColorImpls_;

      // TODO: Is 10 right?
      private readonly IColorValue?[] konstColors_ = new IColorValue?[16];

      public void SetConstColors(IList<Color> constColorImpls) {
        this.constColorImpls_ = constColorImpls;
      }

      public void UpdateKonstColor(int index, Color color) {
        if (index != this.konstIndex_) {
          this.konstIndex_ = index;
          this.konstColor_ = color;
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_KONST);
        }
      }

      public IColorValue GetKonstColorChannel_() {
        var indexOrNull = this.konstIndex_;
        Asserts.Nonnull(indexOrNull);

        var index = indexOrNull.Value;
        //Asserts.True(index >= 0 && index < 4);

        var colorOrNull = this.konstColor_;
        Asserts.Nonnull(colorOrNull);

        var color = colorOrNull.Value;

        var konstColor = this.konstColors_[index];
        if (konstColor == null) {
          this.konstColors_[index] =
              konstColor = this.equations_.CreateColorConstant(
                  color.R / 255f,
                  color.G / 255f,
                  color.B / 255f);
        }

        return this.colorValues_[TevStage.GxCc.GX_CC_KONST] = konstColor;
      }

      public IColorValue GetColor(TevStage.GxCc colorSource) {
        if (this.colorValues_.TryGetValue(colorSource, out var colorValue)) {
          return colorValue;
        }

        if (colorSource == TevStage.GxCc.GX_CC_TEXC) {
          return this.GetTextureColorChannel_();
        }

        if (colorSource == TevStage.GxCc.GX_CC_TEXA) {
          return this.GetTextureAlphaChannel_();
        }

        if (colorSource == TevStage.GxCc.GX_CC_RASC ||
            colorSource == TevStage.GxCc.GX_CC_RASA) {
          return this.GetVertexColorChannel_(colorSource);
        }

        if (colorSource == TevStage.GxCc.GX_CC_KONST) {
          return this.GetKonstColorChannel_();
        }

        if (colorSource >= TevStage.GxCc.GX_CC_C0 &&
            colorSource <= TevStage.GxCc.GX_CC_A2) {
          var (constColorImpl, isColor) = this.GetCCColor_(colorSource);

          var rByte = isColor
                          ? (constColorImpl?.R ?? 255)
                          : (constColorImpl?.A ?? 255);
          var gByte = isColor
                          ? (constColorImpl?.G ?? 255)
                          : (constColorImpl?.A ?? 255);
          var bByte = isColor
                          ? (constColorImpl?.B ?? 255)
                          : (constColorImpl?.A ?? 255);
          var constColor = this.equations_.CreateColorConstant(
              rByte / 255f, gByte / 255f, bByte / 255f);

          return constColor;
        }

        if (!BmdFixedFunctionMaterial.STRICT) {
          return this.colorUndefined_;
        }

        throw new NotImplementedException();
      }

      public IScalarValue GetAlpha(GxCc alphaSource) {
        if (this.alphaValues_.TryGetValue(alphaSource, out var alphaValue)) {
          return alphaValue;
        }

        // TODO: Is this right?
        if (this.colorValues_.TryGetValue(alphaSource, out var colorValue)) {
          return colorValue.R;
        }

        if (alphaSource >= TevStage.GxCc.GX_CC_C0 &&
            alphaSource <= TevStage.GxCc.GX_CC_A2) {
          var (constColorImpl, isColor) = this.GetCCColor_(alphaSource);

          var constColorByte = isColor
                                   ? (constColorImpl?.R ?? 255)
                                   : (constColorImpl?.A ?? 255);
          var constColor = this.equations_.CreateScalarConstant(
              constColorByte / 255f);

          this.alphaValues_.Add(alphaSource, constColor);
          return constColor;
        }

        if (!BmdFixedFunctionMaterial.STRICT) {
          return this.alphaUndefined_;
        }

        throw new NotImplementedException();
      }

      private (Color? color, bool isAlpha) GetCCColor_(GxCc source) {
        var ccIndex = (int) source - (int) TevStage.GxCc.GX_CC_C0;

        var isColor = ccIndex % 2 == 0;
        var index = isColor ? ccIndex / 2 : (ccIndex - 1) / 2;

        return (this.GetCCColor_(index), isColor);
      }

      private Color? GetCCColor_(int index) {
        if (this.constColorImpls_.Count > index) {
          return this.constColorImpls_[index];
        }
        return null;
      }
    }
  }
}