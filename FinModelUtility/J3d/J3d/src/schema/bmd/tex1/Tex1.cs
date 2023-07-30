using fin.schema.data;

using schema.binary;
using schema.binary.attributes;

namespace j3d.schema.bmd.tex1 {
  [BinarySchema]
  [LocalPositions]
  public partial class Tex1 : IBinaryConvertible {
    private readonly AutoMagicUInt32SizedSection<Tex1Data> impl_ =
        new("TEX1") { TweakReadSize = -8 };

    [Ignore]
    public Tex1Data Data => this.impl_.Data;
  }

  [BinarySchema]
  public partial class Tex1Data : IBinaryConvertible {
    [WLengthOfSequence(nameof(TextureHeaders))]
    private ushort textureCount_;

    private readonly ushort padding_ = ushort.MaxValue;

    [WPointerTo(nameof(TextureHeaders))]
    private uint textureHeaderOffset_;

    [WPointerTo(nameof(StringTable))]
    private uint stringTableOffset_;

    [RSequenceLengthSource(nameof(textureCount_))]
    [RAtPosition(nameof(textureHeaderOffset_))]
    public TextureEntry[] TextureHeaders;

    [RAtPosition(nameof(stringTableOffset_))]
    public readonly StringTable StringTable = new();
  }
}