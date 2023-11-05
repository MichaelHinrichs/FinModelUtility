using schema.binary;
using schema.binary.attributes;

namespace dat.schema.animation {
  /// <summary>
  ///   Shamelessly stolen from:
  ///   https://github.com/Ploaj/HSDLib/blob/master/HSDRaw/Common/Animation/HSD_Track.cs
  /// </summary>
  [BinarySchema]
  public partial class FigaTreeTrack : IDatKeyframes, IBinaryDeserializable {
    [IntegerFormat(SchemaIntegerType.UINT16)]
    public uint DataLength { get; set; }

    [IntegerFormat(SchemaIntegerType.INT16)]
    public int StartFrame { get; set; }

    public byte TrackType { get; set; }

    public byte ValueFlag { get; set; }
    public byte TangentFlag { get; set; }

    public byte Unknown { get; set; }

    public uint DataOffset { get; set; }


    [Ignore]
    public JointTrackType JointTrackType => (JointTrackType) this.TrackType;

    [Ignore]
    public byte MatTrackType => this.TrackType;

    [Ignore]
    public byte TexTrackType => this.TrackType;

    [Ignore]
    public LinkedList<(int frame, float value, float? tangent)> Keyframes {
      get;
    } = new();


    [ReadLogic]
    private void ReadKeyframes_(IBinaryReader br) {
      DatKeyframesUtil.ReadKeyframes(br, this, this.Keyframes);
    }
  }
}