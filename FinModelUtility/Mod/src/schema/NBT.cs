using fin.schema.vector;

using schema.binary;

namespace mod.schema {
  [BinarySchema]
  public partial class Nbt : IBiSerializable {
    public Vector3f Normal { get; }= new();
    public Vector3f Binormal { get; }= new();
    public Vector3f Tangent { get; }= new();
  }
}