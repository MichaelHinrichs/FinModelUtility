using gx;

using schema.binary;

namespace jsystem.schema.bmd.mat3 {
  [BinarySchema]
  public partial class TevSwapModeTable : ITevSwapModeTable, IBinaryConvertible {
    public ChannelId R { get; set; }
    public ChannelId G { get; set; }
    public ChannelId B { get; set; }
    public ChannelId A { get; set; }
  }
}
