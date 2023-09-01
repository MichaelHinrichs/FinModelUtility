using fin.model;

namespace hw.api {
  public class HaloWarsModelReader : IModelReader<IHaloWarsModelFileBundle> {
    public IModel ReadModel(IHaloWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
        VisModelFileBundle visModelFileBundle => new VisModelReader().ReadModel(visModelFileBundle),
        XtdModelFileBundle xtdModelFileBundle => new XtdModelReader().ReadModel(xtdModelFileBundle),
      };
  }
}