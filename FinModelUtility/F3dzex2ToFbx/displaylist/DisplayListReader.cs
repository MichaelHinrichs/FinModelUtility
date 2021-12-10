using f3dzex.io;

using fin.util.optional;

namespace f3dzex.displaylist {
  public class DisplayListReader {
    public IDisplayList ReadDisplayList(
        IBankManager bankManager,
        uint address) {
      // TODO: Support branching offsets.
      IoUtil.SplitAddress(address, out var bankIndex, out var offset);
      var bankReader = bankManager[bankIndex];
      bankReader.Position = offset;

      var displayList = new DisplayList();
      IDisplayListInstruction? instruction = null;

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
          instruction.NextSibling =
              Optional<IDisplayListInstruction>.Of(nextInstruction);
        }
        instruction = nextInstruction;

        if (instruction.Opcode == F3dzexOpcode.ENDDL) {
          break;
        }

        if (instruction.Opcode == F3dzexOpcode.DL) {
          address = high;

          // TODO: Support branching offsets.
          IoUtil.SplitAddress(address, out bankIndex, out offset);
          bankReader = bankManager[bankIndex];
          bankReader.Position = offset;
        }
      }

      return displayList;
    }

    private class DisplayList : IDisplayList {
      public IDisplayListInstruction Root { get; set; }
    }

    private class DisplayListInstruction : IDisplayListInstruction {
      public DisplayListInstruction(
          long address,
          uint low,
          uint high) {
        this.Address = address;
        this.Low = low;
        this.High = high;
      }

      public long Address { get; }
      public uint Low { get; }
      public uint High { get; }

      public Optional<IDisplayListInstruction> FirstChild { get; set; }
      public Optional<IDisplayListInstruction> NextSibling { get; set; }
    }
  }
}