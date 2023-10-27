using fin.model;
using fin.util.enums;

namespace gx {
  [Flags]
  public enum GxWrapMode : byte {
    GX_CLAMP,
    GX_REPEAT,
    GX_MIRROR,
  }

  public static class GxWrapModeExtensions {
    public static WrapMode ToFinWrapMode(this GxWrapMode gxWrapMode) {
      var mirror = gxWrapMode.CheckFlag(GxWrapMode.GX_MIRROR);
      var repeat = gxWrapMode.CheckFlag(GxWrapMode.GX_CLAMP);

      if (mirror) {
        return WrapMode.MIRROR_REPEAT;
      }

      if (repeat) {
        return WrapMode.REPEAT;
      }

      return WrapMode.CLAMP;
    }
  }
}