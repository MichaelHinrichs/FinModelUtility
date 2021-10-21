using System.IO;

using fin.io;
using fin.util.asserts;
using fin.util.strings;

namespace zar.format.cmb {
  public class CmbHeader : IDeserializable {
    // TODO: Better way to do this?
    public static CmbVersion Version { get; private set; }

    public uint fileSize { get; private set; }
    public CmbVersion version { get; private set; }
    public string name { get; private set; }
    public uint faceIndicesCount { get; private set; }
    public uint sklOffset { get; private set; }
    public uint qtrsOffset { get; private set; }
    public uint matsOffset { get; private set; }
    public uint texOffset { get; private set; }
    public uint sklmOffset { get; private set; }
    public uint lutsOffset { get; private set; }
    public uint vatrOffset { get; private set; }
    public uint faceIndicesOffset { get; private set; }
    public uint textureDataOffset { get; private set; }
    public uint unk0 { get; private set; }

    public void Read(EndianBinaryReader r) {
      r.AssertMagicText("cmb" + AsciiUtil.GetChar(20));

      this.fileSize = r.ReadUInt32();

      this.version = CmbHeader.Version = (CmbVersion) r.ReadUInt32();


      Asserts.Equal(0, r.ReadUInt32());
      this.name = r.ReadString(16);
      this.faceIndicesCount = r.ReadUInt32();
      this.sklOffset = r.ReadUInt32();

      if (CmbHeader.Version > CmbVersion.OCARINA_OF_TIME_3D) {
        this.qtrsOffset = r.ReadUInt32();
      }

      this.matsOffset = r.ReadUInt32();
      this.texOffset = r.ReadUInt32();
      this.sklmOffset = r.ReadUInt32();
      this.lutsOffset = r.ReadUInt32();
      this.vatrOffset = r.ReadUInt32();
      this.faceIndicesOffset = r.ReadUInt32();
      this.textureDataOffset = r.ReadUInt32();

      if (this.version != CmbVersion.OCARINA_OF_TIME_3D) {
        this.unk0 = r.ReadUInt32();
      }
    }
  }
}