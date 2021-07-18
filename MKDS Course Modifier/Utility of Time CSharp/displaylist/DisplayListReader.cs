using System;
using System.Windows.Forms;

using Microsoft.VisualBasic;

using UoT.util;

namespace UoT {
  public static class DisplayListReader {
    // TODO: Remove combo box input.
    public static int ReadInDL(
        DlManager dlManager,
        uint address,
        ComboBox dListSelection) {
      IoUtil.SplitAddress(address, out var bank, out var offset);
      var data = Asserts.Assert(RamBanks.GetBankByIndex(bank));
      try {
        if (offset < data.Count) {
          // TODO: This jumps into the lowest level DL, but the 0xDE command (DL)
          // actually allows returning back up and calling more DLs. So this seems
          // like it will sometimes overlook any DLs that follow.
          // This should just be deleted and replaced w/ emulating in
          // F3DEX2_Parser.
          if (data[(int) offset] == 0xDE) {
            while (data[(int) offset] == 0xDE) {
              offset = IoUtil.ReadUInt24(data, (uint)(offset + 5L));
            }
          }

          int index = dlManager.Count;
          var displayList = new N64DisplayList();
          dlManager.Add(displayList);
          uint EPLoc = offset;
          dListSelection.Items.Add((index + 1).ToString() +
                                   ". " +
                                   Conversion.Hex(offset));
          displayList.StartPos = new ZSegment();
          displayList.StartPos.Offset = offset;
          displayList.StartPos.Bank = data.Segment;
          displayList.Skip = false;

          PickerUtil.NextRgb(out var r, out var g, out var b);
          displayList.PickCol = new Color3UByte {r = r, g = g, b = b};

          do {
            var commands = displayList.Commands;
            Array.Resize(ref commands, displayList.CommandCount + 1);
            displayList.Commands = commands;
            displayList.Commands[displayList.CommandCount] =
                new DLCommand(data, EPLoc);
            if (data[(int) EPLoc] == (int) F3DZEX.ENDDL | EPLoc >= data.Count) {
              EPLoc = (uint) (EPLoc + 8L);
              break;
            }

            EPLoc = (uint) (EPLoc + 8L);
            displayList.CommandCount += 1;
          } while (true);
          return (int) EPLoc;
        }
      } catch (Exception ex) {
        Interaction.MsgBox("Error reading in display list: " + ex.Message,
                           MsgBoxStyle.Critical,
                           "Exception");
        return default;
      }

      return default;
    }
  }
}