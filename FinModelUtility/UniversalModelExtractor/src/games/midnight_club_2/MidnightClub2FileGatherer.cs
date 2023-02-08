using fin.io;
using fin.io.bundles;
using fin.model;

using uni.platforms;
using uni.util.io;

using xmod.api;


namespace uni.games.midnight_club_2 {
  public class MidnightClub2FileGatherer
      : IFileBundleGatherer<IModelFileBundle> {
    public IFileBundleDirectory<IModelFileBundle>? GatherFileBundles(
        bool assert) {
      var midnightClub2Directory =
          DirectoryConstants.ROMS_DIRECTORY.GetSubdir("midnight_club_2");
      if (!midnightClub2Directory.Exists) {
        return null;
      }

      var fileHierarchy = new FileHierarchy(midnightClub2Directory);

      return new FileHierarchyBundler<IModelFileBundle>(
          subdir => subdir.FilesWithExtension(".xmod")
                          .Select(file => new XmodModelFileBundle {
                              XmodFile = file,
                          })
                          .Concat<IModelFileBundle>(
                              subdir.FilesWithExtension(".ped")
                                    .Select(
                                        file => new PedModelFileBundle {
                                            PedFile = file,
                                        }))).GatherBundles(
          fileHierarchy);
    }
  }
}