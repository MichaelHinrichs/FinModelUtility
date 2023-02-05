using System.Collections.Generic;
using System.IO;

using fin.schema.color;
using fin.schema.vector;
using gx;
using schema.binary;

namespace mod.schema {
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

  [BinarySchema]
  public partial class KeyInfoU8 : IBiSerializable {
    public byte unknown1 = 0;
    public byte unknownA = 0;
    public ushort unknownB = 0;

    public float unknown2 = 0;
    public float unknown3 = 0;

    public string? ToString()
      => $"{this.unknown1} {this.unknown2} {this.unknown3}";
  }

  [BinarySchema]
  public partial class KeyInfoF32 : IBiSerializable {
    public float unknown1 = 0;
    public float unknown2 = 0;
    public float unknown3 = 0;

    public string? ToString()
      => $"{this.unknown1} {this.unknown2} {this.unknown3}";
  }

  [BinarySchema]
  public partial class KeyInfoS10 : IBiSerializable {
    public short unknown1 = 0;
    public readonly short padding = 0; // TODO: Is this right?
    public float unknown2 = 0;
    public float unknown3 = 0;

    public string? ToString()
      => $"{this.unknown1} {this.unknown2} {this.unknown3}";
  };

  [BinarySchema]
  public partial class PCI_Unk1 : IBiSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoU8 unknown2 = new();
    public readonly KeyInfoU8 unknown3 = new();
    public readonly KeyInfoU8 unknown4 = new();
  }

  [BinarySchema]
  public partial class PCI_Unk2 : IBiSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoU8 unknown2 = new();
  }

  [BinarySchema]
  public partial class PolygonColourInfo : IBiSerializable {
    public readonly Rgba32 diffuseColour = new();
    public int unknown2 = 0;
    public float unknown3 = 0;

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public PCI_Unk1[] unknown4;
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public PCI_Unk2[] unknown5;
  }

  public enum LightingInfoFlags {
    USE_SPOTLIGHT = 1
  }

  [BinarySchema]
  public partial class LightingInfo : IBiSerializable {
    public uint typeFlags = 0; // see LightingInfoFlags
    public float unknown2 = 0;
  }

  [BinarySchema]
  public partial class PeInfo : IBiSerializable {
    public int unknown1 = 0;
    public int unknown2 = 0;
    public int unknown3 = 0;
    public int unknown4 = 0;
  };

  [BinarySchema]
  public partial class TexGenData : IBiSerializable {
    public byte unknown1 = 0;
    public byte unknown2 = 0;
    public GxTexGenSrc TexGenSrc { get; set; }
    public byte unknown4 = 0;
  }

  [BinarySchema]
  public partial class TXD_Unk1 : IBiSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoF32 unknown2 = new();
    public readonly KeyInfoF32 unknown3 = new();
    public readonly KeyInfoF32 unknown4 = new();
  }

  [BinarySchema]
  public partial class TextureData : IBiSerializable {
    public int TexAttrIndex = 0;

    public short unknown2 = 0;
    public short unknown3 = 0;

    public byte unknown4 = 0;
    public byte unknown5 = 0;
    public byte unknown6 = 0;
    public byte unknown7 = 0;

    public uint unknown8 = 0;
    public int unknown9 = 0;

    public float unknown10 = 0;
    public float unknown11 = 0;
    public float unknown12 = 0;
    public float unknown13 = 0;
    public float unknown14 = 0;
    public float unknown15 = 0;
    public float unknown16 = 0;
    public float unknown17 = 0;

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TXD_Unk1[] unknown18;
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TXD_Unk1[] unknown19;
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TXD_Unk1[] unknown20;
  };

  [BinarySchema]
  public partial class TextureInfo : IBiSerializable {
    public int unknown1 = 0;
    public readonly Vector3f unknown2 = new();

    // TODO: These appear to be referenced before they're read? Try removing the {}
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TexGenData[] unknown3 = { };
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TextureData[] TexturesInMaterial = { };
  }

  public enum MaterialFlags {
    UsePVW = 1
  }

  public class Material : IBiSerializable {
    public uint flags = 0;
    public uint unknown1 = 0;
    public readonly Rgba32 colour = new();

    public uint TexEnvironmentIndex = 0;
    public readonly PolygonColourInfo colourInfo = new();
    public readonly LightingInfo lightingInfo = new();
    public readonly PeInfo peInfo = new();
    public readonly TextureInfo texInfo = new();

    public void Read(IEndianBinaryReader reader) {
      this.flags = reader.ReadUInt32();
      this.unknown1 = reader.ReadUInt32();
      this.colour.Read(reader);

      if ((this.flags & (uint)MaterialFlags.UsePVW) != 0) {
        this.TexEnvironmentIndex = reader.ReadUInt32();
        this.colourInfo.Read(reader);
        this.lightingInfo.Read(reader);
        this.peInfo.Read(reader);
        this.texInfo.Read(reader);
      }
    }

    public void Write(ISubEndianBinaryWriter writer) {
      throw new System.NotImplementedException();
    }
  }

  [BinarySchema]
  public partial class TCR_Unk1 : IBiSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoS10 unknown2 = new();
    public readonly KeyInfoS10 unknown3 = new();
    public readonly KeyInfoS10 unknown4 = new();

    public string? ToString()
      => $"\t\t\tUNK1: {this.unknown1}\n" +
         $"\t\t\tUNK2: {this.unknown2}\n" +
         $"\t\t\tUNK3: {this.unknown3}\n" +
         $"\t\t\tUNK4: {this.unknown4}\n";
  }

  [BinarySchema]
  public partial class TCR_Unk2 : IBiSerializable {
    public int unknown1 = 0;
    public readonly KeyInfoS10 unknown2 = new();
  }

  [BinarySchema]
  public partial class TEVColReg : IBiSerializable {
    public readonly Rgba64 unknown1 = new();
    public int unknown2 = 0;
    public float unknown3 = 0;
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TCR_Unk1[] unknown4;
    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TCR_Unk2[] unknown5;
  }

  [BinarySchema]
  public partial class ColorCombiner : IBiSerializable {
    public GxCc colorA = 0;
    public GxCc colorB = 0;
    public GxCc colorC = 0;
    public GxCc colorD = 0;

    // TODO: This is a guess
    public TevOp colorOp = 0;
    public byte unknown6 = 0;
    public byte unknown7 = 0;
    public byte unknown8 = 0;
    // TODO: This is a guess
    public ColorRegister colorRegister = 0;

    public byte unknown10 = 0;
    public byte unknown11 = 0;
    public byte unknown12 = 0;
  };

  [BinarySchema]
  public partial class AlphaCombiner : IBiSerializable {
    public GxCa alphaA = 0;
    public GxCa alphaB = 0;
    public GxCa alphaC = 0;
    public GxCa alphaD = 0;

    // TODO: This is a guess
    public TevOp alphaOp = 0;
    public byte unknown6 = 0;
    public byte unknown7 = 0;
    public byte unknown8 = 0;
    // TODO: This is a guess
    public ColorRegister alphaRegister = 0;

    public byte unknown10 = 0;
    public byte unknown11 = 0;
    public byte unknown12 = 0;
  };

  [BinarySchema]
  public partial class TEVStage : IBiSerializable {
    // TODO: This is a guess
    public byte TexCoordId { get; set; }
    // TODO: This is a guess
    public sbyte TexMap { get; set; }
    public byte unknown3 = 0;
    public GxColorChannel ColorChannel { get; set; }
    public byte unknown5 = 0;
    public byte unknown6 = 0;

    public ushort unknown65 = 0;

    public ColorCombiner ColorCombiner { get; } = new();
    public AlphaCombiner AlphaCombiner { get; } = new();
  }

  [BinarySchema]
  public partial class TEVInfo : IBiSerializable {
    // These are probably default values for the 3 color registers.
    // TODO: This is a guess
    [ArrayLengthSource(3)]
    public TEVColReg[] ColorRegisters { get; set; }

    // These are probably konst colors.
    // TODO: This is a guess
    [ArrayLengthSource(4)]
    public Rgba32[] KonstColors { get; set; }

    [ArrayLengthSource(SchemaIntegerType.UINT32)]
    public TEVStage[] TevStages;
  }

  public class MaterialContainer {
    public readonly List<Material> materials = new();
    public readonly List<TEVInfo> texEnvironments = new();
  }
}