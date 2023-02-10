using fin.io;

namespace uni.platforms.gcn.tools {
  public static class MagicTextUtil {
    public static bool Verify(IFileHierarchyFile file, string expected) {
      using var r = file.Impl.OpenRead();
      return Verify(r, expected);
    }

    public static bool Verify(Stream stream, string expected)
      => expected.All(c => (byte) c == stream.ReadByte());
  }
}