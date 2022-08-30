namespace granny3d {
  public static class GrannyUtils {
    public static void SubreadUInt64Pointer(
        EndianBinaryReader er,
        Action<EndianBinaryReader> subread) {
      var offset = er.ReadUInt64();
      if (offset == 0) {
        return;
      }

      er.Subread((long) offset, subread);
    }
  }
}