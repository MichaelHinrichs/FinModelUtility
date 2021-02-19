// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MKDS.KCL
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.MKDS
{
  public class KCL
  {
    public KCL.KCLHeader Header;
    public Vector3[] Vertex;
    public Vector3[] Normals;
    public KCL.Plane[] Planes;
    public KCL.OctreeNode Octree;

    public KCL(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.Header = new KCL.KCLHeader(er);
      er.BaseStream.Position = (long) this.Header.VertexOffset;
      this.Vertex = new Vector3[((this.Header.NormalOffset - this.Header.VertexOffset) / 12U)];
      for (int index = 0; (long) index < (long) ((this.Header.NormalOffset - this.Header.VertexOffset) / 12U); ++index)
        this.Vertex[index] = new Vector3(er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12(), er.ReadSingleInt32Exp12());
      er.BaseStream.Position = (long) this.Header.NormalOffset;
      this.Normals = new Vector3[((this.Header.PlaneOffset + 16U - this.Header.NormalOffset) / 6U)];
      for (int index = 0; (long) index < (long) ((this.Header.PlaneOffset + 16U - this.Header.NormalOffset) / 6U); ++index)
        this.Normals[index] = new Vector3(er.ReadSingleInt16Exp12(), er.ReadSingleInt16Exp12(), er.ReadSingleInt16Exp12());
      er.BaseStream.Position = (long) (this.Header.PlaneOffset + 16U);
      this.Planes = new KCL.Plane[((this.Header.OctreeOffset - (this.Header.PlaneOffset + 16U)) / 16U)];
      for (int index = 0; (long) index < (long) ((this.Header.OctreeOffset - (this.Header.PlaneOffset + 16U)) / 16U); ++index)
        this.Planes[index] = new KCL.Plane(er);
      er.BaseStream.Position = (long) this.Header.OctreeOffset;
      int NrNode = ((~this.Header.Xmask >> this.Header.CoordinateShift) + 1) * ((~this.Header.Ymask >> this.Header.CoordinateShift) + 1) * ((~this.Header.Zmask >> this.Header.CoordinateShift) + 1);
      this.Octree = new KCL.OctreeNode(er, NrNode);
      er.Close();
    }

    private Color GetColor(ushort ID)
    {
      int num1 = (int) ID & 7;
      int num2 = (int) ID >> 3 & 3;
      int num3 = (int) ID >> 5 & 7;
      int num4 = (int) ID >> 8 & 31;
      int num5 = (int) ID >> 13 & 7;
      switch (num4)
      {
        case 0:
          switch (num3)
          {
            case 0:
              return Color.Gray;
            case 1:
              return Color.PaleGoldenrod;
            case 2:
              return Color.Gray;
            case 4:
              return ColorTranslator.FromHtml("#964B00");
            case 5:
              return Color.Snow;
            case 6:
              return Color.LightGray;
            case 7:
              return Color.Gray;
          }
          break;
        case 1:
          switch (num3)
          {
            case 0:
              switch (num5)
              {
                case 0:
                  return Color.LightSkyBlue;
                case 4:
                  return Color.PaleGoldenrod;
              }
              break;
            case 1:
              return Color.PaleGoldenrod;
            case 3:
              return Color.SkyBlue;
            case 4:
              return Color.WhiteSmoke;
          }
          break;
        case 2:
          switch (num3)
          {
            case 2:
              return Color.GhostWhite;
            case 3:
              return ColorTranslator.FromHtml("#6F4E37");
          }
          break;
        case 3:
          switch (num3)
          {
            case 0:
              return Color.Gold;
            case 1:
              return ColorTranslator.FromHtml("#6F4E37");
            case 2:
              return Color.Green;
            case 3:
              return Color.Gold;
            case 4:
              return Color.PapayaWhip;
            case 5:
              return Color.Snow;
          }
          break;
        case 5:
          switch (num3)
          {
            case 3:
              return Color.Gold;
            case 4:
              return Color.GreenYellow;
          }
          break;
        case 6:
          if (num3 == 1)
            return ColorTranslator.FromHtml("#6F4E37");
          break;
        case 7:
          if (num3 == 0)
            return Color.Red;
          break;
        case 8:
          switch (num3)
          {
            case 0:
              return Color.Orange;
            case 1:
              return ColorTranslator.FromHtml("#B06500");
            case 2:
              return Color.Silver;
            case 3:
              return ColorTranslator.FromHtml("#964B00");
            case 7:
              return Color.DeepSkyBlue;
          }
          break;
        case 10:
          switch (num3)
          {
            case 0:
              return Color.FromArgb((int) byte.MaxValue, Color.Black);
            case 1:
              return Color.DarkGoldenrod;
            case 3:
              return Color.FromArgb((int) byte.MaxValue, Color.Snow);
            case 4:
              return Color.DarkSlateBlue;
          }
          break;
        case 11:
          switch (num3)
          {
            case 0:
              return Color.FromArgb(0, Color.Black);
            case 1:
              return Color.DarkSlateBlue;
            case 2:
              return Color.OrangeRed;
          }
          break;
        case 12:
          switch (num3)
          {
            case 1:
              return Color.IndianRed;
            case 2:
              return Color.IndianRed;
            case 3:
              return Color.IndianRed;
          }
          break;
        case 15:
          return Color.FromArgb((int) byte.MaxValue, 0, (int) sbyte.MaxValue);
        case 17:
          return Color.SkyBlue;
        case 18:
          if (num3 == 0)
            return Color.Red;
          break;
        case 19:
          switch (num3)
          {
            case 0:
              return Color.DarkRed;
            case 1:
              return Color.DarkRed;
          }
          break;
        case 20:
          if (num3 == 3)
            return Color.ForestGreen;
          break;
        case 21:
          if (num3 == 4)
            return Color.DarkGreen;
          break;
      }
      return Color.Purple;
    }

    public void Render(bool picking = false)
    {
      int PlaneIdx = 0;
      foreach (KCL.Plane plane in this.Planes)
      {
        Vector3 PositionA;
        Vector3 PositionB;
        Vector3 PositionC;
        Vector3 Normal;
        KCL.GetTriangle(this, PlaneIdx, out PositionA, out PositionB, out PositionC, out Normal);
        Color color = this.GetColor(plane.Type);
        if (picking && color.A != (byte) 0)
          color = Color.FromArgb(PlaneIdx + 1 | -16777216);
        Gl.glColor4f((float) color.R / (float) byte.MaxValue, (float) color.G / (float) byte.MaxValue, (float) color.B / (float) byte.MaxValue, (float) color.A / (float) byte.MaxValue);
        Gl.glBegin(4);
        Gl.glNormal3f(Normal.X, Normal.Y, Normal.Z);
        Gl.glVertex3f(PositionA.X, PositionA.Z, PositionA.Y);
        Gl.glVertex3f(PositionB.X, PositionB.Z, PositionB.Y);
        Gl.glVertex3f(PositionC.X, PositionC.Z, PositionC.Y);
        Gl.glEnd();
        ++PlaneIdx;
      }
    }

    public static void GetTriangle(
      KCL k,
      int PlaneIdx,
      out Vector3 PositionA,
      out Vector3 PositionB,
      out Vector3 PositionC)
    {
      KCL.GetTriangle(k, PlaneIdx, out PositionA, out PositionB, out PositionC, out Vector3 _);
    }

    public static void GetTriangle(
      KCL k,
      int PlaneIdx,
      out Vector3 PositionA,
      out Vector3 PositionB,
      out Vector3 PositionC,
      out Vector3 Normal)
    {
      KCL.Plane plane = k.Planes[PlaneIdx];
      PositionA = k.Vertex[(int) plane.VertexIndex];
      Vector3 right = Normal = k.Normals[(int) plane.NormalIndex];
      Vector3 normal1 = k.Normals[(int) plane.NormalAIndex];
      Vector3 normal2 = k.Normals[(int) plane.NormalBIndex];
      Vector3 normal3 = k.Normals[(int) plane.NormalCIndex];
      Vector3 left1 = Vector3.Cross(normal1, right);
      Vector3 left2 = Vector3.Cross(normal2, right);
      PositionB = PositionA + left2 * (plane.Length / Vector3.Dot(left2, normal3));
      PositionC = PositionA + left1 * (plane.Length / Vector3.Dot(left1, normal3));
    }

    private static bool MKDSCollisionCheck(KCL k, Vector3 Point)
    {
      int num1 = (int) ((double) Point.X - (double) k.Header.SpatialGridX);
      if ((num1 & k.Header.Xmask) != 0)
        return false;
      int num2 = (int) ((double) Point.Y - (double) k.Header.SpatialGridY);
      if ((num2 & k.Header.Ymask) != 0)
        return false;
      int num3 = (int) ((double) Point.Z - (double) k.Header.SpatialGridZ);
      if ((num3 & k.Header.Zmask) != 0)
        return false;
      int index1 = num3 >> k.Header.CoordinateShift << k.Header.ZShift | num2 >> k.Header.CoordinateShift << k.Header.YShift | num1 >> k.Header.CoordinateShift;
      int coordinateShift = k.Header.CoordinateShift;
      int index2;
      for (KCL.OctreeNode childNode = k.Octree.ChildNodes[index1]; !childNode.Leaf; childNode = childNode.ChildNodes[index2])
      {
        --coordinateShift;
        index2 = (num3 >> coordinateShift & 1) << 2 | (num2 >> coordinateShift & 1) << 1 | num1 >> coordinateShift & 1;
      }
      return false;
    }

    public class KCLHeader
    {
      public uint VertexOffset;
      public uint NormalOffset;
      public uint PlaneOffset;
      public uint OctreeOffset;
      public uint Unknown1;
      public float SpatialGridX;
      public float SpatialGridY;
      public float SpatialGridZ;
      public int Xmask;
      public int Ymask;
      public int Zmask;
      public int CoordinateShift;
      public int YShift;
      public int ZShift;
      public uint Unknown2;

      public KCLHeader(EndianBinaryReader er)
      {
        this.VertexOffset = er.ReadUInt32();
        this.NormalOffset = er.ReadUInt32();
        this.PlaneOffset = er.ReadUInt32();
        this.OctreeOffset = er.ReadUInt32();
        this.Unknown1 = er.ReadUInt32();
        this.SpatialGridX = er.ReadSingleInt32Exp12();
        this.SpatialGridY = er.ReadSingleInt32Exp12();
        this.SpatialGridZ = er.ReadSingleInt32Exp12();
        this.Xmask = er.ReadInt32();
        this.Ymask = er.ReadInt32();
        this.Zmask = er.ReadInt32();
        this.CoordinateShift = er.ReadInt32();
        this.YShift = er.ReadInt32();
        this.ZShift = er.ReadInt32();
        this.Unknown2 = er.ReadUInt32();
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write(this.VertexOffset);
        er.Write(this.NormalOffset);
        er.Write(this.PlaneOffset);
        er.Write(this.OctreeOffset);
        er.Write(this.Unknown1);
        er.Write((int) ((double) this.SpatialGridX * 4096.0));
        er.Write((int) ((double) this.SpatialGridY * 4096.0));
        er.Write((int) ((double) this.SpatialGridZ * 4096.0));
        er.Write(this.Xmask);
        er.Write(this.Ymask);
        er.Write(this.Zmask);
        er.Write(this.CoordinateShift);
        er.Write(this.YShift);
        er.Write(this.ZShift);
        er.Write(this.Unknown2);
      }
    }

    public class Plane
    {
      public float Length;
      public ushort VertexIndex;
      public ushort NormalIndex;
      public ushort NormalAIndex;
      public ushort NormalBIndex;
      public ushort NormalCIndex;
      public ushort Type;

      public Plane(EndianBinaryReader er)
      {
        this.Length = er.ReadSingleInt32Exp12();
        this.VertexIndex = er.ReadUInt16();
        this.NormalIndex = er.ReadUInt16();
        this.NormalAIndex = er.ReadUInt16();
        this.NormalBIndex = er.ReadUInt16();
        this.NormalCIndex = er.ReadUInt16();
        this.Type = er.ReadUInt16();
      }

      public void Write(EndianBinaryWriter er)
      {
        er.Write((int) ((double) this.Length * 4096.0));
        er.Write(this.VertexIndex);
        er.Write(this.NormalIndex);
        er.Write(this.NormalAIndex);
        er.Write(this.NormalBIndex);
        er.Write(this.NormalCIndex);
        er.Write(this.Type);
      }
    }

    public class OctreeNode
    {
      public List<KCL.OctreeNode> ChildNodes;
      public List<ushort> Planes;
      public bool Leaf;

      public OctreeNode(EndianBinaryReader er, int NrNode)
      {
        this.ChildNodes = new List<KCL.OctreeNode>();
        this.Planes = new List<ushort>();
        long position = er.BaseStream.Position;
        for (int index = 0; index < NrNode; ++index)
          this.ChildNodes.Add(new KCL.OctreeNode(er, position));
      }

      public OctreeNode(EndianBinaryReader er, long BaseOffset)
      {
        long position = er.BaseStream.Position;
        int num1 = er.ReadInt32();
        this.Leaf = num1 < 0;
        int num2 = num1 & int.MaxValue;
        this.ChildNodes = new List<KCL.OctreeNode>();
        this.Planes = new List<ushort>();
        er.BaseStream.Position = BaseOffset + (long) num2;
        if (this.Leaf)
        {
          er.BaseStream.Position += 2L;
          ushort num3;
          do
          {
            num3 = er.ReadUInt16();
            if (num3 != (ushort) 0)
              this.Planes.Add((ushort) ((uint) num3 - 1U));
          }
          while (num3 != (ushort) 0);
        }
        else
        {
          for (int index = 0; index < 8; ++index)
            this.ChildNodes.Add(new KCL.OctreeNode(er, BaseOffset + (long) num2));
        }
        er.BaseStream.Position = position + 4L;
      }
    }
  }
}
