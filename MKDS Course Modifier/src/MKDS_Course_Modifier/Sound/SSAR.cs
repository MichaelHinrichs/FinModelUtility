// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSAR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SSAR
  {
    public const string Signature = "SSAR";
    public FileHeader.HeaderInfo Header;
    public SSAR.DataSection Data;

    public SSAR(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (SSAR), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Data = new SSAR.DataSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public class DataSection
    {
      public const string Signature = "DATA";
      public DataBlockHeader Header;
      public uint DataOffset;
      public uint NrRecord;
      public SSAR.DataSection.Record[] Records;
      public byte[] Data;

      public DataSection(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "DATA", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.DataOffset = er.ReadUInt32();
          this.NrRecord = er.ReadUInt32();
          this.Records = new SSAR.DataSection.Record[(IntPtr) this.NrRecord];
          for (int index = 0; (long) index < (long) this.NrRecord; ++index)
            this.Records[index] = new SSAR.DataSection.Record(er);
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.DataOffset;
          this.Data = er.ReadBytes((int) (er.BaseStream.Length - (long) this.DataOffset));
          er.BaseStream.Position = position;
          OK = true;
        }
      }

      public class Record
      {
        public uint Offset;
        public ushort bnk;
        public byte vol;
        public byte cpr;
        public byte ppr;
        public byte ply;
        public byte[] unknown2;

        public Record(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
          this.bnk = er.ReadUInt16();
          this.vol = er.ReadByte();
          this.cpr = er.ReadByte();
          this.ppr = er.ReadByte();
          this.ply = er.ReadByte();
          this.unknown2 = er.ReadBytes(2);
        }
      }
    }
  }
}
