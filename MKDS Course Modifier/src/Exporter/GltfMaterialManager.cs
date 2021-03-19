using System.Collections.Generic;
using System.IO;
using System.Linq;

using mkds.cli;

using MKDS_Course_Modifier.GCN;

using SharpGLTF.Materials;

namespace mkds.exporter {
  public class GltfMaterialManager {
    private readonly BMD bmd_;
    private readonly IList<GltfTexture> textures_;
    private readonly IList<GltfMaterial> materials_;

    public GltfMaterialManager(
        BMD bmd,
        IList<(string, BTI)>? pathsAndBtis = null) {
      this.bmd_ = bmd;

      var outputDirectory = new FileInfo(Args.OutputPath).Directory;

      this.textures_ = bmd.TEX1.TextureHeaders.Select((textureHeader, i) => {
                            var textureName =
                                bmd.TEX1.StringTable.Entries[i].Entry;

                            return new GltfTexture(
                                textureName,
                                textureHeader,
                                pathsAndBtis);
                          })
                          .ToList();

      this.materials_ = this.GetMaterials_(bmd);
    }

    public GltfMaterial Get(int entryIndex)
      => this.materials_[this.bmd_.MAT3.MaterialEntryIndieces[entryIndex]];

    private IList<GltfMaterial> GetMaterials_(BMD bmd)
      => bmd.MAT3.MaterialEntries.Select(
                materialEntry => new GltfMaterial(
                    materialEntry,
                    bmd,
                    this.textures_))
            .ToList();
  }
}