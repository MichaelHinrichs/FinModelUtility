namespace System.IO {
  public interface ITextReader {
    long Position { get; set; }

    char ReadChar();

    void AssertChar(char expectedValue);
    void AssertString(string expectedValue);

    bool Matches(out string text, string primary, params string[] secondary);
    bool Matches(out string text, params string[] matches);

    string ReadUpTo(string primary, params string[] secondary);
    string ReadWhile(string primary, params string[] secondary);
  }
}