using System;
using System.IO;

using f3dzex2.io;

using fin.math;

using schema.binary;


namespace f3dzex2.displaylist.opcodes {
  public partial class F3dOpcodeParser : IOpcodeParser {
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
              0,
              4) + 1;
          var writeOffset = BitLogic.ExtractFromRight(
              numVerticesMinusOneAndWriteOffset,
              4,
              4);
          er.AssertUInt16((ushort)(numVertices * 0x10));

          var address = er.ReadUInt32();
          using var ser = n64Memory.OpenAtAddress(address);

          return new VtxOpcodeCommand {
              Vertices = ser.ReadNewArray<F3dVtx>((int) numVertices),
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
        // TODO: Implement these
        case F3dOpcode.G_TEXTURE:
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

    [BinarySchema]
    private partial class F3dVtx : IVtx, IBinaryConvertible {
      public short X { get; }
      public short Y { get; }
      public short Z { get; }

      public short Flag { get; }
      
      public short U { get; }
      public short V { get; }
      
      public byte NormalXOrR { get; }
      public byte NormalYOrG { get; }
      public byte NormalZOrB { get; }
      public byte A { get; }
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