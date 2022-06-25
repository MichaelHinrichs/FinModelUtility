using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Linq;

using fin.model;

using bmd.GCN;

using fin.language.equations.fixedFunction;
using fin.util.asserts;

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
      var colorOne = equations.CreateColorConstant(1);
      var scTwo = equations.CreateScalarConstant(2);
      var scFour = equations.CreateScalarConstant(4);
      var scHalf = equations.CreateScalarConstant(.5);
      var scMinusOne = equations.CreateScalarConstant(-1);

      var isZero = (IColorValue? color) => color == null || color == colorZero;

      var addColorValues = (IColorValue? lhs, IColorValue? rhs) => {
        var lhsIsZero = isZero(lhs);
        var rhsIsZero = isZero(rhs);

        if (lhsIsZero && rhsIsZero) {
          return null;
        }
        if (lhsIsZero) {
          return rhs;
        }
        if (rhsIsZero) {
          return lhs;
        }

        return lhs!.Add(rhs!);
      };

      var subtractColorValues = (IColorValue? lhs, IColorValue? rhs) => {
        var lhsIsZero = isZero(lhs);
        var rhsIsZero = isZero(rhs);

        if ((lhsIsZero && rhsIsZero) || lhs == rhs) {
          return null;
        }
        if (lhsIsZero) {
          return rhs?.Multiply(scMinusOne);
        }
        if (rhsIsZero) {
          return lhs;
        }

        return lhs!.Subtract(rhs!);
      };

      var multiplyColorValues = (IColorValue? lhs, IColorValue? rhs) => {
        if (isZero(lhs) || isZero(rhs)) {
          return null;
        }

        var lhsIsOne = lhs == colorOne;
        var rhsIsOne = rhs == colorOne;

        if (lhsIsOne && rhsIsOne) {
          return colorOne;
        }
        if (lhsIsOne) {
          return rhs;
        }
        if (rhsIsOne) {
          return lhs;
        }

        return lhs!.Multiply(rhs!);
      };

      Func<IColorValue?, IScalarValue, IColorValue?> addColorAndScalar =
          (IColorValue? lhs, IScalarValue rhs) => {
            var lhsIsZero = isZero(lhs);

            if (lhsIsZero) {
              return equations.CreateColor(rhs);
            }

            return lhs!.Add(rhs);
          };

      Func<IColorValue?, IScalarValue, IColorValue?> subtractColorAndScalar =
          (IColorValue? lhs, IScalarValue rhs) => {
            var lhsIsZero = isZero(lhs);

            if (lhsIsZero) {
              return equations.CreateColor(rhs.Multiply(scMinusOne));
            }

            return lhs!.Subtract(rhs);
          };

      var colorManager = new ColorManager(equations);

      // TODO: Where are color constants set inside the materials?
      // TODO: Need to support registers
      // TODO: Need to support multiple vertex colors
      // TODO: Colors should just be RGB in the fixed function library
      // TODO: Seems like only texture 1 is used, is this accurate?

      colorManager.SetConstColors(bmd.MAT3.ColorS10);

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
          colorManager.UpdateTextureColor(null);
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

          colorManager.UpdateTextureColor(texCoordIndex);
          material.SetTextureSource(texCoordIndex, texture);
        }

        // Updates which color is referred to by RASC
        var colorChannel = tevOrder.ChannelID;
        colorManager.UpdateRascColor(colorChannel);

        var konstIndex = materialEntry.TevColorIndexes[tevStage.color_constant_sel];
        var konstColor = bmd.MAT3.ColorS10[konstIndex];
        colorManager.UpdateKonstColor(konstIndex, konstColor);
        colorConstants.Add(konstColor);

        // Set up color logic
        {
          var ccA = tevStage.color_a;
          var ccB = tevStage.color_b;
          var ccC = tevStage.color_c;
          var ccD = tevStage.color_d;

          var colorBias = tevStage.color_bias;
          var colorScale = tevStage.color_scale;

          var colorA = colorManager.GetColor(tevStage.color_a);
          var colorB = colorManager.GetColor(tevStage.color_b);
          var colorC = colorManager.GetColor(tevStage.color_c);
          var colorD = colorManager.GetColor(tevStage.color_d);

          IColorValue? colorValue = null;

          // TODO: Switch this to an enum
          var colorOp = tevStage.color_op;
          switch (colorOp) {
            // ADD: out = a*(1 - c) + b*c + d
            case TevOp.GX_TEV_ADD:
            case TevOp.GX_TEV_SUB: {
              colorValue = ccD != TevStage.GxCc.GX_CC_ZERO ? colorD : null;

              var aTimesOneMinusC = multiplyColorValues(
                  colorA, subtractColorValues(colorOne, colorC));

              var bTimesC = multiplyColorValues(colorC, colorB);

              var rest = addColorValues(aTimesOneMinusC, bTimesC);

              colorValue = colorOp == TevOp.GX_TEV_ADD
                               ? addColorValues(colorValue, rest)
                               : subtractColorValues(colorValue, rest);

              // TODO: Is this right?
              colorValue ??= colorZero;
              //Asserts.Nonnull(colorValue);

              switch (colorBias) {
                case TevStage.TevBias.GX_TB_ZERO: {
                  break;
                }
                case TevStage.TevBias.GX_TB_ADDHALF: {
                  colorValue = addColorAndScalar(colorValue, scHalf);
                  break;
                }
                case TevStage.TevBias.GX_TB_SUBHALF: {
                  colorValue = subtractColorAndScalar(colorValue, scHalf);
                  break;
                }
                default: {
                  Asserts.Fail("Unsupported color bias!");
                  break;
                }
              }

              switch (colorScale) {
                case TevStage.TevScale.GX_CS_SCALE_1: {
                  break;
                }
                case TevStage.TevScale.GX_CS_SCALE_2: {
                  colorValue = colorValue.Multiply(scTwo);
                  break;
                }
                case TevStage.TevScale.GX_CS_SCALE_4: {
                  colorValue = colorValue.Multiply(scFour);
                  break;
                }
                case TevStage.TevScale.GX_CS_DIVIDE_2: {
                  colorValue = colorValue.Divide(scTwo);
                  break;
                }
                default: {
                  Asserts.Fail("Unsupported color scale!");
                  break;
                }
              }

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

          colorManager.UpdateColorRegister(tevStage.color_regid, colorValue);

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

        // TODO: Implement alpha operations
      }

      equations.CreateColorOutput(
          FixedFunctionSource.OUTPUT_COLOR,
          colorManager.GetColor(TevStage.GxCc.GX_CC_CPREV));

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

    private class ColorManager {
      private readonly IColorValue colorUndefined_;

      private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;


      private readonly Dictionary<TevOrder.ColorChannel, IColorValue>
          colorChannelsColors_ = new();

      private readonly Dictionary<TevStage.GxCc, IColorValue>
          colorValues_ = new();

      private readonly Dictionary<TevStage.GxCc, IScalarValue>
          alphaValues_ = new();

      public ColorManager(
          IFixedFunctionEquations<FixedFunctionSource> equations) {
        this.equations_ = equations;

        var colorZero = equations.CreateColorConstant(0);
        var colorOne = equations.CreateColorConstant(1);

        this.colorValues_[TevStage.GxCc.GX_CC_ZERO] = colorZero;
        this.colorValues_[TevStage.GxCc.GX_CC_ONE] = colorOne;

        this.colorUndefined_ =
            equations.CreateColorInput(FixedFunctionSource.UNDEFINED,
                                       colorZero);
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
      private readonly IColorValue?[] constColors_ = new IColorValue?[3];

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
          var index = (int) colorSource - (int) TevStage.GxCc.GX_CC_C0;

          if (index % 2 == 0) {
            var ccIndex = index / 2;

            var constColor = this.constColors_[ccIndex];
            if (constColor == null) {
              var constColorImpl = this.constColorImpls_[ccIndex];

              constColor = this.equations_.CreateColorConstant(
                  constColorImpl.R / 255f,
                  constColorImpl.G / 255f,
                  constColorImpl.B / 255f);
              this.constColors_[ccIndex] = constColor;
              this.colorValues_.Add(colorSource, constColor);
            }

            //var constColor = this.colorRegisterColors_[ColorRegister.GX_TEVREG0 + ccIndex];
            return constColor;
          }

          // TODO: Handle alphas
        }

        if (!BmdFixedFunctionMaterial.STRICT) {
          return this.colorUndefined_;
        }

        throw new NotImplementedException();
      }
    }
  }
}