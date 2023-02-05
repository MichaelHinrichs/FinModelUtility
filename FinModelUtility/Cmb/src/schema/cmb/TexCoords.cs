using fin.schema.vector;
using schema.binary;

namespace cmb.schema.cmb {
  [BinarySchema]
  public partial class TexCoords : IDeserializable {
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public TextureMatrixMode matrixMode { get; private set; }
    public byte referenceCameraIndex { get; private set; }
    [IntegerFormat(SchemaIntegerType.BYTE)]
    public TextureMappingType mappingMethod { get; private set; }
    public byte coordinateIndex { get; private set; }
    public Vector2f scale { get; } = new();
    public float rotation { get; private set; }
    public Vector2f translation { get; } = new();
  }
}