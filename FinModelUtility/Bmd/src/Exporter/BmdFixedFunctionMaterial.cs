using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Linq;
using fin.model;
using bmd.GCN;
using bmd.schema.bmd.mat3;
using fin.image;
using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
using fin.util.asserts;
using gx;
using FinBlendFactor = fin.model.BlendFactor;
using FinLogicOp = fin.model.LogicOp;
using FinAlphaOp = fin.model.AlphaOp;
using FinAlphaCompareType = fin.model.AlphaCompareType;


namespace bmd.exporter {
  using TevStage = TevStageProps;

  /// <summary>
  ///   BMD material, one of the common formats for the GameCube.
  ///
  ///   For more info:
  ///   http://www.amnoid.de/gc/tev.html
  /// </summary>
  public class BmdFixedFunctionMaterial {
    private const bool STRICT = false;

    public override string ToString() => this.Material.Name ?? "(n/a)";

    public BmdFixedFunctionMaterial(
        IMaterialManager materialManager,
        IPopulatedMaterial populatedMaterial,
        IList<BmdTexture> tex1Textures) {
      // TODO: materialEntry.Flag determines draw order

      var materialName = populatedMaterial.Name;

      var textures =
          populatedMaterial.TextureIndices
                           .Select(i => i != -1 ? tex1Textures[i] : null)
                           .ToArray();

      var material = materialManager.AddFixedFunctionMaterial();
      material.Name = materialName;
      material.CullingMode =
          populatedMaterial.CullMode switch {
              GxCullMode.None  => CullingMode.SHOW_BOTH,
              GxCullMode.Front => CullingMode.SHOW_BACK_ONLY,
              GxCullMode.Back  => CullingMode.SHOW_FRONT_ONLY,
              GxCullMode.All   => CullingMode.SHOW_NEITHER,
              _                => throw new ArgumentOutOfRangeException(),
          };

      // Shamelessly copied from:
      // https://github.com/magcius/noclip.website/blob/c5a6d0137128065068b5842ffa9dff04f03eefdb/src/gx/gx_render.ts#L405-L423
      switch (populatedMaterial.BlendMode.BlendMode) {
        case GxBlendMode.NONE: {
          material.SetBlending(
              BlendMode.ADD,
              BlendFactor.ONE,
              BlendFactor.ZERO,
              LogicOp.UNDEFINED);
          break;
        }
        case GxBlendMode.BLEND: {
          material.SetBlending(
              BlendMode.ADD,
              this.ConvertGxBlendFactorToFin_(
                  populatedMaterial.BlendMode.SrcFactor),
              this.ConvertGxBlendFactorToFin_(
                  populatedMaterial.BlendMode.DstFactor),
              LogicOp.UNDEFINED);
          break;
        }
        case GxBlendMode.LOGIC: {
          // TODO: Might not be correct?
          material.SetBlending(
              BlendMode.NONE,
              this.ConvertGxBlendFactorToFin_(
                  populatedMaterial.BlendMode.SrcFactor),
              this.ConvertGxBlendFactorToFin_(
                  populatedMaterial.BlendMode.DstFactor),
              this.ConvertGxLogicOpToFin_(populatedMaterial.BlendMode.LogicOp));
          break;
        }
        case GxBlendMode.SUBTRACT: {
          material.SetBlending(
              BlendMode.REVERSE_SUBTRACT,
              BlendFactor.ONE,
              BlendFactor.ONE,
              LogicOp.UNDEFINED);
          break;
        }
        default: throw new ArgumentOutOfRangeException();
      }

      material.SetAlphaCompare(
          this.ConvertGxAlphaOpToFin_(populatedMaterial.AlphaCompare.MergeFunc),
          this.ConvertGxAlphaCompareTypeToFin_(
              populatedMaterial.AlphaCompare.Func0),
          populatedMaterial.AlphaCompare.Reference0,
          this.ConvertGxAlphaCompareTypeToFin_(
              populatedMaterial.AlphaCompare.Func1),
          populatedMaterial.AlphaCompare.Reference1);

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

      valueManager.SetColorRegisters(populatedMaterial.TevColors);
      valueManager.SetKonstColors(populatedMaterial.KonstColors);

      for (var i = 0; i < populatedMaterial.TevStageInfos.Length; ++i) {
        var tevStage = populatedMaterial.TevStageInfos[i];
        if (tevStage == null) {
          continue;
        }

        var tevOrder = populatedMaterial.TevOrderInfos[i];

        // Updates which texture is referred to by TEXC
        var textureIndex = tevOrder.TexMap;
        if (textureIndex == -1) {
          valueManager.UpdateTextureColor(null);
        } else {
          var bmdTexture = textures[textureIndex];

          // TODO: Share texture definitions between materials?
          var texture = materialManager.CreateTexture(bmdTexture.Image);

          texture.Name = bmdTexture.Name;
          texture.WrapModeU = bmdTexture.WrapModeS;
          texture.WrapModeV = bmdTexture.WrapModeT;
          texture.ColorType = bmdTexture.ColorType;

          var texCoordGen = populatedMaterial.TexCoordGens[tevOrder.TexCoordId]!;
          var texGenSrc = texCoordGen.TexGenSrc;
          switch (texGenSrc) {
            case >= GxTexGenSrc.Tex0 and <= GxTexGenSrc.Tex7: {
              var texCoordIndex = texGenSrc - GxTexGenSrc.Tex0;
              texture.UvIndex = texCoordIndex;
              break;
            }
            case GxTexGenSrc.Normal: {
              texture.UvType = UvType.LINEAR;
              break;
            }
            default: {
              //Asserts.Fail($"Unsupported texGenSrc type: {texGenSrc}");
              texture.UvIndex = 0;
              break;
            }
          }

          valueManager.UpdateTextureColor(textureIndex);
          material.SetTextureSource(textureIndex, texture);
        }

        // Updates which color is referred to by RASC
        var colorChannel = tevOrder.ColorChannelId;
        valueManager.UpdateRascColor(colorChannel);

        // Updates which values are referred to by konst
        valueManager.UpdateKonst(tevOrder.KonstColorSel,
                                 tevOrder.KonstAlphaSel);

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
                colorValue = colorC;
              }
              break;
            }
          }

