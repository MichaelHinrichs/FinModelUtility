using fin.schema.data;

using schema.binary;
using schema.binary.attributes;

namespace j3d.schema.bmd.drw1 {
  [BinarySchema]
  [LocalPositions]
  public partial class Drw1 : IBinaryConvertible {
    private readonly AutoMagicUInt32SizedSection<Drw1Data> impl_ =
        new("DRW1") { TweakReadSize = -8 };

    [Ignore]
    public Drw1Data Data => this.impl_.Data;
  }

  [BinarySchema]
  public partial class Drw1Data : IBinaryConvertible {
    [WLengthOfSequence(nameof(IsWeighted))]
    [WLengthOfSequence(nameof(Data))]
    private ushort count_;

    private readonly ushort padding_ = ushort.MaxValue;

    [WPointerTo(nameof(IsWeighted))]
    private uint isWeightedOffset_;

    [WPointerTo(nameof(Data))]
    private uint dataOffset_;

    [IntegerFormat(SchemaIntegerType.BYTE)]
    [RSequenceLengthSource(nameof(count_))]
    [RAtPosition(nameof(isWeightedOffset_))]
    public bool[] IsWeighted;

    [RSequenceLengthSource(nameof(count_))]
    [RAtPosition(nameof(dataOffset_))]
    public ushort[] Data;
  }
}