using System.Threading.Tasks;


namespace System.IO {
  public sealed partial class EndianBinaryWriter {
    public ISubEndianBinaryWriter EnterBlock(out Task<long> delayedLength)
      => new EndianBinaryWriter(this.Endianness,
                                this.impl_.EnterBlock(out delayedLength));

    public Task<long> GetAbsolutePosition() => this.impl_.GetAbsolutePosition();

    public Task<long> GetAbsoluteLength() => this.impl_.GetAbsoluteLength();

    public Task<long> GetStartPositionOfSubStream()
      => this.impl_.GetStartPositionOfSubStream();

    public Task<long> GetPositionInSubStream() 
      => this.impl_.GetPositionInSubStream();

    public Task<long> GetLengthOfSubStream() => this.GetLengthOfSubStream();

    public Task CompleteAndCopyToDelayed(Stream stream)
      => this.impl_.CompleteAndCopyToDelayed(stream);

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