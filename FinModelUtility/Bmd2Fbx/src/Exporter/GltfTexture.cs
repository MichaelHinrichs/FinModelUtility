using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using fin.io;

using bmd.GCN;

using SharpGLTF.Memory;

using TextureWrapMode = SharpGLTF.Schema2.TextureWrapMode;

namespace bmd.exporter {
  public class GltfTexture {
    private readonly string name_;
    private readonly byte[] imageBytes_;

    public GltfTexture(
        string name,
        BMD.TEX1Section.TextureHeader header,
        IList<(string, BTI)>? pathsAndBtis = null) {
      this.name_ = name;
      this.Header = header;

      Bitmap? image = null;
      var mirrorS = false;
      var mirrorT = false;
      var repeatS = false;
      var repeatT = false;

      // TODO: This doesn't feel right, where can we get the actual name?
      if (name.Contains("_dummy_")) {
        var prefix = name.Substring(0, name.IndexOf("_dummy_"));

        var matchingPathAndBtis = pathsAndBtis
            .SkipWhile(pathAndBti
                           => !new FileInfo(pathAndBti.Item1)
                               .Name.StartsWith(prefix));

        if (matchingPathAndBtis.Count() > 0) {
          var matchingPathAndBti = matchingPathAndBtis.First();

          name = new FileInfo(matchingPathAndBti.Item1).Name;
          var bti = matchingPathAndBti.Item2;

          image = bti.ToBitmap();
          mirrorS = (bti.Header.WrapS & BTI.GX_WRAP_TAG.GX_MIRROR) != 0;
          mirrorT = (bti.Header.WrapT & BTI.GX_WRAP_TAG.GX_MIRROR) != 0;
          repeatS = (bti.Header.WrapS & BTI.GX_WRAP_TAG.GX_REPEAT) != 0;
          repeatT = (bti.Header.WrapT & BTI.GX_WRAP_TAG.GX_REPEAT) != 0;
        }
      }

      if (image == null) {
        image = header.ToBitmap();
        mirrorS =
            (header.WrapS & BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR) != 0;
        mirrorT =
            (header.WrapT & BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR) != 0;
        repeatS =
            (header.WrapS & BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT) != 0;
        repeatT =
            (header.WrapT & BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT) != 0;
      }

      var stream = new MemoryStream();
      image.Save(stream, ImageFormat.Png);

      this.imageBytes_ = stream.ToArray();

      this.MemoryImage = new MemoryImage(this.imageBytes_);

      // TODO: Need to handle wrapping in the shader?
      this.WrapModeS = GltfTexture.GetWrapMode_(mirrorS, repeatS);
      this.WrapModeT = GltfTexture.GetWrapMode_(mirrorT, repeatT);
    }

    public BMD.TEX1Section.TextureHeader Header { get; }
    public MemoryImage MemoryImage { get; }
    public TextureWrapMode WrapModeS { get; }
    public TextureWrapMode WrapModeT { get; }

    public void SaveInDirectory(IDirectory directory) {
      // Some names have invalid characters, so we need to process them out.
      // - e.g. 256??256.png, which are meant to be dimensions
      var name = this.name_.Replace("??", "x");

      File.WriteAllBytes($"{directory.FullName}\\{name}.png",
                         this.imageBytes_);
    }

    private static TextureWrapMode GetWrapMode_(bool mirror, bool repeat) {
      if (mirror) {
        return TextureWrapMode.MIRRORED_REPEAT;
      }

      if (repeat) {
        return TextureWrapMode.REPEAT;
      }

      return TextureWrapMode.CLAMP_TO_EDGE;
    }
  }
}