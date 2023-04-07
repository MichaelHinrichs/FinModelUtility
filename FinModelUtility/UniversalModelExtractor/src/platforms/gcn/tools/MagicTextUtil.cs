using fin.io;

namespace uni.platforms.gcn.tools {
  public static class MagicTextUtil {
    public static bool Verify(IReadOnlyGenericFile file, string expected) {
      using var r = file.OpenRead();
      return Verify(r, expected);
    }

    public static bool Verify(Stream stream, string expected)
      => expected.All(c => (byte) c == stream.ReadByte());
  }
}