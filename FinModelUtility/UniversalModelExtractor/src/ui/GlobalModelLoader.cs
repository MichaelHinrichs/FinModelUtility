using j3d.exporter;
using cmb.api;
using dat.api;
using fin.model;
using glo.api;
using hw.api;
using level5.api;
using mod.cli;
using modl.api;
using sm64.api;
using xmod.api;


namespace uni.ui {
  public class GlobalModelLoader : IModelLoader<IModelFileBundle> {
    public IModel LoadModel(IModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          IBattalionWarsModelFileBundle battalionWarsModelFileBundle
              => new BattalionWarsModelLoader().LoadModel(battalionWarsModelFileBundle),
          BmdModelFileBundle bmdModelFileBundle
              => new BmdModelLoader().LoadModel(bmdModelFileBundle),
          CmbModelFileBundle cmbModelFileBundle
              => new CmbModelLoader().LoadModel(cmbModelFileBundle),
          DatModelFileBundle datModelFileBundle
              => new DatModelLoader().LoadModel(datModelFileBundle),
          GloModelFileBundle gloModelFileBundle
              => new GloModelLoader().LoadModel(gloModelFileBundle),
          IHaloWarsModelFileBundle haloWarsModelFileBundle 
              => new HaloWarsModelLoader().LoadModel(haloWarsModelFileBundle),
          ModModelFileBundle modModelFileBundle
              => new ModModelLoader().LoadModel(modModelFileBundle),
          Sm64LevelModelFileBundle sm64LevelModelFileBundle
              => new Sm64LevelModelLoader().LoadModel(sm64LevelModelFileBundle),
          XcModelFileBundle xcModelFileBundle
              => new XcModelLoader().LoadModel(xcModelFileBundle),
          XmodModelFileBundle xmodModelFileBundle
              => new XmodModelLoader().LoadModel(xmodModelFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };
  }
}