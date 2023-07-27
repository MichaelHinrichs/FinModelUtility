using System.IO;

using cmb.schema.shpa.norm;
using cmb.schema.shpa.posi;

using fin.schema.data;
using fin.util.asserts;

using schema.binary;
using schema.binary.attributes.sequence;

namespace cmb.schema.shpa {
  public partial class Shpa : IBinaryDeserializable {
    public AutoMagicUInt32SizedSection<Posi> Posi { get; } = new("posi", -8);
    public AutoMagicUInt32SizedSection<Norm> Norm { get; } = new("norm", -8);
    public Idxs Idxs { get; } = new();

    public void Read(IEndianBinaryReader r) {
      r.AssertMagicText("shpa");

      var headerLength = r.ReadUInt32();
      var unk0 = r.ReadUInt32();
      r.AssertUInt32(1); // Animation count?
      var name = r.ReadString(16);
      var unk1 = r.ReadUInt32();
      var posiOffset = r.ReadUInt32();
      var normOffset = r.ReadUInt32();
      var idxsOffset = r.ReadUInt32();

      Asserts.Equal(posiOffset, r.Position);
      this.Posi.Read(r);

      Asserts.Equal(normOffset, r.Position);
      this.Norm.Read(r);

      Asserts.Equal(idxsOffset, r.Position);
      this.Idxs.Read(r);
    }
  }

  [BinarySchema]
  public partial class Idxs : IBinaryConvertible {
    private readonly string magic_ = "idxs";

    /// <summary>
    ///   The corresponding indices in the original model to update?
    /// </summary>
    [SequenceLengthSource(SchemaIntegerType.INT32)]
    public ushort[] Indices { get; private set; }
  }
}