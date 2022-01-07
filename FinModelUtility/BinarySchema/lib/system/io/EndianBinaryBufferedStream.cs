namespace System.IO {
  public class EndianBinaryBufferedStream {
    private static readonly Endianness SystemEndianness_
        = BitConverter.IsLittleEndian
              ? Endianness.LittleEndian
              : Endianness.BigEndian;

    public Endianness Endianness { get; set; }

    private bool ShouldReverseBytes_
      => EndianBinaryBufferedStream.SystemEndianness_ != this.Endianness;

    public Stream BaseStream { get; set; }
    public byte[] Buffer { get; private set; }

    public void FillBuffer(int count, int? optStride = null) {
      var stride = optStride ?? count;
      if (this.Buffer == null || this.Buffer.Length < count) {
        this.Buffer = new byte[count];
      }
      this.BaseStream.Read(this.Buffer, 0, count);

      if (!this.ShouldReverseBytes_) {
        return;
      }
      for (var i = 0; i < count; i += stride) {
        Array.Reverse(this.Buffer, i, stride);
      }
    }
  }
}