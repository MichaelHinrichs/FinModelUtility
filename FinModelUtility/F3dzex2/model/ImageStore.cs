using System.Drawing;

using f3dzex2.image;
using f3dzex2.io;

using fin.data.lazy;
using fin.image;
using fin.util.hash;

namespace f3dzex2.model {
  public class ImageStore {
    private readonly LazyDictionary<ImageParams, IImage>
        lazyImageDictionary_;

    public ImageStore(IN64Memory n64Memory) {
      this.lazyImageDictionary_ =
          new(imageParams => {
            if (imageParams.IsInvalid) {
              return FinImage.Create1x1FromColor(imageParams.Color);
            }

            using var er =
                n64Memory.OpenAtSegmentedAddress(imageParams.SegmentedAddress);
            var imageData =
                er.ReadBytes(imageParams.Width * imageParams.Height * 4);

            return new N64ImageParser().Parse(imageParams.ColorFormat,
                                              imageParams.BitsPerTexel,
                                              imageData,
                                              imageParams.Width,
                                              imageParams.Height,
                                              new ushort[] { },
                                              false);
          });
    }

    public IImage GetOrLoadImage(ImageParams prms)
      => this.lazyImageDictionary_[prms];
  }
}