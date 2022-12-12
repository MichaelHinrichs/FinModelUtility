using bmd.schema.bmd.mat3;
using gx;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static bmd.GCN.BMD;

namespace bmd.misc.GCN {

  public class PopulatedMaterial : IPopulatedMaterial {
    public string Name { get; set; }
    public byte Flag;
    public GxCullMode CullMode { get; set; }
    public byte ColorChannelControlsCountIndex;
    public byte TexGensCountIndex;
    public byte TevStagesCountIndex;
    public byte ZCompLocIndex;
    public byte ZModeIndex;
    public byte DitherIndex;

    public Color[] MaterialColors { get; set; }
    public IColorChannelControl?[] ColorChannelControls { get; set; }
    public Color[] AmbientColors { get; set; }
    public ushort[] LightColorIndexes;

    public Color[] KonstColors { get; set; }
    public Color[] TevColors { get; set; }


    public ushort[] TexGenInfo;

    public ushort[] TexGenInfo2;
    public ushort[] TexMatrices;
    public ushort[] DttMatrices;
    public short[] TextureIndices { get; set; }
    public ushort[] TevKonstColorIndexes;
    public byte[] ConstColorSel;
    public byte[] ConstAlphaSel;

    public ITevOrder?[] TevOrderInfos { get; set; }

    public ushort[] TevOrderInfoIndexes;
    public ushort[] TevColorIndexes;
    public ITevStageProps?[] TevStageInfos { get; set; }
    public ITevSwapMode?[] TevSwapModes { get; set; }
    public ITevSwapModeTable?[] TevSwapModeTables { get; set; }
    public ushort[] Unknown2;
    public short FogInfoIndex;
    public IAlphaCompare AlphaCompare { get; set; }
    public IBlendFunction BlendMode { get; set; }
    public short UnknownIndex;

    public ITexCoordGen?[] TexCoordGens { get; set; }

    public PopulatedMaterial(MAT3Section mat3, int index, MaterialEntry entry) {
      this.Name = mat3.MaterialNameTable[index];
      this.Flag = entry.Flag;

      this.CullMode = mat3.CullModes[entry.CullModeIndex];

      this.MaterialColors =
          entry.MaterialColorIndexes
               .Select(i => GetOrNull_(mat3.MaterialColor, i))
               .ToArray();
      this.AmbientColors =
          entry.AmbientColorIndexes
               .Select(i => GetOrNull_(mat3.AmbientColors, i))
               .ToArray();

      this.TevColors =
          entry.TevColorIndexes
               .Select(i => GetOrNull_(mat3.TevColors, i))
               .ToArray();
      this.KonstColors =
          entry.TevKonstColorIndexes
               .Select(i => GetOrNull_(mat3.TevKonstColors, i))
               .ToArray();

      this.ColorChannelControls =
          entry.ColorChannelControlIndexes
               .Select(i => GetOrNull_(mat3.ColorChannelControls, i))
               .ToArray();

      this.TevOrderInfos =
          entry.TevOrderInfoIndexes
               .Select(i => {
                 var tevOrder = GetOrNull_(mat3.TevOrders, i);
                 if (tevOrder == null) {
                   return null;
                 }

                 return new TevOrderWrapper(tevOrder) {
                   KonstAlphaSel = entry.KonstAlphaSel[i],
                   KonstColorSel = entry.KonstColorSel[i],
                 };
               })
               .ToArray();

      this.TevStageInfos =
          entry.TevStageInfoIndexes
               .Select(i => GetOrNull_(mat3.TevStages, i))
               .ToArray();

      this.TevSwapModes =
          entry.TevSwapModeInfo
               .Select(i => GetOrNull_(mat3.TevSwapModes, i))
               .ToArray();
      this.TevSwapModeTables =
          entry.TevSwapModeTable
               .Select(i => GetOrNull_(mat3.TevSwapModeTables, i))
               .ToArray();

      this.TextureIndices =
          entry.TextureIndexes
               .Select(t => (short)(t != -1 ? mat3.TextureIndices[t] : -1))
               .ToArray();

      this.TexCoordGens =
          entry.TexGenInfo
               .Select(i => GetOrNull_(mat3.TexCoordGens, i))
               .ToArray();

      this.AlphaCompare = mat3.AlphaCompares[entry.AlphaCompareIndex];
      this.BlendMode = mat3.BlendFunctions[entry.BlendModeIndex];
    }

    private class TevOrderWrapper : ITevOrder {
      private TevOrder impl_;

      public TevOrderWrapper(TevOrder impl) {
        this.impl_ = impl;
      }

      public byte TexCoordId => this.impl_.TexCoordId;
      public sbyte TexMap => this.impl_.TexMap;
      public GxColorChannel ColorChannelId => this.impl_.ColorChannelId;

      public GxKonstColorSel KonstColorSel { get; set; }
      public GxKonstAlphaSel KonstAlphaSel { get; set; }
    }

    private static T? GetOrNull_<T>(IList<T> array, int i)
        where T : notnull
      => i != -1 ? array[i] : default;
  }
}
