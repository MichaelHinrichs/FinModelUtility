using fin.data.queue;
using fin.io;
using fin.io.bundles;

using HaloWarsTools;

using hw.api;

using uni.platforms;
using uni.platforms.desktop;

namespace uni.games.halo_wars {
  public class HaloWarsModelFileGatherer 
      : IFileBundleGatherer<IHaloWarsModelFileBundle> {
    public IEnumerable<IHaloWarsModelFileBundle> GatherFileBundles(
        bool assert) {
      if (!SteamUtils.TryGetGameDirectory("HaloWarsDE",
                                          out var haloWarsSteamDirectory,
                                          assert)) {
        yield break;
      }

      var scratchDirectory = DirectoryConstants.ROMS_DIRECTORY
                                               .GetSubdir("halo_wars", true);
      
      var context = new HWContext(haloWarsSteamDirectory.FullName,
                                  scratchDirectory.FullName);
      // Expand all compressed/encrypted game files. This also handles the .xmb -> .xml conversion
      context.ExpandAllEraFiles();

      var fileHierarchy = new FileHierarchy(scratchDirectory);

      var mapDirectories =
          fileHierarchy.Root
                       .GetExistingSubdir("scenario/skirmish/design")
                       .Subdirs;
      foreach (var srcMapDirectory in mapDirectories) {
        var xtdFile = srcMapDirectory.FilesWithExtension(".xtd").Single();
        var xttFile = srcMapDirectory.FilesWithExtension(".xtt").Single();
        yield return new XtdModelFileBundle(xtdFile, xttFile);
      }

      var artDirectory = fileHierarchy.Root.GetExistingSubdir("art");
      var artSubdirQueue = new FinQueue<IFileHierarchyDirectory>(artDirectory);
      // TODO: Switch to DFS instead, it's more intuitive as a user
      while (artSubdirQueue.TryDequeue(out var artSubdir)) {
        // TODO: Skip a file if it's already been extracted
        // TODO: Parse UGX files instead, as long as they specify their own animations
        var visFiles = artSubdir.FilesWithExtension(".vis");
        foreach (var visFile in visFiles) {
          yield return new VisModelFileBundle(visFile, context);
        }

        artSubdirQueue.Enqueue(artSubdir.Subdirs);
      }
    }
  }
}