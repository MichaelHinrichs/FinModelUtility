// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.BNCL
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Misc
{
  public class BNCL
  {
    public const string Signature = "JNCL";
    public BNCL.BNCLHeader Header;
    public BNCL.BNCLEntry[] Entries;

    public BNCL(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new BNCL.BNCLHeader(er, "JNCL", out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Entries = new BNCL.BNCLEntry[(int) this.Header.NrObjects];
        for (int index = 0; index < (int) this.Header.NrObjects; ++index)
          this.Entries[index] = new BNCL.BNCLEntry(er);
      }
      er.Close();
    }

    public class BNCLHeader
    {
      public string Type;
      public ushort Zero;
      public ushort NrObjects;

      public BNCLHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 4);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.Zero = er.ReadUInt16();
          this.NrObjects = er.ReadUInt16();
          OK = true;
        }
      }
    }

    public class BNCLEntry
    {
      public byte X;
      public byte Unknown1;
      public byte Y;
      public byte Unknown2;
      public int CellNr;

      public BNCLEntry(EndianBinaryReader er)
      {
        this.X = er.ReadByte();
        this.Unknown1 = er.ReadByte();
        this.Y = er.ReadByte();
        this.Unknown2 = er.ReadByte();
        this.CellNr = er.ReadInt32();
      }
    }
  }
}
