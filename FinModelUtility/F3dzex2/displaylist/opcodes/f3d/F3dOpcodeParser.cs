using System;
using System.IO;

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
          return new PopMtxOpcodeCommand {NumberOfMatrices = 1};
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
          er.AssertUInt16((ushort)(numVertices * 0x10));

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
              VertexOrder  = TriVertexOrder.ABC,
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
              (byte) BitLogic.ExtractFromRight(mipmapLevelsAndTileDescriptor, 3, 3);
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
          var tileDescriptor =
              (TileDescriptorIndex) BitLogic.ExtractFromRight(second, 24, 3);

          var wrapModeT = (F3dWrapMode) BitLogic.ExtractFromRight(second, 18, 2);
          var wrapModeS = (F3dWrapMode) BitLogic.ExtractFromRight(second, 8, 2);

          return new SetTileOpcodeCommand {
              TileDescriptorIndex = tileDescriptor,
              ColorFormat = colorFormat,
              BitsPerTexel = bitSize,
              WrapModeT = wrapModeT,
              WrapModeS = wrapModeS,
          };
        }
        case F3dOpcode.G_SETTILESIZE: {
          er.Position += 3;

          var tileDescriptor = (TileDescriptorIndex) er.ReadByte();

          var widthAndHeight = er.ReadUInt24();
          var width = (ushort) (((widthAndHeight >> 12) >> 2) + 1);
          var height = (ushort) (((widthAndHeight & 0xFFF) >> 2) + 1);

          return new SetTileSizeOpcodeCommand {
              TileDescriptorIndex = tileDescriptor, Width = width, Height = height,
          };
        }
        case F3dOpcode.G_SETCOMBINE: {
            er.Position -= 1 ;
          var wholeCommand = er.ReadUInt64();
          return new SetCombineOpcodeCommand {
              // TODO: Look into what the heck this means
              ClearTextureSegmentedAddress = wholeCommand == 0xFCFFFFFFFFFE793C
          };
        }
        case F3dOpcode.G_LOADBLOCK: {
          er.Position += 3;

          var tileDescriptor = (TileDescriptorIndex) er.ReadByte();

          return new LoadBlockOpcodeCommand {
            TileDescriptorIndex = tileDescriptor,
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
  }
}