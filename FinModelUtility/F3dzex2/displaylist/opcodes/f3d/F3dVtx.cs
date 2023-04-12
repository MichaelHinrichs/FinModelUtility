using schema.binary;

namespace f3dzex2.displaylist.opcodes.f3d {

  [BinarySchema]
  public partial class F3dVtx : IVtx, IBinaryConvertible {
    public short X { get; set; }
    public short Y { get; set; }
    public short Z { get; set; }

    public short Flag { get; set; }

    public short U { get; set; }
    public short V { get; set; }

    public byte NormalXOrR { get; set; }
    public byte NormalYOrG { get; set; }
    public byte NormalZOrB { get; set; }
    public byte A { get; set; }
  }
}
