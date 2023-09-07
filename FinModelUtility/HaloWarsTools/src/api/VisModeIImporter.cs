using fin.io;
using fin.model;
using fin.model.io.importer;

using HaloWarsTools;

namespace hw.api {
  public class VisModelFileBundle : IHaloWarsModelFileBundle {
    public VisModelFileBundle(IReadOnlyTreeFile visFile, HWContext context) {
      this.VisFile = visFile;
      this.Context = context;
    }

    public string GameName => "halo_wars";
    public IReadOnlyTreeFile MainFile => this.VisFile;
    public IReadOnlyTreeFile VisFile { get; }

    public HWContext Context { get; }
  }

  public class VisModelImporter : IModelImporter<VisModelFileBundle> {
    public IModel ImportModel(VisModelFileBundle modelFileBundle) {
      var visResource =
          HWVisResource.FromFile(modelFileBundle.Context,
                                 modelFileBundle.VisFile.FullPath);
      return visResource.Model;
    }
  }
}