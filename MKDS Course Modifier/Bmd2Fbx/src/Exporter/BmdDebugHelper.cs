using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.cli;

using MKDS_Course_Modifier.GCN;

using Newtonsoft.Json;

namespace mkds.exporter {
  public static class BmdDebugHelper {
    public static void ExportFilesInBmd(
        BMD bmd,
        IList<(string, BTI)> pathsAndBtis) {
      var outputDirectory = Args.OutputDirectory;

      // Saves JSON representation of MAT3 for debugging materials
      var jsonSerializer = new JsonSerializer();
      jsonSerializer.Formatting = Formatting.Indented;
      var jsonTextWriter = new StringWriter();
      jsonSerializer.Serialize(jsonTextWriter, bmd.MAT3);
      File.WriteAllText(Path.Join(outputDirectory.FullName, "mat3.txt"),
                        jsonTextWriter.ToString());

      // Saves textures in directory
      var textures = bmd.TEX1.TextureHeaders.Select((textureHeader, i) => {
                          var textureName =
                              bmd.TEX1.StringTable.Entries[i].Entry;

                          return new BmdTexture(
                              textureName,
                              textureHeader,
                              pathsAndBtis);
                        })
                        .ToList();

      foreach (var texture in textures) {
        texture.SaveInDirectory(outputDirectory);
      }
    }
  }
}