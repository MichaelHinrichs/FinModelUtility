using System;

using fin.model;
using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    private static CullingMode currentCullingMode_ = CullingMode.SHOW_NEITHER;

    public static void ResetCulling()
      => SetCulling(CullingMode.SHOW_FRONT_ONLY);


    public static bool SetCulling(CullingMode cullingMode) {
      if (GlUtil.currentCullingMode_ == cullingMode) {
        return false;
      }

      GlUtil.currentCullingMode_ = cullingMode;

      if (cullingMode == CullingMode.SHOW_BOTH) {
        GL.Disable(EnableCap.CullFace);
      } else {
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(cullingMode switch {
            CullingMode.SHOW_FRONT_ONLY => CullFaceMode.Back,
            CullingMode.SHOW_BACK_ONLY  => CullFaceMode.Front,
            CullingMode.SHOW_NEITHER    => CullFaceMode.FrontAndBack,
            _ => throw new ArgumentOutOfRangeException(
                nameof(cullingMode), cullingMode, null)
        });
      }

      return true;
    }
  }
}