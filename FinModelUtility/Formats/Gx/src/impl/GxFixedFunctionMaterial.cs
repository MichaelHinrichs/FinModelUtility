using System.Drawing;

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
  public partial class GxFixedFunctionMaterial {
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
      var registers = Asserts.CastNonnull(materialManager.Registers);

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