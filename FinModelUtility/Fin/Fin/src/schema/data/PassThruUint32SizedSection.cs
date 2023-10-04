using System.Threading.Tasks;

using fin.util.tasks;

using schema.binary;

namespace fin.schema.data {
  public class PassThruUInt32SizedSection<T> : ISizedSection<T>
      where T : IBinaryConvertible {
    public PassThruUInt32SizedSection(T data) {
      this.Data = data;
    }

    public uint Size { get; private set; }
    public T Data { get; set; }

    public int TweakReadSize { get; set; }

    public void Read(IEndianBinaryReader er) {
      this.Size = er.ReadUInt32();

      var tweakedSize = this.Size + this.TweakReadSize;
      var basePosition = er.Position;
      er.SubreadAt(er.Position, (int) tweakedSize, this.Data.Read);

      er.Position = basePosition + tweakedSize;
    }

    public void Write(ISubEndianBinaryWriter ew) {
      var sizeSource = new TaskCompletionSource<uint>();
      ew.WriteUInt32Delayed(sizeSource.Task);

      var startingPositionTask = ew.GetAbsolutePosition();
      this.Data.Write(ew);
      var endPositionTask = ew.GetAbsolutePosition();

      var sizeTask = endPositionTask.Subtract(startingPositionTask);
      var tweakedSizeTask = sizeTask.Subtract(this.TweakReadSize)
                                    .CastChecked<long, uint>();

      tweakedSizeTask.ThenSetResult(sizeSource);
    }
  }
}