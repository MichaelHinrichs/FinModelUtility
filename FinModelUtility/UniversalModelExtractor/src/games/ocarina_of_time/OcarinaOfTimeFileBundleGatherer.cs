using fin.io;
using fin.io.bundles;

using uni.platforms;

using UoT.api;
using UoT.memory;


namespace uni.games.ocarina_of_time {
  public class OcarinaOfTimeFileBundleGatherer
      : IFileBundleGatherer<OotModelFileBundle> {
    public IEnumerable<OotModelFileBundle> GatherFileBundles(
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
        yield return new OotModelFileBundle(root,
                                            ocarinaOfTimeRom,
                                            zObject.FileName,
                                            zObject.Offset,
                                            zObject.Length);
      }

      var gameplayKeep =
          zSegments.Others.Single(other => other.FileName is "gameplay_keep");
      Segments.GAMEPLAY_KEEP = new Segment {
          Offset = gameplayKeep.Offset, Length = gameplayKeep.Length
      };

      var gameplayFieldKeep =
          zSegments.Others.Single(
              other => other.FileName is "gameplay_field_keep");
      Segments.GAMEPLAY_FIELD_KEEP = new Segment {
          Offset = gameplayFieldKeep.Offset, Length = gameplayFieldKeep.Length
      };
    }
  }
}