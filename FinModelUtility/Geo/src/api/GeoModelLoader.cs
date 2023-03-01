using fin.io;
using fin.model;

namespace geo.api {
  public class GeoModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile? MainFile => GeoFile;
    public required IFileHierarchyFile? GeoFile { get; init; }
  }

  public class GeoModelLoader : IModelLoader<GeoModelFileBundle> {
    public IModel LoadModel(GeoModelFileBundle modelFileBundle) {
      var geoFile = modelFileBundle.GeoFile;

      throw new NotImplementedException();
    }
  }
}
