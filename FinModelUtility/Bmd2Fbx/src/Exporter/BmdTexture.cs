using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.io;
using fin.model;

using bmd.schema.bmd.tex1;
using bmd.schema.bti;
using fin.image;

using gx;


namespace bmd.exporter {
  public class BmdTexture {
    public BmdTexture(
        string name,
        TextureEntry header,
        IList<(string, Bti)>? pathsAndBtis = null) {
      this.Name = name;
      this.Header = header;

      var mirrorS = false;
      var mirrorT = false;
      var repeatS = false;
      var repeatT = false;

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

          name = new FileInfo(matchingPathAndBti.Item1).Name;
          var bti = matchingPathAndBti.Item2;

          this.Image = bti.ToBitmap();
          this.ColorType = BmdTexture.GetColorType_(bti.Format);
          mirrorS = (bti.WrapS & GX_WRAP_TAG.GX_MIRROR) != 0;
          mirrorT = (bti.WrapT & GX_WRAP_TAG.GX_MIRROR) != 0;
          repeatS = (bti.WrapS & GX_WRAP_TAG.GX_REPEAT) != 0;
          repeatT = (bti.WrapT & GX_WRAP_TAG.GX_REPEAT) != 0;
        }
      }

      if (this.Image == null) {
        this.Image = header.ToBitmap();
        this.ColorType = BmdTexture.GetColorType_(header.Format);
        mirrorS =
            (header.WrapS & GX_WRAP_TAG.GX_MIRROR) != 0;
        mirrorT =
            (header.WrapT & GX_WRAP_TAG.GX_MIRROR) != 0;
        repeatS =
            (header.WrapS & GX_WRAP_TAG.GX_REPEAT) != 0;
        repeatT =
            (header.WrapT & GX_WRAP_TAG.GX_REPEAT) != 0;
      }

      // TODO: Need to handle wrapping in the shader?
      this.WrapModeS = BmdTexture.GetWrapMode_(mirrorS, repeatS);
      this.WrapModeT = BmdTexture.GetWrapMode_(mirrorT, repeatT);
    }

    public string Name { get; }
    public TextureEntry Header { get; }
    public IImage Image { get; }
    public ColorType ColorType { get; }
    public WrapMode WrapModeS { get; }
    public WrapMode WrapModeT { get; }

    public void SaveInDirectory(IDirectory directory) {
      var stream = new MemoryStream();
      this.Image.ExportToStream(stream, LocalImageFormat.PNG);

      var imageBytes = stream.ToArray();

      // Some names have invalid characters, so we need to process them out.
      // - e.g. 256??256.png, which are meant to be dimensions
      var name = this.Name.Replace("??", "x");

      File.WriteAllBytes(Path.Join(directory.FullName, $"{name}.png"),
                         imageBytes);
    }

    private static WrapMode GetWrapMode_(bool mirror, bool repeat) {
      if (mirror) {
        return WrapMode.MIRROR_REPEAT;
      }

      if (repeat) {
        return WrapMode.REPEAT;
      }

      return WrapMode.CLAMP;
    }

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