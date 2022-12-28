using fin.schema.matrix;
using fin.schema.vector;
using gx;
using schema;


namespace j3d.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TextureMatrixInfo : ITextureMatrixInfo, IBiSerializable {
    public GxTexGenType TexGenType { get; set; }
    public byte info;
    public ushort padding1;
    public Vector3f Center { get; } = new();
    public Vector2f Scale { get; } = new();
    public ushort Rotation;
    public ushort Padding2;
    public Vector2f Translation { get; } = new();
    public Matrix4x4f Matrix { get; } = new();
  }
}