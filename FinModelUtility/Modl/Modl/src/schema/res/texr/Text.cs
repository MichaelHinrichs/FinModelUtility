using fin.image;

using schema.binary;

namespace modl.schema.res.texr {
  public class Text : BTexr, ITexr {
    public IImage Image { get; private set; }

    public void Read(IEndianBinaryReader er) {
      SectionHeaderUtil.AssertNameAndReadSize(er, "TEXT", out _);

      var textureName = er.ReadString(0x10);

      var width = er.ReadUInt32();
      var height = er.ReadUInt32();

      var unknowns0 = er.ReadUInt32s(2);

      var textureType = er.ReadString(8);
      var drawType = er.ReadString(8);

      var unknowns1 = er.ReadUInt32s(8);

      var unknowns2 = er.ReadUInt32s(1);

      er.PushMemberEndianness(Endianness.BigEndian);
      this.Image = textureType switch {
          "A8R8G8B8" => this.ReadA8R8G8B8_(er, width, height),
          "DXT1"     => this.ReadDxt1_(er, width, height),
          "P8"       => this.ReadP8_(er, width, height),
          _          => throw new NotImplementedException(),
      };
      er.PopEndianness();
    }
  }
}
