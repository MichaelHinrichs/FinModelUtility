using System.IO;

using cmb.schema.shpa.norm;
using cmb.schema.shpa.posi;

using fin.schema.data;

using schema.binary;
using schema.binary.attributes;

namespace cmb.schema.shpa {
  [BinarySchema]
  [Endianness(Endianness.LittleEndian)]
  public partial class Shpa : IBinaryConvertible {
    private readonly string magic_ = "shpa";
    private readonly uint headerLength_ = 48;

    public uint unk0;

    private readonly uint animationCount_ = 1;

    [StringLengthSource(16)]
    public string Name { get; set; }

    public uint unk1;


    [WPointerTo(nameof(Posi))]
    private uint posiOffset_;

    [WPointerTo(nameof(Norm))]
    private uint normOffset_;

    [WPointerTo(nameof(Idxs))]
    private uint idxsOffset_;


    [RAtPosition(nameof(posiOffset_))]
    public AutoMagicUInt32SizedSection<Posi> Posi { get; } =
      new("posi") { TweakReadSize = -8, };

    [RAtPosition(nameof(normOffset_))]
    public AutoMagicUInt32SizedSection<Norm> Norm { get; } =
      new("norm") { TweakReadSize = -8, };

    [RAtPosition(nameof(idxsOffset_))]
    public Idxs Idxs { get; } = new();
  }
}