using System.Runtime.CompilerServices;

using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static partial class GlUtil {

    private static int activeTexture_ = -1;

    private static int[] currentTextureBindings_ =
        new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindTexture(int textureIndex, int value) {
      if (GlUtil.currentTextureBindings_[textureIndex] == value) {
        return;
      }

      if (GlUtil.activeTexture_ != textureIndex) {
        GL.ActiveTexture(TextureUnit.Texture0 +
                         (GlUtil.activeTexture_ = textureIndex));
      }

      GL.BindTexture(TextureTarget.Texture2D,
                     GlUtil.currentTextureBindings_[textureIndex] = value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnbindTexture(int textureIndex)
      => BindTexture(textureIndex, -1);
  }
}