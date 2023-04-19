using f3dzex2.io;
using f3dzex2.model;

using System.IO;
using System;


namespace f3dzex2.displaylist.opcodes.f3dzex2 {
  public class F3dzex2OpcodeParser : IOpcodeParser {
    public IOpcodeCommand Parse(IN64Memory n64Memory,
                                IDisplayListReader dlr,
                                IEndianBinaryReader er) {
      var baseOffset = er.Position;
      var opcode = (F3dzex2Opcode) er.ReadByte();
      var opcodeCommand = ParseOpcodeCommand_(n64Memory, dlr, er, opcode);
      er.Position = baseOffset + GetCommandLength_(opcode);
      return opcodeCommand;
    }

    public DisplayListType Type => DisplayListType.F3DZEX2;

    private IOpcodeCommand ParseOpcodeCommand_(IN64Memory n64Memory,
                                               IDisplayListReader dlr,
                                               IEndianBinaryReader er,
                                               F3dzex2Opcode opcode) {
      switch (opcode) {
        case F3dzex2Opcode.G_NOOP:
          return new NoopOpcodeCommand();
        case F3dzex2Opcode.G_VTX: {
          var numVerticesToLoad = (byte) (er.ReadUInt16() >> 4);
          var indexToBeginStoringVertices =
              (byte) ((er.ReadByte() >> 1) - numVerticesToLoad);
          using var ser = n64Memory.OpenAtSegmentedAddress(er.ReadUInt32());
          return new VtxOpcodeCommand {
              IndexToBeginStoringVertices = indexToBeginStoringVertices,
              Vertices = ser.ReadNewArray<F3dVertex>(numVerticesToLoad),
          };
        }
        case F3dzex2Opcode.G_TRI1: {
          return new Tri1OpcodeCommand {
              VertexIndexA = (byte) (er.ReadByte() >> 1),
              VertexIndexB = (byte) (er.ReadByte() >> 1),
              VertexIndexC = (byte) (er.ReadByte() >> 1),
          };
        }
        case F3dzex2Opcode.G_TRI2: {
          var a0 = (byte) (er.ReadByte() >> 1);
          var b0 = (byte) (er.ReadByte() >> 1);
          var c0 = (byte) (er.ReadByte() >> 1);
          er.AssertByte(0);
          var a1 = (byte) (er.ReadByte() >> 1);
          var b1 = (byte) (er.ReadByte() >> 1);
          var c1 = (byte) (er.ReadByte() >> 1);

          return new Tri2OpcodeCommand {
              VertexIndexA0 = a0,
              VertexIndexB0 = b0,
              VertexIndexC0 = c0,
              VertexIndexA1 = a1,
              VertexIndexB1 = b1,
              VertexIndexC1 = c1,
          };
        }
        default:
          throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
      }
    }

    private int GetCommandLength_(F3dzex2Opcode opcode) {
      switch (opcode) {
        case F3dzex2Opcode.G_NOOP:
        case F3dzex2Opcode.G_VTX:
        case F3dzex2Opcode.G_MODIFYVTX:
        case F3dzex2Opcode.G_CULLDL:
        case F3dzex2Opcode.G_TRI1:
        case F3dzex2Opcode.G_TRI2:
        case F3dzex2Opcode.G_QUAD:
        case F3dzex2Opcode.G_SPECIAL_3:
        case F3dzex2Opcode.G_SPECIAL_2:
        case F3dzex2Opcode.G_SPECIAL_1:
        case F3dzex2Opcode.G_DMA_IO:
        case F3dzex2Opcode.G_TEXTURE:
        case F3dzex2Opcode.G_POPMTX:
        case F3dzex2Opcode.G_GEOMETRYMODE:
        case F3dzex2Opcode.G_MTX:
        case F3dzex2Opcode.G_MOVEWORD:
        case F3dzex2Opcode.G_MOVEMEM:
        case F3dzex2Opcode.G_DL:
        case F3dzex2Opcode.G_ENDDL:
        case F3dzex2Opcode.G_SPNOOP:
        case F3dzex2Opcode.G_RDPHALF_1:
        case F3dzex2Opcode.G_SETOTHERMODE_L:
        case F3dzex2Opcode.G_SETOTHERMODE_H:
        case F3dzex2Opcode.G_RDPLOADSYNC:
        case F3dzex2Opcode.G_RDPPIPESYNC:
        case F3dzex2Opcode.G_RDPTILESYNC:
        case F3dzex2Opcode.G_RDPFULLSYNC:
        case F3dzex2Opcode.G_SETKEYGB:
        case F3dzex2Opcode.G_SETKEYR:
        case F3dzex2Opcode.G_SETSCISSOR:
        case F3dzex2Opcode.G_SETPRIMDEPTH:
        case F3dzex2Opcode.G_RDPSETOTHERMODE:
        case F3dzex2Opcode.G_LOADTLUT:
        case F3dzex2Opcode.G_RDPHALF_2:
        case F3dzex2Opcode.G_SETTILESIZE:
        case F3dzex2Opcode.G_LOADBLOCK:
        case F3dzex2Opcode.G_LOADTILE:
        case F3dzex2Opcode.G_FILLRECT:
        case F3dzex2Opcode.G_SETFILLCOLOR:
        case F3dzex2Opcode.G_SETFOGCOLOR:
        case F3dzex2Opcode.G_SETBLENDCOLOR:
        case F3dzex2Opcode.G_SETPRIMCOLOR:
        case F3dzex2Opcode.G_SETENVCOLOR:
        case F3dzex2Opcode.G_SETTIMG:
        case F3dzex2Opcode.G_SETZIMG:
        case F3dzex2Opcode.G_SETCIMG:
          return 1 * 2 * 4;
        case F3dzex2Opcode.G_BRANCH_Z:
        case F3dzex2Opcode.G_LOAD_UCODE:
        case F3dzex2Opcode.G_SETCONVERT:
        case F3dzex2Opcode.G_SETTILE:
        case F3dzex2Opcode.G_SETCOMBINE:
          return 2 * 2 * 4;
        case F3dzex2Opcode.G_TEXRECT:
        case F3dzex2Opcode.G_TEXRECTFLIP:
          return 3 * 2 * 4;
        default:
          throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
      }
    }
  }
}