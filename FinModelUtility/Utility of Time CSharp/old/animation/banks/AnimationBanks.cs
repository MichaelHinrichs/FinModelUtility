using System.Collections.Generic;

using UoT.memory.files;

namespace UoT.animation.banks {
  public interface IAnimationBank {
    string Name { get; }

    RomBank? Bank { get; }
  }

  public static class AnimationBanks {
    public static IReadOnlyList<IAnimationBank>? Banks { get; private set; }

    public static void PopulateFromRomFiles(ZFiles romFiles) {
      var banks = new List<IAnimationBank>();

      banks.Add(new AnimationBank("Inline with model"));

      foreach (var other in romFiles.Others) {
        var region = other.Region;

        if (other.FileName == "link_animetion") {
          banks.Add(new AnimationBank(other.FileName,
                                      new RomBank {Region = region}));
        }
      }

      foreach (var obj in romFiles.Objects) {
        var fileName = obj!.FileName!.ToLower();
        var region = obj.Region;

        if (fileName.Contains("object_") && fileName.Contains("_anime")) {
          banks.Add(new AnimationBank(obj.BetterFileName!,
                                      new RomBank { Region = region }));
        }
      }

      AnimationBanks.Banks = banks;
    }

    private class AnimationBank : IAnimationBank {
      public AnimationBank(string name, RomBank? bank = null) {
        this.Name = name;
        this.Bank = bank;
      }

      public string Name { get; }

      public RomBank? Bank { get; }
    }
  }
}