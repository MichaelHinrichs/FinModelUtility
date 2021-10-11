using System.Collections.Generic;

using fin.io;

using uni.platforms.gcn.tools;
using uni.util.io;

namespace uni.platforms.gcn {
  public class GcnFileHierarchyExtractor {
    private readonly GcmDump gcmDump_ = new();
    private readonly Yaz0Dec yaz0Dec_ = new();
    private readonly RarcDump rarcDump_ = new();

    public IFileHierarchy ExtractFromRom(IFile romFile) {
      IFileHierarchy fileHierarchy;
      this.gcmDump_.Run(romFile, out fileHierarchy);

      var baseRomDirectory = fileHierarchy.Root.Impl;

      // Decompresses all of the archives
      var directoryQueue = new Queue<IFileHierarchyDirectory>();
      directoryQueue.Enqueue(fileHierarchy.Root);
      while (directoryQueue.Count > 0) {
        var directory = directoryQueue.Dequeue();

        // Converts any SZS files into RARC files.
        var didDecrypt = false;
        foreach (var file in directory.Files) {
          if (file.Extension == ".szs") {
            didDecrypt |= this.yaz0Dec_.Run(file, baseRomDirectory);
          }
        }

        // Updates to see any new RARC files.
        if (didDecrypt) {
          directory.Refresh();
        }

        // Extracts contents of any RARC files.
        var didDump = false;
        foreach (var file in directory.Files) {
          if (file.Extension == ".rarc") {
            didDump |= this.rarcDump_.Run(file, baseRomDirectory);
          }
        }

        // Updates to see any new extracted directories.
        if (didDump) {
          directory.Refresh();
        }

        foreach (var subdir in directory.Subdirs) {
          directoryQueue.Enqueue(subdir);
        }
      }

      return fileHierarchy;
    }
  }
}