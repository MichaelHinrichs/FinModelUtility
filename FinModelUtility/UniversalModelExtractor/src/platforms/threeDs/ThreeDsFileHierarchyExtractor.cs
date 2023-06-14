using fin.io;
using fin.io.archive;

using uni.platforms.threeDs.tools;


namespace uni.platforms.threeDs {
  public class ThreeDsFileHierarchyExtractor {
    private readonly GarExtractor garExtractor_ = new();

    public IFileHierarchy ExtractFromRom(
        ISystemFile romFile,
        ISet<string>? junkTerms = null) {
      new HackingToolkit9ds().Run(romFile, out var fileHierarchy);

      var archiveExtractor = new SubArchiveExtractor();

      var didDecompress = false;
      foreach (var directory in fileHierarchy) {
        var didChange = false;
        foreach (var zarFile in directory.FilesWithExtension(".zar")) {
          didChange |=
              archiveExtractor.TryToExtractIntoNewDirectory<ZarReader>(
                  zarFile,
                  new FinDirectory(zarFile.FullNameWithoutExtension)) ==
              ArchiveExtractionResult.NEWLY_EXTRACTED;
        }
        foreach (var garFile in directory.FilesWithExtension(".gar")) {
          didChange |= this.garExtractor_.Extract(garFile);
        }
        foreach (var garFile in directory.Files.Where(file => file.Name.EndsWith(".gar.lzs"))) {
          didChange |= this.garExtractor_.Extract(garFile);
        }

        if (didChange) {
          directory.Refresh();
        }
      }

      if (didDecompress) {
        fileHierarchy.Root.Refresh(true);
      }

      return fileHierarchy;
    }
  }
}