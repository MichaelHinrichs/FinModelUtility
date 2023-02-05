namespace System.IO {
  public sealed partial class TextReader : ITextReader {
    private readonly Stream impl_;

    public TextReader(Stream impl) {
      this.impl_ = impl;
    }

    public long Position {
      get => this.impl_.Position;
      set => this.impl_.Position = value;
    }
  }
}