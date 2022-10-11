using schema;


namespace ast.schema {
  public partial class Ast : IDeserializable {
    public StrmHeader StrmHeader { get; } = new();

    public IList<short>[] ChannelData { get; private set; }

    public void Read(EndianBinaryReader er) {
      this.StrmHeader.Read(er);

      // TODO: This is almost certainly wrong
      switch (this.StrmHeader.Format) {
        case AstAudioFormat.ADPCM: {
          this.ReadAdpcm_(er);
          break;
        }
        case AstAudioFormat.PCM16: {
          this.ReadPcm16_(er);
          break;
        }
        default: throw new NotImplementedException();
      }
    }
  }
}