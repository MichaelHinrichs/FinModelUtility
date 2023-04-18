﻿using System;
using System.Drawing;
using System.Linq;

using f3dzex2.combiner;
using f3dzex2.displaylist;
using f3dzex2.displaylist.opcodes;
using f3dzex2.image;

using fin.data.lazy;
using fin.image;
using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
using fin.math.matrix;
using fin.model;
using fin.model.impl;
using fin.util.enums;
using fin.util.image;


namespace f3dzex2.model {
  public class DlModelBuilder {
    private readonly IN64Hardware n64Hardware_;
    private IMesh currentMesh_;

    private readonly LazyDictionary<ImageParams, IImage>
        lazyImageDictionary_;

    private readonly LazyDictionary<TextureParams, ITexture>
        lazyTextureDictionary_;

    private readonly LazyDictionary<MaterialParams, IMaterial>
        lazyMaterialDictionary_;

    private MaterialParams cachedMaterialParams_;
    private IMaterial cachedMaterial_;
    private readonly IF3dVertices vertices_;
    private bool isMaterialTransparent_ = false;

    /// <summary>
    ///   Each model gets its own DlModelBuilder, but they all need to share
    ///   the same N64 hardware state (RSP/RDP). If they don't, you'll run into
    ///   weird graphical bugs that you'll spend ages debugging. :(
    /// </summary>
    public DlModelBuilder(IN64Hardware n64Hardware) {
      this.n64Hardware_ = n64Hardware;
      this.currentMesh_ = this.Model.Skin.AddMesh();
      this.vertices_ = new F3dVertices(n64Hardware, this.Model);

      lazyImageDictionary_ =
          new(imageParams => {
            if (imageParams.IsInvalid) {
              return FinImage.Create1x1FromColor(this.vertices_.DiffuseColor);
            }

            using var er = this.n64Hardware_.Memory.OpenAtSegmentedAddress(
                imageParams.SegmentedAddress);
            var imageData =
                er.ReadBytes(imageParams.Width *
                             imageParams.Height * 4);

            return new N64ImageParser().Parse(imageParams.ColorFormat,
                                              imageParams.BitsPerTexel,
                                              imageData,
                                              imageParams.Width,
                                              imageParams.Height,
                                              new ushort[] { },
                                              false);
          });

      lazyTextureDictionary_ =
          new(textureParams
                  => {
                    var imageParams = textureParams.ImageParams;
                    var texture = this.Model.MaterialManager.CreateTexture(
                        this.lazyImageDictionary_[imageParams]);

                    var color = this.vertices_.DiffuseColor;
                    texture.Name = !imageParams.IsInvalid
                        ? String.Format("0x{0:X8}", textureParams.SegmentedAddress)
                        : $"rgb({color.R}, {color.G}, {color.B})";

                    texture.WrapModeU = textureParams.WrapModeS.AsFinWrapMode();
                    texture.WrapModeV = textureParams.WrapModeT.AsFinWrapMode();
                    texture.UvType = textureParams.UvType;
                    return texture;
                  });

      lazyMaterialDictionary_ =
          new(materialParams
                  => {
                    var texture0 =
                        this.lazyTextureDictionary_[materialParams.TextureParams0];
                    var texture1 =
                        this.lazyTextureDictionary_[materialParams.TextureParams1];

                    var finMaterial = this.Model.MaterialManager
                                          .AddFixedFunctionMaterial();

                    finMaterial.Name = $"[{texture0.Name}]/[{texture1.Name}]";
                    finMaterial.CullingMode = materialParams.CullingMode;

                    finMaterial.SetTextureSource(0, texture0);
                    finMaterial.SetTextureSource(1, texture1);

                    var equations = finMaterial.Equations;
                    var color0 = equations.CreateColorConstant(0);
                    var color1 = equations.CreateColorConstant(1);
                    var scalar1 = equations.CreateScalarConstant(1);
                    var scalar0 = equations.CreateScalarConstant(0);

                    var colorFixedFunctionOps =
                        new ColorFixedFunctionOps(equations);
                    var scalarFixedFunctionOps =
                        new ScalarFixedFunctionOps(equations);

                    var rsp = this.n64Hardware_.Rsp;
                    var environmentColor = equations.CreateColorConstant(
                        rsp.EnvironmentColor.R / 255.0,
                        rsp.EnvironmentColor.G / 255.0,
                        rsp.EnvironmentColor.B / 255.0);
                    var environmentAlpha = equations.CreateScalarConstant(
                        rsp.EnvironmentColor.A / 255.0);

                    IColorValue combinedColor = color0;
                    IScalarValue combinedAlpha = scalar0;

                    Func<GenericColorMux, IColorValue> getColorValue =
                        (colorMux) => colorMux switch {
                          GenericColorMux.G_CCMUX_COMBINED => combinedColor,
                          GenericColorMux.G_CCMUX_TEXEL0
                              => equations.CreateOrGetColorInput(
                                  FixedFunctionSource.TEXTURE_COLOR_0),
                          GenericColorMux.G_CCMUX_TEXEL1
                              => equations.CreateOrGetColorInput(
                                  FixedFunctionSource.TEXTURE_COLOR_1),
                          GenericColorMux.G_CCMUX_PRIMITIVE => color1,
                          GenericColorMux.G_CCMUX_SHADE
                              => equations.CreateOrGetColorInput(
                                  FixedFunctionSource.VERTEX_COLOR_0),
                          GenericColorMux.G_CCMUX_ENVIRONMENT => environmentColor,
                          GenericColorMux.G_CCMUX_1 => color1,
                          GenericColorMux.G_CCMUX_0 => color0,
                          GenericColorMux.G_CCMUX_NOISE => color1,
                          GenericColorMux.G_CCMUX_CENTER => color1,
                          GenericColorMux.G_CCMUX_K4 => color1,
                          GenericColorMux.G_CCMUX_COMBINED_ALPHA =>
                          equations.CreateColor(combinedAlpha),
                          GenericColorMux.G_CCMUX_TEXEL0_ALPHA
                              => equations.CreateOrGetColorInput(
                                  FixedFunctionSource.TEXTURE_ALPHA_0),
                          GenericColorMux.G_CCMUX_TEXEL1_ALPHA
                              => equations.CreateOrGetColorInput(
                              FixedFunctionSource.TEXTURE_ALPHA_1),
                          GenericColorMux.G_CCMUX_PRIMITIVE_ALPHA => color1,
                          GenericColorMux.G_CCMUX_SHADE_ALPHA
                              => equations.CreateOrGetColorInput(
                                  FixedFunctionSource.VERTEX_ALPHA_0),
                          GenericColorMux.G_CCMUX_ENV_ALPHA =>
                          equations.CreateColor(environmentAlpha),
                          GenericColorMux.G_CCMUX_PRIM_LOD_FRAC => color1,
                          GenericColorMux.G_CCMUX_SCALE => color1,
                          GenericColorMux.G_CCMUX_K5 => color1,
                          _ => throw new ArgumentOutOfRangeException(
                          nameof(colorMux),
                          colorMux,
                          null)
                        };

                    Func<GenericAlphaMux, IScalarValue> getAlphaValue =
                        (alphaMux) => alphaMux switch {
                          GenericAlphaMux.G_ACMUX_COMBINED => combinedAlpha,
                          GenericAlphaMux.G_ACMUX_TEXEL0 =>                      
                              equations.CreateOrGetScalarInput(
                        FixedFunctionSource.TEXTURE_ALPHA_0),
                          GenericAlphaMux.G_ACMUX_TEXEL1 =>
                              equations.CreateOrGetScalarInput(
                                  FixedFunctionSource.TEXTURE_ALPHA_1),
                          GenericAlphaMux.G_ACMUX_PRIMITIVE => scalar1,
                          GenericAlphaMux.G_ACMUX_SHADE
                              => equations.CreateOrGetScalarInput(
                                  FixedFunctionSource.VERTEX_ALPHA_0),
                          GenericAlphaMux.G_ACMUX_ENVIRONMENT => environmentAlpha,
                          GenericAlphaMux.G_ACMUX_1 => scalar1,
                          GenericAlphaMux.G_ACMUX_0 => scalar0,
                          GenericAlphaMux.G_ACMUX_PRIM_LOD_FRAC => scalar1,
                          GenericAlphaMux.G_ACMUX_LOD_FRACTION => scalar1,
                          _ => throw new ArgumentOutOfRangeException(
                          nameof(alphaMux),
                          alphaMux,
                          null)
                        };

                    foreach (var combinerCycleParams in new[] {
                             materialParams.CombinerCycleParams0,
                             materialParams.CombinerCycleParams1
                         }) {
                      var cA = getColorValue(combinerCycleParams.ColorMuxA);
                      var cB = getColorValue(combinerCycleParams.ColorMuxB);
                      var cC = getColorValue(combinerCycleParams.ColorMuxC);
                      var cD = getColorValue(combinerCycleParams.ColorMuxD);

                      combinedColor = colorFixedFunctionOps.Add(
                          colorFixedFunctionOps.Multiply(
                              colorFixedFunctionOps.Subtract(cA, cB),
                              cC),
                          cD) ?? colorFixedFunctionOps.Zero;

                      var aA = getAlphaValue(combinerCycleParams.AlphaMuxA);
                      var aB = getAlphaValue(combinerCycleParams.AlphaMuxB);
                      var aC = getAlphaValue(combinerCycleParams.AlphaMuxC);
                      var aD = getAlphaValue(combinerCycleParams.AlphaMuxD);

                      combinedAlpha = scalarFixedFunctionOps.Add(
                          scalarFixedFunctionOps.Multiply(
                              scalarFixedFunctionOps.Subtract(aA, aB),
                              aC),
                          aD) ?? scalarFixedFunctionOps.Zero;
                    }

                    equations.CreateColorOutput(FixedFunctionSource.OUTPUT_COLOR,
                                                combinedColor);
                    equations.CreateScalarOutput(FixedFunctionSource.OUTPUT_ALPHA,
                                                 combinedAlpha);

                    if (finMaterial.Textures.Any(
                            texture
                                => ImageUtil.GetTransparencyType(texture.Image) ==
                                   ImageTransparencyType.TRANSPARENT)) {
                      finMaterial.SetAlphaCompare(AlphaOp.Or,
                                                  AlphaCompareType.Always,
                                                  .5f,
                                                  AlphaCompareType.Always,
                                                  0);
                      finMaterial.SetBlending(BlendMode.ADD,
                                              BlendFactor.SRC_ALPHA,
                                              BlendFactor.ONE_MINUS_SRC_ALPHA,
                                              LogicOp.UNDEFINED);
                    } else {
                      finMaterial.SetAlphaCompare(AlphaOp.Or,
                                                  AlphaCompareType.Greater,
                                                  .5f,
                                                  AlphaCompareType.Never,
                                                  0);
                    }

                    return finMaterial;
                  });
    }

