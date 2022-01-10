// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.OBJ
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

#pragma warning disable CS8603

namespace bmd._3D_Formats
{
  public class OBJ
  {
    public string MLTName = (string) null;
    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> Normals = new List<Vector3>();
    public List<Vector2> TexCoords = new List<Vector2>();
    public List<Color> VertexColors = new List<Color>();
    public List<OBJ.Face> Faces = new List<OBJ.Face>();

    public OBJ(string file)
    {
      StreamReader streamReader = new StreamReader((Stream) File.OpenRead(file));
      string str1 = "";
      CultureInfo cultureInfo = new CultureInfo("en-US");
      string str2;
      while ((str2 = streamReader.ReadLine()) != null)
      {
        string str3 = str2.Trim();
        if (str3.Length >= 1 && str3[0] != '#')
        {
          string[] strArray1 = str3.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray1.Length >= 1)
          {
            switch (strArray1[0])
            {
              case "mtllib":
                this.MLTName = Path.GetDirectoryName(file) + "\\" + str3.Substring(strArray1[0].Length + 1).Trim();
                break;
              case "usemtl":
                if (strArray1.Length >= 2)
                {
                  str1 = strArray1[1];
                  break;
                }
                continue;
              case "v":
                if (strArray1.Length >= 4)
                {
                  this.Vertices.Add(new Vector3(float.Parse(strArray1[1], (IFormatProvider) cultureInfo), float.Parse(strArray1[2], (IFormatProvider) cultureInfo), float.Parse(strArray1[3], (IFormatProvider) cultureInfo)));
                  break;
                }
                continue;
              case "vt":
                if (strArray1.Length >= 2)
                {
                  this.TexCoords.Add(new Vector2(float.Parse(strArray1[1], (IFormatProvider) cultureInfo), strArray1.Length < 3 ? 0.0f : float.Parse(strArray1[2], (IFormatProvider) cultureInfo)));
                  break;
                }
                continue;
              case "vn":
                if (strArray1.Length >= 4)
                {
                  this.Normals.Add(new Vector3(float.Parse(strArray1[1], (IFormatProvider) cultureInfo), float.Parse(strArray1[2], (IFormatProvider) cultureInfo), float.Parse(strArray1[3], (IFormatProvider) cultureInfo)));
                  break;
                }
                continue;
              case "vc":
                if (strArray1.Length >= 4)
                {
                  this.VertexColors.Add(Color.FromArgb((int) ((double) float.Parse(strArray1[1], (IFormatProvider) cultureInfo) * (double) byte.MaxValue), (int) ((double) float.Parse(strArray1[2], (IFormatProvider) cultureInfo) * (double) byte.MaxValue), (int) ((double) float.Parse(strArray1[3], (IFormatProvider) cultureInfo) * (double) byte.MaxValue)));
                  break;
                }
                continue;
              case "f":
                if (strArray1.Length >= 4)
                {
                  OBJ.Face face = new OBJ.Face();
                  face.MaterialName = str1;
                  for (int index = 0; index < strArray1.Length - 1; ++index)
                  {
                    string[] strArray2 = strArray1[index + 1].Split('/');
                    face.VertexIndieces.Add(int.Parse(strArray2[0]) - 1);
                    if (strArray2[1] != "")
                      face.TexCoordIndieces.Add(int.Parse(strArray2[1]) - 1);
                    if (strArray2.Length > 2 && strArray2[2] != "")
                      face.NormalIndieces.Add(int.Parse(strArray2[2]) - 1);
                    if (strArray2.Length > 3)
                      face.VertexColorIndieces.Add(int.Parse(strArray2[3]) - 1);
                  }
                  this.Faces.Add(face);
                  break;
                }
                continue;
            }
          }
        }
      }
      streamReader.Close();
    }

    public OBJ()
    {
    }

