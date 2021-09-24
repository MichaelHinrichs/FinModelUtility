// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBMA
  {
    public const string Signature = "BMA0";
    public FileHeader Header;
    public NSBMA.MatColAnmSet matColAnmSet;

    public NSBMA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BMA0", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.matColAnmSet = new NSBMA.MatColAnmSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public class MatColAnmSet
    {
      public const string Signature = "MAT0";
      public DataBlockHeader header;
      public Dictionary<NSBMA.MatColAnmSet.MatColAnmSetData> dict;
      public NSBMA.MatColAnmSet.MatColAnm[] matColAnm;

      public MatColAnmSet(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset(nameof (MatColAnmSet));
        bool OK1;
        this.header = new DataBlockHeader(er, "MAT0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.dict = new Dictionary<NSBMA.MatColAnmSet.MatColAnmSetData>(er);
          this.matColAnm = new NSBMA.MatColAnmSet.MatColAnm[(int) this.dict.numEntry];
          long position = er.BaseStream.Position;
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            er.BaseStream.Position = er.GetMarker(nameof (MatColAnmSet)) + (long) this.dict[index].Value.Offset;
            this.matColAnm[index] = new NSBMA.MatColAnmSet.MatColAnm(er, out OK1);
            if (!OK1)
            {
              OK = false;
              return;
            }
          }
          er.BaseStream.Position = position;
          OK = true;
        }
      }

      public class MatColAnmSetData : DictionaryData
      {
        public uint Offset;

        public override void Read(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
        }
      }

      public class MatColAnm
      {
        public AnmHeader anmHeader;
        public ushort numFrame;
        public ushort flag;
        public Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict;

        public MatColAnm(EndianBinaryReader er, out bool OK)
        {
          er.SetMarkerOnCurrentOffset(nameof (MatColAnm));
          bool OK1;
          this.anmHeader = new AnmHeader(er, AnmHeader.Category0.M, AnmHeader.Category1.AM, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.numFrame = er.ReadUInt16();
            this.flag = er.ReadUInt16();
            this.dict = new Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData>(er);
            er.RemoveMarker(nameof (MatColAnm));
            OK = true;
          }
        }

        public class DictMatColAnmData : DictionaryData
        {
          private uint NNS_G3D_MATCANM_ELEM_STEP_MASK = 3221225472;
          private uint NNS_G3D_MATCANM_ELEM_STEP_1 = 0;
          private uint NNS_G3D_MATCANM_ELEM_STEP_2 = 1073741824;
          private uint NNS_G3D_MATCANM_ELEM_STEP_4 = 2147483648;
          private uint NNS_G3D_MATCANM_ELEM_CONST = 536870912;
          private uint NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK = 536805376;
          private uint NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK = (uint) ushort.MaxValue;
          public uint tagDiffuse;
          public uint tagAmbient;
          public uint tagSpecular;
          public uint tagEmission;
          public uint tagPolygonAlpha;
          public ushort ConstDiffuse;
          public ushort ConstAmbient;
          public ushort ConstSpecular;
          public ushort ConstEmission;
          public byte ConstPolygonAlpha;
          public ushort[] Diffuse;
          public ushort[] Ambient;
          public ushort[] Specular;
          public ushort[] Emission;
          public byte[] PolygonAlpha;

          public override void Read(EndianBinaryReader er)
          {
            this.tagDiffuse = er.ReadUInt32();
            this.tagAmbient = er.ReadUInt32();
            this.tagSpecular = er.ReadUInt32();
            this.tagEmission = er.ReadUInt32();
            this.tagPolygonAlpha = er.ReadUInt32();
            long position = er.BaseStream.Position;
            if (((int) this.tagDiffuse & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
            {
              this.ConstDiffuse = (ushort) (this.tagDiffuse & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
            }
            else
            {
              er.BaseStream.Position = er.GetMarker(nameof (MatColAnm)) + (long) (ushort) (this.tagDiffuse & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
              int num = ((int) this.tagDiffuse & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0 ? (((int) this.tagDiffuse & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) != 0 ? 4 : 2) : 1;
              this.Diffuse = er.ReadUInt16s((int) ((this.tagDiffuse & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16) / num);
            }
            if (((int) this.tagAmbient & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
            {
              this.ConstAmbient = (ushort) (this.tagAmbient & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
            }
            else
            {
              er.BaseStream.Position = er.GetMarker(nameof (MatColAnm)) + (long) (ushort) (this.tagAmbient & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
              int num = ((int) this.tagAmbient & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0 ? (((int) this.tagAmbient & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) != 0 ? 4 : 2) : 1;
              this.Ambient = er.ReadUInt16s((int) ((this.tagAmbient & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16) / num);
            }
            if (((int) this.tagSpecular & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
            {
              this.ConstSpecular = (ushort) (this.tagSpecular & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
            }
            else
            {
              er.BaseStream.Position = er.GetMarker(nameof (MatColAnm)) + (long) (ushort) (this.tagSpecular & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
              int num = ((int) this.tagSpecular & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0 ? (((int) this.tagSpecular & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) != 0 ? 4 : 2) : 1;
              this.Specular = er.ReadUInt16s((int) ((this.tagSpecular & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16) / num);
            }
            if (((int) this.tagEmission & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
            {
              this.ConstEmission = (ushort) (this.tagEmission & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
            }
            else
            {
              er.BaseStream.Position = er.GetMarker(nameof (MatColAnm)) + (long) (ushort) (this.tagEmission & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
              int num = ((int) this.tagEmission & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0 ? (((int) this.tagEmission & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) != 0 ? 4 : 2) : 1;
              this.Emission = er.ReadUInt16s((int) ((this.tagEmission & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16) / num);
            }
            if (((int) this.tagPolygonAlpha & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
            {
              this.ConstPolygonAlpha = (byte) (this.tagPolygonAlpha & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
            }
            else
            {
              er.BaseStream.Position = er.GetMarker(nameof (MatColAnm)) + (long) (ushort) (this.tagPolygonAlpha & this.NNS_G3D_MATCANM_ELEM_OFFSET_CONSTANT_MASK);
              int num = ((int) this.tagPolygonAlpha & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0 ? (((int) this.tagPolygonAlpha & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) != 0 ? 4 : 2) : 1;
              this.PolygonAlpha = er.ReadBytes((int) ((this.tagPolygonAlpha & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16) / num);
            }
            er.BaseStream.Position = position;
          }

          public ushort GetValue(uint Info, ushort data, ushort[] arraydata, int Frame)
          {
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
              return data;
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_STEP_MASK) == 0)
              return arraydata[Frame];
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0)
            {
              if ((Frame & 1) == 0)
                return arraydata[Frame >> 1];
              return (long) Frame > (long) ((Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16) ? arraydata[(((Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16 >> 1) + 1U)] : (ushort) (((int) arraydata[Frame >> 1] + (int) arraydata[(Frame >> 1) + 1]) / 2);
            }
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) == 0)
              return 0;
            if ((Frame & 3) == 0)
              return arraydata[Frame >> 2];
            if ((long) Frame > (long) ((Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16))
              return arraydata[(long) ((Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) >> 16 >> 2) + (long) (Frame & 3)];
            if ((Frame & 1) == 0)
              return (ushort) (((int) arraydata[Frame >> 2] + (int) arraydata[(Frame >> 2) + 1]) / 2);
            int index1;
            int index2;
            if ((Frame & 2) != 0)
            {
              index1 = Frame >> 2;
              index2 = index1 + 1;
            }
            else
            {
              index2 = Frame >> 2;
              index1 = index2 + 1;
            }
            ushort num1 = arraydata[index2];
            ushort num2 = arraydata[index1];
            return (ushort) (((int) num1 + (int) num1 + (int) num1 + (int) num2) / 4);
          }

          public byte GetValue(uint Info, byte data, byte[] arraydata, int Frame)
          {
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_CONST) != 0)
              return data;
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_STEP_MASK) == 0)
              return arraydata[Frame];
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_STEP_2) != 0)
            {
              if ((Frame & 1) == 0)
                return arraydata[Frame >> 1];
              return (long) Frame > (long) (Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK) ? arraydata[(Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK >> 1)] : (byte) (((int) arraydata[Frame >> 1] + (int) arraydata[(Frame >> 1) + 1]) / 2);
            }
            if (((int) Info & (int) this.NNS_G3D_MATCANM_ELEM_STEP_4) == 0)
              return 0;
            if ((Frame & 3) == 0)
              return arraydata[Frame >> 2];
            if ((long) Frame > (long) (Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK))
              return arraydata[(Info & this.NNS_G3D_MATCANM_ELEM_LAST_INTERP_MASK >> 2)];
            if ((Frame & 1) == 0)
              return (byte) (((int) arraydata[Frame >> 2] + (int) arraydata[(Frame >> 2) + 1]) / 2);
            int index1;
            int index2;
            if ((Frame & 2) != 0)
            {
              index1 = Frame >> 2;
              index2 = index1 + 1;
            }
            else
            {
              index2 = Frame >> 2;
              index1 = index2 + 1;
            }
            ushort num1 = (ushort) arraydata[index2];
            ushort num2 = (ushort) arraydata[index1];
            return (byte) (((int) num1 + (int) num1 + (int) num1 + (int) num2) / 4);
          }
        }
      }
    }
  }
}
