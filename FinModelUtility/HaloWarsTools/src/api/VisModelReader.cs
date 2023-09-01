using fin.io;
using fin.model;

using HaloWarsTools;

namespace hw.api {
  public class VisModelFileBundle : IHaloWarsModelFileBundle {
    public VisModelFileBundle(IFileHierarchyFile visFile, HWContext context) {
      this.VisFile = visFile;
      this.Context = context;
    }

    public string GameName => "halo_wars";
    public IFileHierarchyFile MainFile => this.VisFile;
    public IFileHierarchyFile VisFile { get; }

    public HWContext Context { get; }
  }

  public class VisModelReader : IModelReader<VisModelFileBundle> {
    public IModel ReadModel(VisModelFileBundle modelFileBundle) {
      var visResource =
          HWVisResource.FromFile(modelFileBundle.Context,
                                 modelFileBundle.VisFile.FullPath);
      return visResource.Model;
    }
  }
}