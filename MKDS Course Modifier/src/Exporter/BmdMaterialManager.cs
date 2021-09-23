using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using fin.cli;
using fin.model;

using MKDS_Course_Modifier.GCN;

namespace mkds.exporter {
  public class BmdMaterialManager {
    private readonly BMD bmd_;
    private readonly IList<BmdTexture> textures_;
    private readonly IList<BmdMaterial> materials_;

    public BmdMaterialManager(
        IModel model,
        BMD bmd,
        IList<(string, BTI)>? pathsAndBtis = null) {
      var outputDirectory = new FileInfo(Args.OutputPath).Directory;

      this.bmd_ = bmd;

      // Saves JSON representation of MAT3 for debugging materials
      var jsonSerializer = new JsonSerializer();
      jsonSerializer.Formatting = Formatting.Indented;
      var jsonTextWriter = new StringWriter();
      jsonSerializer.Serialize(jsonTextWriter, bmd.MAT3);
      File.WriteAllText($"{outputDirectory.FullName}\\mat3.txt",
                        jsonTextWriter.ToString());

      // Saves textures in directory
      this.textures_ = bmd.TEX1.TextureHeaders.Select((textureHeader, i) => {
                            var textureName =
                                bmd.TEX1.StringTable.Entries[i].Entry;

                            return new BmdTexture(
                                textureName,
                                textureHeader,
                                pathsAndBtis);
                          })
                          .ToList();

      foreach (var texture in this.textures_) {
        texture.SaveInDirectory(outputDirectory);
      }

      this.materials_ = this.GetMaterials_(model, bmd);
    }

    public BmdMaterial Get(int entryIndex)
      => this.materials_[this.bmd_.MAT3.MaterialEntryIndieces[entryIndex]];

    private IList<BmdMaterial> GetMaterials_(IModel model, BMD bmd)
      => bmd.MAT3.MaterialEntries.Select(
                (_, i) => new BmdMaterial(
                    model.MaterialManager,
                    i,
                    bmd,
                    this.textures_))
            .ToList();
  }
}