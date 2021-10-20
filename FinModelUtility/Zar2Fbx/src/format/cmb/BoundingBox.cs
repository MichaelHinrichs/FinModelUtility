using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class BoundingBox : IDeserializable {
    // M-1 checked all files, and Min/Max are the only values to ever change
    public uint unk0;
    public uint unk1;
    public readonly float[] min = new float[3];
    public readonly float[] max = new float[3];
    public int unk2;
    public int unk3;
    public uint unk4;

    public void Read(EndianBinaryReader r) {
      this.unk0 = r.ReadUInt32();
      this.unk1 = r.ReadUInt32();
      r.ReadSingles(this.min, 3);
      r.ReadSingles(this.max, 3);
      this.unk2 = r.ReadInt32();
      this.unk3 = r.ReadInt32();
      this.unk4 = r.ReadUInt32();
    }
  }
}
