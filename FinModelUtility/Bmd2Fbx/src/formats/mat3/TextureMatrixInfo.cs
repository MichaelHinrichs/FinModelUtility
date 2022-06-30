using schema;


namespace bmd.formats.mat3 {
  [Schema]
  public partial class TextureMatrixInfo : IBiSerializable {
    public ushort Unknown1;
    public ushort Padding1;
    public float Unknown2;
    public float Unknown3;
    public float Unknown4;
    public float Unknown5;
    public float Unknown6;
    public ushort Unknown7;
    public ushort Padding2;
    public float Unknown8;
    public float Unknown9;
    public readonly float[] Matrix = new float[16];
  }

}
