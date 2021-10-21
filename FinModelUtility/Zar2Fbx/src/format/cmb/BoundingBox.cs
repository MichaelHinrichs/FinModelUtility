using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class BoundingBox : IDeserializable {
    // M-1 checked all files, and Min/Max are the only values to ever change
    public uint unk0 { get; private set; }
    public uint unk1 { get; private set; }
    public float[] min { get; } = new float[3];
    public float[] max { get; } = new float[3];
    public int unk2 { get; private set; }
    public int unk3 { get; private set; }
    public uint unk4 { get; private set; }

    public void Read(EndianBinaryReader r) {
      this.unk0 = r.ReadUInt32();
      this.unk1 = r.ReadUInt32();
      r.ReadSingles(this.min);
      r.ReadSingles(this.max);
      this.unk2 = r.ReadInt32();
      this.unk3 = r.ReadInt32();
      this.unk4 = r.ReadUInt32();
    }
  }
}