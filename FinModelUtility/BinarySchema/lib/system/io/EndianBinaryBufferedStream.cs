namespace System.IO {
  public class EndianBinaryBufferedStream {
    private Endianness endianness_;

    public Endianness Endianness {
      get => this.endianness_;
      set {
        this.endianness_ = value;
        this.ShouldReverseBytes =
            EndiannessUtil.SystemEndianness != this.Endianness;
      }
    }

    public bool ShouldReverseBytes { get; private set; }

    public Stream BaseStream { get; set; }
    public byte[] Buffer { get; private set; }

    public void FillBuffer(int count, int? optStride = null) {
      var stride = optStride ?? count;
      if (this.Buffer == null || this.Buffer.Length < count) {
        this.Buffer = new byte[count];
      }
      this.BaseStream.Read(this.Buffer, 0, count);

      if (!this.ShouldReverseBytes) {
        return;
      }
      for (var i = 0; i < count; i += stride) {
        Array.Reverse(this.Buffer, i, stride);
      }
    }
  }
}