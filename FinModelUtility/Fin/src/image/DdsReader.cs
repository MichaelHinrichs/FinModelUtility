using System;
using System.IO;
using System.Linq;

using ImageFormat = Pfim.ImageFormat;


namespace fin.image {
  public class DdsReader {
    public IMipMap<IImage> Read(Stream stream) {
      using var pfimImage = Pfim.Pfim.FromStream(stream);

      return MipMapUtil.From(
          pfimImage.MipMaps.Select(pfimMipMap => {
                     var mmWidth = pfimMipMap.Width;
                     var mmHeight = pfimMipMap.Height;

                     switch (pfimImage.Format) {
                       case ImageFormat.Rgba32: {
                         var image = new Rgba32Image(mmWidth, mmHeight);
                         image.Mutate((_, setHandler) => {
                           for (var y = 0; y < mmHeight; ++y) {
                             for (var x = 0; x < mmWidth; ++x) {
                               var i = pfimMipMap.DataOffset +
                                       4 * (y * mmWidth + x);

                               var b = pfimImage.Data[i + 0];
                               var g = pfimImage.Data[i + 1];
                               var r = pfimImage.Data[i + 2];
                               var a = pfimImage.Data[i + 3];

                               setHandler(x, y, r, g, b, a);
                             }
                           }
                         });
                         return image as IImage;
                       }
                       case ImageFormat.Rgb24: {
                         var image = new Rgb24Image(mmWidth, mmHeight);
                         image.Mutate((_, setHandler) => {
                           for (var y = 0; y < mmHeight; ++y) {
                             for (var x = 0; x < mmWidth; ++x) {
                               var i = pfimMipMap.DataOffset +
                                       3 * (y * mmWidth + x);

                               var b = pfimImage.Data[i + 0];
                               var g = pfimImage.Data[i + 1];
                               var r = pfimImage.Data[i + 2];

                               setHandler(x, y, r, g, b);
                             }
                           }
                         });
                         return image;
                       }
                       default:
                         throw new NotImplementedException(
                             $"Unsupported Pfim format: {pfimImage.Format}");
                     }
                   })
                   .ToList());
    }
  }
}