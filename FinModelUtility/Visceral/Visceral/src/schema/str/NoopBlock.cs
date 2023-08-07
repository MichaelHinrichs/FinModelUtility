using schema.binary;
using schema.binary.attributes;

namespace visceral.schema.str {

  [BinarySchema]
  public partial class NoopBlock : IBlock {
    public NoopBlock(BlockType type) {
      this.Type = type;
    }

    [Ignore]
    public BlockType Type { get; }
  }
}
