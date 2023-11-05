using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace fin.util.strings {
  public static class StringUtil {
    public static string Repeat(string str, int times) {
      if (times == 0) {
        return "";
      }

      if (times == 1) {
        return str;
      }

      var builder = new StringBuilder();
      for (var i = 0; i < times; ++i) {
        builder.Append(str);
      }

      return builder.ToString();
    }

    public static string Concat(
        IEnumerable<string> strs,
        string separator = "") {
      if (!strs.Any()) {
        return "";
      }

      var builder = new StringBuilder();

      builder.Append(strs.First());

      foreach (var str in strs.Skip(1)) {
        builder.Append(separator);
        builder.Append(str);
      }

      return builder.ToString();
    }

    public static string[] SplitNewlines(string text)
      => Regex.Split(text, "\r\n|\r|\n");

    public static string SubstringUpTo(this string str, char c) {
      var indexTo = str.IndexOf(c);
      return indexTo >= 0 ? str[..indexTo] : str;
    }

    public static string SubstringUpTo(this string str, string substr) {
      var indexTo = str.IndexOf(substr);
      return indexTo >= 0 ? str[..indexTo] : str;
    }

    public static string SubstringAfter(this string str, string substr) {
      var indexTo = str.IndexOf(substr);
      return indexTo >= 0 ? str[(indexTo + substr.Length)..] : str;
    }
  }
}