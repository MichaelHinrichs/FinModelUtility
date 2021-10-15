using uni.platforms;
using uni.platforms.threeDs;

namespace uni.games.ocarina_of_time_3d {
  public class OcarinaOfTime3dExtractor {
    public void ExtractAll() {
      var ocarinaOfTime3dRom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetFile(
              "ocarina_of_time_3d.cia");

      var fileHierarchy =
          new ThreeDsFileHierarchyExtractor().ExtractFromRom(ocarinaOfTime3dRom);
    }
  }
}
