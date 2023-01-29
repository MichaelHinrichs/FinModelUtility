using System.Reflection.Emit;
using System.Text;


namespace System.IO {
  public sealed partial class EndianBinaryReader {
    public void AssertChar(char expectedValue)
      => this.AssertChar(Encoding.ASCII, expectedValue);

    public char ReadChar() => this.ReadChar(Encoding.ASCII);

    public char[] ReadChars(long count) =>
        this.ReadChars(Encoding.ASCII, count);

    public char[] ReadChars(char[] dst) => this.ReadChars(Encoding.ASCII, dst);


    public void AssertChar(Encoding encoding, char expectedValue)
      => EndianBinaryReader.Assert(expectedValue, this.ReadChar(encoding));

    public char ReadChar(Encoding encoding) {
      this.AssertNotEof();
      var encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.BufferedStream_.FillBuffer(encodingSize, encodingSize);
      return encoding.GetChars(this.BufferedStream_.Buffer, 0, encodingSize)[0];
    }

    public char[] ReadChars(Encoding encoding, long count)
      => this.ReadChars(encoding, new char[count]);

    public char[] ReadChars(Encoding encoding, char[] dst) {
      var encodingSize = EndianBinaryReader.GetEncodingSize_(encoding);
      this.BufferedStream_.FillBuffer(encodingSize * dst.Length, encodingSize);
      encoding.GetChars(this.BufferedStream_.Buffer,
                        0,
                        encodingSize * dst.Length,
                        dst,
                        0);
      return dst;
    }

    private static int GetEncodingSize_(Encoding encoding) {
      return encoding == Encoding.UTF8 ||
             encoding == Encoding.ASCII ||
             encoding != Encoding.Unicode &&
             encoding != Encoding.BigEndianUnicode
                 ? 1
                 : 2;
    }

    public string ReadUpTo(char endToken) => ReadUpTo(Encoding.ASCII, endToken);
    public string ReadUpTo(Encoding encoding, char endToken) {
      var strBuilder = new StringBuilder();
      while (!Eof) {
        var c = this.ReadChar(encoding);
        if (c == endToken) {
          break;
        }

        strBuilder.Append(c);
      }
      return strBuilder.ToString();
    }

    public string ReadUpTo(params string[] endTokens) => ReadUpTo(Encoding.ASCII, endTokens);
    public string ReadUpTo(Encoding encoding, params string[] endTokens) {
      var strBuilder = new StringBuilder();
      while (!Eof) {
        var firstC = this.ReadChar(encoding);
        var originalOffset = Position;
        
        foreach (var endToken in endTokens) {
          if (firstC == endToken[0]) {
            for (var i = 1; i < endToken.Length; ++i) {
              var c = this.ReadChar(encoding);
              if (c != endToken[1]) {
                Position = originalOffset;
                break;
              }
            }
            goto Done;
          }
        }

        strBuilder.Append(firstC);
      }
      Done:
      return strBuilder.ToString();
    }


    public string ReadLine() => ReadLine(Encoding.ASCII);
    public string ReadLine(Encoding encoding) => ReadUpTo(encoding, "\n", "\r\n");


    public void AssertString(string expectedValue)
      => this.AssertString(Encoding.ASCII, expectedValue);

    public string ReadString(long count)
      => this.ReadString(Encoding.ASCII, count);


    public void AssertString(Encoding encoding, string expectedValue)
      => EndianBinaryReader.Assert(
          expectedValue.TrimEnd('\0'),
          this.ReadString(encoding, expectedValue.Length));

    public string ReadString(Encoding encoding, long count) {
      this.AssertNotEof();
      return new string(this.ReadChars(encoding, count)).TrimEnd('\0');
    }


    public void AssertStringNT(string expectedValue)
      => this.AssertStringNT(Encoding.ASCII, expectedValue);

    public string ReadStringNT() => this.ReadStringNT(Encoding.ASCII);


    public void AssertStringNT(Encoding encoding, string expectedValue)
      => EndianBinaryReader.Assert(
          expectedValue,
          this.ReadStringNT(encoding));

    public string ReadStringNT(Encoding encoding) => ReadUpTo(encoding, '\0');

    public void AssertMagicText(string expectedText) {
      var actualText = this.ReadString(expectedText.Length);

      if (expectedText != actualText) {
        throw new Exception(
            $"Expected to find magic text \"{expectedText}\", but found \"{actualText}\"");
      }
    }
  }
}