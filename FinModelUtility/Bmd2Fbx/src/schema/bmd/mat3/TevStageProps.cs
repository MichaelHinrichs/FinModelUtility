using gx;

using schema;


namespace bmd.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevStageProps : IBiSerializable {
    private byte padding0_;

    public GxCc color_a;
    public GxCc color_b;
    public GxCc color_c;
    public GxCc color_d;
    public TevOp color_op;
    public TevBias color_bias;
    public TevScale color_scale;
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool color_clamp;
    public ColorRegister color_regid;

    public GxCa alpha_a;
    public GxCa alpha_b;
    public GxCa alpha_c;
    public GxCa alpha_d;
    public TevOp alpha_op;
    public TevBias alpha_bias;
    public TevScale alpha_scale;
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public bool alpha_clamp;
    public ColorRegister alpha_regid;

    private byte padding1_;
  }
}
