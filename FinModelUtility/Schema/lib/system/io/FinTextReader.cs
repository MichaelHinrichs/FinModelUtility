namespace System.IO {
  public sealed partial class FinTextReader : ITextReader {
    private readonly Stream baseStream_;

    public FinTextReader(Stream baseStream) {
      this.baseStream_ = baseStream;
    }

    ~FinTextReader() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.baseStream_.Dispose();

    public long Position {
      get => this.baseStream_.Position;
      set => this.baseStream_.Position = value;
    }

    public long Length => this.baseStream_.Length;
    public bool Eof => this.Position >= this.Length;
  }
}