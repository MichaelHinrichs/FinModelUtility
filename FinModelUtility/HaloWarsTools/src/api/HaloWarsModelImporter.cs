using fin.model;
using fin.model.io.importer;

namespace hw.api {
  public class HaloWarsModelImporter : IModelImporter<IHaloWarsModelFileBundle> {
    public IModel ImportModel(IHaloWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
        VisModelFileBundle visModelFileBundle => new VisModelImporter().ImportModel(visModelFileBundle),
        XtdModelFileBundle xtdModelFileBundle => new XtdModelImporter().ImportModel(xtdModelFileBundle),
      };
  }
}