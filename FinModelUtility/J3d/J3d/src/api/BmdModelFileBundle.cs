using System.Collections.Generic;

using fin.io;
using fin.model.io;
using fin.util.enumerables;

namespace j3d.api {
  public class BmdModelFileBundle : IModelFileBundle {
    public required string GameName { get; init; }
    public IFileHierarchyFile MainFile => this.BmdFile;

    public IEnumerable<IReadOnlyGenericFile> Files
      => this.BmdFile.Yield()
             .ConcatIfNonnull(this.BcxFiles)
             .ConcatIfNonnull(this.BtiFiles);

    public IFileHierarchyFile BmdFile { get; set; }
    public IReadOnlyList<IFileHierarchyFile>? BcxFiles { get; set; }
    public IReadOnlyList<IFileHierarchyFile>? BtiFiles { get; set; }
    public float FrameRate { get; set; } = 30;
  }
}
