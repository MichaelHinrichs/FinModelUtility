using schema;
using schema.attributes.ignore;


namespace fin.schema.data {
  public interface IMagicSection<T> : ISizedSection<T>
      where T : IBiSerializable {
    string Magic { get; }
  }

  /// <summary>
  ///   Schema class that implements a uint32-sized section without needing to
  ///   worry about passing in an instance of the contained data. This should
  ///   be adequate for most cases, except when the data class needs to access
  ///   parent data.
  /// </summary>
  [BinarySchema]
  public partial class AutoMagicUInt32SizedSection<T> : IMagicSection<T>
      where T : IBiSerializable, new() {
    private readonly PassThruMagicUint32SizedSection<T> impl_;

    public AutoMagicUInt32SizedSection(string magic) {
      this.impl_ = new(magic, new T());
    }

    [Ignore]
    public string Magic => this.impl_.Magic;

    [Ignore]
    public T Data => this.impl_.Data;
  }

  [BinarySchema]
  public partial class PassThruMagicUint32SizedSection<T> : IMagicSection<T>
      where T : IBiSerializable {
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