namespace System.IO {
  public class EndianBinaryBufferedStream : IEndiannessStack {
    private readonly IEndiannessStack endiannessImpl_ =
        new EndiannessStackImpl();

    public Stream BaseStream { get; set; }
    public byte[] Buffer { get; private set; }

    public void FillBuffer(int count, int? optStride = null) {
      var stride = optStride ?? count;
      if (this.Buffer == null || this.Buffer.Length < count) {
        this.Buffer = new byte[count];
      }
      this.BaseStream.Read(this.Buffer, 0, count);

      if (!this.IsOppositeEndiannessOfSystem) {
        return;
      }
      for (var i = 0; i < count; i += stride) {
        Array.Reverse(this.Buffer, i, stride);
      }
    }

    public Endianness Endianness {
      get => this.endiannessImpl_.Endianness;
      set => this.endiannessImpl_.Endianness = value;
    }

    public bool IsOppositeEndiannessOfSystem
      => this.endiannessImpl_.IsOppositeEndiannessOfSystem;

    public void PushClassEndianness(Endianness endianness)
      => this.endiannessImpl_.PushClassEndianness(endianness);

    public void PushFieldEndianness(Endianness endianness)
      => this.endiannessImpl_.PushFieldEndianness(endianness);

    public void PopEndianness() => this.endiannessImpl_.PopEndianness();
  }
}