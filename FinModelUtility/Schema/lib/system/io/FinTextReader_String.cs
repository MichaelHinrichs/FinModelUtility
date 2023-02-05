using System.Linq;
using System.Text;

using schema.binary.util;

using static System.Net.Mime.MediaTypeNames;

namespace System.IO {
  public sealed partial class FinTextReader {
    public char ReadChar() => (char) this.baseStream_.ReadByte();

    public void AssertChar(char expectedValue)
      => Asserts.Equal(expectedValue, this.ReadChar());

    public void AssertString(string expectedValue) {
      for (var i = 0; i < expectedValue.Length; ++i) {
        AssertChar(expectedValue[i]);
      }
    }

    public bool Matches(out string text,
                        string primary,
                        params string[] secondary)
      => Matches(out text, new[] { primary }.Concat(secondary).ToArray());

    public bool Matches(out string text, string[] matches) {
      var originalPosition = this.Position;
      foreach (var match in matches) {
        foreach (var c in match) {
          if (c != this.ReadChar()) {
            goto DidNotMatch;
          }
        }

        text = match;
        return true;

        DidNotMatch:
        this.Position = originalPosition;
      }

      text = String.Empty;
      return false;
    }

    public string ReadUpTo(string primary, params string[] secondary) {
      var sb = new StringBuilder();

      var matches = new[] { primary }.Concat(secondary).ToArray();

      while (!Eof) {
        if (!Matches(out var text, matches)) {
          sb.Append(this.ReadChar());
        } else {
          this.Position -= text.Length;
          break;
        }
      }

      return sb.ToString();
    }

    public string ReadWhile(string primary, params string[] secondary) {
      var sb = new StringBuilder();

      var matches = new[] { primary }.Concat(secondary).ToArray();
      while (!Eof && this.Matches(out var text, matches)) {
        sb.Append(text);
      }

      return sb.ToString();
    }
  }
}