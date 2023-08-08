using fin.image;
using fin.model;

using OpenTK.Graphics.OpenGL;

using System.Buffers;
using System.Runtime.CompilerServices;

using FinTextureMinFilter = fin.model.TextureMinFilter;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using TextureMagFilter = fin.model.TextureMagFilter;
using TextureMinFilter = OpenTK.Graphics.OpenGL.TextureMinFilter;


namespace fin.ui.graphics.gl {
  public class GlTexture : IDisposable {
    private static readonly Dictionary<ITexture, GlTexture> cache_ = new();


    private const int UNDEFINED_ID = -1;
    private int id_ = UNDEFINED_ID;
    private readonly ITexture texture_;

    public static GlTexture FromTexture(ITexture texture) {
      if (!cache_.TryGetValue(texture, out var glTexture)) {
        glTexture = new GlTexture(texture);
        cache_[texture] = glTexture;
      }

      return glTexture;
    }

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

    private GlTexture(ITexture texture) {
      this.texture_ = texture;

      GL.GenTextures(1, out int id);
      this.id_ = id;

      var target = TextureTarget.Texture2D;
      GL.BindTexture(target, this.id_);
      {
        this.LoadImageIntoTexture_(texture.Image);

        var finBorderColor = texture.BorderColor;
        var hasBorderColor = finBorderColor != null;
        GL.TexParameter(target,
                        TextureParameterName.TextureWrapS,
                        (int) GlTexture.ConvertFinWrapToGlWrap_(
                            texture.WrapModeU,
                            hasBorderColor));
        GL.TexParameter(target,
                        TextureParameterName.TextureWrapT,
                        (int) GlTexture.ConvertFinWrapToGlWrap_(
                            texture.WrapModeV,
                            hasBorderColor));

        if (hasBorderColor) {
          var glBorderColor = new[] {
              finBorderColor.Rf,
              finBorderColor.Gf,
              finBorderColor.Bf,
              finBorderColor.Af
          };

          GL.TexParameter(target,
                          TextureParameterName.TextureBorderColor,
                          glBorderColor);
        }

        if (texture.MinFilter is FinTextureMinFilter.NEAR_MIPMAP_NEAR
                                 or FinTextureMinFilter.NEAR_MIPMAP_LINEAR
                                 or FinTextureMinFilter.LINEAR_MIPMAP_NEAR
                                 or FinTextureMinFilter.LINEAR_MIPMAP_LINEAR) {
          GL.GenerateTextureMipmap(this.id_);
        }

        GL.TexParameter(
            target,
            TextureParameterName.TextureMinFilter,
            (int) (texture.MinFilter switch {
                FinTextureMinFilter.NEAR   => TextureMinFilter.Nearest,
                FinTextureMinFilter.LINEAR => TextureMinFilter.Linear,
                FinTextureMinFilter.NEAR_MIPMAP_NEAR => TextureMinFilter
                    .NearestMipmapNearest,
                FinTextureMinFilter.NEAR_MIPMAP_LINEAR => TextureMinFilter
                    .NearestMipmapLinear,
                FinTextureMinFilter.LINEAR_MIPMAP_NEAR => TextureMinFilter
                    .LinearMipmapNearest,
                FinTextureMinFilter.LINEAR_MIPMAP_LINEAR => TextureMinFilter
                    .LinearMipmapLinear,
            }));
        GL.TexParameter(
            target,
            TextureParameterName.TextureMagFilter,
            (int) (texture.MagFilter switch {
                TextureMagFilter.NEAR => OpenTK.Graphics.OpenGL.TextureMagFilter
                                               .Nearest,
                TextureMagFilter.LINEAR => OpenTK.Graphics.OpenGL
                                                 .TextureMagFilter.Linear,
            }));
      }
      GL.BindTexture(target, UNDEFINED_ID);
    }

    private static readonly ArrayPool<byte> pool_ = ArrayPool<byte>.Shared;

    private void LoadImageIntoTexture_(IImage image) {
      var imageWidth = image.Width;
      var imageHeight = image.Height;

      PixelInternalFormat pixelInternalFormat;
      PixelFormat pixelFormat;

      byte[] pixelBytes;
      switch (image) {
        case Rgba32Image rgba32Image: {
          pixelBytes = pool_.Rent(4 * imageWidth * imageHeight);
          pixelInternalFormat = PixelInternalFormat.Rgba;
          pixelFormat = PixelFormat.Rgba;
          rgba32Image.GetRgba32Bytes(pixelBytes);
          break;
        }
        case Rgb24Image rgb24Image: {
          pixelBytes = pool_.Rent(3 * imageWidth * imageHeight);
          pixelInternalFormat = PixelInternalFormat.Rgb;
          pixelFormat = PixelFormat.Rgb;
          rgb24Image.GetRgb24Bytes(pixelBytes);
          break;
        }
        case La16Image ia16Image: {
          pixelBytes = pool_.Rent(2 * imageWidth * imageHeight);
          pixelInternalFormat = PixelInternalFormat.LuminanceAlpha;
          pixelFormat = PixelFormat.LuminanceAlpha;
          ia16Image.GetIa16Bytes(pixelBytes);
          break;
        }
        case L8Image i8Image: {
          pixelBytes = pool_.Rent(imageWidth * imageHeight);
          pixelInternalFormat = PixelInternalFormat.Luminance;
          pixelFormat = PixelFormat.Luminance;
          i8Image.GetI8Bytes(pixelBytes);
          break;
        }
        default: {
          pixelBytes = pool_.Rent(4 * imageWidth * imageHeight);
          pixelInternalFormat = PixelInternalFormat.Rgba;
          pixelFormat = PixelFormat.Rgba;
          image.Access(getHandler => {
            for (var y = 0; y < imageHeight; y++) {
              for (var x = 0; x < imageWidth; x++) {
                getHandler(x, y, out var r, out var g, out var b, out var a);

                var outI = 4 * (y * imageWidth + x);
                pixelBytes[outI] = r;
                pixelBytes[outI + 1] = g;
                pixelBytes[outI + 2] = b;
                pixelBytes[outI + 3] = a;
              }
            }
          });
          break;
        }
      }

      GL.TexImage2D(TextureTarget.Texture2D,
                    0,
                    pixelInternalFormat,
                    imageWidth,
                    imageHeight,
                    0,
                    pixelFormat,
                    PixelType.UnsignedByte,
                    pixelBytes);

      pool_.Return(pixelBytes);
    }

    ~GlTexture() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      GlTexture.cache_.Remove(this.texture_);

      var id = this.id_;
      GL.DeleteTextures(1, ref id);

      this.id_ = UNDEFINED_ID;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Bind(int textureIndex = 0)
      => GlUtil.BindTexture(textureIndex, this.id_);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unbind(int textureIndex = 0)
      => GlUtil.UnbindTexture(textureIndex);

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
                nameof(wrapMode),
                wrapMode,
                null)
        };
  }
}