using System;
using System.IO;
using System.Numerics;

using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

namespace fin.model.io.exporters.gltf {
  public static class GltfTextureUtil {
    public static TextureBuilder UseTexture(this ChannelBuilder channelBuilder,
                                            ITexture finTexture) {
      var textureBuilder = channelBuilder.UseTexture();
      textureBuilder
          .WithPrimaryImage(
              GltfTextureUtil.GetGltfImageFromFinTexture_(finTexture))
          .WithCoordinateSet(0)
          .WithSampler(
              GltfTextureUtil.ConvertWrapMode_(finTexture.WrapModeU),
              GltfTextureUtil.ConvertWrapMode_(finTexture.WrapModeV),
              GltfTextureUtil.ConvertMinFilter_(finTexture.MinFilter),
              GltfTextureUtil.ConvertMagFilter_(finTexture.MagFilter));

      textureBuilder.WithTransform(
          new Vector2(finTexture.Offset.X, finTexture.Offset.Y),
          new Vector2(finTexture.Scale.X, finTexture.Scale.Y),
          finTexture.RotationDegrees);

      return textureBuilder;
    }

    private static MemoryImage GetGltfImageFromFinTexture_(
        ITexture finTexture) {
      using var imageStream = new MemoryStream();
      finTexture.Image.ExportToStream(imageStream, finTexture.BestImageFormat);
      var imageBytes = imageStream.ToArray();
      return new MemoryImage(imageBytes);
    }

    private static TextureWrapMode ConvertWrapMode_(WrapMode wrapMode)
      => wrapMode switch {
          WrapMode.CLAMP         => TextureWrapMode.CLAMP_TO_EDGE,
          WrapMode.REPEAT        => TextureWrapMode.REPEAT,
          WrapMode.MIRROR_REPEAT => TextureWrapMode.MIRRORED_REPEAT,
          _ => throw new ArgumentOutOfRangeException(
              nameof(wrapMode),
              wrapMode,
              null)
      };

    private static TextureMipMapFilter ConvertMinFilter_(
        TextureMinFilter minFilter)
      => minFilter switch {
          TextureMinFilter.NEAR   => TextureMipMapFilter.NEAREST,
          TextureMinFilter.LINEAR => TextureMipMapFilter.LINEAR,
          TextureMinFilter.NEAR_MIPMAP_NEAR => TextureMipMapFilter
              .NEAREST_MIPMAP_NEAREST,
          TextureMinFilter.NEAR_MIPMAP_LINEAR => TextureMipMapFilter
              .NEAREST_MIPMAP_LINEAR,
          TextureMinFilter.LINEAR_MIPMAP_NEAR => TextureMipMapFilter
              .LINEAR_MIPMAP_NEAREST,
          TextureMinFilter.LINEAR_MIPMAP_LINEAR => TextureMipMapFilter
              .LINEAR_MIPMAP_LINEAR,
      };

    private static TextureInterpolationFilter ConvertMagFilter_(
        TextureMagFilter magFilter)
      => magFilter switch {
          TextureMagFilter.NEAR   => TextureInterpolationFilter.NEAREST,
          TextureMagFilter.LINEAR => TextureInterpolationFilter.LINEAR,
      };
  }
}