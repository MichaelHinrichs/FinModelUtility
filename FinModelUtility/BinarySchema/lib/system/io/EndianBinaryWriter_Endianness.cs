namespace System.IO {
  public sealed partial class EndianBinaryWriter : IEndiannessStack {
    private readonly IEndiannessStack endiannessImpl_ =
        new EndiannessStackImpl();

    public Endianness Endianness => this.endiannessImpl_.Endianness;

    public bool Reverse => this.endiannessImpl_.Reverse;

    public void PushClassEndianness(Endianness endianness)
      => this.endiannessImpl_.PushClassEndianness(endianness);

    public void PushFieldEndianness(Endianness endianness)
      => this.endiannessImpl_.PushFieldEndianness(endianness);

    public void PopEndianness() => this.endiannessImpl_.PopEndianness();
  }
}