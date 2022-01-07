using System.IO;

using schema;

namespace zar.format.cmb {
  public class Sampler : IDeserializable {
    public bool isAbs { get; private set; }
    public sbyte index { get; private set; }
    public LutInput input { get; private set; }

    // TODO: LutScale only accepts these values
    // Quarter = 0.25,
    // Half = 0.5,
    // One = 1.0,
    // Two = 2.0,
    // Four = 4.0,
    // Eight = 8.0
    public float scale { get; private set; }

    public void Read(EndianBinaryReader r) {
      this.isAbs = r.ReadByte() != 0;
      this.index = r.ReadSByte();
      this.input = (LutInput) r.ReadUInt16();
      this.scale = r.ReadSingle();
    }
  }
}