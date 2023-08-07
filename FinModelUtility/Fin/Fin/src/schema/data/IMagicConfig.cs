using System.IO;

using schema.binary;

namespace fin.schema.data {
  public interface IMagicConfig<TMagic, in TData>
      where TMagic : notnull
      where TData : IBinaryConvertible {
    TMagic ReadMagic(IEndianBinaryReader er);
    void WriteMagic(ISubEndianBinaryWriter ew, TMagic magic);

    TMagic GetMagic(TData data);
  }
}