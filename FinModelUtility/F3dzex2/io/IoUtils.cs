using System.Runtime.CompilerServices;

namespace f3dzex2.io {
  public static class IoUtils {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SplitSegmentedAddress(
        uint address,
        out byte bank,
        out uint offset) {
      bank = (byte) (address >> 24);
      offset = address << 8 >> 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint MergeSegmentedAddress(byte bank, uint offset)
      => (uint) ((bank << 24) | (offset & 0x00ffffff));
  }
}