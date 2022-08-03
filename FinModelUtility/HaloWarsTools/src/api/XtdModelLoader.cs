using fin.io;
using fin.model;


namespace hw.api {
  public class XtdModelFileBundle : IHaloWarsModelFileBundle {
    public XtdModelFileBundle(IFileHierarchyFile xtdFile) {
      this.XtdFile = xtdFile;
    }

    public IFileHierarchyFile MainFile => this.XtdFile;
    public IFileHierarchyFile XtdFile { get; }
  }

  public class XtdModelLoader {
  }
}
