using System;

using fin.image;
using fin.image.io;


namespace cmb.schema.cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class CmbImageReader : IImageReader {
    private readonly IImageReader impl_;

    public CmbImageReader(int width, int height, GlTextureFormat format) {
      this.impl_ = format switch {
          GlTextureFormat.RGBA4444 => new Rgba4444ImageReader(width, height),
          GlTextureFormat.RGBA5551 => new Rgba5551ImageReader(width, height),
          GlTextureFormat.RGBA8    => new Rgba32ImageReader(width, height),
          GlTextureFormat.RGB565   => new Rgb565ImageReader(width, height),
          GlTextureFormat.RGB8     => new Rgb24ImageReader(width, height),
          GlTextureFormat.L4       => new L4ImageReader(width, height),
          GlTextureFormat.Gas      => new L8ImageReader(width, height),
          GlTextureFormat.Shadow   => new L8ImageReader(width, height),
          GlTextureFormat.L8       => new L8ImageReader(width, height),
          GlTextureFormat.A8       => new A8ImageReader(width, height),
          GlTextureFormat.ETC1     => new Etc1ImageReader(width, height, false),
          GlTextureFormat.ETC1a4   => new Etc1ImageReader(width, height, true),
          GlTextureFormat.LA8      => new La16ImageReader(width, height),
          _ => throw new ArgumentOutOfRangeException(
              nameof(format),
              format,
              null)
      };
    }

    public IImage Read(byte[] srcBytes) => this.impl_.Read(srcBytes);
  }
}