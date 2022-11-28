using System.Collections.Generic;
using System.Linq;

using fin.model;

using bmd.GCN;
using bmd.schema.bti;

namespace bmd.exporter {
  public class BmdMaterialManager {
    private readonly BMD bmd_;
    private readonly IList<BmdTexture> textures_;
    private readonly IList<BmdFixedFunctionMaterial> materials_;

    public BmdMaterialManager(
        IModel model,
        BMD bmd,
        IList<(string, Bti)>? pathsAndBtis = null) {
      this.bmd_ = bmd;

      this.textures_ = bmd.TEX1.TextureHeaders.Select((textureHeader, i) => {
                            var textureName =
                                bmd.TEX1.StringTable.Entries[i].Entry;

                            return new BmdTexture(
                                textureName,
                                textureHeader,
                                pathsAndBtis);
                          })
                          .ToList();

      this.materials_ = this.GetMaterials_(model, bmd);
    }

    public BmdFixedFunctionMaterial Get(int entryIndex)
      => this.materials_[this.bmd_.MAT3.MaterialEntryIndieces[entryIndex]];

    private IList<BmdFixedFunctionMaterial> GetMaterials_(IModel model, BMD bmd)
      => bmd.MAT3.MaterialEntries.Select(
                (_, i) => new BmdFixedFunctionMaterial(
                    model.MaterialManager,
                    i,
                    bmd,
                    this.textures_))
            .ToList();
  }
}