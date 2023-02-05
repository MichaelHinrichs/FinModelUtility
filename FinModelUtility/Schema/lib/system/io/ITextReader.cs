using System.Text.RegularExpressions;

namespace System.IO {
  public interface ITextReader : IDataReader {
    bool Matches(out string text, params string[] matches);

    string ReadUpTo(params string[] matches);
    string ReadWhile(params string[] matches);
  }
}