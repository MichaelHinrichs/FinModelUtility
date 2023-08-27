using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.image;
using fin.io;
using fin.model;

using gx;

using j3d.schema.bmd.tex1;
using j3d.schema.bti;

namespace j3d.exporter {
  public class BmdGxTexture : IGxTexture {
    public BmdGxTexture(
        string name,
        TextureEntry header,
        IList<(string, Bti)>? pathsAndBtis = null) {
      this.Name = name;
      this.DefaultHeader = header;

      // TODO: This doesn't feel right, where can we get the actual name?
      if (pathsAndBtis != null && name.Contains("_dummy")) {
        var prefix = name.Substring(0, name.IndexOf("_dummy")).ToLower();

        var matchingPathAndBtis = pathsAndBtis
            .SkipWhile(pathAndBti
                           => !new FileInfo(pathAndBti.Item1)
                               .Name.ToLower()
                               .StartsWith(prefix));

        if (matchingPathAndBtis.Count() > 0) {
          var matchingPathAndBti = matchingPathAndBtis.First();

          this.Name = new FileInfo(matchingPathAndBti.Item1).Name;
          var bti = matchingPathAndBti.Item2;

          this.OverrideHeader = bti;
        }
      }

      this.Image = this.Header.ToBitmap();
      this.ColorType = BmdGxTexture.GetColorType_(this.Header.Format);
    }

    public string Name { get; }

    public IImage Image { get; }
    public TextureEntry Header => OverrideHeader ?? DefaultHeader;
    private TextureEntry DefaultHeader { get; }
    private TextureEntry? OverrideHeader { get; }

    public GX_WRAP_TAG WrapModeS => this.Header.WrapS;
    public GX_WRAP_TAG WrapModeT => this.Header.WrapT;
    public GX_MIN_TEXTURE_FILTER MinTextureFilter => this.Header.MinFilter;
    public GX_MAG_TEXTURE_FILTER MagTextureFilter => this.Header.MagFilter;

    public ColorType ColorType { get; }

    private static ColorType GetColorType_(TextureFormat textureFormat) {
      switch (textureFormat) {
        case TextureFormat.I4:
        case TextureFormat.I8:
        case TextureFormat.A4_I4:
        case TextureFormat.A8_I8:
          return ColorType.INTENSITY;

        case TextureFormat.R5_G6_B5:
        case TextureFormat.A3_RGB5:
        case TextureFormat.ARGB8:
        case TextureFormat.INDEX4:
        case TextureFormat.INDEX8:
        case TextureFormat.INDEX14_X2:
        case TextureFormat.S3TC1:
          return ColorType.COLOR;

        default:
          throw new NotImplementedException();
      }
    }
  }
}