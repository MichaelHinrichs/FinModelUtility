// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.SM64DS.BMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.SM64DS
{
  public class BMD
  {
    public BMD.BMDHeader Header;
    public BMD.BMDBone[] Bones;
    public BMD.BMDDisplayList[] DisplayLists;
    public BMD.BMDTexture[] Textures;
    public BMD.BMDPalette[] Palettes;
    public BMD.BMDMaterial[] Materials;

    public BMD(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      this.Header = new BMD.BMDHeader(er);
      er.BaseStream.Position = (long) this.Header.OffsetBones;
      this.Bones = new BMD.BMDBone[this.Header.NrBones];
      for (int Idx = 0; (long) Idx < (long) this.Header.NrBones; ++Idx)
        this.Bones[Idx] = new BMD.BMDBone(er, this.Bones, Idx, (int) this.Header.PosScale);
      er.BaseStream.Position = (long) this.Header.OffsetDisplayLists;
      this.DisplayLists = new BMD.BMDDisplayList[this.Header.NrDisplayLists];
      for (int index = 0; (long) index < (long) this.Header.NrDisplayLists; ++index)
        this.DisplayLists[index] = new BMD.BMDDisplayList(er, this.Header.OffsetTransformBoneMap);
      er.BaseStream.Position = (long) this.Header.OffsetTextures;
      this.Textures = new BMD.BMDTexture[this.Header.NrTextures];
      for (int index = 0; (long) index < (long) this.Header.NrTextures; ++index)
        this.Textures[index] = new BMD.BMDTexture(er);
      er.BaseStream.Position = (long) this.Header.OffsetPalettes;
      this.Palettes = new BMD.BMDPalette[this.Header.NrPalettes];
      for (int index = 0; (long) index < (long) this.Header.NrPalettes; ++index)
        this.Palettes[index] = new BMD.BMDPalette(er);
      er.BaseStream.Position = (long) this.Header.OffsetMaterials;
      this.Materials = new BMD.BMDMaterial[this.Header.NrMaterials];
      for (int index = 0; (long) index < (long) this.Header.NrMaterials; ++index)
        this.Materials[index] = new BMD.BMDMaterial(er);
      er.Close();
    }

    public void Render(BCA Bca, int BcaFrame)
    {
      bool flag1 = Bca != null;
      Matrix4[] matrix4Array = (Matrix4[]) null;
      if (flag1)
        matrix4Array = Bca.GetMatrices(this.Bones, BcaFrame, (int) this.Header.PosScale);
      int num1 = 0;
      while (true)
      {
        GlNitro.Nitro3DContext Context = new GlNitro.Nitro3DContext();
        foreach (BMD.BMDBone bone in this.Bones)
        {
          for (int index1 = 0; (long) index1 < (long) bone.NrDLMatPairs; ++index1)
          {
            BMD.BMDMaterial material = this.Materials[(int) bone.PolyMat.Values.ToArray<byte>()[index1]];
            BMD.BMDDisplayList displayList = this.DisplayLists[(int) bone.PolyMat.Keys.ToArray<byte>()[index1]];
            byte num2 = bone.PolyMat.Values.ToArray<byte>()[index1];
            bool flag2;
            if (material.TexID != -1 && this.Textures[material.TexID].Fmt != Graphic.GXTexFmt.GX_TEXFMT_NONE)
            {
              flag2 = true;
              if ((this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3 || ((int) (material.polyAttr >> 16) & 31) != 31) && num1 != 1)
                flag2 = false;
              if ((this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || (this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256) || (this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || this.Textures[material.TexID].Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)) && ((int) (material.polyAttr >> 16) & 31) == 31 && num1 != 0)
                flag2 = false;
            }
            else
              flag2 = true;
            if (flag2)
            {
              Gl.glBindTexture(3553, (int) num2 + 1);
              Gl.glMatrixMode(5890);
              if (material.TexID == -1)
                Gl.glLoadIdentity();
              else
                Gl.glLoadMatrixf(material.GetMatrix((int) this.Textures[material.TexID].S, (int) this.Textures[material.TexID].T));
              Gl.glEnable(3008);
              Gl.glAlphaFunc(516, 0.0f);
              Context.LightEnabled[0] = ((int) material.polyAttr & 1) == 1;
              Context.LightEnabled[1] = ((int) (material.polyAttr >> 1) & 1) == 1;
              Context.LightEnabled[2] = ((int) (material.polyAttr >> 2) & 1) == 1;
              Context.LightEnabled[3] = ((int) (material.polyAttr >> 3) & 1) == 1;
              Color color1 = Graphic.ConvertABGR1555((short) ((int) material.diffAmb & (int) short.MaxValue));
              Color color2 = Graphic.ConvertABGR1555((short) ((int) (material.diffAmb >> 16) & (int) short.MaxValue));
              Context.DiffuseColor = color1;
              Context.AmbientColor = color2;
              Color color3 = Graphic.ConvertABGR1555((short) ((int) material.specEmi & (int) short.MaxValue));
              Color color4 = Graphic.ConvertABGR1555((short) ((int) (material.specEmi >> 16) & (int) short.MaxValue));
              Context.SpecularColor = color3;
              Context.EmissionColor = color4;
              switch (material.polyAttr >> 14 & 1U)
              {
                case 0:
                  Gl.glDepthFunc(513);
                  break;
                case 1:
                  Gl.glDepthFunc(514);
                  break;
              }
              int num3 = -1;
              switch (material.polyAttr >> 4 & 3U)
              {
                case 0:
                  num3 = 8448;
                  break;
                case 1:
                  num3 = 8449;
                  break;
                case 2:
                  num3 = 8448;
                  break;
                case 3:
                  num3 = 8448;
                  break;
              }
              Gl.glTexEnvi(8960, 8704, num3);
              Context.Alpha = (int) (material.polyAttr >> 16) & 31;
              if (((int) (material.diffAmb >> 15) & 1) == 1)
                Gl.glColor4f((float) color1.R / (float) byte.MaxValue, (float) color1.G / (float) byte.MaxValue, (float) color1.B / (float) byte.MaxValue, (float) Context.Alpha / 31f);
              else
                Gl.glColor4f(0.0f, 0.0f, 0.0f, (float) Context.Alpha / 31f);
              Context.UseSpecularReflectionTable = ((int) (material.specEmi >> 15) & 1) == 1;
              int mode = -1;
              switch (material.polyAttr >> 6 & 3U)
              {
                case 0:
                  mode = 1032;
                  break;
                case 1:
                  mode = 1028;
                  break;
                case 2:
                  mode = 1029;
                  break;
                case 3:
                  mode = 0;
                  break;
              }
              Gl.glCullFace(mode);
              Gl.glMatrixMode(5888);
              Gl.glDisable(3168);
              Gl.glDisable(3169);
            }
            MTX44[] mtX44Array = new MTX44[31];
            int index2 = 0;
            foreach (ushort boneId in displayList.DL.BoneIDs)
            {
              if (!flag1)
              {
                mtX44Array[index2] = (MTX44) this.Bones[(int) boneId].GetMatrix();
                ++index2;
              }
              else
              {
                mtX44Array[index2] = this.Matrix4ToMTX44(matrix4Array[(int) boneId]);
                ++index2;
              }
            }
            Context.MatrixStack = mtX44Array;
            if (flag2)
            {
              GlNitro.glNitroGx(displayList.DL.Data, new MTX44(), ref Context, (int) this.Header.PosScale, false);
              Gl.glEnd();
            }
          }
        }
        if (num1 == 0)
          ++num1;
        else
          break;
      }
    }

    private MTX44 Matrix4ToMTX44(Matrix4 Mtx)
    {
      return (MTX44) new float[16]
      {
        Mtx.M11,
        Mtx.M12,
        Mtx.M13,
        Mtx.M14,
        Mtx.M21,
        Mtx.M22,
        Mtx.M23,
        Mtx.M24,
        Mtx.M31,
        Mtx.M32,
        Mtx.M33,
        Mtx.M34,
        Mtx.M41,
        Mtx.M42,
        Mtx.M43,
        Mtx.M44
      };
    }

    public class BMDHeader
    {
      public uint PosScale;
      public uint NrBones;
      public uint OffsetBones;
      public uint NrDisplayLists;
      public uint OffsetDisplayLists;
      public uint NrTextures;
      public uint OffsetTextures;
      public uint NrPalettes;
      public uint OffsetPalettes;
      public uint NrMaterials;
      public uint OffsetMaterials;
      public uint OffsetTransformBoneMap;
      public uint OffsetTexPalDataBlock;

      public BMDHeader(EndianBinaryReader er)
      {
        this.PosScale = (uint) (1 << er.ReadInt32());
        this.NrBones = er.ReadUInt32();
        this.OffsetBones = er.ReadUInt32();
        this.NrDisplayLists = er.ReadUInt32();
        this.OffsetDisplayLists = er.ReadUInt32();
        this.NrTextures = er.ReadUInt32();
        this.OffsetTextures = er.ReadUInt32();
        this.NrPalettes = er.ReadUInt32();
        this.OffsetPalettes = er.ReadUInt32();
        this.NrMaterials = er.ReadUInt32();
        this.OffsetMaterials = er.ReadUInt32();
        this.OffsetTransformBoneMap = er.ReadUInt32();
        this.OffsetTexPalDataBlock = er.ReadUInt32();
      }
    }

    public class BMDBone
    {
      public uint BoneID;
      public uint OffsetBoneName;
      public string Name;
      public short OffsetParent;
      public ushort HasChildren;
      public uint OffsetNextSibling;
      public float Sx;
      public float Sy;
      public float Sz;
      public short Rx;
      public short Ry;
      public short Rz;
      public ushort Padding;
      public float Tx;
      public float Ty;
      public float Tz;
      public uint NrDLMatPairs;
      public uint OffsetMatIDList;
      public uint OffsetDLIDList;
      public Dictionary<byte, byte> PolyMat;
      public uint Flags;
      public Matrix4 Mtx;

      public BMDBone(EndianBinaryReader er, BMD.BMDBone[] Bones, int Idx, int Posscale)
      {
        this.BoneID = er.ReadUInt32();
        this.OffsetBoneName = er.ReadUInt32();
        long position1 = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetBoneName;
        this.Name = er.ReadStringNT(Encoding.ASCII);
        er.BaseStream.Position = position1;
        this.OffsetParent = er.ReadInt16();
        this.HasChildren = er.ReadUInt16();
        this.OffsetNextSibling = er.ReadUInt32();
        this.Sx = er.ReadSingleInt32Exp12();
        this.Sy = er.ReadSingleInt32Exp12();
        this.Sz = er.ReadSingleInt32Exp12();
        this.Rx = er.ReadInt16();
        this.Ry = er.ReadInt16();
        this.Rz = er.ReadInt16();
        this.Padding = er.ReadUInt16();
        this.Tx = er.ReadSingleInt32Exp12();
        this.Ty = er.ReadSingleInt32Exp12();
        this.Tz = er.ReadSingleInt32Exp12();
        this.NrDLMatPairs = er.ReadUInt32();
        this.OffsetMatIDList = er.ReadUInt32();
        this.OffsetDLIDList = er.ReadUInt32();
        long position2 = er.BaseStream.Position;
        this.PolyMat = new Dictionary<byte, byte>();
        for (int index = 0; (long) index < (long) this.NrDLMatPairs; ++index)
        {
          er.BaseStream.Position = (long) this.OffsetDLIDList + (long) index;
          byte key = er.ReadByte();
          er.BaseStream.Position = (long) this.OffsetMatIDList + (long) index;
          byte num = er.ReadByte();
          this.PolyMat.Add(key, num);
        }
        er.BaseStream.Position = position2;
        this.Flags = er.ReadUInt32();
        this.Mtx = this.GetMatrix(Bones, Idx, Posscale);
      }

      private Matrix4 GetMatrix(BMD.BMDBone[] Bones, int Idx, int Posscale)
      {
        Matrix4 right = Matrix4.Scale(this.Sx, this.Sy, this.Sz);
        Matrix4 rotationX = Matrix4.CreateRotationX((float) ((double) this.Rx * 3.14159274101257 / 2048.0));
        Matrix4 rotationY = Matrix4.CreateRotationY((float) ((double) this.Ry * 3.14159274101257 / 2048.0));
        Matrix4 rotationZ = Matrix4.CreateRotationZ((float) ((double) this.Rz * 3.14159274101257 / 2048.0));
        Matrix4 translation = Matrix4.CreateTranslation(this.Tx / (float) Posscale, this.Ty / (float) Posscale, this.Tz / (float) Posscale);
        Matrix4 result = Matrix4.Identity;
        Matrix4.Mult(ref result, ref right, out result);
        Matrix4.Mult(ref result, ref rotationX, out result);
        Matrix4.Mult(ref result, ref rotationY, out result);
        Matrix4.Mult(ref result, ref rotationZ, out result);
        Matrix4.Mult(ref result, ref translation, out result);
        if (this.OffsetParent < (short) 0)
          Matrix4.Mult(ref result, ref Bones[Idx + (int) this.OffsetParent].Mtx, out result);
        return result;
      }

      public float[] GetMatrix()
      {
        return new float[16]
        {
          this.Mtx.M11,
          this.Mtx.M12,
          this.Mtx.M13,
          this.Mtx.M14,
          this.Mtx.M21,
          this.Mtx.M22,
          this.Mtx.M23,
          this.Mtx.M24,
          this.Mtx.M31,
          this.Mtx.M32,
          this.Mtx.M33,
          this.Mtx.M34,
          this.Mtx.M41,
          this.Mtx.M42,
          this.Mtx.M43,
          this.Mtx.M44
        };
      }

      public override string ToString()
      {
        return this.Name;
      }
    }

    public class BMDDisplayList
    {
      public uint Unknown;
      public uint OffsetDL;
      public BMD.BMDDisplayList.DisplayList DL;

      public BMDDisplayList(EndianBinaryReader er, uint OffsetTransformBoneMap)
      {
        this.Unknown = er.ReadUInt32();
        this.OffsetDL = er.ReadUInt32();
        long position = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetDL;
        this.DL = new BMD.BMDDisplayList.DisplayList(er, OffsetTransformBoneMap);
        er.BaseStream.Position = position;
      }

      public class DisplayList
      {
        public uint NrTransform;
        public uint OffsetTransformList;
        public uint DisplayListDataSize;
        public uint OffsetDisplayListData;
        public ushort[] BoneIDs;
        public byte[] Data;

        public DisplayList(EndianBinaryReader er, uint OffsetTransformBoneMap)
        {
          this.NrTransform = er.ReadUInt32();
          this.OffsetTransformList = er.ReadUInt32();
          this.DisplayListDataSize = er.ReadUInt32();
          this.OffsetDisplayListData = er.ReadUInt32();
          long position = er.BaseStream.Position;
          this.BoneIDs = new ushort[this.NrTransform];
          for (int index = 0; (long) index < (long) this.NrTransform; ++index)
          {
            er.BaseStream.Position = (long) this.OffsetTransformList + (long) index;
            byte num = er.ReadByte();
            er.BaseStream.Position = (long) OffsetTransformBoneMap + (long) (2 * (int) num);
            this.BoneIDs[index] = er.ReadUInt16();
          }
          er.BaseStream.Position = (long) this.OffsetDisplayListData;
          this.Data = er.ReadBytes((int) this.DisplayListDataSize);
          er.BaseStream.Position = position;
        }
      }
    }

    public class BMDTexture
    {
      public uint OffsetTexName;
      public string Name;
      public uint OffsetTexData;
      public uint TexDataSize;
      public ushort Width;
      public ushort Height;
      public uint texImageParam;
      public ushort S;
      public ushort T;
      public Graphic.GXTexFmt Fmt;
      public bool TransparentColor;
      public byte[] Data;
      public byte[] Data4x4;

      public BMDTexture(EndianBinaryReader er)
      {
        this.OffsetTexName = er.ReadUInt32();
        long position1 = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetTexName;
        this.Name = er.ReadStringNT(Encoding.ASCII);
        er.BaseStream.Position = position1;
        this.OffsetTexData = er.ReadUInt32();
        this.TexDataSize = er.ReadUInt32();
        this.Width = er.ReadUInt16();
        this.Height = er.ReadUInt16();
        this.texImageParam = er.ReadUInt32();
        this.S = (ushort) (8U << (int) ((this.texImageParam & 7340032U) >> 20));
        this.T = (ushort) (8U << (int) ((this.texImageParam & 58720256U) >> 23));
        this.Fmt = (Graphic.GXTexFmt) ((this.texImageParam & 469762048U) >> 26);
        this.TransparentColor = ((int) (this.texImageParam >> 29) & 1) == 1;
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetTexData;
        this.Data = er.ReadBytes((int) this.TexDataSize);
        if (this.Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4)
          this.Data4x4 = er.ReadBytes((int) this.TexDataSize / 2);
        er.BaseStream.Position = position2;
      }

      public System.Drawing.Bitmap ToBitmap(BMD.BMDPalette Palette)
      {
        return Graphic.ConvertData(this.Data, Palette?.Data, this.Data4x4, 0, (int) this.S, (int) this.T, this.Fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, this.TransparentColor, true);
      }

      public override string ToString()
      {
        return this.Name;
      }
    }

    public class BMDPalette
    {
      public uint OffsetPalName;
      public string Name;
      public uint OffsetPalData;
      public uint PalDataSize;
      public uint Unknown;
      public byte[] Data;

      public BMDPalette(EndianBinaryReader er)
      {
        this.OffsetPalName = er.ReadUInt32();
        long position1 = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetPalName;
        this.Name = er.ReadStringNT(Encoding.ASCII);
        er.BaseStream.Position = position1;
        this.OffsetPalData = er.ReadUInt32();
        this.PalDataSize = er.ReadUInt32();
        this.Unknown = er.ReadUInt32();
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetPalData;
        this.Data = er.ReadBytes((int) this.PalDataSize);
        er.BaseStream.Position = position2;
      }

      public override string ToString()
      {
        return this.Name;
      }
    }

    public class BMDMaterial
    {
      public uint OffsetMatName;
      public string Name;
      public int TexID;
      public int PalID;
      public float Ss;
      public float St;
      public int R;
      public float Ts;
      public float Tt;
      public uint texImageParam;
      public uint polyAttr;
      public uint diffAmb;
      public uint specEmi;

      public BMDMaterial(EndianBinaryReader er)
      {
        this.OffsetMatName = er.ReadUInt32();
        long position = er.BaseStream.Position;
        er.BaseStream.Position = (long) this.OffsetMatName;
        this.Name = er.ReadStringNT(Encoding.ASCII);
        er.BaseStream.Position = position;
        this.TexID = er.ReadInt32();
        this.PalID = er.ReadInt32();
        this.Ss = er.ReadSingleInt32Exp12();
        this.St = er.ReadSingleInt32Exp12();
        this.R = er.ReadInt32();
        this.Ts = er.ReadSingleInt32Exp12();
        this.Tt = er.ReadSingleInt32Exp12();
        this.texImageParam = er.ReadUInt32();
        this.polyAttr = er.ReadUInt32();
        this.diffAmb = er.ReadUInt32();
        this.specEmi = er.ReadUInt32();
      }

      public float[] GetMatrix(int TexWidth, int TexHeight)
      {
        Matrix4 right = Matrix4.Scale(this.Ss / (float) TexWidth, this.St / (float) TexHeight, 1f);
        Matrix4 rotationZ = Matrix4.CreateRotationZ((float) ((double) this.R * 3.14159274101257 / 2048.0));
        Matrix4 translation = Matrix4.CreateTranslation(this.Ts, this.Tt, 0.0f);
        Matrix4 matrix4 = Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(Matrix4.Identity, right), rotationZ), translation);
        return new float[16]
        {
          matrix4.M11,
          matrix4.M12,
          matrix4.M13,
          matrix4.M14,
          matrix4.M21,
          matrix4.M22,
          matrix4.M23,
          matrix4.M24,
          matrix4.M31,
          matrix4.M32,
          matrix4.M33,
          matrix4.M34,
          matrix4.M41,
          matrix4.M42,
          matrix4.M43,
          matrix4.M44
        };
      }

      public override string ToString()
      {
        return this.Name;
      }
    }
  }
}
