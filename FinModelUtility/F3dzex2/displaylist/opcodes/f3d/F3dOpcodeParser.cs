﻿using System;
using System.IO;

using f3dzex2.combiner;
using f3dzex2.image;
using f3dzex2.io;
using f3dzex2.model;

using fin.math;


namespace f3dzex2.displaylist.opcodes.f3d {
  public class F3dOpcodeParser : IOpcodeParser {
    public IOpcodeCommand Parse(IN64Memory n64Memory,
                                IDisplayListReader dlr,
                                IEndianBinaryReader er) {
      var baseOffset = er.Position;
      var opcode = (F3dOpcode) er.ReadByte();
      var opcodeCommand = ParseOpcodeCommand_(n64Memory, dlr, er, opcode);
      er.Position = baseOffset + GetCommandLength_(opcode);
      return opcodeCommand;
    }

    private IOpcodeCommand ParseOpcodeCommand_(IN64Memory n64Memory,
                                               IDisplayListReader dlr,
                                               IEndianBinaryReader er,
                                               F3dOpcode opcode) {
      switch (opcode) {
        case F3dOpcode.G_MTX: {
          var mtxParams = er.ReadByte();
          er.AssertUInt16(0);
          var address = er.ReadUInt32();
          return new MtxOpcodeCommand {
              Params = mtxParams, RamAddress = address,
          };
        }
        case F3dOpcode.G_POPMTX: {
          er.AssertUInt24(0);
          er.AssertUInt32(0);
          return new PopMtxOpcodeCommand { NumberOfMatrices = 1 };
        }
        case F3dOpcode.G_VTX: {
          var numVerticesMinusOneAndWriteOffset = er.ReadByte();
          var numVertices = BitLogic.ExtractFromRight(
              numVerticesMinusOneAndWriteOffset,
              4,
              4) + 1;
          var writeOffset = BitLogic.ExtractFromRight(
              numVerticesMinusOneAndWriteOffset,
              0,
              4);
          er.AssertUInt16((ushort) (numVertices * 0x10));

          var address = er.ReadUInt32();
          using var ser = n64Memory.OpenAtSegmentedAddress(address);

          return new VtxOpcodeCommand {
              Vertices = ser.ReadNewArray<F3dVertex>((int) numVertices),
              IndexToBeginStoringVertices = (byte) writeOffset,
          };
        }
        case F3dOpcode.G_DL: {
          var storeReturnAddress = er.ReadByte() == 0;
          er.AssertUInt16(0);
          var address = er.ReadUInt32();
          return new DlOpcodeCommand {
              PossibleBranches =
                  dlr.ReadPossibleDisplayLists(n64Memory, this, address),
              PushCurrentDlToStack = storeReturnAddress
          };
        }
        case F3dOpcode.G_ENDDL: {
          er.AssertUInt24(0);
          er.AssertUInt32(0);
          return new EndDlOpcodeCommand();
        }
        case F3dOpcode.G_TRI1: {
          er.AssertUInt32(0);
          return new Tri1OpcodeCommand {
              VertexOrder = TriVertexOrder.ABC,
              VertexIndexA = (byte) (er.ReadByte() / 0xA),
              VertexIndexB = (byte) (er.ReadByte() / 0xA),
              VertexIndexC = (byte) (er.ReadByte() / 0xA),
          };
        }
        case F3dOpcode.G_SETENVCOLOR: {
          er.AssertUInt24(0);
          return new SetEnvColorOpcodeCommand {
              R = er.ReadByte(),
              G = er.ReadByte(),
              B = er.ReadByte(),
              A = er.ReadByte(),
          };
        }
        case F3dOpcode.G_SETFOGCOLOR: {
          er.AssertUInt24(0);
          return new SetFogColorOpcodeCommand {
              R = er.ReadByte(),
              G = er.ReadByte(),
              B = er.ReadByte(),
              A = er.ReadByte(),
          };
        }
        case F3dOpcode.G_SETTIMG: {
          N64ImageParser.SplitN64ImageFormat(er.ReadByte(),
                                             out var colorFormat,
                                             out var bitSize);
          er.AssertUInt16(0);
          return new SetTimgOpcodeCommand {
              ColorFormat = colorFormat,
              BitsPerTexel = bitSize,
              TextureSegmentedAddress = er.ReadUInt32(),
          };
        }
        case F3dOpcode.G_SETGEOMETRYMODE: {
          er.AssertUInt24(0);
          return new SetGeometryModeOpcodeCommand {
              FlagsToEnable = (GeometryMode) er.ReadUInt32()
          };
        }
        case F3dOpcode.G_CLEARGEOMETRYMODE: {
          er.AssertUInt24(0);
          return new ClearGeometryModeOpcodeCommand {
              FlagsToDisable = (GeometryMode) er.ReadUInt32()
          };
        }
        case F3dOpcode.G_TEXTURE: {
          er.AssertByte(0);

          var mipmapLevelsAndTileDescriptor = er.ReadByte();
          var tileDescriptor =
              (TileDescriptorIndex) BitLogic.ExtractFromRight(
                  mipmapLevelsAndTileDescriptor,
                  0,
                  3);
          var maximumNumberOfMipmaps =
              (byte) BitLogic.ExtractFromRight(mipmapLevelsAndTileDescriptor,
                                               3,
                                               3);
          var newTileDescriptorState = (TileDescriptorState) er.ReadByte();
          var horizontalScale = er.ReadUInt16();
          var verticalScale = er.ReadUInt16();

          return new TextureOpcodeCommand {
              TileDescriptorIndex = tileDescriptor,
              NewTileDescriptorState = newTileDescriptorState,
              HorizontalScaling = horizontalScale,
              VerticalScaling = verticalScale,
              MaximumNumberOfMipmaps = maximumNumberOfMipmaps,
          };
        }
        case F3dOpcode.G_SETTILE: {
          er.Position -= 1;
          var first = er.ReadUInt32();
          var second = er.ReadUInt32();

          var colorFormat =
              (N64ColorFormat) BitLogic.ExtractFromRight(first, 21, 3);
          var bitSize =
              (BitsPerTexel) BitLogic.ExtractFromRight(first, 19, 2);
          var num64BitValuesPerRow =
              (ushort) BitLogic.ExtractFromRight(first, 9, 9);
          var offsetOfTextureInTmem =
              (ushort) BitLogic.ExtractFromRight(first, 0, 9);

          var tileDescriptor =
              (TileDescriptorIndex) BitLogic.ExtractFromRight(second, 24, 3);
          var wrapModeT =
              (F3dWrapMode) BitLogic.ExtractFromRight(second, 18, 2);
          var wrapModeS = (F3dWrapMode) BitLogic.ExtractFromRight(second, 8, 2);

          return new SetTileOpcodeCommand {
              TileDescriptorIndex = tileDescriptor,
              ColorFormat = colorFormat,
              BitsPerTexel = bitSize,
              WrapModeT = wrapModeT,
              WrapModeS = wrapModeS,
              Num64BitValuesPerRow = num64BitValuesPerRow,
              OffsetOfTextureInTmem = offsetOfTextureInTmem,
          };
        }
        case F3dOpcode.G_SETTILESIZE: {
          er.Position += 3;

          var tileDescriptor = (TileDescriptorIndex) er.ReadByte();

          var widthAndHeight = er.ReadUInt24();
          var width =
              (ushort) (widthAndHeight >>
                        12); // (ushort) (((widthAndHeight >> 12) >> 2) + 1);
          var height =
              (ushort) (widthAndHeight &
                        0xFFF); // (ushort) (((widthAndHeight & 0xFFF) >> 2) + 1);

          return new SetTileSizeOpcodeCommand {
              TileDescriptorIndex = tileDescriptor,
              Width = width,
              Height = height,
          };
        }
        case F3dOpcode.G_SETCOMBINE: {
          //           aaaa cccc ceee gggi iiik kkkk 
          // bbbb jjjj mmmo oodd dfff hhhl llnn nppp

          var first = er.ReadUInt24();
          var second = er.ReadUInt32();

          var a = BitLogic.ExtractFromRight(first, 20, 4);
          var c = BitLogic.ExtractFromRight(first, 15, 5);
          var e = BitLogic.ExtractFromRight(first, 12, 3);
          var g = BitLogic.ExtractFromRight(first, 9, 3);
          var i = BitLogic.ExtractFromRight(first, 5, 4);
          var k = BitLogic.ExtractFromRight(first, 0, 5);

          var b = BitLogic.ExtractFromRight(second, 28, 4);
          var j = BitLogic.ExtractFromRight(second, 24, 4);
          var m = BitLogic.ExtractFromRight(second, 21, 3);
          var o = BitLogic.ExtractFromRight(second, 18, 3);
          var d = BitLogic.ExtractFromRight(second, 15, 3);
          var f = BitLogic.ExtractFromRight(second, 12, 3);
          var h = BitLogic.ExtractFromRight(second, 9, 3);
          var l = BitLogic.ExtractFromRight(second, 6, 3);
          var n = BitLogic.ExtractFromRight(second, 3, 3);
          var p = BitLogic.ExtractFromRight(second, 0, 3);

          return new SetCombineOpcodeCommand {
              CombinerCycleParams0 = new CombinerCycleParams {
                  ColorMuxA = GetColorMuxA_(a),
                  ColorMuxB = GetColorMuxB_(b),
                  ColorMuxC = GetColorMuxC_(c),
                  ColorMuxD = GetColorMuxD_(d),
                  AlphaMuxA = this.GetAlphaMuxABD_(e),
                  AlphaMuxB = this.GetAlphaMuxABD_(f),
                  AlphaMuxC = this.GetAlphaMuxC_(g),
                  AlphaMuxD = this.GetAlphaMuxABD_(h),
              },
              CombinerCycleParams1 = new CombinerCycleParams {
                  ColorMuxA = GetColorMuxA_(i),
                  ColorMuxB = GetColorMuxB_(j),
                  ColorMuxC = GetColorMuxC_(k),
                  ColorMuxD = GetColorMuxD_(l),
                  AlphaMuxA = this.GetAlphaMuxABD_(m),
                  AlphaMuxB = this.GetAlphaMuxABD_(n),
                  AlphaMuxC = this.GetAlphaMuxC_(o),
                  AlphaMuxD = this.GetAlphaMuxABD_(p),
              }
          };
        }
        case F3dOpcode.G_LOADBLOCK: {
          er.Position += 3;

          var tileDescriptor = (TileDescriptorIndex) er.ReadByte();
          var texelsAndDxt = er.ReadUInt24();
          var texels = texelsAndDxt >> 12;

          return new LoadBlockOpcodeCommand {
              TileDescriptorIndex = tileDescriptor, Texels = (ushort) texels,
          };
        }
        case F3dOpcode.G_MOVEMEM: {
          var commandType = (DmemAddress) er.ReadByte();
          var sizeInBytes = er.ReadUInt16();
          var segmentedAddress = er.ReadUInt32();

          return new MoveMemOpcodeCommand {
              DmemAddress = commandType, SegmentedAddress = segmentedAddress,
          };
        }
        // TODO: Implement these
        case F3dOpcode.G_MOVEWORD:
        case F3dOpcode.G_SETOTHERMODE_L:
        case F3dOpcode.G_SETOTHERMODE_H:
          return new NoopOpcodeCommand();
        case F3dOpcode.G_RDPLOADSYNC:
        case F3dOpcode.G_RDPPIPESYNC:
        case F3dOpcode.G_RDPTILESYNC:
        case F3dOpcode.G_RDPFULLSYNC:
        case F3dOpcode.G_SPNOOP:
        case F3dOpcode.G_NOOP: {
          er.AssertUInt24(0);
          er.AssertUInt32(0);
          return new NoopOpcodeCommand();
        }
        default:
          throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
      }
    }

