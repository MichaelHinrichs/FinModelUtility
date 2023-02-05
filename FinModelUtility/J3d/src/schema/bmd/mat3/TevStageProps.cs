using gx;

using schema.binary;


namespace j3d.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevStageProps : ITevStageProps, IBiSerializable {
    private byte padding0_;

    public GxCc color_a { get; set; }
    public GxCc color_b { get; set; }
    public GxCc color_c { get; set; }
    public GxCc color_d { get; set; }
    public TevOp color_op { get; set; }
    public TevBias color_bias { get; set; }
    public TevScale color_scale { get; set; }
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool color_clamp { get; set; }
    public ColorRegister color_regid { get; set; }

    public GxCa alpha_a { get; set; }
    public GxCa alpha_b { get; set; }
    public GxCa alpha_c { get; set; }
    public GxCa alpha_d { get; set; }
    public TevOp alpha_op { get; set; }
    public TevBias alpha_bias { get; set; }
    public TevScale alpha_scale { get; set; }
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool alpha_clamp { get; set; }
    public ColorRegister alpha_regid { get; set; }

    private byte padding1_;
  }
}
