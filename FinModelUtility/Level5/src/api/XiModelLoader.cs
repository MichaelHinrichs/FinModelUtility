using fin.io;
using fin.model;
using fin.model.impl;
using level5.schema;


namespace level5.api {
  public class XiModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.XcFile;

    public IFileHierarchyFile XcFile { get; set; }
  }

  public class XiModelLoader : IModelLoader<XiModelFileBundle> {
    public IModel LoadModel(XiModelFileBundle modelFileBundle) {
      var xcFile = modelFileBundle.XcFile;
      var xc = xcFile.Impl.ReadNew<Xc>(Endianness.LittleEndian);

      var model = new ModelImpl();

      return model;
    }
  }
}