using System;
using System.IO;

using UoT.memory.files;
using UoT.memory.map;
using UoT.util.array;

namespace UoT {
  public enum Bank5Type {
    FIELD,
    DANGEON,
  }

  public interface IBank : IIndexable<byte> {
    byte Segment { get; }
  }

  public class RomBank : BIndexable, IBank {
    public byte Segment { get; set; }
    public void Resize(int size) => this.Region!.Resize(size);

    public IShardedMemory? Region { get; set; }
    public override int Count => this.Region?.Count ?? 0;

    public override byte this[int offset] {
      get => this.Region![offset];
      set => this.Region![offset] = value;
    }

    public void PopulateFromFile(string filename) {
      throw new NotSupportedException();
      //this.impl_ = File.ReadAllBytes(filename);
      //this.StartOffset = 0;
    }

    public void WriteToFile(string filename)
      => throw new NotSupportedException();
    // => File.WriteAllBytes(filename, this.impl_);

    public void WriteToStream(FileStream fs, int fsOffset) {
      throw new NotSupportedException();
      //fs.Position = fsOffset;
      //fs.Write(this.impl_, 0, this.Count);
    }
  }

  public static class RamBanks {
    public static void PopulateFromRomFiles(ZFiles romFiles) {
      RamBanks.ActiveBank5Type = Bank5Type.FIELD;

      foreach (var other in romFiles.Others) {
        var region = other.Region;

        switch (other.FileName) {
          case "gameplay_keep": {
            RamBanks.GameplayKeep.Region = region;
            break;
          }
          case "gameplay_field_keep": {
            RamBanks.GameplayFieldKeep.Region = region;
            break;
          }
          case "gameplay_dangeon_keep": {
            RamBanks.GameplayDangeonKeep.Region = region;
            break;
          }
          case "icon_item_static": {
            RamBanks.IconItemStatic.Region = region;
            break;
          }
          case "icon_item_24_static": {
            RamBanks.IconItem24Static.Region = region;
            break;
          }
        }
      }
    }


    public static RomBank ZFileBuffer { get; } = new RomBank();

    /// <summary>
    ///   Bank 2, Current Scene.
    /// </summary>
    public static RomBank ZSceneBuffer { get; } = new RomBank {Segment = 2};

    public static RomBank GameplayKeep { get; } = new RomBank {Segment = 4};


    public static Bank5Type ActiveBank5Type { get; set; }

    public static RomBank GameplayFieldKeep { get; } =
      new RomBank {Segment = 5};

    public static RomBank GameplayDangeonKeep { get; } =
      new RomBank {Segment = 5};


    /// <summary>
    ///   Bank 8, "icon_item_static". Contains animated textures, such as eyes,
    ///   mouths, etc.
    /// </summary>
    public static RomBank IconItemStatic { get; } = new RomBank {Segment = 8};

    /// <summary>
    ///   Bank 9, "icon_item_24_static". Contains animated textures.
    /// </summary>
    public static RomBank IconItem24Static { get; } = new RomBank {Segment = 9};


    public static int CurrentBank => RamBanks.ZFileBuffer.Segment;

    public static bool IsValidBank(byte bankIndex) {
      if (bankIndex == RamBanks.CurrentBank) {
        return true;
      }

      switch (bankIndex) {
        case 2:
          return RamBanks.ZSceneBuffer.Count > 0;
        case 4:
          return true;
        case 5:
          return true;

        default:
          return false;
      }
    }

    public static IBank? GetBankByIndex(uint bankIndex) {
      if (bankIndex == RamBanks.CurrentBank) {
        return RamBanks.ZFileBuffer;
      }

      switch (bankIndex) {
        // TODO: Support case 0, direct reference!
        // TODO: Support case 3, "Current Room". (?)
        case 2:
          return RamBanks.ZSceneBuffer;
        case 4:
          return RamBanks.GameplayKeep;
        case 5:
          return RamBanks.ActiveBank5Type == Bank5Type.FIELD
                     ? RamBanks.GameplayFieldKeep
                     : RamBanks.GameplayDangeonKeep;

        default:
          // TODO: Should throw an error for unsupported banks.
          return null;
      }
    }
  }
}