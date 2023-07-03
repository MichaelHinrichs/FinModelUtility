using schema.binary;

namespace visceral.schema.bnk {
  public class Bnk : IBinaryDeserializable {
    public void Read(IEndianBinaryReader er) {
      er.Position = 0x24;

      var animationHeaderCount = er.ReadUInt32();

      er.Position = 0x2C;
      var animationDataCount = er.ReadUInt32();
    }
  }
}