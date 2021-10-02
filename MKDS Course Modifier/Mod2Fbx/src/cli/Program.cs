using System.IO;

using fin.exporter.assimp;
using fin.exporter.gltf;
using fin.io;

using mod.gcn;

namespace mod.cli {
  class Program {
    public static void Main(string[] args) {
      /*var inPath =
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\chappy.mod";*/

      var inPath =
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\kogane\kogane.mod";
      using var s = new EndianBinaryReader(File.OpenRead(inPath));

      var mod = new Mod(s);
      //ExportToObj.Export(mod);

      var model = ModelConverter.Convert(mod);
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