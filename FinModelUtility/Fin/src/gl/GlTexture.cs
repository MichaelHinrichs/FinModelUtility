using System;

using fin.model;
using fin.util.image;

using Tao.OpenGl;


namespace fin.gl {
  public class GlTexture : IDisposable {
    private const int UNDEFINED_ID = -1;

    private int id_ = UNDEFINED_ID;

    public GlTexture(ITexture texture) {
      Gl.glGenTextures(1, out int id);
      this.id_ = id;

      var target = Gl.GL_TEXTURE_2D;
      Gl.glBindTexture(target, this.id_);
      {
        var imageData = texture.ImageData;

        var imageWidth = imageData.Width;
        var imageHeight = imageData.Height;

        BitmapUtil.InvokeAsLocked(imageData, bmpData => {
          unsafe {
            var rgba = new byte[4 * imageWidth * imageHeight];

            var scan0 = bmpData.Scan0;
            var ptr = (byte*)bmpData.Scan0.ToPointer();
            for (var y = 0; y < imageHeight; y++) {
              for (var x = 0; x < imageWidth; x++) {
                var i = 4 * (y * imageWidth + x);

                var b = ptr[i];
                var g = ptr[i + 1];
                var r = ptr[i + 2];
                var a = ptr[i + 3];

                rgba[i] = r;
                rgba[i + 1] = g;
                rgba[i + 2] = b;
                rgba[i + 3] = a;
              }
            }

            Gl.glTexImage2D(target, 0, Gl.GL_RGBA, imageWidth, imageHeight, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, rgba);
          }
        });

        // Gl.glGenerateMipmap(target);

        Gl.glTexParameteri(target, Gl.GL_TEXTURE_WRAP_S,
                           GlTexture.ConvertFinWrapToGlWrap_(texture.WrapModeU));
        Gl.glTexParameteri(target, Gl.GL_TEXTURE_WRAP_T,
                           GlTexture.ConvertFinWrapToGlWrap_(texture.WrapModeV));

        Gl.glTexParameteri(target, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST);
        Gl.glTexParameteri(target, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
      }
      Gl.glBindTexture(target, UNDEFINED_ID);
    }

    ~GlTexture() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      var id = this.id_;
      Gl.glDeleteTextures(1, ref id);

      this.id_ = UNDEFINED_ID;
    }

    public void Bind(int textureIndex = 0) {
      Gl.glActiveTexture(Gl.GL_TEXTURE0 + textureIndex);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, this.id_);
    }

    public void Unbind(int textureIndex = 0) {
      Gl.glActiveTexture(Gl.GL_TEXTURE0 + textureIndex);
      Gl.glBindTexture(Gl.GL_TEXTURE_2D, UNDEFINED_ID);
    }

    private static int ConvertFinWrapToGlWrap_(WrapMode wrapMode) =>
        wrapMode switch {
            WrapMode.CLAMP         => Gl.GL_CLAMP,
            WrapMode.REPEAT        => Gl.GL_REPEAT,
            WrapMode.MIRROR_REPEAT => Gl.GL_MIRRORED_REPEAT,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(wrapMode), wrapMode, null)
        };
  }
}