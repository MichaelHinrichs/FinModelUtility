namespace System.IO {
  public sealed partial class FinTextReader : ITextReader {
    private readonly Stream impl_;

    public FinTextReader(Stream impl) {
      this.impl_ = impl;
    }

    ~FinTextReader() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() => this.impl_.Dispose();

    public long Position {
      get => this.impl_.Position;
      set => this.impl_.Position = value;
    }
  }
}