using System;
using System.IO;

using fin.io;

namespace zar.format.cmb {
  public class Mesh : IDeserializable {
    public ushort shapeIndex;
    public byte materialIndex;
    public byte id;

    public void Read(EndianBinaryReader r) {
      this.shapeIndex = r.ReadUInt16();
      this.materialIndex = r.ReadByte();
      this.id = r.ReadByte();

      // Some of these values are possibly crc32
      switch (CmbHeader.Version) {
        case CmbVersion.MAJORAS_MASK_3D: {
          r.Position += 0x8;
          break;
        }
        case CmbVersion.EVER_OASIS: {
          r.Position += 0xC;
          break;
        }
        case CmbVersion.LUIGIS_MANSION_3D: {
          r.Position += 0x54;
          break;
        }
      }
    }
  }
}
