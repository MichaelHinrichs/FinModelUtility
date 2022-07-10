namespace f3dzex2.io {
  public static class IoUtil {
    public static void SplitAddress(
        uint address,
        out byte bank,
        out uint offset) {
      bank = (byte)(address >> 24);
      offset = address << 8 >> 8;
    }

    public static uint MergeAddress(byte bank, uint offset) {
      return (uint)((bank << 24) | (offset & 0x00ffffff));
    }
  }
}