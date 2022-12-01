namespace System.IO {
  public class EndianBinaryBufferedStream : IEndiannessStack {
    private readonly IEndiannessStack endiannessImpl_;

    public EndianBinaryBufferedStream(Endianness? endianness) {
      this.endiannessImpl_ = new EndiannessStackImpl(endianness);
    }

    public Stream BaseStream { get; set; }
    public byte[] Buffer { get; private set; }

    public void FillBuffer(long count, int? optStride = null) {
      var stride = optStride ?? count;
      if (this.Buffer == null || this.Buffer.Length < count) {
        this.Buffer = new byte[count];
      }
      this.BaseStream.Read(this.Buffer, 0, (int)count);

      if (!this.IsOppositeEndiannessOfSystem) {
        return;
      }
      for (var i = 0L; i < count; i += stride) {
        Array.Reverse(this.Buffer, (int)i, (int)stride);
      }
    }

    public Endianness Endianness => this.endiannessImpl_.Endianness;

    public bool IsOppositeEndiannessOfSystem
      => this.endiannessImpl_.IsOppositeEndiannessOfSystem;

    public void PushStructureEndianness(Endianness endianness)
      => this.endiannessImpl_.PushStructureEndianness(endianness);

    public void PushMemberEndianness(Endianness endianness)
      => this.endiannessImpl_.PushMemberEndianness(endianness);

    public void PopEndianness() => this.endiannessImpl_.PopEndianness();
  }
}