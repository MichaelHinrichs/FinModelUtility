using System.IO;

using f3dzex2.io;


namespace f3dzex2.displaylist.opcodes {
  public interface IOpcodeCommand { }

  public interface IOpcodeParser {
    IOpcodeCommand Parse(IN64Memory n64Memory,
                         IDisplayListReader dlr,
                         IEndianBinaryReader er);
  }
}