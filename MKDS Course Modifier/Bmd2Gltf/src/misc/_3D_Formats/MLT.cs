// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.MLT
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier._3D_Formats
{
  public class MLT
  {
    public List<MLT.Material> Materials = new List<MLT.Material>();

    public MLT(string file)
    {
      StreamReader streamReader = new StreamReader((Stream) File.OpenRead(file));
      MLT.Material material = (MLT.Material) null;
      CultureInfo cultureInfo = new CultureInfo("en-US");
      string str1;
      while ((str1 = streamReader.ReadLine()) != null)
      {
        string str2 = str1.Trim();
        if (str2.Length >= 1 && str2[0] != '#')
        {
          string[] strArray = str2.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length >= 1)
          {
            switch (strArray[0])
            {
              case "newmtl":
                if (strArray.Length >= 2)
                {
                  if (material != null)
                    this.Materials.Add(material);
                  material = new MLT.Material(strArray[1]);
                  break;
                }
                continue;
              case "Ka":
                if (strArray.Length >= 4)
                {
                  float num1 = float.Parse(strArray[1], (IFormatProvider) cultureInfo);
                  float num2 = float.Parse(strArray[2], (IFormatProvider) cultureInfo);
                  float num3 = float.Parse(strArray[3], (IFormatProvider) cultureInfo);
                  material.AmbientColor = Color.FromArgb((int) ((double) num1 * (double) byte.MaxValue), (int) ((double) num2 * (double) byte.MaxValue), (int) ((double) num3 * (double) byte.MaxValue));
                  break;
                }
                continue;
              case "Kd":
                if (strArray.Length >= 4)
                {
                  float num1 = float.Parse(strArray[1], (IFormatProvider) cultureInfo);
                  float num2 = float.Parse(strArray[2], (IFormatProvider) cultureInfo);
                  float num3 = float.Parse(strArray[3], (IFormatProvider) cultureInfo);
                  material.DiffuseColor = Color.FromArgb((int) ((double) num1 * (double) byte.MaxValue), (int) ((double) num2 * (double) byte.MaxValue), (int) ((double) num3 * (double) byte.MaxValue));
                  break;
                }
                continue;
              case "Ks":
                if (strArray.Length >= 4)
                {
                  float num1 = float.Parse(strArray[1], (IFormatProvider) cultureInfo);
                  float num2 = float.Parse(strArray[2], (IFormatProvider) cultureInfo);
                  float num3 = float.Parse(strArray[3], (IFormatProvider) cultureInfo);
                  material.SpecularColor = Color.FromArgb((int) ((double) num1 * (double) byte.MaxValue), (int) ((double) num2 * (double) byte.MaxValue), (int) ((double) num3 * (double) byte.MaxValue));
                  break;
                }
                continue;
              case "d":
                if (strArray.Length >= 2)
                {
                  material.Alpha = float.Parse(strArray[1], (IFormatProvider) cultureInfo);
                  break;
                }
                continue;
              case "map_Kd":
                try
                {
                  material.DiffuseMap = new Bitmap((Stream) new MemoryStream(File.ReadAllBytes(Path.GetDirectoryName(file) + "\\" + str2.Substring(strArray[0].Length + 1).Trim())));
                  break;
                }
                catch
                {
                  material.DiffuseMap = (Bitmap) null;
                  break;
                }
            }
          }
        }
      }
      this.Materials.Add(material);
      streamReader.Close();
    }

    public void Write(string Mlt)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(Mlt) + "\\" + Path.GetFileNameWithoutExtension(Mlt) + "_tex\\");
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine("# Created by MKDS Course Modifer (OBJ UV Fixer)");
      stringBuilder1.AppendLine();
      int num1 = 0;
      foreach (MLT.Material material in this.Materials)
      {
        stringBuilder1.AppendFormat("newmtl {0}\n", (object) material.Name);
        stringBuilder1.AppendFormat("Ka {0} {1} {2}\n", (object) ((float) material.AmbientColor.R / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) material.AmbientColor.G / (float) byte.MaxValue).ToString().Replace(",", "."), (object) ((float) material.AmbientColor.B / (float) byte.MaxValue).ToString().Replace(",", "."));
        StringBuilder stringBuilder2 = stringBuilder1;
        float num2 = (float) material.DiffuseColor.R / (float) byte.MaxValue;
        string str1 = num2.ToString().Replace(",", ".");
        num2 = (float) material.DiffuseColor.G / (float) byte.MaxValue;
        string str2 = num2.ToString().Replace(",", ".");
        string str3 = ((float) material.DiffuseColor.B / (float) byte.MaxValue).ToString().Replace(",", ".");
        stringBuilder2.AppendFormat("Kd {0} {1} {2}\n", (object) str1, (object) str2, (object) str3);
        StringBuilder stringBuilder3 = stringBuilder1;
        float num3 = (float) material.SpecularColor.R / (float) byte.MaxValue;
        string str4 = num3.ToString().Replace(",", ".");
        num3 = (float) material.SpecularColor.G / (float) byte.MaxValue;
        string str5 = num3.ToString().Replace(",", ".");
        string str6 = ((float) material.SpecularColor.B / (float) byte.MaxValue).ToString().Replace(",", ".");
        stringBuilder3.AppendFormat("Ks {0} {1} {2}\n", (object) str4, (object) str5, (object) str6);
        stringBuilder1.AppendFormat("d {0}\n", (object) material.Alpha.ToString().Replace(",", "."));
        if (material.DiffuseMap != null)
        {
          stringBuilder1.AppendFormat("map_Kd {0}_tex/{1}_cmp2.png\n", (object) Path.GetFileNameWithoutExtension(Mlt), (object) num1);
          material.DiffuseMap.Save(Path.GetDirectoryName(Mlt) + "\\" + Path.GetFileNameWithoutExtension(Mlt) + "_tex\\" + (object) num1 + "_cmp2.png", ImageFormat.Png);
          ++num1;
        }
        stringBuilder1.AppendLine();
      }
      File.Create(Mlt).Close();
      File.WriteAllBytes(Mlt, Encoding.ASCII.GetBytes(stringBuilder1.ToString()));
    }

    public MLT.Material GetMaterialByName(string Name)
    {
      foreach (MLT.Material material in this.Materials)
      {
        if (material.Name == Name)
          return material;
      }
      return (MLT.Material) null;
    }

    public class Material
    {
      public float Alpha = 1f;
      public Bitmap DiffuseMap = (Bitmap) null;
      public string Name;
      public Color DiffuseColor;
      public Color AmbientColor;
      public Color SpecularColor;

      public Material(string Name)
      {
        this.Name = Name;
      }

      public override string ToString()
      {
        return this.Name;
      }
    }
  }
}
