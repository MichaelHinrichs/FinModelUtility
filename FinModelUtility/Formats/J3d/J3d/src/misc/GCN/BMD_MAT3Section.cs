using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using gx;

using j3d.G3D_Binary_File_Format;
using j3d.schema.bmd;
using j3d.schema.bmd.mat3;

using schema.binary;

#pragma warning disable CS8604

namespace j3d.GCN {
  public partial class BMD {
    public partial class MAT3Section {
      public const string Signature = "MAT3";
      public DataBlockHeader Header;
      public ushort NrMaterials;
      public uint[] Offsets;
      public MaterialEntry[] MaterialEntries;
      public BmdPopulatedMaterial[] PopulatedMaterials;
      public ushort[] MaterialEntryIndieces;
      public short[] TextureIndices;
      public GxCullMode[] CullModes;
      public Color[] MaterialColor;
      public Color[] LightColors;
      public Color[] AmbientColors;
      public Color[] TevColors;
      public Color[] TevKonstColors;
      public AlphaCompare[] AlphaCompares;
      public BlendFunction[] BlendFunctions;
      public DepthFunction[] DepthFunctions;
      public TevStageProps[] TevStages;
      public TevSwapMode[] TevSwapModes;
      public TevSwapModeTable[] TevSwapModeTables;
      public TexCoordGen[] TexCoordGens;
      public ColorChannelControl[] ColorChannelControls;
      public TextureMatrixInfo[] TextureMatrices;
      public TevOrder[] TevOrders;
      public StringTable MaterialNameTable;

      public readonly List<MatIndirectTexturingEntry>
          MatIndirectTexturingEntries = new();

