using fin.io;
using fin.log;
using fin.model;

using xmod.schema;

namespace xmod.api {
  public class XmodModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.XmodFile;
    public required IFileHierarchyFile XmodFile { get; init; }
  }

  public class XmodModelLoader : IModelLoader<XmodModelFileBundle> {
    public IModel LoadModel(XmodModelFileBundle modelFileBundle) {
      using var tr =
          new FinTextReader(modelFileBundle.XmodFile.Impl.OpenRead());

      var xmod = new Xmod();
      xmod.Read(tr);

      return default!;
    }
  }
}