using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.io;
using fin.util.asserts;

using uni.platforms.gcn.tools;
using uni.util.io;

namespace uni.platforms.gcn {
  public class GcnFileHierarchyExtractor {
    private readonly GcmDump gcmDump_ = new();
    private readonly Yaz0Dec yaz0Dec_ = new();
    private readonly RarcDump rarcDump_ = new();

    public IFileHierarchy ExtractFromRom(IFile romFile) {
      this.gcmDump_.Run(romFile, out var fileHierarchy);

      // Decompresses all of the archives
      foreach (var fileHierarchyDirectory in fileHierarchy) {
        // Converts any SZS files into RARC files.
        var szsFiles =
            fileHierarchyDirectory.Files
                                  .Where(file => file.Extension == ".szs")
                                  .ToArray();
        var didDecrypt = false;
        foreach (var szsFile in szsFiles) {
          didDecrypt |= this.yaz0Dec_.Run(szsFile);
        }

        // Updates to see any new RARC files.
        if (didDecrypt) {
          fileHierarchyDirectory.Refresh();
        }


        // Extracts contents of any RARC files.
        var rarcFiles =
            fileHierarchyDirectory.Files
                                  .Where(file => file.Extension == ".rarc")
                                  .ToArray();
        var didDump = false;
        foreach (var rarcFile in rarcFiles) {
          didDump |= this.rarcDump_.Run(rarcFile);
        }

        // Updates to see any new extracted directories.
        if (didDump) {
          fileHierarchyDirectory.Refresh();
        }


        // Cleans up any SZSs/RARCs where possible.
        var rarcSubdirs =
            fileHierarchyDirectory.Subdirs
                                  .Where(
                                      subdir => subdir.Name.EndsWith(
                                          ".rarc_dir"))
                                  .ToArray();
        var didClean = false;
        foreach (var rarcSubdir in rarcSubdirs) {
          if (rarcSubdir.Subdirs.Count == 1) {
            var subdir = rarcSubdir.Subdirs[0];

            var subdirName = subdir.Name;

            var rarcPath =
                rarcSubdir.FullName.Substring(0,
                                              rarcSubdir.FullName.Length -
                                              "_dir".Length);
            Asserts.True(rarcPath.EndsWith(".rarc"));
            var szsPath =
                rarcPath.Substring(0,
                                   rarcPath.Length - " 0.rarc".Length);
            Asserts.True(szsPath.EndsWith(".szs"));

            var junkIndex = rarcSubdir.Name.IndexOf(".szs 0.rarc_dir");
            Asserts.True(junkIndex != -1, "Unsupported rarc directory name!");

            var rarcSubdirName = rarcSubdir.Name.Substring(0, junkIndex);
            var rarcSubdirPath =
                Path.Join(rarcSubdir.Impl.GetParent().FullName, rarcSubdirName);

            // If subdir has same name or is an abbreviation of the parent, 
            // just collapses them with the parent name.
            if (subdirName.Length <= rarcSubdirName.Length &&
                subdirName.ToLower() ==
                rarcSubdirName.Substring(0, subdirName.Length).ToLower()) {
              Directory.Move(subdir.FullName, rarcSubdirPath);
            }
            // If parent has same name or is an abbreviation of the subdir,
            // just collapses them with the subdir name.
            else if (subdirName.Length >= rarcSubdirName.Length &&
                subdirName.Substring(0, rarcSubdirName.Length).ToLower() ==
                rarcSubdirName.ToLower()) {
              Directory.Move(subdir.FullName,
                             Path.Join(rarcSubdir.Impl.GetParent().FullName,
                                       subdirName));
            }
            // If subdir has a different name, merges their names together and
            // collapses them.
            else {
              Directory.Move(subdir.FullName, $"{rarcSubdirPath}_{subdirName}");
            }

            // Gets rid of the unneeded RARC/SZS files/directories.
            File.Delete(szsPath);
            File.Delete(rarcPath);
            Directory.Delete(rarcSubdir.FullName);
          } else {
            Asserts.Fail("Multiple children in RARC!");
          }
        }
      }

      return fileHierarchy;
    }
  }
}