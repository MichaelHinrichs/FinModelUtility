using fin.io;

using uni.platforms;
using uni.platforms.gcn;
using uni.platforms.gcn.tools;

namespace uni.games.pikmin_2 {
  public class Pikmin2Extractor {
    public void ExtractAll() {
      var pikmin2Rom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "pikmin_2.gcm");

      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(pikmin2Rom);

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