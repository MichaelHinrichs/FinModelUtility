using System;
using System.Drawing;

using fin.model;
using fin.util.image;

using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public class GlTexture : IDisposable {
    private const int UNDEFINED_ID = -1;

    private int id_ = UNDEFINED_ID;

    public GlTexture(Bitmap bitmap) {
      GL.GenTextures(1, out int id);
      this.id_ = id;

      var target = TextureTarget.Texture2D;
      GL.BindTexture(target, this.id_);
      {
        this.LoadBitmapIntoTexture_(bitmap);
      }
      GL.BindTexture(target, UNDEFINED_ID);
    }

    public GlTexture(ITexture texture) {
      GL.GenTextures(1, out int id);
      this.id_ = id;

      var target = TextureTarget.Texture2D;
      GL.BindTexture(target, this.id_);
      {
        this.LoadBitmapIntoTexture_(texture.ImageData);

        // Gl.glGenerateMipmap(target);

        GL.TexParameter(target,
                        TextureParameterName.TextureWrapS,
                        (int) GlTexture.ConvertFinWrapToGlWrap_(
                            texture.WrapModeU));
        GL.TexParameter(target,
                        TextureParameterName.TextureWrapT,
                        (int) GlTexture.ConvertFinWrapToGlWrap_(
                            texture.WrapModeV));

        GL.TexParameter(target, TextureParameterName.TextureMinFilter,
                        (int) TextureMinFilter.Nearest);
        GL.TexParameter(target, TextureParameterName.TextureMagFilter,
                        (int) TextureMagFilter.Linear);
      }
      GL.BindTexture(target, UNDEFINED_ID);
    }

    private void LoadBitmapIntoTexture_(Bitmap imageData) {
      var imageWidth = imageData.Width;
      var imageHeight = imageData.Height;

      BitmapUtil.InvokeAsLocked(imageData, bmpData => {
        unsafe {
          var ptr = (byte*) bmpData.Scan0.ToPointer();
          var rgba = new byte[4 * imageWidth * imageHeight];
          switch (bmpData.PixelFormat) {
            case System.Drawing.Imaging.PixelFormat.Format32bppArgb: {
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
              break;
            }
            case System.Drawing.Imaging.PixelFormat.Format24bppRgb: {
              for (var y = 0; y < imageHeight; y++) {
                for (var x = 0; x < imageWidth; x++) {
                  var i = 3 * (y * imageWidth + x);

                  var b = ptr[i];
                  var g = ptr[i + 1];
                  var r = ptr[i + 2];

                  rgba[i] = r;
                  rgba[i + 1] = g;
                  rgba[i + 2] = b;
                  rgba[i + 3] = 255;
                }
              }
              break;
            }
            default: throw new NotImplementedException();
          }
          GL.TexImage2D(TextureTarget.Texture2D,
                        0,
                        PixelInternalFormat.Rgba,
                        imageWidth, imageHeight,
                        0,
                        PixelFormat.Rgba,
                        PixelType.UnsignedByte,
                        rgba);
        }
      });
    }

    ~GlTexture() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      var id = this.id_;
      GL.DeleteTextures(1, ref id);

      this.id_ = UNDEFINED_ID;
    }

    public void Bind(int textureIndex = 0) {
      GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
      GL.BindTexture(TextureTarget.Texture2D, this.id_);
    }

    public void Unbind(int textureIndex = 0) {
      GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
      GL.BindTexture(TextureTarget.Texture2D, UNDEFINED_ID);
    }

    private static TextureWrapMode ConvertFinWrapToGlWrap_(WrapMode wrapMode) =>
        wrapMode switch {
            WrapMode.CLAMP         => TextureWrapMode.ClampToEdge,
            WrapMode.REPEAT        => TextureWrapMode.Repeat,
            WrapMode.MIRROR_REPEAT => TextureWrapMode.MirroredRepeat,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(wrapMode), wrapMode, null)
        };
  }
}