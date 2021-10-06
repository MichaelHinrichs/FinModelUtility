// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.PAZ
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.IO;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Misc
{
  public class PAZ
  {
    public const string Signature = "add\0";
    public PAZ.PAZHeader Header;
    public List<PAZ.PAZFileEntry> Entries;

    public PAZ(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new PAZ.PAZHeader(er, out OK);
      if (!OK)
      {
        int num = (int) System.Windows.MessageBox.Show("Error 1");
      }
      else
      {
        this.Entries = new List<PAZ.PAZFileEntry>();
        for (int index = 0; (long) index < (long) this.Header.NrFiles; ++index)
          this.Entries.Add(new PAZ.PAZFileEntry(er));
      }
      er.Close();
    }

    public PAZ.PAZFileEntry GetFileByName(string Name)
    {
      foreach (PAZ.PAZFileEntry entry in this.Entries)
      {
        if (entry.Name == Name)
          return entry;
      }
      return (PAZ.PAZFileEntry) null;
    }

    public void GetDirectoryContents(ListView l)
    {
      foreach (PAZ.PAZFileEntry entry in this.Entries)
      {
        ListViewItem l1 = new ListViewItem(entry.Name);
        l1.SubItems.Add(FileSize.FormatSize(entry.Data.Length));
        ByteFileInfo file = new ByteFileInfo(entry);
        string type = FileHandler.GetType(file);
        if (file.IsLZ77Compressed)
          l1.ForeColor = Color.Blue;
        file.Dispose();
        FileHandler.SetListViewItemInfo(l1, type, l);
        l.Items.Add(l1);
      }
    }

    public class PAZHeader
    {
      public string Type;
      public uint Unknown1;
      public uint Offset1;
      public uint Offset2;
      public uint NrFiles;
      public ulong FileSize;
      public uint Padding;

      public PAZHeader(EndianBinaryReader er, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != "add\0")
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadUInt32();
          this.Offset1 = er.ReadUInt32();
          this.Offset2 = er.ReadUInt32();
          this.NrFiles = er.ReadUInt32();
          this.FileSize = er.ReadUInt64();
          this.Padding = er.ReadUInt32();
          OK = true;
        }
      }
    }

    public class PAZFileEntry
    {
      public uint Offset;
      public ulong Size;
      public ushort Unknown1;
      public ushort Unknown2;
      public uint NameOffset;
      public uint Padding1;
      public uint Padding2;
      public uint Padding3;
      public string Name;
      public byte[] Data;

      public PAZFileEntry(EndianBinaryReader er)
      {
        this.Offset = er.ReadUInt32();
        this.Size = er.ReadUInt64();
        this.Unknown1 = er.ReadUInt16();
        this.Unknown2 = er.ReadUInt16();
        this.NameOffset = er.ReadUInt32();
        this.Padding1 = er.ReadUInt32();
        this.Padding2 = er.ReadUInt32();
        this.Padding3 = er.ReadUInt32();
        long position = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.NameOffset;
        this.Name = er.ReadStringNT(Encoding.ASCII);
        er.BaseStream.Position = (long) this.Offset;
        this.Data = er.ReadBytes((int) this.Size);
        er.BaseStream.Position = position;
      }

      public override string ToString()
      {
        return this.Name;
      }
    }
  }
}
