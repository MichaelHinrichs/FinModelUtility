namespace granny3d {
  public static class GrannyUtils {
    public static void SubreadRef(
        IEndianBinaryReader er,
        Action<IEndianBinaryReader> subread) {
      var offset = er.ReadUInt64();
      if (offset == 0) {
        return;
      }

      er.Subread((long) offset, subread);
    }

    public static void SubreadRefToArray(
        IEndianBinaryReader er,
        Action<IEndianBinaryReader, uint> subread) {
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