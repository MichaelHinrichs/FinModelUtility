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

      // TODO: Where are colors set inside the materials?

      for (var i = 0; i < materialEntry.TevStageInfo.Length; ++i) {
        var tevStageIndex = materialEntry.TevStageInfo[i];
        if (tevStageIndex == 65535) {
          continue;
        }

        var tevStage = bmd.MAT3.TevStages[tevStageIndex];

        var tevOrderIndex = materialEntry.TevOrderInfo[i];
        var tevOrder = bmd.MAT3.Tevorders[tevOrderIndex];

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

          material.SetTextureSource(texCoordIndex, texture);
        }

        // TODO: This seems to update which color RASC is referring to
        //tevOrder.ChannelID;

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
                var term = colorB.Multiply(colorC);
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
      private readonly Dictionary<TevStage.GxCc, IColorValue>
          colorValues_ = new();

      private readonly Dictionary<TevStage.GxCc, IScalarValue>
          alphaValues_ = new();

      private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;

      public ColorManager(
          IFixedFunctionEquations<FixedFunctionSource> equations) {
        this.equations_ = equations;

        var colorZero = equations.CreateColorConstant(0, 0);
        var colorOne = equations.CreateColorConstant(1);

        this.colorValues_[TevStage.GxCc.GX_CC_TEXC] =
            equations.CreateColorInput(FixedFunctionSource.TEXTURE, colorZero);
        this.colorValues_[TevStage.GxCc.GX_CC_RASC] =
            equations.CreateColorInput(FixedFunctionSource.VERTEX_COLOR,
                                       colorZero);
        this.colorValues_[TevStage.GxCc.GX_CC_RASA] =
            equations.CreateColorInput(FixedFunctionSource.VERTEX_ALPHA,
                                       colorZero);
        this.colorValues_[TevStage.GxCc.GX_CC_ZERO] = colorZero;
        this.colorValues_[TevStage.GxCc.GX_CC_ONE] = colorOne;
      }

      public void UpdateColorPrevious(IColorValue colorValue)
        => this.colorValues_[TevStage.GxCc.GX_CC_CPREV] = colorValue;

      public void UpdateTextureColor(
          IColorNamedValue<FixedFunctionSource>? colorValue) {
        if (colorValue != null) {
          this.colorValues_[TevStage.GxCc.GX_CC_TEXC] = colorValue;
        } else {
          this.colorValues_.Remove(TevStage.GxCc.GX_CC_TEXC);
        }
      }


      public IColorValue GetColor(TevStage.GxCc colorSource)
        => this.colorValues_[colorSource];

      public IScalarValue GetAlpha(TevStage.GxCc alphaSource)
        => this.alphaValues_[alphaSource];
    }
  }
}