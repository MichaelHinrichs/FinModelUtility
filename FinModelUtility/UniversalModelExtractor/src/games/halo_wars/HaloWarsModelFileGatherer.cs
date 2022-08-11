using fin.data.queue;
using fin.exporter.assimp.indirect;
using fin.io;
using fin.model;

using HaloWarsTools;

using hw.api;

using uni.platforms;
using uni.platforms.desktop;


namespace uni.games.halo_wars {
  public class HaloWarsModelFileGatherer : IModelFileGatherer<
      IHaloWarsModelFileBundle> {
    public IModelDirectory<IHaloWarsModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var haloWarsSteamDirectory =
          SteamUtils.GetGameDirectory("HaloWarsDE", assert);
      if (haloWarsSteamDirectory == null) {
        return null;
      }

      var scratchDirectory = DirectoryConstants.ROMS_DIRECTORY
                                               .GetSubdir("halo_wars", true);
      
      var context = new HWContext(haloWarsSteamDirectory.FullName,
                                  scratchDirectory.FullName);
      // Expand all compressed/encrypted game files. This also handles the .xmb -> .xml conversion
      context.ExpandAllEraFiles();

      var fileHierarchy = new FileHierarchy(scratchDirectory);

      var rootNode = new ModelDirectory<IHaloWarsModelFileBundle>("halo_wars");

      var mapDirectories =
          fileHierarchy.Root
                       .TryToGetSubdir("scenario/skirmish/design")
                       .Subdirs;
      foreach (var srcMapDirectory in mapDirectories) {
        var xtdFile = srcMapDirectory.FilesWithExtension(".xtd").Single();
        var xttFile = srcMapDirectory.FilesWithExtension(".xtt").Single();
        rootNode.AddFileBundleRelative(
            new XtdModelFileBundle(xtdFile, xttFile, context));
      }

      var artDirectory = fileHierarchy.Root.TryToGetSubdir("art");
      var artSubdirQueue = new FinQueue<IFileHierarchyDirectory>(artDirectory);
      // TODO: Switch to DFS instead, it's more intuitive as a user
      while (artSubdirQueue.TryDequeue(out var artSubdir)) {
        // TODO: Skip a file if it's already been extracted
        // TODO: Parse UGX files instead, as long as they specify their own animations
        var visFiles = artSubdir.FilesWithExtension(".vis");
        foreach (var visFile in visFiles) {
          rootNode.AddFileBundleRelative(new VisModelFileBundle(visFile, context));
        }

        artSubdirQueue.Enqueue(artSubdir.Subdirs);
      }

      return rootNode;
    }
  }
}