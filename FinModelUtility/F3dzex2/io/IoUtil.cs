using System.Runtime.CompilerServices;

namespace f3dzex2.io {
  public static class IoUtil {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SplitAddress(
        uint address,
        out byte bank,
        out uint offset) {
      bank = (byte) (address >> 24);
      offset = address << 8 >> 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint MergeAddress(byte bank, uint offset)
      => (uint) ((bank << 24) | (offset & 0x00ffffff));
  }
}