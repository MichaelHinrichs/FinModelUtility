using System.IO;

namespace zar.schema.cmb {
  public static class CmbStringUtil {
    public static string ReadString(EndianBinaryReader r, int length)
      => r.ReadString(length).Replace("\0", "");
  }
}