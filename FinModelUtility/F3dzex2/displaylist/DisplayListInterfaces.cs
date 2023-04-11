using System.Collections.Generic;

using f3dzex2.displaylist.opcodes;

namespace f3dzex2.displaylist {
  public interface IDisplayList {
    IReadOnlyList<IOpcodeCommand> OpcodeCommands { get; set; }
  }
}