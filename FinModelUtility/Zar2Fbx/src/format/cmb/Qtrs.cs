using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Qtrs : IDeserializable {
    public uint chunkSize { get; private set; }
    public BoundingBox[] boundingBoxes { get; private set; }

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("qtrs");

      this.chunkSize = r.ReadUInt32();

      this.boundingBoxes = new BoundingBox[r.ReadUInt32()];
      for (var i = 0; i < this.boundingBoxes.Length; ++i) {
        var boundingBox = new BoundingBox();
        boundingBox.Read(r);
        this.boundingBoxes[i] = boundingBox;
      }
    }
  }
}
