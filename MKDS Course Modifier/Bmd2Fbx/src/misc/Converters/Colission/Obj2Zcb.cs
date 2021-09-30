// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.Obj2Zcb
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Converters.Colission
{
  internal class Obj2Zcb
  {
    public static void ConvertToZcb(string infile, string outfile)
    {
      object[] objArray = Obj2Zcb.read_obj(infile);
      Obj2Zcb.write_zcb(outfile, objArray[0] as List<Triangle>, (Vector3) objArray[1], (Vector3) objArray[2], 256, objArray[3] as string[], objArray[4] as int[]);
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

    private static void write_zcb(
      string filename,
      List<Triangle> triangles,
      Vector3 bb_min,
      Vector3 bb_max,
      int start_width,
      string[] names,
      int[] materialu)
    {
      zcbType zcbType = new zcbType(names);
      zcbType.DialogResult = DialogResult.None;
      int num1 = (int) zcbType.ShowDialog();
      do
        ;
      while (zcbType.DialogResult != DialogResult.OK);
      Dictionary<string, uint> mapping = zcbType.Mapping;
      Dictionary<string, bool> colli = zcbType.Colli;
      EndianBinaryWriter endianBinaryWriter = new EndianBinaryWriter((Stream) File.Create(filename), Endianness.LittleEndian);
      endianBinaryWriter.Write("BLCM1BCZ", Encoding.ASCII, false);
      endianBinaryWriter.Write(0U);
      endianBinaryWriter.Write(5U);
      endianBinaryWriter.Write(new byte[16]
      {
        (byte) 4,
        (byte) 3,
        (byte) 2,
        (byte) 1,
        (byte) 4,
        (byte) 3,
        (byte) 2,
        (byte) 1,
        (byte) 4,
        (byte) 3,
        (byte) 2,
        (byte) 1,
        (byte) 4,
        (byte) 3,
        (byte) 2,
        (byte) 1
      }, 0, 16);
      endianBinaryWriter.Write("BXTV", Encoding.ASCII, false);
      endianBinaryWriter.Write((uint) (triangles.Count * 12 * 3 + 12));
      endianBinaryWriter.Write((ushort) (triangles.Count * 3));
      endianBinaryWriter.Write(new byte[2]
      {
        (byte) 2,
        (byte) 1
      }, 0, 2);
      foreach (Triangle triangle in triangles)
      {
        endianBinaryWriter.Write((int) ((double) triangle.u.X * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.u.Y * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.u.Z * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.v.X * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.v.Y * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.v.Z * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.w.X * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.w.Y * 4096.0));
        endianBinaryWriter.Write((int) ((double) triangle.w.Z * 4096.0));
      }
      endianBinaryWriter.Write("BLCP", Encoding.ASCII, false);
      endianBinaryWriter.Write((uint) (4 * names.Length + 12));
      endianBinaryWriter.Write((ushort) names.Length);
      endianBinaryWriter.Write(new byte[2]
      {
        (byte) 2,
        (byte) 1
      }, 0, 2);
      for (int index = 0; index < names.Length; ++index)
        endianBinaryWriter.Write(Bytes.StringToByte(string.Format("{0:X8}", (object) mapping[names[index]])), 0, 4);
      endianBinaryWriter.Write("BMRN", Encoding.ASCII, false);
      endianBinaryWriter.Write((uint) (triangles.Count * 6 * 4 + 12));
      endianBinaryWriter.Write((ushort) (triangles.Count * 4));
      endianBinaryWriter.Write(new byte[2]
      {
        (byte) 2,
        (byte) 1
      }, 0, 2);
      foreach (Triangle triangle in triangles)
      {
        endianBinaryWriter.Write((short) ((double) triangle.n.X * 4096.0));
        endianBinaryWriter.Write((short) ((double) triangle.n.Y * 4096.0));
        endianBinaryWriter.Write((short) ((double) triangle.n.Z * 4096.0));
        Vector3 vector3_1 = -Helpers.unit(Vector3.Cross(triangle.w - triangle.u, triangle.n));
        Vector3 vector3_2 = Helpers.unit(Vector3.Cross(triangle.v - triangle.u, triangle.n));
        Vector3 vector3_3 = Helpers.unit(Vector3.Cross(triangle.w - triangle.v, triangle.n));
        endianBinaryWriter.Write((short) ((double) vector3_1.X * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_1.Y * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_1.Z * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_2.X * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_2.Y * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_2.Z * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_3.X * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_3.Y * 4096.0));
        endianBinaryWriter.Write((short) ((double) vector3_3.Z * 4096.0));
      }
      endianBinaryWriter.Write("BIRT", Encoding.ASCII, false);
      endianBinaryWriter.Write((uint) (triangles.Count * 16 + 12));
      endianBinaryWriter.Write((ushort) triangles.Count);
      endianBinaryWriter.Write(new byte[2]
      {
        (byte) 2,
        (byte) 1
      }, 0, 2);
      int num2 = 0;
      int num3 = 0;
      Triangle triangle1;
      foreach (Triangle triangle2 in triangles)
      {
        triangle1 = triangle2;
        endianBinaryWriter.Write((short) num2);
        int num4 = num2 + 1;
        endianBinaryWriter.Write((short) num4);
        int num5 = num4 + 1;
        endianBinaryWriter.Write((short) num5);
        num2 = num5 + 1;
        endianBinaryWriter.Write((short) materialu[num2 / 3 - 1]);
        endianBinaryWriter.Write((short) num3);
        int num6 = num3 + 1;
        endianBinaryWriter.Write((short) num6);
        int num7 = num6 + 1;
        endianBinaryWriter.Write((short) num7);
        int num8 = num7 + 1;
        endianBinaryWriter.Write((short) num8);
        num3 = num8 + 1;
      }
      List<OctreeNode> octreeNodeList = new List<OctreeNode>();
      List<int> intList = new List<int>();
      int num9 = 0;
      foreach (Triangle triangle2 in triangles)
      {
        triangle1 = triangle2;
        intList.Add(num9);
        ++num9;
      }
      long position = endianBinaryWriter.BaseStream.Position;
      endianBinaryWriter.Write("BDRG", Encoding.ASCII, false);
      endianBinaryWriter.Write(0U);
      endianBinaryWriter.Write((ushort) 1);
      endianBinaryWriter.Write((ushort) 1);
      for (int index1 = 0; index1 < 1; ++index1)
      {
        for (int index2 = 0; index2 < 1; ++index2)
        {
          endianBinaryWriter.Write((short) triangles.Count);
          for (int index3 = 0; index3 < triangles.Count; ++index3)
            endianBinaryWriter.Write((short) index3);
          endianBinaryWriter.Write((short) 0);
        }
      }
      endianBinaryWriter.BaseStream.Position = position + 4L;
      endianBinaryWriter.Write((uint) (endianBinaryWriter.BaseStream.Length - position));
      endianBinaryWriter.BaseStream.Position = 8L;
      endianBinaryWriter.Write((uint) endianBinaryWriter.BaseStream.Length);
      endianBinaryWriter.Close();
    }
  }
}
