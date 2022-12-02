using System.Threading.Tasks;


namespace System.IO {
  public sealed partial class EndianBinaryWriter {
    public EndianBinaryWriter EnterBlock(out Task<long> delayedLength)
      => new(this.Endianness,
             this.impl_.EnterBlock(out delayedLength));

    public Task CompleteAndCopyToDelayed(Stream stream)
      => this.impl_.CompleteAndCopyToDelayed(stream);

    public Task<long> GetAbsoluteDelayedPosition()
      => this.impl_.GetAbsoluteDelayedPosition();

    public Task<long> GetAbsoluteDelayedLength()
      => this.impl_.GetAbsoluteDelayedLength();

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