﻿using System.Drawing;

using fin.data.lazy;
using fin.image;
using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
using fin.model;
using fin.util.asserts;

using FinBlendFactor = fin.model.BlendFactor;
using FinLogicOp = fin.model.LogicOp;
using FinAlphaOp = fin.model.AlphaOp;

namespace gx {
  /// <summary>
  ///   BMD material, one of the common formats for the GameCube.
  ///
  ///   For more info:
  ///   http://www.amnoid.de/gc/tev.html
  /// </summary>
  public class GxFixedFunctionMaterial {
    private const bool STRICT = false;

    public override string ToString() => this.Material.Name ?? "(n/a)";

    public GxFixedFunctionMaterial(
        IModel model,
        IMaterialManager materialManager,
        IPopulatedMaterial populatedMaterial,
        IList<IGxTexture> tex1Textures,
        ILazyDictionary<(IGxTexture, ITexCoordGen?, ITextureMatrixInfo?),
            ITexture> lazyTextureDictionary) {
      // TODO: materialEntry.Flag determines draw order

      var materialName = populatedMaterial.Name;

      var textures =
          populatedMaterial.TextureIndices?
                           .Select(i => i != -1 ? tex1Textures[i] : null)
                           .ToArray()
          ?? tex1Textures;

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

      var depthFunction = populatedMaterial.DepthFunction;
      material.DepthMode =
          depthFunction.Enable
              ? (depthFunction.WriteNewValueIntoDepthBuffer
                  ? DepthMode.USE_DEPTH_BUFFER
                  : DepthMode.SKIP_WRITE_TO_DEPTH_BUFFER)
              : DepthMode.IGNORE_DEPTH_BUFFER;
      material.DepthCompareType =
          ConvertGxDepthCompareTypeToFin_(depthFunction.Func);

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

      var equations = material.Equations;
      var registers = materialManager.Registers;

      var colorZero = equations.CreateColorConstant(0);
      var colorHalf = equations.CreateColorConstant(.5);
      var colorOne = equations.CreateColorConstant(1);

      var scZero = equations.CreateScalarConstant(0);
      var scOne = equations.CreateScalarConstant(1);
      var scTwo = equations.CreateScalarConstant(2);
      var scFour = equations.CreateScalarConstant(4);
      var scHalf = equations.CreateScalarConstant(.5);
      var scMinusHalf = equations.CreateScalarConstant(-.5);
      var sc255 = equations.CreateScalarConstant(255);
      var sc255Sqr = equations.CreateScalarConstant(256 * 255);

      var colorFixedFunctionOps = new ColorFixedFunctionOps(equations);
      var scalarFixedFunctionOps = new ScalarFixedFunctionOps(equations);

      var valueManager = new ValueManager(equations, registers);

      valueManager.SetColorRegisters(populatedMaterial.ColorRegisters);
      valueManager.SetKonstColors(populatedMaterial.KonstColors);

      var vertexColors = new IColorValue[2];
      var vertexAlphas = new IScalarValue[2];
      for (byte i = 0; i < 2; i++) {
        vertexColors[i] = equations.CreateColorInput(
            FixedFunctionSource.VERTEX_COLOR_0 + i,
            colorZero);
        vertexAlphas[i] = equations.CreateScalarInput(
            FixedFunctionSource.VERTEX_ALPHA_0 + i,
            scZero);
      }

      for (var i = 0; i < 4; ++i) {
        var colorChannelControl = populatedMaterial.ColorChannelControls?[i];
        if (colorChannelControl == null) {
          continue;
        }

        var activeLights =
            colorChannelControl.LitMask.GetActiveLights().ToArray();

        // TODO: Properly handle lights and attenuation and stuff

        // TODO: Expose material/ambient registers to side panel

        if (i % 2 == 0) {
          var colorIndex = (byte) (i / 2);

          var vertexColor = vertexColors[colorIndex];

          var materialColor = populatedMaterial.MaterialColors[colorIndex];
          var materialColorRegisterValue =
              colorChannelControl.MaterialSrc switch {
                  GxColorSrc.Register => registers.GetOrCreateColorRegister(
                      $"GxMaterialColor{colorIndex}",
                      equations.CreateColorConstant(
                          materialColor.R / 255.0,
                          materialColor.G / 255.0,
                          materialColor.B / 255.0)
                  ),
                  GxColorSrc.Vertex => vertexColor,
              };

          var colorValue = materialColorRegisterValue;

          var isLightingEnabled = colorChannelControl.LightingEnabled;
          if (isLightingEnabled) {
            var ambientColor = populatedMaterial.AmbientColors[colorIndex];
            var ambientColorRegisterValue =
                colorChannelControl.AmbientSrc switch {
                    GxColorSrc.Register => registers.GetOrCreateColorRegister(
                        $"GxAmbientColor{colorIndex}",
                        equations.CreateColorConstant(
                            ambientColor.R / 255.0,
                            ambientColor.G / 255.0,
                            ambientColor.B / 255.0)),
                    GxColorSrc.Vertex => vertexColor,
                };

            // TODO: Factor in how colors are merged in channel control
            IColorValue? mergedLightColor = null;
            // TODO: Should these be averaged?
            foreach (var activeLight in activeLights) {
              var lightSrc = FixedFunctionSource.LIGHT_0_COLOR +
                             activeLight;
              mergedLightColor = colorFixedFunctionOps.Add(
                  mergedLightColor,
                  equations.CreateOrGetColorInput(lightSrc, colorZero));
            }

            var illuminationColor =
                colorFixedFunctionOps.Add(mergedLightColor,
                                          ambientColorRegisterValue);
            if (illuminationColor != null) {
              illuminationColor.Clamp = true;
            }

            colorValue =
                colorFixedFunctionOps.Multiply(materialColorRegisterValue,
                                               illuminationColor);
          }

          var color = colorValue ?? colorZero;
          valueManager.UpdateColorChannelColor(
              colorIndex switch {
                  0 => GxColorChannel.GX_COLOR0A0,
                  1 => GxColorChannel.GX_COLOR1A1,
              },
              color);
          valueManager.UpdateColorChannelColor(
              colorIndex switch {
                  0 => GxColorChannel.GX_COLOR0,
                  1 => GxColorChannel.GX_COLOR1,
              },
              color);
        } else {
          var alphaIndex = (byte) ((i - 1) / 2);

          var vertexAlpha = vertexAlphas[alphaIndex];

          var materialColor = populatedMaterial.MaterialColors[alphaIndex];
          var materialAlphaRegisterValue =
              colorChannelControl.MaterialSrc switch {
                  GxColorSrc.Register => registers.GetOrCreateScalarRegister(
                      $"GxMaterialAlpha{alphaIndex}",
                      equations.CreateScalarConstant(
                          materialColor.A / 255.0)),
                  GxColorSrc.Vertex => vertexAlpha,
              };

          var alphaValue = materialAlphaRegisterValue;

          var isLightingEnabled = colorChannelControl.LightingEnabled;
          if (isLightingEnabled) {
            var ambientColor = populatedMaterial.AmbientColors[alphaIndex];
            var ambientAlphaRegisterValue =
                colorChannelControl.AmbientSrc switch {
                    GxColorSrc.Register => registers.GetOrCreateScalarRegister(
                        $"GxAmbientAlpha{alphaIndex}",
                        equations.CreateScalarConstant(
                            ambientColor.A / 255.0)),
                    GxColorSrc.Vertex => vertexAlpha,
                };

            // TODO: Factor in how colors are merged in channel control
            IScalarValue? mergedLightAlpha = null;
            // TODO: Should these be averaged?
            foreach (var activeLight in activeLights) {
              var lightSrc = FixedFunctionSource.LIGHT_0_ALPHA +
                             activeLight;
              mergedLightAlpha = scalarFixedFunctionOps.Add(
                  mergedLightAlpha,
                  equations.CreateOrGetScalarInput(lightSrc, scZero));
            }

            var illuminationAlpha =
                scalarFixedFunctionOps.Add(mergedLightAlpha,
                                           ambientAlphaRegisterValue);
            if (illuminationAlpha != null) {
              illuminationAlpha.Clamp = true;
            }

            alphaValue =
                scalarFixedFunctionOps.Multiply(
                    materialAlphaRegisterValue,
                    illuminationAlpha);
          }

          var alpha = alphaValue ?? scZero;
          valueManager.UpdateColorChannelAlpha(
              alphaIndex switch {
                  0 => GxColorChannel.GX_COLOR0A0,
                  1 => GxColorChannel.GX_COLOR1A1,
              },
              alpha
          );
          valueManager.UpdateColorChannelAlpha(
              alphaIndex switch {
                  0 => GxColorChannel.GX_ALPHA0,
                  1 => GxColorChannel.GX_ALPHA1,
              },
              alpha
          );
        }
      }

      for (var i = 0; i < populatedMaterial.TevStageInfos.Length; ++i) {
        var tevStage = populatedMaterial.TevStageInfos[i];
        if (tevStage == null) {
          continue;
        }

        //var tevSwapMode = populatedMaterial.TevSwapModes[i];
        var tevOrder = populatedMaterial.TevOrderInfos[i];

        // Updates which texture is referred to by TEXC
        var textureIndex = tevOrder.TexMap;
        if (textureIndex == -1 || (!STRICT && textureIndex >= textures.Count)) {
          valueManager.UpdateTextureIndex(null);
        } else {
          var bmdTexture = textures[textureIndex];

          var texCoordGen =
              populatedMaterial.TexCoordGens[tevOrder.TexCoordId]!;

          var texMatrixType = texCoordGen.TexMatrix;
          var texMatrixIndex = (texMatrixType - GxTexMatrix.TexMtx0) / 3;
          var texMatrix = texMatrixType != GxTexMatrix.Identity
              ? populatedMaterial.TextureMatrices[texMatrixIndex]
              : null;

          var texture =
              lazyTextureDictionary[(bmdTexture, texCoordGen, texMatrix)];

          valueManager.UpdateTextureIndex(textureIndex);
          material.SetTextureSource(textureIndex, texture);
        }

        // Updates which color is referred to by RASC
        var colorChannel = tevOrder.ColorChannelId;
        valueManager.UpdateRascChannel(colorChannel);

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

            case TevOp.GX_TEV_COMP_R8_GT: {
              colorValue = colorFixedFunctionOps.Add(
                  colorD,
                  colorA.R.TernaryOperator(
                      BoolComparisonType.GREATER_THAN,
                      colorB.R,
                      colorC,
                      colorZero));
              break;
            }
            case TevOp.GX_TEV_COMP_R8_EQ: {
              colorValue = colorFixedFunctionOps.Add(
                  colorD,
                  colorA.R.TernaryOperator(
                      BoolComparisonType.EQUAL_TO,
                      colorB.R,
                      colorC,
                      colorZero));
              break;
            }

            case TevOp.GX_TEV_COMP_GR16_GT: {
              var valueA = scalarFixedFunctionOps.Add(
                  scalarFixedFunctionOps.Multiply(colorA.G, sc255Sqr),
                  scalarFixedFunctionOps.Multiply(colorA.R, sc255)) ?? scZero;
              var valueB = scalarFixedFunctionOps.Add(
                  scalarFixedFunctionOps.Multiply(colorB.G, sc255Sqr),
                  scalarFixedFunctionOps.Multiply(colorB.R, sc255)) ?? scZero;

              colorValue = colorFixedFunctionOps.Add(
                  colorD,
                  valueA.TernaryOperator(
                      BoolComparisonType.GREATER_THAN,
                      valueB,
                      colorC,
                      colorZero));
              break;
            }

            default: {
              if (GxFixedFunctionMaterial.STRICT) {
                throw new NotImplementedException();
              } else {
                colorValue = colorC;
              }

              break;
            }
          }

          valueManager.UpdateColorRegister(tevStage.color_regid,
                                           colorValue ?? colorZero);
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
              alphaValue.Clamp = tevStage.alpha_clamp;

              break;
            }

