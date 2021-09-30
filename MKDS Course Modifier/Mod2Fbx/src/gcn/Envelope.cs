using System.Collections.Generic;
using System.IO;

namespace mod.gcn {
  public class Envelope : IGcnSerializable {
    public readonly List<ushort> indices = new();
    public readonly List<float> weights = new();

    public void Read(EndianBinaryReader reader) {
      var numValues = reader.ReadUInt16();
      this.indices.Clear();
      this.weights.Clear();

      for (var i = 0; i < numValues; ++i) {
        this.indices.Add(reader.ReadUInt16());
        this.weights.Add(reader.ReadSingle());
      }
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write((ushort) this.indices.Count);

      for (var i = 0; i < this.indices.Count; ++i) {
        writer.Write(this.indices[i]);
        writer.Write(this.weights[i]);
      }
    }
  }
}