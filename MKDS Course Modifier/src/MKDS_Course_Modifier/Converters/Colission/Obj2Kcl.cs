// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.Obj2Kcl
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Converters.Colission
{
  internal class Obj2Kcl
  {
    private static float maxcubesize = -1f;

    public static void ConvertToKcl(string infile, string outfile)
    {
      object[] objArray = Obj2Kcl.read_obj(infile);
      Obj2Kcl.write_kcl(outfile, objArray[0] as List<Triangle>, (Vector3) objArray[1], (Vector3) objArray[2], 50, 128, objArray[3] as string[], objArray[4] as int[]);
    }

    private static object[] read_obj(string filename)
    {
      Vector3 vector3_1 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
      Vector3 vector3_2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
      List<Vector3> vector3List1 = new List<Vector3>();
      List<Triangle> triangleList = new List<Triangle>();
      List<Vector3> vector3List2 = new List<Vector3>();
      List<int> intList = new List<int>();
      List<string> stringList = new List<string>();
      StreamReader streamReader = new StreamReader((Stream) File.OpenRead(filename));
      string str1 = "";
      CultureInfo cultureInfo = new CultureInfo("en-US");
      string str2;
      while ((str2 = streamReader.ReadLine()) != null)
      {
        string str3 = str2.Trim();
        if (str3.Length >= 1 && str3[0] != '#')
        {
          string[] strArray = str3.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length >= 1)
          {
            switch (strArray[0])
            {
              case "usemtl":
                if (strArray.Length >= 2)
                {
                  str1 = strArray[1];
                  if (!stringList.Contains(str1))
                  {
                    stringList.Add(str1);
                    break;
                  }
                  break;
                }
                continue;
              case "v":
                if (strArray.Length >= 4)
                {
                  float x = float.Parse(strArray[1], (IFormatProvider) cultureInfo);
                  float y = float.Parse(strArray[2], (IFormatProvider) cultureInfo);
                  float z = float.Parse(strArray[3], (IFormatProvider) cultureInfo);
                  if ((double) x < (double) vector3_1.X)
                    vector3_1.X = x;
                  if ((double) y < (double) vector3_1.Y)
                    vector3_1.Y = y;
                  if ((double) z < (double) vector3_1.Z)
                    vector3_1.Z = z;
                  if ((double) x > (double) vector3_2.X)
                    vector3_2.X = x;
                  if ((double) y > (double) vector3_2.Y)
                    vector3_2.Y = y;
                  if ((double) z > (double) vector3_2.Z)
                    vector3_2.Z = z;
                  vector3List1.Add(new Vector3(x, y, z));
                  break;
                }
                continue;
              case "vn":
                if (strArray.Length >= 4)
                {
                  float x = float.Parse(strArray[1], (IFormatProvider) cultureInfo);
                  float y = float.Parse(strArray[2], (IFormatProvider) cultureInfo);
                  float z = float.Parse(strArray[3], (IFormatProvider) cultureInfo);
                  vector3List2.Add(new Vector3(x, y, z));
                  break;
                }
                continue;
              case "f":
                if (strArray.Length >= 4)
                {
                  Vector3 u = vector3List1[int.Parse(strArray[1].Split('/')[0]) - 1];
                  Vector3 v = vector3List1[int.Parse(strArray[2].Split('/')[0]) - 1];
                  Vector3 w = vector3List1[int.Parse(strArray[3].Split('/')[0]) - 1];
                  if (strArray[1].Split('/').Length == 2)
                  {
                    Vector3 n = vector3List2[int.Parse(strArray[1].Split('/')[2]) - 1];
                    if ((double) Helpers.norm_sq(Vector3.Cross(v - u, w - u)) >= 0.01)
                    {
                      triangleList.Add(new Triangle(u, v, w, n));
                      intList.Add(stringList.IndexOf(str1));
                      break;
                    }
                    continue;
                  }
                  if ((double) Helpers.norm_sq(Vector3.Cross(v - u, w - u)) >= 0.01)
                  {
                    triangleList.Add(new Triangle(u, v, w));
                    intList.Add(stringList.IndexOf(str1));
                    break;
                  }
                  continue;
                }
                continue;
            }
          }
        }
      }
      streamReader.Close();
      return new object[5]
      {
        (object) triangleList,
        (object) vector3_1,
        (object) vector3_2,
        (object) stringList.ToArray(),
        (object) intList.ToArray()
      };
    }

    private static void write_kcl(
      string filename,
      List<Triangle> triangles,
      Vector3 bb_min,
      Vector3 bb_max,
      int max_triangles,
      int min_width,
      string[] names,
      int[] materialu)
    {
      bb_min -= new Vector3(25f, 25f, 25f);
      bb_max += new Vector3(25f, 25f, 25f);
      kclType kclType = new kclType(names);
      kclType.DialogResult = DialogResult.None;
      int num1 = (int) kclType.ShowDialog();
      do
        ;
      while (kclType.DialogResult != DialogResult.OK);
      Dictionary<string, ushort> mapping = kclType.Mapping;
      Dictionary<string, bool> colli = kclType.Colli;
      int val2 = Helpers.next_exponent((float) min_width, 2);
      int num2 = Math.Max(Helpers.next_exponent(bb_max.X - bb_min.X, 2), val2);
      int num3 = Math.Max(Helpers.next_exponent(bb_max.Y - bb_min.Y, 2), val2);
      int num4 = Math.Max(Helpers.next_exponent(bb_max.Z - bb_min.Z, 2), val2);
      int num5 = (int) Helpers.max((float) num2, (float) num4);
      int num6 = num5;
      int num7 = Math.Max((int) Helpers.min((float) num5, (float) num3, (float) num6), val2);
      float num8 = (float) Math.Pow(2.0, (double) (num5 - num7));
      float num9 = (float) Math.Pow(2.0, (double) (num3 - num7));
      float num10 = (float) Math.Pow(2.0, (double) (num6 - num7));
      float width = (float) Math.Pow(2.0, (double) num7);
      Obj2Kcl.maxcubesize = width;
      List<OctreeNode> octreeNodeList = new List<OctreeNode>();
      List<int> indices = new List<int>();
      int index1 = 0;
      foreach (Triangle triangle in triangles)
      {
        if (colli[names[materialu[index1]]])
          indices.Add(index1);
        ++index1;
      }
      for (int index2 = 0; (double) index2 < (double) num10; ++index2)
      {
        for (int index3 = 0; (double) index3 < (double) num9; ++index3)
        {
          for (int index4 = 0; (double) index4 < (double) num8; ++index4)
            octreeNodeList.Add(new OctreeNode(bb_min + width * new Vector3((float) index4, (float) index3, (float) index2), width, triangles, indices, max_triangles, min_width));
        }
      }
      EndianBinaryWriter endianBinaryWriter = new EndianBinaryWriter((Stream) File.Create(filename), Endianness.LittleEndian);
      endianBinaryWriter.Write(0U);
      endianBinaryWriter.Write(0U);
      endianBinaryWriter.Write(0U);
      endianBinaryWriter.Write(0U);
      endianBinaryWriter.Write(122880U);
      endianBinaryWriter.Write((int) ((double) bb_min.X * 4096.0));
      endianBinaryWriter.Write((int) ((double) bb_min.Y * 4096.0));
      endianBinaryWriter.Write((int) ((double) bb_min.Z * 4096.0));
      endianBinaryWriter.Write((uint) (-1 << num5 & -1));
      endianBinaryWriter.Write((uint) (-1 << num3 & -1));
      endianBinaryWriter.Write((uint) (-1 << num6 & -1));
      endianBinaryWriter.Write((uint) num7);
      endianBinaryWriter.Write((uint) (num5 - num7));
      endianBinaryWriter.Write((uint) (num5 - num7 + num3 - num7));
      endianBinaryWriter.Write(102400U);
      int position1 = (int) endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = 0L;
      endianBinaryWriter.Write(position1);
      endianBinaryWriter.BaseStream.Position = (long) position1;
      List<Vector3> b1 = new List<Vector3>();
      List<int> intList1 = new List<int>();
      int index5 = 0;
      foreach (Triangle triangle in triangles)
      {
        if (colli[names[materialu[index5]]])
        {
          int num11 = Helpers.containsVector3(triangle.u, b1);
          if (num11 == -1)
          {
            b1.Add(triangle.u);
            intList1.Add(b1.Count - 1);
          }
          else
            intList1.Add(num11);
        }
        ++index5;
      }
      foreach (Vector3 vector3 in b1)
      {
        endianBinaryWriter.Write((int) ((double) vector3.X * 4096.0));
        endianBinaryWriter.Write((int) ((double) vector3.Y * 4096.0));
        endianBinaryWriter.Write((int) ((double) vector3.Z * 4096.0));
      }
      int position2 = (int) endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = 4L;
      endianBinaryWriter.Write(position2);
      endianBinaryWriter.BaseStream.Position = (long) position2;
      List<Vector3> b2 = new List<Vector3>();
      List<int> intList2 = new List<int>();
      int index6 = 0;
      foreach (Triangle triangle in triangles)
      {
        if (colli[names[materialu[index6]]])
        {
          Vector3 a1 = -Helpers.unit(Vector3.Cross(triangle.w - triangle.u, triangle.n));
          Vector3 a2 = Helpers.unit(Vector3.Cross(triangle.v - triangle.u, triangle.n));
          Vector3 a3 = Helpers.unit(Vector3.Cross(triangle.w - triangle.v, triangle.n));
          int num11 = Helpers.containsVector3(triangle.n, b2);
          if (num11 == -1)
          {
            b2.Add(triangle.n);
            intList2.Add(b2.Count - 1);
          }
          else
            intList2.Add(num11);
          int num12 = Helpers.containsVector3(a1, b2);
          if (num12 == -1)
          {
            b2.Add(a1);
            intList2.Add(b2.Count - 1);
          }
          else
            intList2.Add(num12);
          int num13 = Helpers.containsVector3(a2, b2);
          if (num13 == -1)
          {
            b2.Add(a2);
            intList2.Add(b2.Count - 1);
          }
          else
            intList2.Add(num13);
          int num14 = Helpers.containsVector3(a3, b2);
          if (num14 == -1)
          {
            b2.Add(a3);
            intList2.Add(b2.Count - 1);
          }
          else
            intList2.Add(num14);
        }
        ++index6;
      }
      foreach (Vector3 vector3 in b2)
      {
        endianBinaryWriter.Write((short) ((double) vector3.X * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3.Y * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3.Z * 4096.0));
      }
      while (endianBinaryWriter.BaseStream.Position % 4L != 0L)
        endianBinaryWriter.Write((byte) 0);
      int position3 = (int) endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = 8L;
      endianBinaryWriter.Write(position3 - 16);
      endianBinaryWriter.BaseStream.Position = (long) position3;
      int index7 = 0;
      int index8 = 0;
      foreach (Triangle triangle in triangles)
      {
        if (colli[names[materialu[index8]]])
        {
          Vector3 right = Helpers.unit(Vector3.Cross(triangle.w - triangle.v, triangle.n));
          endianBinaryWriter.Write((int) ((double) Vector3.Dot(triangle.w - triangle.u, right) * 4096.0));
          endianBinaryWriter.Write((ushort) intList1[index7]);
          endianBinaryWriter.Write((ushort) intList2[4 * index7]);
          endianBinaryWriter.Write((ushort) intList2[4 * index7 + 1]);
          endianBinaryWriter.Write((ushort) intList2[4 * index7 + 2]);
          endianBinaryWriter.Write((ushort) intList2[4 * index7 + 3]);
          endianBinaryWriter.Write(Bytes.StringToByte(string.Format("{0:X4}", (object) mapping[names[materialu[index8]]])), 0, 2);
          ++index7;
        }
        ++index8;
      }
      int position4 = (int) endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.BaseStream.Position = 12L;
      endianBinaryWriter.Write(position4);
      endianBinaryWriter.BaseStream.Position = (long) position4;
      long position5 = endianBinaryWriter.BaseStream.Position;
      long position6;
      long offset = position6 = endianBinaryWriter.BaseStream.Position;
      Queue<OctreeNode> octreeNodeQueue = new Queue<OctreeNode>();
      Collection<ushort> collection = new Collection<ushort>();
      Queue<long> longQueue = new Queue<long>();
      for (int index2 = 0; index2 < octreeNodeList.Count; ++index2)
      {
        longQueue.Enqueue(position6);
        octreeNodeQueue.Enqueue(octreeNodeList[index2]);
      }
      long num15 = position6 + (long) (octreeNodeList.Count * 4);
      int num16 = 0;
      int count = octreeNodeList.Count;
      while (octreeNodeQueue.Count > 0)
      {
        OctreeNode octreeNode = octreeNodeQueue.Dequeue();
        if (octreeNode.is_leaf)
        {
          endianBinaryWriter.Write(0);
          longQueue.Dequeue();
        }
        else
        {
          endianBinaryWriter.Write((int) (num15 - longQueue.Dequeue()));
          for (int index2 = 0; index2 < octreeNode.branches.Count; ++index2)
          {
            octreeNodeQueue.Enqueue(octreeNode.branches[index2]);
            longQueue.Enqueue(num15);
            ++count;
          }
          num15 += 32L;
        }
        ++num16;
      }
      long position7 = endianBinaryWriter.BaseStream.Position;
      long num17 = offset;
      endianBinaryWriter.BaseStream.Seek(offset, SeekOrigin.Begin);
      for (int index2 = 0; index2 < octreeNodeList.Count; ++index2)
      {
        longQueue.Enqueue(num17);
        octreeNodeQueue.Enqueue(octreeNodeList[index2]);
      }
      long num18 = num17 + (long) (octreeNodeList.Count * 4);
      int num19 = 0;
      while (octreeNodeQueue.Count > 0)
      {
        long position8 = endianBinaryWriter.BaseStream.Position;
        OctreeNode octreeNode = octreeNodeQueue.Dequeue();
        if (octreeNode.is_leaf)
        {
          endianBinaryWriter.Write((int) ((long) int.MinValue | endianBinaryWriter.BaseStream.Length - longQueue.Dequeue() - 2L));
          endianBinaryWriter.BaseStream.Seek(0L, SeekOrigin.End);
          for (int index2 = 0; index2 < octreeNode.indices.Count; ++index2)
            endianBinaryWriter.Write((ushort) (octreeNode.indices[index2] + 1));
          endianBinaryWriter.Write((ushort) 0);
          for (int index2 = 0; index2 < octreeNode.indices.Count; ++index2)
            collection.Add((ushort) (octreeNode.indices[index2] + 1));
          collection.Add((ushort) 0);
          endianBinaryWriter.BaseStream.Seek(position8 + 4L, SeekOrigin.Begin);
        }
        else
        {
          longQueue.Dequeue();
          endianBinaryWriter.BaseStream.Seek(4L, SeekOrigin.Current);
          for (int index2 = 0; index2 < octreeNode.branches.Count; ++index2)
          {
            octreeNodeQueue.Enqueue(octreeNode.branches[index2]);
            longQueue.Enqueue(num18);
          }
          num18 += 32L;
        }
        ++num19;
      }
      endianBinaryWriter.Close();
    }
  }
}
