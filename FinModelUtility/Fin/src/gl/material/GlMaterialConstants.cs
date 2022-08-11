using System.Drawing;

using fin.util.image;


namespace fin.gl.material {
  public static class GlMaterialConstants {
    public static GlTexture NULL_WHITE_TEXTURE;

    public static GlTexture NULL_GRAY_TEXTURE;

    static GlMaterialConstants() {
      NULL_WHITE_TEXTURE ??=
          new GlTexture(BitmapUtil.Create1x1WithColor(Color.White));
      NULL_WHITE_TEXTURE ??=
          new GlTexture(BitmapUtil.Create1x1WithColor(Color.Gray));
    }

    public static void DisposeIfNotCommon(GlTexture texture) {
      if (texture != NULL_WHITE_TEXTURE && texture != NULL_GRAY_TEXTURE) {
        texture.Dispose();
      }
    }
  }
}