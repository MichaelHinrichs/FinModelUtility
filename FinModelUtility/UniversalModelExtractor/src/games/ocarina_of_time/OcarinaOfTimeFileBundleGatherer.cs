using fin.io;
using fin.io.bundles;

using uni.platforms;

using UoT.api;


namespace uni.games.ocarina_of_time {
  public class OcarinaOfTimeFileBundleGatherer
      : IFileBundleGatherer<OcarinaOfTimeModelFileBundle> {
    public IEnumerable<OcarinaOfTimeModelFileBundle> GatherFileBundles(
        bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "ocarina_of_time.z64",
              out var ocarinaOfTimeRom)) {
        yield break;
      }

      var ocarinaOfTimeDirectory =
          DirectoryConstants.ROMS_DIRECTORY.GetSubdir("ocarina_of_time", true);
      var fileHierarchy = new FileHierarchy(ocarinaOfTimeDirectory);
      var root = fileHierarchy.Root;

      var zSegments = ZSegments.GetFiles(ocarinaOfTimeRom);
      foreach (var zObject in zSegments.Objects) {
        yield return new OcarinaOfTimeModelFileBundle(root,
          ocarinaOfTimeRom,
          zObject.FileName,
          zObject.Offset,
          zObject.Length);
      }
    }
  }
}