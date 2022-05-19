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
  }
}
