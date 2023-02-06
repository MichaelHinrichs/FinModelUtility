using fin.image;
using fin.util.asserts;
using mod.schema.image;
using schema.binary;
using schema.binary.attributes.ignore;


namespace mod.schema {
  [BinarySchema]
  public partial class Texture : IBinaryConvertible {
    public enum TextureFormat : uint {
      RGB565 = 0,
      CMPR = 1,
      RGB5A3 = 2,
      I4 = 3,
      I8 = 4,
      IA4 = 5,
      IA8 = 6,
      RGBA32 = 7,
    }

    [Ignore]
    public int index;

    [Ignore]
    public string Name => "texture" + this.index + "_" + this.format;

    public ushort width = 0;
    public ushort height = 0;
    public TextureFormat format = 0;

    public readonly uint[] unknowns = new uint[5];

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public byte[] imageData { get; set; }

    public IImage ToImage() {
      BImageFormat? imageFormat = null;
      if (this.format == TextureFormat.RGB5A3) {
        imageFormat = new Rgb5A3(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.RGB565) {
        imageFormat = new Rgb565(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.CMPR) {
        imageFormat = new Cmpr(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.I4) {
        imageFormat = new I4(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.I8) {
        imageFormat = new I8(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.IA4) {
        imageFormat = new IA4(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.IA8) {
        imageFormat = new IA8(this.imageData, this.width, this.height);
      } else if (this.format == TextureFormat.RGBA32) {
        imageFormat = new Rgba32(this.imageData, this.width, this.height);
      } else {
        Asserts.Fail($"Unsupported type: {this.format}");
      }

      return imageFormat.Image;
    }
  }

  public enum TilingMode : byte {
    REPEAT = 0,
    CLAMP = 1,
    MIRROR_REPEAT = 2,
  }

  [BinarySchema]
  public partial class TextureAttributes : IBinaryConvertible {
    public ushort index = 0;
    private readonly ushort padding_ = 0;
    public TilingMode TilingModeS { get; set; }
    public TilingMode TilingModeT { get; set; }
    public ushort unknown1 = 0;
    public float unknown2 = 0;
  }
}