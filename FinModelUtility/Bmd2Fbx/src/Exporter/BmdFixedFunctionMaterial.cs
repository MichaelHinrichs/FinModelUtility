using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using fin.model;

using mkds.gcn.bmd;

using bmd.GCN;

using fin.language.equations.fixedFunction;
using fin.util.asserts;

using BlendFactor = fin.model.BlendFactor;
using LogicOp = fin.model.LogicOp;

namespace bmd.exporter {
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
    public BmdFixedFunctionMaterial(
        IMaterialManager materialManager,
        int materialEntryIndex,
        BMD bmd,
        IList<BmdTexture> textures) {
      var materialEntry = bmd.MAT3.MaterialEntries[materialEntryIndex];
      var materialName = bmd.MAT3.StringTable[materialEntryIndex];

      var material = materialManager.AddFixedFunctionMaterial();
      material.Name = materialName;

      this.Material = material;

      // TODO: Need to use material entry indices

      var equations = material.Equations;

      var colorOne = equations.CreateColorConstant(1);
      var scTwo = equations.CreateScalarConstant(2);
      var scFour = equations.CreateScalarConstant(4);
      var scHalf = equations.CreateScalarConstant(.5);
      var scMinusHalf = equations.CreateScalarConstant(-.5);

      var colorManager = new ColorManager(equations);

      // TODO: Where are color constants set inside the materials?
      // TODO: Need to support registers
      // TODO: Need to support multiple vertex colors
      // TODO: Colors should just be RGB in the fixed function library
      // TODO: Seems like only texture 1 is used, is this accurate?

      for (var i = 0; i < materialEntry.TevStageInfo.Length; ++i) {
        var tevStageIndex = materialEntry.TevStageInfo[i];
        if (tevStageIndex == 65535) {
          continue;
        }

        var tevStage = bmd.MAT3.TevStages[tevStageIndex];

        var tevOrderIndex = materialEntry.TevOrderInfo[i];
        var tevOrder = bmd.MAT3.Tevorders[tevOrderIndex];

        // Updates which texture is referred to by TEXC
        var texStageIndex = tevOrder.TexMap;
        if (texStageIndex == 255) {
          colorManager.UpdateTextureColor(null);
        } else {
          var texStage = materialEntry.TexStages[texStageIndex];
          var textureIndex = bmd.MAT3.TextureIndieces[texStage];

          var bmdTexture = textures[textureIndex];

          // TODO: Share texture definitions between materials?
          var texture = materialManager.CreateTexture(bmdTexture.Image);

          texture.Name = bmdTexture.Name;
          texture.WrapModeU = bmdTexture.WrapModeS;
          texture.WrapModeV = bmdTexture.WrapModeT;

          var texCoordIndex = materialEntry.Unknown1[tevOrder.TexcoordID];
          texture.UvIndex = texCoordIndex;

          colorManager.UpdateTextureColor(texCoordIndex);
          material.SetTextureSource(texCoordIndex, texture);
        }

        // Updates which color is referred to by RASC
        var colorChannel = tevOrder.ChannelID;
        colorManager.UpdateRascColor(colorChannel);


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
            case 0: {
              var isCOne = ccC == TevStage.GxCc.GX_CC_ONE;

              if (ccA != TevStage.GxCc.GX_CC_ZERO && !isCOne) {
                colorValue = colorA.Multiply(colorOne.Subtract(colorC));
              }

              if (ccB != TevStage.GxCc.GX_CC_ZERO &&
                  ccC != TevStage.GxCc.GX_CC_ZERO) {
                IColorValue? term = null;

                var isBOne = ccB == TevStage.GxCc.GX_CC_ONE;

                if (!isBOne && isCOne) {
                  term = colorB;
                } else if (isBOne && !isCOne) {
                  term = colorC;
                } else {
                  term = colorB.Multiply(colorC);
                }

                Asserts.Nonnull(term);
                colorValue = colorValue == null ? term : colorValue.Add(term);
              }

              if (ccD != TevStage.GxCc.GX_CC_ZERO) {
                colorValue = colorValue == null
                                 ? colorD
                                 : colorValue.Add(colorD);
              }

              Asserts.Nonnull(colorValue);

              break;
            }

            default:
              throw new NotImplementedException();
          }

          if (colorOp == TevStage.TevOp.GX_TEV_ADD ||
              colorOp == TevStage.TevOp.GX_TEV_SUB) {
            switch (colorBias) {
              case TevStage.TevBias.GX_TB_ZERO: {
                break;
              }
              case TevStage.TevBias.GX_TB_ADDHALF: {
                colorValue = colorValue.Add(scHalf);
                break;
              }
              case TevStage.TevBias.GX_TB_SUBHALF: {
                colorValue = colorValue.Add(scMinusHalf);
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
          }

          colorManager.UpdateColorPrevious(colorValue);
        }

        // TODO: Implement alpha operations
      }

      equations.CreateColorOutput(
          FixedFunctionSource.OUTPUT_COLOR,
          colorManager.GetColor(TevStage.GxCc.GX_CC_CPREV));

      var sb = new StringBuilder();

      {
        using var os = new StringWriter(sb);

        // TODO: Print textures, colors

        new FixedFunctionEquationsPrettyPrinter<FixedFunctionSource>()
            .Print(os, equations);
      }

      var output = sb.ToString();

      ;
    }

    public IMaterial Material { get; }

    private class ColorManager {
      private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;

      private readonly IColorValue?[] textureColors_ = new IColorValue?[8];

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
      }

      public void UpdateColorPrevious(IColorValue colorValue)
        => this.colorValues_[TevStage.GxCc.GX_CC_CPREV] = colorValue;

      /*public void UpdateTextureColor(
          IColorNamedValue<FixedFunctionSource>? colorValue) {
        if (colorValue != null) {
          this.colorValues_[TevStage.GxCc.GX_CC_TEXC] = colorValue;
        } else {
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_TEXC);
        }
      }*/

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

      private TevOrder.ColorChannel? colorChannel_;

      public void UpdateRascColor(TevOrder.ColorChannel? colorChannel) {
        if (this.colorChannel_ != colorChannel) {
          this.colorChannel_ = colorChannel;
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_RASC);
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_RASA);
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
                  (FixedFunctionSource) (FixedFunctionSource.TEXTURE_0 + index),
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[TevStage.GxCc.GX_CC_TEXC] = texture;
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

      public IColorValue GetColor(TevStage.GxCc colorSource) {
        if (this.colorValues_.TryGetValue(colorSource, out var colorValue)) {
          return colorValue;
        }

        if (colorSource == TevStage.GxCc.GX_CC_TEXC) {
          return this.GetTextureColorChannel_();
        }

        if (colorSource == TevStage.GxCc.GX_CC_RASC ||
            colorSource == TevStage.GxCc.GX_CC_RASA) {
          return this.GetVertexColorChannel_(colorSource);
        }

        throw new NotImplementedException();
      }
    }
  }
}