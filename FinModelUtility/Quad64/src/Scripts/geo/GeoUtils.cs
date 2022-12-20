namespace sm64.scripts.geo {
  public static class GeoUtils {
    public static Endianness Endianness => Endianness.BigEndian;

    public static void SplitAddress(uint address,
                                    out byte segment,
                                    out uint offset) {
      segment = (byte)(address >> 24);
      offset = address & 0xFFFFFF;
    }

    public static uint MergeAddress(byte segment, uint offset)
      => (uint)((segment << 24) | (offset & 0xFFFFFF));

    public static bool IsDisplayListAndDrawingLayerEnabled(byte param)
      => (param & 0x80) != 0;

    public static GeoDrawingLayer GetDrawingLayerFromParams(byte param)
      => (GeoDrawingLayer)
          (IsDisplayListAndDrawingLayerEnabled(param) ? param & 0xF : 0);

    public static GeoTranslateAndRotateFormat GetTranslateAndRotateFormat(
        byte param)
      => (GeoTranslateAndRotateFormat)((param & 0x70) >> 4);
  }
}