            default: {
              if (GxFixedFunctionMaterial.STRICT) {
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
      private readonly IFixedFunctionRegisters registers_;

      private readonly Dictionary<GxCc, IColorValue>
          colorValues_ = new();

      private readonly Dictionary<GxCa, IScalarValue>
          alphaValues_ = new();

      public ValueManager(
          IFixedFunctionEquations<FixedFunctionSource> equations,
          IFixedFunctionRegisters registers) {
        this.equations_ = equations;
        this.registers_ = registers;

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

      private readonly IColorValue?[] textureColors_ = new IColorValue?[8];
      private readonly IColorValue?[] textureAlphas_ = new IColorValue?[8];
      private int? textureIndex_ = null;

      public void UpdateTextureIndex(int? index) {
        this.textureIndex_ = index;

        if (index != null) {
          Asserts.True(index >= 0 && index < 8);
        }
      }

      private IColorValue GetTextureColorChannel_() {
        var indexOrNull = this.textureIndex_;

        IColorValue texture;
        if (indexOrNull == null && !STRICT) {
          texture = colorUndefined_;
        } else {
          Asserts.Nonnull(indexOrNull);

          var index = indexOrNull.Value;
          Asserts.True(index >= 0 && index < 8);

          texture = this.textureColors_[index];
          if (texture == null) {
            this.textureColors_[index] =
                texture = this.equations_.CreateOrGetColorInput(
                    (FixedFunctionSource) (FixedFunctionSource.TEXTURE_COLOR_0 +
                                           index),
                    this.equations_.CreateColorConstant(0));
          }
        }

        return texture;
      }

      private IColorValue GetTextureAlphaChannel_() {
        var indexOrNull = this.textureIndex_;

        IColorValue texture;
        if (indexOrNull == null && !STRICT) {
          texture = colorUndefined_;
        } else {
          Asserts.Nonnull(indexOrNull);

          var index = indexOrNull.Value;
          Asserts.True(index >= 0 && index < 8);

          texture = this.textureAlphas_[index];
          if (texture == null) {
            this.textureAlphas_[index] =
                texture = this.equations_.CreateOrGetColorInput(
                    (FixedFunctionSource) (FixedFunctionSource.TEXTURE_ALPHA_0 +
                                           index),
                    this.equations_.CreateColorConstant(0));
          }
        }

        return texture;
      }

      private IScalarValue GetTextureAlphaChannelAsAlpha_() {
        var indexOrNull = this.textureIndex_;

        IScalarValue texture;
        if (indexOrNull == null && !STRICT) {
          texture = alphaUndefined_;
        } else {
          Asserts.Nonnull(indexOrNull);

          var index = indexOrNull.Value;
          Asserts.True(index >= 0 && index < 8);

          texture =
              this.equations_.CreateOrGetScalarInput(
                  (FixedFunctionSource) (FixedFunctionSource.TEXTURE_ALPHA_0 +
                                         index),
                  this.equations_.CreateScalarConstant(0));
        }

        return texture;
      }


      private GxColorChannel? colorChannel_;

      public void UpdateRascChannel(GxColorChannel? colorChannel) {
        this.colorChannel_ = colorChannel;
      }

      private readonly Dictionary<GxColorChannel, IColorValue>
          colorChannelColorColors_ = new();

      private readonly Dictionary<GxColorChannel, IColorValue>
          colorChannelColorAlphas_ = new();

      private readonly Dictionary<GxColorChannel, IScalarValue>
          colorChannelAlphas_ = new();

      public void UpdateColorChannelColor(
          GxColorChannel colorChannel,
          IColorValue colorValue) {
        this.colorChannelColorColors_[colorChannel] = colorValue;
      }

      public void UpdateColorChannelAlpha(
          GxColorChannel colorChannel,
          IScalarValue alphaValue) {
        this.colorChannelAlphas_[colorChannel] = alphaValue;
        this.colorChannelColorAlphas_[colorChannel] =
            this.equations_.CreateColor(alphaValue);
      }

      private IColorValue GetVertexColorChannel_(GxCc colorSource) {
        var channelOrNull = this.colorChannel_;
        Asserts.Nonnull(channelOrNull);

        var channel = channelOrNull.Value;
        var color = colorSource switch {
            GxCc.GX_CC_RASC => colorChannelColorColors_[channel],
            GxCc.GX_CC_RASA => colorChannelColorAlphas_[channel],
            _               => throw new NotImplementedException()
        };

        return color;
      }

      // TODO: Switch from vertex alpha to ambient/diffuse lights when applicable
      private IScalarValue GetVertexAlphaChannel_() {
        var channelOrNull = this.colorChannel_;
        Asserts.Nonnull(channelOrNull);

        var channel = channelOrNull.Value;
        var alpha = colorChannelAlphas_[channel];

        return alpha;
      }

      private IList<Color> colorRegisterColors_;
      private IList<Color> konstColorImpls_;

      public void SetColorRegisters(IList<Color> colorRegisterColors) {
        this.colorRegisterColors_ = colorRegisterColors;
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
              konstRgb.R / 255d,
              konstRgb.G / 255d,
              konstRgb.B / 255d);
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
          var (color, index, isColor) =
              this.GetColorRegisterColorForSource_(colorSource);

          if (isColor) {
            return this.registers_.GetOrCreateColorRegister(
                $"GxColor{index}",
                this.equations_.CreateColorConstant(
                    (color?.R ?? 255) / 255f,
                    (color?.G ?? 255) / 255f,
                    (color?.B ?? 255) / 255f));
          }

          return new ColorWrapper(this.registers_.GetOrCreateScalarRegister(
              $"GxAlpha{index}",
              this.equations_.CreateScalarConstant(
                  (color?.A ?? 255) / 255f)));
        }

        if (!GxFixedFunctionMaterial.STRICT) {
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
          var index = caIndex;
          var color = this.GetColorRegisterColorForIndex_(index);

          return this.registers_.GetOrCreateScalarRegister($"GxAlpha{index}",
            this.equations_.CreateScalarConstant(
                (color?.A ?? 255) / 255f));
        }

        if (!GxFixedFunctionMaterial.STRICT) {
          return this.alphaUndefined_;
        }

        throw new NotImplementedException();
      }

      private (Color? color, int index, bool isAlpha)
          GetColorRegisterColorForSource_(
          GxCc source) {
        var ccIndex = (int) source - (int) GxCc.GX_CC_C0;

        var isColor = ccIndex % 2 == 0;
        var index = isColor ? ccIndex / 2 : (ccIndex - 1) / 2;

        return (this.GetColorRegisterColorForIndex_(index), index, isColor);
      }

      private Color? GetColorRegisterColorForIndex_(int index) {
        if (this.colorRegisterColors_.Count > index) {
          return this.colorRegisterColors_[index];
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
              nameof(gxBlendFactor),
              gxBlendFactor,
              null)
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
              nameof(gxLogicOp),
              gxLogicOp,
              null)
      };

