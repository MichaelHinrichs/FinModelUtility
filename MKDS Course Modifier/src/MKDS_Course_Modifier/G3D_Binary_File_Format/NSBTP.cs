// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTP
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBTP
  {
    public const string Signature = "BTP0";
    public FileHeader Header;
    public NSBTP.TexPatAnmSet texPatAnmSet;

    public NSBTP(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BTP0", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.texPatAnmSet = new NSBTP.TexPatAnmSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public class TexPatAnmSet
    {
      public const string Signature = "PAT0";
      public DataBlockHeader header;
      public Dictionary<NSBTP.TexPatAnmSet.TexPatAnmSetData> dict;
      public NSBTP.TexPatAnmSet.TexPatAnm[] texPatAnm;

      public TexPatAnmSet(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset(nameof (TexPatAnmSet));
        bool OK1;
        this.header = new DataBlockHeader(er, "PAT0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.dict = new Dictionary<NSBTP.TexPatAnmSet.TexPatAnmSetData>(er);
          this.texPatAnm = new NSBTP.TexPatAnmSet.TexPatAnm[(int) this.dict.numEntry];
          long position = er.BaseStream.Position;
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            er.BaseStream.Position = er.GetMarker(nameof (TexPatAnmSet)) + (long) this.dict[index].Value.Offset;
            this.texPatAnm[index] = new NSBTP.TexPatAnmSet.TexPatAnm(er, out OK1);
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

      public class TexPatAnmSetData : DictionaryData
      {
        public uint Offset;

        public override void Read(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
        }
      }

      public class TexPatAnm
      {
        public AnmHeader anmHeader;
        public ushort numFrame;
        public byte numTex;
        public byte numPltt;
        public ushort ofsTexName;
        public ushort ofsPlttName;
        public Dictionary<NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData> dict;
        public NSBTP.TexPatAnmSet.TexPatAnm.DictName[] texName;
        public NSBTP.TexPatAnmSet.TexPatAnm.DictName[] plttName;

        public TexPatAnm(EndianBinaryReader er, out bool OK)
        {
          er.SetMarkerOnCurrentOffset(nameof (TexPatAnm));
          bool OK1;
          this.anmHeader = new AnmHeader(er, AnmHeader.Category0.M, AnmHeader.Category1.PT, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.numFrame = er.ReadUInt16();
            this.numTex = er.ReadByte();
            this.numPltt = er.ReadByte();
            this.ofsTexName = er.ReadUInt16();
            this.ofsPlttName = er.ReadUInt16();
            this.dict = new Dictionary<NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData>(er);
            long position = er.BaseStream.Position;
            er.BaseStream.Position = er.GetMarker(nameof (TexPatAnm)) + (long) this.ofsTexName;
            this.texName = new NSBTP.TexPatAnmSet.TexPatAnm.DictName[(int) this.numTex];
            for (int index = 0; index < (int) this.numTex; ++index)
              this.texName[index] = new NSBTP.TexPatAnmSet.TexPatAnm.DictName(er);
            er.BaseStream.Position = er.GetMarker(nameof (TexPatAnm)) + (long) this.ofsPlttName;
            this.plttName = new NSBTP.TexPatAnmSet.TexPatAnm.DictName[(int) this.numPltt];
            for (int index = 0; index < (int) this.numPltt; ++index)
              this.plttName[index] = new NSBTP.TexPatAnmSet.TexPatAnm.DictName(er);
            er.BaseStream.Position = position;
            er.RemoveMarker(nameof (TexPatAnm));
            OK = true;
          }
        }

        public class DictTexPatAnmData : DictionaryData
        {
          public ushort numFV;
          public ushort flag;
          public float ratioDataFrame;
          public ushort offset;
          public NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData.TexPatFV[] texPatFV;

          public override void Read(EndianBinaryReader er)
          {
            this.numFV = er.ReadUInt16();
            this.flag = er.ReadUInt16();
            this.ratioDataFrame = er.ReadSingleInt16Exp12();
            this.offset = er.ReadUInt16();
            this.texPatFV = new NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData.TexPatFV[(int) this.numFV];
            long position = er.BaseStream.Position;
            er.BaseStream.Position = er.GetMarker(nameof (TexPatAnm)) + (long) this.offset;
            for (int index = 0; index < (int) this.numFV; ++index)
              this.texPatFV[index] = new NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData.TexPatFV(er);
            er.BaseStream.Position = position;
          }

          public void GetData(out int TexIdx, out int PlttIdx, int Frame)
          {
            TexIdx = (int) this.texPatFV[(int) ((double) Frame / (1.0 / (double) this.ratioDataFrame))].idTex;
            PlttIdx = (int) this.texPatFV[(int) ((double) Frame / (1.0 / (double) this.ratioDataFrame))].idPltt;
          }

          public class TexPatFV
          {
            public ushort idxFrame;
            public byte idTex;
            public byte idPltt;

            public TexPatFV(EndianBinaryReader er)
            {
              this.idxFrame = er.ReadUInt16();
              this.idTex = er.ReadByte();
              this.idPltt = er.ReadByte();
            }
          }
        }

        public class DictName
        {
          public string name;

          public DictName(EndianBinaryReader er)
          {
            this.name = er.ReadString(Encoding.ASCII, 16).Replace("\0", "");
          }

          public static implicit operator string(NSBTP.TexPatAnmSet.TexPatAnm.DictName n)
          {
            return n.name;
          }
        }
      }
    }
  }
}
