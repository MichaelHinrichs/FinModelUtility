using System.IO;

using fin.schema.data;

using schema.binary;
using schema.binary.attributes;

namespace j3d.schema.bmd.jnt1 {
  [BinarySchema]
  [LocalPositions]
  public partial class Jnt1 : IBinaryConvertible {
    private readonly AutoMagicUInt32SizedSection<Jnt1Data> impl_ =
        new("JNT1") { TweakReadSize = -8 };

    [Ignore]
    public Jnt1Data Data => this.impl_.Data;
  }

  [BinarySchema]
  public partial class Jnt1Data : IBinaryConvertible {
    [WLengthOfSequence(nameof(Joints))]
    [WLengthOfSequence(nameof(RemapTable))]
    private ushort jointCount_;

    private readonly ushort padding_ = ushort.MaxValue;

    [WPointerTo(nameof(Joints))]
    private uint jointEntryOffset_;

    [WPointerTo(nameof(RemapTable))]
    private uint remapTableOffset_;

    [WPointerTo(nameof(StringTable))]
    private uint stringTableOffset_;

    [RSequenceLengthSource(nameof(jointCount_))]
    [RAtPosition(nameof(jointEntryOffset_))]
    public Jnt1Entry[] Joints { get; set; }

    [RSequenceLengthSource(nameof(jointCount_))]
    [RAtPosition(nameof(remapTableOffset_))]
    public ushort[] RemapTable { get; set; }

    [RAtPosition(nameof(stringTableOffset_))]
    public StringTable StringTable { get; } = new();
  }
}