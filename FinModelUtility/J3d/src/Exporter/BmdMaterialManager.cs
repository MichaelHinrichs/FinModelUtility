using System.Collections.Generic;
using System.Linq;

using fin.color;
using fin.model;
using gx;
using j3d.GCN;
using j3d.schema.bti;


namespace j3d.exporter {
  public class BmdMaterialManager {
    private readonly BMD bmd_;
    private readonly IList<IGxTexture> textures_;
    private readonly IList<GxFixedFunctionMaterial> materials_;

    public BmdMaterialManager(
        IModel model,
        BMD bmd,
        IList<(string, Bti)>? pathsAndBtis = null) {
      this.bmd_ = bmd;

      this.textures_ = bmd.TEX1.TextureHeaders.Select((textureHeader, i) => {
                            var textureName =
                                bmd.TEX1.StringTable.Entries[i].Entry;

                            return (IGxTexture) new BmdGxTexture(
                                textureName,
                                textureHeader,
                                pathsAndBtis);
                          })
                          .ToList();

      this.materials_ = this.GetMaterials_(model, bmd);

      // TODO: How to set up lights from mat3??? Also, is this ever actually used???
    }

    public GxFixedFunctionMaterial Get(int entryIndex)
      => this.materials_[this.bmd_.MAT3.MaterialEntryIndieces[entryIndex]];

    private IList<GxFixedFunctionMaterial> GetMaterials_(IModel model, BMD bmd)
      => bmd.MAT3.MaterialEntries.Select(
                (_, i) => new GxFixedFunctionMaterial(
                    model,
                    model.MaterialManager,
                    bmd.MAT3.PopulatedMaterials[i],
                    this.textures_))
            .ToList();
  }
}