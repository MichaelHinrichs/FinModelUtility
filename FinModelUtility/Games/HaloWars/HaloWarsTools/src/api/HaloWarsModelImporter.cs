using fin.model;
using fin.model.io.importers;

namespace hw.api {
  public class HaloWarsModelImporter : IModelImporter<IHaloWarsModelFileBundle> {
    public IModel Import(IHaloWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
        VisModelFileBundle visModelFileBundle => new VisModelImporter().Import(visModelFileBundle),
        XtdModelFileBundle xtdModelFileBundle => new XtdModelImporter().Import(xtdModelFileBundle),
      };
  }
}