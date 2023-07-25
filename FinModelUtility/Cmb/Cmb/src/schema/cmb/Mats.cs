using System.Collections.Generic;
using System.IO;

using schema.binary;

namespace cmb.schema.cmb {
  public class Mats : IBinaryDeserializable {
    public uint chunkSize;
    public Material[] materials;

    public void Read(IEndianBinaryReader r) {
      r.AssertMagicText("mats");

      this.chunkSize = r.ReadUInt32();

      this.materials = new Material[r.ReadUInt32()];
      for (var i = 0; i < this.materials.Length; ++i) {
        var material = new Material();
        material.Read(r);
        this.materials[i] = material;
      }

      var combiners = new List<Combiner>();
      foreach (var material in this.materials) {
        for (var i = 0; i < material.texEnvStageCount; ++i) {
          var combiner = new Combiner();
          combiner.Read(r);
          combiners.Add(combiner);
        }
      }

      foreach (var material in this.materials) {
        material.texEnvStages.Clear();

        foreach (var i in material.texEnvStagesIndices) {
          if (i != -1) {
            material.texEnvStages.Add(combiners[i]);
          }
        }
      }
    }
  }
}
