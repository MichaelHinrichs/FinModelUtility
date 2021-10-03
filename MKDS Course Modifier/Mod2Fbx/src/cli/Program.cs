using System.IO;

using fin.exporter.assimp;
using fin.exporter.gltf;
using fin.io;

using mod.gcn;
using mod.gcn.animation;

namespace mod.cli {
  class Program {
    public static void Main(string[] args) {
      var inPath =
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\chappy.mod";
      //var inPath =
      //    @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\pom\pom.mod";
      using var s = new EndianBinaryReader(File.OpenRead(inPath));
      var mod = new Mod(s);

      using var s2 = new EndianBinaryReader(File.OpenRead(@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\chappy.anm"));
      var anm = new Anm();
      anm.Read(s2);

      var model = ModelConverter.Convert(mod, anm);
      new GltfExporter().Export(
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\test.glb"),
          model);
      /*new AssimpIndirectExporter().Export(
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\test.fbx"),
          model);*/

    }
  }
}