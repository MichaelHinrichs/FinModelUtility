using fin.io;
using fin.io.archive;
using fin.util.strings;

using uni.platforms.threeDs.tools;
using uni.platforms.threeDs.tools.ctrtool;


namespace uni.platforms.threeDs {
  public class ThreeDsFileHierarchyExtractor {
    public bool TryToExtractFromGame(string gameName,
                                     out IFileHierarchy fileHierarchy) {
      if (!this.TryToFindRom_(gameName, out var romFile)) {
        fileHierarchy = default;
        return false;
      }

      fileHierarchy = this.ExtractFromRom_(romFile);
      return true;
    }

    private bool TryToFindRom_(string gameName, out ISystemFile romFile)
      => DirectoryConstants.ROMS_DIRECTORY
                           .TryToGetExistingFileWithExtension(
                               gameName,
                               out romFile,
                               ".cci",
                               ".3ds",
                               ".cia");

    private IFileHierarchy ExtractFromRom_(ISystemFile romFile) {
      IFileHierarchy fileHierarchy;
      switch (romFile.Extension) {
        case ".cia": {
          new Ctrtool.CiaExtractor().Run(romFile, out fileHierarchy);
          break;
        }
        case ".3ds":
        case ".cci": {
          new Ctrtool.CciExtractor().Run(romFile, out fileHierarchy);
          break;
        }
        default: throw new NotSupportedException();
      }

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
          didChange |=
              archiveExtractor.TryToExtractIntoNewDirectory<GarReader>(
                  garFile,
                  new FinDirectory(garFile.FullNameWithoutExtension)) ==
              ArchiveExtractionResult.NEWLY_EXTRACTED;
        }

        foreach (var garFile in directory.Files.Where(
                     file => file.Name.EndsWith(".gar.lzs"))) {
          didChange |=
              archiveExtractor.TryToExtractIntoNewDirectory<GarReader>(
                  garFile,
                  new FinDirectory(
                      StringUtil.UpTo(garFile.FullName, ".gar"))) ==
              ArchiveExtractionResult.NEWLY_EXTRACTED;
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