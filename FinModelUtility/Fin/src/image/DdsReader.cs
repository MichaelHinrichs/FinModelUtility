using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using fin.util.image;


namespace fin.image {
  public class DdsReader {
    public unsafe IMipMap<Bitmap> Read(Stream stream) {
      using var pfimImage = Pfim.Pfim.FromStream(stream);

      // Convert from Pfim's backend agnostic image format into GDI+'s image format
      var format = pfimImage.Format switch {
          Pfim.ImageFormat.Rgba32 => PixelFormat.Format32bppArgb,
          Pfim.ImageFormat.Rgb24 => PixelFormat.Format24bppRgb,
          _ => throw new NotImplementedException(
                   $"Unsupported Pfim format: {pfimImage.Format}")
      };

      return MipMapUtil.From(
          pfimImage.MipMaps.Select(pfimMipMap => {
                     var mmWidth = pfimMipMap.Width;
                     var mmHeight = pfimMipMap.Height;

                     var bitmap = new Bitmap(mmWidth, mmHeight, format);
                     BitmapUtil.InvokeAsLocked(
                         bitmap,
                         pfimImage,
                         (bmpData, pfimImage) => {
                           var ptr = (byte*) bmpData.Scan0.ToPointer();
                           for (var i = 0;
                                i < pfimMipMap.Stride * mmHeight;
                                ++i) {
                             ptr[i] = pfimImage.Data[pfimMipMap.DataOffset + i];
                           }
                         });

                     return bitmap;
                   })
                   .ToList());
    }
  }
}