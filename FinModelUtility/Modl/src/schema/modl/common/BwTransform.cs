using schema;


namespace modl.schema.modl.common {

  [BinarySchema]
  public partial class BwTransform : IBiSerializable {
    public BwPosition Position { get; } = new();
    public BwRotation Rotation { get; } = new();
  }

  [BinarySchema]
  public partial class BwPosition : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
  }

  [BinarySchema]
  public partial class BwRotation : IBiSerializable {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
  }
}
