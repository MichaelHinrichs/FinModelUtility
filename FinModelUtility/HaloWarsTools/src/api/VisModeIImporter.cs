using fin.model;
using fin.model.io;
using fin.model.io.importer;

using HaloWarsTools;

namespace hw.api {
  public class VisModelImporter : IModelImporter<VisModelFileBundle> {
    public IModel ImportModel(VisModelFileBundle modelFileBundle,
                              IModelParameters? modelParameters = null) {
      var visResource =
          HWVisResource.FromFile(modelFileBundle.Context,
                                 modelFileBundle.VisFile.FullPath);
      return visResource.Model;
    }
  }
}