using fin.io;

namespace uni.platforms.gcn.tools {
  public static class MagicTextUtil {
    public static bool Verify(IFileHierarchyFile file, string expected) {
      using var r = file.Impl.OpenRead();
      return expected.All(c => (byte) c == r.ReadByte());
    }
  }
}
