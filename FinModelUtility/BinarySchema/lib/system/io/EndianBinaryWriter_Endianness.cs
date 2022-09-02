namespace System.IO {
  public sealed partial class EndianBinaryWriter : IEndiannessStack {
    private readonly IEndiannessStack endiannessImpl_ =
        new EndiannessStackImpl();

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