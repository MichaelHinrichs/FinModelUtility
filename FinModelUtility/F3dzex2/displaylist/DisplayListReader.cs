using System.Collections.Generic;
using System.IO;
using System.Linq;

using f3dzex2.displaylist.opcodes;
using f3dzex2.io;


namespace f3dzex2.displaylist {
  public interface IDisplayListReader {
    IDisplayList ReadDisplayList(
        IN64Memory n64Memory,
        IOpcodeParser opcodeParser,
        uint address);

    IReadOnlyList<IDisplayList> ReadPossibleDisplayLists(
        IN64Memory n64Memory,
        IOpcodeParser opcodeParser,
        uint address);

    IDisplayList ReadDisplayList(IN64Memory n64Memory,
                                 IOpcodeParser opcodeParser,
                                 IEndianBinaryReader er);
  }

  public class DisplayListReader : IDisplayListReader {
    public IDisplayList ReadDisplayList(IN64Memory n64Memory,
                                        IOpcodeParser opcodeParser,
                                        uint address)
      => this.ReadPossibleDisplayLists(n64Memory, opcodeParser, address)
             .Single();

    public IReadOnlyList<IDisplayList> ReadPossibleDisplayLists(
        IN64Memory n64Memory,
        IOpcodeParser opcodeParser,
        uint address) {
      var options = new LinkedList<IDisplayList>();
      foreach (var impl in n64Memory.OpenPossibilitiesAtAddress(address)) {
        using var er = impl;
        options.AddLast(this.ReadDisplayList(n64Memory, opcodeParser, er));
      }
      return options.ToArray();
    }

    public IDisplayList ReadDisplayList(IN64Memory n64Memory,
                                        IOpcodeParser opcodeParser,
                                        IEndianBinaryReader er) {
      var opcodeCommands = new LinkedList<IOpcodeCommand>();
      while (true) {
        var opcodeCommand = opcodeParser.Parse(n64Memory, this, er);
        opcodeCommands.AddLast(opcodeCommand);

        if (opcodeCommand is DlOpcodeCommand {PushCurrentDlToStack: false}) {
          break;
        }
        if (opcodeCommand is EndDlOpcodeCommand) {
          break;
        }
      }
      return new DisplayList {
          OpcodeCommands = opcodeCommands.ToArray(),
          Type = opcodeParser.Type,
      };
    }

    private class DisplayList : IDisplayList {
      public required IReadOnlyList<IOpcodeCommand> OpcodeCommands { get; init; }
      public required DisplayListType Type { get; init; }
    }
  }
}