namespace modl.schema.anim {
  internal static class WeirdFloatMath {
    public static ulong CONCAT44_(uint first, uint second) =>
        ((ulong) first << 32) | second;

    public static double INTERPRET_AS_DOUBLE_(ulong value) {
      Span<byte> bytes = stackalloc byte[8];
      BitConverter.TryWriteBytes(bytes, value);
      return BitConverter.ToDouble(bytes);
    }

    public static float INTERPRET_AS_SINGLE_(uint value) {
      Span<byte> bytes = stackalloc byte[4];
      BitConverter.TryWriteBytes(bytes, value);
      return BitConverter.ToSingle(bytes);
    }
  }
}
