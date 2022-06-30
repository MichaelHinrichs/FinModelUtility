using schema;


namespace bmd.formats {
  [Schema]
  public partial class DepthFunction : IDeserializable {
    public byte Enable;
    public byte Func;
    public byte UpdateEnable;
    public byte Padding;
  }
}
