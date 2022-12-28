using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.io;
using fin.model;
using fin.image;

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
          this.ColorType = BmdGxTexture.GetColorType_(bti.Format);
          this.WrapModeS = bti.WrapS;
          this.WrapModeT = bti.WrapT;
        }
      }

      if (this.Image == null) {
        this.Image = header.ToBitmap();
        this.ColorType = BmdGxTexture.GetColorType_(header.Format);
        this.WrapModeS = header.WrapS;
        this.WrapModeT = header.WrapT;
      }
    }

    public string Name { get; }
    public TextureEntry Header { get; }
    public IImage Image { get; }
    public ColorType ColorType { get; }
    public GX_WRAP_TAG WrapModeS { get; }
    public GX_WRAP_TAG WrapModeT { get; }

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