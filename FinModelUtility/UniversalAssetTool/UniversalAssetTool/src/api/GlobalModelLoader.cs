﻿using cmb.api;

using dat.api;

using fin.model;
using fin.model.io;
using fin.model.io.importers;

using glo.api;

using hw.api;

using jsystem.api;

using level5.api;

using mod.api;

using modl.api;

using pmdc.api;

using UoT.api;

using visceral.api;

using xmod.api;

namespace uni.api {
  public class GlobalModelImporter : IModelImporter<IModelFileBundle> {
    public IModel Import(IModelFileBundle modelFileBundle)
      => modelFileBundle switch {
          IBattalionWarsModelFileBundle battalionWarsModelFileBundle
              => new BattalionWarsModelImporter().Import(
                  battalionWarsModelFileBundle),
          BmdModelFileBundle bmdModelFileBundle
              => new BmdModelImporter().Import(bmdModelFileBundle),
          CmbModelFileBundle cmbModelFileBundle
              => new CmbModelImporter().Import(cmbModelFileBundle),
          DatModelFileBundle datModelFileBundle
              => new DatModelImporter().Import(datModelFileBundle),
          GeoModelFileBundle geoModelFileBundle
              => new GeoModelImporter().Import(geoModelFileBundle),
          GloModelFileBundle gloModelFileBundle
              => new GloModelImporter().Import(gloModelFileBundle),
          XtdModelFileBundle xtdModelFileBundle 
              => new XtdModelImporter().Import(xtdModelFileBundle),
          ModModelFileBundle modModelFileBundle
              => new ModModelImporter().Import(modModelFileBundle),
          OmdModelFileBundle omdModelFileBundle
              => new OmdModelImporter().Import(omdModelFileBundle),
          OotModelFileBundle ootModelFileBundle
              => new OotModelImporter().Import(ootModelFileBundle),
          PedModelFileBundle pedModelFileBundle
              => new PedModelImporter().Import(pedModelFileBundle),
          XcModelFileBundle xcModelFileBundle
              => new XcModelImporter().Import(xcModelFileBundle),
          XmodModelFileBundle xmodModelFileBundle
              => new XmodModelImporter().Import(xmodModelFileBundle),
          _ => throw new ArgumentOutOfRangeException(nameof(modelFileBundle))
      };
  }
}