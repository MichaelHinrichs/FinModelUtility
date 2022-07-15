// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Chadsoft.CTools.Image;
using bmd._3D_Formats;
//using MKDS_Course_Modifier.Converters._3D;
//using MKDS_Course_Modifier.Converters.Colission;
using bmd.G3D_Binary_File_Format;
//using MKDS_Course_Modifier.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

using bmd.schema.bmd;
using bmd.schema.bmd.inf1;
using bmd.schema.bmd.jnt1;
using bmd.schema.bmd.mat3;
using bmd.schema.bmd.shp1;
using bmd.schema.bmd.vtx1;

using fin.model;
using fin.model.impl;
using fin.util.asserts;
using fin.util.color;
using fin.util.image;

#pragma warning disable CS8604

namespace bmd.GCN {
  public partial class BMD {
    public BmdHeader Header;
    public BMD.INF1Section INF1;
    public BMD.VTX1Section VTX1;
    public BMD.EVP1Section EVP1;
    public BMD.DRW1Section DRW1;
    public BMD.JNT1Section JNT1;
    public BMD.SHP1Section SHP1;
    public BMD.MAT3Section MAT3;
    public BMD.TEX1Section TEX1;

    public BMD(byte[] file)
    {
      using EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.BigEndian);
      this.Header = er.ReadNew<BmdHeader>();