    public void Write(string Obj)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine("# Created by MKDS Course Modifer");
      stringBuilder1.AppendLine();
      if (this.MLTName != null)
        stringBuilder1.AppendFormat("mtllib {0}\n", (object) (Path.GetFileNameWithoutExtension(Obj) + ".mtl"));
      stringBuilder1.AppendLine();
      foreach (Vector3 vertex in this.Vertices)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        float num = vertex.X;
        string str1 = num.ToString().Replace(",", ".");
        num = vertex.Y;
        string str2 = num.ToString().Replace(",", ".");
        num = vertex.Z;
        string str3 = num.ToString().Replace(",", ".");
        stringBuilder2.AppendFormat("v {0} {1} {2}\n", (object) str1, (object) str2, (object) str3);
      }
      stringBuilder1.AppendLine();
      foreach (Vector2 texCoord in this.TexCoords)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        float num = texCoord.X;
        string str1 = num.ToString().Replace(",", ".");
        num = texCoord.Y;
        string str2 = num.ToString().Replace(",", ".");
        stringBuilder2.AppendFormat("vt {0} {1}\n", (object) str1, (object) str2);
      }
      stringBuilder1.AppendLine();
      foreach (Vector3 normal in this.Normals)
      {
        StringBuilder stringBuilder2 = stringBuilder1;
        float num = normal.X;
        string str1 = num.ToString().Replace(",", ".");
        num = normal.Y;
        string str2 = num.ToString().Replace(",", ".");
        num = normal.Z;
        string str3 = num.ToString().Replace(",", ".");
        stringBuilder2.AppendFormat("vn {0} {1} {2}\n", (object) str1, (object) str2, (object) str3);
      }
      stringBuilder1.AppendLine();
      foreach (Color vertexColor in this.VertexColors)
        stringBuilder1.AppendFormat("vc {0} {1} {2}\n", (object) ((float) vertexColor.R / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) vertexColor.G / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) vertexColor.B / (float) byte.MaxValue).ToString().Replace(",", "."));
      stringBuilder1.AppendLine();
      string str = "";
      foreach (OBJ.Face face in this.Faces)
      {
        if (str != face.MaterialName)
        {
          stringBuilder1.AppendFormat("usemtl {0}\n", (object) face.MaterialName);
          stringBuilder1.AppendLine();
          str = face.MaterialName;
        }
        bool flag1 = face.VertexIndieces.Count != 0;
        bool flag2 = face.NormalIndieces.Count != 0;
        bool flag3 = face.TexCoordIndieces.Count != 0;
        bool flag4 = face.VertexColorIndieces.Count != 0;
        if (flag1 && flag2 && flag3 && flag4)
          stringBuilder1.AppendFormat("f {0}/{1}/{2}/{9} {3}/{4}/{5}/{10} {6}/{7}/{8}/{11}\n", (object) (face.VertexIndieces[0] + 1), (object) (face.TexCoordIndieces[0] + 1), (object) (face.NormalIndieces[0] + 1), (object) (face.VertexIndieces[1] + 1), (object) (face.TexCoordIndieces[1] + 1), (object) (face.NormalIndieces[1] + 1), (object) (face.VertexIndieces[2] + 1), (object) (face.TexCoordIndieces[2] + 1), (object) (face.NormalIndieces[2] + 1), (object) (face.VertexColorIndieces[0] + 1), (object) (face.VertexColorIndieces[1] + 1), (object) (face.VertexColorIndieces[2] + 1));
        else if (flag1 && flag2 && flag3)
          stringBuilder1.AppendFormat("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}\n", (object) (face.VertexIndieces[0] + 1), (object) (face.TexCoordIndieces[0] + 1), (object) (face.NormalIndieces[0] + 1), (object) (face.VertexIndieces[1] + 1), (object) (face.TexCoordIndieces[1] + 1), (object) (face.NormalIndieces[1] + 1), (object) (face.VertexIndieces[2] + 1), (object) (face.TexCoordIndieces[2] + 1), (object) (face.NormalIndieces[2] + 1));
        else if (flag1 && flag3)
          stringBuilder1.AppendFormat("f {0}/{1} {2}/{3} {4}/{5}\n", (object) (face.VertexIndieces[0] + 1), (object) (face.TexCoordIndieces[0] + 1), (object) (face.VertexIndieces[1] + 1), (object) (face.TexCoordIndieces[1] + 1), (object) (face.VertexIndieces[2] + 1), (object) (face.TexCoordIndieces[2] + 1));
        else if (flag1 && flag2)
          stringBuilder1.AppendFormat("f {0}//{1} {2}//{3} {4}//{5}\n", (object) (face.VertexIndieces[0] + 1), (object) (face.NormalIndieces[0] + 1), (object) (face.VertexIndieces[1] + 1), (object) (face.NormalIndieces[1] + 1), (object) (face.VertexIndieces[2] + 1), (object) (face.NormalIndieces[2] + 1));
        else if (flag1)
          stringBuilder1.AppendFormat("f {0} {1} {2}\n", (object) (face.VertexIndieces[0] + 1), (object) (face.VertexIndieces[1] + 1), (object) (face.VertexIndieces[2] + 1));
      }
      File.Create(Obj).Close();
      File.WriteAllBytes(Obj, Encoding.ASCII.GetBytes(stringBuilder1.ToString()));
      if (this.MLTName == null)
        return;
      new MLT(this.MLTName).Write(Path.GetDirectoryName(Obj) + "\\" + Path.GetFileNameWithoutExtension(Obj) + ".mtl");
    }

    public static OBJ FixNitroUV(OBJ Input)
    {
      MLT mlt = new MLT(Input.MLTName);
      OBJ obj = new OBJ();
      obj.MLTName = Input.MLTName;
      obj.Vertices = Input.Vertices;
      obj.Normals = Input.Normals;
      int num1 = 0;
      foreach (OBJ.Face face1 in Input.Faces)
      {
        Vector2[] vector2Array = new Vector2[3]
        {
          Input.TexCoords[face1.TexCoordIndieces[0]],
          Input.TexCoords[face1.TexCoordIndieces[1]],
          Input.TexCoords[face1.TexCoordIndieces[2]]
        };
        MLT.Material materialByName = mlt.GetMaterialByName(face1.MaterialName);
        if (materialByName.DiffuseMap != null)
        {
          float num2 = 2047.938f / (float) materialByName.DiffuseMap.Width;
          float num3 = -2048f / (float) materialByName.DiffuseMap.Width;
          float num4 = 2047.938f / (float) materialByName.DiffuseMap.Height;
          float num5 = -2048f / (float) materialByName.DiffuseMap.Height;
          float num6 = vector2Array[0].X % 1f;
          float num7 = vector2Array[0].Y % 1f;
          vector2Array[1].X = vector2Array[1].X - vector2Array[0].X + num6;
          vector2Array[1].Y = vector2Array[1].Y - vector2Array[0].Y + num7;
          vector2Array[2].X = vector2Array[2].X - vector2Array[0].X + num6;
          vector2Array[2].Y = vector2Array[2].Y - vector2Array[0].Y + num7;
          vector2Array[0].X = num6;
          vector2Array[0].Y = num7;
          for (; (double) vector2Array[0].X <= (double) num3 || (double) vector2Array[1].X <= (double) num3 || (double) vector2Array[2].X <= (double) num3; ++vector2Array[2].X)
          {
            ++vector2Array[0].X;
            ++vector2Array[1].X;
          }
          for (; (double) vector2Array[0].X >= (double) num2 || (double) vector2Array[1].X >= (double) num2 || (double) vector2Array[2].X >= (double) num2; --vector2Array[2].X)
          {
            --vector2Array[0].X;
            --vector2Array[1].X;
          }
          for (; (double) vector2Array[0].Y <= (double) num5 || (double) vector2Array[1].Y <= (double) num5 || (double) vector2Array[2].Y <= (double) num5; ++vector2Array[2].Y)
          {
            ++vector2Array[0].Y;
            ++vector2Array[1].Y;
          }
          for (; (double) vector2Array[0].Y >= (double) num4 || (double) vector2Array[1].Y >= (double) num4 || (double) vector2Array[2].Y >= (double) num4; --vector2Array[2].Y)
          {
            --vector2Array[0].Y;
            --vector2Array[1].Y;
          }
          if ((double) vector2Array[0].X <= (double) num3 || (double) vector2Array[1].X <= (double) num3 || ((double) vector2Array[2].X <= (double) num3 || (double) vector2Array[0].X >= (double) num2) || ((double) vector2Array[1].X >= (double) num2 || (double) vector2Array[2].X >= (double) num2 || ((double) vector2Array[0].Y <= (double) num5 || (double) vector2Array[1].Y <= (double) num5)) || ((double) vector2Array[2].Y <= (double) num5 || (double) vector2Array[0].Y >= (double) num4 || (double) vector2Array[1].Y >= (double) num4) || (double) vector2Array[2].Y >= (double) num4)
          {
            // TODO: Message box
            // int num8 = (int) MessageBox.Show("Your model seems to contain a face which texture is repeated too many to fix. Try splitting the face up, or less repeating the texture before trying again.\r\nMaterial Name: " + face1.MaterialName + "\r\nThere may be more though.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return (OBJ) null;
          }
        }
        obj.TexCoords.AddRange((IEnumerable<Vector2>) vector2Array);
        OBJ.Face face2 = new OBJ.Face();
        face2.MaterialName = face1.MaterialName;
        face2.NormalIndieces = face1.NormalIndieces;
        face2.TexCoordIndieces.AddRange((IEnumerable<int>) new int[3]
        {
          num1,
          num1 + 1,
          num1 + 2
        });
        num1 += 3;
        face2.VertexIndieces = face1.VertexIndieces;
        obj.Faces.Add(face2);
      }
      return obj;
    }

    public class Face
    {
      public string MaterialName = (string) null;
      public List<int> VertexIndieces = new List<int>();
      public List<int> NormalIndieces = new List<int>();
      public List<int> TexCoordIndieces = new List<int>();
      public List<int> VertexColorIndieces = new List<int>();
    }
  }
}
