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

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
  };

  public enum LightingInfoFlags {
    USE_SPOTLIGHT = 1
  }

  public class LightingInfo : IGcnSerializable {
    u32 m_typeFlags = 0; // see LightingInfoFlags
    f32 m_unknown2 = 0;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
  };

  public class PeInfo : IGcnSerializable {
    s32 m_unknown1 = 0;
    s32 m_unknown2 = 0;
    s32 m_unknown3 = 0;
    s32 m_unknown4 = 0;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
  };

  public class TexGenData : IGcnSerializable {
    u8 m_unknown1 = 0;
    u8 m_unknown2 = 0;
    u8 m_unknown3 = 0;
    u8 m_unknown4 = 0;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
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

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream& os, TextureData const& t);
  };

  public class TextureInfo : IGcnSerializable {
    s32 m_unknown1 = 0;
    Vector3f m_unknown2;
    std::vector<TexGenData> m_unknown3;
    std::vector<TextureData> m_unknown4;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream& os, TextureInfo const& ti);
  };

  public enum MaterialFlags {
    UsePVW = 1
  };

  public class Material : IGcnSerializable {
    u32 m_flags = 0;
    u32 m_unknown1 = 0;
    ColourU8 m_colour;

    u32 m_unknown2 = 0;
    PolygonColourInfo m_colourInfo;
    LightingInfo m_lightingInfo;
    PeInfo m_peInfo;
    TextureInfo m_texInfo;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream&, Material const&);
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
    s32 m_unknown1 = 0;
    KeyInfoS10 m_unknown2;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream&, TCR_Unk2 const&);
  };

  public class TEVColReg : IGcnSerializable {
    ColourU16 m_unknown1;
    s32 m_unknown2 = 0;
    f32 m_unknown3 = 0;
    std::vector<TCR_Unk1> m_unknown4;
    std::vector<TCR_Unk2> m_unknown5;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream&, TEVColReg const&);
  };

  public class PVWCombiner : IGcnSerializable {
    u8 m_unknown1 = 0;
    u8 m_unknown2 = 0;
    u8 m_unknown3 = 0;
    u8 m_unknown4 = 0;
    u8 m_unknown5 = 0;
    u8 m_unknown6 = 0;
    u8 m_unknown7 = 0;
    u8 m_unknown8 = 0;
    u8 m_unknown9 = 0;
    u8 m_unknown10 = 0;
    u8 m_unknown11 = 0;
    u8 m_unknown12 = 0;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream&, PVWCombiner const&);
  };

  public class TEVStage : IGcnSerializable {
    u8 m_unknown1 = 0;
    u8 m_unknown2 = 0;
    u8 m_unknown3 = 0;
    u8 m_unknown4 = 0;
    u8 m_unknown5 = 0;
    u8 m_unknown6 = 0;
    PVWCombiner m_unknown7;
    PVWCombiner m_unknown8;

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream&, TEVStage const&);
  };

  public class TEVInfo : IGcnSerializable {
    // Probably RGB
    TEVColReg m_unknown1;
    TEVColReg m_unknown2;
    TEVColReg m_unknown3;

    ColourU8 m_unknown4;
    ColourU8 m_unknown5;
    ColourU8 m_unknown6;
    ColourU8 m_unknown7;

    public readonly List<TEVStage> m_unknown8 = new();

    void read(util::fstream_reader& reader);
    void write(util::fstream_writer& writer);
    friend std::ostream& operator <<(std::ostream&, TEVInfo const&);
  };

  public class MaterialContainer {
    public readonly List<Material> m_materials = new();
    public readonly List<TEVInfo> m_texEnvironments = new();
  }
}