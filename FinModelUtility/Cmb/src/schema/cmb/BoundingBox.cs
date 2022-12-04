using fin.schema.vector;
using System.IO;
using schema;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class BoundingBox : IBiSerializable {
    // M-1 checked all files, and Min/Max are the only values to ever change
    public uint unk0 { get; private set; }
    public uint unk1 { get; private set; }
    public Vector3f min { get; } = new();
    public Vector3f max { get; } = new();
    public int unk2 { get; private set; }
    public int unk3 { get; private set; }
    public uint unk4 { get; private set; }
  }
}