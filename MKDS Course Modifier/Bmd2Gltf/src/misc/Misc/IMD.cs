// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.IMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Xml;

namespace MKDS_Course_Modifier.Misc
{
  public class IMD
  {
    public static void FixUV(string InPath)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(InPath);
      XmlNode xmlNode1 = (XmlNode) null;
      XmlNode xmlNode2 = (XmlNode) null;
      XmlNode xmlNode3 = (XmlNode) null;
      XmlNode xmlNode4 = (XmlNode) null;
      foreach (XmlNode xmlNode5 in xmlDocument.ChildNodes[1].ChildNodes[1])
      {
        if (xmlNode5.Name == "material_array")
          xmlNode1 = xmlNode5;
        else if (xmlNode5.Name == "polygon_array")
          xmlNode3 = xmlNode5;
        else if (xmlNode5.Name == "node_array")
          xmlNode4 = xmlNode5;
        else if (xmlNode5.Name == "tex_image_array")
          xmlNode2 = xmlNode5;
      }
      foreach (XmlNode xmlNode5 in xmlNode4)
      {
        foreach (XmlNode xmlNode6 in xmlNode5)
        {
          if (Convert.ToInt32(xmlNode1.ChildNodes[Convert.ToInt32(xmlNode6.Attributes[1].Value)].Attributes[21].Value) != -1)
          {
            int int32_1 = Convert.ToInt32(xmlNode2.ChildNodes[Convert.ToInt32(xmlNode1.ChildNodes[Convert.ToInt32(xmlNode6.Attributes[1].Value)].Attributes[21].Value)].Attributes[2].Value);
            int int32_2 = Convert.ToInt32(xmlNode2.ChildNodes[Convert.ToInt32(xmlNode1.ChildNodes[Convert.ToInt32(xmlNode6.Attributes[1].Value)].Attributes[21].Value)].Attributes[3].Value);
            foreach (XmlNode xmlNode7 in xmlNode3.ChildNodes[Convert.ToInt32(xmlNode6.Attributes[2].Value)])
            {
              foreach (XmlNode xmlNode8 in xmlNode7.ChildNodes[1])
              {
                float num1 = 0.0f;
                float num2 = 0.0f;
                foreach (XmlNode xmlNode9 in xmlNode8)
                {
                  if (xmlNode9.Name == "tex")
                  {
                    float num3 = float.Parse(xmlNode9.Attributes[0].Value.Split(' ')[0].Replace(".", ","));
                    float num4 = float.Parse(xmlNode9.Attributes[0].Value.Split(' ')[1].Replace(".", ","));
                    float num5 = 0.0f;
                    float num6 = 0.0f;
                    while ((double) num3 >= 2048.0 || (double) num3 <= -2048.0)
                    {
                      if ((double) num3 >= 2048.0)
                      {
                        num3 -= (float) int32_1;
                        num5 -= (float) int32_1;
                      }
                      if ((double) num3 <= -2048.0)
                      {
                        num3 += (float) int32_1;
                        num5 += (float) int32_1;
                      }
                    }
                    while ((double) num4 >= 2048.0 || (double) num4 <= -2048.0)
                    {
                      if ((double) num4 >= 2048.0)
                      {
                        num4 -= (float) int32_2;
                        num6 -= (float) int32_2;
                      }
                      if ((double) num4 <= -2048.0)
                      {
                        num4 += (float) int32_2;
                        num6 += (float) int32_2;
                      }
                    }
                    if ((double) num5 > (double) num1)
                      num1 = num5;
                    if ((double) num6 > (double) num2)
                      num2 = num6;
                  }
                }
                foreach (XmlNode xmlNode9 in xmlNode8)
                {
                  if (xmlNode9.Name == "tex")
                  {
                    float num3 = float.Parse(xmlNode9.Attributes[0].Value.Split(' ')[0].Replace(".", ",")) + num1;
                    float num4 = float.Parse(xmlNode9.Attributes[0].Value.Split(' ')[1].Replace(".", ",")) + num2;
                    while ((double) num3 >= 2048.0 || (double) num3 <= -2048.0)
                    {
                      if ((double) num3 >= 2048.0)
                        num3 -= (float) int32_1;
                      if ((double) num3 <= -2048.0)
                        num3 += (float) int32_1;
                    }
                    while ((double) num4 >= 2048.0 || (double) num4 <= -2048.0)
                    {
                      if ((double) num4 >= 2048.0)
                        num4 -= (float) int32_2;
                      if ((double) num4 <= -2048.0)
                        num4 += (float) int32_2;
                    }
                    xmlNode9.Attributes[0].Value = num3.ToString("0.000000").Replace(",", ".") + " " + num4.ToString("0.000000").Replace(",", ".");
                  }
                }
              }
            }
          }
        }
      }
      xmlDocument.Save(InPath);
    }

    public enum PrimitiveType
    {
      triangles,
      quads,
      triangle_strip,
      quad_strip,
    }
  }
}
