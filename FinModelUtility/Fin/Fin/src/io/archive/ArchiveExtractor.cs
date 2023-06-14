using System.Collections.Generic;
using System.IO;

namespace fin.io.archive {
  public interface IArchiveExtractor {
    void ExtractTo(ISystemDirectory systemDirectory,
                   IEnumerable<ArchiveFile> archiveFiles);
  }

  public class ArchiveExtractor : IArchiveExtractor {
    public void ExtractTo(ISystemDirectory rootSystemDirectory,
                          IEnumerable<ArchiveFile> archiveFiles) {
      foreach (var archiveFile in archiveFiles) {
        var dstFile = new FinFile(
            Path.Join(rootSystemDirectory.FullName,
                      archiveFile.RelativeName));
        dstFile.GetParent().Create();

        var bytes = archiveFile.GetDataHandler();
        dstFile.WriteAllBytes(bytes);
      }
    }
  }
}