// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BLO
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G3D_Binary_File_Format;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using Tao.OpenGl;

namespace MKDS_Course_Modifier.GCN
{
  public class BLO
  {
    public BLO.BLOHeader Header;
    public BLO.SVR1 Svr1;
    public BLO.INF1 Inf1;
    public BLO.TEX1 Tex1;
    public BLO.FNT1 Fnt1;
    public BLO.MAT1 Mat1;
    public BLO.PAN2 ROOT;
    public BLO.EXT1 Ext1;

    public BLO(byte[] Data, string Path)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(Data), Endianness.BigEndian);
      bool OK;
      this.Header = new BLO.BLOHeader(er, out OK);
      if (!OK)
      {
        int num1 = (int) MessageBox.Show("Error 1");
      }
      else
      {
        this.Svr1 = new BLO.SVR1(er, out OK);
        if (!OK)
        {
          int num2 = (int) MessageBox.Show("Error 2");
        }
        else
        {
          this.Inf1 = new BLO.INF1(er, out OK);
          if (!OK)
          {
            int num3 = (int) MessageBox.Show("Error 3");
          }
          else
          {
            this.Tex1 = new BLO.TEX1(er, Path, out OK);
            if (!OK)
            {
              int num4 = (int) MessageBox.Show("Error 4");
            }
            else
            {
              this.Fnt1 = new BLO.FNT1(er, out OK);
              if (!OK)
              {
                int num5 = (int) MessageBox.Show("Error 5");
              }
              else
              {
                this.Mat1 = new BLO.MAT1(er, out OK);
                if (!OK)
                {
                  int num6 = (int) MessageBox.Show("Error 6");
                }
                else
                {
                  this.ROOT = new BLO.PAN2(er, out OK);
                  if (!OK)
                  {
                    int num7 = (int) MessageBox.Show("Error 7");
                  }
                  else
                  {
                    Stack<BLO.BLOLayoutElement> bloLayoutElementStack = new Stack<BLO.BLOLayoutElement>();
                    string str1 = "-";
                    BLO.BLOLayoutElement bloLayoutElement = new BLO.BLOLayoutElement();
                    bloLayoutElement.Children.Add((BLO.BLOLayoutElement) this.ROOT);
                    Console.WriteLine(str1 + " " + this.ROOT.ToString() + " - PAN2");
                    while (er.BaseStream.Position < er.BaseStream.Length)
                    {
                      string str2 = er.ReadString(Encoding.ASCII, 4);
                      er.BaseStream.Position -= 4L;
                      switch (str2)
                      {
                        case "PAN2":
                          bloLayoutElement.Children.Add((BLO.BLOLayoutElement) new BLO.PAN2(er, out OK));
                          bloLayoutElement.Children.Last<BLO.BLOLayoutElement>().Parent = bloLayoutElement;
                          Console.WriteLine(str1 + " " + bloLayoutElement.Children.Last<BLO.BLOLayoutElement>().ToString() + " - PAN2");
                          break;
                        case "PIC2":
                          bloLayoutElement.Children.Add((BLO.BLOLayoutElement) new BLO.PIC2(er, out OK));
                          bloLayoutElement.Children.Last<BLO.BLOLayoutElement>().Parent = bloLayoutElement;
                          Console.WriteLine(str1 + " " + bloLayoutElement.Children.Last<BLO.BLOLayoutElement>().ToString() + " - PIC2");
                          break;
                        case "WIN2":
                          bloLayoutElement.Children.Add((BLO.BLOLayoutElement) new BLO.WIN2(er, out OK));
                          bloLayoutElement.Children.Last<BLO.BLOLayoutElement>().Parent = bloLayoutElement;
                          Console.WriteLine(str1 + " " + bloLayoutElement.Children.Last<BLO.BLOLayoutElement>().ToString() + " - WIN2");
                          break;
                        case "BGN1":
                          er.BaseStream.Position += 8L;
                          bloLayoutElementStack.Push(bloLayoutElement);
                          bloLayoutElement = bloLayoutElement.Children.Last<BLO.BLOLayoutElement>();
                          str1 += "-";
                          break;
                        case "END1":
                          er.BaseStream.Position += 8L;
                          bloLayoutElement = bloLayoutElementStack.Pop();
                          str1 = str1.Remove(str1.Length - 1);
                          break;
                        case "EXT1":
                          this.Ext1 = new BLO.EXT1(er, out OK);
                          break;
                        case "This":
                          goto label_25;
                        default:
                          int num8 = (int) MessageBox.Show("Unknown Section: " + str2);
                          er.BaseStream.Position += 4L;
                          er.BaseStream.Position += (long) (er.ReadUInt32() - 4U);
                          break;
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
label_25:
      er.Close();
    }

    public System.Drawing.Bitmap ToBitmap()
    {
      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int) this.Inf1.Width, (int) this.Inf1.Height);
      Graphics g = Graphics.FromImage((Image) bitmap);
      this.DrawPAN2(ref g, this.ROOT);
      g.Flush();
      g.Dispose();
      return bitmap;
    }

    private void DrawPAN2(ref Graphics g, BLO.PAN2 Pane)
    {
      foreach (BLO.BLOLayoutElement child in Pane.Children)
      {
        if (child is BLO.PAN2)
          this.DrawPAN2(ref g, (BLO.PAN2) child);
        else if (child is BLO.PIC2)
          this.DrawPIC2(ref g, (BLO.PIC2) child);
      }
    }

    private void DrawPIC2(ref Graphics g, BLO.PIC2 Picture)
    {
      System.Drawing.Bitmap bitmap = this.Tex1.Textures[(int) this.Mat1.TextureIndieces[(int) this.Mat1.MaterialEntries[(int) this.Mat1.MaterialEntryIndieces[(int) Picture.MaterialID]].TextureIndices[0]]].ToBitmap();
      float x = Picture.BasePane.Position.X;
      float y = Picture.BasePane.Position.Y;
      g.DrawImage((Image) bitmap, x, y, Picture.BasePane.Size.X, Picture.BasePane.Size.Y);
      foreach (BLO.BLOLayoutElement child in Picture.Children)
      {
        if (child is BLO.PAN2)
          this.DrawPAN2(ref g, (BLO.PAN2) child);
        else if (child is BLO.PIC2)
          this.DrawPIC2(ref g, (BLO.PIC2) child);
      }
    }

    public void BindTextures()
    {
      int Nr = 0;
      foreach (int materialEntryIndiece in this.Mat1.MaterialEntryIndieces)
      {
        foreach (short textureIndex in this.Mat1.MaterialEntries[materialEntryIndiece].TextureIndices)
        {
          ++Nr;
          if (textureIndex != (short) -1)
          {
            BTI texture = this.Tex1.Textures[(int) this.Mat1.TextureIndieces[(int) textureIndex]];
            if (texture != null)
              GlNitro.glNitroTexImage2D(texture.ToBitmap(), Nr, texture.Header.GetGlWrapModeS(), texture.Header.GetGlWrapModeT(), texture.Header.GetGlFilterModeMin(), texture.Header.GetGlFilterModeMag());
          }
        }
      }
    }

    public class BLOHeader
    {
      public string Signature1;
      public string Signature2;
      public uint FileSize;
      public uint NrSections;

      public BLOHeader(EndianBinaryReader er, out bool OK)
      {
        this.Signature1 = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature1 != "SCRN")
        {
          OK = false;
        }
        else
        {
          this.Signature2 = er.ReadString(Encoding.ASCII, 4);
          if (this.Signature2 != "blo2")
          {
            OK = false;
          }
          else
          {
            this.FileSize = er.ReadUInt32();
            this.NrSections = er.ReadUInt32();
            OK = true;
          }
        }
      }
    }

    public class SVR1
    {
      public string Signature;
      public byte[] Unknown;

      public SVR1(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (SVR1))
        {
          OK = false;
        }
        else
        {
          this.Unknown = er.ReadBytes(12);
          OK = true;
        }
      }
    }

    public class INF1
    {
      public string Signature;
      public uint SectionSize;
      public ushort Width;
      public ushort Height;
      public uint Unknown;

      public INF1(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (INF1))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          this.Width = er.ReadUInt16();
          this.Height = er.ReadUInt16();
          this.Unknown = er.ReadUInt32();
          while (er.BaseStream.Position % 32L != 0L)
          {
            int num = (int) er.ReadByte();
          }
          OK = true;
        }
      }
    }

    public class TEX1
    {
      public string Signature;
      public uint SectionSize;
      public ushort NrTextures;
      public ushort Unknown;
      public uint FileNameOffsetTableOffset;
      public ushort NrEntries;
      public ushort[] FileNameOffsets;
      public string[] FileNames;
      public BTI[] Textures;

      public TEX1(EndianBinaryReader er, string path, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (TEX1))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          this.NrTextures = er.ReadUInt16();
          this.Unknown = er.ReadUInt16();
          this.FileNameOffsetTableOffset = er.ReadUInt32();
          er.BaseStream.Position = position1 + (long) this.FileNameOffsetTableOffset;
          this.NrEntries = er.ReadUInt16();
          long position2 = er.BaseStream.Position;
          this.FileNameOffsets = er.ReadUInt16s((int) this.NrEntries);
          this.FileNames = new string[(int) this.NrEntries];
          int num1 = 0;
          foreach (ushort fileNameOffset in this.FileNameOffsets)
          {
            er.BaseStream.Position = position2 + (long) fileNameOffset - 1L;
            this.FileNames[num1++] = er.ReadString(Encoding.ASCII, (int) er.ReadByte());
          }
          this.Textures = new BTI[(int) this.NrEntries];
          path = new DirectoryInfo(Path.GetDirectoryName(path)).Parent.FullName + "\\timg\\";
          if (Directory.Exists(path))
          {
            int num2 = 0;
            foreach (string fileName in this.FileNames)
            {
              try
              {
                this.Textures[num2++] = new BTI(File.ReadAllBytes(path + fileName));
              }
              catch
              {
              }
            }
          }
          er.BaseStream.Position = position1 + (long) this.SectionSize;
          OK = true;
        }
      }
    }

