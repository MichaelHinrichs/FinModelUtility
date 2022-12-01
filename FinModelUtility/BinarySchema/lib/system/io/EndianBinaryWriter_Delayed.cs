using System.Threading.Tasks;

using schema.io;


namespace System.IO {
  public sealed partial class EndianBinaryWriter {
    public Task<long> EnterBlockAndGetDelayedLength(
        Action<IDelayedContentOutputStream, Task<long>> handler)
      => this.impl_.EnterBlockAndGetDelayedLength(handler);

    public Task CompleteAndCopyToDelayed(Stream stream)
      => this.impl_.CompleteAndCopyToDelayed(stream);

    public Task<long> GetDelayedPosition() => this.impl_.GetDelayedPosition();
    public Task<long> GetDelayedLength() => this.impl_.GetDelayedLength();

    private void WriteBufferDelayed_(Task<byte[]> delayedBytes) {
      var isReversed = this.IsOppositeEndiannessOfSystem;
      this.impl_.WriteDelayed(
          delayedBytes.ContinueWith(bytesTask => {
            var bytes = bytesTask.Result;
            if (isReversed) {
              Array.Reverse(bytes, 0, bytes.Length);
            }
            return bytes;
          }));
    }

    public void WriteUInt32Delayed(Task<uint> delayedValue) {
      this.WriteBufferDelayed_(
          delayedValue.ContinueWith(
              valueTask => BitConverter.GetBytes(valueTask.Result)));
    }
  }
}