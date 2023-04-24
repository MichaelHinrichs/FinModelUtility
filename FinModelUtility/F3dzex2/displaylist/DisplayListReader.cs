﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using f3dzex2.displaylist.opcodes;
using f3dzex2.io;


namespace f3dzex2.displaylist {
  public interface IDisplayListReader {
    IDisplayList ReadDisplayList(
        IReadOnlyN64Memory n64Memory,
        IOpcodeParser opcodeParser,
        uint segmentedAddress);

    IReadOnlyList<IDisplayList> ReadPossibleDisplayLists(
        IReadOnlyN64Memory n64Memory,
        IOpcodeParser opcodeParser,
        uint segmentedAddress);

    IDisplayList ReadDisplayList(IReadOnlyN64Memory n64Memory,
                                 IOpcodeParser opcodeParser,
                                 IEndianBinaryReader er);
  }

  public class DisplayListReader : IDisplayListReader {
    public IDisplayList ReadDisplayList(IReadOnlyN64Memory n64Memory,
                                        IOpcodeParser opcodeParser,
                                        uint segmentedAddress)
      => this.ReadPossibleDisplayLists(n64Memory, opcodeParser, segmentedAddress)
             .Single();

    public IReadOnlyList<IDisplayList> ReadPossibleDisplayLists(
        IReadOnlyN64Memory n64Memory,
        IOpcodeParser opcodeParser,
        uint segmentedAddress) {
      var options = new LinkedList<IDisplayList>();
      foreach (var impl in n64Memory.OpenPossibilitiesAtSegmentedAddress(segmentedAddress)) {
        using var er = impl;
        options.AddLast(this.ReadDisplayList(n64Memory, opcodeParser, er));
      }
      return options.ToArray();
    }

    public IDisplayList ReadDisplayList(IReadOnlyN64Memory n64Memory,
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