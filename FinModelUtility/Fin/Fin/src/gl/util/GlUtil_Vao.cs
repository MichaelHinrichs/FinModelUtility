using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    private static int currentVaoId_ = -1;

    public static void BindVao(int vaoId) {
      if (GlUtil.currentVaoId_ == vaoId) {
        return;
      }

      GL.BindVertexArray(GlUtil.currentVaoId_ = vaoId);
    }
  }
}