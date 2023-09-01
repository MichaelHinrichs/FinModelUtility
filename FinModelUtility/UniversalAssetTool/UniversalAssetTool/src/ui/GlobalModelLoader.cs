using cmb.api;

using dat.api;

using fin.model;

using glo.api;

using hw.api;

using j3d.api;

using level5.api;

using mod.cli;

using modl.api;

using UoT.api;

using visceral.api;

using xmod.api;

namespace uni.ui {
  public class GlobalModelReader : IModelReader<IModelFileBundle> {
    public IModel ReadModel(IModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          IBattalionWarsModelFileBundle battalionWarsModelFileBundle
              => new BattalionWarsModelReader().ReadModel(battalionWarsModelFileBundle),
          BmdModelFileBundle bmdModelFileBundle
              => new BmdModelReader().ReadModel(bmdModelFileBundle),
          CmbModelFileBundle cmbModelFileBundle
              => new CmbModelReader().ReadModel(cmbModelFileBundle),
          DatModelFileBundle datModelFileBundle
              => new DatModelReader().ReadModel(datModelFileBundle),
          GeoModelFileBundle geoModelFileBundle
              => new GeoModelReader().ReadModel(geoModelFileBundle),
          GloModelFileBundle gloModelFileBundle
              => new GloModelReader().ReadModel(gloModelFileBundle),
          IHaloWarsModelFileBundle haloWarsModelFileBundle 
              => new HaloWarsModelReader().ReadModel(haloWarsModelFileBundle),
          ModModelFileBundle modModelFileBundle
              => new ModModelReader().ReadModel(modModelFileBundle),
          OotModelFileBundle ootModelFileBundle
              => new OotModelReader().ReadModel(ootModelFileBundle),
          PedModelFileBundle pedModelFileBundle
              => new PedModelReader().ReadModel(pedModelFileBundle),
          XcModelFileBundle xcModelFileBundle
              => new XcModelReader().ReadModel(xcModelFileBundle),
          XmodModelFileBundle xmodModelFileBundle
              => new XmodModelReader().ReadModel(xmodModelFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };
  }
}