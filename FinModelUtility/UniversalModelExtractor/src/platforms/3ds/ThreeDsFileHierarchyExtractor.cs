using System.Collections.Generic;

using fin.io;

using uni.platforms.threeDs.tools;
using uni.util.io;

namespace uni.platforms.threeDs {
  public class ThreeDsFileHierarchyExtractor {
    public IFileHierarchy ExtractFromRom(
        IFile romFile,
        ISet<string>? junkTerms = null) {
      new HackingToolkit9ds().Run(romFile, out var fileHierarchy);
      return fileHierarchy;
    }
  }
}