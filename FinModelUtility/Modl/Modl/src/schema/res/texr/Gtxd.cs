using fin.image;
using fin.image.formats;

using schema.binary;

using SixLabors.ImageSharp.PixelFormats;

namespace modl.schema.res.texr {
  public class Gtxd : BTexr, ITexr {
    public IImage Image { get; private set; }

    public unsafe void Read(IEndianBinaryReader er) {
      SectionHeaderUtil.AssertNameAndReadSize(er, "GTXD", out _);
      var textureName = er.ReadString(0x20);

      er.PushMemberEndianness(Endianness.BigEndian);
      var width = er.ReadUInt32();
      var height = er.ReadUInt32();

      var unknowns0 = er.ReadUInt32s(2);

      var rawTextureType = er.ReadString(8)
                             .Replace("\0", "")
                             .ToCharArray();
      Array.Reverse(rawTextureType);
      var textureType = new string(rawTextureType);
      var drawType = er.ReadString(8);

      var unknown = er.ReadChars(48);

      var image = textureType switch {
          "A8R8G8B8" => this.ReadA8R8G8B8_(er, width, height),
          "DXT1"     => this.ReadDxt1_(er, width, height),
          "P8"       => this.ReadP8_(er, width, height),
          "P4"       => this.ReadP4_(er, width, height),
          "IA8"      => this.ReadIA8_(er, width, height),
          "IA4"      => this.ReadIA4_(er, width, height),
          "I8"       => this.ReadI8_(er, width, height),
          "I4"       => this.ReadI4_(er, width, height),
          _          => throw new NotImplementedException(),
      };
      this.Image = image;
      er.PopEndianness();

      if (textureName.ToLower().EndsWith("_bump")) {
        var normalTextureName = textureName.Replace(
            "_bump",
            "_normal",
            StringComparison.CurrentCultureIgnoreCase);

        var normalImage =
            new Rgb24Image(PixelFormat.RGB888, image.Width, image.Height);
        using var normalImageLock = normalImage.Lock();
        var normalImageScan0 = normalImageLock.pixelScan0;

        image.Access(bumpGetHandler => {
          for (var y = 0; y < image.Height; ++y) {
            for (var x = 0; x < image.Width; ++x) {

              bumpGetHandler(
                  x,
                  y,
                  out var bumpIntensity,
                  out var _,
                  out var _,
                  out var bumpAlpha);

              normalImageScan0[y * image.Width + x] =
                  new Rgb24(bumpIntensity, bumpAlpha, 255);
            }
          }
        });

        this.Image = normalImage;
      }
    }
  }
}