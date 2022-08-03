using fin.io;
using fin.model;


namespace hw.api {
  public class VisModelFileBundle : IHaloWarsModelFileBundle {
    public VisModelFileBundle(IFileHierarchyFile visFile) {
      this.VisFile = visFile;
    }

    public IFileHierarchyFile MainFile => this.VisFile;
    public IFileHierarchyFile VisFile { get; }
  }

  public class VisModelLoader {
  }
}
