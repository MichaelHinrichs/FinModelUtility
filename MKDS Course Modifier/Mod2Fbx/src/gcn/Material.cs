using System.Collections.Generic;
using System.IO;

namespace mod.gcn {
  ////////////////////////////////////////////////////////////////////
  // NOTE: the names of the classes are taken directly from sysCore //
  // with the exception of unknowns (_Unk)                          //
  ////////////////////////////////////////////////////////////////////
  // Also, I am using signed types because I am unsure whether or   //
  // not negative values are needed                                 //
  ////////////////////////////////////////////////////////////////////
  // PCI = PolygonColourInfo                                        //
  // TXD = TextureData                                              //
  // TEV = TextureEnvironment                                       //
  // TCR = Texture Environment (TEV) Colour Register                //
  ////////////////////////////////////////////////////////////////////

  public class KeyInfoU8 : IGcnSerializable {
    public byte unknown1 = 0;
    public float unknown2 = 0;
    public float unknown3 = 0;

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadByte();
      reader.ReadByte();
      reader.ReadUInt16();

      this.unknown2 = reader.ReadSingle();
      this.unknown3 = reader.ReadSingle();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.unknown1);
      writer.Write((byte) 0);
      writer.Write((byte) 0);
      writer.Write((byte) 0);

      writer.Write(this.unknown2);
      writer.Write(this.unknown3);
    }

    public string? ToString()
      => $"{this.unknown1} {this.unknown2} {this.unknown3}";
  }

  public class KeyInfoF32 : IGcnSerializable {
    public float unknown1 = 0;
    public float unknown2 = 0;
    public float unknown3 = 0;

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadSingle();
      this.unknown2 = reader.ReadSingle();
      this.unknown3 = reader.ReadSingle();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.unknown1);
      writer.Write(this.unknown2);
      writer.Write(this.unknown3);
    }

    public string? ToString()
      => $"{this.unknown1} {this.unknown2} {this.unknown3}";
  }

  public class KeyInfoS10 : IGcnSerializable {
    public short unknown1 = 0;
    public float unknown2 = 0;
    public float unknown3 = 0;

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadInt16();
      reader.ReadInt16();
      this.unknown2 = reader.ReadSingle();
      this.unknown3 = reader.ReadSingle();
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.unknown1);
      writer.Write((short) 0);
      writer.Write(this.unknown2);
      writer.Write(this.unknown3);
    }

    public string? ToString()
      => $"{this.unknown1} {this.unknown2} {this.unknown3}";
  };

  public class PCI_Unk1 : IGcnSerializable {
    s32 m_unknown1 = 0;
    KeyInfoU8 m_unknown2;
    KeyInfoU8 m_unknown3;
    KeyInfoU8 m_unknown4;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  }

  public class PCI_Unk2 : IGcnSerializable {
    s32 m_unknown1 = 0;
    KeyInfoU8 m_unknown2;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
  }

  public class PolygonColourInfo : IGcnSerializable {
    ColourU8 m_diffuseColour;
    s32 m_unknown2 = 0;
    f32 m_unknown3 = 0;
    std::vector<PCI_Unk1> m_unknown4;
    std::vector<PCI_Unk2> m_unknown5;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public enum LightingInfoFlags {
    USE_SPOTLIGHT = 1
  }

  public class LightingInfo : IGcnSerializable {
    u32 m_typeFlags = 0; // see LightingInfoFlags
    f32 m_unknown2 = 0;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class PeInfo : IGcnSerializable {
    s32 m_unknown1 = 0;
    s32 m_unknown2 = 0;
    s32 m_unknown3 = 0;
    s32 m_unknown4 = 0;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class TexGenData : IGcnSerializable {
    u8 m_unknown1 = 0;
    u8 m_unknown2 = 0;
    u8 m_unknown3 = 0;
    u8 m_unknown4 = 0;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class TXD_Unk1 : IGcnSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoF32 unknown2 = new();
    public readonly KeyInfoF32 unknown3 = new();
    public readonly KeyInfoF32 unknown4 = new();

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadInt32();
      this.unknown2.Read(reader);
      this.unknown3.Read(reader);
      this.unknown4.Read(reader);
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.unknown1);
      this.unknown2.Write(writer);
      this.unknown3.Write(writer);
      this.unknown4.Write(writer);
    }
  }

  public class TextureData : IGcnSerializable {
    s32 m_unknown1 = 0;
    s16 m_unknown2 = 0;
    s16 m_unknown3 = 0;

    u8 m_unknown4 = 0;
    u8 m_unknown5 = 0;
    u8 m_unknown6 = 0;
    u8 m_unknown7 = 0;

    u32 m_unknown8 = 0;
    s32 m_unknown9 = 0;

    f32 m_unknown10 = 0;
    f32 m_unknown11 = 0;
    f32 m_unknown12 = 0;
    f32 m_unknown13 = 0;
    f32 m_unknown14 = 0;
    f32 m_unknown15 = 0;
    f32 m_unknown16 = 0;
    f32 m_unknown17 = 0;

    std::vector<TXD_Unk1> m_unknown18;
    std::vector<TXD_Unk1> m_unknown19;
    std::vector<TXD_Unk1> m_unknown20;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class TextureInfo : IGcnSerializable {
    s32 m_unknown1 = 0;
    Vector3f m_unknown2;
    std::vector<TexGenData> m_unknown3;
    std::vector<TextureData> m_unknown4;

    public void Read(EndianBinaryReader reader) {
      throw new System.NotImplementedException();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public enum MaterialFlags {
    UsePVW = 1
  };

  public class Material : IGcnSerializable {
    public uint flags = 0;
    public uint unknown1 = 0;
    public readonly ColourU8 colour = new();

    public uint unknown2 = 0;
    public readonly PolygonColourInfo colourInfo = new();
    public readonly LightingInfo lightingInfo = new();
    public readonly PeInfo peInfo = new();
    public readonly TextureInfo texInfo = new();

    public void Read(EndianBinaryReader reader) {
      this.flags = reader.ReadUInt32();
      this.unknown1 = reader.ReadUInt32();
      this.colour.Read(reader);

      if ((this.flags & (uint) MaterialFlags.UsePVW) != 0) {
        this.unknown2 = reader.ReadUInt32();
        this.colourInfo.Read(reader);
        this.lightingInfo.Read(reader);
        this.peInfo.Read(reader);
        this.texInfo.Read(reader);
      }
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class TCR_Unk1 : IGcnSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoS10 unknown2 = new();
    public readonly KeyInfoS10 unknown3 = new();
    public readonly KeyInfoS10 unknown4 = new();

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadInt32();
      this.unknown2.Read(reader);
      this.unknown3.Read(reader);
      this.unknown4.Read(reader);
    }

    public void Write(EndianBinaryWriter writer) {
      writer.Write(this.unknown1);
      this.unknown2.Write(writer);
      this.unknown3.Write(writer);
      this.unknown4.Write(writer);
    }

    public string? ToString()
      => $"\t\t\tUNK1: {this.unknown1}\n" +
         $"\t\t\tUNK2: {this.unknown2}\n" +
         $"\t\t\tUNK3: {this.unknown3}\n" +
         $"\t\t\tUNK4: {this.unknown4}\n";
  }

  public class TCR_Unk2 : IGcnSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoS10 unknown2 = new();

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadInt32();
      this.unknown2.Read(reader);
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class TEVColReg : IGcnSerializable {
    public readonly ColourU16 unknown1 = new();
    public int unknown2 = 0;
    public float unknown3 = 0;
    public readonly List<TCR_Unk1> unknown4 = new();
    public readonly List<TCR_Unk2> unknown5 = new();

    public void Read(EndianBinaryReader reader) {
      this.unknown1.Read(reader);
      this.unknown2 = reader.ReadInt32();
      this.unknown3 = reader.ReadSingle();

      this.unknown4.Clear();
      var numUnknown4 = reader.ReadUInt32();
      for (var i = 0; i < numUnknown4; ++i) {
        var unk = new TCR_Unk1();
        unk.Read(reader);
        this.unknown4.Add(unk);
      }

      this.unknown5.Clear();
      var numUnknown5 = reader.ReadUInt32();
      for (var i = 0; i < numUnknown5; ++i) {
        var unk = new TCR_Unk2();
        unk.Read(reader);
        this.unknown5.Add(unk);
      }
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class PVWCombiner : IGcnSerializable {
    public byte unknown1 = 0;
    public byte unknown2 = 0;
    public byte unknown3 = 0;
    public byte unknown4 = 0;
    public byte unknown5 = 0;
    public byte unknown6 = 0;
    public byte unknown7 = 0;
    public byte unknown8 = 0;
    public byte unknown9 = 0;
    public byte unknown10 = 0;
    public byte unknown11 = 0;
    public byte unknown12 = 0;

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadByte();
      this.unknown2 = reader.ReadByte();
      this.unknown3 = reader.ReadByte();
      this.unknown4 = reader.ReadByte();
      this.unknown5 = reader.ReadByte();
      this.unknown6 = reader.ReadByte();
      this.unknown7 = reader.ReadByte();
      this.unknown8 = reader.ReadByte();
      this.unknown9 = reader.ReadByte();
      this.unknown10 = reader.ReadByte();
      this.unknown11 = reader.ReadByte();
      this.unknown12 = reader.ReadByte();
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  };

  public class TEVStage : IGcnSerializable {
    public byte unknown1 = 0;
    public byte unknown2 = 0;
    public byte unknown3 = 0;
    public byte unknown4 = 0;
    public byte unknown5 = 0;
    public byte unknown6 = 0;
    public readonly PVWCombiner unknown7 = new();
    public readonly PVWCombiner unknown8 = new();

    public void Read(EndianBinaryReader reader) {
      this.unknown1 = reader.ReadByte();
      this.unknown2 = reader.ReadByte();
      this.unknown3 = reader.ReadByte();
      this.unknown4 = reader.ReadByte();
      this.unknown5 = reader.ReadByte();
      this.unknown6 = reader.ReadByte();
      reader.ReadUInt16();
      this.unknown7.Read(reader);
      this.unknown8.Read(reader);
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  }

  public class TEVInfo : IGcnSerializable {
    // Probably RGB
    public readonly TEVColReg unknown1 = new();
    public readonly TEVColReg unknown2 = new();
    public readonly TEVColReg unknown3 = new();

    public readonly ColourU8 unknown4 = new();
    public readonly ColourU8 unknown5 = new();
    public readonly ColourU8 unknown6 = new();
    public readonly ColourU8 unknown7 = new();

    public readonly List<TEVStage> unknown8 = new();

    public void Read(EndianBinaryReader reader) {
      this.unknown1.Read(reader);
      this.unknown2.Read(reader);
      this.unknown3.Read(reader);

      this.unknown4.Read(reader);
      this.unknown5.Read(reader);
      this.unknown6.Read(reader);
      this.unknown7.Read(reader);

      this.unknown8.Clear();
      var numUnknown8 = reader.ReadUInt32();
      for (var i = 0; i < numUnknown8; ++i) {
        var stage = new TEVStage();
        stage.Read(reader);
        this.unknown8.Add(stage);
      }
    }

    public void Write(EndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  }

  public class MaterialContainer {
    public readonly List<Material> materials = new();
    public readonly List<TEVInfo> texEnvironments = new();
  }
}