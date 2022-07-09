using schema;


namespace modl.schema.modl.bw1.node {
  [Schema]
  public partial class Bw1RnodMatrix : IBiSerializable {
    public float[] Matrix { get; } = new float[4 * 4];
  }
}
