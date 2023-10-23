namespace modl.schema.anim {
  public static class WeirdFloatMath {
    public static ulong Concat44(uint first, uint second)
      => ((ulong) first << 32) | second;

    public static double InterpretAsDouble(ulong value) {
      Span<byte> bytes = stackalloc byte[8];
      BitConverter.TryWriteBytes(bytes, value);
      return BitConverter.ToDouble(bytes);
    }

    public static float InterpretAsSingle(uint value) {
      Span<byte> bytes = stackalloc byte[4];
      BitConverter.TryWriteBytes(bytes, value);
      return BitConverter.ToSingle(bytes);
    }

    public static double CreateWeirdDoubleFromUInt32(uint value)
      => WeirdFloatMath.InterpretAsDouble(
             WeirdFloatMath.Concat44(0x43300000, value)) -
         WeirdFloatMath.InterpretAsDouble(0x4330000000000000);
  }
}