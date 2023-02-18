using System.IO;

using schema.binary.attributes.size;
using schema.binary;
using schema.binary.attributes.ignore;

namespace fin.schema.data {
  [BinarySchema]
  public partial class PassThruUint32SizedSection<T> : ISizedSection<T>
      where T : IBinaryConvertible {
    [Ignore]
    private readonly int tweakSize_;

    [SizeOfMemberInBytes(nameof(Data))]
    private uint size_;

    public T Data { get; }

    public PassThruUint32SizedSection(T data) {
      this.Data = data;
    }

    public PassThruUint32SizedSection(T data, int tweakSize) {
      this.Data = data;
      this.tweakSize_ = tweakSize;
    }

    public void Read(IEndianBinaryReader er) {
      this.size_ = er.ReadUInt32();
      
      var useSize = this.size_ + this.tweakSize_;
      var basePosition = er.Position;
      er.Subread(er.Position, (int) useSize, this.Data.Read);
      er.Position = basePosition + useSize;
    }
  }
}