using Tao.OpenGl;

namespace UoT {
  public static class GlConstants {
    public const double NEAR = 1;
    public const double FAR = 999999;

    private static readonly float[] LAmbient = {1.0F, 1.0F, 1.0F, 1.0F};
    private static readonly float[] LDiffuse = {1.0F, 1.0F, 1.0F, 1.0F};
    private static readonly float[] LPosition = {1.0F, 1.0F, 1.0F, 1.0F};

    public static void ResetGl() {
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glEnable(Gl.GL_POINT_SMOOTH);
      Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

      Gl.glClearDepth(5.0F);

      Gl.glDepthFunc(Gl.GL_LEQUAL);
      Gl.glEnable(Gl.GL_DEPTH_TEST);
      Gl.glDepthMask(Gl.GL_TRUE);

      Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, LAmbient);
      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, LDiffuse);
      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, LAmbient);
      Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, LPosition);

      Gl.glEnable(Gl.GL_LIGHT0);

      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_NORMALIZE);

      Gl.glEnable(Gl.GL_CULL_FACE);
      Gl.glCullFace(Gl.GL_BACK);

      Gl.glEnable(Gl.GL_BLEND);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

      Gl.glClearColor(0.2f, 0.5f, 0.7f, 1);
    }
  }
}