    private FinAlphaOp ConvertGxAlphaOpToFin_(GxAlphaOp bmdAlphaOp)
      => bmdAlphaOp switch {
          GxAlphaOp.And  => FinAlphaOp.And,
          GxAlphaOp.Or   => FinAlphaOp.Or,
          GxAlphaOp.XOR  => FinAlphaOp.XOR,
          GxAlphaOp.XNOR => FinAlphaOp.XNOR,
          _ => throw new ArgumentOutOfRangeException(
              nameof(bmdAlphaOp),
              bmdAlphaOp,
              null)
      };

    private AlphaCompareType ConvertGxAlphaCompareTypeToFin_(
        GxCompareType gxAlphaCompareType)
      => gxAlphaCompareType switch {
          GxCompareType.Never   => AlphaCompareType.Never,
          GxCompareType.Less    => AlphaCompareType.Less,
          GxCompareType.Equal   => AlphaCompareType.Equal,
          GxCompareType.LEqual  => AlphaCompareType.LEqual,
          GxCompareType.Greater => AlphaCompareType.Greater,
          GxCompareType.NEqual  => AlphaCompareType.NEqual,
          GxCompareType.GEqual  => AlphaCompareType.GEqual,
          GxCompareType.Always  => AlphaCompareType.Always,
          _ => throw new ArgumentOutOfRangeException(
              nameof(gxAlphaCompareType),
              gxAlphaCompareType,
              null)
      };

    private DepthCompareType ConvertGxDepthCompareTypeToFin_(
        GxCompareType gxDepthCompareType)
      => gxDepthCompareType switch {
          GxCompareType.Never   => DepthCompareType.Never,
          GxCompareType.Less    => DepthCompareType.Less,
          GxCompareType.Equal   => DepthCompareType.Equal,
          GxCompareType.LEqual  => DepthCompareType.LEqual,
          GxCompareType.Greater => DepthCompareType.Greater,
          GxCompareType.NEqual  => DepthCompareType.NEqual,
          GxCompareType.GEqual  => DepthCompareType.GEqual,
          GxCompareType.Always  => DepthCompareType.Always,
          _ => throw new ArgumentOutOfRangeException(
              nameof(gxDepthCompareType),
              gxDepthCompareType,
              null)
      };
  }
}