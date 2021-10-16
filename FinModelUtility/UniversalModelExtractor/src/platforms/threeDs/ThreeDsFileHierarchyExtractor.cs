using System.Collections.Generic;
using System.Linq;

using fin.io;

using uni.platforms.threeDs.tools;
using uni.util.io;

namespace uni.platforms.threeDs {
  public class ThreeDsFileHierarchyExtractor {
    private readonly ZarExtractor zarExtractor_ = new();

    public IFileHierarchy ExtractFromRom(
        IFile romFile,
        ISet<string>? junkTerms = null) {
      new HackingToolkit9ds().Run(romFile, out var fileHierarchy);

      var didDecompress = false;
      fileHierarchy.ForEach(directory => {
        var zarFiles = directory.Files.Where(file => file.Extension == ".zar")
                                .ToArray();

        var didChange = false;
        foreach (var zarFile in zarFiles) {
          didChange |= this.zarExtractor_.Extract(zarFile);
        }

        if (didChange) {
          directory.Refresh();
        }
      });

      if (didDecompress) {
        fileHierarchy.Root.Refresh(true);
      }

      return fileHierarchy;
    }
  }
}