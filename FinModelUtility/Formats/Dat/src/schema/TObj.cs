using System.Drawing;

using dat.image;

using fin.image;

using gx;

using schema.binary;

namespace dat.schema {
  /// <summary>
  ///   Texture object.
  ///
  ///   Shamelessly copied from:
  ///   https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/DAT.cs#L1281
  ///   https://github.com/jam1garner/Smash-Forge/blob/c0075bca364366bbea2d3803f5aeae45a4168640/Smash%20Forge/Filetypes/Melee/LibWii/TLP.cs#L166
  /// </summary>
  public class TObj : IBinaryDeserializable {
    public IImage Image { get; private set; }

    public void Read(IBinaryReader br) {
      br.Position += 4 * 13;

      var wrapS = br.ReadUInt32();
      var wrapT = br.ReadUInt32();
      var scaleW = br.ReadByte();
      var scaleH = br.ReadByte();

      br.Position += 2 + 12;

      var imageOffset = br.ReadUInt32();
      var paletteOffset = br.ReadUInt32();

      br.Position = imageOffset;
      var imageDataOffset = br.ReadUInt32();
      var width = br.ReadUInt16();
      var height = br.ReadUInt16();
      var format = (GxTextureFormat) br.ReadUInt32();

      // TODO: Add support for indexed textures
      try {
        br.Position = imageDataOffset;
        this.Image = new DatImageReader(width, height, format).ReadImage(br);
      } catch {
        this.Image = FinImage.Create1x1FromColor(Color.Magenta);
      }
    }
  }
}