using fin.io;

using uni.platforms.threeDs.tools;


namespace uni.platforms.threeDs {
  public class ThreeDsFileHierarchyExtractor {
    private readonly ZarExtractor zarExtractor_ = new();
    private readonly GarExtractor garExtractor_ = new();

    public IFileHierarchy ExtractFromRom(
        IFile romFile,
        ISet<string>? junkTerms = null) {
      new HackingToolkit9ds().Run(romFile, out var fileHierarchy);

      var didDecompress = false;
      foreach (var directory in fileHierarchy) {
        var didChange = false;
        foreach (var zarFile in directory.FilesWithExtension(".zar")) {
          didChange |= this.zarExtractor_.Extract(zarFile);
        }
        foreach (var garFile in directory.FilesWithExtension(".gar")) {
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