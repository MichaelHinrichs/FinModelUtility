using System.Collections.Generic;
using System.Linq;

using fin.model;

using bmd.GCN;
using bmd.schema.bti;
using gx;


namespace bmd.exporter {
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
    }

    public GxFixedFunctionMaterial Get(int entryIndex)
      => this.materials_[this.bmd_.MAT3.MaterialEntryIndieces[entryIndex]];

    private IList<GxFixedFunctionMaterial> GetMaterials_(IModel model, BMD bmd)
      => bmd.MAT3.MaterialEntries.Select(
                (_, i) => new GxFixedFunctionMaterial(
                    model.MaterialManager,
                    bmd.MAT3.PopulatedMaterials[i],
                    this.textures_))
            .ToList();
  }
}