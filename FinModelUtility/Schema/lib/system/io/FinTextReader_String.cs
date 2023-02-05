using System.Linq;
using System.Text;

using schema.binary.util;

namespace System.IO {
  public sealed partial class FinTextReader {
    public char ReadChar() => (char) this.impl_.ReadByte();

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
      var position = this.impl_.Position;
      foreach (var match in matches) {
        foreach (var c in match) {
          if (c != this.ReadChar()) {
            goto DidNotMatch;
          }
        }

        text = match;
        return true;

        DidNotMatch:
        this.Position = position;
      }

      text = String.Empty;
      return false;
    }

    public string ReadUpTo(string primary, params string[] secondary) {
      var sb = new StringBuilder();

      var matches = new[] { primary }.Concat(secondary).ToArray();
      while (!Matches(out var text, matches)) {
        sb.Append(text);
      }

      return sb.ToString();
    }

    public string ReadWhile(string primary, params string[] secondary) {
      var sb = new StringBuilder();

      var matches = new[] { primary }.Concat(secondary).ToArray();
      while (Matches(out var text, matches)) {
        sb.Append(text);
      }

      return sb.ToString();
    }
  }
}