using fin.io;
using fin.model;

using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using mod.cli;

using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.pikmin_1 {
  public class
      Pikmin1ModelFileGatherer : IModelFileGatherer<ModModelFileBundle> {
    public IModelDirectory<ModModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var pikmin1Rom =
          DirectoryConstants.ROMS_DIRECTORY.GetExistingFile(
              "pikmin_1.gcm");

      var options = GcnFileHierarchyExtractor.Options.Empty();
      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(options, pikmin1Rom);

      var rootModelDirectory =
          new ModelDirectory<ModModelFileBundle>("pikmin_1");
      var parentObjectDirectory = rootModelDirectory.AddSubdir("data")
                                                    .AddSubdir("objects");

      var queue =
          new Queue<(IFileHierarchyDirectory,
              IModelDirectory<ModModelFileBundle>)>();
      queue.Enqueue((fileHierarchy.Root, rootModelDirectory));
      while (queue.Any()) {
        var (directory, node) = queue.Dequeue();

        // TODO: Handle special cases:
        // - olimar
        // - pikmin
        // - frog

        var modFiles = directory.FilesWithExtension(".mod").ToArray();
        var anmFiles = directory.FilesWithExtension(".anm").ToArray();
        foreach (var modFile in modFiles) {
          var anmFile =
              anmFiles
                  .FirstOrDefault(anmFile => anmFile.NameWithoutExtension ==
                                             modFile.NameWithoutExtension);

          node.AddFileBundle(new ModModelFileBundle {
              ModFile = modFile.Impl,
              AnmFile = anmFile?.Impl,
          });
        }

        foreach (var subdir in directory.Subdirs) {
          queue.Enqueue((subdir, node.AddSubdir(subdir.Name)));
        }
      }

      return rootModelDirectory;
    }
  }
}