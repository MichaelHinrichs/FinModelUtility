// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SDAT
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound
{
  public class SDAT
  {
    public const string Signature = "SDAT";
    public FileHeader.HeaderInfo Header;
    public uint nSymbOffset;
    public uint nSymbSize;
    public uint nInfoOffset;
    public uint nInfoSize;
    public uint nFatOffset;
    public uint nFatSize;
    public uint nFileOffset;
    public uint nFileSize;
    public SDAT.SymbolBlock SYMB;
    public SDAT.InfoBlock INFO;
    public SDAT.FatBlock FAT;
    public SDAT.FileBlock FILE;

    public SDAT(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      er.ClearMarkers();
      bool OK;
      this.Header = new FileHeader.HeaderInfo(er, nameof (SDAT), out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.nSymbOffset = er.ReadUInt32();
        this.nSymbSize = er.ReadUInt32();
        this.nInfoOffset = er.ReadUInt32();
        this.nInfoSize = er.ReadUInt32();
        this.nFatOffset = er.ReadUInt32();
        this.nFatSize = er.ReadUInt32();
        this.nFileOffset = er.ReadUInt32();
        this.nFileSize = er.ReadUInt32();
        er.ReadBytes(16);
        er.BaseStream.Position = (long) this.nSymbOffset;
        if (this.nSymbSize != 0U)
        {
          this.SYMB = new SDAT.SymbolBlock(er, out OK);
          if (!OK)
          {
            int num2 = (int) MessageBox.Show("Error 2");
            goto label_11;
          }
        }
        er.BaseStream.Position = (long) this.nInfoOffset;
        this.INFO = new SDAT.InfoBlock(er, out OK);
        if (!OK)
        {
          int num3 = (int) MessageBox.Show("Error 3");
        }
        else
        {
          er.BaseStream.Position = (long) this.nFatOffset;
          this.FAT = new SDAT.FatBlock(er, out OK);
          if (!OK)
          {
            int num2 = (int) MessageBox.Show("Error 4");
          }
          else
          {
            er.BaseStream.Position = (long) this.nFileOffset;
            this.FILE = new SDAT.FileBlock(er, out OK);
            if (!OK)
            {
              int num4 = (int) MessageBox.Show("Error 5");
            }
          }
        }
      }
label_11:
      er.ClearMarkers();
      er.Close();
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      er.Write(0U);
      er.Write(0U);
      er.Write(0U);
      er.Write(0U);
      er.Write(0U);
      er.Write(0U);
      er.Write(0U);
      er.Write(0U);
      er.Write(new byte[16], 0, 16);
      if (this.SYMB != null)
      {
        long position = er.BaseStream.Position;
        er.BaseStream.Position = 16L;
        er.Write((uint) position);
        er.BaseStream.Position = position;
        throw new NotSupportedException("Symbol block writing is not implemented yet.");
      }
      long position1 = er.BaseStream.Position;
      er.BaseStream.Position = 24L;
      er.Write((uint) position1);
      er.BaseStream.Position = position1;
      this.INFO.Write(er);
      long position2 = er.BaseStream.Position;
      er.BaseStream.Position = 28L;
      er.Write((uint) (position2 - position1));
      er.BaseStream.Position = position2;
      long position3 = er.BaseStream.Position;
      er.BaseStream.Position = 32L;
      er.Write((uint) position3);
      er.BaseStream.Position = position3;
      this.FAT.Write(er);
      long position4 = er.BaseStream.Position;
      er.BaseStream.Position = 36L;
      er.Write((uint) (position4 - position3));
      er.BaseStream.Position = position4;
      long position5 = er.BaseStream.Position;
      er.BaseStream.Position = 40L;
      er.Write((uint) position5);
      er.BaseStream.Position = position5;
      this.FILE.Write(er);
      long position6 = er.BaseStream.Position;
      er.BaseStream.Position = 44L;
      er.Write((uint) (position6 - position5));
      er.BaseStream.Position = position6;
      long position7 = er.BaseStream.Position;
      er.BaseStream.Position = 8L;
      er.Write((uint) position7);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class SymbolBlock
    {
      public const string Signature = "SYMB";
      public DataBlockHeader Header;
      public uint[] nRecOffset;
      public SDAT.SymbolBlock.SymbolRecord SEQRecord;
      public SDAT.SymbolBlock.SymbolRecord2 SEQARCRecord;
      public SDAT.SymbolBlock.SymbolRecord BANKRecord;
      public SDAT.SymbolBlock.SymbolRecord WAVEARCRecord;
      public SDAT.SymbolBlock.SymbolRecord PLAYERRecord;
      public SDAT.SymbolBlock.SymbolRecord GROUPRecord;
      public SDAT.SymbolBlock.SymbolRecord PLAYER2Record;
      public SDAT.SymbolBlock.SymbolRecord STRMRecord;

      public SymbolBlock(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset("Symbol");
        bool OK1;
        this.Header = new DataBlockHeader(er, "SYMB", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.nRecOffset = er.ReadUInt32s(8);
          er.ReadBytes(24);
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.nRecOffset[0] + er.GetMarker("Symbol");
          this.SEQRecord = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = (long) this.nRecOffset[1] + er.GetMarker("Symbol");
          this.SEQARCRecord = new SDAT.SymbolBlock.SymbolRecord2(er);
          er.BaseStream.Position = (long) this.nRecOffset[2] + er.GetMarker("Symbol");
          this.BANKRecord = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = (long) this.nRecOffset[3] + er.GetMarker("Symbol");
          this.WAVEARCRecord = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = (long) this.nRecOffset[4] + er.GetMarker("Symbol");
          this.PLAYERRecord = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = (long) this.nRecOffset[5] + er.GetMarker("Symbol");
          this.GROUPRecord = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = (long) this.nRecOffset[6] + er.GetMarker("Symbol");
          this.PLAYER2Record = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = (long) this.nRecOffset[7] + er.GetMarker("Symbol");
          this.STRMRecord = new SDAT.SymbolBlock.SymbolRecord(er);
          er.BaseStream.Position = position;
          er.RemoveMarker("Symbol");
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        this.Header.Write(er, 0);
        long position2 = er.BaseStream.Position;
        er.Write(new uint[8], 0, 8);
        er.Write(new byte[24], 0, 24);
        int NamesOffset = (int) (er.BaseStream.Position - position1 + (long) (this.SEQRecord.Names.Length * 4) + 4L + (long) (this.SEQARCRecord.Entries.Length * 8) + 4L + (long) (this.BANKRecord.Names.Length * 4) + 4L + (long) (this.WAVEARCRecord.Names.Length * 4) + 4L + (long) (this.PLAYERRecord.Names.Length * 4) + 4L + (long) (this.GROUPRecord.Names.Length * 4) + 4L + (long) (this.PLAYER2Record.Names.Length * 4) + 4L + (long) (this.STRMRecord.Names.Length * 4) + 4L);
        long position3 = er.BaseStream.Position;
        er.BaseStream.Position = position2;
        er.Write((uint) (position3 - position1));
        er.BaseStream.Position = position3;
        this.SEQRecord.Write(er, (int) position1, ref NamesOffset);
        long position4 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 4L;
        er.Write((uint) (position4 - position1));
        er.BaseStream.Position = position4;
        this.SEQARCRecord.Write(er, (int) position1, ref NamesOffset);
        long position5 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 8L;
        er.Write((uint) (position5 - position1));
        er.BaseStream.Position = position5;
        this.BANKRecord.Write(er, (int) position1, ref NamesOffset);
        long position6 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 12L;
        er.Write((uint) (position6 - position1));
        er.BaseStream.Position = position6;
        this.WAVEARCRecord.Write(er, (int) position1, ref NamesOffset);
        long position7 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 16L;
        er.Write((uint) (position7 - position1));
        er.BaseStream.Position = position7;
        this.PLAYERRecord.Write(er, (int) position1, ref NamesOffset);
        long position8 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 20L;
        er.Write((uint) (position8 - position1));
        er.BaseStream.Position = position8;
        this.GROUPRecord.Write(er, (int) position1, ref NamesOffset);
        long position9 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 24L;
        er.Write((uint) (position9 - position1));
        er.BaseStream.Position = position9;
        this.PLAYER2Record.Write(er, (int) position1, ref NamesOffset);
        long position10 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 28L;
        er.Write((uint) (position10 - position1));
        er.BaseStream.Position = position10;
        this.STRMRecord.Write(er, (int) position1, ref NamesOffset);
        er.BaseStream.Position = er.BaseStream.Length;
        while (er.BaseStream.Position % 16L != 0L)
          er.Write((byte) 0);
        long position11 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position11 - position1));
        er.BaseStream.Position = position11;
      }

      public class SymbolRecord
      {
        public uint nCount;
        public uint[] nEntryOffset;
        public string[] Names;

        public SymbolRecord(EndianBinaryReader er)
        {
          this.nCount = er.ReadUInt32();
          this.nEntryOffset = er.ReadUInt32s((int) this.nCount);
          long position = er.BaseStream.Position;
          this.Names = new string[(IntPtr) this.nCount];
          for (int index = 0; (long) index < (long) this.nCount; ++index)
          {
            if (this.nEntryOffset[index] != 0U)
            {
              er.BaseStream.Position = (long) this.nEntryOffset[index] + er.GetMarker("Symbol");
              this.Names[index] = er.ReadStringNT(Encoding.ASCII);
            }
          }
          er.BaseStream.Position = position;
        }

        public void Write(EndianBinaryWriter er, int BaseOffset, ref int NamesOffset)
        {
          er.Write((uint) this.Names.Length);
          int num = NamesOffset;
          for (int index = 0; index < this.Names.Length; ++index)
          {
            if (this.Names[index] != null)
            {
              er.Write((uint) NamesOffset);
              NamesOffset += this.Names[index].Length + 1;
            }
          }
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) (BaseOffset + num);
          for (int index = 0; index < this.Names.Length; ++index)
          {
            if (this.Names[index] != null)
              er.Write(this.Names[index], Encoding.ASCII, true);
          }
          er.BaseStream.Position = position;
        }
      }

      public class SymbolRecord2
      {
        public uint nCount;
        public SDAT.SymbolBlock.SymbolRecord2.Entry[] Entries;

        public SymbolRecord2(EndianBinaryReader er)
        {
          this.nCount = er.ReadUInt32();
          this.Entries = new SDAT.SymbolBlock.SymbolRecord2.Entry[(IntPtr) this.nCount];
          for (int index = 0; (long) index < (long) this.nCount; ++index)
            this.Entries[index] = new SDAT.SymbolBlock.SymbolRecord2.Entry(er);
        }

        public void Write(EndianBinaryWriter er, int BaseOffset, ref int NamesOffset)
        {
          er.Write((uint) this.Entries.Length);
          foreach (SDAT.SymbolBlock.SymbolRecord2.Entry entry in this.Entries)
            entry.Write(er, BaseOffset, ref NamesOffset);
        }

        public class Entry
        {
          public uint GroupOffset;
          public uint SubRecordsOffset;
          public string GroupName;
          public SDAT.SymbolBlock.SymbolRecord SubRecords;

          public Entry(EndianBinaryReader er)
          {
            this.GroupOffset = er.ReadUInt32();
            this.SubRecordsOffset = er.ReadUInt32();
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.GroupOffset + er.GetMarker("Symbol");
            this.GroupName = er.ReadStringNT(Encoding.ASCII);
            er.BaseStream.Position = (long) this.SubRecordsOffset + er.GetMarker("Symbol");
            this.SubRecords = new SDAT.SymbolBlock.SymbolRecord(er);
            er.BaseStream.Position = position;
          }

          public void Write(EndianBinaryWriter er, int BaseOffset, ref int NamesOffset)
          {
            int num = NamesOffset;
            er.Write((uint) NamesOffset);
            NamesOffset += this.GroupName.Length + 1;
            long position1 = er.BaseStream.Position;
            er.BaseStream.Position = (long) (num + BaseOffset);
            er.Write(this.GroupName, Encoding.ASCII, true);
            er.BaseStream.Position = position1;
            er.Write((uint) NamesOffset);
            long position2 = er.BaseStream.Position;
            er.BaseStream.Position = (long) (NamesOffset + BaseOffset);
            NamesOffset += this.SubRecords.Names.Length * 4 + 4;
            this.SubRecords.Write(er, BaseOffset, ref NamesOffset);
            er.BaseStream.Position = position2;
          }
        }
      }
    }

    public class InfoBlock
    {
      public const string Signature = "INFO";
      public DataBlockHeader Header;
      public uint[] nRecOffset;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.SEQInfo> SEQRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.SEQARCInfo> SEQARCRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.BANKInfo> BANKRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.WAVEARCInfo> WAVEARCRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.PLAYERInfo> PLAYERRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.GROUPInfo> GROUPRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.STREAMPLAYERInfo> STREAMPLAYERRecord;
      public SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.STREAMInfo> STREAMRecord;

      public InfoBlock(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset("Info");
        bool OK1;
        this.Header = new DataBlockHeader(er, "INFO", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.nRecOffset = er.ReadUInt32s(8);
          er.ReadBytes(24);
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.nRecOffset[0] + er.GetMarker("Info");
          this.SEQRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.SEQInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[1] + er.GetMarker("Info");
          this.SEQARCRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.SEQARCInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[2] + er.GetMarker("Info");
          this.BANKRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.BANKInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[3] + er.GetMarker("Info");
          this.WAVEARCRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.WAVEARCInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[4] + er.GetMarker("Info");
          this.PLAYERRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.PLAYERInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[5] + er.GetMarker("Info");
          this.GROUPRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.GROUPInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[6] + er.GetMarker("Info");
          this.STREAMPLAYERRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.STREAMPLAYERInfo>(er);
          er.BaseStream.Position = (long) this.nRecOffset[7] + er.GetMarker("Info");
          this.STREAMRecord = new SDAT.InfoBlock.InfoRecord<SDAT.InfoBlock.STREAMInfo>(er);
          er.BaseStream.Position = position;
          er.RemoveMarker("Info");
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        this.Header.Write(er, 0);
        long position2 = er.BaseStream.Position;
        er.Write(new uint[8], 0, 8);
        er.Write(new byte[24], 0, 24);
        long position3 = er.BaseStream.Position;
        er.BaseStream.Position = position2;
        er.Write((uint) (position3 - position1));
        er.BaseStream.Position = position3;
        this.SEQRecord.Write(er, (int) position1);
        long position4 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 4L;
        er.Write((uint) (position4 - position1));
        er.BaseStream.Position = position4;
        this.SEQARCRecord.Write(er, (int) position1);
        long position5 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 8L;
        er.Write((uint) (position5 - position1));
        er.BaseStream.Position = position5;
        this.BANKRecord.Write(er, (int) position1);
        long position6 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 12L;
        er.Write((uint) (position6 - position1));
        er.BaseStream.Position = position6;
        this.WAVEARCRecord.Write(er, (int) position1);
        long position7 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 16L;
        er.Write((uint) (position7 - position1));
        er.BaseStream.Position = position7;
        this.PLAYERRecord.Write(er, (int) position1);
        long position8 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 20L;
        er.Write((uint) (position8 - position1));
        er.BaseStream.Position = position8;
        this.GROUPRecord.Write(er, (int) position1);
        long position9 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 24L;
        er.Write((uint) (position9 - position1));
        er.BaseStream.Position = position9;
        this.STREAMPLAYERRecord.Write(er, (int) position1);
        long position10 = er.BaseStream.Position;
        er.BaseStream.Position = position2 + 28L;
        er.Write((uint) (position10 - position1));
        er.BaseStream.Position = position10;
        this.STREAMRecord.Write(er, (int) position1);
        long position11 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position11 - position1));
        er.BaseStream.Position = position11;
      }

      public class InfoRecord<T> where T : SDAT.InfoBlock.SDATInfo, new()
      {
        public uint nCount;
        public uint[] nEntryOffset;
        public List<T> Entries;

        public InfoRecord(EndianBinaryReader er)
        {
          this.nCount = er.ReadUInt32();
          this.nEntryOffset = er.ReadUInt32s((int) this.nCount);
          long position = er.BaseStream.Position;
          this.Entries = new List<T>();
          for (int index = 0; (long) index < (long) this.nCount; ++index)
          {
            er.BaseStream.Position = (long) this.nEntryOffset[index] + er.GetMarker("Info");
            this.Entries.Add(new T());
            this.Entries[index].Read(er);
          }
          er.BaseStream.Position = position;
        }

        public void Write(EndianBinaryWriter er, int BaseOffset)
        {
          er.Write((uint) this.Entries.Count);
          int num = (int) (er.BaseStream.Position - (long) BaseOffset + (long) (4 * this.Entries.Count));
          for (int index = 0; index < this.Entries.Count; ++index)
          {
            er.Write((uint) num);
            num += this.Entries[index].GetLength();
          }
          for (int index = 0; index < this.Entries.Count; ++index)
            this.Entries[index].Write(er);
        }

        public T this[int i]
        {
          get
          {
            return this.Entries[i];
          }
          set
          {
            this.Entries[i] = value;
          }
        }
      }

      public class SDATInfo
      {
        public virtual void Read(EndianBinaryReader er)
        {
        }

        public virtual void Write(EndianBinaryWriter er)
        {
        }

        public virtual int GetLength()
        {
          return -1;
        }
      }

      public class SEQInfo : SDAT.InfoBlock.SDATInfo
      {
        public uint fileID;
        public byte[] unknown2;

        public override int GetLength()
        {
          return 12;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.fileID = er.ReadUInt32();
          this.bank = er.ReadUInt16();
          this.volume = er.ReadByte();
          this.channelPrio = er.ReadByte();
          this.playerPrio = er.ReadByte();
          this.playerNo = er.ReadByte();
          this.unknown2 = er.ReadBytes(2);
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.fileID);
          er.Write(this.bank);
          er.Write(this.volume);
          er.Write(this.channelPrio);
          er.Write(this.playerPrio);
          er.Write(this.playerNo);
          er.Write(this.unknown2, 0, 2);
        }

        public ushort bank { get; set; }

        public byte volume { get; set; }

        public byte channelPrio { get; set; }

        public byte playerPrio { get; set; }

        public byte playerNo { get; set; }
      }

      public class SEQARCInfo : SDAT.InfoBlock.SDATInfo
      {
        public uint fileID;

        public override int GetLength()
        {
          return 4;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.fileID = er.ReadUInt32();
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.fileID);
        }
      }

      public class BANKInfo : SDAT.InfoBlock.SDATInfo
      {
        public uint fileID;

        public override int GetLength()
        {
          return 12;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.fileID = er.ReadUInt32();
          this.wavearc = er.ReadUInt16s(4);
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.fileID);
          er.Write(this.wavearc, 0, 4);
        }

        public ushort[] wavearc { get; private set; }
      }

      public class WAVEARCInfo : SDAT.InfoBlock.SDATInfo
      {
        public uint fileID;

        public override int GetLength()
        {
          return 4;
        }

        public override void Read(EndianBinaryReader er)
        {
          uint num = er.ReadUInt32();
          this.fileID = num & 16777215U;
          this.flags = (byte) (num >> 24);
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write((uint) ((ulong) ((int) this.flags << 24) | (ulong) (this.fileID & 16777215U)));
        }

        public byte flags { get; set; }
      }

      public class PLAYERInfo : SDAT.InfoBlock.SDATInfo
      {
        public byte padding;

        public override int GetLength()
        {
          return 8;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.seqMax = er.ReadByte();
          this.padding = er.ReadByte();
          this.allocChBitFlag = er.ReadUInt16();
          this.heapSize = er.ReadUInt32();
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.seqMax);
          er.Write(this.padding);
          er.Write(this.allocChBitFlag);
          er.Write(this.heapSize);
        }

        public byte seqMax { get; set; }

        public ushort allocChBitFlag { get; set; }

        public uint heapSize { get; set; }
      }

      public class GROUPInfo : SDAT.InfoBlock.SDATInfo
      {
        public uint nCount;

        public override int GetLength()
        {
          return 4 + this.SubRecords.Count * 8;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.nCount = er.ReadUInt32();
          this.SubRecords = new List<SDAT.InfoBlock.GROUPInfo.GroupItem>();
          for (int index = 0; (long) index < (long) this.nCount; ++index)
            this.SubRecords.Add(new SDAT.InfoBlock.GROUPInfo.GroupItem(er));
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write((uint) this.SubRecords.Count);
          foreach (SDAT.InfoBlock.GROUPInfo.GroupItem subRecord in this.SubRecords)
            subRecord.Write(er);
        }

        public List<SDAT.InfoBlock.GROUPInfo.GroupItem> SubRecords { get; set; }

        public class GroupItem
        {
          public ushort padding;

          public GroupItem(EndianBinaryReader er)
          {
            this.type = er.ReadByte();
            this.loadFlag = er.ReadByte();
            this.padding = er.ReadUInt16();
            this.index = er.ReadUInt32();
          }

          public void Write(EndianBinaryWriter er)
          {
            er.Write(this.type);
            er.Write(this.loadFlag);
            er.Write(this.padding);
            er.Write(this.index);
          }

          public byte type { get; set; }

          public byte loadFlag { get; set; }

          public uint index { get; set; }
        }
      }

      public class STREAMPLAYERInfo : SDAT.InfoBlock.SDATInfo
      {
        public byte[] padding;

        public override int GetLength()
        {
          return 24;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.numChannels = er.ReadByte();
          this.chNo = er.ReadBytes(16);
          this.padding = er.ReadBytes(7);
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.numChannels);
          er.Write(this.chNo, 0, 16);
          er.Write(this.padding, 0, 7);
        }

        public byte numChannels { get; set; }

        public byte[] chNo { get; set; }
      }

      public class STREAMInfo : SDAT.InfoBlock.SDATInfo
      {
        public uint fileID;

        public override int GetLength()
        {
          return 8;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.fileID = er.ReadUInt32();
          this.volume = er.ReadByte();
          this.playerPrio = er.ReadByte();
          this.playerNo = er.ReadByte();
          this.flags = er.ReadByte();
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.fileID);
          er.Write(this.volume);
          er.Write(this.playerPrio);
          er.Write(this.playerNo);
          er.Write(this.flags);
        }

        public byte volume { get; set; }

        public byte playerPrio { get; set; }

        public byte playerNo { get; set; }

        public byte flags { get; set; }
      }
    }

    public class FatBlock
    {
      public const string Signature = "FAT ";
      public DataBlockHeader Header;
      public uint nCount;
      public SDAT.FatBlock.FatRecord[] Records;

      public FatBlock(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "FAT ", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.nCount = er.ReadUInt32();
          this.Records = new SDAT.FatBlock.FatRecord[(IntPtr) this.nCount];
          for (int index = 0; (long) index < (long) this.nCount; ++index)
            this.Records[index] = new SDAT.FatBlock.FatRecord(er);
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        this.Header.Write(er, 0);
        er.Write((uint) this.Records.Length);
        long FileOffset = er.BaseStream.Position + (long) (this.Records.Length * 16) + 12L;
        while (FileOffset % 32L != 0L)
          ++FileOffset;
        for (int index = 0; index < this.Records.Length; ++index)
          this.Records[index].Write(er, ref FileOffset);
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position2 - position1));
        er.BaseStream.Position = position2;
      }

      public class FatRecord
      {
        public uint nOffset;
        public uint nSize;
        public byte[] Data;

        public FatRecord(EndianBinaryReader er)
        {
          this.nOffset = er.ReadUInt32();
          this.nSize = er.ReadUInt32();
          er.ReadBytes(8);
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.nOffset;
          this.Data = er.ReadBytes((int) this.nSize);
          er.BaseStream.Position = position;
        }

        public void Write(EndianBinaryWriter er, ref long FileOffset)
        {
          er.Write((uint) FileOffset);
          er.Write((uint) this.Data.Length);
          er.Write(new byte[8], 0, 8);
          long position = er.BaseStream.Position;
          er.BaseStream.Position = FileOffset;
          er.Write(this.Data, 0, this.Data.Length);
          int num = 0;
          while (er.BaseStream.Position % 32L != 0L)
          {
            er.Write((byte) 0);
            ++num;
          }
          er.BaseStream.Position = position;
          FileOffset += (long) (this.Data.Length + num);
        }
      }
    }

    public class FileBlock
    {
      public const string Signature = "FILE";
      public DataBlockHeader Header;
      public uint nCount;

      public FileBlock(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "FILE", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.nCount = er.ReadUInt32();
          while (er.BaseStream.Position % 32L != 0L)
          {
            int num = (int) er.ReadByte();
          }
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        long position = er.BaseStream.Position;
        this.Header.Write(er, (int) (er.BaseStream.Length - position));
        er.Write(this.nCount);
        while (er.BaseStream.Position % 32L != 0L)
          er.Write((byte) 0);
        er.BaseStream.Position = er.BaseStream.Length;
      }
    }
  }
}
