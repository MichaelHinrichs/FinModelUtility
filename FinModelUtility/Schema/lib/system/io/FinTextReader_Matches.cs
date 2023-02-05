using System.Text;

namespace System.IO {
  public sealed partial class FinTextReader {
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

    public string ReadUpTo(params string[] terminators) {
      var sb = new StringBuilder();

      while (!Eof) {
        if (!Matches(out var text, terminators)) {
          sb.Append(this.ReadChar());
        } else {
          this.Position -= text.Length;
          break;
        }
      }

      this.ReadWhile(terminators);

      return sb.ToString();
    }

    public string ReadWhile(params string[] matches) {
      var sb = new StringBuilder();

      while (!Eof && this.Matches(out var text, matches)) {
        sb.Append(text);
      }

      return sb.ToString();
    }
  }
}