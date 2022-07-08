using System;
using System.IO;

using fin.util.strings;

using schema;

namespace zar.schema.cmb {
  public class Shp : IBiSerializable {
    public uint chunkSize;
    public Sepd[] shapes;

    // M-1:
    // No idea... but it does something to materials and it's never used on ANY model but link's in OoT3D
    // Set to 0x58 on "link_v2.cmb"
    public uint flags;

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("shp" + AsciiUtil.GetChar(0x20));

      this.chunkSize = r.ReadUInt32();
      this.shapes = new Sepd[r.ReadUInt32()];
      this.flags = r.ReadUInt32();

      for (var i = 0; i < this.shapes.Length; ++i) {
        r.ReadInt16(); // ShapeOffset(s)
      }

      r.Align(4);

      for (var i = 0; i < this.shapes.Length; ++i) {
        var shape = new Sepd();
        shape.Read(r);
        this.shapes[i] = shape;
      }
    }

    public void Write(EndianBinaryWriter w) {
      throw new NotImplementedException();
    }
  }
}
