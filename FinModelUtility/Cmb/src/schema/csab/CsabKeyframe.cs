namespace cmb.schema.csab {
  public class CsabKeyframe {
    public uint Time { get; set; }
    public float Value { get; set; }

    public float? IncomingTangent { get; set; }
    public float? OutgoingTangent { get; set; }
  }
}
