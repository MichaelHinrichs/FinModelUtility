using System.IO;

using schema;

namespace zar.format.cmb {
  public class VertexAttribute : IDeserializable {
    public uint start { get; private set; }
    public float scale { get; private set; }
    public DataType dataType { get; private set; }
    public VertexAttributeMode mode { get; private set; }
    public float[] constants { get; } = new float[4];

    public void Read(EndianBinaryReader r) {
      this.start = r.ReadUInt32();
      this.scale = r.ReadSingle();
      this.dataType = (DataType) r.ReadUInt16();
      this.mode = (VertexAttributeMode) r.ReadUInt16();
      r.ReadSingles(this.constants);
    }
  }
}