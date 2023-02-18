using schema.binary;
using schema.binary.attributes.ignore;

namespace fin.schema.data {
  [BinarySchema]
  public partial class PassThruMagicUint32SizedSection<T> : IMagicSection<T>
      where T : IBinaryConvertible {
    public string Magic { get; }

    private readonly PassThruUint32SizedSection<T> impl_;

    public PassThruMagicUint32SizedSection(
        string magic,
        T data) {
      this.Magic = magic;
      this.impl_ = new PassThruUint32SizedSection<T>(data);
    }

    [Ignore]
    public T Data => this.impl_.Data;
  }
}