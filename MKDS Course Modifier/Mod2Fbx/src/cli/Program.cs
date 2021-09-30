using System.IO;

using mod.gcn;

namespace mod.cli {
  class Program {
    public static void Main(string[] args) {
      var inPath =
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\chappy.mod";
      using var s = new EndianBinaryReader(File.OpenRead(inPath));

      //var mod = new Mod(s);
      //ExportToObj.ExportToObj(mod);
    }
  }
}