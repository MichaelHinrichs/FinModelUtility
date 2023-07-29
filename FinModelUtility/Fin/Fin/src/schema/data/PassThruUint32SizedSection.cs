using System.IO;

using schema.binary;
using schema.binary.attributes;

namespace fin.schema.data {
  [BinarySchema]
  public partial class PassThruUInt32SizedSection<T> : ISizedSection<T>
      where T : IBinaryConvertible {
    [WSizeOfMemberInBytes(nameof(Data))]
    private uint size_;

    [Ignore]
    public uint Size => this.size_;

    public T Data { get; set; }

    public PassThruUInt32SizedSection(T data) {
      this.Data = data;
    }

    [Ignore]
    public int TweakReadSize { get; set; }

    [Ignore]
    public bool UseLocalSpace { get; set; } = true;


    public void Read(IEndianBinaryReader er) {
      this.size_ = er.ReadUInt32();

      var useSize = this.size_ + this.TweakReadSize;
      var basePosition = er.Position;
      if (this.UseLocalSpace) {
        er.Subread(er.Position, (int) useSize, this.Data.Read);
      } else {
        this.Data.Read(er);
      }

      er.Position = basePosition + useSize;
    }
  }
}