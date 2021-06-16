using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using fin.model;

using MKDS_Course_Modifier.GCN;

namespace mkds.exporter {
  public class BmdTexture {
    private readonly string name_;

    public BmdTexture(
        string name,
        BMD.TEX1Section.TextureHeader header,
        IList<(string, BTI)>? pathsAndBtis = null) {
      this.name_ = name;
      this.Header = header;

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

          this.Image = bti.ToBitmap();
          mirrorS = (bti.Header.WrapS & BTI.GX_WRAP_TAG.GX_MIRROR) != 0;
          mirrorT = (bti.Header.WrapT & BTI.GX_WRAP_TAG.GX_MIRROR) != 0;
          repeatS = (bti.Header.WrapS & BTI.GX_WRAP_TAG.GX_REPEAT) != 0;
          repeatT = (bti.Header.WrapT & BTI.GX_WRAP_TAG.GX_REPEAT) != 0;
        }
      }

      if (this.Image == null) {
        this.Image = header.ToBitmap();
        mirrorS =
            (header.WrapS & BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR) != 0;
        mirrorT =
            (header.WrapT & BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR) != 0;
        repeatS =
            (header.WrapS & BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT) != 0;
        repeatT =
            (header.WrapT & BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT) != 0;
      }

      // TODO: Need to handle wrapping in the shader?
      this.WrapModeS = BmdTexture.GetWrapMode_(mirrorS, repeatS);
      this.WrapModeT = BmdTexture.GetWrapMode_(mirrorT, repeatT);
    }

    public BMD.TEX1Section.TextureHeader Header { get; }
    public Bitmap Image { get; }
    public WrapMode WrapModeS { get; }
    public WrapMode WrapModeT { get; }

    public void SaveInDirectory(DirectoryInfo directory) {
      var stream = new MemoryStream();
      this.Image.Save(stream, ImageFormat.Png);

      var imageBytes = stream.ToArray();
      
      File.WriteAllBytes($"{directory.FullName}\\{this.name_}.png",
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
  }
}