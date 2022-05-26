using System;

using fin.model;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;


namespace fin.gl {
  public static class GlUtil {
    public static bool IsInitialized { get; private set; }

    public static void Init() {
      if (IsInitialized) {
        return;
      }

      IsInitialized = true;

      Glut.glutInit();
      Glut.glutInitDisplayMode(Glut.GLUT_SINGLE | Glut.GLUT_RGB);

      Wgl.ReloadFunctions();
      Gl.ReloadFunctions();
    }

    public static void SetCulling(CullingMode cullingMode) {
      if (cullingMode == CullingMode.SHOW_BOTH) {
        Gl.glDisable(Gl.GL_CULL_FACE);
        return;
      }

      Gl.glEnable(Gl.GL_CULL_FACE);
      Gl.glCullFace(cullingMode switch {
          CullingMode.SHOW_FRONT_ONLY => Gl.GL_BACK,
          CullingMode.SHOW_BACK_ONLY  => Gl.GL_FRONT,
          CullingMode.SHOW_NEITHER    => Gl.GL_FRONT_AND_BACK,
          _                           => throw new ArgumentOutOfRangeException(nameof(cullingMode), cullingMode, null)
      });
    }
  }
}
