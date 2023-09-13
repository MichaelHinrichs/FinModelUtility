// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

//using MKDS_Course_Modifier.Converters._3D;
//using MKDS_Course_Modifier.Converters.Colission;
//using MKDS_Course_Modifier.UI;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;

using fin.schema;
using fin.schema.matrix;
using fin.util.asserts;
using fin.util.color;

using gx;

using j3d._3D_Formats;
using j3d.G3D_Binary_File_Format;
using j3d.misc.GCN;
using j3d.schema.bmd;
using j3d.schema.bmd.drw1;
using j3d.schema.bmd.inf1;
using j3d.schema.bmd.jnt1;
using j3d.schema.bmd.mat3;
using j3d.schema.bmd.shp1;
using j3d.schema.bmd.tex1;
using j3d.schema.bmd.vtx1;

using schema.binary;

#pragma warning disable CS8604

namespace j3d.GCN {
  public partial class BMD {
    public BmdHeader Header;
    public Inf1 INF1 { get; set; }
    public VTX1Section VTX1;
    public EVP1Section EVP1;
    public Drw1 DRW1 { get; set; }
    public Jnt1 JNT1 { get; set; }
    public SHP1Section SHP1;
    public MAT3Section MAT3;
    public Tex1 TEX1 { get; set; }

    public BMD(byte[] file)
    {
      using var er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.BigEndian);
      this.Header = er.ReadNew<BmdHeader>();

