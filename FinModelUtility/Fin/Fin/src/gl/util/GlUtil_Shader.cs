using System.Runtime.CompilerServices;

using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {
    private static int currentShader_ = -1;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UseProgram(int shader) {
      if (GlUtil.currentShader_ == shader) {
        return;
      }

      GlUtil.currentShader_ = shader;
      GL.UseProgram(shader);
    }
  }
}