using System;
using System.IO;


namespace f3dzex2.displaylist.opcodes {
  public class F3dOpcodeParser : IOpcodeParser {
    public IOpcodeCommand Parse(IEndianBinaryReader er) {
      var baseOffset = er.Position;
      var opcode = (F3dOpcode) er.ReadByte();

      IOpcodeCommand opcodeCommand;
      switch (opcode) {
        case F3dOpcode.G_NOOP:
        case F3dOpcode.G_SPNOOP:
        default: {
          opcodeCommand = new NoopOpcodeCommand();
          break;
        }
      }

      er.Position = baseOffset + GetCommandLength_(opcode);
      return opcodeCommand;
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
        default: throw new ArgumentOutOfRangeException(nameof(opcode), opcode, null);
      }
    }
  }
}
