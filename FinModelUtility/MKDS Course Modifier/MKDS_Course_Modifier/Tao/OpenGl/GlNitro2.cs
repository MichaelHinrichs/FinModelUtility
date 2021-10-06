// Decompiled with JetBrains decompiler
// Type: Tao.OpenGl.GlNitro
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using MKDS_Course_Modifier.MPDS;
using MKDS_Course_Modifier.SM64DS;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Tao.Platform.Windows;

namespace Tao.OpenGl
{
  public class GlNitro2
  {
    public static void glNitroTexImage2D(System.Drawing.Bitmap b, BMD.BMDMaterial m, int Nr)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9728);
      Gl.glTexParameteri(3553, 10240, 9728);
      bool flag1 = ((int) (m.texImageParam >> 16) & 1) == 1;
      bool flag2 = ((int) (m.texImageParam >> 17) & 1) == 1;
      bool flag3 = ((int) (m.texImageParam >> 18) & 1) == 1;
      bool flag4 = ((int) (m.texImageParam >> 19) & 1) == 1;
      int num1 = !flag1 || !flag3 ? (!flag1 ? 10496 : 10497) : 33648;
      int num2 = !flag2 || !flag4 ? (!flag2 ? 10496 : 10497) : 33648;
      Gl.glTexParameterf(3553, 10242, (float) num1);
      Gl.glTexParameterf(3553, 10243, (float) num2);
    }

    public static void glNitroTexImage2D(System.Drawing.Bitmap b, HBDF.MDLFBlock.TextureBlock m, int Nr)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9728);
      Gl.glTexParameteri(3553, 10240, 9728);
      bool flag1 = ((int) (m.texImageParam >> 16) & 1) == 1;
      bool flag2 = ((int) (m.texImageParam >> 17) & 1) == 1;
      bool flag3 = ((int) (m.texImageParam >> 18) & 1) == 1;
      bool flag4 = ((int) (m.texImageParam >> 19) & 1) == 1;
      int num1 = !flag1 || !flag3 ? (!flag1 ? 10496 : 10497) : 33648;
      int num2 = !flag2 || !flag4 ? (!flag2 ? 10496 : 10497) : 33648;
      Gl.glTexParameterf(3553, 10242, (float) num1);
      Gl.glTexParameterf(3553, 10243, (float) num2);
    }

    public static void glNitroBindTextures(BMD b, int offset)
    {
      int num = 1;
      foreach (BMD.BMDMaterial material in b.Materials)
      {
        if (material.TexID == -1)
          ++num;
        else if (material.PalID != -1)
          GlNitro2.glNitroTexImage2D(b.Textures[material.TexID].ToBitmap(b.Palettes[material.PalID]), material, num++);
        else
          GlNitro2.glNitroTexImage2D(b.Textures[material.TexID].ToBitmap((BMD.BMDPalette) null), material, num++);
      }
    }

    public static void glNitroBindTextures(HBDF b, int offset)
    {
      if (b.TEXSBlocks.Length == 0)
        return;
      int num = 0;
      foreach (HBDF.MDLFBlock.TextureBlock texture in b.MDLFBlocks[0].Textures)
      {
        string str = b.MDLFBlocks[0].Names.Strings[(int) b.MDLFBlocks[0].NrMaterials + num * 2 + 1];
        int index1 = 0;
        for (int index2 = 0; index2 < b.TEXSBlocks[0].TEXOBlocks.Length; ++index2)
        {
          if (b.TEXSBlocks[0].TEXOBlocks[index2].TexName.Name == str)
          {
            index1 = index2;
            break;
          }
        }
        if (b.TEXSBlocks[0].TEXOBlocks[index1].PalName != null)
          GlNitro2.glNitroTexImage2D(b.TEXSBlocks[0].GetIMGOByName(b.TEXSBlocks[0].TEXOBlocks[index1].TexName.Name).ToBitmap(b.TEXSBlocks[0].GetPLTOByName(b.TEXSBlocks[0].TEXOBlocks[index1].PalName.Name)), texture, num + offset);
        else
          GlNitro2.glNitroTexImage2D(b.TEXSBlocks[0].GetIMGOByName(b.TEXSBlocks[0].TEXOBlocks[index1].TexName.Name).ToBitmap((HBDF.TEXSBlock.PLTOBlock) null), texture, num + offset);
        ++num;
      }
    }

    public static void glNitroBindTextures(HBDF.TEXSBlock b, int offset)
    {
      int num = 0;
      foreach (HBDF.TEXSBlock.TEXOBlock texoBlock in b.TEXOBlocks)
      {
        if (texoBlock.PalName != null)
          GlNitro.glNitroTexImage2D(b.GetIMGOByName(texoBlock.TexName.Name).ToBitmap(b.GetPLTOByName(texoBlock.PalName.Name)), num + offset, 10496, 9728);
        else
          GlNitro.glNitroTexImage2D(b.GetIMGOByName(texoBlock.TexName.Name).ToBitmap((HBDF.TEXSBlock.PLTOBlock) null), num + offset, 10496, 9728);
        ++num;
      }
    }
  }
}
