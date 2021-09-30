// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBTX
  {
    public const string Signature = "BTX0";
    public FileHeader Header;
    public NSBTX.TexplttSet TexPlttSet;

    public NSBTX(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BTX0", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        er.SetMarkerOnCurrentOffset("TexplttSet");
        this.TexPlttSet = new NSBTX.TexplttSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public NSBTX()
    {
      this.Header = new FileHeader("BTX0", (ushort) 1);
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      long position = er.BaseStream.Position;
      er.BaseStream.Position = 16L;
      er.Write((uint) position);
      er.BaseStream.Position = position;
      this.TexPlttSet.Write(er);
      er.BaseStream.Position = 8L;
      er.Write((uint) er.BaseStream.Length);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class TexplttSet
    {
      public const string Signature = "TEX0";
      public DataBlockHeader Header;
      public NSBTX.TexplttSet.texInfo TexInfo;
      public NSBTX.TexplttSet.tex4x4Info Tex4x4Info;
      public NSBTX.TexplttSet.plttInfo PlttInfo;
      public Dictionary<NSBTX.TexplttSet.DictTexData> dictTex;
      public Dictionary<NSBTX.TexplttSet.DictPlttData> dictPltt;

      public TexplttSet(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "TEX0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.TexInfo = new NSBTX.TexplttSet.texInfo(er);
          this.Tex4x4Info = new NSBTX.TexplttSet.tex4x4Info(er);
          this.PlttInfo = new NSBTX.TexplttSet.plttInfo(er);
          this.dictTex = new Dictionary<NSBTX.TexplttSet.DictTexData>(er);
          for (int index = 0; index < (int) this.dictTex.numEntry; ++index)
            this.dictTex[index].Value.ReadData(er, this.TexInfo.ofsTex, this.Tex4x4Info.ofsTex, this.Tex4x4Info.ofsTexPlttIdx);
          this.dictPltt = new Dictionary<NSBTX.TexplttSet.DictPlttData>(er);
          List<uint> source = new List<uint>();
          for (int index = 0; index < (int) this.dictPltt.numEntry; ++index)
            source.Add(this.dictPltt[index].Value.offset);
          List<uint> list = source.Distinct<uint>().ToList<uint>();
          list.Sort();
          for (int index1 = 0; index1 < (int) this.dictPltt.numEntry; ++index1)
          {
            int index2 = list.IndexOf(this.dictPltt[index1].Value.offset);
            if (index2 == list.Count - 1)
              this.dictPltt[index1].Value.ReadData(er, this.PlttInfo.ofsPlttData, (uint) er.BaseStream.Length - (list[index2] + this.PlttInfo.ofsPlttData + (uint) er.GetMarker(nameof (TexplttSet))));
            else
              this.dictPltt[index1].Value.ReadData(er, this.PlttInfo.ofsPlttData, list[index2 + 1] - list[index2]);
          }
          OK = true;
        }
      }

      public TexplttSet()
      {
        this.Header = new DataBlockHeader("TEX0", 0U);
      }

      public void Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        this.Header.Write(er, 0);
        List<byte> byteList1 = new List<byte>();
        List<byte> byteList2 = new List<byte>();
        List<byte> byteList3 = new List<byte>();
        foreach (NSBTX.TexplttSet.DictTexData dictTexData in this.dictTex.entry.data)
        {
          if (dictTexData.Fmt != Graphic.GXTexFmt.GX_TEXFMT_COMP4x4)
          {
            byteList1.AddRange((IEnumerable<byte>) dictTexData.Data);
          }
          else
          {
            byteList2.AddRange((IEnumerable<byte>) dictTexData.Data);
            byteList3.AddRange((IEnumerable<byte>) dictTexData.Data4x4);
          }
        }
        List<byte> byteList4 = new List<byte>();
        foreach (NSBTX.TexplttSet.DictPlttData dictPlttData in this.dictPltt.entry.data)
          byteList4.AddRange((IEnumerable<byte>) dictPlttData.Data);
        this.TexInfo.ofsDict = (ushort) 60;
        this.TexInfo.sizeTex = (uint) byteList1.Count;
        this.TexInfo.ofsTex = (uint) (68 + ((int) this.dictTex.numEntry + 1) * 4 + 4 + (int) this.dictTex.numEntry * 8 + (int) this.dictTex.numEntry * 16 + 8 + ((int) this.dictPltt.numEntry + 1) * 4 + 4 + (int) this.dictPltt.numEntry * 4 + (int) this.dictPltt.numEntry * 16);
        this.Tex4x4Info.ofsDict = (ushort) 60;
        this.Tex4x4Info.sizeTex = (uint) byteList2.Count;
        this.Tex4x4Info.ofsTex = (uint) (68 + ((int) this.dictTex.numEntry + 1) * 4 + 4 + (int) this.dictTex.numEntry * 8 + (int) this.dictTex.numEntry * 16 + 8 + ((int) this.dictPltt.numEntry + 1) * 4 + 4 + (int) this.dictPltt.numEntry * 4 + (int) this.dictPltt.numEntry * 16 + byteList1.Count);
        this.Tex4x4Info.ofsTexPlttIdx = (uint) (68 + ((int) this.dictTex.numEntry + 1) * 4 + 4 + (int) this.dictTex.numEntry * 8 + (int) this.dictTex.numEntry * 16 + 8 + ((int) this.dictPltt.numEntry + 1) * 4 + 4 + (int) this.dictPltt.numEntry * 4 + (int) this.dictPltt.numEntry * 16 + byteList1.Count + byteList2.Count);
        this.PlttInfo.ofsDict = (ushort) (68 + ((int) this.dictTex.numEntry + 1) * 4 + 4 + (int) this.dictTex.numEntry * 8 + (int) this.dictTex.numEntry * 16);
        this.PlttInfo.sizePltt = (uint) byteList4.Count;
        this.PlttInfo.ofsPlttData = (uint) (68 + ((int) this.dictTex.numEntry + 1) * 4 + 4 + (int) this.dictTex.numEntry * 8 + (int) this.dictTex.numEntry * 16 + 8 + ((int) this.dictPltt.numEntry + 1) * 4 + 4 + (int) this.dictPltt.numEntry * 4 + (int) this.dictPltt.numEntry * 16 + byteList1.Count + byteList2.Count + byteList3.Count);
        this.TexInfo.Write(er);
        this.Tex4x4Info.Write(er);
        this.PlttInfo.Write(er);
        uint num1 = 0;
        uint num2 = 0;
        for (int index = 0; index < (int) this.dictTex.numEntry; ++index)
        {
          KeyValuePair<string, NSBTX.TexplttSet.DictTexData> keyValuePair = this.dictTex[index];
          if (keyValuePair.Value.Fmt != Graphic.GXTexFmt.GX_TEXFMT_COMP4x4)
          {
            keyValuePair = this.dictTex[index];
            keyValuePair.Value.Offset = num1;
            int num3 = (int) num1;
            keyValuePair = this.dictTex[index];
            int length = keyValuePair.Value.Data.Length;
            num1 = (uint) (num3 + length);
          }
          else
          {
            keyValuePair = this.dictTex[index];
            keyValuePair.Value.Offset = num2;
            int num3 = (int) num2;
            keyValuePair = this.dictTex[index];
            int length = keyValuePair.Value.Data.Length;
            num2 = (uint) (num3 + length);
          }
        }
        this.dictTex.Write(er);
        uint num4 = 0;
        for (int index = 0; index < (int) this.dictPltt.numEntry; ++index)
        {
          KeyValuePair<string, NSBTX.TexplttSet.DictPlttData> keyValuePair = this.dictPltt[index];
          keyValuePair.Value.offset = num4;
          int num3 = (int) num4;
          keyValuePair = this.dictPltt[index];
          int length = keyValuePair.Value.Data.Length;
          num4 = (uint) (num3 + length);
        }
        this.dictPltt.Write(er);
        er.Write(byteList1.ToArray(), 0, byteList1.Count);
        er.Write(byteList2.ToArray(), 0, byteList2.Count);
        er.Write(byteList3.ToArray(), 0, byteList3.Count);
        er.Write(byteList4.ToArray(), 0, byteList4.Count);
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position2 - position1));
        er.BaseStream.Position = position2;
      }

      public class texInfo
      {
        public uint vramKey;
        public uint sizeTex;
        public ushort ofsDict;
        public ushort flag;
        public uint ofsTex;

        public texInfo(EndianBinaryReader er)
        {
          this.vramKey = er.ReadUInt32();
          this.sizeTex = (uint) er.ReadUInt16() << 3;
          this.ofsDict = er.ReadUInt16();
          this.flag = er.ReadUInt16();
          er.ReadBytes(2);
          this.ofsTex = er.ReadUInt32();
        }

        public texInfo()
        {
        }

        public void Write(EndianBinaryWriter er)
        {
          er.Write(this.vramKey);
          er.Write((ushort) (this.sizeTex >> 3));
          er.Write(this.ofsDict);
          er.Write(this.flag);
          er.Write((ushort) 0);
          er.Write(this.ofsTex);
        }
      }

      public class tex4x4Info
      {
        public uint vramKey;
        public uint sizeTex;
        public ushort ofsDict;
        public ushort flag;
        public uint ofsTex;
        public uint ofsTexPlttIdx;

        public tex4x4Info(EndianBinaryReader er)
        {
          this.vramKey = er.ReadUInt32();
          this.sizeTex = (uint) er.ReadUInt16() << 3;
          this.ofsDict = er.ReadUInt16();
          this.flag = er.ReadUInt16();
          er.ReadBytes(2);
          this.ofsTex = er.ReadUInt32();
          this.ofsTexPlttIdx = er.ReadUInt32();
        }

        public tex4x4Info()
        {
        }

        public void Write(EndianBinaryWriter er)
        {
          er.Write(this.vramKey);
          er.Write((ushort) (this.sizeTex >> 3));
          er.Write(this.ofsDict);
          er.Write(this.flag);
          er.Write((ushort) 0);
          er.Write(this.ofsTex);
          er.Write(this.ofsTexPlttIdx);
        }
      }

      public class plttInfo
      {
        public uint vramKey;
        public uint sizePltt;
        public ushort flag;
        public ushort ofsDict;
        public uint ofsPlttData;

        public plttInfo(EndianBinaryReader er)
        {
          this.vramKey = er.ReadUInt32();
          this.sizePltt = (uint) er.ReadUInt16() << 3;
          this.flag = er.ReadUInt16();
          this.ofsDict = er.ReadUInt16();
          er.ReadBytes(2);
          this.ofsPlttData = er.ReadUInt32();
        }

        public plttInfo()
        {
        }

        public void Write(EndianBinaryWriter er)
        {
          er.Write(this.vramKey);
          er.Write((ushort) (this.sizePltt >> 3));
          er.Write(this.flag);
          er.Write(this.ofsDict);
          er.Write((ushort) 0);
          er.Write(this.ofsPlttData);
        }
      }

      public class DictTexData : DictionaryData
      {
        private int[] DataLength = new int[8]
        {
          0,
          8,
          2,
          4,
          8,
          2,
          8,
          16
        };
        public uint texImageParam;
        public uint Offset;
        public ushort S;
        public ushort T;
        public Graphic.GXTexFmt Fmt;
        public bool TransparentColor;
        public uint extraParam;
        public byte[] Data;
        public byte[] Data4x4;

        public override ushort GetDataSize()
        {
          return 8;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.texImageParam = er.ReadUInt32();
          this.Offset = (uint) (((int) this.texImageParam & (int) ushort.MaxValue) << 3);
          this.S = (ushort) (8U << (int) ((this.texImageParam & 7340032U) >> 20));
          this.T = (ushort) (8U << (int) ((this.texImageParam & 58720256U) >> 23));
          this.Fmt = (Graphic.GXTexFmt) ((this.texImageParam & 469762048U) >> 26);
          this.TransparentColor = ((int) (this.texImageParam >> 29) & 1) == 1;
          this.extraParam = er.ReadUInt32();
        }

        public override void Write(EndianBinaryWriter er)
        {
          int num1 = 0;
          int num2 = 0;
          do
          {
            ++num1;
          }
          while (8 << num1 != (int) this.S);
          do
          {
            ++num2;
          }
          while (8 << num2 != (int) this.T);
          this.texImageParam = (uint) ((ulong) ((this.TransparentColor ? 1 : 0) << 29 | (int) (byte) this.Fmt << 26 | (num2 & 7) << 23 | (num1 & 7) << 20) | (ulong) (this.Offset >> 3 & (uint) ushort.MaxValue));
          er.Write(this.texImageParam);
          this.extraParam = (uint) (int.MinValue | ((int) this.T & 1023) << 11 | (int) this.S & 1023);
          er.Write(this.extraParam);
        }

        public void ReadData(
          EndianBinaryReader er,
          uint BaseOffsetTex,
          uint BaseOffsetTex4x4,
          uint BaseOffsetTex4x4Info)
        {
          long position = er.BaseStream.Position;
          if (this.Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4)
          {
            er.BaseStream.Position = (long) (this.Offset + BaseOffsetTex4x4) + er.GetMarker(nameof (TexplttSet));
            this.Data = er.ReadBytes((int) this.S * (int) this.T * this.DataLength[(int) this.Fmt] / 8);
            er.BaseStream.Position = (long) (this.Offset / 2U + BaseOffsetTex4x4Info) + er.GetMarker(nameof (TexplttSet));
            this.Data4x4 = er.ReadBytes((int) this.S * (int) this.T * this.DataLength[(int) this.Fmt] / 8 / 2);
          }
          else
          {
            er.BaseStream.Position = (long) (this.Offset + BaseOffsetTex) + er.GetMarker(nameof (TexplttSet));
            this.Data = er.ReadBytes((int) this.S * (int) this.T * this.DataLength[(int) this.Fmt] / 8);
          }
          er.BaseStream.Position = position;
        }

        public Bitmap ToBitmap(NSBTX.TexplttSet.DictPlttData Palette)
        {
          return Graphic.ConvertData(this.Data, Palette?.Data, this.Data4x4, 0, (int) this.S, (int) this.T, this.Fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, this.TransparentColor, true);
        }
      }

      public class DictPlttData : DictionaryData
      {
        public uint offset;
        public ushort flag;
        public byte[] Data;

        public override ushort GetDataSize()
        {
          return 4;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.offset = (uint) er.ReadUInt16() << 3;
          this.flag = er.ReadUInt16();
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write((ushort) (this.offset >> 3));
          er.Write(this.flag);
        }

        public void ReadData(EndianBinaryReader er, uint BaseOffsetPltt, uint Length)
        {
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) (this.offset + BaseOffsetPltt) + er.GetMarker(nameof (TexplttSet));
          this.Data = er.ReadBytes((int) Length);
          er.BaseStream.Position = position;
        }
      }
    }
  }
}
