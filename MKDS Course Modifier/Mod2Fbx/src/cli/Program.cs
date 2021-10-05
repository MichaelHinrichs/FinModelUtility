using System.IO;

using fin.exporter.assimp;
using fin.exporter.gltf;
using fin.io;
using fin.util.asserts;

using mod.gcn;
using mod.gcn.animation;

namespace mod.cli {
  class Program {
    public static void Main(string[] args) {
      Program.GetForTesting(out var modFile, out var anmFile);

      using var s = new EndianBinaryReader(modFile.OpenRead());
      var mod = new Mod(s);

      for (var i = 0; i < mod.textures.Count; ++i) {
        var texture = mod.textures[i];
        texture.ToBitmap().Save(@"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\out\texture" + i + ".png");
      }

      Anm? anm = null;
      if (anmFile != null) {
        using var s2 = new EndianBinaryReader(anmFile.OpenRead());
        anm = new Anm();
        anm.Read(s2);
      }

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

    public static void GetForTesting(out IFile modFile, out IFile? anmFile) {
      modFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\beatle\beatle.mod");
      anmFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\beatle\beatle.anm");

      /*modFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\frog\frog.mod");
      anmFile = new FinFile(
          @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\frog\frog.anm");

      /*Program.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappy\"),
          out modFile,
          out anmFile);

      /*Program.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\bosses\pom\"),
          out modFile,
          out anmFile);

      /*Program.GetFromDirectory(
          new FinDirectory(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pkmn1.gcm_dir\dataDir\tekis\chappb\"),
          out modFile,
          out anmFile);*/
    }

    public static void GetFromDirectory(
        IDirectory directory,
        out IFile modFile,
        out IFile? anmFile) {
      modFile = Files.GetFileWithExtension(directory.Info, "mod");

      var anmFiles = Files.GetFilesWithExtension(directory.Info, "anm");
      Asserts.True(anmFiles.Length <= 1, "Found more than one anm file!");
      anmFile = anmFiles[0];
    }
  }
}