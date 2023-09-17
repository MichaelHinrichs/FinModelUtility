using schema.binary;

namespace modl.schema.anim.bw2 {
  public class Bw2AnimBone : IBwAnimBone, IBinaryDeserializable {
    public string GetIdentifier() => this.Name;

    public string Name { get; set; }

    public uint PositionKeyframeCount { get; set; }
    public uint RotationKeyframeCount { get; set; }

    private readonly ulong padding0_ = 0;
    public float XPosDelta { get; set; }
    public float YPosDelta { get; set; }
    public float ZPosDelta { get; set; }
    public float XPosMin { get; set; }
    public float YPosMin { get; set; }
    public float ZPosMin { get; set; }
    private readonly uint padding1_ = 0;

    public void Read(IEndianBinaryReader er) {
      this.Name = er.ReadString(16);

      this.PositionKeyframeCount = er.ReadUInt32();
      this.RotationKeyframeCount = er.ReadUInt32();

      er.AssertUInt64(0);

      this.XPosDelta = er.ReadSingle();
      this.YPosDelta = er.ReadSingle();
      this.ZPosDelta = er.ReadSingle();

      this.XPosMin = er.ReadSingle();
      this.YPosMin = er.ReadSingle();
      this.ZPosMin = er.ReadSingle();

      er.AssertUInt32(0);

      var values = er.ReadBytes(4);
    }
  }
}