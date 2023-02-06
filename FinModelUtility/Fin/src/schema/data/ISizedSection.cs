using schema.binary;
using schema.binary.attributes.ignore;
using schema.binary.attributes.size;


namespace fin.schema.data {
  public interface ISizedSection<T> : IBinaryConvertible
      where T : IBinaryConvertible {
    T Data { get; }
  }

  /// <summary>
  ///   Schema class that implements a uint32-sized section without needing to
  ///   worry about passing in an instance of the contained data. This should
  ///   be adequate for most cases, except when the data class needs to access
  ///   parent data.
  /// </summary>
  [BinarySchema]
  public partial class AutoUInt32SizedSection<T> : ISizedSection<T>
      where T : IBinaryConvertible, new() {
    private readonly PassThruUint32SizedSection<T> impl_;

    [Ignore]
    public T Data => this.impl_.Data;

    public AutoUInt32SizedSection() {
      this.impl_ = new(new T());
    }
  }

  [BinarySchema]
  public partial class PassThruUint32SizedSection<T> : ISizedSection<T>
      where T : IBinaryConvertible {
    [SizeOfMemberInBytes(nameof(Data))]
    private uint size_;

    public T Data { get; }

    public PassThruUint32SizedSection(T data) {
      this.Data = data;
    }
  }
}