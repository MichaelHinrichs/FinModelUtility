using System.IO;

using schema;

namespace zar.format.cmb {
  [Schema]
  public partial class BoundingBox : IDeserializable {
    // M-1 checked all files, and Min/Max are the only values to ever change
    public uint unk0 { get; private set; }
    public uint unk1 { get; private set; }
    public float[] min { get; } = new float[3];
    public float[] max { get; } = new float[3];
    public int unk2 { get; private set; }
    public int unk3 { get; private set; }
    public uint unk4 { get; private set; }
  }
}