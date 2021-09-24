// Decompiled with JetBrains decompiler
// Type: Tao.OpenGl.GlNitro
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
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
  public class GlNitro
  {
    private const float SCALE_IV = 4096f;

    /*public static void glNitroTexImage2D(
      System.Drawing.Bitmap b,
      NSBMD.ModelSet.Model.MaterialSet.Material m,
      int Nr)
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

    public static void glNitroTexImage2D(System.Drawing.Bitmap b, int Nr, int WrapMode)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, 9729);
      Gl.glTexParameteri(3553, 10240, 9729);
      Gl.glTexParameterf(3553, 10242, (float) WrapMode);
      Gl.glTexParameterf(3553, 10243, (float) WrapMode);
    }*/

    /*public static void glNitroTexImage2D(System.Drawing.Bitmap b, int Nr, int WrapMode, int FilterMode)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, FilterMode);
      Gl.glTexParameteri(3553, 10240, FilterMode);
      Gl.glTexParameterf(3553, 10242, (float) WrapMode);
      Gl.glTexParameterf(3553, 10243, (float) WrapMode);
    }

    public static void glNitroTexImage2D(
      System.Drawing.Bitmap b,
      int Nr,
      int WrapModeS,
      int WrapModeT,
      int FilterModeMin,
      int FilterModeMag)
    {
      Gl.glBindTexture(3553, Nr);
      Gl.glColor3f(1f, 1f, 1f);
      BitmapData bitmapdata = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      Gl.glTexImage2D(3553, 0, 32856, b.Width, b.Height, 0, 32993, 5121, bitmapdata.Scan0);
      b.UnlockBits(bitmapdata);
      Gl.glTexParameteri(3553, 10241, FilterModeMin);
      Gl.glTexParameteri(3553, 10240, FilterModeMag);
      Gl.glTexParameterf(3553, 10242, (float) WrapModeS);
      Gl.glTexParameterf(3553, 10243, (float) WrapModeT);
    }

    /*public static void glNitroBindTextures(NSBMD mod, int offset)
    {
      if (mod.TexPlttSet == null)
        return;
      for (int index1 = 0; index1 < mod.modelSet.models.Length; ++index1)
      {
        for (int index2 = 0; index2 < mod.modelSet.models[index1].materials.materials.Length; ++index2)
        {
          NSBTX.TexplttSet.DictTexData dictTexData = (NSBTX.TexplttSet.DictTexData) null;
          for (int index3 = 0; index3 < (int) mod.modelSet.models[index1].materials.dictTexToMatList.numEntry; ++index3)
          {
            if (((IEnumerable<int>) mod.modelSet.models[index1].materials.dictTexToMatList[index3].Value.Materials).Contains<int>(index2))
            {
              int index4 = index3;
              KeyValuePair<string, NSBTX.TexplttSet.DictTexData> keyValuePair;
              for (int index5 = 0; index5 < (int) mod.TexPlttSet.dictTex.numEntry; ++index5)
              {
                keyValuePair = mod.TexPlttSet.dictTex[index5];
                if (keyValuePair.Key == mod.modelSet.models[index1].materials.dictTexToMatList[index3].Key)
                {
                  index4 = index5;
                  break;
                }
              }
              keyValuePair = mod.TexPlttSet.dictTex[index4];
              dictTexData = keyValuePair.Value;
              break;
            }
          }
          if (dictTexData != null)
          {
            mod.modelSet.models[index1].materials.materials[index2].Fmt = dictTexData.Fmt;
            mod.modelSet.models[index1].materials.materials[index2].origHeight = dictTexData.T;
            mod.modelSet.models[index1].materials.materials[index2].origWidth = dictTexData.S;
            NSBTX.TexplttSet.DictPlttData Palette = (NSBTX.TexplttSet.DictPlttData) null;
            if (dictTexData.Fmt != Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
            {
              for (int index3 = 0; index3 < (int) mod.modelSet.models[index1].materials.dictPlttToMatList.numEntry; ++index3)
              {
                if (((IEnumerable<int>) mod.modelSet.models[index1].materials.dictPlttToMatList[index3].Value.Materials).Contains<int>(index2))
                {
                  int index4 = index3;
                  KeyValuePair<string, NSBTX.TexplttSet.DictPlttData> keyValuePair;
                  for (int index5 = 0; index5 < (int) mod.TexPlttSet.dictPltt.numEntry; ++index5)
                  {
                    keyValuePair = mod.TexPlttSet.dictPltt[index5];
                    if (keyValuePair.Key == mod.modelSet.models[index1].materials.dictPlttToMatList[index3].Key)
                    {
                      index4 = index5;
                      break;
                    }
                  }
                  keyValuePair = mod.TexPlttSet.dictPltt[index4];
                  Palette = keyValuePair.Value;
                  break;
                }
              }
            }
            GlNitro.glNitroTexImage2D(dictTexData.ToBitmap(Palette), mod.modelSet.models[index1].materials.materials[index2], index2 + offset);
          }
        }
      }
    }*/

    /*public static void glNitroGx(
      byte[] polydata,
      MTX44 Curmtx,
      int Alpha,
      ref MTX44[] MatrixStack,
      int PosScale,
      bool picking = false)
    {
    }

    /*public static void glNitroGx(
      byte[] polydata,
      MTX44 Curmtx,
      ref GlNitro.Nitro3DContext Context,
      int PosScale,
      bool picking = false)
    {
      Gl.glMatrixMode(5888);
      int num1 = -1;
      if (polydata == null)
        return;
      int offset1 = 0;
      int length = polydata.Length;
      int[] numArray1 = new int[4];
      float[] v = new float[3];
      float[] numArray2 = new float[3];
      MTX44 m = Curmtx;
      MTX44 mtX44_1 = Curmtx;
      while (offset1 < length)
      {
        for (int index = 0; index < 4; ++index)
        {
          if (offset1 >= length)
          {
            numArray1[index] = (int) byte.MaxValue;
          }
          else
          {
            numArray1[index] = (int) polydata[offset1];
            ++offset1;
          }
        }
        for (int index1 = 0; index1 < 4 && offset1 < length; ++index1)
        {
          switch (numArray1[index1])
          {
            case 16:
              int num2 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              Context.MatrixMode = num2;
              break;
            case 18:
              offset1 += 4;
              break;
            case 19:
              offset1 += 4;
              break;
            case 20:
              int index2 = Bytes.Read4BytesAsInt32(polydata, offset1) & 31;
              offset1 += 4;
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                Context.MatrixStack[index2].CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 21:
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.LoadIdentity();
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 22:
              for (int index3 = 0; index3 < 16; ++index3)
              {
                if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
                  m[index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 23:
              for (int index3 = 0; index3 < 4; ++index3)
              {
                int index4 = 0;
                while (index4 < 3)
                {
                  if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
                    m[index4, index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++index3;
                }
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 24:
              MTX44 b1 = new MTX44();
              b1.LoadIdentity();
              for (int index3 = 0; index3 < 16; ++index3)
              {
                b1[index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.MultMatrix(b1).CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 25:
              MTX44 b2 = new MTX44();
              b2.LoadIdentity();
              for (int index3 = 0; index3 < 4; ++index3)
              {
                int index4 = 0;
                while (index4 < 3)
                {
                  b2[index4, index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++index3;
                }
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.MultMatrix(b2).CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 26:
              MTX44 b3 = new MTX44();
              b3.LoadIdentity();
              for (int index3 = 0; index3 < 3; ++index3)
              {
                int index4 = 0;
                while (index4 < 3)
                {
                  b3[index4, index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++index3;
                }
              }
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.MultMatrix(b3).CopyValuesTo(m);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 27:
              int num3 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset2 = offset1 + 4;
              int num4 = Bytes.Read4BytesAsInt32(polydata, offset2);
              int offset3 = offset2 + 4;
              int num5 = Bytes.Read4BytesAsInt32(polydata, offset3);
              offset1 = offset3 + 4;
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.Scale((float) num3 / 4096f / (float) PosScale, (float) num4 / 4096f / (float) PosScale, (float) num5 / 4096f / (float) PosScale);
                break;
              }
              break;
            case 28:
              int data1 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset4 = offset1 + 4;
              int data2 = Bytes.Read4BytesAsInt32(polydata, offset4);
              int offset5 = offset4 + 4;
              int data3 = Bytes.Read4BytesAsInt32(polydata, offset5);
              offset1 = offset5 + 4;
              if (Context.MatrixMode == 1 || Context.MatrixMode == 2)
              {
                m.translate((float) GlNitro.sign(data1, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data2, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data3, 32) / 4096f / (float) PosScale);
                mtX44_1 = m.Clone();
                break;
              }
              break;
            case 32:
              long num6 = (long) Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              long num7 = num6 & 31L;
              long num8 = num6 >> 5 & 31L;
              long num9 = num6 >> 10 & 31L;
              if (!picking)
              {
                Gl.glColor4f((float) num7 / 31f, (float) num8 / 31f, (float) num9 / 31f, (float) Context.Alpha / 31f);
                break;
              }
              break;
            case 33:
              int num10 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num11 = num10 & 1023;
              if ((num11 & 512) != 0)
                num11 |= -1024;
              int num12 = num10 >> 10 & 1023;
              if ((num12 & 512) != 0)
                num12 |= -1024;
              int num13 = num10 >> 20 & 1023;
              if ((num13 & 512) != 0)
                num13 |= -1024;
              Vector3 vector3_1 = new Vector3((float) num11 / 512f, (float) num12 / 512f, (float) num13 / 512f);
              MTX44 mtX44_2 = mtX44_1.Clone();
              mtX44_2[12] = 0.0f;
              mtX44_2[13] = 0.0f;
              mtX44_2[14] = 0.0f;
              float[] numArray3 = mtX44_2.MultVector(new float[3]
              {
                vector3_1.X,
                vector3_1.Y,
                vector3_1.Z
              });
              vector3_1 = new Vector3(numArray3[0], numArray3[1], numArray3[2]);
              Vector3[] vector3Array1 = new Vector3[4];
              Vector3[] vector3Array2 = new Vector3[4];
              Vector3[] vector3Array3 = new Vector3[4];
              for (int index3 = 0; index3 < 4; ++index3)
              {
                if (Context.LightEnabled[index3])
                {
                  float num14 = Math.Max(Math.Min(Math.Max(0.0f, Vector3.Dot(-Context.LightVectors[index3], vector3_1)), 1f), 0.0f);
                  vector3Array1[index3] = Vector3.Multiply(num14 * Context.LightColors[index3].ToVector3(), Context.DiffuseColor.ToVector3());
                  vector3Array2[index3] = Vector3.Multiply(Context.LightColors[index3].ToVector3(), Context.AmbientColor.ToVector3());
                  float num15 = Math.Max(Math.Min(Math.Max(0.0f, (float) Math.Cos(2.0 * (double) Vector3.CalculateAngle(-((Context.LightVectors[index3] + new Vector3(0.0f, 0.0f, -1f)) / 2f), vector3_1))), 1f), 0.0f);
                  vector3Array3[index3] = !Context.UseSpecularReflectionTable ? Vector3.Multiply(num15 * Context.LightColors[index3].ToVector3(), Context.SpecularColor.ToVector3()) : Vector3.Multiply((float) Context.SpecularReflectionTable[(int) ((double) num15 * (double) sbyte.MaxValue)] / (float) byte.MaxValue * Context.LightColors[index3].ToVector3(), Context.SpecularColor.ToVector3());
                }
              }
              Vector3 vector3_2 = Context.EmissionColor.ToVector3();
              for (int index3 = 0; index3 < 4; ++index3)
              {
                if (Context.LightEnabled[index3])
                  vector3_2 += vector3Array1[index3] + vector3Array2[index3] + vector3Array3[index3];
              }
              vector3_2.X = Math.Min(1f, vector3_2.X);
              vector3_2.Y = Math.Min(1f, vector3_2.Y);
              vector3_2.Z = Math.Min(1f, vector3_2.Z);
              Gl.glColor4f(vector3_2.X, vector3_2.Y, vector3_2.Z, (float) Context.Alpha / 31f);
              break;
            case 34:
              int num16 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num17 = num16 & (int) ushort.MaxValue;
              if ((num17 & 32768) != 0)
                num17 |= -65536;
              int num18 = num16 >> 16 & (int) ushort.MaxValue;
              if ((num18 & 32768) != 0)
                num18 |= -65536;
              Gl.glTexCoord2f((float) num17 / 16f, (float) num18 / 16f);
              break;
            case 35:
              int num19 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset6 = offset1 + 4;
              int num20 = GlNitro.sign(num19 & (int) ushort.MaxValue, 16);
              int num21 = GlNitro.sign(num19 >> 16 & (int) ushort.MaxValue, 16);
              int num22 = Bytes.Read4BytesAsInt32(polydata, offset6);
              offset1 = offset6 + 4;
              int num23 = GlNitro.sign(num22 & (int) ushort.MaxValue, 16);
              v[0] = (float) num20 / 4096f;
              v[1] = (float) num21 / 4096f;
              v[2] = (float) num23 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 36:
              int num24 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num25 = GlNitro.sign(num24 & 1023, 10);
              int num26 = GlNitro.sign(num24 >> 10 & 1023, 10);
              int num27 = GlNitro.sign(num24 >> 20 & 1023, 10);
              v[0] = (float) num25 / 64f;
              v[1] = (float) num26 / 64f;
              v[2] = (float) num27 / 64f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 37:
              int num28 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num29 = GlNitro.sign(num28 & (int) ushort.MaxValue, 16);
              int num30 = GlNitro.sign(num28 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num29 / 4096f;
              v[1] = (float) num30 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 38:
              int num31 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num32 = GlNitro.sign(num31 & (int) ushort.MaxValue, 16);
              int num33 = GlNitro.sign(num31 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num32 / 4096f;
              v[2] = (float) num33 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 39:
              int num34 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num35 = GlNitro.sign(num34 & (int) ushort.MaxValue, 16);
              int num36 = GlNitro.sign(num34 >> 16 & (int) ushort.MaxValue, 16);
              v[1] = (float) num35 / 4096f;
              v[2] = (float) num36 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 40:
              int num37 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num38 = GlNitro.sign(num37 & 1023, 10);
              int num39 = GlNitro.sign(num37 >> 10 & 1023, 10);
              int num40 = GlNitro.sign(num37 >> 20 & 1023, 10);
              v[0] += (float) num38 / 4096f;
              v[1] += (float) num39 / 4096f;
              v[2] += (float) num40 / 4096f;
              Gl.glVertex3fv(m.MultVector(v));
              break;
            case 41:
              Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              break;
            case 42:
              offset1 += 4;
              break;
            case 43:
              offset1 += 4;
              break;
            case 48:
              uint num41 = Bytes.Read4BytesAsUInt32(polydata, offset1);
              offset1 += 4;
              Color color1 = Graphic.ConvertABGR1555((short) ((int) num41 & (int) short.MaxValue));
              Color color2 = Graphic.ConvertABGR1555((short) ((int) (num41 >> 16) & (int) short.MaxValue));
              if (((int) num41 >> 15 & 1) == 1)
                Gl.glColor4f((float) color1.R / (float) byte.MaxValue, (float) color1.G / (float) byte.MaxValue, (float) color1.B / (float) byte.MaxValue, (float) Context.Alpha / 31f);
              Context.DiffuseColor = color1;
              Context.AmbientColor = color2;
              break;
            case 49:
              offset1 += 4;
              break;
            case 50:
              offset1 += 4;
              int num42 = (int) MessageBox.Show("0x32: Light Vector");
              break;
            case 51:
              offset1 += 4;
              break;
            case 52:
              offset1 += 128;
              break;
            case 64:
              int num43 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              num1 = num43;
              int mode;
              switch (num43)
              {
                case 0:
                  mode = 4;
                  break;
                case 1:
                  mode = 7;
                  break;
                case 2:
                  mode = 5;
                  break;
                case 3:
                  mode = 8;
                  break;
                default:
                  throw new Exception();
              }
              Gl.glBegin(mode);
              break;
            case 65:
              Gl.glEnd();
              break;
            case 80:
              offset1 += 4;
              break;
            case 96:
              offset1 += 4;
              break;
            case 112:
              offset1 += 12;
              break;
            case 113:
              offset1 += 8;
              break;
            case 114:
              offset1 += 4;
              break;
          }
        }
      }
    }

    /*public static Group glNitroGxRipper(
      byte[] polydata,
      MTX44 Curmtx,
      int Alpha,
      ref MTX44[] MatrixStack,
      int PosScale,
      NSBMD.ModelSet.Model.MaterialSet.Material m)
    {
      int num1 = -1;
      if (polydata == null)
        return (Group) null;
      int offset1 = 0;
      int length = polydata.Length;
      int[] numArray1 = new int[4];
      float[] v = new float[3];
      float[] numArray2 = new float[3];
      MTX44 m1 = Curmtx;
      Group group = new Group();
      Vector3 vector3 = new Vector3(float.NaN, 0.0f, 0.0f);
      Color color = Color.White;
      Vector2 vector2 = new Vector2(float.NaN, 0.0f);
      List<Vector3> vector3List1 = new List<Vector3>();
      List<Vector3> vector3List2 = new List<Vector3>();
      List<Vector2> vector2List = new List<Vector2>();
      while (offset1 < length)
      {
        for (int index = 0; index < 4; ++index)
        {
          if (offset1 >= length)
          {
            numArray1[index] = (int) byte.MaxValue;
          }
          else
          {
            numArray1[index] = (int) polydata[offset1];
            ++offset1;
          }
        }
        for (int index1 = 0; index1 < 4 && offset1 < length; ++index1)
        {
          switch (numArray1[index1])
          {
            case 16:
              int num2 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              switch (num2)
              {
                default:
                  continue;
              }
            case 18:
              offset1 += 4;
              break;
            case 19:
              offset1 += 4;
              break;
            case 20:
              int index2 = Bytes.Read4BytesAsInt32(polydata, offset1) & 31;
              offset1 += 4;
              MatrixStack[index2].CopyValuesTo(m1);
              break;
            case 21:
              m1.LoadIdentity();
              break;
            case 22:
              for (int index3 = 0; index3 < 16; ++index3)
              {
                m1[index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              break;
            case 23:
              for (int index3 = 0; index3 < 4; ++index3)
              {
                int index4 = 0;
                while (index4 < 3)
                {
                  m1[index4, index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++index3;
                }
              }
              break;
            case 24:
              MTX44 b1 = new MTX44();
              b1.LoadIdentity();
              for (int index3 = 0; index3 < 16; ++index3)
              {
                b1[index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                offset1 += 4;
              }
              m1.MultMatrix(b1).CopyValuesTo(m1);
              break;
            case 25:
              MTX44 b2 = new MTX44();
              b2.LoadIdentity();
              for (int index3 = 0; index3 < 4; ++index3)
              {
                int index4 = 0;
                while (index4 < 3)
                {
                  b2[index4, index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++index3;
                }
              }
              m1.MultMatrix(b2).CopyValuesTo(m1);
              break;
            case 26:
              MTX44 b3 = new MTX44();
              b3.LoadIdentity();
              for (int index3 = 0; index3 < 3; ++index3)
              {
                int index4 = 0;
                while (index4 < 3)
                {
                  b3[index4, index3] = (float) Bytes.Read4BytesAsInt32(polydata, offset1) / 4096f;
                  offset1 += 4;
                  ++index3;
                }
              }
              m1.MultMatrix(b3).CopyValuesTo(m1);
              break;
            case 27:
              int num3 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset2 = offset1 + 4;
              int num4 = Bytes.Read4BytesAsInt32(polydata, offset2);
              int offset3 = offset2 + 4;
              int num5 = Bytes.Read4BytesAsInt32(polydata, offset3);
              offset1 = offset3 + 4;
              m1.Scale((float) num3 / 4096f / (float) PosScale, (float) num4 / 4096f / (float) PosScale, (float) num5 / 4096f / (float) PosScale);
              break;
            case 28:
              int data1 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset4 = offset1 + 4;
              int data2 = Bytes.Read4BytesAsInt32(polydata, offset4);
              int offset5 = offset4 + 4;
              int data3 = Bytes.Read4BytesAsInt32(polydata, offset5);
              offset1 = offset5 + 4;
              m1.translate((float) GlNitro.sign(data1, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data2, 32) / 4096f / (float) PosScale, (float) GlNitro.sign(data3, 32) / 4096f / (float) PosScale);
              break;
            case 32:
              int num6 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num7 = num6 & 31;
              int num8 = num6 >> 5 & 31;
              int num9 = num6 >> 10 & 31;
              color = Graphic.ConvertABGR1555((short) num6);
              break;
            case 33:
              int num10 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num11 = num10 & 1023;
              if ((num11 & 512) != 0)
                num11 |= -1024;
              int num12 = num10 >> 10 & 1023;
              if ((num12 & 512) != 0)
                num12 |= -1024;
              int num13 = num10 >> 20 & 1023;
              if ((num13 & 512) != 0)
                num13 |= -1024;
              vector3 = new Vector3((float) num11 / 512f, (float) num12 / 512f, (float) num13 / 512f);
              break;
            case 34:
              int num14 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num15 = num14 & (int) ushort.MaxValue;
              if ((num15 & 32768) != 0)
                num15 |= -65536;
              int num16 = num14 >> 16 & (int) ushort.MaxValue;
              if ((num16 & 32768) != 0)
                num16 |= -65536;
              vector2 = new Vector2((float) (((double) m.scaleS == 0.0 ? 1.0 : (double) m.scaleS) / (double) m.origWidth * ((double) num15 / 16.0)) / (float) (uint) (((int) (m.texImageParam >> 18) & 1) + 1), (float) (-(((double) m.scaleT == 0.0 ? 1.0 : (double) m.scaleT) / (double) m.origHeight) * ((double) num16 / 16.0)) / (float) (uint) (((int) (m.texImageParam >> 19) & 1) + 1));
              break;
            case 35:
              int num17 = Bytes.Read4BytesAsInt32(polydata, offset1);
              int offset6 = offset1 + 4;
              int num18 = GlNitro.sign(num17 & (int) ushort.MaxValue, 16);
              int num19 = GlNitro.sign(num17 >> 16 & (int) ushort.MaxValue, 16);
              int num20 = Bytes.Read4BytesAsInt32(polydata, offset6);
              offset1 = offset6 + 4;
              int num21 = GlNitro.sign(num20 & (int) ushort.MaxValue, 16);
              v[0] = (float) num18 / 4096f;
              v[1] = (float) num19 / 4096f;
              v[2] = (float) num21 / 4096f;
              float[] numArray3 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray3[0], numArray3[1], numArray3[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 36:
              int num22 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num23 = GlNitro.sign(num22 & 1023, 10);
              int num24 = GlNitro.sign(num22 >> 10 & 1023, 10);
              int num25 = GlNitro.sign(num22 >> 20 & 1023, 10);
              v[0] = (float) num23 / 64f;
              v[1] = (float) num24 / 64f;
              v[2] = (float) num25 / 64f;
              float[] numArray4 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray4[0], numArray4[1], numArray4[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 37:
              int num26 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num27 = GlNitro.sign(num26 & (int) ushort.MaxValue, 16);
              int num28 = GlNitro.sign(num26 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num27 / 4096f;
              v[1] = (float) num28 / 4096f;
              float[] numArray5 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray5[0], numArray5[1], numArray5[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 38:
              int num29 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num30 = GlNitro.sign(num29 & (int) ushort.MaxValue, 16);
              int num31 = GlNitro.sign(num29 >> 16 & (int) ushort.MaxValue, 16);
              v[0] = (float) num30 / 4096f;
              v[2] = (float) num31 / 4096f;
              float[] numArray6 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray6[0], numArray6[1], numArray6[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 39:
              int num32 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num33 = GlNitro.sign(num32 & (int) ushort.MaxValue, 16);
              int num34 = GlNitro.sign(num32 >> 16 & (int) ushort.MaxValue, 16);
              v[1] = (float) num33 / 4096f;
              v[2] = (float) num34 / 4096f;
              float[] numArray7 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray7[0], numArray7[1], numArray7[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 40:
              int num35 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              int num36 = GlNitro.sign(num35 & 1023, 10);
              int num37 = GlNitro.sign(num35 >> 10 & 1023, 10);
              int num38 = GlNitro.sign(num35 >> 20 & 1023, 10);
              v[0] += (float) num36 / 4096f;
              v[1] += (float) num37 / 4096f;
              v[2] += (float) num38 / 4096f;
              float[] numArray8 = m1.MultVector(v);
              vector3List1.Add(new Vector3(numArray8[0], numArray8[1], numArray8[2]));
              vector3List2.Add(vector3);
              vector2List.Add(vector2);
              break;
            case 41:
              Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              break;
            case 42:
              offset1 += 4;
              // TODO: Message box
              //int num39 = (int) MessageBox.Show("");
              break;
            case 43:
              offset1 += 4;
              // TODO: Message box
              //int num40 = (int) MessageBox.Show("");
              break;
            case 48:
              uint num41 = Bytes.Read4BytesAsUInt32(polydata, offset1);
              offset1 += 4;
              Graphic.ConvertABGR1555((short) ((int) num41 & (int) short.MaxValue));
              Graphic.ConvertABGR1555((short) ((int) (num41 >> 16) & (int) short.MaxValue));
              break;
            case 49:
              offset1 += 4;
              break;
            case 50:
              offset1 += 4;
              int num42 = (int) MessageBox.Show("0x32: Light Vector");
              break;
            case 51:
              offset1 += 4;
              int num43 = (int) MessageBox.Show("");
              break;
            case 52:
              offset1 += 128;
              int num44 = (int) MessageBox.Show("");
              break;
            case 64:
              int num45 = Bytes.Read4BytesAsInt32(polydata, offset1);
              offset1 += 4;
              num1 = num45;
              break;
            case 65:
              switch (num1)
              {
                case 0:
                  group.Add(new Polygon(PolygonType.Triangle, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray(), (Color[]) null));
                  break;
                case 1:
                  group.Add(new Polygon(PolygonType.Quad, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray(), (Color[]) null));
                  break;
                case 2:
                  group.Add(new Polygon(PolygonType.TriangleStrip, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray(), (Color[]) null));
                  break;
                case 3:
                  group.Add(new Polygon(PolygonType.QuadStrip, vector3List2.ToArray(), vector2List.ToArray(), vector3List1.ToArray(), (Color[]) null));
                  break;
              }
              vector3List2.Clear();
              vector3List1.Clear();
              vector2List.Clear();
              break;
            case 80:
              offset1 += 4;
              int num46 = (int) MessageBox.Show("");
              break;
            case 96:
              offset1 += 4;
              int num47 = (int) MessageBox.Show("");
              break;
            case 112:
              offset1 += 12;
              int num48 = (int) MessageBox.Show("");
              break;
            case 113:
              offset1 += 8;
              int num49 = (int) MessageBox.Show("");
              break;
            case 114:
              offset1 += 4;
              int num50 = (int) MessageBox.Show("");
              break;
          }
        }
      }
      return group;
    }
    */
    /*private static int sign(int data, int size)
    {
      if ((data & 1 << size - 1) != 0)
        data |= -1 << size;
      return data;
    }

    public static float[] glNitroPivot(float[] ab, int pv, int neg)
    {
      float[] numArray = new float[16];
      numArray[15] = 1f;
      float num1 = 1f;
      float num2 = ab[0];
      float num3 = ab[1];
      float num4 = num2;
      float num5 = num3;
      switch (neg)
      {
        case 1:
        case 3:
        case 5:
        case 7:
        case 9:
        case 11:
        case 13:
        case 15:
          num1 = -1f;
          break;
      }
      switch (neg - 2)
      {
        case 0:
        case 1:
        case 4:
        case 5:
        case 8:
        case 9:
        case 12:
        case 13:
          num5 = -num5;
          break;
      }
      switch (neg - 4)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 8:
        case 9:
        case 10:
        case 11:
          num4 = -num4;
          break;
      }
      switch (pv)
      {
        case 0:
          numArray[0] = num1;
          numArray[5] = num2;
          numArray[6] = num3;
          numArray[9] = num5;
          numArray[10] = num4;
          break;
        case 1:
          numArray[1] = num1;
          numArray[4] = num2;
          numArray[6] = num3;
          numArray[8] = num5;
          numArray[10] = num4;
          break;
        case 2:
          numArray[2] = num1;
          numArray[4] = num2;
          numArray[5] = num3;
          numArray[8] = num5;
          numArray[9] = num4;
          break;
        case 3:
          numArray[4] = num1;
          numArray[1] = num2;
          numArray[2] = num3;
          numArray[9] = num5;
          numArray[10] = num4;
          break;
        case 4:
          numArray[5] = num1;
          numArray[0] = num2;
          numArray[2] = num3;
          numArray[8] = num5;
          numArray[10] = num4;
          break;
        case 5:
          numArray[6] = num1;
          numArray[0] = num2;
          numArray[1] = num3;
          numArray[8] = num5;
          numArray[9] = num4;
          break;
        case 6:
          numArray[8] = num1;
          numArray[1] = num2;
          numArray[2] = num3;
          numArray[5] = num5;
          numArray[6] = num4;
          break;
        case 7:
          numArray[9] = num1;
          numArray[0] = num2;
          numArray[2] = num3;
          numArray[4] = num5;
          numArray[6] = num4;
          break;
        case 8:
          numArray[10] = num1;
          numArray[0] = num2;
          numArray[1] = num3;
          numArray[4] = num5;
          numArray[5] = num4;
          break;
        case 9:
          numArray[0] = -num2;
          break;
      }
      return numArray;
    }*/

    private static void CreateBillboardMatrix(
      float[] matrix,
      Vector3 right,
      Vector3 up,
      Vector3 look,
      Vector3 pos)
    {
      matrix[0] = right.X;
      matrix[1] = right.Y;
      matrix[2] = right.Z;
      matrix[3] = 0.0f;
      matrix[4] = up.X;
      matrix[5] = up.Y;
      matrix[6] = up.Z;
      matrix[7] = 0.0f;
      matrix[8] = look.X;
      matrix[9] = look.Y;
      matrix[10] = look.Z;
      matrix[11] = 0.0f;
      matrix[12] = pos.X;
      matrix[13] = pos.Y;
      matrix[14] = pos.Z;
      matrix[15] = 1f;
    }

    public static float[] BillboardPoint(Vector3 pos, Vector3 camPos, Vector3 camUp)
    {
      Vector3 vector3 = camPos - pos;
      vector3.Normalize();
      Vector3 right = Vector3.Cross(camUp, vector3);
      Vector3 up = Vector3.Cross(vector3, right);
      float[] matrix = new float[16];
      GlNitro.CreateBillboardMatrix(matrix, right, up, vector3, pos);
      return matrix;
    }

    /*public static System.Drawing.Bitmap ScreenShot(SimpleOpenGlControl simpleOpenGlControl1)
    {
      System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(simpleOpenGlControl1.Width, simpleOpenGlControl1.Height);
      BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
      Gl.glReadPixels(0, 0, simpleOpenGlControl1.Width, simpleOpenGlControl1.Height, 32993, 5121, bitmapdata.Scan0);
      bitmap.UnlockBits(bitmapdata);
      bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
      return bitmap;
    }*/

    /*public static byte[] GenerateDl(
      Vector3[] Positions,
      Vector2[] TexCoords,
      Vector3[] Normals,
      Color[] Colors)
    {
      GlNitro.DisplayListEncoder displayListEncoder = new GlNitro.DisplayListEncoder();
      displayListEncoder.Begin((byte) 0);
      for (int index = 0; index < Positions.Length; index += 3)
      {
        displayListEncoder.TexCoord(TexCoords[index]);
        if (Colors != null)
          displayListEncoder.Color(Colors[index]);
        else
          displayListEncoder.Color(Color.White);
        Vector3 position1 = Positions[index];
        displayListEncoder.BestVertex(position1);
        displayListEncoder.TexCoord(TexCoords[index + 1]);
        if (Colors != null)
          displayListEncoder.Color(Colors[index + 1]);
        else
          displayListEncoder.Color(Color.White);
        Vector3 position2 = Positions[index + 1];
        displayListEncoder.BestVertex(position2);
        displayListEncoder.TexCoord(TexCoords[index + 2]);
        if (Colors != null)
          displayListEncoder.Color(Colors[index + 2]);
        else
          displayListEncoder.Color(Color.White);
        Vector3 position3 = Positions[index + 2];
        displayListEncoder.BestVertex(position3);
      }
      displayListEncoder.End();
      return displayListEncoder.GetDisplayList();
    }*/

    public static byte[] RemoveNormals(byte[] DL)
    {
      GlNitro.DisplayListRecoder displayListRecoder = new GlNitro.DisplayListRecoder(DL);
      displayListRecoder.RemoveNormals();
      return displayListRecoder.GetDisplayList();
    }

    public class Nitro3DContext
    {
      public int MatrixMode = 2;
      public MTX44[] MatrixStack = new MTX44[31];
      public Vector3[] LightVectors = new Vector3[4]
      {
        new Vector3(0.0f, -1f, -1f),
        new Vector3(0.998047f, -1f, 0.0f),
        new Vector3(0.0f, -1f, 0.998047f),
        new Vector3(-1f, -1f, 0.0f)
      };
      public Color[] LightColors = new Color[4]
      {
        Color.White,
        Color.White,
        Color.White,
        Color.White
      };
      public bool[] LightEnabled = new bool[4];
      public byte[] SpecularReflectionTable = new byte[128]
      {
        (byte) 0,
        (byte) 2,
        (byte) 4,
        (byte) 6,
        (byte) 8,
        (byte) 10,
        (byte) 12,
        (byte) 14,
        (byte) 16,
        (byte) 18,
        (byte) 20,
        (byte) 22,
        (byte) 24,
        (byte) 26,
        (byte) 28,
        (byte) 30,
        (byte) 32,
        (byte) 34,
        (byte) 36,
        (byte) 38,
        (byte) 40,
        (byte) 42,
        (byte) 44,
        (byte) 46,
        (byte) 48,
        (byte) 50,
        (byte) 52,
        (byte) 54,
        (byte) 56,
        (byte) 58,
        (byte) 60,
        (byte) 62,
        (byte) 64,
        (byte) 66,
        (byte) 68,
        (byte) 70,
        (byte) 72,
        (byte) 74,
        (byte) 76,
        (byte) 78,
        (byte) 80,
        (byte) 82,
        (byte) 84,
        (byte) 86,
        (byte) 88,
        (byte) 90,
        (byte) 92,
        (byte) 94,
        (byte) 96,
        (byte) 98,
        (byte) 100,
        (byte) 102,
        (byte) 104,
        (byte) 106,
        (byte) 108,
        (byte) 110,
        (byte) 112,
        (byte) 114,
        (byte) 116,
        (byte) 118,
        (byte) 120,
        (byte) 122,
        (byte) 124,
        (byte) 126,
        (byte) 129,
        (byte) 131,
        (byte) 133,
        (byte) 135,
        (byte) 137,
        (byte) 139,
        (byte) 141,
        (byte) 143,
        (byte) 145,
        (byte) 147,
        (byte) 149,
        (byte) 151,
        (byte) 153,
        (byte) 155,
        (byte) 157,
        (byte) 159,
        (byte) 161,
        (byte) 163,
        (byte) 165,
        (byte) 167,
        (byte) 169,
        (byte) 171,
        (byte) 173,
        (byte) 175,
        (byte) 177,
        (byte) 179,
        (byte) 181,
        (byte) 183,
        (byte) 185,
        (byte) 187,
        (byte) 189,
        (byte) 191,
        (byte) 193,
        (byte) 195,
        (byte) 197,
        (byte) 199,
        (byte) 201,
        (byte) 203,
        (byte) 205,
        (byte) 207,
        (byte) 209,
        (byte) 211,
        (byte) 213,
        (byte) 215,
        (byte) 217,
        (byte) 219,
        (byte) 221,
        (byte) 223,
        (byte) 225,
        (byte) 227,
        (byte) 229,
        (byte) 231,
        (byte) 233,
        (byte) 235,
        (byte) 237,
        (byte) 239,
        (byte) 241,
        (byte) 243,
        (byte) 245,
        (byte) 247,
        (byte) 249,
        (byte) 251,
        (byte) 253,
        byte.MaxValue
      };
      public int Alpha = 31;
      public bool UseSpecularReflectionTable;
      public Color DiffuseColor;
      public Color AmbientColor;
      public Color SpecularColor;
      public Color EmissionColor;

      public Nitro3DContext()
      {
        for (int index = 0; index < 31; ++index)
          this.MatrixStack[index] = new MTX44();
      }
    }

    public class DisplayListEncoder
    {
      private List<byte> DisplayList = new List<byte>();
      private byte[] Commands = new byte[4];
      private int CommandId = 0;
      private List<byte> Args = new List<byte>();
      private Vector3 VtxState = new Vector3(float.NaN, float.NaN, float.NaN);

      public void Nop()
      {
        this.AddCommand((byte) 0);
      }

      public void Color(Color c)
      {
        this.AddCommand((byte) 32, (uint) Graphic.encodeColor(c.ToArgb()));
      }

      public void Normal(Vector3 Normal)
      {
        this.AddCommand((byte) 33, (uint) (((int) ((double) Normal.Z * 512.0) & 1023) << 20 | ((int) ((double) Normal.Y * 512.0) & 1023) << 10 | (int) ((double) Normal.X * 512.0) & 1023));
      }

      public void TexCoord(Vector2 TexCoord)
      {
        this.AddCommand((byte) 34, (uint) (((int) ((double) TexCoord.Y * 16.0) & (int) ushort.MaxValue) << 16 | (int) ((double) TexCoord.X * 16.0) & (int) ushort.MaxValue));
      }

      public void BestVertex(Vector3 Position)
      {
        if ((double) this.VtxState.X == (double) Position.X)
          this.VertexYZ(Position);
        else if ((double) this.VtxState.Y == (double) Position.Y)
          this.VertexXZ(Position);
        else if ((double) this.VtxState.Z == (double) Position.Z)
          this.VertexXY(Position);
        else if (((int) ((double) Math.Abs(Position.X) * 4096.0) & 63) == 0 && ((int) ((double) Math.Abs(Position.Y) * 4096.0) & 63) == 0 && ((int) ((double) Math.Abs(Position.Z) * 4096.0) & 63) == 0)
          this.Vertex10(Position);
        else
          this.Vertex(Position);
        this.VtxState = Position;
      }

      public void Vertex(Vector3 Position)
      {
        this.AddCommand((byte) 35, (uint) (((int) ((double) Position.Y * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.X * 4096.0) & (int) ushort.MaxValue), (uint) (int) ((double) Position.Z * 4096.0) & (uint) ushort.MaxValue);
      }

      public void Vertex10(Vector3 Position)
      {
        this.AddCommand((byte) 36, (uint) (((int) ((double) Position.Z * 64.0) & 1023) << 20 | ((int) ((double) Position.Y * 64.0) & 1023) << 10 | (int) ((double) Position.X * 64.0) & 1023));
      }

      public void VertexXY(Vector3 Position)
      {
        this.AddCommand((byte) 37, (uint) (((int) ((double) Position.Y * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.X * 4096.0) & (int) ushort.MaxValue));
      }

      public void VertexXZ(Vector3 Position)
      {
        this.AddCommand((byte) 38, (uint) (((int) ((double) Position.Z * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.X * 4096.0) & (int) ushort.MaxValue));
      }

      public void VertexYZ(Vector3 Position)
      {
        this.AddCommand((byte) 39, (uint) (((int) ((double) Position.Z * 4096.0) & (int) ushort.MaxValue) << 16 | (int) ((double) Position.Y * 4096.0) & (int) ushort.MaxValue));
      }

      public void VertexDiff(Vector3 Position)
      {
        this.AddCommand((byte) 40, (uint) (((int) ((double) Position.Z * 4096.0) & 1023) << 20 | ((int) ((double) Position.Y * 4096.0) & 1023) << 10 | (int) ((double) Position.X * 4096.0) & 1023));
      }

      public void Begin(byte VertexType)
      {
        this.AddCommand((byte) 64, (uint) VertexType);
      }

      public void End()
      {
        if (this.CommandId == 0)
        {
          this.AddCommand((byte) 65);
          this.Flush();
          this.Flush();
        }
        else
          this.AddCommand((byte) 65);
      }

      public void Flush()
      {
        for (int commandId = this.CommandId; commandId < 4; ++commandId)
          this.Nop();
      }

      public void AddCommand(byte Id)
      {
        this.AddCommand(Id, new uint[0]);
      }

      public void AddCommand(byte Id, params uint[] Params)
      {
        this.Commands[this.CommandId] = Id;
        ++this.CommandId;
        foreach (uint num in Params)
          this.Args.AddRange((IEnumerable<byte>) BitConverter.GetBytes(num));
        if (this.CommandId != 4)
          return;
        this.DisplayList.AddRange((IEnumerable<byte>) this.Commands);
        this.DisplayList.AddRange((IEnumerable<byte>) this.Args);
        this.Commands = new byte[4];
        this.Args.Clear();
        this.CommandId = 0;
      }

      public byte[] GetDisplayList()
      {
        if (this.CommandId != 0)
          this.Flush();
        this.Flush();
        return this.DisplayList.ToArray();
      }
    }

    private class DisplayListRecoder
    {
      private Dictionary<byte, int> NrParams = new Dictionary<byte, int>()
      {
        {
          (byte) 0,
          0
        },
        {
          (byte) 16,
          1
        },
        {
          (byte) 17,
          0
        },
        {
          (byte) 18,
          1
        },
        {
          (byte) 19,
          1
        },
        {
          (byte) 20,
          1
        },
        {
          (byte) 21,
          0
        },
        {
          (byte) 22,
          16
        },
        {
          (byte) 23,
          12
        },
        {
          (byte) 24,
          16
        },
        {
          (byte) 25,
          12
        },
        {
          (byte) 26,
          9
        },
        {
          (byte) 27,
          3
        },
        {
          (byte) 28,
          3
        },
        {
          (byte) 32,
          1
        },
        {
          (byte) 33,
          1
        },
        {
          (byte) 34,
          1
        },
        {
          (byte) 35,
          2
        },
        {
          (byte) 36,
          1
        },
        {
          (byte) 37,
          1
        },
        {
          (byte) 38,
          1
        },
        {
          (byte) 39,
          1
        },
        {
          (byte) 40,
          1
        },
        {
          (byte) 41,
          1
        },
        {
          (byte) 42,
          1
        },
        {
          (byte) 43,
          1
        },
        {
          (byte) 48,
          1
        },
        {
          (byte) 49,
          1
        },
        {
          (byte) 50,
          1
        },
        {
          (byte) 51,
          1
        },
        {
          (byte) 52,
          32
        },
        {
          (byte) 64,
          1
        },
        {
          (byte) 65,
          0
        },
        {
          (byte) 80,
          1
        },
        {
          (byte) 96,
          1
        },
        {
          (byte) 112,
          3
        },
        {
          (byte) 113,
          2
        },
        {
          (byte) 114,
          1
        },
        {
          byte.MaxValue,
          0
        }
      };
      private byte[] DL;

      public DisplayListRecoder(byte[] DL)
      {
        if (DL == null)
          throw new ArgumentNullException("The display list can not be null.");
        this.DL = DL;
      }

      public byte[] GetDisplayList()
      {
        return this.DL;
      }

      public void RemoveNormals()
      {
        this.RemoveCommand((byte) 33);
      }

      private void RemoveCommand(byte Nr)
      {
        GlNitro.DisplayListEncoder displayListEncoder = new GlNitro.DisplayListEncoder();
        int offset = 0;
        int length = this.DL.Length;
        int[] numArray = new int[4];
        while (offset < length)
        {
          for (int index = 0; index < 4; ++index)
          {
            if (offset >= length)
            {
              numArray[index] = (int) byte.MaxValue;
            }
            else
            {
              numArray[index] = (int) this.DL[offset];
              ++offset;
            }
          }
          for (int index = 0; index < 4 && offset < length; ++index)
          {
            if (numArray[index] != (int) Nr)
              displayListEncoder.AddCommand((byte) numArray[index], Bytes.ReadMultipleBytesAsUInt32s(this.DL, offset, this.NrParams[Nr]));
            offset += this.NrParams[(byte) numArray[index]] * 4;
          }
        }
        this.DL = displayListEncoder.GetDisplayList();
      }
    }
  }
}