    private int GetCommandLength_(F3dOpcode opcode) {
      switch (opcode) {
        case F3dOpcode.G_SPNOOP:
        case F3dOpcode.G_MTX:
        case F3dOpcode.G_MOVEMEM:
        case F3dOpcode.G_VTX:
        case F3dOpcode.G_DL:
        case F3dOpcode.G_RDPHALF_CONT:
        case F3dOpcode.G_RDPHALF_2:
        case F3dOpcode.G_RDPHALF_1:
        case F3dOpcode.G_CLEARGEOMETRYMODE:
        case F3dOpcode.G_SETGEOMETRYMODE:
        case F3dOpcode.G_ENDDL:
        case F3dOpcode.G_SETOTHERMODE_H:
        case F3dOpcode.G_SETOTHERMODE_L:
        case F3dOpcode.G_TEXTURE:
        case F3dOpcode.G_MOVEWORD:
        case F3dOpcode.G_POPMTX:
        case F3dOpcode.G_CULLDL:
        case F3dOpcode.G_TRI1:
        case F3dOpcode.G_NOOP:
        case F3dOpcode.G_RDPLOADSYNC:
        case F3dOpcode.G_RDPPIPESYNC:
        case F3dOpcode.G_RDPTILESYNC:
        case F3dOpcode.G_RDPFULLSYNC:
        case F3dOpcode.G_SETKEYGB:
        case F3dOpcode.G_SETKEYR:
        case F3dOpcode.G_SETSCISSOR:
        case F3dOpcode.G_SETPRIMDEPTH:
        case F3dOpcode.G_RDPSETOTHERMODE:
        case F3dOpcode.G_LOADTLUT:
        case F3dOpcode.G_SETTILESIZE:
        case F3dOpcode.G_LOADBLOCK:
        case F3dOpcode.G_LOADTILE:
        case F3dOpcode.G_SETTILE:
        case F3dOpcode.G_FILLRECT:
        case F3dOpcode.G_SETFILLCOLOR:
        case F3dOpcode.G_SETFOGCOLOR:
        case F3dOpcode.G_SETBLENDCOLOR:
        case F3dOpcode.G_SETPRIMCOLOR:
        case F3dOpcode.G_SETENVCOLOR:
        case F3dOpcode.G_SETCOMBINE:
        case F3dOpcode.G_SETTIMG:
        case F3dOpcode.G_SETZIMG:
        case F3dOpcode.G_SETCIMG:
          return 1 * 2 * 4;
        case F3dOpcode.G_TEXRECT:
        case F3dOpcode.G_TEXRECTFLIP:
          return 3 * 2 * 4;
        default:
          throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
      }
    }

