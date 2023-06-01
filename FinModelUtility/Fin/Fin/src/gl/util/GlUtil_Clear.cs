using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    public static void ClearColorAndDepth() {
      GlUtil.ResetDepth();
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }
  }
}