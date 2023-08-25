using fin.io;
using fin.io.bundles;
using fin.model;

using uni.platforms;
using uni.util.io;

using xmod.api;

namespace uni.games.midnight_club_2 {
  public class MidnightClub2FileGatherer
      : IFileBundleGatherer<IModelFileBundle> {
    public IEnumerable<IModelFileBundle> GatherFileBundles(
        bool assert) {
      if (!DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingSubdir(
              "midnight_club_2",
              out var midnightClub2Directory)) {
        return Enumerable.Empty<IModelFileBundle>();
      }

      var fileHierarchy = new FileHierarchy(midnightClub2Directory);

      return new FileHierarchyAssetBundleSeparator<IModelFileBundle>(
          fileHierarchy,
          subdir => subdir.FilesWithExtension(".xmod")
                          .Select(file => new XmodModelFileBundle {
                              GameName = "midnight_club_2",
                              XmodFile = file,
                          })
                          .Concat<IModelFileBundle>(
                              subdir.FilesWithExtension(".ped")
                                    .Select(
                                        file => new PedModelFileBundle {
                                            PedFile = file,
                                        }))).GatherFileBundles(assert);
    }
  }
}