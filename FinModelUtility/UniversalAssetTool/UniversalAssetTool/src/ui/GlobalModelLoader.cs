using cmb.api;

using dat.api;

using fin.model;
using fin.model.io;
using fin.model.io.importer;

using glo.api;

using hw.api;

using j3d.api;

using level5.api;

using mod.api;

using modl.api;

using UoT.api;

using visceral.api;

using xmod.api;

namespace uni.ui {
  public class GlobalModelImporter : IModelImporter<IModelFileBundle> {
    public IModel ImportModel(IModelFileBundle modelFileBundle,
                              IModelParameters? modelParameters = null)
      => modelFileBundle switch {
          IBattalionWarsModelFileBundle battalionWarsModelFileBundle
              => new BattalionWarsModelImporter().ImportModel(
                  battalionWarsModelFileBundle,
                  modelParameters),
          BmdModelFileBundle bmdModelFileBundle
              => new BmdModelImporter().ImportModel(
                  bmdModelFileBundle,
                  modelParameters),
          CmbModelFileBundle cmbModelFileBundle
              => new CmbModelImporter().ImportModel(
                  cmbModelFileBundle,
                  modelParameters),
          DatModelFileBundle datModelFileBundle
              => new DatModelImporter().ImportModel(
                  datModelFileBundle,
                  modelParameters),
          GeoModelFileBundle geoModelFileBundle
              => new GeoModelImporter().ImportModel(
                  geoModelFileBundle,
                  modelParameters),
          GloModelFileBundle gloModelFileBundle
              => new GloModelImporter().ImportModel(
                  gloModelFileBundle,
                  modelParameters),
          IHaloWarsModelFileBundle haloWarsModelFileBundle
              => new HaloWarsModelImporter().ImportModel(
                  haloWarsModelFileBundle,
                  modelParameters),
          ModModelFileBundle modModelFileBundle
              => new ModModelImporter().ImportModel(
                  modModelFileBundle,
                  modelParameters),
          OotModelFileBundle ootModelFileBundle
              => new OotModelImporter().ImportModel(
                  ootModelFileBundle,
                  modelParameters),
          PedModelFileBundle pedModelFileBundle
              => new PedModelImporter().ImportModel(
                  pedModelFileBundle,
                  modelParameters),
          XcModelFileBundle xcModelFileBundle
              => new XcModelImporter().ImportModel(
                  xcModelFileBundle,
                  modelParameters),
          XmodModelFileBundle xmodModelFileBundle
              => new XmodModelImporter().ImportModel(
                  xmodModelFileBundle,
                  modelParameters),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };
  }
}