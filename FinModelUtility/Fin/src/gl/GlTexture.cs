using System;
using System.Drawing;

using fin.image;
using fin.model;
using fin.util.image;

using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public class GlTexture : IDisposable {
    private const int UNDEFINED_ID = -1;

    private int id_ = UNDEFINED_ID;

    public GlTexture(IImage image) {
      GL.GenTextures(1, out int id);
      this.id_ = id;

      var target = TextureTarget.Texture2D;
      GL.BindTexture(target, this.id_);
      {
        this.LoadImageIntoTexture_(image);
      }
      GL.BindTexture(target, UNDEFINED_ID);
    }

    public GlTexture(ITexture texture) {
      GL.GenTextures(1, out int id);
      this.id_ = id;

      var target = TextureTarget.Texture2D;
      GL.BindTexture(target, this.id_);
      {
        this.LoadImageIntoTexture_(texture.Image);

        // Gl.glGenerateMipmap(target);

        var finBorderColor = texture.BorderColor;
        var hasBorderColor = finBorderColor != null;
        GL.TexParameter(target,
                        TextureParameterName.TextureWrapS,
                        (int) GlTexture.ConvertFinWrapToGlWrap_(
                            texture.WrapModeU, hasBorderColor));
        GL.TexParameter(target,
                        TextureParameterName.TextureWrapT,
                        (int) GlTexture.ConvertFinWrapToGlWrap_(
                            texture.WrapModeV, hasBorderColor));

        if (hasBorderColor) {
          var glBorderColor = new[] {
              finBorderColor.Rf,
              finBorderColor.Gf,
              finBorderColor.Bf,
              finBorderColor.Af
          };

          GL.TexParameter(target, TextureParameterName.TextureBorderColor,
                          glBorderColor);
        }

        GL.TexParameter(target, TextureParameterName.TextureMinFilter,
                        (int) TextureMinFilter.Nearest);
        GL.TexParameter(target, TextureParameterName.TextureMagFilter,
                        (int) TextureMagFilter.Linear);
      }
      GL.BindTexture(target, UNDEFINED_ID);
    }

    private void LoadImageIntoTexture_(IImage image) {
      var imageWidth = image.Width;
      var imageHeight = image.Height;

      var rgba = new byte[4 * imageWidth * imageHeight];
      image.Access(getHandler => {
        for (var y = 0; y < imageHeight; y++) {
          for (var x = 0; x < imageWidth; x++) {
            getHandler(x, y, out var r, out var g, out var b, out var a);

            var outI = 4 * (y * imageWidth + x);
            rgba[outI] = r;
            rgba[outI + 1] = g;
            rgba[outI + 2] = b;
            rgba[outI + 3] = a;
          }
        }
      });

      // TODO: Use different formats
      GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    imageWidth, imageHeight,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    rgba);
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

    private static TextureWrapMode ConvertFinWrapToGlWrap_(
        WrapMode wrapMode,
        bool hasBorderColor) =>
        wrapMode switch {
            WrapMode.CLAMP => hasBorderColor
                                  ? TextureWrapMode.ClampToBorder
                                  : TextureWrapMode.ClampToEdge,
            WrapMode.REPEAT        => TextureWrapMode.Repeat,
            WrapMode.MIRROR_REPEAT => TextureWrapMode.MirroredRepeat,
            _ => throw new ArgumentOutOfRangeException(
                     nameof(wrapMode), wrapMode, null)
        };
  }
}