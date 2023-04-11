using System.IO;


namespace f3dzex2.displaylist.opcodes {
  public interface IOpcodeCommand { }

  public interface IOpcodeParser {
    IOpcodeCommand Parse(IEndianBinaryReader er);
  }
}