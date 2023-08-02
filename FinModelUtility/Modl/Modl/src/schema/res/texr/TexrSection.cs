using fin.util.asserts;

using schema.binary;


namespace modl.schema.res.texr {
  public class TexrSection : IBinaryConvertible {
    private enum TexrMode {
      BW1,
      BW2
    }

    public string FileName { get; private set; }

    public List<BwTexrFile> Textures { get; } = new();

    public void Read(IEndianBinaryReader er) {
      SectionHeaderUtil.AssertNameAndReadSize(
          er, "TEXR", out var texrLength);
      var expectedTexrSectionEnd = er.Position + texrLength;

      this.FileName = er.ReadString(er.ReadInt32());

      SectionHeaderUtil.ReadNameAndSize(
          er, out var textureSectionName, out var btfLength);
      var mode = textureSectionName switch {
          "XBTF" => TexrMode.BW1,
          "GBTF" => TexrMode.BW2,
          _      => throw new NotSupportedException(),
      };

      var expectedBtfEnd = er.Position + btfLength;

      Asserts.Equal(expectedTexrSectionEnd, expectedBtfEnd);

      this.Textures.Clear();
      var textureCount = er.ReadUInt32();

      var sectionName = mode switch {
          TexrMode.BW1 => "TEXT",
          TexrMode.BW2 => "GTXD",
      };
      var textureNameLength = mode switch {
          TexrMode.BW1 => 0x10,
          TexrMode.BW2 => 0x20,
      };

      for (var i = 0; i < textureCount; ++i) {
        var baseOffset = er.Position;
        SectionHeaderUtil.AssertNameAndReadSize(
            er, sectionName, out var textureLength);
        var endOffset = er.Position + textureLength;

        var textureName = er.ReadString(textureNameLength);

        er.Position = baseOffset;
        var data = er.ReadBytes(endOffset - baseOffset);

        Textures.Add(new BwTexrFile(textureName, data));
      }

      Asserts.Equal(expectedTexrSectionEnd, er.Position);
    }

    public void Write(ISubEndianBinaryWriter ew) {
      throw new NotImplementedException();
    }
  }

  public record BwTexrFile(string Name, byte[] Data);
}