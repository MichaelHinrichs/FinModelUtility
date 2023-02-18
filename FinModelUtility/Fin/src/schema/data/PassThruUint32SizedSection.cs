using schema.binary.attributes.size;
using schema.binary;

namespace fin.schema.data {

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
