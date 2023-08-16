using System.Text;

namespace fin.util.strings {
  public static class StringExtensions {
    public static string Reverse(this string str) {
      var sb = new StringBuilder();
      for (var i = str.Length - 1; i >= 0; --i) {
        sb.Append(str[i]);
      }
      return sb.ToString();
    }
  }
}
