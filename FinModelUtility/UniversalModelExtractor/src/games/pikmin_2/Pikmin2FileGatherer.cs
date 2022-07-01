using bmd.exporter;

using fin.io;
using fin.log;
using fin.model;
using fin.util.asserts;

using uni.msg;
using uni.platforms;
using uni.platforms.gcn;


namespace uni.games.pikmin_2 {
  public class Pikmin2FileGatherer : IModelFileGatherer<BmdModelFileBundle> {
    private readonly ILogger logger_ = Logging.Create<Pikmin2Extractor>();

    public IModelDirectory<BmdModelFileBundle>? GatherModelFileBundles(
        bool assert) {
      var pikmin2Rom =
          DirectoryConstants.ROMS_DIRECTORY.TryToGetExistingFile(
              "pikmin_2.gcm");
      if (pikmin2Rom == null) {
        return null;
      }

      var options = GcnFileHierarchyExtractor.Options.Standard()
                                             .PruneRarcDumpNames("arc", "data");
      var fileHierarchy =
          new GcnFileHierarchyExtractor().ExtractFromRom(
              options,
              pikmin2Rom);

      var rootNode = new ModelDirectory<BmdModelFileBundle>("pikmin_2");

      this.ExtractPikminAndCaptainModels_(rootNode, fileHierarchy);
      this.ExtractAllFromSeparateDirectories_(rootNode, fileHierarchy);
      this.ExtractAllFromMergedDirectories_(rootNode, fileHierarchy);
      this.ExtractLeafBudFlower_(rootNode, fileHierarchy);
      this.ExtractAllTreasures_(rootNode, fileHierarchy);

      return rootNode;
    }

    /// <summary>
    ///   Gets from separate model/animation szs (e.g. Enemies)
    /// </summary>
    private void ExtractAllFromSeparateDirectories_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IFileHierarchy fileHierarchy) {
      foreach (var subdir in fileHierarchy) {
        var modelSubdir =
            subdir.Subdirs.SingleOrDefault(dir => dir.Name == "model");
        var animSubdir =
            subdir.Subdirs.SingleOrDefault(dir => dir.Name == "anim");

        if (modelSubdir != null && animSubdir != null) {
          var bmdFiles = modelSubdir.Files;
          var bcxFiles = animSubdir.Files;
          var btiFiles = subdir.FilesWithExtensionRecursive(".bti").ToArray();

          this.ExtractModels_(parentNode.AddSubdir(subdir.Name), bmdFiles,
                              bcxFiles, btiFiles);
        }
      }
    }

    /// <summary>
    ///   Gets from model/animations in same szs (e.g. user\Kando)
    /// </summary>
    private void ExtractAllFromMergedDirectories_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IFileHierarchy fileHierarchy) {
      foreach (var subdir in fileHierarchy) {
        var arcSubdir =
            subdir.Subdirs.SingleOrDefault(dir => dir.Name == "arc");

        if (arcSubdir != null &&
            arcSubdir.FilesWithExtension(".bmd").Any()) {
          this.ExtractModelsInDirectoryAutomatically_(
              parentNode.AddSubdir(subdir.Name), arcSubdir);
        }
      }
    }

    private void ExtractPikminAndCaptainModels_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IFileHierarchy fileHierarchy) {
      var pikminAndCaptainBaseDirectory =
          fileHierarchy.Root.TryToGetSubdir(
              @"user\Kando\piki\pikis_designer");

      var bcxFiles =
          pikminAndCaptainBaseDirectory.TryToGetSubdir("motion")
                                       .Files;

      var captainSubdir =
          pikminAndCaptainBaseDirectory.TryToGetSubdir("orima_model");
      this.ExtractModels_(parentNode.AddSubdir("captains"),
                          captainSubdir.Files,
                          bcxFiles);

      var pikminSubdir =
          pikminAndCaptainBaseDirectory.TryToGetSubdir("piki_model");
      this.ExtractModels_(parentNode.AddSubdir("pikmin"),
                          pikminSubdir.Files,
                          bcxFiles);
    }

    private void ExtractAllTreasures_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IFileHierarchy fileHierarchy) {
      var treasureBaseDirectory =
          fileHierarchy.Root.TryToGetSubdir(@"user\Abe\Pellet");

      var baseDirectory = parentNode.AddSubdir("treasures");

      foreach (var locale in treasureBaseDirectory.Subdirs) {
        var localeNode = baseDirectory.AddSubdir(locale.Name);

        foreach (var treasure in locale.Subdirs) {
          var treasureNode = localeNode.AddSubdir(treasure.Name);

          var bmdFiles = treasure.Files.Where(file => file.Extension == ".bmd")
                                 .ToArray();
          if (bmdFiles.Length > 0) {
            var bcxFiles =
                treasure.Files
                        .Where(file => file.Extension == ".bca" ||
                                       file.Extension == ".bck")
                        .ToList();
            this.ExtractModels_(treasureNode, bmdFiles, bcxFiles);
          }
        }
      }
    }

    private void ExtractLeafBudFlower_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IFileHierarchy fileHierarchy) {
      var leafBudFlowerDirectory =
          fileHierarchy.Root.TryToGetSubdir(
              @"user\Kando\piki\pikis_designer\happa_model");
      this.ExtractModelsInDirectoryAutomatically_(
          parentNode.AddSubdir("leaf_bud_flower"), leafBudFlowerDirectory);
    }

    private void ExtractModelsInDirectoryAutomatically_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IFileHierarchyDirectory directory) {
      var bmdFiles = directory.FilesWithExtension(".bmd").ToArray();
      if (bmdFiles.Length > 0) {
        var bcxFiles = directory.FilesWithExtensions(".bca", ".bck")
                                .ToList();
        var btiFiles = directory.FilesWithExtension(".bti").ToList();
        this.ExtractModels_(parentNode, bmdFiles, bcxFiles, btiFiles);
      }
    }

    private void ExtractModels_(
        IModelDirectory<BmdModelFileBundle> parentNode,
        IReadOnlyList<IFileHierarchyFile> bmdFiles,
        IReadOnlyList<IFileHierarchyFile>? bcxFiles = null,
        IReadOnlyList<IFileHierarchyFile>? btiFiles = null
    ) {
      Asserts.True(bmdFiles.Count > 0);

      foreach (var bmdFile in bmdFiles) {
        parentNode.AddFileBundle(
            new BmdModelFileBundle {
                BmdFile = bmdFile,
                BcxFiles = bcxFiles,
                BtiFiles = btiFiles,
            });
      }
    }
  }
}