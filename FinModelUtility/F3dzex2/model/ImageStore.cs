﻿using System.Drawing;

using f3dzex2.image;
using f3dzex2.io;

using fin.data.lazy;
using fin.image;
using fin.util.hash;

namespace f3dzex2.model {
  public struct ImageParams {
    public ImageParams() { }

    public N64ColorFormat ColorFormat { get; set; } = N64ColorFormat.RGBA;
    public BitsPerTexel BitsPerTexel { get; set; } = BitsPerTexel._16BPT;
    public Color Color { get; set; }

    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public uint SegmentedAddress { get; set; }

    public override int GetHashCode() => FluentHash.Start()
                                                   .With(this.ColorFormat)
                                                   .With(this.BitsPerTexel)
                                                   .With(this.Color.ToArgb())
                                                   .With(this.Width)
                                                   .With(this.Height)
                                                   .With(this.SegmentedAddress);

    public bool IsInvalid => this.Width == 0 || this.Height == 0 ||
                             this.SegmentedAddress == 0;

    public override bool Equals(object? other) {
      if (ReferenceEquals(this, other)) {
        return true;
      }

      if (other is ImageParams otherImageParams) {
        return this.ColorFormat == otherImageParams.ColorFormat &&
               this.BitsPerTexel == otherImageParams.BitsPerTexel &&
               this.Color.ToArgb() == otherImageParams.Color.ToArgb() &&
               this.Width == otherImageParams.Width &&
               this.Height == otherImageParams.Height &&
               this.SegmentedAddress == otherImageParams.SegmentedAddress;
      }

      return false;
    }
  }

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