    /// <summary>
    ///   http://ultra64.ca/files/documentation/online-manuals/man/pro-man/pro12/index12.6.html#:~:text=Color%20Combiner%20(CC)%20can%20perform,in%20one%2Dcycle%20mode).
    /// </summary>
    private GenericColorMux GetColorMuxA_(uint value) => value switch {
        0              => GenericColorMux.G_CCMUX_COMBINED,
        1              => GenericColorMux.G_CCMUX_TEXEL0,
        2              => GenericColorMux.G_CCMUX_TEXEL1,
        3              => GenericColorMux.G_CCMUX_PRIMITIVE,
        4              => GenericColorMux.G_CCMUX_SHADE,
        5              => GenericColorMux.G_CCMUX_ENVIRONMENT,
        6              => GenericColorMux.G_CCMUX_1,
        7              => GenericColorMux.G_CCMUX_NOISE,
        >= 8 and <= 15 => GenericColorMux.G_CCMUX_0,
        _              => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private GenericColorMux GetColorMuxB_(uint value) => value switch {
        0              => GenericColorMux.G_CCMUX_COMBINED,
        1              => GenericColorMux.G_CCMUX_TEXEL0,
        2              => GenericColorMux.G_CCMUX_TEXEL1,
        3              => GenericColorMux.G_CCMUX_PRIMITIVE,
        4              => GenericColorMux.G_CCMUX_SHADE,
        5              => GenericColorMux.G_CCMUX_ENVIRONMENT,
        6              => GenericColorMux.G_CCMUX_CENTER,
        7              => GenericColorMux.G_CCMUX_K4,
        >= 8 and <= 15 => GenericColorMux.G_CCMUX_0,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private GenericColorMux GetColorMuxC_(uint value) => value switch {
        0  => GenericColorMux.G_CCMUX_COMBINED,
        1  => GenericColorMux.G_CCMUX_TEXEL0,
        2  => GenericColorMux.G_CCMUX_TEXEL1,
        3  => GenericColorMux.G_CCMUX_PRIMITIVE,
        4  => GenericColorMux.G_CCMUX_SHADE,
        5  => GenericColorMux.G_CCMUX_ENVIRONMENT,
        6  => GenericColorMux.G_CCMUX_SCALE,
        7  => GenericColorMux.G_CCMUX_COMBINED_ALPHA,
        8  => GenericColorMux.G_CCMUX_TEXEL0_ALPHA,
        9  => GenericColorMux.G_CCMUX_TEXEL1_ALPHA,
        10 => GenericColorMux.G_CCMUX_PRIMITIVE_ALPHA,
        11 => GenericColorMux.G_CCMUX_SHADE_ALPHA,
        12 => GenericColorMux.G_CCMUX_ENV_ALPHA,
        13 => GenericColorMux.G_CCMUX_LOD_FRAC,
        14 => GenericColorMux.G_CCMUX_PRIM_LOD_FRAC,
        15 => GenericColorMux.G_CCMUX_K5,
        >= 16 and <= 31 => GenericColorMux.G_CCMUX_0,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private GenericColorMux GetColorMuxD_(uint value) => value switch {
        0 => GenericColorMux.G_CCMUX_COMBINED,
        1 => GenericColorMux.G_CCMUX_TEXEL0,
        2 => GenericColorMux.G_CCMUX_TEXEL1,
        3 => GenericColorMux.G_CCMUX_PRIMITIVE,
        4 => GenericColorMux.G_CCMUX_SHADE,
        5 => GenericColorMux.G_CCMUX_ENVIRONMENT,
        6 => GenericColorMux.G_CCMUX_1,
        7 => GenericColorMux.G_CCMUX_0,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private GenericAlphaMux GetAlphaMuxABD_(uint value) => value switch {
      0 => GenericAlphaMux.G_ACMUX_COMBINED,
      1 => GenericAlphaMux.G_ACMUX_TEXEL0,
      2 => GenericAlphaMux.G_ACMUX_TEXEL1,
      3 => GenericAlphaMux.G_ACMUX_PRIMITIVE,
      4 => GenericAlphaMux.G_ACMUX_SHADE,
      5 => GenericAlphaMux.G_ACMUX_ENVIRONMENT,
      6 => GenericAlphaMux.G_ACMUX_1,
      7 => GenericAlphaMux.G_ACMUX_0,
      _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private GenericAlphaMux GetAlphaMuxC_(uint value) => value switch {
        0 => GenericAlphaMux.G_ACMUX_LOD_FRACTION,
        1 => GenericAlphaMux.G_ACMUX_TEXEL0,
        2 => GenericAlphaMux.G_ACMUX_TEXEL1,
        3 => GenericAlphaMux.G_ACMUX_PRIMITIVE,
        4 => GenericAlphaMux.G_ACMUX_SHADE,
        5 => GenericAlphaMux.G_ACMUX_ENVIRONMENT,
        6 => GenericAlphaMux.G_ACMUX_PRIM_LOD_FRAC,
        7 => GenericAlphaMux.G_ACMUX_0,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
  }
}