      bool OK;
      while (!er.Eof)
      {
        switch (er.ReadString(4))
        {
          case nameof(INF1):
            er.Position -= 4L;
            this.INF1 = er.ReadNew<Inf1>();
            break;
          case nameof (VTX1):
            er.Position -= 4L;
            this.VTX1 = new VTX1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 4");
              return;
            } else
              break;
          case nameof (EVP1):
            er.Position -= 4L;
            this.EVP1 = new EVP1Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 4");
              return;
            } else
              break;
          case nameof (DRW1):
            er.Position -= 4L;
            this.DRW1 = er.ReadNew<Drw1>();
            break;
          case nameof (JNT1):
            er.Position -= 4L;
            this.JNT1 = er.ReadNew<Jnt1>();
            break;
          case nameof (SHP1):
            er.Position -= 4L;
            this.SHP1 = new SHP1Section(er, out OK);
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
            this.MAT3 = new MAT3Section(er, out OK);
            if (!OK)
            {
              // TODO: Message box
              //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 8");
              return;
            } else
              break;
          case nameof (TEX1):
            er.Position -= 4L;
            this.TEX1 = er.ReadNew<Tex1>();
            break;
          default:
            return;
        }
      }
    }

    public MA.Node[] GetJoints()
    {
      var nodeIndexStack = new Stack<int>();
      nodeIndexStack.Push(-1);
      var nodeList = new List<MA.Node>();
      int nodeIndex = -1;
      foreach (Inf1Entry entry in this.INF1.Data.Entries)
      {
        switch (entry.Type)
        {
          case Inf1EntryType.TERMINATOR:
            goto label_7;
          case Inf1EntryType.HIERARCHY_DOWN:
            nodeIndexStack.Push(nodeIndex);
            break;
          case Inf1EntryType.HIERARCHY_UP:
            nodeIndexStack.Pop();
            break;
          case Inf1EntryType.JOINT:
            var jnt1 = this.JNT1.Data;
            var jointIndex = jnt1.RemapTable[entry.Index];
            nodeList.Add(new MA.Node(
                             jnt1.Joints[jointIndex],
                             jnt1.StringTable[jointIndex],
                             nodeIndexStack.Peek()));
            nodeIndex = entry.Index;
            break;
          case Inf1EntryType.MATERIAL:
            break;
          case Inf1EntryType.SHAPE:
            break;
        }
      }
label_7:
      return nodeList.ToArray();
    }

    public partial class VTX1Section {
      public Color[][] Colors = new Color[2][];
      public Texcoord[][] Texcoords = new Texcoord[8][];
      public const string Signature = "VTX1";
      public DataBlockHeader Header;
      public uint ArrayFormatOffset;
      public uint[] Offsets;
      public ArrayFormat[] ArrayFormats;
      public Vector3[] Positions;
      public Vector3[] Normals;

      public VTX1Section(IEndianBinaryReader er, out bool OK)
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
              if (arrayFormat.ArrayType is GxAttribute.CLR0 or GxAttribute.CLR1) {
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
          IEndianBinaryReader er) {
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
          case GxAttribute.POS:
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
          case GxAttribute.NRM:
            if (Format.ComponentCount != 0U)
              break;
            this.Normals = new Vector3[floatList.Count / 3];
            for (int index = 0; index < floatList.Count - 2; index += 3)
              this.Normals[index / 3] = new Vector3(
                  floatList[index],
                  floatList[index + 1],
                  floatList[index + 2]);
            break;
          case GxAttribute.TEX0:
          case GxAttribute.TEX1:
          case GxAttribute.TEX2:
          case GxAttribute.TEX3:
          case GxAttribute.TEX4:
          case GxAttribute.TEX5:
          case GxAttribute.TEX6:
          case GxAttribute.TEX7:
            var texCoordIndex = Format.ArrayType - GxAttribute.TEX0;
            switch (Format.ComponentCount) {
              case 0:
                this.Texcoords[texCoordIndex] = new Texcoord[floatList.Count];
                for (int index = 0; index < floatList.Count; ++index)
                  this.Texcoords[texCoordIndex][index] =
                      new Texcoord(floatList[index], 0.0f);
                return;
              case 1:
                this.Texcoords[texCoordIndex] =
                    new Texcoord[floatList.Count / 2];
                for (int index = 0; index < floatList.Count - 1; index += 2)
                  this.Texcoords[texCoordIndex][index / 2] =
                      new Texcoord(
                          floatList[index],
                          floatList[index + 1]);
                return;
              default:
                return;
            }
          default:
            throw new NotImplementedException();
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
          IEndianBinaryReader er) {
        var colorIndex = Format.ArrayType - GxAttribute.CLR0;

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

        this.Colors[colorIndex] = colors;
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
          this.B = b;
          this.A = a;
        }

        public static implicit operator System.Drawing.Color(Color c)
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
      public Matrix3x4f[] InverseBindMatrices { get; set; }
      public MultiMatrix[] WeightedIndices;

      public EVP1Section(IEndianBinaryReader er, out bool OK)
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
          this.WeightedIndices = new MultiMatrix[(int) this.Count];
          int val1 = 0;
          for (int index1 = 0; index1 < (int) this.Count; ++index1)
          {
            this.WeightedIndices[index1] = new MultiMatrix();
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
          this.InverseBindMatrices = new Matrix3x4f[val1];
          for (int index = 0; index < val1; ++index) {
            this.InverseBindMatrices[index] = er.ReadNew<Matrix3x4f>();
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


    public partial class SHP1Section {
      public const string Signature = "SHP1";
      public DataBlockHeader Header;
      public ushort NrBatch;
      public ushort Padding;
      public uint BatchesOffset;
      public uint ShapeRemapTableOffset;
      public short[] ShapeRemapTable;
      public uint Zero;
      public uint BatchAttribsOffset;
      public uint MatrixTableOffset;
      public uint DataOffset;
      public uint MatrixDataOffset;
      public uint PacketLocationsOffset;
      public Batch[] Batches;

      public SHP1Section(IEndianBinaryReader er, out bool OK)
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
          this.ShapeRemapTableOffset = er.ReadUInt32();
          this.Zero = er.ReadUInt32();
          this.BatchAttribsOffset = er.ReadUInt32();
          this.MatrixTableOffset = er.ReadUInt32();
          this.DataOffset = er.ReadUInt32();
          this.MatrixDataOffset = er.ReadUInt32();
          this.PacketLocationsOffset = er.ReadUInt32();
          long position2 = er.Position;
          {
            er.Position = position1 + (long)this.BatchesOffset;
            this.Batches = new Batch[(int)this.NrBatch];
            for (int index = 0; index < (int)this.NrBatch; ++index) {
              this.Batches[index] = new Batch(er, position1, this);
            }
          }
          {
            er.Position = position1 + (long)this.ShapeRemapTableOffset;
            this.ShapeRemapTable = er.ReadInt16s(this.NrBatch);
          }
          er.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public enum MatrixType : byte {
        Mtx = 0,
        BBoard = 1,
        YBBoard = 2,
        Multi = 3,
      }

      public partial class Batch {
        public bool[] HasColors = new bool[2];
        public bool[] HasTexCoords = new bool[8];
        public MatrixType MatrixType;

        [Unknown]
        public byte Unknown1;

        public ushort NrPacket;
        public ushort AttribsOffset;
        public ushort FirstMatrixData;
        public ushort FirstPacketLocation;

        [Unknown]
        public ushort Unknown2;

        public float BoundingSphereReadius;
        public float[] BoundingBoxMin;
        public float[] BoundingBoxMax;
        public BatchAttribute[] BatchAttributes;
        public bool HasMatrixIndices;
        public bool HasPositions;
        public bool HasNormals;
        public PacketLocation[] PacketLocations;
        public Packet[] Packets;

        public Batch(
            IEndianBinaryReader er,
            long baseoffset,
            SHP1Section Parent) {
          this.MatrixType = (MatrixType) er.ReadByte();
          this.Unknown1 = er.ReadByte();
          this.NrPacket = er.ReadUInt16();
          this.AttribsOffset = er.ReadUInt16();
          this.FirstMatrixData = er.ReadUInt16();
          this.FirstPacketLocation = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.BoundingSphereReadius = er.ReadSingle();
          this.BoundingBoxMin = er.ReadSingles(3);
          this.BoundingBoxMax = er.ReadSingles(3);
          long position = er.Position;
          er.Position = baseoffset +
                                   (long) Parent.BatchAttribsOffset +
                                   (long) this.AttribsOffset;
          List<BatchAttribute> source = new List<BatchAttribute>();
          {
            BatchAttribute entry;
            do {
              entry = er.ReadNew<BatchAttribute>();
              source.Add(entry);
            } while ((uint) entry.Attribute != byte.MaxValue);
          }

        source.Remove(source.Last<BatchAttribute>());
          this.BatchAttributes = source.ToArray();
          for (int index = 0; index < this.BatchAttributes.Length; ++index)
          {
            if (this.BatchAttributes[index].DataType != 1U && this.BatchAttributes[index].DataType != 3U)
              throw new Exception();
            switch (this.BatchAttributes[index].Attribute) {
              case GxAttribute.PNMTXIDX:
                this.HasMatrixIndices = true;
                break;
              case GxAttribute.POS:
                this.HasPositions = true;
                break;
              case GxAttribute.NRM:
                this.HasNormals = true;
                break;
              case GxAttribute.CLR0:
              case GxAttribute.CLR1:
                this.HasColors[
                        this.BatchAttributes[index].Attribute -
                        GxAttribute.CLR0] =
                    true;
                break;
              case GxAttribute.TEX0:
              case GxAttribute.TEX1:
              case GxAttribute.TEX2:
              case GxAttribute.TEX3:
              case GxAttribute.TEX4:
              case GxAttribute.TEX5:
              case GxAttribute.TEX6:
              case GxAttribute.TEX7:
                this.HasTexCoords[this.BatchAttributes[index].Attribute -
                                  GxAttribute.TEX0] =
                    true;
                break;
            }
          }
          this.Packets = new Packet[(int) this.NrPacket];
          this.PacketLocations = new PacketLocation[(int) this.NrPacket];
          for (int index = 0; index < (int) this.NrPacket; ++index)
          {
            er.Position = baseoffset + (long) Parent.PacketLocationsOffset + (long) (((int) this.FirstPacketLocation + index) * 8);
            var packetLocation = new PacketLocation();
            packetLocation.Read(er);
            this.PacketLocations[index] = packetLocation;

            er.Position = baseoffset + (long) Parent.DataOffset + (long) this.PacketLocations[index].Offset;
            this.Packets[index] = new Packet(er, (int) this.PacketLocations[index].Size, this.BatchAttributes);
            er.Position = baseoffset + (long) Parent.MatrixDataOffset + (long) (((int) this.FirstMatrixData + index) * 8);
            this.Packets[index].MatrixData = er.ReadNew<MatrixData>();
            er.Position = baseoffset + (long) Parent.MatrixTableOffset + (long) (2U * this.Packets[index].MatrixData.FirstIndex);
            this.Packets[index].MatrixTable = er.ReadUInt16s((int) this.Packets[index].MatrixData.Count);
          }
          er.Position = position;
        }

        public class Packet
        {
          public Primitive[] Primitives;
          public ushort[] MatrixTable;
          public MatrixData MatrixData;

          public Packet(
            IEndianBinaryReader er,
            int Length,
            BatchAttribute[] Attributes)
          {
            List<Primitive> primitiveList = new List<Primitive>();
            bool flag = false;
            int num1 = 0;
            while (!flag)
            {
              Primitive primitive = new Primitive();
              primitive.Type = (Primitive.GXPrimitive) er.ReadByte();
              ++num1;
              if (primitive.Type == (Primitive.GXPrimitive) 0 || num1 >= Length)
              {
                flag = true;
              }
              else
              {
                ushort num2 = er.ReadUInt16();
                num1 += 2;
                primitive.Points = new Primitive.Index[(int) num2];
                for (int index1 = 0; index1 < (int) num2; ++index1)
                {
                  primitive.Points[index1] = new Primitive.Index();
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
                    switch (Attributes[index2].Attribute) {
                      case GxAttribute.PNMTXIDX:
                        primitive.Points[index1].MatrixIndex = num3;
                        break;
                      case GxAttribute.POS:
                        primitive.Points[index1].PosIndex = num3;
                        break;
                      case GxAttribute.NRM:
                        primitive.Points[index1].NormalIndex = num3;
                        break;
                      case GxAttribute.CLR0:
                      case GxAttribute.CLR1:
                        primitive.Points[index1]
                                 .ColorIndex[
                                     (Attributes[index2].Attribute -
                                      GxAttribute.CLR0)] = num3;
                        break;
                      case GxAttribute.TEX0:
                      case GxAttribute.TEX1:
                      case GxAttribute.TEX2:
                      case GxAttribute.TEX3:
                      case GxAttribute.TEX4:
                      case GxAttribute.TEX5:
                      case GxAttribute.TEX6:
                      case GxAttribute.TEX7:
                        primitive.Points[index1]
                                 .TexCoordIndex[
                                     (Attributes[index2].Attribute -
                                      GxAttribute.TEX0)] = num3;
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
            public GXPrimitive Type;
            public Index[] Points;

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
        }
      }
    }

    
    public partial class MAT3Section {
      public const string Signature = "MAT3";
      public DataBlockHeader Header;
      public ushort NrMaterials;
      public uint[] Offsets;
      public MaterialEntry[] MaterialEntries;
      public BmdPopulatedMaterial[] PopulatedMaterials;
      public ushort[] MaterialEntryIndieces;
      public short[] TextureIndices;
      public GxCullMode[] CullModes;
      public Color[] MaterialColor;
      public Color[] LightColors;
      public Color[] AmbientColors;
      public Color[] TevColors;
      public Color[] TevKonstColors;
      public AlphaCompare[] AlphaCompares;
      public BlendFunction[] BlendFunctions;
      public DepthFunction[] DepthFunctions;
      public TevStageProps[] TevStages;
      public TevSwapMode[] TevSwapModes;
      public TevSwapModeTable[] TevSwapModeTables;
      public TexCoordGen[] TexCoordGens;
      public ColorChannelControl[] ColorChannelControls;
      public TextureMatrixInfo[] TextureMatrices;
      public TevOrder[] TevOrders;
      public StringTable MaterialNameTable;

      public readonly List<MatIndirectTexturingEntry>
          MatIndirectTexturingEntries = new();

      public MAT3Section(IEndianBinaryReader er, out bool OK)
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
          this.MaterialEntries = new MaterialEntry[sectionLengths[0] / 332];
          for (int index = 0; index < sectionLengths[0] / 332; ++index)
            this.MaterialEntries[index] = er.ReadNew<MaterialEntry>();
          
          er.Position = position1 + (long) this.Offsets[1];
          this.MaterialEntryIndieces = er.ReadUInt16s((int) this.NrMaterials);
          
          er.Position = position1 + (long) this.Offsets[2];
          this.MaterialNameTable = er.ReadNew<StringTable>();

          var indirectTexturesOffset =
              er.Position = position1 + this.Offsets[3];
          this.MatIndirectTexturingEntries.Clear();
          while ((er.Position - indirectTexturesOffset) < sectionLengths[3]) {
            this.MatIndirectTexturingEntries.Add(
                er.ReadNew<MatIndirectTexturingEntry>());
          }

          er.Position = position1 + (long)this.Offsets[4];
          this.CullModes = new GxCullMode[sectionLengths[4] / 4];
          for (var index = 0; index < sectionLengths[4] / 4; ++index)
            this.CullModes[index] = (GxCullMode) er.ReadInt32();

          er.Position = position1 + (long) this.Offsets[5];
          this.MaterialColor = new Color[sectionLengths[5] / 4];
          for (int index = 0; index < sectionLengths[5] / 4; ++index)
            this.MaterialColor[index] = er.ReadColor8();

          er.Position = position1 + (long) this.Offsets[7];
          this.ColorChannelControls =
              new ColorChannelControl[sectionLengths[7] / 8];
          for (var i = 0; i < this.ColorChannelControls.Length; ++i) {
            this.ColorChannelControls[i] = er.ReadNew<ColorChannelControl>();
          }

          er.Position = position1 + (long) this.Offsets[8];
          this.AmbientColors = new Color[sectionLengths[8] / 4];
          for (int index = 0; index < sectionLengths[8] / 4; ++index)
            this.AmbientColors[index] = er.ReadColor8();

          er.Position = position1 + this.Offsets[9];
          this.LightColors = new Color[sectionLengths[9] / 8];
          for (int index = 0; index < this.LightColors.Length; ++index) {
            this.LightColors[index] = er.ReadColor16();
          }

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
          this.TevColors = new Color[sectionLengths[17] / 8];
          for (int index = 0; index < this.TevColors.Length; ++index)
            this.TevColors[index] = er.ReadColor16();
          
          er.Position = position1 + (long) this.Offsets[18];
          this.TevKonstColors = new Color[sectionLengths[18] / 4];
          for (int index = 0; index < this.TevKonstColors.Length; ++index)
            this.TevKonstColors[index] = er.ReadColor8();

          // TODO: Add support for tev counts (19)

          er.Position = position1 + (long) this.Offsets[20];
          er.ReadNewArray(out this.TevStages, sectionLengths[20] / 20);

          er.Position = position1 + (long)this.Offsets[21];
          er.ReadNewArray(out this.TevSwapModes, sectionLengths[21] / 4);

          er.Position = position1 + (long)this.Offsets[22];
          er.ReadNewArray(out this.TevSwapModeTables, sectionLengths[22] / 4);

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

          this.PopulatedMaterials = this.MaterialEntries.Select((entry, index) => new BmdPopulatedMaterial(this, index, entry)).ToArray();
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
    }
  }
}
