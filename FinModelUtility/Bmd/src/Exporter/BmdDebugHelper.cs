using System.Collections.Generic;
using System.IO;
using System.Linq;

using bmd.GCN;
using bmd.schema.bti;
using fin.io;

using Newtonsoft.Json;

namespace bmd.exporter {
  public static class BmdDebugHelper {
    public static void ExportFilesInBmd(
        IDirectory outputDirectory,
        BMD bmd,
        string bmdName,
        IList<(string, Bti)> pathsAndBtis) {
      // Saves JSON representation of MAT3 for debugging materials
      var jsonSerializer = new JsonSerializer();
      jsonSerializer.Formatting = Formatting.Indented;
      var jsonTextWriter = new StringWriter();
      jsonSerializer.Serialize(jsonTextWriter, bmd.MAT3);
      File.WriteAllText(
          Path.Join(outputDirectory.FullName, bmdName + "_mat3.txt"),
          jsonTextWriter.ToString());

      // Saves textures in directory
      var textures = bmd.TEX1.TextureHeaders.Select((textureHeader, i) => {
                          var textureName =
                              bmd.TEX1.StringTable.Entries[i].Entry;

                          return new BmdGxTexture(
                              textureName,
                              textureHeader,
                              pathsAndBtis);
                        })
                        .ToList();

      // TODO: Move to indirect model exporter
      foreach (var texture in textures) {
        texture.SaveInDirectory(outputDirectory);
      }
    }
  }
}