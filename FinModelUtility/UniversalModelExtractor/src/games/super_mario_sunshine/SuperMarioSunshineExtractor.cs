using System.Linq;

using fin.io;

using uni.platforms;
using uni.platforms.gcn;
using uni.platforms.gcn.tools;

namespace uni.games.super_mario_sunshine {
  public class SuperMarioSunshineExtractor {
    public void ExtractAll() {
      var superMarioSunshineRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "super_mario_sunshine.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(superMarioSunshineRom);

      var bmdFile =
          new FinFile(
              @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\super_mario_sunshine.gcm_dir\data\mario.szs 0.rarc_dir\mario\bmd\ma_mdl1.bmd");

      //new Bmd2Fbx().Run(bmdFile, null, null, fileHierarchy.Root.Impl);

      /*foreach (var fileHierarchyDirectory in fileHierarchy) {

        var files = fileHierarchyDirectory.Files;
        var hasSimpleModel =
            files.Any(file => file.Name.EndsWith("_model.bmd"));

        if (hasSimpleModel) {

        }
      }*/
    }
  }
}