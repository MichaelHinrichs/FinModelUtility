using System.Collections.Generic;

using f3dzex2.displaylist.opcodes;
using f3dzex2.io;

namespace f3dzex2.displaylist {
  public class DisplayListReader {
    public IDisplayList ReadDisplayList(
        IBankManager bankManager,
        uint address) {
      // TODO: Support branching offsets.
      IoUtil.SplitAddress(address, out var bankIndex, out var offset);
      var bankReader = bankManager[bankIndex];
      bankReader.Position = offset;

      var displayList = new DisplayList();
      /*IDisplayListInstruction? instruction = null;

      while (true) {
        var low = bankReader.ReadUInt32();
        var high = bankReader.ReadUInt32();

        var nextInstruction =
            new DisplayListInstruction(bankReader.Position,
                                       low,
                                       high);

        if (instruction == null) {
          displayList.Root = nextInstruction;
        } else  {
          instruction.NextSibling = nextInstruction;
        }
        instruction = nextInstruction;

        if (instruction.Opcode == F3dzex2Opcode.G_ENDDL) {
          break;
        }

        if (instruction.Opcode == F3dzex2Opcode.G_DL) {
          address = high;

          // TODO: Support branching offsets.
          IoUtil.SplitAddress(address, out bankIndex, out offset);
          bankReader = bankManager[bankIndex];
          bankReader.Position = offset;
        }
      }*/

      return displayList;
    }

    private class DisplayList : IDisplayList {
      public IReadOnlyList<IOpcodeCommand> OpcodeCommands { get; set; }
    }
  }
}