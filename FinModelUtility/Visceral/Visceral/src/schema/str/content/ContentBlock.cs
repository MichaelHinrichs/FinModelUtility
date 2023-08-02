using fin.schema.data;

using schema.binary;

namespace visceral.schema.str.content {
  [BinarySchema]
  public partial class ContentBlock : IBlock {
    public SwitchMagicWrapper<ContentType, IContent> Impl { get; }
      = new(er => (ContentType) er.ReadUInt32(),
            (ew, magic) => ew.WriteUInt32((uint) magic),
            magic => magic switch {
                ContentType.Header         => new FileInfo(),
                ContentType.Data           => new UncompressedData(),
                ContentType.CompressedData => new RefPackCompressedData(),
            });

    public override string ToString() => this.Impl.ToString();
  }
}