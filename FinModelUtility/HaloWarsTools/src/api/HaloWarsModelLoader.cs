using fin.model;

namespace hw.api {
  public class HaloWarsModelLoader : IModelLoader<IHaloWarsModelFileBundle> {
    public IModel LoadModel(IHaloWarsModelFileBundle modelFileBundle)
      => modelFileBundle switch {
        VisModelFileBundle visModelFileBundle => new VisModelLoader().LoadModel(visModelFileBundle),
        XtdModelFileBundle xtdModelFileBundle => new XtdModelLoader().LoadModel(xtdModelFileBundle),
      };
  }
}