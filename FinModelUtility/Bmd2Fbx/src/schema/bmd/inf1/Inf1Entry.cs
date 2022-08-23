using schema;


namespace bmd.schema.bmd.inf1 {
  [BinarySchema]
  public partial class Inf1Entry : IBiSerializable {
    public ushort Type;
    public ushort Index;

    public override string ToString()
      => this.Type switch {
          0  => "Terminator",
          1  => "Hierarchy Down",
          2  => "Hierarchy Up",
          16 => "Joint (" + (object) this.Index + ")",
          17 => "Material (" + (object) this.Index + ")",
          18 => "Shape (" + (object) this.Index + ")",
          _  => "Unknown"
      };
  }
}