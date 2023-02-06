using gx;
using schema.binary;


namespace j3d.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevSwapModeTable : ITevSwapModeTable, IBinaryConvertible {
    public ChannelId R { get; set; }
    public ChannelId G { get; set; }
    public ChannelId B { get; set; }
    public ChannelId A { get; set; }
  }
}
