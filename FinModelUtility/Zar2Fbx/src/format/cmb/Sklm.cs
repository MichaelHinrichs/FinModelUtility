using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Sklm : IDeserializable {
    public uint chunkSize;
    public uint mshOffset;
    public uint shpOffset;
    public readonly Mshs meshes = new();
    public readonly Shp shapes = new();

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("sklm");

      this.chunkSize = r.ReadUInt32();
      this.mshOffset = r.ReadUInt32();
      this.shpOffset = r.ReadUInt32();

      this.meshes.Read(r);
      this.shapes.Read(r);
    }
  }
}
