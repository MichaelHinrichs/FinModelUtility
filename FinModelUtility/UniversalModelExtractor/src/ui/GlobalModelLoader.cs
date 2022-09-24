using bmd.exporter;

using cmb.api;

using dat.api;

using fin.model;

using glo.api;

using hw.api;
using level5.api;
using mod.cli;

using modl.api;


namespace uni.ui {
  public class GlobalModelLoader : IModelLoader<IModelFileBundle> {
    public IModel LoadModel(IModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          BmdModelFileBundle bmdModelFileBundle
              => new BmdModelLoader().LoadModel(bmdModelFileBundle),
          CmbModelFileBundle cmbModelFileBundle
              => new CmbModelLoader().LoadModel(cmbModelFileBundle),
          DatModelFileBundle datModelFileBundle
              => new DatModelLoader().LoadModel(datModelFileBundle),
          GloModelFileBundle gloModelFileBundle
              => new GloModelLoader().LoadModel(gloModelFileBundle),
          ModModelFileBundle modModelFileBundle
              => new ModModelLoader().LoadModel(modModelFileBundle),
          ModlModelFileBundle modlModelFileBundle
              => new ModlModelLoader().LoadModel(modlModelFileBundle),
          OutModelFileBundle outModelFileBundle
              => new OutModelLoader().LoadModel(outModelFileBundle),
          VisModelFileBundle visModelFileBundle
              => new VisModelLoader().LoadModel(visModelFileBundle),
          XiModelFileBundle xiModelFileBundle
              => new XiModelLoader().LoadModel(xiModelFileBundle),
          XtdModelFileBundle xtdModelFileBundle
              => new XtdModelLoader().LoadModel(xtdModelFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };
  }
}
