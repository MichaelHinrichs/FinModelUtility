using System.IO;

using fin.image;

using SixLabors.ImageSharp.PixelFormats;


namespace cmb.schema.cmb.image {
  /// <summary>
  ///   Stolen from:
  ///   https://github.com/magcius/noclip.website/blob/master/src/oot3d/pica_texture.ts
  /// </summary>
  public class L4ImageReader : BTiledImageReader<L8> {
    public L4ImageReader(int width, int height) : base(width, height) { }

    protected override IImage<L8> CreateImage_(int width, int height)
      => new I8Image(width, height);

    protected override unsafe void Decode(IEndianBinaryReader er,
                                          L8* scan0,
                                          int offset) {
      var value = er.ReadByte();

      var upper = (byte) ((value >> 4) * 17);
      var lower = (byte) ((value & 0xF) * 17);

      scan0[2 * offset + 0] = new L8(upper);
      scan0[2 * offset + 1] = new L8(lower);
    }
  }
}