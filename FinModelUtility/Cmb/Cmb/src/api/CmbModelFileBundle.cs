using System.Collections.Generic;

using fin.io;
using fin.model;
using fin.util.enumerables;

namespace cmb.api {
  public class CmbModelFileBundle : IModelFileBundle {
    public CmbModelFileBundle(string gameName,
                              IFileHierarchyFile cmbFile) :
        this(gameName, cmbFile, null, null, null) { }

    public CmbModelFileBundle(string gameName,
                              IFileHierarchyFile cmbFile,
                              IReadOnlyList<IFileHierarchyFile>? csabFiles) :
        this(gameName, cmbFile, csabFiles, null, null) { }

    public CmbModelFileBundle(string gameName,
                              IFileHierarchyFile cmbFile,
                              IReadOnlyList<IFileHierarchyFile>? csabFiles,
                              IReadOnlyList<IFileHierarchyFile>? ctxbFiles,
                              IReadOnlyList<IFileHierarchyFile>? shpaFiles) {
      this.GameName = gameName;
      this.CmbFile = cmbFile;
      this.CsabFiles = csabFiles;
      this.CtxbFiles = ctxbFiles;
      this.ShpaFiles = shpaFiles;
    }

    public string GameName { get; }

    public IFileHierarchyFile MainFile => this.CmbFile;

    public IEnumerable<IReadOnlyGenericFile> Files
      => this.CmbFile.Yield()
             .ConcatIfNonnull(this.CsabFiles)
             .ConcatIfNonnull(this.CtxbFiles)
             .ConcatIfNonnull(this.ShpaFiles);

    public IFileHierarchyFile CmbFile { get; }
    public IReadOnlyList<IFileHierarchyFile>? CsabFiles { get; }
    public IReadOnlyList<IFileHierarchyFile>? CtxbFiles { get; }
    public IReadOnlyList<IFileHierarchyFile>? ShpaFiles { get; }
  }
}
