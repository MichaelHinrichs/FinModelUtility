using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class MatIndirectTexturingEntry : IBiSerializable {
    public byte Unknown0 { get; set; }
    public byte Unknown1 { get; set; }
    private readonly ushort padding_ = ushort.MaxValue;

    [ArrayLengthSource(4)]
    public IndTexOrder[] IndTexOrder { get; set; }
    [ArrayLengthSource(3)]
    public IndTexMatrix[] IndTexMatrix { get; set; }

    [ArrayLengthSource(4)]
    public IndTexCoordScale[] IndTexCoordScale { get; set; }

    [ArrayLengthSource(16)]
    public TevIndirect[] TevIndirect { get; set; }
  }

  [BinarySchema]
  public partial class IndTexOrder : IBiSerializable {
    public sbyte TexCoord { get; set; }
    public sbyte TexMap { get; set; }
    private readonly ushort padding_ = ushort.MaxValue;
  }

  [BinarySchema]
  public partial class IndTexMatrix : IBiSerializable {
    public float[] OffsetMatrix { get; } = new float[2 * 3];
    public sbyte ScaleExponent { get; set; }
    private readonly byte padding1_ = 0xff;
    private readonly byte padding2_ = 0xff;
    private readonly byte padding3_ = 0xff;
  }

  [BinarySchema]
  public partial class IndTexCoordScale : IBiSerializable {
    public byte ScaleS { get; set; }
    public byte ScaleT { get; set; }
    private readonly ushort padding_ = ushort.MaxValue;
  }

  [BinarySchema]
  public partial class TevIndirect : IBiSerializable {
    public byte TevStageId { get; set; }
    public byte IndTexFormat { get; set; }
    public byte IndTexBiasSel { get; set; }
    public byte IndTexMtdId { get; set; }
    public byte IndTexWrapS { get; set; }
    public byte IndTexWrapT { get; set; }
    public byte AddPrev { get; set; }
    public byte UtcLod { get; set; }
    public byte A { get; set; }
    private readonly byte padding1_ = 0xff;
    private readonly byte padding2_ = 0xff;
    private readonly byte padding3_ = 0xff;
  }
}