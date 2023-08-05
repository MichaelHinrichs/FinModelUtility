using System.IO;

using schema.binary;

namespace fin.schema.data {
  public class StringMagicConfig<TData> : IMagicConfig<string, TData>
      where TData : IBinaryConvertible {
    private readonly uint length_;

    public StringMagicConfig(uint length) {
      this.length_ = length;
    }

    public string ReadMagic(IEndianBinaryReader er)
      => er.ReadString(this.length_);

    public void WriteMagic(ISubEndianBinaryWriter ew, string magic)
      => ew.WriteString(magic);

    public string GetMagic(TData magic) {

    }
  }
}