      bool OK;
      while (er.Position != er.Length)
      {
        switch (er.ReadString(Encoding.ASCII, 4))
        {
          case nameof (INF1):
            er.Position -= 4L;
            this.INF1 = new BMD.INF1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 2");
              return;
            }
            else
              break;
          case nameof (VTX1):
            er.Position -= 4L;
            this.VTX1 = new BMD.VTX1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 4");
              return;
            } else
              break;
          case nameof (EVP1):
            er.Position -= 4L;
            this.EVP1 = new BMD.EVP1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 4");
              return;
            } else
              break;
          case nameof (DRW1):
            er.Position -= 4L;
            this.DRW1 = new BMD.DRW1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 5");
              return;
            } else
              break;
          case nameof (JNT1):
            er.Position -= 4L;
            this.JNT1 = new BMD.JNT1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 6");
              return;
            } else
              break;
          case nameof (SHP1):
            er.Position -= 4L;
            this.SHP1 = new BMD.SHP1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 7");
              return;
            } else
              break;
          case "MAT1":
          case "MAT2":
          case nameof (MAT3):
            er.Position -= 4L;
            this.MAT3 = new BMD.MAT3Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 8");
              return;
            } else
              break;
          case nameof (TEX1):
            er.Position -= 4L;
            this.TEX1 = new BMD.TEX1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 9");
              return;
            } else
              break;
          default:
            return;
        }
      }
    }

    public MA.Node[] GetJoints()
    {
      Stack<BMD.Node> nodeStack = new Stack<BMD.Node>();
      nodeStack.Push((BMD.Node) null);
      List<MA.Node> nodeList = new List<MA.Node>();
      BMD.Node node = (BMD.Node) null;
      foreach (Inf1Entry entry in this.INF1.Entries)
      {
        switch (entry.Type)
        {
          case 0:
            goto label_7;
          case 1:
            nodeStack.Push(node);
            break;
          case 2:
            nodeStack.Pop();
            break;
          case 16:
            nodeList.Add(new MA.Node(this.JNT1.Joints[(int) entry.Index].Tx, this.JNT1.Joints[(int) entry.Index].Ty, this.JNT1.Joints[(int) entry.Index].Tz, (float) ((double) this.JNT1.Joints[(int) entry.Index].Rx / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Ry / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Rz / 32768.0 * 180.0), this.JNT1.Joints[(int) entry.Index].Sx, this.JNT1.Joints[(int) entry.Index].Sy, this.JNT1.Joints[(int) entry.Index].Sz, this.JNT1.StringTable[(int) entry.Index], nodeStack.Peek() == null ? (string) null : nodeStack.Peek().Name));
            node = new BMD.Node(this.JNT1.StringTable[(int) entry.Index], nodeStack.Peek() == null ? (BMD.Node) null : nodeStack.Peek());
            break;
        }
      }
label_7:
      return nodeList.ToArray();
    }

    public class INF1Section
    {
      public const string Signature = "INF1";
      public DataBlockHeader Header;
      public ushort Unknown1;
      public ushort Padding;
      public uint Unknown2;
      public uint NrVertex;
      public uint EntryOffset;
      public Inf1Entry[] Entries;

      public INF1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "INF1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt32();
          this.NrVertex = er.ReadUInt32();
          this.EntryOffset = er.ReadUInt32();
          long position2 = er.Position;
          er.Position = position1 + (long) this.EntryOffset;
          List<Inf1Entry> source = new List<Inf1Entry>();
          source.Add(er.ReadNew<Inf1Entry>());
          while (source.Last<Inf1Entry>().Type != (ushort) 0)
            source.Add(er.ReadNew<Inf1Entry>());
          er.Position = position1 + (long) this.Header.size;
          this.Entries = source.ToArray();
          OK = true;
        }
      }
    }

    public partial class VTX1Section {
      public BMD.VTX1Section.Color[][] Colors = new BMD.VTX1Section.Color[2][];
      public BMD.VTX1Section.Texcoord[][] Texcoords = new BMD.VTX1Section.Texcoord[8][];
      public const string Signature = "VTX1";
      public DataBlockHeader Header;
      public uint ArrayFormatOffset;
      public uint[] Offsets;
      public ArrayFormat[] ArrayFormats;
      public Vector3[] Positions;
      public Vector3[] Normals;

      public VTX1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "VTX1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.ArrayFormatOffset = er.ReadUInt32();
          this.Offsets = er.ReadUInt32s(13);
          long position2 = er.Position;
          int length1 = 0;
          foreach (uint offset in this.Offsets)
          {
            if (offset != 0U)
              ++length1;
          }

          er.Position = position1 + (long) this.ArrayFormatOffset;
          er.ReadNewArray(out this.ArrayFormats, length1);

          int index1 = 0;
          for (int k = 0; k < 13; ++k)
          {
            if (this.Offsets[k] != 0U)
            {
              ArrayFormat arrayFormat = this.ArrayFormats[index1];
              int length2 = this.GetLength(k);
              er.Position = position1 + (long) this.Offsets[k];
              if (arrayFormat.ArrayType >= 11 && arrayFormat.ArrayType <= 12) {
                this.ReadColorArray(arrayFormat, length2, er);
              } else {
                this.ReadVertexArray(arrayFormat, length2, er);
              }
              ++index1;
            }
          }
          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      private int GetLength(int k)
      {
        int offset = (int) this.Offsets[k];
        for (int index = k + 1; index < 13; ++index)
        {
          if (this.Offsets[index] != 0U)
            return (int) this.Offsets[index] - offset;
        }
        return (int) this.Header.size - offset;
      }

      private void ReadVertexArray(
          ArrayFormat Format,
          int Length,
          EndianBinaryReader er) {
        List<float> floatList = new List<float>();
        switch (Format.DataType) {
          case 3:
            float num1 = (float) Math.Pow(0.5, (double) Format.DecimalPoint);
            for (int index = 0; index < Length / 2; ++index)
              floatList.Add((float) er.ReadInt16() * num1);
            break;
          case 4:
            floatList.AddRange((IEnumerable<float>) er.ReadSingles(Length / 4));
            break;
          default:
            throw new NotImplementedException();
        }
        switch (Format.ArrayType) {
          case 9:
            switch (Format.ComponentCount) {
              case 0:
                this.Positions = new Vector3[floatList.Count / 2];
                for (int index = 0; index < floatList.Count - 1; index += 2)
                  this.Positions[index / 2] =
                      new Vector3(floatList[index], floatList[index + 1], 0.0f);
                return;
              case 1:
                this.Positions = new Vector3[floatList.Count / 3];
                for (int index = 0; index < floatList.Count - 2; index += 3)
                  this.Positions[index / 3] = new Vector3(
                      floatList[index],
                      floatList[index + 1],
                      floatList[index + 2]);
                return;
              default:
                return;
            }
          case 10:
            if (Format.ComponentCount != 0U)
              break;
            this.Normals = new Vector3[floatList.Count / 3];
            for (int index = 0; index < floatList.Count - 2; index += 3)
              this.Normals[index / 3] = new Vector3(
                  floatList[index],
                  floatList[index + 1],
                  floatList[index + 2]);
            break;
          case 13:
          case 14:
          case 15:
          case 16:
          case 17:
          case 18:
          case 19:
          case 20:
            uint num3 = Format.ArrayType - 13U;
            switch (Format.ComponentCount) {
              case 0:
                this.Texcoords[num3] =
                    new BMD.VTX1Section.Texcoord[floatList.Count];
                for (int index = 0; index < floatList.Count; ++index)
                  this.Texcoords[num3][index] =
                      new BMD.VTX1Section.Texcoord(floatList[index], 0.0f);
                return;
              case 1:
                this.Texcoords[num3] =
                    new BMD.VTX1Section.Texcoord[floatList.Count / 2];
                for (int index = 0; index < floatList.Count - 1; index += 2)
                  this.Texcoords[num3][index / 2] =
                      new BMD.VTX1Section.Texcoord(
                          floatList[index],
                          floatList[index + 1]);
                return;
              default:
                return;
            }
        }
      }

      private enum ColorDataType {
        RGB565 = 0,
        RGB8 = 1,
        RGBX8 = 2,
        RGBA4 = 3,
        RGBA6 = 4,
        RGBA8 = 5,
      }

      /// <summary>
      ///   Colors are a special case:
      ///   https://wiki.cloudmodding.com/tww/BMD_and_BDL#Data_Types
      /// </summary>
      private void ReadColorArray(
          ArrayFormat Format,
          int byteLength,
          EndianBinaryReader er) {
        uint num2 = Format.ArrayType - 11U;

        var colorDataType = (ColorDataType) Format.DataType;
        var expectedComponentCount = colorDataType switch {
            ColorDataType.RGB565 => 3,
            ColorDataType.RGB8   => 3,
            ColorDataType.RGBX8  => 4,
            ColorDataType.RGBA4  => 4,
            ColorDataType.RGBA6  => 4,
            ColorDataType.RGBA8  => 4,
            _                    => throw new ArgumentOutOfRangeException()
        };

        var actualComponentCount = (int) (3 + Format.ComponentCount);
        Asserts.Equal(expectedComponentCount, actualComponentCount);

        var colorCount = colorDataType switch {
            ColorDataType.RGB565 => byteLength / 2,
            ColorDataType.RGB8   => byteLength / 3,
            ColorDataType.RGBX8  => byteLength / 4,
            ColorDataType.RGBA4  => byteLength / 2,
            ColorDataType.RGBA6  => byteLength / 3,
            ColorDataType.RGBA8  => byteLength / 4,
            _                    => throw new ArgumentOutOfRangeException()
        };

        var colors = new Color[colorCount];
        for (var i = 0; i < colorCount; ++i) {
          Color color;
          switch (colorDataType) {
            case ColorDataType.RGB565: {
              ColorUtil.SplitRgb565(er.ReadUInt16(), out var r, out var g, out var b);
              color = new Color(r, g, b, 255);
              break;
            }
            case ColorDataType.RGB8: {
              color = new Color(er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte(),
                                255);
              break;
            }
            case ColorDataType.RGBX8: {
              color = new Color(er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte());
              break;
            }
            case ColorDataType.RGBA4: {
              throw new ArgumentOutOfRangeException();
            }
            case ColorDataType.RGBA6: {
              throw new ArgumentOutOfRangeException();
            }
            case ColorDataType.RGBA8: {
              color = new Color(er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte());
              break;
            }
            default: throw new ArgumentOutOfRangeException();
          }

          Asserts.Nonnull(color);

          colors[i] = color;
        }

        this.Colors[num2] = colors;
      }

      public class Color
      {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
          this.R = r;
          this.G = g;
          this.B = g;
          this.A = b;
        }

        public static implicit operator System.Drawing.Color(BMD.VTX1Section.Color c)
        {
          return System.Drawing.Color.FromArgb((int) c.A, (int) c.R, (int) c.G, (int) c.B);
        }
      }

      public class Texcoord
      {
        public float S;
        public float T;

        public Texcoord(float s, float t)
        {
          this.S = s;
          this.T = t;
        }
      }
    }

    public class EVP1Section
    {
      public const string Signature = "EVP1";
      public DataBlockHeader Header;
      public ushort Count;
      public ushort Padding;
      public uint[] Offsets;
      public byte[] Counts;
      public MTX44[] InverseBindMatrices;
      public BMD.EVP1Section.MultiMatrix[] WeightedIndices;

      public EVP1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "EVP1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Count = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.Offsets = er.ReadUInt32s(4);
          long position2 = er.Position;
          er.Position = position1 + (long) this.Offsets[0];
          this.Counts = er.ReadBytes((int) this.Count);

          er.Position = position1 + (long) this.Offsets[1];
          this.WeightedIndices = new BMD.EVP1Section.MultiMatrix[(int) this.Count];
          int val1 = 0;
          for (int index1 = 0; index1 < (int) this.Count; ++index1)
          {
            this.WeightedIndices[index1] = new BMD.EVP1Section.MultiMatrix();
            this.WeightedIndices[index1].Indices = new ushort[(int) this.Counts[index1]];
            for (int index2 = 0; index2 < (int) this.Counts[index1]; ++index2)
            {
              this.WeightedIndices[index1].Indices[index2] = er.ReadUInt16();
              val1 = Math.Max(val1, (int) this.WeightedIndices[index1].Indices[index2] + 1);
            }
          }
          
          er.Position = position1 + (long) this.Offsets[2];
          for (int index1 = 0; index1 < (int) this.Count; ++index1)
          {
            this.WeightedIndices[index1].Weights = new float[(int) this.Counts[index1]];
            for (int index2 = 0; index2 < (int) this.Counts[index1]; ++index2)
              this.WeightedIndices[index1].Weights[index2] = er.ReadSingle();
          }

          er.Position = position1 + (long) this.Offsets[3];
          this.InverseBindMatrices = new MTX44[val1];
          for (int index = 0; index < val1; ++index)
          {
            float[] array = er.ReadSingles(12);
            Array.Resize<float>(ref array, 16);
            array[15] = 1f;
            this.InverseBindMatrices[index] = (MTX44) array;
          }
          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public class MultiMatrix
      {
        public float[] Weights;
        public ushort[] Indices;
      }
    }

    public class DRW1Section
    {
      public const string Signature = "DRW1";
      public DataBlockHeader Header;
      public ushort Count;
      public ushort Padding;
      public uint IsWeightedOffset;
      public uint DataOffset;
      public bool[] IsWeighted;
      public ushort[] Data;

      public DRW1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "DRW1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Count = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.IsWeightedOffset = er.ReadUInt32();
          this.DataOffset = er.ReadUInt32();

          er.Position = position1 + (long) this.IsWeightedOffset;
          this.IsWeighted = new bool[(int) this.Count];
          for (int index = 0; index < (int) this.Count; ++index)
            this.IsWeighted[index] = er.ReadByte() == (byte) 1;
          
          er.Position = position1 + (long) this.DataOffset;
          this.Data = er.ReadUInt16s((int) this.Count);

          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }
    }

    public partial class JNT1Section {
      public const string Signature = "JNT1";
      public DataBlockHeader Header;
      public ushort NrJoints;
      public ushort Padding;
      public uint JointEntryOffset;
      public uint UnknownOffset;
      public uint StringTableOffset;
      public Jnt1Entry[] Joints;
      public BMD.Stringtable StringTable;

      public JNT1Section(EndianBinaryReader er, out bool OK)
      {
        long position = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "JNT1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrJoints = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.JointEntryOffset = er.ReadUInt32();
          this.UnknownOffset = er.ReadUInt32();
          this.StringTableOffset = er.ReadUInt32();
          er.Position = position + (long) this.StringTableOffset;
          this.StringTable = new BMD.Stringtable(er);
          er.Position = position + (long) this.JointEntryOffset;
          er.ReadNewArray(out this.Joints, this.NrJoints);
          er.Position = position + (long) this.Header.size;
          OK = true;
        }
      }
    }

    public partial class SHP1Section {
      public const string Signature = "SHP1";
      public DataBlockHeader Header;
      public ushort NrBatch;
      public ushort Padding;
      public uint BatchesOffset;
      public uint UnknownOffset;
      public uint Zero;
      public uint BatchAttribsOffset;
      public uint MatrixTableOffset;
      public uint DataOffset;
      public uint MatrixDataOffset;
      public uint PacketLocationsOffset;
      public BMD.SHP1Section.Batch[] Batches;

      public SHP1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "SHP1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrBatch = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.BatchesOffset = er.ReadUInt32();
          this.UnknownOffset = er.ReadUInt32();
          this.Zero = er.ReadUInt32();
          this.BatchAttribsOffset = er.ReadUInt32();
          this.MatrixTableOffset = er.ReadUInt32();
          this.DataOffset = er.ReadUInt32();
          this.MatrixDataOffset = er.ReadUInt32();
          this.PacketLocationsOffset = er.ReadUInt32();
          long position2 = er.Position;
          er.Position = position1 + (long) this.BatchesOffset;
          this.Batches = new BMD.SHP1Section.Batch[(int) this.NrBatch];
          for (int index = 0; index < (int) this.NrBatch; ++index)
            this.Batches[index] = new BMD.SHP1Section.Batch(er, position1, this);
          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public partial class Batch {
        public bool[] HasColors = new bool[2];
        public bool[] HasTexCoords = new bool[8];
        public byte MatrixType;
        public byte Unknown1;
        public ushort NrPacket;
        public ushort AttribsOffset;
        public ushort FirstMatrixData;
        public ushort FirstPacketLocation;
        public ushort Unknown2;
        public float Unknown3;
        public float[] BoundingBoxMin;
        public float[] BoundingBoxMax;
        public BatchAttribute[] BatchAttributes;
        public bool HasMatrixIndices;
        public bool HasPositions;
        public bool HasNormals;
        public PacketLocation[] PacketLocations;
        public BMD.SHP1Section.Batch.Packet[] Packets;

        public Batch(
            EndianBinaryReader er,
            long baseoffset,
            BMD.SHP1Section Parent) {
          this.MatrixType = er.ReadByte();
          this.Unknown1 = er.ReadByte();
          this.NrPacket = er.ReadUInt16();
          this.AttribsOffset = er.ReadUInt16();
          this.FirstMatrixData = er.ReadUInt16();
          this.FirstPacketLocation = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadSingle();
          this.BoundingBoxMin = er.ReadSingles(3);
          this.BoundingBoxMax = er.ReadSingles(3);
          long position = er.Position;
          er.Position = baseoffset +
                                   (long) Parent.BatchAttribsOffset +
                                   (long) this.AttribsOffset;
          List<BatchAttribute> source = new List<BatchAttribute>();
          {
            var entry = er.ReadNew<BatchAttribute>();
            source.Add(entry);

            while (source.Last<BatchAttribute>()
                         .Attribute !=
                   (uint) byte.MaxValue) {
              entry = new BatchAttribute();
              entry.Read(er);
              source.Add(entry);
            }
          }

        source.Remove(source.Last<BatchAttribute>());
          this.BatchAttributes = source.ToArray();
          for (int index = 0; index < this.BatchAttributes.Length; ++index)
          {
            if (this.BatchAttributes[index].DataType != 1U && this.BatchAttributes[index].DataType != 3U)
              throw new Exception();
            switch (this.BatchAttributes[index].Attribute)
            {
              case 0:
                this.HasMatrixIndices = true;
                break;
              case 9:
                this.HasPositions = true;
                break;
              case 10:
                this.HasNormals = true;
                break;
              case 11:
              case 12:
                this.HasColors[this.BatchAttributes[index].Attribute - 11U] = true;
                break;
              case 13:
              case 14:
              case 15:
              case 16:
              case 17:
              case 18:
              case 19:
              case 20:
                this.HasTexCoords[this.BatchAttributes[index].Attribute - 13U] = true;
                break;
            }
          }
          this.Packets = new BMD.SHP1Section.Batch.Packet[(int) this.NrPacket];
          this.PacketLocations = new PacketLocation[(int) this.NrPacket];
          for (int index = 0; index < (int) this.NrPacket; ++index)
          {
            er.Position = baseoffset + (long) Parent.PacketLocationsOffset + (long) (((int) this.FirstPacketLocation + index) * 8);
            var packetLocation = new PacketLocation();
            packetLocation.Read(er);
            this.PacketLocations[index] = packetLocation;

            er.Position = baseoffset + (long) Parent.DataOffset + (long) this.PacketLocations[index].Offset;
            this.Packets[index] = new BMD.SHP1Section.Batch.Packet(er, (int) this.PacketLocations[index].Size, this.BatchAttributes);
            er.Position = baseoffset + (long) Parent.MatrixDataOffset + (long) (((int) this.FirstMatrixData + index) * 8);
            this.Packets[index].MatrixData = new BMD.SHP1Section.Batch.Packet.Matrixdata(er);
            er.Position = baseoffset + (long) Parent.MatrixTableOffset + (long) (2U * this.Packets[index].MatrixData.FirstIndex);
            this.Packets[index].MatrixTable = er.ReadUInt16s((int) this.Packets[index].MatrixData.Count);
          }
          er.Position = position;
        }

        public class Packet
        {
          public BMD.SHP1Section.Batch.Packet.Primitive[] Primitives;
          public ushort[] MatrixTable;
          public BMD.SHP1Section.Batch.Packet.Matrixdata MatrixData;

          public Packet(
            EndianBinaryReader er,
            int Length,
            BatchAttribute[] Attributes)
          {
            List<BMD.SHP1Section.Batch.Packet.Primitive> primitiveList = new List<BMD.SHP1Section.Batch.Packet.Primitive>();
            bool flag = false;
            int num1 = 0;
            while (!flag)
            {
              BMD.SHP1Section.Batch.Packet.Primitive primitive = new BMD.SHP1Section.Batch.Packet.Primitive();
              primitive.Type = (BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive) er.ReadByte();
              ++num1;
              if (primitive.Type == (BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive) 0 || num1 >= Length)
              {
                flag = true;
              }
              else
              {
                ushort num2 = er.ReadUInt16();
                num1 += 2;
                primitive.Points = new BMD.SHP1Section.Batch.Packet.Primitive.Index[(int) num2];
                for (int index1 = 0; index1 < (int) num2; ++index1)
                {
                  primitive.Points[index1] = new BMD.SHP1Section.Batch.Packet.Primitive.Index();
                  for (int index2 = 0; index2 < Attributes.Length; ++index2)
                  {
                    ushort num3 = 0;
                    switch (Attributes[index2].DataType)
                    {
                      case 1:
                        num3 = (ushort) er.ReadByte();
                        ++num1;
                        break;
                      case 3:
                        num3 = er.ReadUInt16();
                        num1 += 2;
                        break;
                    }
                    switch (Attributes[index2].Attribute)
                    {
                      case 0:
                        primitive.Points[index1].MatrixIndex = num3;
                        break;
                      case 9:
                        primitive.Points[index1].PosIndex = num3;
                        break;
                      case 10:
                        primitive.Points[index1].NormalIndex = num3;
                        break;
                      case 11:
                      case 12:
                        primitive.Points[index1].ColorIndex[(Attributes[index2].Attribute - 11U)] = num3;
                        break;
                      case 13:
                      case 14:
                      case 15:
                      case 16:
                      case 17:
                      case 18:
                      case 19:
                      case 20:
                        primitive.Points[index1].TexCoordIndex[(Attributes[index2].Attribute - 13U)] = num3;
                        break;
                    }
                  }
                }
                primitiveList.Add(primitive);
              }
            }
            this.Primitives = primitiveList.ToArray();
          }

          public class Primitive
          {
            public BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive Type;
            public BMD.SHP1Section.Batch.Packet.Primitive.Index[] Points;

            public int GetGlPrimitive()
            {
              switch (this.Type)
              {
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_QUADS:
                  return 7;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLES:
                  return 4;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLESTRIP:
                  return 5;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLEFAN:
                  return 6;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_LINES:
                  return 1;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_LINESTRIP:
                  return 3;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_POINTS:
                  return 0;
                default:
                  return -1;
              }
            }

            public enum GXPrimitive
            {
              GX_QUADS = 128, // 0x00000080
              GX_TRIANGLES = 144, // 0x00000090
              GX_TRIANGLESTRIP = 152, // 0x00000098
              GX_TRIANGLEFAN = 160, // 0x000000A0
              GX_LINES = 168, // 0x000000A8
              GX_LINESTRIP = 176, // 0x000000B0
              GX_POINTS = 184, // 0x000000B8
            }

            public class Index
            {
              public ushort[] ColorIndex = new ushort[2];
              public ushort[] TexCoordIndex = new ushort[8];
              public ushort MatrixIndex;
              public ushort PosIndex;
              public ushort NormalIndex;
            }
          }

          public class Matrixdata
          {
            public ushort Unknown;
            public ushort Count;
            public uint FirstIndex;

            public Matrixdata(EndianBinaryReader er)
            {
              this.Unknown = er.ReadUInt16();
              this.Count = er.ReadUInt16();
              this.FirstIndex = er.ReadUInt32();
            }
          }
        }
      }
    }

    public enum CullMode {
      None = 0,  // Do not cull any primitives
      Front = 1, // Cull front-facing primitives
      Back = 2,  // Cull back-facing primitives
      All = 3    // Cull all primitives
    }

    public enum GxKonstColorSel {
      KCSel_1 = 0x00,     // Constant 1.0
      KCSel_7_8 = 0x01,   // Constant 7/8
      KCSel_3_4 = 0x02,   // Constant 3/4
      KCSel_5_8 = 0x03,   // Constant 5/8
      KCSel_1_2 = 0x04,   // Constant 1/2
      KCSel_3_8 = 0x05,   // Constant 3/8
      KCSel_1_4 = 0x06,   // Constant 1/4
      KCSel_1_8 = 0x07,   // Constant 1/8
      KCSel_K0 = 0x0C,    // K0[RGB] Register
      KCSel_K1 = 0x0D,    // K1[RGB] Register
      KCSel_K2 = 0x0E,    // K2[RGB] Register
      KCSel_K3 = 0x0F,    // K3[RGB] Register
      KCSel_K0_R = 0x10,  // K0[RRR] Register
      KCSel_K1_R = 0x11,  // K1[RRR] Register
      KCSel_K2_R = 0x12,  // K2[RRR] Register
      KCSel_K3_R = 0x13,  // K3[RRR] Register
      KCSel_K0_G = 0x14,  // K0[GGG] Register
      KCSel_K1_G = 0x15,  // K1[GGG] Register
      KCSel_K2_G = 0x16,  // K2[GGG] Register
      KCSel_K3_G = 0x17,  // K3[GGG] Register
      KCSel_K0_B = 0x18,  // K0[BBB] Register
      KCSel_K1_B = 0x19,  // K1[BBB] Register
      KCSel_K2_B = 0x1A,  // K2[BBB] Register
      KCSel_K3_B = 0x1B,  // K3[BBB] Register
      KCSel_K0_A = 0x1C,  // K0[AAA] Register
      KCSel_K1_A = 0x1D,  // K1[AAA] Register
      KCSel_K2_A = 0x1E,  // K2[AAA] Register
      KCSel_K3_A = 0x1F   // K3[AAA] Register
    }

    public enum GxKonstAlphaSel {
      KASel_1 = 0x00,     // Constant 1.0
      KASel_7_8 = 0x01,   // Constant 7/8
      KASel_3_4 = 0x02,   // Constant 3/4
      KASel_5_8 = 0x03,   // Constant 5/8
      KASel_1_2 = 0x04,   // Constant 1/2
      KASel_3_8 = 0x05,   // Constant 3/8
      KASel_1_4 = 0x06,   // Constant 1/4
      KASel_1_8 = 0x07,   // Constant 1/8
      KASel_K0_R = 0x10,  // K0[R] Register
      KASel_K1_R = 0x11,  // K1[R] Register
      KASel_K2_R = 0x12,  // K2[R] Register
      KASel_K3_R = 0x13,  // K3[R] Register
      KASel_K0_G = 0x14,  // K0[G] Register
      KASel_K1_G = 0x15,  // K1[G] Register
      KASel_K2_G = 0x16,  // K2[G] Register
      KASel_K3_G = 0x17,  // K3[G] Register
      KASel_K0_B = 0x18,  // K0[B] Register
      KASel_K1_B = 0x19,  // K1[B] Register
      KASel_K2_B = 0x1A,  // K2[B] Register
      KASel_K3_B = 0x1B,  // K3[B] Register
      KASel_K0_A = 0x1C,  // K0[A] Register
      KASel_K1_A = 0x1D,  // K1[A] Register
      KASel_K2_A = 0x1E,  // K2[A] Register
      KASel_K3_A = 0x1F   // K3[A] Register
    }



    public partial class MAT3Section {
      public const string Signature = "MAT3";
      public DataBlockHeader Header;
      public ushort NrMaterials;
      public uint[] Offsets;
      public BMD.MAT3Section.MaterialEntry[] MaterialEntries;
      public BMD.MAT3Section.PopulatedMaterial[] PopulatedMaterials;
      public ushort[] MaterialEntryIndieces;
      public short[] TextureIndices;
      public CullMode[] CullModes;
      public System.Drawing.Color[] MaterialColor;
      public System.Drawing.Color[] AmbientColors;
      public System.Drawing.Color[] ColorS10;
      public System.Drawing.Color[] Color3;
      public AlphaCompare[] AlphaCompares;
      public BlendFunction[] BlendFunctions;
      public DepthFunction[] DepthFunctions;
      public BMD.MAT3Section.TevStageProps[] TevStages;
      public TexCoordGen[] TexCoordGens;
      public TextureMatrixInfo[] TextureMatrices;
      public TevOrder[] TevOrders;
      public BMD.Stringtable MaterialNameTable;

      public MAT3Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "MAT3", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrMaterials = er.ReadUInt16();

          er.AssertUInt16(0xffff); // padding

          this.Offsets = er.ReadUInt32s(30);
          int[] sectionLengths = this.GetSectionLengths();
          long position2 = er.Position;

          // TODO: There is a bunch more data that isn't even read yet:
          // https://github.com/RenolY2/SuperBMD/blob/ccc86e21493275bcd9d86f65b516b85d95c83abd/SuperBMDLib/source/Materials/Enums/Mat3OffsetIndex.cs

          er.Position = position1 + (long) this.Offsets[0];
          this.MaterialEntries = new BMD.MAT3Section.MaterialEntry[sectionLengths[0] / 332];
          for (int index = 0; index < sectionLengths[0] / 332; ++index)
            this.MaterialEntries[index] = new BMD.MAT3Section.MaterialEntry(er);
          
          er.Position = position1 + (long) this.Offsets[1];
          this.MaterialEntryIndieces = er.ReadUInt16s((int) this.NrMaterials);
          
          er.Position = position1 + (long) this.Offsets[2];
          this.MaterialNameTable = new BMD.Stringtable(er);

          // TODO: Add support for indirect textures (3)

          er.Position = position1 + (long)this.Offsets[4];
          this.CullModes = new CullMode[sectionLengths[4] / 4];
          for (var index = 0; index < sectionLengths[4] / 4; ++index)
            this.CullModes[index] = (CullMode) er.ReadInt32();

          er.Position = position1 + (long) this.Offsets[5];
          this.MaterialColor = new System.Drawing.Color[sectionLengths[5] / 4];
          for (int index = 0; index < sectionLengths[5] / 4; ++index)
            this.MaterialColor[index] = er.ReadColor8();

          // TODO: Add support for color channel info (7)

          er.Position = position1 + (long) this.Offsets[8];
          this.AmbientColors = new System.Drawing.Color[sectionLengths[8] / 4];
          for (int index = 0; index < sectionLengths[8] / 4; ++index)
            this.AmbientColors[index] = er.ReadColor8();

          // TODO: Add support for light colors (9)
          // TODO: Add support for texgen counts (10)

          er.Position = position1 + this.Offsets[11];
          er.ReadNewArray(out this.TexCoordGens, sectionLengths[11] / 4);

          // TODO: Add support for post tex coord gens (12)

          er.Position = position1 + (long) this.Offsets[13];
          er.ReadNewArray(out this.TextureMatrices, sectionLengths[13] / 100);

          // TODO: Add support for post tex matrices (14)

          er.Position = position1 + (long) this.Offsets[15];
          this.TextureIndices = er.ReadInt16s(sectionLengths[15] / 2);

          er.Position = position1 + (long) this.Offsets[16];
          er.ReadNewArray(out this.TevOrders, sectionLengths[16] / 4);

          er.Position = position1 + (long) this.Offsets[17];
          this.ColorS10 = new System.Drawing.Color[sectionLengths[17] / 8];
          for (int index = 0; index < sectionLengths[17] / 8; ++index)
            this.ColorS10[index] = er.ReadColor16();
          
          er.Position = position1 + (long) this.Offsets[18];
          this.Color3 = new System.Drawing.Color[4];
          for (int index = 0; index < 4; ++index)
            this.Color3[index] = er.ReadColor8();

          // TODO: Add support for tev counts (19)

          er.Position = position1 + (long) this.Offsets[20];
          this.TevStages = new BMD.MAT3Section.TevStageProps[sectionLengths[20] / 20];
          for (int index = 0; index < sectionLengths[20] / 20; ++index)
            this.TevStages[index] = new BMD.MAT3Section.TevStageProps(er);

          // TODO: Add support for tev swap modes (21)
          // TODO: Add support for tev swap mode table (22)
          // TODO: Add support for fog modes (23)

          er.Position = position1 + (long) this.Offsets[24];
          er.ReadNewArray(out this.AlphaCompares, sectionLengths[24] / 8);

          er.Position = position1 + (long) this.Offsets[25];
          er.ReadNewArray(out this.BlendFunctions, sectionLengths[25] / 4);
          
          er.Position = position1 + (long) this.Offsets[26];
          er.ReadNewArray(out this.DepthFunctions, sectionLengths[26] / 4);

          er.Position = position1 + (long) this.Header.size;
          OK = true;

          // TODO: Add support for nbt scale (29)

          this.PopulatedMaterials = this.MaterialEntries.Select((entry, index) => new PopulatedMaterial(this, index, entry)).ToArray();
        }
      }

      public int[] GetSectionLengths()
      {
        int[] numArray = new int[30];
        for (int index1 = 0; index1 < 30; ++index1)
        {
          int num1 = 0;
          if (this.Offsets[index1] != 0U)
          {
            int num2 = (int) this.Header.size;
            for (int index2 = index1 + 1; index2 < 30; ++index2)
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
        public byte Flag;
        public byte CullModeIndex;
        public byte ColorChannelControlsCountIndex;
        public byte TexGensCountIndex;
        public byte TevStagesCountIndex;
        public byte ZCompLocIndex;
        public byte ZModeIndex;
        public byte DitherIndex;

        public short[] MaterialColorIndexes;
        public ushort[] ColorChannelControlIndexes;
        public ushort[] AmbientColorIndexes;
        public ushort[] LightColorIndexes;

        public ushort[] TexGenInfo;
        public TexGenType[] TexGenTypes;
        
        public ushort[] TexGenInfo2;
        public ushort[] TexMatrices;
        public ushort[] DttMatrices;
        public short[] TextureIndexes;
        public ushort[] TevKonstColorIndexes;
        public GxKonstColorSel[] KonstColorSel;
        public GxKonstAlphaSel[] KonstAlphaSel;
        public short[] TevOrderInfoIndexes;
        public ushort[] TevColorIndexes;
        public short[] TevStageInfoIndexes;
        public ushort[] TevSwapModeInfo;
        public ushort[] TevSwapModeTable;
        public ushort[] Unknown2;
        public short FogInfoIndex;
        public short AlphaCompareIndex;
        public short BlendModeIndex;
        public short UnknownIndex;

        // https://github.com/LordNed/WindEditor/wiki/BMD-and-BDL-Model-Format#material-entry
        public MaterialEntry(EndianBinaryReader er) {
          this.Flag = er.ReadByte();
          this.CullModeIndex = er.ReadByte();
          this.ColorChannelControlsCountIndex = er.ReadByte();
          this.TexGensCountIndex = er.ReadByte();
          this.TevStagesCountIndex = er.ReadByte();
          this.ZCompLocIndex = er.ReadByte();
          this.ZModeIndex = er.ReadByte();
          this.DitherIndex = er.ReadByte();
          
          this.MaterialColorIndexes = er.ReadInt16s(2);
          this.ColorChannelControlIndexes = er.ReadUInt16s(4);
          this.AmbientColorIndexes = er.ReadUInt16s(2);
          this.LightColorIndexes = er.ReadUInt16s(8);
          
          this.TexGenInfo = er.ReadUInt16s(8);
          this.TexGenTypes =
              this.TexGenInfo
                  .Select(texGenType
                              => texGenType != 65535
                                     ? (TexGenType) texGenType
                                     : TexGenType.UNDEFINED)
                  .ToArray();

          this.TexGenInfo2 = er.ReadUInt16s(8);
          this.TexMatrices = er.ReadUInt16s(10);
          this.DttMatrices = er.ReadUInt16s(20);
          this.TextureIndexes = er.ReadInt16s(8);
          this.TevKonstColorIndexes = er.ReadUInt16s(4);
          this.KonstColorSel =
              er.ReadBytes(16)
                .Select(konstColor => (GxKonstColorSel) konstColor)
                .ToArray();
          this.KonstAlphaSel =
              er.ReadBytes(16)
                .Select(konstAlpha => (GxKonstAlphaSel) konstAlpha)
                .ToArray();
          this.TevOrderInfoIndexes = er.ReadInt16s(16);
          this.TevColorIndexes = er.ReadUInt16s(4);
          this.TevStageInfoIndexes = er.ReadInt16s(16);
          this.TevSwapModeInfo = er.ReadUInt16s(16);
          this.TevSwapModeTable = er.ReadUInt16s(4);
          this.Unknown2 = er.ReadUInt16s(12);
          this.FogInfoIndex = er.ReadInt16();
          this.AlphaCompareIndex = er.ReadInt16();
          this.BlendModeIndex = er.ReadInt16();
          this.UnknownIndex = er.ReadInt16();
        }

        public enum TexGenType {
          GX_TG_MTX3x4,
          GX_TG_MTX2x4,
          GX_TG_BUMP0,
          GX_TG_BUMP1,
          GX_TG_BUMP2,
          GX_TG_BUMP3,
          GX_TG_BUMP4,
          GX_TG_BUMP5,
          GX_TG_BUMP6,
          GX_TG_BUMP7,
          GX_TG_SRTG,
          UNDEFINED,
      }
      }

      public class PopulatedMaterial {
        public string Name;
        public byte Flag;
        public CullMode CullMode;
        public byte ColorChannelControlsCountIndex;
        public byte TexGensCountIndex;
        public byte TevStagesCountIndex;
        public byte ZCompLocIndex;
        public byte ZModeIndex;
        public byte DitherIndex;

        public Color[] MaterialColors;
        public ushort[] ColorChannelControlIndexes;
        public Color[] AmbientColors;
        public ushort[] LightColorIndexes;

        public ushort[] TexGenInfo;
        public MaterialEntry.TexGenType[] TexGenTypes;

        public ushort[] TexGenInfo2;
        public ushort[] TexMatrices;
        public ushort[] DttMatrices;
        public short[] TextureIndices;
        public ushort[] TevKonstColorIndexes;
        public byte[] ConstColorSel;
        public byte[] ConstAlphaSel;

        public TevOrder[] TevOrderInfos;

        public ushort[] TevOrderInfoIndexes;
        public ushort[] TevColorIndexes;
        public TevStageProps[] TevStageInfos;
        public ushort[] TevSwapModeInfo;
        public ushort[] TevSwapModeTable;
        public ushort[] Unknown2;
        public short FogInfoIndex;
        public AlphaCompare AlphaCompare;
        public BlendFunction BlendMode;
        public short UnknownIndex;

        public PopulatedMaterial(MAT3Section mat3, int index, MaterialEntry entry) {
          this.Name = mat3.MaterialNameTable[index];
          this.Flag = entry.Flag;

          this.CullMode = mat3.CullModes[entry.CullModeIndex];

          this.MaterialColors =
              entry.MaterialColorIndexes
                   .Select(i => GetOrNull(mat3.MaterialColor, i))
                   .ToArray();
          this.AmbientColors =
              entry.AmbientColorIndexes
                   .Select(i => GetOrNull(mat3.AmbientColors, i))
                   .ToArray();

          this.TevOrderInfos =
              entry.TevOrderInfoIndexes
                   .Select(i => GetOrNull(mat3.TevOrders, i))
                   .ToArray();

          this.TevStageInfos = 
              entry.TevStageInfoIndexes
                   .Select(i => GetOrNull(mat3.TevStages, i))
                   .ToArray();

          this.TextureIndices =
              entry.TextureIndexes
                   .Select(t => (short) (t != -1 ? mat3.TextureIndices[t] : -1))
                   .ToArray();

          this.AlphaCompare = mat3.AlphaCompares[entry.AlphaCompareIndex];
          this.BlendMode = mat3.BlendFunctions[entry.BlendModeIndex];
        }

        private static T? GetOrNull<T>(IList<T> array, int i)
            where T : notnull
          => i != -1 ? array[i] : default;
      }

      public class TevStageProps {
        public GxCc color_a;
        public GxCc color_b;
        public GxCc color_c;
        public GxCc color_d;
        public GxCa alpha_a;
        public GxCa alpha_b;
        public GxCa alpha_c;
        public GxCa alpha_d;
        public TevOp color_op;
        public TevOp alpha_op;
        public ColorRegister color_regid;
        public ColorRegister alpha_regid;
        public byte pad;
        public byte texcoord;
        public sbyte texmap;
        public TevBias color_bias;
        public TevScale color_scale;
        public TevBias alpha_bias;
        public TevScale alpha_scale;
        public bool color_clamp;
        public bool alpha_clamp;

        public TevStageProps(EndianBinaryReader er) {
          er.ReadByte();
          this.color_a = (GxCc) er.ReadByte();
          this.color_b = (GxCc) er.ReadByte();
          this.color_c = (GxCc) er.ReadByte();
          this.color_d = (GxCc) er.ReadByte();
          this.color_op = (TevOp) er.ReadByte();
          this.color_bias = (TevBias) er.ReadByte();
          this.color_scale = (TevScale) er.ReadByte();
          this.color_clamp = er.ReadByte() == (byte) 1;
          this.color_regid = (ColorRegister) er.ReadByte();
          this.alpha_a = (GxCa) er.ReadByte();
          this.alpha_b = (GxCa) er.ReadByte();
          this.alpha_c = (GxCa) er.ReadByte();
          this.alpha_d = (GxCa) er.ReadByte();
          this.alpha_op = (TevOp) er.ReadByte();
          this.alpha_bias = (TevBias) er.ReadByte();
          this.alpha_scale = (TevScale) er.ReadByte();
          this.alpha_clamp = er.ReadByte() == (byte) 1;
          this.alpha_regid = (ColorRegister) er.ReadByte();
          er.ReadByte();
        }

        public enum GxCc {
          GX_CC_CPREV,
          GX_CC_APREV,
          GX_CC_C0,
          GX_CC_A0,
          GX_CC_C1,
          GX_CC_A1,
          GX_CC_C2,
          GX_CC_A2,
          GX_CC_TEXC,
          GX_CC_TEXA,
          GX_CC_RASC,
          GX_CC_RASA,
          GX_CC_ONE,
          GX_CC_HALF,
          GX_CC_KONST,
          GX_CC_ZERO,
        }

        public enum GxCa {
          GX_CA_APREV,
          GX_CA_A0,
          GX_CA_A1,
          GX_CA_A2,
          GX_CA_TEXA,
          GX_CA_RASA,
          GX_CA_KONST,
          GX_CA_ZERO,
        }

        public enum TevOp {
          GX_TEV_ADD = 0,
          GX_TEV_SUB = 1,
          GX_TEV_COMP_R8_GT = 8,
          GX_TEV_COMP_R8_EQ = 9,
          GX_TEV_COMP_GR16_GT = 10,
          GX_TEV_COMP_GR16_EQ = 11,
          GX_TEV_COMP_BGR24_GT = 12,
          GX_TEV_COMP_BGR24_EQ = 13,
          GX_TEV_COMP_RGB8_GT = 14,
          GX_TEV_COMP_RGB8_EQ = 15
        }

        public enum TevBias {
          GX_TB_ZERO,
          GX_TB_ADDHALF,
          GX_TB_SUBHALF
        }

        public enum TevScale {
          GX_CS_SCALE_1,
          GX_CS_SCALE_2,
          GX_CS_SCALE_4,
          GX_CS_DIVIDE_2
        }

        public enum ColorRegister {
          GX_TEVPREV,
          GX_TEVREG0,
          GX_TEVREG1,
          GX_TEVREG2,
        }
      }
    }

    public class TEX1Section
    {
      public const string Signature = "TEX1";
      public DataBlockHeader Header;
      public ushort NrTextures;
      public ushort Padding;
      public uint TextureHeaderOffset;
      public uint StringTableOffset;
      public BMD.Stringtable StringTable;
      public BMD.TEX1Section.TextureHeader[] TextureHeaders;

      public TEX1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "TEX1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrTextures = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.TextureHeaderOffset = er.ReadUInt32();
          this.StringTableOffset = er.ReadUInt32();
          long position2 = er.Position;
          er.Position = position1 + (long) this.StringTableOffset;
          this.StringTable = new BMD.Stringtable(er);
          er.Position = position1 + (long) this.TextureHeaderOffset;
          this.TextureHeaders = new BMD.TEX1Section.TextureHeader[(int) this.NrTextures];
          for (int idx = 0; idx < (int) this.NrTextures; ++idx)
            this.TextureHeaders[idx] = new BMD.TEX1Section.TextureHeader(er, position1, idx);
          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public enum TextureFormat : byte {
        I4 = 0,
        I8 = 1,
        A4_I4 = 2,
        A8_I8 = 3,
        R5_G6_B5 = 4,
        A3_RGB5 = 5,
        ARGB8 = 6,
        INDEX4 = 8,
        INDEX8 = 9,
        INDEX14_X2 = 10, // 0x0000000A
        S3TC1 = 14, // 0x0000000E
      }

      public enum PaletteFormat : byte {
        PAL_A8_I8,
        PAL_R5_G6_B5,
        PAL_A3_RGB5,
      }

      public enum GX_WRAP_TAG: byte {
        GX_CLAMP,
        GX_REPEAT,
        GX_MIRROR,
        GX_MAXTEXWRAPMODE,
      }

      public enum GX_TEXTURE_FILTER : byte {
        GX_NEAR,
        GX_LINEAR,
        GX_NEAR_MIP_NEAR,
        GX_LIN_MIP_NEAR,
        GX_NEAR_MIP_LIN,
        GX_LIN_MIP_LIN,
        GX_NEAR2,
        GX_NEAR3,
      }

      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      public class TextureHeader {
        // Do not modify any of these types or the order!
        public BMD.TEX1Section.TextureFormat Format;
        public Byte AlphaSetting;
        public UInt16 Width;
        public UInt16 Height;
        public BMD.TEX1Section.GX_WRAP_TAG WrapS;
        public BMD.TEX1Section.GX_WRAP_TAG WrapT;
        public Byte PalettesEnabled;
        public BMD.TEX1Section.PaletteFormat PaletteFormat;
        public UInt16 NrPaletteEntries;
        public UInt32 PaletteOffset;
        public IColor[] palette;
        public UInt32 BorderColor;
        public BMD.TEX1Section.GX_TEXTURE_FILTER MinFilter;
        public BMD.TEX1Section.GX_TEXTURE_FILTER MagFilter;
        public UInt16 Unknown4;
        public Byte NrMipMap;
        public Byte Unknown5;
        public UInt16 LodBias;
        public UInt32 DataOffset;

        [NonSerialized]
        public byte[] Data;

        public TextureHeader(EndianBinaryReader er, long baseoffset, int idx) {
          var pos = er.Position;

          this.Format = (BMD.TEX1Section.TextureFormat) er.ReadByte();
          this.AlphaSetting = er.ReadByte();
          this.Width = er.ReadUInt16();
          this.Height = er.ReadUInt16();
          this.WrapS = (BMD.TEX1Section.GX_WRAP_TAG) er.ReadByte();
          this.WrapT = (BMD.TEX1Section.GX_WRAP_TAG) er.ReadByte();
          this.PalettesEnabled = er.ReadByte();
          this.PaletteFormat = (BMD.TEX1Section.PaletteFormat) er.ReadByte();
          this.NrPaletteEntries = er.ReadUInt16();
          this.PaletteOffset = er.ReadUInt32();
          this.BorderColor = er.ReadUInt32();
          this.MinFilter = (BMD.TEX1Section.GX_TEXTURE_FILTER) er.ReadByte();
          this.MagFilter = (BMD.TEX1Section.GX_TEXTURE_FILTER) er.ReadByte();
          this.Unknown4 = er.ReadUInt16();
          this.NrMipMap = er.ReadByte();
          this.Unknown5 = er.ReadByte();
          this.LodBias = er.ReadUInt16();
          this.DataOffset = er.ReadUInt32();

          long position = er.Position;
          {
            er.Position = baseoffset + (long)this.DataOffset + (long)(32 * (idx + 1));
            this.Data = er.ReadBytes(this.GetCompressedBufferSize());
          }

          this.palette = new IColor[this.NrPaletteEntries];
          {
            er.Position = pos + this.PaletteOffset;
            for (var i = 0; i < this.NrPaletteEntries; ++i) {

              switch (this.PaletteFormat) {
                case PaletteFormat.PAL_A8_I8: {
                  var alpha = er.ReadByte();
                  var intensity = er.ReadByte();
                  this.palette[i] =
                      ColorImpl.FromRgbaBytes(intensity,
                                              intensity,
                                              intensity,
                                              alpha);
                  break;
                }
                case PaletteFormat.PAL_R5_G6_B5: {
                  this.palette[i] = ColorUtil.ParseRgb565(er.ReadUInt16());
                  break;
                }
                // TODO: There seems to be a bug reading the palette, these colors look weird
                case PaletteFormat.PAL_A3_RGB5: {
                  this.palette[i] = ColorUtil.ParseRgb5A3(er.ReadUInt16());
                  break;
                }
                default: 
                  throw new ArgumentOutOfRangeException();
              }
            }
          }
          er.Position = position;
        }

        // TODO: Share this implementation w/ BTI
        public unsafe System.Drawing.Bitmap ToBitmap() {
          Bitmap bitmap;
          var isIndex4 = this.Format == TextureFormat.INDEX4;
          var isIndex8 = this.Format == TextureFormat.INDEX8;
          if (isIndex4 || isIndex8) {
            bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            BitmapUtil.InvokeAsLocked(
                bitmap,
                bitmapData => {
                  var indices = new byte[this.Width * this.Height];
                  if (isIndex4) {
                    for (var i = 0; i < this.Data.Length; ++i) {
                      var two = this.Data[i];

                      var firstIndex = two >> 4;
                      var secondIndex = two & 0x0F;

                      indices[2 * i + 0] = (byte) firstIndex;
                      indices[2 * i + 1] = (byte) secondIndex;
                    }
                  } else {
                    indices = this.Data;
                  }

                  var blockWidth = 8;
                  var blockHeight = isIndex4 ? 8 : 4;

                  var index = 0;
                  var bytes = (byte*)bitmapData.Scan0.ToPointer();
                  for (var ty = 0; ty < this.Height / blockHeight; ty++) {
                    for (var tx = 0; tx < this.Width / blockWidth; tx++) {

                      for (var y = 0; y < blockHeight; ++y) {
                        for (var x = 0; x < blockWidth; ++x) {
                          var color = this.palette[indices[index++]];

                          var i = (ty * blockHeight + y) * this.Width + tx * blockWidth + x;
                          bytes[4 * i + 0] = color.Bb;
                          bytes[4 * i + 1] = color.Gb;
                          bytes[4 * i + 2] = color.Rb;
                          bytes[4 * i + 3] = color.Ab;
                        }
                      }
                    }
                  }
                });

            return bitmap;
          } 
          
          ImageDataFormat imageDataFormat = (ImageDataFormat)null;
          switch (this.Format) {
            case TextureFormat.I4:
              imageDataFormat = ImageDataFormat.I4;
              break;
            case TextureFormat.I8:
              imageDataFormat = ImageDataFormat.I8;
              break;
            case TextureFormat.A4_I4:
              imageDataFormat = ImageDataFormat.IA4;
              break;
            case TextureFormat.A8_I8:
              imageDataFormat = ImageDataFormat.IA8;
              break;
            case TextureFormat.R5_G6_B5:
              imageDataFormat = ImageDataFormat.RGB565;
              break;
            case TextureFormat.A3_RGB5:
              imageDataFormat = ImageDataFormat.RGB5A3;
              break;
            case TextureFormat.ARGB8:
              imageDataFormat = ImageDataFormat.Rgba32;
              break;
            case TextureFormat.S3TC1:
              imageDataFormat = ImageDataFormat.Cmpr;
              break;
            default:
              throw new NotImplementedException();
          }

          byte[] numArray = imageDataFormat.ConvertFrom(this.Data, (int)this.Width, (int)this.Height, (ProgressChangedEventHandler)null); 
          bitmap = new System.Drawing.Bitmap((int)this.Width, (int)this.Height);
          BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, (int)this.Width, (int)this.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          for (int ofs = 0; ofs < numArray.Length; ++ofs)
            Marshal.WriteByte(bitmapdata.Scan0, ofs, numArray[ofs]);
          bitmap.UnlockBits(bitmapdata);
          return bitmap;
        }

        private int GetCompressedBufferSize() {
          int num1 = (int) this.Width + (8 - (int) this.Width % 8) % 8;
          int num2 = (int) this.Width + (4 - (int) this.Width % 4) % 4;
          int num3 = (int) this.Height + (8 - (int) this.Height % 8) % 8;
          int num4 = (int) this.Height + (4 - (int) this.Height % 4) % 4;
          switch (this.Format)
          {
            case BMD.TEX1Section.TextureFormat.I4:
              return num1 * num3 / 2;
            case BMD.TEX1Section.TextureFormat.I8:
              return num1 * num4;
            case BMD.TEX1Section.TextureFormat.A4_I4:
              return num1 * num4;
            case BMD.TEX1Section.TextureFormat.A8_I8:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.R5_G6_B5:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.A3_RGB5:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.ARGB8:
              return num2 * num4 * 4;
            case BMD.TEX1Section.TextureFormat.INDEX4:
              return num1 * num3 / 2;
            case BMD.TEX1Section.TextureFormat.INDEX8:
              return num1 * num4;
            case BMD.TEX1Section.TextureFormat.INDEX14_X2:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.S3TC1:
              return num2 * num4 / 2;
            default:
              return -1;
          }
        }
      }
    }

    public class Stringtable
    {
      public ushort NrStrings;
      public ushort Padding;
      public BMD.Stringtable.StringTableEntry[] Entries;

      public Stringtable(EndianBinaryReader er)
      {
        long position = er.Position;
        this.NrStrings = er.ReadUInt16();
        this.Padding = er.ReadUInt16();
        this.Entries = new BMD.Stringtable.StringTableEntry[(int) this.NrStrings];
        for (int index = 0; index < (int) this.NrStrings; ++index)
          this.Entries[index] = new BMD.Stringtable.StringTableEntry(er, position);
      }

      public string this[int index] => (string) this.Entries[index];

      public int this[string index]
      {
        get
        {
          for (int index1 = 0; index1 < this.Entries.Length; ++index1)
          {
            if ((string) this.Entries[index1] == index)
              return index1;
          }
          return -1;
        }
      }

      public class StringTableEntry
      {
        public ushort Unknown;
        public ushort Offset;
        public string Entry;

        public StringTableEntry(EndianBinaryReader er, long baseoffset)
        {
          this.Unknown = er.ReadUInt16();
          this.Offset = er.ReadUInt16();
          long position = er.Position;
          er.Position = baseoffset + (long) this.Offset;
          this.Entry = er.ReadStringNT(Encoding.ASCII);
          er.Position = position;
        }

        public override string ToString() => this.Entry;

        public static implicit operator string(
            BMD.Stringtable.StringTableEntry e)
          => e.Entry;
      }
    }

    private class Node
    {
      public BMD.Node Parent;
      public string Name;

      public Node(string Name, BMD.Node Parent)
      {
        this.Name = Name;
        this.Parent = Parent;
      }
    }
  }
}