      public MAT3Section(IEndianBinaryReader er, out bool OK) {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "MAT3", out OK1);
        if (!OK1) {
          OK = false;
        } else {
          this.NrMaterials = er.ReadUInt16();

          er.AssertUInt16(0xffff); // padding

          this.Offsets = er.ReadUInt32s(30);
          int[] sectionLengths = this.GetSectionLengths();
          long position2 = er.Position;

          // TODO: There is a bunch more data that isn't even read yet:
          // https://github.com/RenolY2/SuperBMD/blob/ccc86e21493275bcd9d86f65b516b85d95c83abd/SuperBMDLib/source/Materials/Enums/Mat3OffsetIndex.cs

          er.Position = position1 + (long) this.Offsets[0];
          this.MaterialEntries = new MaterialEntry[sectionLengths[0] / 332];
          for (int index = 0; index < sectionLengths[0] / 332; ++index)
            this.MaterialEntries[index] = er.ReadNew<MaterialEntry>();

          er.Position = position1 + (long) this.Offsets[1];
          this.MaterialEntryIndieces = er.ReadUInt16s((int) this.NrMaterials);

          er.Position = position1 + (long) this.Offsets[2];
          this.MaterialNameTable = er.ReadNew<StringTable>();

          var indirectTexturesOffset =
              er.Position = position1 + this.Offsets[3];
          this.MatIndirectTexturingEntries.Clear();
          while ((er.Position - indirectTexturesOffset) < sectionLengths[3]) {
            this.MatIndirectTexturingEntries.Add(
                er.ReadNew<MatIndirectTexturingEntry>());
          }

          er.Position = position1 + (long) this.Offsets[4];
          this.CullModes = new GxCullMode[sectionLengths[4] / 4];
          for (var index = 0; index < sectionLengths[4] / 4; ++index)
            this.CullModes[index] = (GxCullMode) er.ReadInt32();

          er.Position = position1 + (long) this.Offsets[5];
          this.MaterialColor = new Color[sectionLengths[5] / 4];
          for (int index = 0; index < sectionLengths[5] / 4; ++index)
            this.MaterialColor[index] = er.ReadColor8();

          er.Position = position1 + (long) this.Offsets[7];
          this.ColorChannelControls =
              new ColorChannelControl[sectionLengths[7] / 8];
          for (var i = 0; i < this.ColorChannelControls.Length; ++i) {
            this.ColorChannelControls[i] = er.ReadNew<ColorChannelControl>();
          }

          er.Position = position1 + (long) this.Offsets[8];
          this.AmbientColors = new Color[sectionLengths[8] / 4];
          for (int index = 0; index < sectionLengths[8] / 4; ++index)
            this.AmbientColors[index] = er.ReadColor8();

          er.Position = position1 + this.Offsets[9];
          this.LightColors = new Color[sectionLengths[9] / 8];
          for (int index = 0; index < this.LightColors.Length; ++index) {
            this.LightColors[index] = er.ReadColor16();
          }

          // TODO: Add support for texgen counts (10)

          er.Position = position1 + this.Offsets[11];
          er.ReadNewArray(out this.TexCoordGens, sectionLengths[11] / 4);

          // TODO: Add support for post tex coord gens (12)

          er.Position = position1 + (long) this.Offsets[13];
          er.ReadNewArray(out this.TextureMatrices, sectionLengths[13] / 100);

          // TODO: Add support for post tex matrices (14)

          er.Position = position1 + (long) this.Offsets[15];
          this.TextureIndices = er.ReadInt16s(sectionLengths[15] / 2);

          er.Position = position1 + (long) this.Offsets[16];
          er.ReadNewArray(out this.TevOrders, sectionLengths[16] / 4);

          er.Position = position1 + (long) this.Offsets[17];
          this.TevColors = new Color[sectionLengths[17] / 8];
          for (int index = 0; index < this.TevColors.Length; ++index)
            this.TevColors[index] = er.ReadColor16();

          er.Position = position1 + (long) this.Offsets[18];
          this.TevKonstColors = new Color[sectionLengths[18] / 4];
          for (int index = 0; index < this.TevKonstColors.Length; ++index)
            this.TevKonstColors[index] = er.ReadColor8();

          // TODO: Add support for tev counts (19)

          er.Position = position1 + (long) this.Offsets[20];
          er.ReadNewArray(out this.TevStages, sectionLengths[20] / 20);

          er.Position = position1 + (long) this.Offsets[21];
          er.ReadNewArray(out this.TevSwapModes, sectionLengths[21] / 4);

          er.Position = position1 + (long) this.Offsets[22];
          er.ReadNewArray(out this.TevSwapModeTables, sectionLengths[22] / 4);

          // TODO: Add support for fog modes (23)

          er.Position = position1 + (long) this.Offsets[24];
          er.ReadNewArray(out this.AlphaCompares, sectionLengths[24] / 8);

          er.Position = position1 + (long) this.Offsets[25];
          er.ReadNewArray(out this.BlendFunctions, sectionLengths[25] / 4);

          er.Position = position1 + (long) this.Offsets[26];
          er.ReadNewArray(out this.DepthFunctions, sectionLengths[26] / 4);

          er.Position = position1 + (long) this.Header.size;
          OK = true;

          // TODO: Add support for nbt scale (29)

          this.PopulatedMaterials = this.MaterialEntries
                                        .Select(
                                            (entry, index)
                                                => new BmdPopulatedMaterial(
                                                    this,
                                                    index,
                                                    entry))
                                        .ToArray();
        }
      }

      public int[] GetSectionLengths() {
        int[] numArray = new int[30];
        for (int index1 = 0; index1 < 30; ++index1) {
          int num1 = 0;
          if (this.Offsets[index1] != 0U) {
            int num2 = (int) this.Header.size;
            for (int index2 = index1 + 1; index2 < 30; ++index2) {
              if (this.Offsets[index2] != 0U) {
                num2 = (int) this.Offsets[index2];
                break;
              }
            }

            num1 = num2 - (int) this.Offsets[index1];
          }

          numArray[index1] = num1;
        }

        return numArray;
      }
    }
  }
}