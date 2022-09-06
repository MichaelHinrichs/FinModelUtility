namespace granny3d {
  public static class GrannyUtils {
    public static void SubreadRef(
        EndianBinaryReader er,
        Action<EndianBinaryReader> subread) {
      var offset = er.ReadUInt64();
      if (offset == 0) {
        return;
      }

      er.Subread((long) offset, subread);
    }

    public static void SubreadRefToArray(
        EndianBinaryReader er,
        Action<EndianBinaryReader, uint> subread) {
      var count = er.ReadUInt32();
      var offset = er.ReadUInt64();
      if (offset == 0) {
        return;
      }

      er.Subread((long)offset, ser => {
        subread(ser, count);
      });
    }
  }
}