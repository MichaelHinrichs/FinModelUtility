namespace UoT {
  public static class PickerUtil {
    private static byte nextR_;
    private static byte nextG_;
    private static byte nextB_;

    public static void Reset() {
      PickerUtil.nextR_ = PickerUtil.nextG_ = PickerUtil.nextB_ = 0;
    }

    public static void NextRgb(out byte r, out byte g, out byte b) {
      r = PickerUtil.nextR_;
      g = PickerUtil.nextG_;
      b = PickerUtil.nextB_;

      if (++PickerUtil.nextR_ == 0 &&
          ++PickerUtil.nextG_ == 0 &&
          ++PickerUtil.nextB_ == 0) {
        // TODO: Throw an error?
      }
    }
  }
}