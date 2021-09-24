// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.BMG
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Misc
{
  public class BMG
  {
    public const string Signature = "MESGbmg1";
    public BMG.BMGHeader Header;
    public BMG.INF1Section INF1;
    public BMG.DAT1Section DAT1;

    public BMG(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new BMG.BMGHeader(er, "MESGbmg1", out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.INF1 = new BMG.INF1Section(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
        else
        {
          this.DAT1 = new BMG.DAT1Section(er, this.INF1, out OK);
          if (!OK)
          {
            int num3 = (int) MessageBox.Show("Error 3");
          }
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public byte[] Save()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.Write(er);
      long num = this.INF1.Write(er);
      uint[] numArray = this.DAT1.Write(er);
      er.BaseStream.Position = num;
      er.Write(numArray, 0, (int) this.INF1.NrOffset);
      er.BaseStream.Position = 8L;
      er.Write((uint) er.BaseStream.Length);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class BMGHeader
    {
      public string Type;
      public uint FileSize;
      public uint NrSections;
      public uint Unknown1;
      public uint Unknown2;
      public uint Unknown3;
      public uint Unknown4;

      public BMGHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 8);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.FileSize = er.ReadUInt32();
          this.NrSections = er.ReadUInt32();
          this.Unknown1 = er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.Unknown3 = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          OK = true;
        }
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.Type, Encoding.ASCII, false);
        er.Write(0U);
        er.Write(this.NrSections);
        er.Write(this.Unknown1);
        er.Write(this.Unknown2);
        er.Write(this.Unknown3);
        er.Write(this.Unknown4);
      }
    }

    public class INF1Section
    {
      public const string Signature = "INF1";
      public DataBlockHeader Header;
      public ushort NrOffset;
      public ushort Unknown1;
      public uint Unknown2;
      public uint[] Offsets;

      public INF1Section(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "INF1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrOffset = er.ReadUInt16();
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt32();
          this.Offsets = er.ReadUInt32s((int) this.NrOffset);
          er.ReadBytes((int) ((long) this.Header.size - (long) ((int) this.NrOffset * 4) - 16L));
          OK = true;
        }
      }

      public long Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        this.Header.Write(er, 0);
        er.Write(this.NrOffset);
        er.Write(this.Unknown1);
        er.Write(this.Unknown2);
        long position2 = er.BaseStream.Position;
        er.Write(new uint[(int) this.NrOffset], 0, (int) this.NrOffset);
        while (er.BaseStream.Position % 15L == 0L)
          er.Write((byte) 0);
        long position3 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position3 - position1));
        er.BaseStream.Position = position3;
        return position2;
      }
    }

    public class DAT1Section
    {
      public const string Signature = "DAT1";
      public DataBlockHeader Header;
      public string[] Strings;

      public DAT1Section(EndianBinaryReader er, BMG.INF1Section Offsets, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "DAT1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          er.SetMarkerOnCurrentOffset("DAT1");
          this.Strings = new string[(int) Offsets.NrOffset];
          for (int index = 0; index < (int) Offsets.NrOffset; ++index)
          {
            er.BaseStream.Position = er.GetMarker("DAT1") + (long) Offsets.Offsets[index];
            string str1 = "";
            while (true)
            {
              char ch = er.ReadChar(Encoding.Unicode);
              switch (ch)
              {
                case char.MinValue:
                  goto label_8;
                case '\n':
                  str1 += "\r\n";
                  continue;
                case '\x001A':
                  string str2 = str1 + "[#";
                  int count = (int) er.ReadByte() - 2;
                  --er.BaseStream.Position;
                  str1 = str2 + BitConverter.ToString(er.ReadBytes(count)).Replace("-", "") + "]";
                  continue;
                default:
                  str1 += (string) (object) ch;
                  continue;
              }
            }
label_8:
            this.Strings[index] = str1;
          }
          while (er.BaseStream.Position % 4L != 0L)
          {
            int num = (int) er.ReadByte();
          }
          OK = true;
        }
      }

      public uint[] Write(EndianBinaryWriter er)
      {
        List<uint> uintList = new List<uint>();
        long num1 = er.BaseStream.Position + 4L;
        this.Header.Write(er, 0);
        long position1 = er.BaseStream.Position;
        er.Write((ushort) 0);
        foreach (string str1 in this.Strings)
        {
          uintList.Add((uint) (er.BaseStream.Position - position1));
          for (int index1 = 0; index1 < str1.Length; ++index1)
          {
            if (str1[index1] == '\r')
            {
              er.Write('\n', Encoding.Unicode);
              ++index1;
            }
            else if (index1 != str1.Length - 1 && str1[index1] == '[' && str1[index1 + 1] == '#')
            {
              er.Write('\x001A', Encoding.Unicode);
              int num2 = index1 + 2;
              int num3;
              while (true)
              {
                string str2 = "";
                string str3 = str1;
                int index2 = num2;
                num3 = index2 + 1;
                char ch = str3[index2];
                if (ch != ']')
                {
                  string str4 = str2 + ch;
                  string str5 = str1;
                  int index3 = num3;
                  num2 = index3 + 1;
                  // ISSUE: variable of a boxed type
                  char local = str5[index3];
                  string a = str4 + local;
                  er.Write(Bytes.StringToByte(a)[0]);
                }
                else
                  break;
              }
              index1 = num3 - 1;
            }
            else
              er.Write(str1[index1], Encoding.Unicode);
          }
          er.Write((byte) 0);
          er.Write((byte) 0);
        }
        while (er.BaseStream.Position % 4L != 0L)
          er.Write((byte) 0);
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = num1;
        er.Write((uint) ((ulong) (position2 - num1) + 4UL));
        er.BaseStream.Position = position2;
        return uintList.ToArray();
      }

      public byte[] ToTxt()
      {
        MemoryStream memoryStream = new MemoryStream();
        TextWriter textWriter = (TextWriter) new StreamWriter((Stream) memoryStream, Encoding.Unicode);
        for (int index = 0; index < this.Strings.Length; ++index)
        {
          textWriter.Write(this.Strings[index]);
          textWriter.Write("/p");
        }
        textWriter.Flush();
        byte[] array = memoryStream.ToArray();
        textWriter.Close();
        return array;
      }

      public void FromTxt(byte[] file)
      {
        TextReader textReader = (TextReader) new StreamReader((Stream) new MemoryStream(file), Encoding.Unicode);
        this.Strings = textReader.ReadToEnd().Split(new string[1]
        {
          "/p"
        }, StringSplitOptions.RemoveEmptyEntries);
        textReader.Close();
      }
    }
  }
}
