// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.PAL
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Misc
{
  public class PAL
  {
    public const string Signature = "RIFF";
    public DataBlockHeader Header;
    public PAL.PALdataSection PALdata;

    public PAL(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new DataBlockHeader(er, "RIFF", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.PALdata = new PAL.PALdataSection(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
      }
      er.Close();
    }

    public PAL(Color[] Palette)
    {
      this.Header = new DataBlockHeader("RIFF", (uint) (12 + Palette.Length * 4));
      this.PALdata = new PAL.PALdataSection(Palette);
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er, 12 + (int) this.PALdata.Header.SectionSize);
      this.PALdata.Write(er);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class PALdataSection
    {
      public const string Signature = "PAL data";
      public PAL.PALdataSection.PALdataSectionHeader Header;
      public ushort Unknown;
      public ushort NrColors;
      public Color[] Palette;

      public PALdataSection(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new PAL.PALdataSection.PALdataSectionHeader(er, "PAL data", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Unknown = er.ReadUInt16();
          this.NrColors = er.ReadUInt16();
          this.Palette = new Color[(int) this.NrColors];
          for (int index = 0; index < (int) this.NrColors; ++index)
            this.Palette[index] = Color.FromArgb(er.ReadInt32());
          OK = true;
        }
      }

      public PALdataSection(Color[] Colors)
      {
        this.Header = new PAL.PALdataSection.PALdataSectionHeader("PAL data", (uint) (Colors.Length * 4));
        this.Unknown = (ushort) 768;
        this.NrColors = (ushort) Colors.Length;
        this.Palette = Colors;
      }

      public void Write(EndianBinaryWriter er)
      {
        this.Header.Write(er, (uint) (this.Palette.Length * 4));
        er.Write(this.Unknown);
        er.Write((ushort) this.Palette.Length);
        foreach (Color color in this.Palette)
          er.Write((uint) color.ToArgb());
      }

      public class PALdataSectionHeader
      {
        public string Type;
        public uint SectionSize;

        public PALdataSectionHeader(EndianBinaryReader er, string Signature, out bool OK)
        {
          this.Type = er.ReadString(Encoding.ASCII, 8);
          if (this.Type != Signature)
          {
            OK = false;
          }
          else
          {
            this.SectionSize = er.ReadUInt32();
            OK = true;
          }
        }

        public PALdataSectionHeader(string Signature, uint SectionSize)
        {
          this.Type = Signature;
          this.SectionSize = SectionSize;
        }

        public void Write(EndianBinaryWriter er, uint SectionSize)
        {
          er.Write(this.Type, Encoding.ASCII, false);
          er.Write(SectionSize);
        }
      }
    }
  }
}