    public IModel Model { get; } = new ModelImpl();

    public IReadOnlyFinMatrix4x4 Matrix {
      set => this.n64Hardware_.Rsp.Matrix = value;
    }

    public int GetNumberOfTriangles() =>
        this.Model.Skin.Meshes
            .SelectMany(mesh => mesh.Primitives)
            .Select(primitive => primitive.Vertices.Count / 3)
            .Sum();

    public void AddDl(IDisplayList dl) {
      foreach (var opcodeCommand in dl.OpcodeCommands) {
        switch (opcodeCommand) {
          case NoopOpcodeCommand _:
            break;
          case DlOpcodeCommand dlOpcodeCommand: {
            foreach (var childDl in dlOpcodeCommand.PossibleBranches) {
              AddDl(childDl);
            }

            if (!dlOpcodeCommand.PushCurrentDlToStack) {
              return;
            }

            break;
          }
          case EndDlOpcodeCommand _: {
            return;
          }
          case MtxOpcodeCommand mtxOpcodeCommand:
            break;
          case PopMtxOpcodeCommand popMtxOpcodeCommand:
            break;
          case SetEnvColorOpcodeCommand setEnvColorOpcodeCommand: {
            this.n64Hardware_.Rsp.EnvironmentColor = Color.FromArgb(
                setEnvColorOpcodeCommand.A,
                setEnvColorOpcodeCommand.R,
                setEnvColorOpcodeCommand.G,
                setEnvColorOpcodeCommand.B);
            break;
          }
          case SetFogColorOpcodeCommand setFogColorOpcodeCommand:
            break;
          // Geometry mode commands
          case SetGeometryModeOpcodeCommand setGeometryModeOpcodeCommand: {
            var flagsToEnable = setGeometryModeOpcodeCommand.FlagsToEnable;
            this.n64Hardware_.Rsp.GeometryMode |= flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.n64Hardware_.Rdp.Tmem.CullingMode =
                  this.n64Hardware_.Rsp.GeometryMode.GetCullingModeNonEx2();
            }

            break;
          }
          case ClearGeometryModeOpcodeCommand clearGeometryModeOpcodeCommand: {
            var flagsToEnable = clearGeometryModeOpcodeCommand.FlagsToDisable;
            this.n64Hardware_.Rsp.GeometryMode &= ~flagsToEnable;
            if (flagsToEnable.CheckFlag(GeometryMode.G_CULL_FRONT_NONEX2) ||
                flagsToEnable.CheckFlag(GeometryMode.G_CULL_BACK_NONEX2)) {
              this.n64Hardware_.Rdp.Tmem.CullingMode =
                  this.n64Hardware_.Rsp.GeometryMode.GetCullingModeNonEx2();
            }

            break;
          }
          case SetTileOpcodeCommand setTileOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpSetTile(
                setTileOpcodeCommand.ColorFormat,
                setTileOpcodeCommand.BitsPerTexel,
                setTileOpcodeCommand.Num64BitValuesPerRow,
                setTileOpcodeCommand.OffsetOfTextureInTmem,
                setTileOpcodeCommand.TileDescriptorIndex,
                setTileOpcodeCommand.WrapModeS,
                setTileOpcodeCommand.WrapModeT);
            break;
          }
          case SetTileSizeOpcodeCommand setTileSizeOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpSetTileSize(0,
              0,
              setTileSizeOpcodeCommand
                  .TileDescriptorIndex,
              setTileSizeOpcodeCommand.Width,
              setTileSizeOpcodeCommand.Height);
            break;
          }
          case SetTimgOpcodeCommand setTimgOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpSetTextureImage(
                setTimgOpcodeCommand.ColorFormat,
                setTimgOpcodeCommand.BitsPerTexel,
                0,
                setTimgOpcodeCommand
                    .TextureSegmentedAddress);
            break;
          }
          case TextureOpcodeCommand textureOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsSpTexture(
                textureOpcodeCommand.HorizontalScaling,
                textureOpcodeCommand.VerticalScaling,
                textureOpcodeCommand.MaximumNumberOfMipmaps,
                textureOpcodeCommand.TileDescriptorIndex,
                textureOpcodeCommand.NewTileDescriptorState);
            break;
          }
          case SetCombineOpcodeCommand setCombineOpcodeCommand: {
            this.n64Hardware_.Rdp.CombinerCycleParams0 =
                setCombineOpcodeCommand.CombinerCycleParams0;
            this.n64Hardware_.Rdp.CombinerCycleParams1 =
                setCombineOpcodeCommand.CombinerCycleParams1;
            break;
          }
          case VtxOpcodeCommand vtxOpcodeCommand: {
            this.vertices_.LoadVertices(
                vtxOpcodeCommand.Vertices,
                vtxOpcodeCommand.IndexToBeginStoringVertices);
            break;
          }
          case Tri1OpcodeCommand tri1OpcodeCommand: {
            var material = this.GetOrCreateMaterial_();
            var vertices =
                tri1OpcodeCommand.VertexIndicesInOrder.Select(
                    this.vertices_.GetOrCreateVertexAtIndex);
            var triangles = this.currentMesh_.AddTriangles(vertices.ToArray())
                                .SetMaterial(material)
                                .SetVertexOrder(VertexOrder.NORMAL);

            if (isMaterialTransparent_) {
              triangles.SetInversePriority(1);
            } else {
              triangles.SetInversePriority(0);
            }

            break;
          }
          case LoadBlockOpcodeCommand loadBlockOpcodeCommand: {
            this.n64Hardware_.Rdp.Tmem.GsDpLoadBlock(0,
              0,
              loadBlockOpcodeCommand.TileDescriptorIndex,
              loadBlockOpcodeCommand.Texels,
              0);
            break;
          }
          case MoveMemOpcodeCommand moveMemOpcodeCommand: {
            // TODO: How to handle this in a more generalized way?
            switch (moveMemOpcodeCommand.DmemAddress) {
              // Diffuse light
              // https://hack64.net/wiki/doku.php?id=super_mario_64:fast3d_display_list_commands
              case DmemAddress.G_MV_L0: {
                using var er =
                    this.n64Hardware_.Memory.OpenAtSegmentedAddress(
                        moveMemOpcodeCommand.SegmentedAddress);
                var r = er.ReadByte();
                var g = er.ReadByte();
                var b = er.ReadByte();

                // TODO: Support normalized light direction

                this.vertices_.DiffuseColor = Color.FromArgb(r, g, b);

                break;
              }
              // Ambient light
              case DmemAddress.G_MV_L1: {
                break;
              }
            }

            break;
          }
          default:
            throw new ArgumentOutOfRangeException(nameof(opcodeCommand));
        }
      }
    }

    private IMaterial GetOrCreateMaterial_() {
      var newMaterialParams = this.n64Hardware_.Rdp.Tmem.GetMaterialParams();
      if (!cachedMaterialParams_.Equals(newMaterialParams)) {
        this.cachedMaterialParams_ = newMaterialParams;
        this.cachedMaterial_ = this.lazyMaterialDictionary_[newMaterialParams];
        this.isMaterialTransparent_ =
            ImageUtil.GetTransparencyType(
                this.cachedMaterial_.Textures.First().Image) ==
            ImageTransparencyType.TRANSPARENT;
      }

      return this.cachedMaterial_;
    }
  }
}