          valueManager.UpdateColorRegister(tevStage.color_regid, colorValue);
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
        }
      }

      equations.CreateColorOutput(
          FixedFunctionSource.OUTPUT_COLOR,
          valueManager.GetColor(GxCc.GX_CC_CPREV));

      equations.CreateScalarOutput(
          FixedFunctionSource.OUTPUT_ALPHA,
          valueManager.GetAlpha(GxCa.GX_CA_APREV));

      // TODO: Set up compiled texture?
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

        var colorImage = FinImage.Create1x1FromColor(colorConstant);
        var colorTexture = materialManager.CreateTexture(colorImage);
        material.CompiledTexture = colorTexture;
      }
    }

    public IMaterial Material { get; }

    private class ValueManager {
      private readonly IColorValue colorUndefined_;
      private readonly IScalarValue alphaUndefined_;

      private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;


      private readonly Dictionary<GxColorChannel, IColorValue>
          colorChannelsColors_ = new();

      private readonly Dictionary<GxColorChannel, IScalarValue>
          alphaChannelsColors_ = new();

      private readonly Dictionary<GxCc, IColorValue>
          colorValues_ = new();

      private readonly Dictionary<GxCa, IScalarValue>
          alphaValues_ = new();

      public ValueManager(
          IFixedFunctionEquations<FixedFunctionSource> equations) {
        this.equations_ = equations;

        var colorZero = equations.CreateColorConstant(0);
        var colorOne = equations.CreateColorConstant(1);

        this.colorValues_[GxCc.GX_CC_ZERO] = colorZero;
        this.colorValues_[GxCc.GX_CC_ONE] = colorOne;

        this.alphaValues_[GxCa.GX_CA_ZERO] =
            equations.CreateScalarConstant(0);

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
            ColorRegister.GX_TEVPREV => GxCc.GX_CC_CPREV,
            ColorRegister.GX_TEVREG0 => GxCc.GX_CC_C0,
            ColorRegister.GX_TEVREG1 => GxCc.GX_CC_C1,
            ColorRegister.GX_TEVREG2 => GxCc.GX_CC_C2,
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
            ColorRegister.GX_TEVPREV => GxCa.GX_CA_APREV,
            ColorRegister.GX_TEVREG0 => GxCa.GX_CA_A0,
            ColorRegister.GX_TEVREG1 => GxCa.GX_CA_A1,
            ColorRegister.GX_TEVREG2 => GxCa.GX_CA_A2,
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
          this.colorValues_[GxCc.GX_CC_TEXC] = colorValue;
        } else {
          this.colorValues_.Remove(GxCc.GX_CC_TEXC);
        }
      }*/

      private readonly IColorValue?[] textureColors_ = new IColorValue?[8];
      private readonly IColorValue?[] textureAlphas_ = new IColorValue?[8];
      private int? textureIndex_ = null;

      public void UpdateTextureColor(int? index) {
        if (this.textureIndex_ != index) {
          this.textureIndex_ = index;
          this.colorValues_.Remove(GxCc.GX_CC_TEXC);
          this.colorValues_.Remove(GxCc.GX_CC_TEXA);
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
                  (FixedFunctionSource)(FixedFunctionSource.TEXTURE_COLOR_0 +
                                        index),
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[GxCc.GX_CC_TEXC] = texture;
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
                  (FixedFunctionSource)(FixedFunctionSource.TEXTURE_ALPHA_0 +
                                        index),
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[GxCc.GX_CC_TEXA] = texture;
      }

      private IScalarValue GetTextureAlphaChannelAsAlpha_() {
        var indexOrNull = this.textureIndex_;
        Asserts.Nonnull(indexOrNull);

        var index = indexOrNull.Value;
        Asserts.True(index >= 0 && index < 8);

        var texture =
            this.equations_.CreateScalarInput(
                (FixedFunctionSource)(FixedFunctionSource.TEXTURE_ALPHA_0 +
                                      index),
                this.equations_.CreateScalarConstant(0));

        return this.alphaValues_[GxCa.GX_CA_TEXA] = texture;
      }


      private GxColorChannel? colorChannel_;

      public void UpdateRascColor(GxColorChannel? colorChannel) {
        if (this.colorChannel_ != colorChannel) {
          this.colorChannel_ = colorChannel;
          this.colorValues_.Remove(GxCc.GX_CC_RASC);
          this.colorValues_.Remove(GxCc.GX_CC_RASA);
          this.alphaValues_.Remove(GxCa.GX_CA_RASA);
        }
      }

      // TODO: Switch from vertex color to ambient/diffuse lights when applicable
      private IColorValue GetVertexColorChannel_(GxCc colorSource) {
        var channelOrNull = this.colorChannel_;
        Asserts.Nonnull(channelOrNull);

        var channel = channelOrNull.Value;

        if (!this.colorChannelsColors_.TryGetValue(channel, out var color)) {
          // TODO: Handle different color channels properly, how does vertex color factor in??
          var source = colorSource switch {
              GxCc.GX_CC_RASC => channel switch {
                  GxColorChannel.GX_COLOR0A0 =>
                      FixedFunctionSource.DIFFUSE_LIGHTING_COLOR,
                  GxColorChannel.GX_COLOR1A1 =>
                      FixedFunctionSource.AMBIENT_LIGHTING_COLOR,
                  _ => throw new NotImplementedException()
              },
              GxCc.GX_CC_RASA => channel switch {
                  GxColorChannel.GX_COLOR0A0 =>
                      FixedFunctionSource.DIFFUSE_LIGHTING_ALPHA,
                  GxColorChannel.GX_COLOR1A1 =>
                      FixedFunctionSource.AMBIENT_LIGHTING_ALPHA,
                  _ => throw new NotImplementedException()
              },
              _ => throw new NotImplementedException()
          };

          this.colorChannelsColors_[channel] =
              color = this.equations_.CreateColorInput(
                  source,
                  this.equations_.CreateColorConstant(0));
        }

        return this.colorValues_[colorSource] = color;
      }

      // TODO: Switch from vertex alpha to ambient/diffuse lights when applicable
      private IScalarValue GetVertexAlphaChannel_() {
        var channelOrNull = this.colorChannel_;
        Asserts.Nonnull(channelOrNull);

        var channel = channelOrNull.Value;


        if (!this.alphaChannelsColors_.TryGetValue(channel, out var alpha)) {
          var source = channel switch {
              GxColorChannel.GX_COLOR0A0 => FixedFunctionSource
                  .DIFFUSE_LIGHTING_ALPHA,
              GxColorChannel.GX_COLOR1A1 => FixedFunctionSource
                  .AMBIENT_LIGHTING_ALPHA,
              _ => throw new NotImplementedException()
          };

          this.alphaChannelsColors_[channel] =
              alpha = this.equations_.CreateScalarInput(
                  source,
                  this.equations_.CreateScalarConstant(0));
        }

        return this.alphaValues_[GxCa.GX_CA_RASA] = alpha;
      }

      private IList<Color> constColorImpls_;
      private IList<Color> konstColorImpls_;

      public void SetColorRegisters(IList<Color> constColorImpls) {
        this.constColorImpls_ = constColorImpls;
      }

      public void SetKonstColors(IList<Color> konstColors) {
        this.konstColorImpls_ = konstColors;
      }

      private GxKonstColorSel tevStageColorConstantSel_;
      private GxKonstAlphaSel tevStageAlphaConstantSel_;

      public void UpdateKonst(GxKonstColorSel tevStageColorConstantSel,
                              GxKonstAlphaSel tevStageAlphaConstantSel) {
        this.tevStageColorConstantSel_ = tevStageColorConstantSel;
        this.tevStageAlphaConstantSel_ = tevStageAlphaConstantSel;
      }

      public bool TryGetEnumIndex_<T>(T value, T min, T max, out int index)
          where T : IComparable, IConvertible {
        var minCompare = value.CompareTo(min);
        var maxCompare = value.CompareTo(max);

        if (minCompare >= 0 && maxCompare <= 0) {
          index = value.ToInt32(null) - min.ToInt32(null);
          return true;
        }

        index = -1;
        return false;
      }

      // https://github.com/magcius/bmdview/blob/master/tev.markdown#gx_settevkcolorsel
      public IColorValue GetKonstColor_(GxKonstColorSel sel) {
        if (TryGetEnumIndex_(sel,
                             GxKonstColorSel.KCSel_1,
                             GxKonstColorSel.KCSel_1_8,
                             out var fracIndex)) {
          var numerator = 8 - fracIndex;
          var intensity = numerator / 8f;
          return this.equations_.CreateColorConstant(intensity);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstColorSel.KCSel_K0,
                             GxKonstColorSel.KCSel_K3,
                             out var rgbIndex)) {
          var konstRgb = this.konstColorImpls_[rgbIndex];
          return this.equations_.CreateColorConstant(
              konstRgb.R / 255d, konstRgb.G / 255d, konstRgb.B / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstColorSel.KCSel_K0_R,
                             GxKonstColorSel.KCSel_K3_R,
                             out var rIndex)) {
          var konstR = this.konstColorImpls_[rIndex];
          return this.equations_.CreateColorConstant(
              konstR.R / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstColorSel.KCSel_K0_G,
                             GxKonstColorSel.KCSel_K3_G,
                             out var gIndex)) {
          var konstG = this.konstColorImpls_[gIndex];
          return this.equations_.CreateColorConstant(
              konstG.G / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstColorSel.KCSel_K0_B,
                             GxKonstColorSel.KCSel_K3_B,
                             out var bIndex)) {
          var konstB = this.konstColorImpls_[bIndex];
          return this.equations_.CreateColorConstant(
              konstB.B / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstColorSel.KCSel_K0_A,
                             GxKonstColorSel.KCSel_K3_A,
                             out var aIndex)) {
          var konstA = this.konstColorImpls_[aIndex];
          return this.equations_.CreateColorConstant(
              konstA.A / 255d);
        }

        throw new NotImplementedException();
      }

      public IScalarValue GetKonstAlpha_(GxKonstAlphaSel sel) {
        if (TryGetEnumIndex_(sel,
                             GxKonstAlphaSel.KASel_1,
                             GxKonstAlphaSel.KASel_1_8,
                             out var fracIndex)) {
          var numerator = 8 - fracIndex;
          var intensity = numerator / 8f;
          return this.equations_.CreateScalarConstant(intensity);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstAlphaSel.KASel_K0_R,
                             GxKonstAlphaSel.KASel_K3_R,
                             out var rIndex)) {
          var konstR = this.konstColorImpls_[rIndex];
          return this.equations_.CreateScalarConstant(
              konstR.R / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstAlphaSel.KASel_K0_G,
                             GxKonstAlphaSel.KASel_K3_G,
                             out var gIndex)) {
          var konstG = this.konstColorImpls_[gIndex];
          return this.equations_.CreateScalarConstant(
              konstG.G / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstAlphaSel.KASel_K0_B,
                             GxKonstAlphaSel.KASel_K3_B,
                             out var bIndex)) {
          var konstB = this.konstColorImpls_[bIndex];
          return this.equations_.CreateScalarConstant(
              konstB.B / 255d);
        }

        if (TryGetEnumIndex_(sel,
                             GxKonstAlphaSel.KASel_K0_A,
                             GxKonstAlphaSel.KASel_K3_A,
                             out var aIndex)) {
          var konstA = this.konstColorImpls_[aIndex];
          return this.equations_.CreateScalarConstant(
              konstA.A / 255d);
        }

        throw new NotImplementedException();
      }

      public IColorValue GetColor(GxCc colorSource) {
        if (this.colorValues_.TryGetValue(colorSource, out var colorValue)) {
          return colorValue;
        }

        if (colorSource == GxCc.GX_CC_TEXC) {
          return this.GetTextureColorChannel_();
        }

        if (colorSource == GxCc.GX_CC_TEXA) {
          return this.GetTextureAlphaChannel_();
        }

        if (colorSource == GxCc.GX_CC_RASC ||
            colorSource == GxCc.GX_CC_RASA) {
          return this.GetVertexColorChannel_(colorSource);
        }

        if (colorSource == GxCc.GX_CC_KONST) {
          return this.GetKonstColor_(this.tevStageColorConstantSel_);
        }

        if (colorSource >= GxCc.GX_CC_C0 &&
            colorSource <= GxCc.GX_CC_A2) {
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

      public IScalarValue GetAlpha(GxCa alphaSource) {
        if (this.alphaValues_.TryGetValue(alphaSource, out var alphaValue)) {
          return alphaValue;
        }

        if (alphaSource == GxCa.GX_CA_TEXA) {
          return this.GetTextureAlphaChannelAsAlpha_();
        }

        if (alphaSource == GxCa.GX_CA_RASA) {
          return this.GetVertexAlphaChannel_();
        }

        if (alphaSource == GxCa.GX_CA_KONST) {
          return this.GetKonstAlpha_(this.tevStageAlphaConstantSel_);
        }

        if (TryGetEnumIndex_(
                alphaSource,
                GxCa.GX_CA_A0,
                GxCa.GX_CA_A2,
                out var caIndex)) {
          var index = 1 + caIndex;
          var constColorImpl = this.GetCCColor_(index);

          var constColorByte = constColorImpl?.A ?? 255;
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
        var ccIndex = (int)source - (int)GxCc.GX_CC_C0;

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

    private FinBlendFactor ConvertGxBlendFactorToFin_(
        GxBlendFactor gxBlendFactor)
      => gxBlendFactor switch {
          GxBlendFactor.ZERO      => FinBlendFactor.ZERO,
          GxBlendFactor.ONE       => FinBlendFactor.ONE,
          GxBlendFactor.SRC_COLOR => FinBlendFactor.SRC_COLOR,
          GxBlendFactor.ONE_MINUS_SRC_COLOR => FinBlendFactor
              .ONE_MINUS_SRC_COLOR,
          GxBlendFactor.SRC_ALPHA => FinBlendFactor.SRC_ALPHA,
          GxBlendFactor.ONE_MINUS_SRC_ALPHA => FinBlendFactor
              .ONE_MINUS_SRC_ALPHA,
          GxBlendFactor.DST_ALPHA => FinBlendFactor.DST_ALPHA,
          GxBlendFactor.ONE_MINUS_DST_ALPHA => FinBlendFactor
              .ONE_MINUS_DST_ALPHA,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(gxBlendFactor), gxBlendFactor, null)
      };

    private FinLogicOp ConvertGxLogicOpToFin_(GxLogicOp gxLogicOp)
      => gxLogicOp switch {
          GxLogicOp.CLEAR         => FinLogicOp.CLEAR,
          GxLogicOp.AND           => FinLogicOp.AND,
          GxLogicOp.AND_REVERSE   => FinLogicOp.AND_REVERSE,
          GxLogicOp.COPY          => FinLogicOp.COPY,
          GxLogicOp.AND_INVERTED  => FinLogicOp.AND_INVERTED,
          GxLogicOp.NOOP          => FinLogicOp.NOOP,
          GxLogicOp.XOR           => FinLogicOp.XOR,
          GxLogicOp.OR            => FinLogicOp.OR,
          GxLogicOp.NOR           => FinLogicOp.NOR,
          GxLogicOp.EQUIV         => FinLogicOp.EQUIV,
          GxLogicOp.INVERT        => FinLogicOp.INVERT,
          GxLogicOp.OR_REVERSE    => FinLogicOp.OR_REVERSE,
          GxLogicOp.COPY_INVERTED => FinLogicOp.COPY_INVERTED,
          GxLogicOp.OR_INVERTED   => FinLogicOp.OR_INVERTED,
          GxLogicOp.NAND          => FinLogicOp.NAND,
          GxLogicOp.SET           => FinLogicOp.SET,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(gxLogicOp), gxLogicOp, null)
      };

    private FinAlphaOp ConvertGxAlphaOpToFin_(GxAlphaOp bmdAlphaOp)
      => bmdAlphaOp switch {
          GxAlphaOp.And  => FinAlphaOp.And,
          GxAlphaOp.Or   => FinAlphaOp.Or,
          GxAlphaOp.XOR  => FinAlphaOp.XOR,
          GxAlphaOp.XNOR => FinAlphaOp.XNOR,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(bmdAlphaOp), bmdAlphaOp, null)
      };

    private FinAlphaCompareType ConvertGxAlphaCompareTypeToFin_(
        GxAlphaCompareType gxAlphaAlphaCompareType)
      => gxAlphaAlphaCompareType switch {
          GxAlphaCompareType.Never   => FinAlphaCompareType.Never,
          GxAlphaCompareType.Less    => FinAlphaCompareType.Less,
          GxAlphaCompareType.Equal   => FinAlphaCompareType.Equal,
          GxAlphaCompareType.LEqual  => FinAlphaCompareType.LEqual,
          GxAlphaCompareType.Greater => FinAlphaCompareType.Greater,
          GxAlphaCompareType.NEqual  => FinAlphaCompareType.NEqual,
          GxAlphaCompareType.GEqual  => FinAlphaCompareType.GEqual,
          GxAlphaCompareType.Always  => FinAlphaCompareType.Always,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(gxAlphaAlphaCompareType), gxAlphaAlphaCompareType,
                   null)
      };
  }
}