    public class FNT1
    {
      public string Signature;
      public uint SectionSize;
      public ushort NrFonts;
      public ushort Unknown;
      public uint FileNameOffsetTableOffset;
      public ushort NrEntries;
      public ushort[] FileNameOffsets;
      public string[] FileNames;

      public FNT1(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (FNT1))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          this.NrFonts = er.ReadUInt16();
          this.Unknown = er.ReadUInt16();
          this.FileNameOffsetTableOffset = er.ReadUInt32();
          er.BaseStream.Position = position1 + (long) this.FileNameOffsetTableOffset;
          this.NrEntries = er.ReadUInt16();
          long position2 = er.BaseStream.Position;
          this.FileNameOffsets = er.ReadUInt16s((int) this.NrEntries);
          this.FileNames = new string[(int) this.NrEntries];
          int num = 0;
          foreach (ushort fileNameOffset in this.FileNameOffsets)
          {
            er.BaseStream.Position = position2 + (long) fileNameOffset - 1L;
            this.FileNames[num++] = er.ReadString(Encoding.ASCII, (int) er.ReadByte());
          }
          er.BaseStream.Position = position1 + (long) this.SectionSize;
          OK = true;
        }
      }
    }

    public class MAT1
    {
      public string Signature;
      public uint SectionSize;
      public ushort NrMaterials;
      public ushort Unknown1;
      public uint FirstMaterialOffset;
      public uint[] Offsets;
      public BLO.MAT1.MaterialEntry[] MaterialEntries;
      public ushort[] MaterialEntryIndieces;
      public BLO.MAT1.MaterialNames Names;
      public System.Drawing.Color[] Color1;
      public System.Drawing.Color[] Color2;
      public BLO.MAT1.TexMatrix[] Texmatrices;
      public ushort[] TextureIndieces;
      public BLO.MAT1.TevOrder[] Tevorders;
      public System.Drawing.Color[] ColorS10;
      public System.Drawing.Color[] Color3;
      public BLO.MAT1.TevStageProps[] TevStages;
      public BLO.MAT1.AlphaCompare[] AlphaCompares;
      public BLO.MAT1.BlendFunction[] BlendFunctions;

      public MAT1(EndianBinaryReader er, out bool OK)
      {
        long position = er.BaseStream.Position;
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (MAT1))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          this.NrMaterials = er.ReadUInt16();
          this.Unknown1 = er.ReadUInt16();
          this.Offsets = er.ReadUInt32s(23);
          int[] sectionLengths = this.GetSectionLengths();
          er.BaseStream.Position = position + (long) this.Offsets[0];
          this.MaterialEntries = new BLO.MAT1.MaterialEntry[sectionLengths[0] / 232];
          for (int index = 0; index < (int) this.NrMaterials; ++index)
            this.MaterialEntries[index] = new BLO.MAT1.MaterialEntry(er);
          er.BaseStream.Position = position + (long) this.Offsets[1];
          this.MaterialEntryIndieces = er.ReadUInt16s((int) this.NrMaterials);
          er.BaseStream.Position = position + (long) this.Offsets[2];
          this.Names = new BLO.MAT1.MaterialNames(er);
          er.BaseStream.Position = position + (long) this.Offsets[5];
          this.Color1 = new System.Drawing.Color[sectionLengths[5] / 4];
          for (int index = 0; index < sectionLengths[5] / 4; ++index)
            this.Color1[index] = er.ReadColor8();
          er.BaseStream.Position = position + (long) this.Offsets[10];
          this.Texmatrices = new BLO.MAT1.TexMatrix[sectionLengths[10] / 36];
          for (int index = 0; index < sectionLengths[10] / 36; ++index)
            this.Texmatrices[index] = new BLO.MAT1.TexMatrix(er);
          er.BaseStream.Position = position + (long) this.Offsets[11];
          this.TextureIndieces = er.ReadUInt16s(sectionLengths[11] / 2);
          er.BaseStream.Position = position + (long) this.Offsets[13];
          this.Tevorders = new BLO.MAT1.TevOrder[sectionLengths[13] / 4];
          for (int index = 0; index < sectionLengths[13] / 4; ++index)
            this.Tevorders[index] = new BLO.MAT1.TevOrder(er);
          er.BaseStream.Position = position + (long) this.Offsets[14];
          this.ColorS10 = new System.Drawing.Color[sectionLengths[14] / 8];
          for (int index = 0; index < sectionLengths[14] / 8; ++index)
            this.ColorS10[index] = er.ReadColor16();
          er.BaseStream.Position = position + (long) this.Offsets[15];
          this.Color3 = new System.Drawing.Color[sectionLengths[15] / 4];
          for (int index = 0; index < sectionLengths[15] / 4; ++index)
            this.Color3[index] = er.ReadColor8();
          er.BaseStream.Position = position + (long) this.Offsets[17];
          this.TevStages = new BLO.MAT1.TevStageProps[sectionLengths[17] / 20];
          for (int index = 0; index < sectionLengths[17] / 20; ++index)
            this.TevStages[index] = new BLO.MAT1.TevStageProps(er);
          er.BaseStream.Position = position + (long) this.Offsets[20];
          this.AlphaCompares = new BLO.MAT1.AlphaCompare[sectionLengths[20] / 8];
          for (int index = 0; index < sectionLengths[20] / 8; ++index)
            this.AlphaCompares[index] = new BLO.MAT1.AlphaCompare(er);
          er.BaseStream.Position = position + (long) this.Offsets[21];
          this.BlendFunctions = new BLO.MAT1.BlendFunction[sectionLengths[21] / 4];
          for (int index = 0; index < sectionLengths[21] / 4; ++index)
            this.BlendFunctions[index] = new BLO.MAT1.BlendFunction(er);
          er.BaseStream.Position = position + (long) this.SectionSize;
          OK = true;
        }
      }

      public int[] GetSectionLengths()
      {
        int[] numArray = new int[23];
        for (int index1 = 0; index1 < 23; ++index1)
        {
          int num1 = 0;
          if (this.Offsets[index1] != 0U)
          {
            int num2 = (int) this.SectionSize;
            for (int index2 = index1 + 1; index2 < 23; ++index2)
            {
              if (this.Offsets[index2] != 0U)
              {
                num2 = (int) this.Offsets[index2];
                break;
              }
            }
            num1 = num2 - (int) this.Offsets[index1];
          }
          numArray[index1] = num1;
        }
        return numArray;
      }

      public class MaterialEntry
      {
        public BLOShader Shader = (BLOShader) null;
        public byte[] Unknown1;
        public ushort Unknown2;
        public ushort MatColorID;
        public ushort Unknown3;
        public ushort Unknown4;
        public ushort Unknown5;
        public ushort Unknown6;
        public short[] UnknownIndices1;
        public short[] TexMtxIndices;
        public short[] TextureIndices;
        public ushort Unknown8;
        public ushort[] Color3;
        public byte[] ConstColorSel;
        public byte[] ConstAlphaSel;
        public short[] TevOrderInfo;
        public ushort[] ColorS10;
        public short[] UnknownIndices2;
        public short[] UnknownIndices3;
        public byte[] Unknown9;
        public ushort BlendModeIdx;
        public ushort Unknown10;

        public MaterialEntry(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadBytes(8);
          this.MatColorID = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadUInt16();
          this.Unknown4 = er.ReadUInt16();
          this.Unknown5 = er.ReadUInt16();
          this.Unknown6 = er.ReadUInt16();
          this.UnknownIndices1 = er.ReadInt16s(8);
          this.TexMtxIndices = er.ReadInt16s(10);
          this.TextureIndices = er.ReadInt16s(8);
          this.Unknown8 = er.ReadUInt16();
          this.Color3 = er.ReadUInt16s(4);
          this.ConstColorSel = er.ReadBytes(16);
          this.ConstAlphaSel = er.ReadBytes(16);
          this.TevOrderInfo = er.ReadInt16s(16);
          this.ColorS10 = er.ReadUInt16s(4);
          this.UnknownIndices2 = er.ReadInt16s(16);
          this.UnknownIndices3 = er.ReadInt16s(16);
          this.Unknown9 = er.ReadBytes(10);
          this.BlendModeIdx = er.ReadUInt16();
          this.Unknown10 = er.ReadUInt16();
        }
      }

      public class MaterialNames
      {
        public ushort NrEntries;
        public ushort Unknown;
        public BLO.MAT1.MaterialNames.MaterialOffsetEntry[] OffsetEntries;
        public string[] Names;

        public MaterialNames(EndianBinaryReader er)
        {
          long position = er.BaseStream.Position;
          this.NrEntries = er.ReadUInt16();
          this.Unknown = er.ReadUInt16();
          this.OffsetEntries = new BLO.MAT1.MaterialNames.MaterialOffsetEntry[(int) this.NrEntries];
          for (int index = 0; index < (int) this.NrEntries; ++index)
            this.OffsetEntries[index] = new BLO.MAT1.MaterialNames.MaterialOffsetEntry(er);
          this.Names = new string[(int) this.NrEntries];
          for (int index = 0; index < (int) this.NrEntries; ++index)
          {
            er.BaseStream.Position = position + (long) this.OffsetEntries[index].Offset;
            this.Names[index] = er.ReadStringNT(Encoding.ASCII);
          }
          while (er.BaseStream.Position % 4L != 0L)
          {
            int num = (int) er.ReadByte();
          }
        }

        public class MaterialOffsetEntry
        {
          public ushort Unknown;
          public ushort Offset;

          public MaterialOffsetEntry(EndianBinaryReader er)
          {
            this.Unknown = er.ReadUInt16();
            this.Offset = er.ReadUInt16();
          }
        }
      }

      public class TexMatrix
      {
        public ushort Unknown1;
        public ushort Unknown2;
        public Vector3 Translation;
        public Vector2 Scale;
        public float Rotation;
        public Vector2 Unknown3;

        public TexMatrix(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Translation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
          this.Scale = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Rotation = er.ReadSingle();
          this.Unknown3 = new Vector2(er.ReadSingle(), er.ReadSingle());
        }
      }

      public class TevOrder
      {
        public byte TexcoordID;
        public byte TexMap;
        public byte ChannelID;
        public byte Unknown;

        public TevOrder(EndianBinaryReader er)
        {
          this.TexcoordID = er.ReadByte();
          this.TexMap = er.ReadByte();
          this.ChannelID = er.ReadByte();
          this.Unknown = er.ReadByte();
        }
      }

      public class TevStageProps
      {
        public byte color_a;
        public byte color_b;
        public byte color_c;
        public byte color_d;
        public byte alpha_a;
        public byte alpha_b;
        public byte alpha_c;
        public byte alpha_d;
        public byte color_op;
        public byte alpha_op;
        public byte color_regid;
        public byte alpha_regid;
        public byte pad;
        public byte texcoord;
        public byte texmap;
        public byte color_constant_sel;
        public byte alpha_constant_sel;
        public byte color_bias;
        public byte color_scale;
        public byte alpha_bias;
        public byte alpha_scale;
        public bool color_clamp;
        public bool alpha_clamp;

        public TevStageProps(EndianBinaryReader er)
        {
          er.ReadByte();
          this.color_a = er.ReadByte();
          this.color_b = er.ReadByte();
          this.color_c = er.ReadByte();
          this.color_d = er.ReadByte();
          this.color_op = er.ReadByte();
          this.color_bias = er.ReadByte();
          this.color_scale = er.ReadByte();
          this.color_clamp = er.ReadByte() == (byte) 1;
          this.color_regid = er.ReadByte();
          this.alpha_a = er.ReadByte();
          this.alpha_b = er.ReadByte();
          this.alpha_c = er.ReadByte();
          this.alpha_d = er.ReadByte();
          this.alpha_op = er.ReadByte();
          this.alpha_bias = er.ReadByte();
          this.alpha_scale = er.ReadByte();
          this.alpha_clamp = er.ReadByte() == (byte) 1;
          this.alpha_regid = er.ReadByte();
          er.ReadByte();
        }
      }

      public class AlphaCompare
      {
        public byte Func0;
        public byte Ref0;
        public byte MergeFunc;
        public byte Func1;
        public byte Ref1;

        public AlphaCompare(EndianBinaryReader er)
        {
          this.Func0 = er.ReadByte();
          this.Ref0 = er.ReadByte();
          this.MergeFunc = er.ReadByte();
          this.Func1 = er.ReadByte();
          this.Ref1 = er.ReadByte();
          er.ReadBytes(3);
        }
      }

      public class BlendFunction
      {
        public byte BlendMode;
        public byte SrcFactor;
        public byte DstFactor;
        public byte LogicOp;

        public BlendFunction(EndianBinaryReader er)
        {
          this.BlendMode = er.ReadByte();
          this.SrcFactor = er.ReadByte();
          this.DstFactor = er.ReadByte();
          this.LogicOp = er.ReadByte();
        }

        public void ApplyBlendMode()
        {
          if (this.BlendMode == (byte) 0)
          {
            Gl.glDisable(3042);
          }
          else
          {
            Gl.glEnable(3042);
            Gl.glBlendEquation(this.GetGlBlendMode((int) this.BlendMode));
            Gl.glBlendFunc(this.GetGlBlendFactor((int) this.SrcFactor), this.GetGlBlendFactor((int) this.DstFactor));
            Gl.glLogicOp(this.GetGlLogicOp((int) this.LogicOp));
          }
        }

        private int GetGlBlendFactor(int factor)
        {
          return new int[8]
          {
            0,
            1,
            768,
            769,
            770,
            771,
            772,
            773
          }[factor];
        }

        private int GetGlBlendMode(int type)
        {
          return new int[4]{ 0, 32774, 32779, 32778 }[type];
        }

        private int GetGlLogicOp(int op)
        {
          return new int[16]
          {
            5376,
            5377,
            5378,
            5379,
            5380,
            5381,
            5382,
            5383,
            5384,
            5385,
            5386,
            5387,
            5388,
            5389,
            5390,
            5391
          }[op];
        }
      }
    }

    public class BLOLayoutElement
    {
      public List<BLO.BLOLayoutElement> Children = new List<BLO.BLOLayoutElement>();
      public BLO.BLOLayoutElement Parent;

      public virtual void Render(BLO Layout)
      {
      }

      protected void DrawQuad(
        float X,
        float Y,
        float Width,
        float Height,
        BLO.MAT1.MaterialEntry mat,
        Vector2 TLTexCoord,
        Vector2 TRTexCoord,
        Vector2 BLTexCoord,
        Vector2 BRTexCoord,
        System.Drawing.Color TLColor,
        System.Drawing.Color TRColor,
        System.Drawing.Color BLColor,
        System.Drawing.Color BRColor)
      {
        float[,] numArray1 = new float[4, 2]
        {
          {
            X,
            0.0f
          },
          {
            X + Width,
            0.0f
          },
          {
            X + Width,
            0.0f
          },
          {
            X,
            0.0f
          }
        };
        numArray1[0, 1] = Y;
        numArray1[1, 1] = Y;
        numArray1[2, 1] = Y + Height;
        numArray1[3, 1] = Y + Height;
        float[] numArray2 = new float[4]
        {
          (float) TLColor.R / (float) byte.MaxValue,
          (float) TLColor.G / (float) byte.MaxValue,
          (float) TLColor.B / (float) byte.MaxValue,
          (float) TLColor.A / (float) byte.MaxValue
        };
        float[] numArray3 = new float[4]
        {
          (float) TRColor.R / (float) byte.MaxValue,
          (float) TRColor.G / (float) byte.MaxValue,
          (float) TRColor.B / (float) byte.MaxValue,
          (float) TRColor.A / (float) byte.MaxValue
        };
        float[] numArray4 = new float[4]
        {
          (float) BRColor.R / (float) byte.MaxValue,
          (float) BRColor.G / (float) byte.MaxValue,
          (float) BRColor.B / (float) byte.MaxValue,
          (float) BRColor.A / (float) byte.MaxValue
        };
        float[] numArray5 = new float[4]
        {
          (float) BLColor.R / (float) byte.MaxValue,
          (float) BLColor.G / (float) byte.MaxValue,
          (float) BLColor.B / (float) byte.MaxValue,
          (float) BLColor.A / (float) byte.MaxValue
        };
        Gl.glBegin(4);
        for (int index = 0; index < mat.TextureIndices.Length; ++index)
        {
          if (mat.TextureIndices[index] != (short) -1)
            Gl.glMultiTexCoord2f(33984 + index, TLTexCoord.X, TLTexCoord.Y);
        }
        if (mat.TextureIndices[0] == (short) -1)
          Gl.glMultiTexCoord2f(33984, TLTexCoord.X, TLTexCoord.Y);
        Gl.glColor4f(numArray2[0], numArray2[1], numArray2[2], numArray2[3]);
        Gl.glVertex3f(numArray1[0, 0], numArray1[0, 1], 0.0f);
        for (int index = 0; index < mat.TextureIndices.Length; ++index)
        {
          if (mat.TextureIndices[index] != (short) -1)
            Gl.glMultiTexCoord2f(33984 + index, TRTexCoord.X, TRTexCoord.Y);
        }
        if (mat.TextureIndices[0] == (short) -1)
          Gl.glMultiTexCoord2f(33984, TRTexCoord.X, TRTexCoord.Y);
        Gl.glColor4f(numArray3[0], numArray3[1], numArray3[2], numArray3[3]);
        Gl.glVertex3f(numArray1[1, 0], numArray1[1, 1], 0.0f);
        for (int index = 0; index < mat.TextureIndices.Length; ++index)
          Gl.glMultiTexCoord2f(33984 + index, BRTexCoord.X, BRTexCoord.Y);
        if (mat.TextureIndices[0] == (short) -1)
          Gl.glMultiTexCoord2f(33984, BRTexCoord.X, BRTexCoord.Y);
        Gl.glColor4f(numArray4[0], numArray4[1], numArray4[2], numArray4[3]);
        Gl.glVertex3f(numArray1[2, 0], numArray1[2, 1], 0.0f);
        for (int index = 0; index < mat.TextureIndices.Length; ++index)
        {
          if (mat.TextureIndices[index] != (short) -1)
            Gl.glMultiTexCoord2f(33984 + index, TLTexCoord.X, TLTexCoord.Y);
        }
        if (mat.TextureIndices[0] == (short) -1)
          Gl.glMultiTexCoord2f(33984, TLTexCoord.X, TLTexCoord.Y);
        Gl.glColor4f(numArray2[0], numArray2[1], numArray2[2], numArray2[3]);
        Gl.glVertex3f(numArray1[0, 0], numArray1[0, 1], 0.0f);
        for (int index = 0; index < mat.TextureIndices.Length; ++index)
          Gl.glMultiTexCoord2f(33984 + index, BRTexCoord.X, BRTexCoord.Y);
        if (mat.TextureIndices[0] == (short) -1)
          Gl.glMultiTexCoord2f(33984, BRTexCoord.X, BRTexCoord.Y);
        Gl.glColor4f(numArray4[0], numArray4[1], numArray4[2], numArray4[3]);
        Gl.glVertex3f(numArray1[2, 0], numArray1[2, 1], 0.0f);
        for (int index = 0; index < mat.TextureIndices.Length; ++index)
        {
          if (mat.TextureIndices[index] != (short) -1)
            Gl.glMultiTexCoord2f(33984 + index, BLTexCoord.X, BLTexCoord.Y);
        }
        if (mat.TextureIndices[0] == (short) -1)
          Gl.glMultiTexCoord2f(33984, BLTexCoord.X, BLTexCoord.Y);
        Gl.glColor4f(numArray5[0], numArray5[1], numArray5[2], numArray5[3]);
        Gl.glVertex3f(numArray1[3, 0], numArray1[3, 1], 0.0f);
        Gl.glEnd();
      }
    }

    public class PAN2 : BLO.BLOLayoutElement
    {
      public string Signature;
      public uint SectionSize;
      public ushort Unknown1;
      public ushort Unknown2;
      public ushort Unknown3;
      public ushort Reserved;
      public string Name;
      public Vector2 Size;
      public Vector2 Scale;
      public Vector3 Rotation;
      public Vector2 Position;
      public uint Unknown4;

      protected PAN2()
      {
      }

      public PAN2(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (PAN2))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadUInt16();
          this.Reserved = er.ReadUInt16();
          this.Name = er.ReadString(Encoding.ASCII, 16);
          this.Size = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Scale = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Rotation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
          this.Position = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Unknown4 = er.ReadUInt32();
          OK = true;
        }
      }

      public override void Render(BLO Layout)
      {
        Gl.glPushMatrix();
        Gl.glTranslatef(this.Position.X, this.Position.Y, 0.0f);
        Gl.glRotatef(this.Rotation.X, 1f, 0.0f, 0.0f);
        Gl.glRotatef(this.Rotation.Y, 0.0f, 1f, 0.0f);
        Gl.glRotatef(this.Rotation.Z, 0.0f, 0.0f, 1f);
        Gl.glScalef(this.Scale.X, this.Scale.Y, 1f);
        foreach (BLO.BLOLayoutElement child in this.Children)
          child.Render(Layout);
        Gl.glPopMatrix();
      }

      public override string ToString()
      {
        return this.Name.Replace("\0", "");
      }
    }

    public class pan2 : BLO.PAN2
    {
      public pan2(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (pan2))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadUInt16();
          this.Reserved = er.ReadUInt16();
          this.Name = er.ReadString(Encoding.ASCII, 16);
          this.Size = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Scale = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Rotation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
          this.Position = new Vector2(er.ReadSingle(), er.ReadSingle());
          this.Unknown4 = er.ReadUInt32();
          OK = true;
        }
      }
    }

    public class PIC2 : BLO.BLOLayoutElement
    {
      public string Signature;
      public uint SectionSize;
      public BLO.pan2 BasePane;
      public ushort Unknown1;
      public ushort Unknown2;
      public ushort MaterialID;
      public ushort Reserved;
      public ushort Unknown3;
      public ushort Unknown4;
      public ushort Unknown5;
      public ushort Unknown6;
      public Vector2 TLTexCoord;
      public Vector2 TRTexCoord;
      public Vector2 BLTexCoord;
      public Vector2 BRTexCoord;
      public System.Drawing.Color TLColor;
      public System.Drawing.Color TRColor;
      public System.Drawing.Color BLColor;
      public System.Drawing.Color BRColor;

      public PIC2(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (PIC2))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          bool OK1;
          this.BasePane = new BLO.pan2(er, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Unknown1 = er.ReadUInt16();
            this.Unknown2 = er.ReadUInt16();
            this.MaterialID = er.ReadUInt16();
            this.Reserved = er.ReadUInt16();
            this.Unknown3 = er.ReadUInt16();
            this.Unknown4 = er.ReadUInt16();
            this.Unknown5 = er.ReadUInt16();
            this.Unknown6 = er.ReadUInt16();
            this.TLTexCoord = new Vector2((float) er.ReadUInt16() / 256f, (float) er.ReadUInt16() / 256f);
            this.TRTexCoord = new Vector2((float) er.ReadUInt16() / 256f, (float) er.ReadUInt16() / 256f);
            this.BLTexCoord = new Vector2((float) er.ReadUInt16() / 256f, (float) er.ReadUInt16() / 256f);
            this.BRTexCoord = new Vector2((float) er.ReadUInt16() / 256f, (float) er.ReadUInt16() / 256f);
            this.TLColor = er.ReadColor8();
            this.TRColor = er.ReadColor8();
            this.BLColor = er.ReadColor8();
            this.BRColor = er.ReadColor8();
            OK = true;
          }
        }
      }

      public override void Render(BLO Layout)
      {
        Gl.glPushMatrix();
        BLO.MAT1.MaterialEntry materialEntry = Layout.Mat1.MaterialEntries[(int) Layout.Mat1.MaterialEntryIndieces[(int) this.MaterialID]];
        Layout.Mat1.BlendFunctions[(int) materialEntry.BlendModeIdx].ApplyBlendMode();
        Gl.glEnable(3008);
        Gl.glAlphaFunc(519, 0.0f);
        List<int> intList = new List<int>();
        for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
        {
          if (materialEntry.TextureIndices[index] != (short) -1)
          {
            Gl.glActiveTexture(33984 + index);
            Gl.glBindTexture(3553, (int) this.MaterialID * 8 + index + 1);
            Gl.glEnable(3553);
            intList.Add((int) this.MaterialID * 8 + index + 1);
          }
        }
        if (materialEntry.TextureIndices[0] == (short) -1)
        {
          Gl.glActiveTexture(33984);
          Gl.glColor4f(1f, 1f, 1f, 1f);
          Gl.glBindTexture(3553, (int) this.MaterialID * 8 + 1);
          Gl.glEnable(3553);
          intList.Add((int) this.MaterialID * 8 + 1);
        }
        Gl.glMatrixMode(5890);
        for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
        {
          Gl.glActiveTexture(33984 + index);
          Gl.glLoadIdentity();
          if (materialEntry.TextureIndices[index] != (short) -1)
          {
            BLO.MAT1.TexMatrix texmatrice = Layout.Mat1.Texmatrices[(int) materialEntry.TexMtxIndices[index]];
            Gl.glTranslatef(texmatrice.Translation.X, texmatrice.Translation.Y, 0.0f);
            Gl.glScalef(texmatrice.Scale.X, texmatrice.Scale.Y, 1f);
            Gl.glRotatef(texmatrice.Rotation, 0.0f, 0.0f, 1f);
            Gl.glTranslatef(texmatrice.Unknown3.X, texmatrice.Unknown3.Y, 0.0f);
            Gl.glTranslatef(texmatrice.Unknown3.X / texmatrice.Scale.X - texmatrice.Translation.X, texmatrice.Unknown3.Y / texmatrice.Scale.Y - texmatrice.Translation.Y, 0.0f);
          }
        }
        if (materialEntry.TextureIndices[0] == (short) -1)
        {
          Gl.glActiveTexture(33984);
          Gl.glLoadIdentity();
        }
        Gl.glMatrixMode(5888);
        if (materialEntry.Shader == null)
        {
          materialEntry.Shader = new BLOShader(materialEntry, Layout.Mat1, intList.ToArray());
          materialEntry.Shader.Compile();
        }
        materialEntry.Shader.Enable();
        Gl.glTranslatef(this.BasePane.Position.X, this.BasePane.Position.Y, 0.0f);
        Gl.glRotatef(this.BasePane.Rotation.X, 1f, 0.0f, 0.0f);
        Gl.glRotatef(this.BasePane.Rotation.Y, 0.0f, 1f, 0.0f);
        Gl.glRotatef(this.BasePane.Rotation.Z, 0.0f, 0.0f, 1f);
        Gl.glScalef(this.BasePane.Scale.X, this.BasePane.Scale.Y, 1f);
        Gl.glPushMatrix();
        Gl.glTranslatef(-0.5f * this.BasePane.Size.X * (float) (((int) this.BasePane.Unknown3 & (int) byte.MaxValue) % 3), (float) (0.5 * (double) this.BasePane.Size.Y * -(double) (((int) this.BasePane.Unknown3 & (int) byte.MaxValue) / 3)), 0.0f);
        float[] numArray1 = new float[16];
        Gl.glGetFloatv(2982, numArray1);
        MTX44 mtX44 = new MTX44();
        mtX44.SetValues(numArray1);
        float[] numArray2 = mtX44.MultVector(new float[3]);
        float[] numArray3 = mtX44.MultVector(new float[3]
        {
          this.BasePane.Size.X,
          this.BasePane.Size.Y,
          0.0f
        });
        numArray3[0] -= numArray2[0];
        numArray3[1] -= numArray2[1];
        Console.WriteLine(this.BasePane.ToString() + " - " + (object) numArray2[0] + "; " + (object) numArray2[1] + "; - " + (object) numArray3[0] + "; " + (object) numArray3[1] + ";");
        this.DrawQuad(0.0f, 0.0f, this.BasePane.Size.X, this.BasePane.Size.Y, materialEntry, this.TLTexCoord, this.TRTexCoord, this.BLTexCoord, this.BRTexCoord, this.TLColor, this.TRColor, this.BLColor, this.BRColor);
        Gl.glPopMatrix();
        foreach (BLO.BLOLayoutElement child in this.Children)
          child.Render(Layout);
        Gl.glPopMatrix();
      }

      public override string ToString()
      {
        return this.BasePane.Name.Replace("\0", "");
      }

      public enum XOrigin
      {
        Left,
        Center,
        Right,
      }

      public enum YOrigin
      {
        Top,
        Center,
        Bottom,
      }
    }

    public class WIN2 : BLO.BLOLayoutElement
    {
      public string Signature;
      public uint SectionSize;
      public BLO.pan2 BasePane;
      public ushort Unknown1;
      public byte[] Reserved1;
      public System.Drawing.Color UnknownColor1;
      public System.Drawing.Color UnknownColor2;
      public ushort TLMaterialID;
      public ushort TRMaterialID;
      public ushort BLMaterialID;
      public ushort BRMaterialID;
      public byte Unknown2;
      public byte Reserved2;
      public ushort Unknown3;
      public ushort Unknown4;
      public ushort Unknown5;
      public ushort Unknown6;
      public ushort Unknown7;
      public ushort Unknown8;
      public ushort Reserved3;
      public ushort Unknown9;
      public ushort Unknown10;
      public ushort Unknown11;
      public ushort Unknown12;
      public System.Drawing.Color TLColor;
      public System.Drawing.Color TRColor;
      public System.Drawing.Color BLColor;
      public System.Drawing.Color BRColor;

      public WIN2(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (WIN2))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          bool OK1;
          this.BasePane = new BLO.pan2(er, out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Unknown1 = er.ReadUInt16();
            this.Reserved1 = er.ReadBytes(6);
            this.UnknownColor1 = er.ReadColor8();
            this.UnknownColor2 = er.ReadColor8();
            this.TLMaterialID = er.ReadUInt16();
            this.TRMaterialID = er.ReadUInt16();
            this.BLMaterialID = er.ReadUInt16();
            this.BRMaterialID = er.ReadUInt16();
            this.Unknown2 = er.ReadByte();
            this.Reserved2 = er.ReadByte();
            this.Unknown3 = er.ReadUInt16();
            this.Unknown4 = er.ReadUInt16();
            this.Unknown5 = er.ReadUInt16();
            this.Unknown6 = er.ReadUInt16();
            this.Unknown7 = er.ReadUInt16();
            this.Unknown8 = er.ReadUInt16();
            this.Reserved3 = er.ReadUInt16();
            this.Unknown9 = er.ReadUInt16();
            this.Unknown10 = er.ReadUInt16();
            this.Unknown11 = er.ReadUInt16();
            this.Unknown12 = er.ReadUInt16();
            this.TLColor = er.ReadColor8();
            this.TRColor = er.ReadColor8();
            this.BLColor = er.ReadColor8();
            this.BRColor = er.ReadColor8();
            OK = true;
          }
        }
      }

      public override void Render(BLO Layout)
      {
        Gl.glPushMatrix();
        BLO.MAT1.MaterialEntry materialEntry = Layout.Mat1.MaterialEntries[(int) Layout.Mat1.MaterialEntryIndieces[(int) this.TLMaterialID]];
        Layout.Mat1.BlendFunctions[(int) materialEntry.BlendModeIdx].ApplyBlendMode();
        Gl.glEnable(3008);
        Gl.glAlphaFunc(519, 0.0f);
        List<int> intList = new List<int>();
        for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
        {
          if (materialEntry.TextureIndices[index] != (short) -1)
          {
            Gl.glActiveTexture(33984 + index);
            Gl.glBindTexture(3553, (int) this.TLMaterialID * 8 + index + 1);
            Gl.glEnable(3553);
            intList.Add((int) this.TLMaterialID * 8 + index + 1);
          }
        }
        if (materialEntry.TextureIndices[0] == (short) -1)
        {
          Gl.glActiveTexture(33984);
          Gl.glColor4f(1f, 1f, 1f, 1f);
          Gl.glBindTexture(3553, (int) this.TLMaterialID * 8 + 1);
          Gl.glEnable(3553);
          intList.Add((int) this.TLMaterialID * 8 + 1);
        }
        Gl.glMatrixMode(5890);
        for (int index = 0; index < materialEntry.TextureIndices.Length; ++index)
        {
          if (materialEntry.TextureIndices[index] != (short) -1)
          {
            Gl.glActiveTexture(33984 + index);
            Gl.glLoadIdentity();
          }
        }
        if (materialEntry.TextureIndices[0] == (short) -1)
        {
          Gl.glActiveTexture(33984);
          Gl.glLoadIdentity();
        }
        Gl.glMatrixMode(5888);
        if (materialEntry.Shader == null)
        {
          materialEntry.Shader = new BLOShader(materialEntry, Layout.Mat1, intList.ToArray());
          materialEntry.Shader.Compile();
        }
        materialEntry.Shader.Enable();
        Gl.glTranslatef(this.BasePane.Position.X, this.BasePane.Position.Y, 0.0f);
        Gl.glRotatef(this.BasePane.Rotation.X, 1f, 0.0f, 0.0f);
        Gl.glRotatef(this.BasePane.Rotation.Y, 0.0f, 1f, 0.0f);
        Gl.glRotatef(this.BasePane.Rotation.Z, 0.0f, 0.0f, 1f);
        Gl.glScalef(this.BasePane.Scale.X, this.BasePane.Scale.Y, 1f);
        Gl.glPushMatrix();
        Gl.glTranslatef(-0.5f * this.BasePane.Size.X * (float) (((int) this.BasePane.Unknown3 & (int) byte.MaxValue) % 3), (float) (0.5 * (double) this.BasePane.Size.Y * -(double) (((int) this.BasePane.Unknown3 & (int) byte.MaxValue) / 3)), 0.0f);
        float x = this.BasePane.Size.X / 2f / (float) Layout.Tex1.Textures[(int) Layout.Mat1.TextureIndieces[(int) materialEntry.TextureIndices[0]]].Header.Width;
        float y = this.BasePane.Size.Y / 2f / (float) Layout.Tex1.Textures[(int) Layout.Mat1.TextureIndieces[(int) materialEntry.TextureIndices[0]]].Header.Height;
        this.DrawQuad(0.0f, 0.0f, this.BasePane.Size.X / 2f, this.BasePane.Size.Y / 2f, materialEntry, new Vector2(0.0f, 0.0f), new Vector2(x, 0.0f), new Vector2(0.0f, y), new Vector2(x, y), this.TLColor, this.TRColor, this.BLColor, this.BRColor);
        this.DrawQuad(this.BasePane.Size.X / 2f, 0.0f, this.BasePane.Size.X / 2f, this.BasePane.Size.Y / 2f, materialEntry, new Vector2(x, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(x, y), new Vector2(0.0f, y), this.TRColor, this.TLColor, this.BRColor, this.BLColor);
        this.DrawQuad(this.BasePane.Size.X / 2f, this.BasePane.Size.Y / 2f, this.BasePane.Size.X / 2f, this.BasePane.Size.Y / 2f, materialEntry, new Vector2(x, y), new Vector2(0.0f, y), new Vector2(x, 0.0f), new Vector2(0.0f, 0.0f), this.BRColor, this.BLColor, this.TRColor, this.TLColor);
        this.DrawQuad(0.0f, this.BasePane.Size.Y / 2f, this.BasePane.Size.X / 2f, this.BasePane.Size.Y / 2f, materialEntry, new Vector2(0.0f, y), new Vector2(x, y), new Vector2(0.0f, 0.0f), new Vector2(x, 0.0f), this.BLColor, this.BRColor, this.TLColor, this.TRColor);
        Gl.glPopMatrix();
        foreach (BLO.BLOLayoutElement child in this.Children)
          child.Render(Layout);
        Gl.glPopMatrix();
      }

      public override string ToString()
      {
        return this.BasePane.Name.Replace("\0", "");
      }
    }

    public class EXT1
    {
      public string Signature;
      public uint SectionSize;

      public EXT1(EndianBinaryReader er, out bool OK)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (EXT1))
        {
          OK = false;
        }
        else
        {
          this.SectionSize = er.ReadUInt32();
          OK = true;
        }
      }
    }
  }
}
