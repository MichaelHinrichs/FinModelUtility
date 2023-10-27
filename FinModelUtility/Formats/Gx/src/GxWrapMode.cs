using fin.model;

namespace gx {
  [Flags]
  public enum GxWrapMode : byte {
    GX_CLAMP,
    GX_REPEAT,
    GX_MIRROR,
  }

  public static class GxWrapModeExtensions {
    public static WrapMode ToFinWrapMode(this GxWrapMode gxWrapMode) {
      var mirror = (gxWrapMode & GxWrapMode.GX_MIRROR) != 0;
      var repeat = (gxWrapMode & GxWrapMode.GX_REPEAT) != 0;

      if (mirror && repeat) {
        return WrapMode.MIRROR_REPEAT;
      }

      if (mirror) {
        return WrapMode.MIRROR_CLAMP;
      }

      if (repeat) {
        return WrapMode.REPEAT;
      }

      return WrapMode.CLAMP;
    }
  }
}