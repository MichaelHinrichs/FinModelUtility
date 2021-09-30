// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBVA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBVA
  {
    public const string Signature = "BVA0";
    public FileHeader Header;
    public NSBVA.VisAnmSet visAnmSet;

    public NSBVA(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BVA0", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 0");
      }
      else
      {
        this.visAnmSet = new NSBVA.VisAnmSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 1");
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public class VisAnmSet
    {
      public const string Signature = "VIS0";
      public DataBlockHeader header;
      public Dictionary<NSBVA.VisAnmSet.VisAnmSetData> dict;
      public NSBVA.VisAnmSet.VisAnm[] visAnm;

      public VisAnmSet(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset(nameof (VisAnmSet));
        bool OK1;
        this.header = new DataBlockHeader(er, "VIS0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.dict = new Dictionary<NSBVA.VisAnmSet.VisAnmSetData>(er);
          this.visAnm = new NSBVA.VisAnmSet.VisAnm[(int) this.dict.numEntry];
          long position = er.BaseStream.Position;
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            er.BaseStream.Position = er.GetMarker(nameof (VisAnmSet)) + (long) this.dict[index].Value.Offset;
            this.visAnm[index] = new NSBVA.VisAnmSet.VisAnm(er, out OK1);
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

      public class VisAnmSetData : DictionaryData
      {
        public uint Offset;

        public override void Read(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
        }
      }

      public class VisAnm
      {
        public AnmHeader anmHeader;
        public ushort numFrame;
        public ushort numNode;
        public ushort size;
        public ushort Padding;
        public uint[] visData;

        public VisAnm(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.anmHeader = new AnmHeader(er, AnmHeader.Category0.V, AnmHeader.Category1.AV, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.numFrame = er.ReadUInt16();
            this.numNode = er.ReadUInt16();
            this.size = er.ReadUInt16();
            this.Padding = er.ReadUInt16();
            this.visData = er.ReadUInt32s(1 + ((int) this.numFrame * (int) this.numNode >> 5));
            OK = true;
          }
        }

        public bool GetFrame(int FrameNr, int NodeID)
        {
          int num = FrameNr * (int) this.numNode + NodeID;
          return ((int) this.visData[num >> 5] & 1 << num) != 0;
        }
      }
    }
  }
}
