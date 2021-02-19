// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.MPDS.HBDF
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.MPDS
{
  public class HBDF
  {
    public const string Signature = "HBDF";
    public DataBlockHeader Header;
    public HBDF.MDLFBlock[] MDLFBlocks;
    public HBDF.TEXSBlock[] TEXSBlocks;
    public HBDF.ANMFBlock[] ANMFBlocks;

    public HBDF(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new DataBlockHeader(er, nameof (HBDF), out OK);
      if (!OK)
      {
        int num = (int) MessageBox.Show("Error 1");
      }
      else
      {
        List<HBDF.MDLFBlock> mdlfBlockList = new List<HBDF.MDLFBlock>();
        List<HBDF.TEXSBlock> texsBlockList = new List<HBDF.TEXSBlock>();
        while (er.BaseStream.Length != er.BaseStream.Position)
        {
          switch (er.ReadString(Encoding.ASCII, 4)) {
            case "MDLF":
              er.BaseStream.Position -= 4L;
              mdlfBlockList.Add(new HBDF.MDLFBlock(er, out OK));
              break;
            case "TEXS":
              er.BaseStream.Position -= 4L;
              texsBlockList.Add(new HBDF.TEXSBlock(er, out OK));
              break;
            case "ANMF":
              er.ReadBytes(er.ReadInt32());
              break;
          }
        }
        this.MDLFBlocks = mdlfBlockList.ToArray();
        this.TEXSBlocks = texsBlockList.ToArray();
      }
      er.Close();
    }

    public class MDLFBlock
    {
      public const string Signature = "MDLF";
      public DataBlockHeader Header;
      public uint Unknown1;
      public uint Unknown2;
      public uint Unknown3;
      public uint Unknown4;
      public uint Unknown5;
      public uint Unknown6;
      public uint Unknown7;
      public uint Unknown8;
      public uint Unknown9;
      public uint Unknown10;
      public short NrOBJO;
      public short NrMaterials;
      public short NrTextures;
      public short NrMatrixBlock;
      public HBDF.MDLFBlock.MaterialBlock[] Materials;
      public HBDF.MDLFBlock.TextureBlock[] Textures;
      public HBDF.MDLFBlock.MatrixBlock[] MatrixBlocks;
      public HBDF.MDLFBlock.OBJOBlock[] OBJOBlocks;
      public HBDF.MDLFBlock.STRBBlock Names;

      public MDLFBlock(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "MDLF", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.Unknown3 = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          this.Unknown5 = er.ReadUInt32();
          this.Unknown6 = er.ReadUInt32();
          this.Unknown7 = er.ReadUInt32();
          this.Unknown8 = er.ReadUInt32();
          this.Unknown9 = er.ReadUInt32();
          this.Unknown10 = er.ReadUInt32();
          this.NrOBJO = er.ReadInt16();
          this.NrMaterials = er.ReadInt16();
          this.NrTextures = er.ReadInt16();
          this.NrMatrixBlock = er.ReadInt16();
          this.Materials = new HBDF.MDLFBlock.MaterialBlock[(int) this.NrMaterials];
          for (int index = 0; index < (int) this.NrMaterials; ++index)
            this.Materials[index] = new HBDF.MDLFBlock.MaterialBlock(er);
          this.Textures = new HBDF.MDLFBlock.TextureBlock[(int) this.NrTextures];
          for (int index = 0; index < (int) this.NrTextures; ++index)
            this.Textures[index] = new HBDF.MDLFBlock.TextureBlock(er);
          this.MatrixBlocks = new HBDF.MDLFBlock.MatrixBlock[(int) this.NrMatrixBlock];
          for (int index = 0; index < (int) this.NrMatrixBlock; ++index)
            this.MatrixBlocks[index] = new HBDF.MDLFBlock.MatrixBlock(er);
          this.OBJOBlocks = new HBDF.MDLFBlock.OBJOBlock[(int) this.NrOBJO];
          for (int index = 0; index < (int) this.NrOBJO; ++index)
          {
            this.OBJOBlocks[index] = new HBDF.MDLFBlock.OBJOBlock(er, out OK1);
            if (!OK1)
            {
              OK = false;
              return;
            }
          }
          this.Names = new HBDF.MDLFBlock.STRBBlock(er, out OK1);
          if (!OK1)
            OK = false;
          else
            OK = true;
        }
      }

      public void Render(HBDF.TEXSBlock Textures, int Texoffset = 1)
      {
        int num1 = 0;
        while (true)
        {
          MTX44 mtX44 = new MTX44();
          GlNitro.Nitro3DContext Context = new GlNitro.Nitro3DContext();
          bool flag = true;
          Gl.glDisable(2896);
          int num2 = (int) this.NrMaterials + (int) this.NrTextures * 2;
          foreach (HBDF.MDLFBlock.OBJOBlock objoBlock in this.OBJOBlocks)
          {
            string str1 = this.Names.Strings[num2++];
            if (objoBlock.MESH != null)
            {
              if (objoBlock.MESH.NrUnknownBlock > (short) 1)
              {
                for (int index1 = 0; index1 < (int) objoBlock.MESH.NrUnknownBlock; ++index1)
                {
                  Color color1 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown2 & (int) short.MaxValue));
                  Color color2 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown2 >> 16) & (int) short.MaxValue));
                  Context.DiffuseColor = color1;
                  Context.AmbientColor = color2;
                  Color color3 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown3 & (int) short.MaxValue));
                  Color color4 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown3 >> 16) & (int) short.MaxValue));
                  Context.SpecularColor = color3;
                  Context.EmissionColor = color4;
                  Context.LightEnabled[0] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown6 >> 16) & 1) == 1;
                  Context.LightEnabled[1] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown6 >> 17) & 1) == 1;
                  Context.LightEnabled[2] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown6 >> 18) & 1) == 1;
                  Context.LightEnabled[3] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].Unknown6 >> 19) & 1) == 1;
                  if (this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].TexID != (short) -1 && Textures != null)
                  {
                    flag = true;
                    string str2 = this.Names.Strings[(int) this.NrMaterials + (int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].TexID * 2 + 1];
                    int index2 = 0;
                    for (int index3 = 0; index3 < Textures.TEXOBlocks.Length; ++index3)
                    {
                      if (Textures.TEXOBlocks[index3].TexName.Name == str2)
                      {
                        index2 = index3;
                        break;
                      }
                    }
                    if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3) && num1 != 1)
                      flag = false;
                    if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || (Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256) || (Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)) && num1 != 0)
                      flag = false;
                    Gl.glMatrixMode(5890);
                    Gl.glBindTexture(3553, (int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[index1].Unknown1].TexID + Texoffset);
                    Gl.glLoadIdentity();
                    Gl.glTranslatef(this.Textures[index2].Tx, this.Textures[index2].Ty, this.Textures[index2].Tz);
                    Gl.glScalef(1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Width, 1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Height, 1f);
                    Gl.glScalef(this.Textures[index2].Sx, this.Textures[index2].Sy, this.Textures[index2].Sz);
                    Gl.glMatrixMode(5888);
                  }
                  else
                    Gl.glBindTexture(3553, 0);
                  MTX44[] mtX44Array = new MTX44[31];
                  if (flag)
                  {
                    MTX44 matrix = objoBlock.GetMatrix(this.OBJOBlocks);
                    matrix.Scale(objoBlock.MESH.Sx, objoBlock.MESH.Sy, objoBlock.MESH.Sz);
                    Context.MatrixStack = mtX44Array;
                    GlNitro.glNitroGx(((IEnumerable<byte>) objoBlock.MESH.Data).ToList<byte>().GetRange((int) objoBlock.MESH.UnknownBlocks[index1].Unknown3 * 4, (int) objoBlock.MESH.UnknownBlocks[index1].Unknown4 * 4).ToArray(), matrix, ref Context, 1, false);
                    Gl.glEnd();
                  }
                }
              }
              else
              {
                Color color1 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown2 & (int) short.MaxValue));
                Color color2 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown2 >> 16) & (int) short.MaxValue));
                Context.DiffuseColor = color1;
                Context.AmbientColor = color2;
                Color color3 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown3 & (int) short.MaxValue));
                Color color4 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown3 >> 16) & (int) short.MaxValue));
                Context.SpecularColor = color3;
                Context.EmissionColor = color4;
                Context.LightEnabled[0] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown6 >> 16) & 1) == 1;
                Context.LightEnabled[1] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown6 >> 17) & 1) == 1;
                Context.LightEnabled[2] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown6 >> 18) & 1) == 1;
                Context.LightEnabled[3] = ((int) (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].Unknown6 >> 19) & 1) == 1;
                if (this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].TexID != (short) -1 && Textures != null)
                {
                  flag = true;
                  string str2 = this.Names.Strings[(int) this.NrMaterials + (int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].TexID * 2 + 1];
                  int index1 = 0;
                  for (int index2 = 0; index2 < Textures.TEXOBlocks.Length; ++index2)
                  {
                    if (Textures.TEXOBlocks[index2].TexName.Name == str2)
                    {
                      index1 = index2;
                      break;
                    }
                  }
                  if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3) && num1 != 1)
                    flag = false;
                  if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || (Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256) || (Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)) && num1 != 0)
                    flag = false;
                  Gl.glMatrixMode(5890);
                  Gl.glBindTexture(3553, (int) this.Materials[(int) objoBlock.MESH.UnknownBlocks[0].Unknown1].TexID + Texoffset);
                  Gl.glLoadIdentity();
                  Gl.glTranslatef(this.Textures[index1].Tx, this.Textures[index1].Ty, this.Textures[index1].Tz);
                  Gl.glScalef(1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Width, 1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Height, 1f);
                  Gl.glScalef(this.Textures[index1].Sx, this.Textures[index1].Sy, this.Textures[index1].Sz);
                  Gl.glMatrixMode(5888);
                }
                else
                  Gl.glBindTexture(3553, 0);
                MTX44[] mtX44Array = new MTX44[31];
                if (flag)
                {
                  MTX44 matrix = objoBlock.GetMatrix(this.OBJOBlocks);
                  matrix.Scale(objoBlock.MESH.Sx, objoBlock.MESH.Sy, objoBlock.MESH.Sz);
                  Context.MatrixStack = mtX44Array;
                  GlNitro.glNitroGx(objoBlock.MESH.Data, matrix, ref Context, 1, false);
                  Gl.glEnd();
                }
              }
            }
            else if (objoBlock.SKIN != null)
            {
              if (objoBlock.SKIN.NrUnknownBlock > (short) 1)
              {
                for (int index1 = 0; index1 < (int) objoBlock.SKIN.NrUnknownBlock; ++index1)
                {
                  Color color1 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown2 & (int) short.MaxValue));
                  Color color2 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown2 >> 16) & (int) short.MaxValue));
                  Context.DiffuseColor = color1;
                  Context.AmbientColor = color2;
                  Color color3 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown3 & (int) short.MaxValue));
                  Color color4 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown3 >> 16) & (int) short.MaxValue));
                  Context.SpecularColor = color3;
                  Context.EmissionColor = color4;
                  Context.UseSpecularReflectionTable = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown3 >> 15) & 1) == 1;
                  Context.LightEnabled[0] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown6 >> 16) & 1) == 1;
                  Context.LightEnabled[1] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown6 >> 17) & 1) == 1;
                  Context.LightEnabled[2] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown6 >> 18) & 1) == 1;
                  Context.LightEnabled[3] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].Unknown6 >> 19) & 1) == 1;
                  if (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].TexID != (short) -1)
                  {
                    flag = true;
                    string str2 = this.Names.Strings[(int) this.NrMaterials + (int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].TexID * 2 + 1];
                    int index2 = 0;
                    for (int index3 = 0; index3 < Textures.TEXOBlocks.Length; ++index3)
                    {
                      if (Textures.TEXOBlocks[index3].TexName.Name == str2)
                      {
                        index2 = index3;
                        break;
                      }
                    }
                    if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3) && num1 != 1)
                      flag = false;
                    if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || (Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256) || (Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)) && num1 != 0)
                      flag = false;
                    Gl.glMatrixMode(5890);
                    Gl.glBindTexture(3553, (int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[index1].Unknown1].TexID + Texoffset);
                    Gl.glLoadIdentity();
                    Gl.glScalef(1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Width, 1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index2].TexName.Name).Height, 1f);
                    Gl.glScalef(this.Textures[index2].Sx, this.Textures[index2].Sy, this.Textures[index2].Sz);
                    Gl.glMatrixMode(5888);
                  }
                  else
                    Gl.glBindTexture(3553, 0);
                  MTX44[] mtX44Array = new MTX44[31];
                  if (objoBlock.SKIN.ENVS != null)
                  {
                    for (int index2 = 0; (long) index2 < (long) objoBlock.SKIN.ENVS.NrOffsets; ++index2)
                    {
                      mtX44Array[index2 + 2] = objoBlock.GetMatrix(this.OBJOBlocks);
                      mtX44Array[index2 + 2].Scale(objoBlock.SKIN.Sx, objoBlock.SKIN.Sy, objoBlock.SKIN.Sz);
                    }
                  }
                  if (flag)
                  {
                    MTX44 matrix = objoBlock.GetMatrix(this.OBJOBlocks);
                    matrix.Scale(objoBlock.SKIN.Sx, objoBlock.SKIN.Sy, objoBlock.SKIN.Sz);
                    Context.MatrixStack = mtX44Array;
                    GlNitro.glNitroGx(((IEnumerable<byte>) objoBlock.SKIN.Data).ToList<byte>().GetRange((int) objoBlock.SKIN.UnknownBlocks[index1].Unknown3 * 4, (int) objoBlock.SKIN.UnknownBlocks[index1].Unknown4 * 4).ToArray(), matrix, ref Context, 1, false);
                    Gl.glEnd();
                  }
                }
              }
              else
              {
                Color color1 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown2 & (int) short.MaxValue));
                Color color2 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown2 >> 16) & (int) short.MaxValue));
                Context.DiffuseColor = color1;
                Context.AmbientColor = color2;
                Color color3 = Graphic.ConvertABGR1555((short) ((int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown3 & (int) short.MaxValue));
                Color color4 = Graphic.ConvertABGR1555((short) ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown3 >> 16) & (int) short.MaxValue));
                Context.SpecularColor = color3;
                Context.EmissionColor = color4;
                Context.LightEnabled[0] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown6 >> 16) & 1) == 1;
                Context.LightEnabled[1] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown6 >> 17) & 1) == 1;
                Context.LightEnabled[2] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown6 >> 18) & 1) == 1;
                Context.LightEnabled[3] = ((int) (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].Unknown6 >> 19) & 1) == 1;
                if (this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].TexID != (short) -1)
                {
                  flag = true;
                  string str2 = this.Names.Strings[(int) this.NrMaterials + (int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].TexID * 2 + 1];
                  int index1 = 0;
                  for (int index2 = 0; index2 < Textures.TEXOBlocks.Length; ++index2)
                  {
                    if (Textures.TEXOBlocks[index2].TexName.Name == str2)
                    {
                      index1 = index2;
                      break;
                    }
                  }
                  if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3) && num1 != 1)
                    flag = false;
                  if ((Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || (Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256) || (Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)) && num1 != 0)
                    flag = false;
                  Gl.glMatrixMode(5890);
                  Gl.glBindTexture(3553, (int) this.Materials[(int) objoBlock.SKIN.UnknownBlocks[0].Unknown1].TexID + Texoffset);
                  Gl.glLoadIdentity();
                  Gl.glScalef(1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Width, 1f / (float) Textures.GetIMGOByName(Textures.TEXOBlocks[index1].TexName.Name).Height, 1f);
                  Gl.glScalef(this.Textures[index1].Sx, this.Textures[index1].Sy, this.Textures[index1].Sz);
                  Gl.glMatrixMode(5888);
                }
                else
                  Gl.glBindTexture(3553, 0);
                MTX44[] mtX44Array = new MTX44[31];
                if (objoBlock.SKIN.ENVS != null)
                {
                  for (int index = 0; (long) index < (long) objoBlock.SKIN.ENVS.NrOffsets; ++index)
                  {
                    mtX44Array[index + 2] = objoBlock.GetMatrix(this.OBJOBlocks);
                    mtX44Array[index + 2].Scale(objoBlock.SKIN.Sx, objoBlock.SKIN.Sy, objoBlock.SKIN.Sz);
                  }
                }
                if (flag)
                {
                  MTX44 matrix = objoBlock.GetMatrix(this.OBJOBlocks);
                  matrix.Scale(objoBlock.SKIN.Sx, objoBlock.SKIN.Sy, objoBlock.SKIN.Sz);
                  Context.MatrixStack = mtX44Array;
                  GlNitro.glNitroGx(objoBlock.SKIN.Data, matrix, ref Context, 1, false);
                  Gl.glEnd();
                }
              }
            }
          }
          if (num1 == 0)
            ++num1;
          else
            break;
        }
      }

      public class MaterialBlock
      {
        public uint Unknown1;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public short TexID;
        public ushort Unknown5;
        public uint Unknown6;

        public MaterialBlock(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.Unknown3 = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          this.TexID = er.ReadInt16();
          this.Unknown5 = er.ReadUInt16();
          this.Unknown6 = er.ReadUInt32();
        }
      }

      public class TextureBlock
      {
        public uint Unknown1;
        public uint texImageParam;
        public float Tx;
        public float Ty;
        public float Tz;
        public uint Unknown5;
        public uint Unknown6;
        public uint Unknown7;
        public float Sx;
        public float Sy;
        public float Sz;
        public ushort Unknown8;
        public ushort Unknown9;

        public TextureBlock(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt32();
          this.texImageParam = er.ReadUInt32();
          this.Tx = er.ReadSingleInt32Exp12();
          this.Ty = er.ReadSingleInt32Exp12();
          this.Tz = er.ReadSingleInt32Exp12();
          this.Unknown5 = er.ReadUInt32();
          this.Unknown6 = er.ReadUInt32();
          this.Unknown7 = er.ReadUInt32();
          this.Sx = er.ReadSingleInt32Exp12();
          this.Sy = er.ReadSingleInt32Exp12();
          this.Sz = er.ReadSingleInt32Exp12();
          this.Unknown8 = er.ReadUInt16();
          this.Unknown9 = er.ReadUInt16();
          if ((double) this.Tx == 0.0 && (double) this.Ty == 0.0 && (double) this.Tz == 0.0)
            return;
          Debugger.Break();
        }

        public float[] GetMatrix()
        {
          return (float[]) new MTX44();
        }
      }

      public class MatrixBlock
      {
        public uint Index;
        public float Tx;
        public float Ty;
        public float Tz;
        public int Rx;
        public int Ry;
        public int Rz;
        public float Sx;
        public float Sy;
        public float Sz;

        public MatrixBlock(EndianBinaryReader er)
        {
          this.Index = er.ReadUInt32();
          this.Tx = er.ReadSingleInt32Exp12();
          this.Ty = er.ReadSingleInt32Exp12();
          this.Tz = er.ReadSingleInt32Exp12();
          this.Rx = er.ReadInt32();
          this.Ry = er.ReadInt32();
          this.Rz = er.ReadInt32();
          this.Sx = er.ReadSingleInt32Exp12();
          this.Sy = er.ReadSingleInt32Exp12();
          this.Sz = er.ReadSingleInt32Exp12();
        }

        public MTX44 GetMatrix(MTX44 Base)
        {
          MTX44 mtX44_1 = Base;
          mtX44_1.translate(this.Tx, this.Ty, this.Tz);
          Matrix4 Mtx = Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(Matrix4.Identity, Matrix4.CreateRotationX((float) this.Rx / 16384f)), Matrix4.CreateRotationY((float) this.Ry / 16384f)), Matrix4.CreateRotationZ((float) this.Rz / 16384f));
          MTX44 mtX44_2 = mtX44_1.MultMatrix(this.Matrix4ToMTX44(Mtx));
          mtX44_2.Scale(this.Sx, this.Sy, this.Sz);
          return mtX44_2;
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
      }

      public class OBJOBlock
      {
        public const string Signature = "OBJO";
        public DataBlockHeader Header;
        public ushort Unknown1;
        public short Parent;
        public uint Unknown3;
        public uint Unknown4;
        public float Tx;
        public float Ty;
        public float Tz;
        public int Rx;
        public int Ry;
        public int Rz;
        public float Sx;
        public float Sy;
        public float Sz;
        public HBDF.MDLFBlock.OBJOBlock.MESHBlock MESH;
        public HBDF.MDLFBlock.OBJOBlock.SKINBlock SKIN;

        public OBJOBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "OBJO", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Unknown1 = er.ReadUInt16();
            this.Parent = er.ReadInt16();
            this.Unknown3 = er.ReadUInt32();
            this.Unknown4 = er.ReadUInt32();
            this.Tx = er.ReadSingleInt32Exp12();
            this.Ty = er.ReadSingleInt32Exp12();
            this.Tz = er.ReadSingleInt32Exp12();
            this.Rx = er.ReadInt32();
            this.Ry = er.ReadInt32();
            this.Rz = er.ReadInt32();
            this.Sx = er.ReadSingleInt32Exp12();
            this.Sy = er.ReadSingleInt32Exp12();
            this.Sz = er.ReadSingleInt32Exp12();
            if (this.Unknown1 == (ushort) 2)
            {
              this.MESH = new HBDF.MDLFBlock.OBJOBlock.MESHBlock(er, out OK1);
              if (!OK1)
              {
                OK = false;
                return;
              }
            }
            else if (this.Unknown1 == (ushort) 3)
            {
              this.SKIN = new HBDF.MDLFBlock.OBJOBlock.SKINBlock(er, out OK1);
              if (!OK1)
              {
                OK = false;
                return;
              }
            }
            OK = true;
          }
        }

        public MTX44 GetMatrix(HBDF.MDLFBlock.OBJOBlock[] Blocks)
        {
          MTX44 mtX44_1 = this.Parent != (short) -1 ? Blocks[(int) this.Parent].GetMatrix(Blocks) : new MTX44();
          mtX44_1.translate(this.Tx, this.Ty, this.Tz);
          Matrix4 Mtx = Matrix4.Mult(Matrix4.Mult(Matrix4.Mult(Matrix4.Identity, Matrix4.CreateRotationX((float) this.Rx / 16384f)), Matrix4.CreateRotationY((float) this.Ry / 16384f)), Matrix4.CreateRotationZ((float) this.Rz / 16384f));
          MTX44 mtX44_2 = mtX44_1.MultMatrix(this.Matrix4ToMTX44(Mtx));
          mtX44_2.Scale(this.Sx, this.Sy, this.Sz);
          return mtX44_2;
        }

        private Vector3 vecCross_(Vector3 a, Vector3 b)
        {
          return new Vector3((float) (((double) a.Y * (double) b.Z - (double) a.Z * (double) b.Y) / 4096.0), (float) (((double) a.Z * (double) b.X - (double) a.X * (double) b.Z) / 4096.0), (float) (((double) a.X * (double) b.Y - (double) a.Y * (double) b.X) / 4096.0));
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

        public class MESHBlock
        {
          public const string Signature = "MESH";
          public DataBlockHeader Header;
          public float Sx;
          public float Sy;
          public float Sz;
          public uint Unknown1;
          public uint Unknown2;
          public uint Unknown3;
          public uint Unknown4;
          public short NrUnknownBlock;
          public ushort DLSize;
          public HBDF.MDLFBlock.OBJOBlock.MESHBlock.UnknownBlock[] UnknownBlocks;
          public byte[] Data;

          public MESHBlock(EndianBinaryReader er, out bool OK)
          {
            bool OK1;
            this.Header = new DataBlockHeader(er, "MESH", out OK1);
            if (!OK1)
            {
              OK = false;
            }
            else
            {
              this.Sx = er.ReadSingleInt32Exp12();
              this.Sy = er.ReadSingleInt32Exp12();
              this.Sz = er.ReadSingleInt32Exp12();
              this.Unknown1 = er.ReadUInt32();
              this.Unknown2 = er.ReadUInt32();
              this.Unknown3 = er.ReadUInt32();
              this.Unknown4 = er.ReadUInt32();
              this.NrUnknownBlock = er.ReadInt16();
              this.DLSize = er.ReadUInt16();
              this.UnknownBlocks = new HBDF.MDLFBlock.OBJOBlock.MESHBlock.UnknownBlock[(int) this.NrUnknownBlock];
              for (int index = 0; index < (int) this.NrUnknownBlock; ++index)
                this.UnknownBlocks[index] = new HBDF.MDLFBlock.OBJOBlock.MESHBlock.UnknownBlock(er);
              this.Data = er.ReadBytes((int) this.DLSize);
              OK = true;
            }
          }

          public class UnknownBlock
          {
            public ushort Unknown1;
            public ushort Unknown2;
            public ushort Unknown3;
            public ushort Unknown4;

            public UnknownBlock(EndianBinaryReader er)
            {
              this.Unknown1 = er.ReadUInt16();
              this.Unknown2 = er.ReadUInt16();
              this.Unknown3 = er.ReadUInt16();
              this.Unknown4 = er.ReadUInt16();
            }
          }
        }

        public class SKINBlock
        {
          public const string Signature = "SKIN";
          public DataBlockHeader Header;
          public float Sx;
          public float Sy;
          public float Sz;
          public uint Unknown1;
          public uint Unknown2;
          public uint Unknown3;
          public uint Unknown4;
          public short NrUnknownBlock;
          public ushort DLSize;
          public uint Unknown5;
          public uint Unknown6;
          public uint Unknown7;
          public ushort Unknown8;
          public ushort Unknown9;
          public HBDF.MDLFBlock.OBJOBlock.SKINBlock.UnknownBlock[] UnknownBlocks;
          public byte[] Data;
          public HBDF.MDLFBlock.OBJOBlock.SKINBlock.ENVSBlock ENVS;

          public SKINBlock(EndianBinaryReader er, out bool OK)
          {
            bool OK1;
            this.Header = new DataBlockHeader(er, "SKIN", out OK1);
            if (!OK1)
            {
              OK = false;
            }
            else
            {
              long position = er.BaseStream.Position;
              this.Sx = er.ReadSingleInt32Exp12();
              this.Sy = er.ReadSingleInt32Exp12();
              this.Sz = er.ReadSingleInt32Exp12();
              this.Unknown1 = er.ReadUInt32();
              this.Unknown2 = er.ReadUInt32();
              this.Unknown3 = er.ReadUInt32();
              this.Unknown4 = er.ReadUInt32();
              this.NrUnknownBlock = er.ReadInt16();
              this.DLSize = er.ReadUInt16();
              this.Unknown5 = er.ReadUInt32();
              this.Unknown6 = er.ReadUInt32();
              this.Unknown7 = er.ReadUInt32();
              this.Unknown8 = er.ReadUInt16();
              this.Unknown9 = er.ReadUInt16();
              this.UnknownBlocks = new HBDF.MDLFBlock.OBJOBlock.SKINBlock.UnknownBlock[(int) this.NrUnknownBlock];
              for (int index = 0; index < (int) this.NrUnknownBlock; ++index)
                this.UnknownBlocks[index] = new HBDF.MDLFBlock.OBJOBlock.SKINBlock.UnknownBlock(er);
              this.Data = er.ReadBytes((int) this.DLSize);
              if (this.Unknown7 != 0U)
              {
                this.ENVS = new HBDF.MDLFBlock.OBJOBlock.SKINBlock.ENVSBlock(er, out OK1);
                if (!OK1)
                {
                  OK = false;
                  return;
                }
              }
              else
                er.ReadBytes((int) this.Header.size - (int) (er.BaseStream.Position - position));
              OK = true;
            }
          }

          public class UnknownBlock
          {
            public ushort Unknown1;
            public ushort Unknown2;
            public ushort Unknown3;
            public ushort Unknown4;

            public UnknownBlock(EndianBinaryReader er)
            {
              this.Unknown1 = er.ReadUInt16();
              this.Unknown2 = er.ReadUInt16();
              this.Unknown3 = er.ReadUInt16();
              this.Unknown4 = er.ReadUInt16();
            }
          }

          public class ENVSBlock
          {
            public const string Signature = "ENVS";
            public DataBlockHeader Header;
            public uint NrOffsets;
            public uint[] Offsets;
            public HBDF.MDLFBlock.OBJOBlock.SKINBlock.ENVSBlock.ENVSEntry[] Entries;
            public ushort Unknown1;

            public ENVSBlock(EndianBinaryReader er, out bool OK)
            {
              bool OK1;
              this.Header = new DataBlockHeader(er, "ENVS", out OK1);
              if (!OK1)
              {
                OK = false;
              }
              else
              {
                this.NrOffsets = er.ReadUInt32();
                this.Offsets = er.ReadUInt32s((int) this.NrOffsets);
                this.Entries = new HBDF.MDLFBlock.OBJOBlock.SKINBlock.ENVSBlock.ENVSEntry[this.NrOffsets];
                for (int index = 0; (long) index < (long) this.NrOffsets; ++index)
                  this.Entries[index] = new HBDF.MDLFBlock.OBJOBlock.SKINBlock.ENVSBlock.ENVSEntry(er);
                while (er.BaseStream.Position % 8L != 0L)
                {
                  int num = (int) er.ReadByte();
                }
                OK = true;
              }
            }

            public class ENVSEntry
            {
              public uint MatrixBlockID;
              public ushort Unknown1;

              public ENVSEntry(EndianBinaryReader er)
              {
                this.MatrixBlockID = er.ReadUInt32();
                this.Unknown1 = er.ReadUInt16();
              }
            }
          }
        }
      }

      public class STRBBlock
      {
        public const string Signature = "STRB";
        public DataBlockHeader Header;
        public string[] Strings;

        public STRBBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "STRB", out OK1);
          long position = er.BaseStream.Position;
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            List<string> stringList = new List<string>();
            while (er.BaseStream.Position - position < (long) this.Header.size)
              stringList.Add(er.ReadStringNT(Encoding.ASCII));
            while (er.BaseStream.Position % 4L != 0L)
            {
              int num = (int) er.ReadByte();
            }
            this.Strings = stringList.ToArray();
            OK = true;
          }
        }
      }
    }

    public class TEXSBlock
    {
      public const string Signature = "TEXS";
      public DataBlockHeader Header;
      public ushort Nr1;
      public ushort Nr2;
      public ushort Nr3;
      public ushort Padding;
      public HBDF.TEXSBlock.TEXOBlock[] TEXOBlocks;
      public HBDF.TEXSBlock.IMGOBlock[] IMGOBlocks;
      public HBDF.TEXSBlock.PLTOBlock[] PLTOBlocks;

      public TEXSBlock(byte[] file)
      {
        EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
        bool OK;
        this.Header = new DataBlockHeader(er, "TEXS", out OK);
        if (!OK)
        {
          int num1 = (int) MessageBox.Show("Error 1");
        }
        else
        {
          this.Nr1 = er.ReadUInt16();
          this.Nr2 = er.ReadUInt16();
          this.Nr3 = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          int num2 = (int) this.Nr1 + (int) this.Nr2 + (int) this.Nr3;
          List<HBDF.TEXSBlock.TEXOBlock> texoBlockList = new List<HBDF.TEXSBlock.TEXOBlock>();
          List<HBDF.TEXSBlock.IMGOBlock> imgoBlockList = new List<HBDF.TEXSBlock.IMGOBlock>();
          List<HBDF.TEXSBlock.PLTOBlock> pltoBlockList = new List<HBDF.TEXSBlock.PLTOBlock>();
          for (int index = 0; index < num2; ++index)
          {
            switch (er.ReadString(Encoding.ASCII, 4))
            {
              case "TEXO":
                er.BaseStream.Position -= 4L;
                texoBlockList.Add(new HBDF.TEXSBlock.TEXOBlock(er, out OK));
                break;
              case "IMGO":
                er.BaseStream.Position -= 4L;
                imgoBlockList.Add(new HBDF.TEXSBlock.IMGOBlock(er, out OK));
                break;
              case "PLTO":
                er.BaseStream.Position -= 4L;
                pltoBlockList.Add(new HBDF.TEXSBlock.PLTOBlock(er, out OK));
                break;
            }
          }
          this.TEXOBlocks = texoBlockList.ToArray();
          this.IMGOBlocks = imgoBlockList.ToArray();
          this.PLTOBlocks = pltoBlockList.ToArray();
        }
        er.Close();
      }

      public TEXSBlock(EndianBinaryReader er, out bool OK)
      {
        bool OK1;
        this.Header = new DataBlockHeader(er, "TEXS", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Nr1 = er.ReadUInt16();
          this.Nr2 = er.ReadUInt16();
          this.Nr3 = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          int num = (int) this.Nr1 + (int) this.Nr2 + (int) this.Nr3;
          List<HBDF.TEXSBlock.TEXOBlock> texoBlockList = new List<HBDF.TEXSBlock.TEXOBlock>();
          List<HBDF.TEXSBlock.IMGOBlock> imgoBlockList = new List<HBDF.TEXSBlock.IMGOBlock>();
          List<HBDF.TEXSBlock.PLTOBlock> pltoBlockList = new List<HBDF.TEXSBlock.PLTOBlock>();
          for (int index = 0; index < num; ++index)
          {
            switch (er.ReadString(Encoding.ASCII, 4))
            {
              case "TEXO":
                er.BaseStream.Position -= 4L;
                texoBlockList.Add(new HBDF.TEXSBlock.TEXOBlock(er, out OK1));
                break;
              case "IMGO":
                er.BaseStream.Position -= 4L;
                imgoBlockList.Add(new HBDF.TEXSBlock.IMGOBlock(er, out OK1));
                break;
              case "PLTO":
                er.BaseStream.Position -= 4L;
                pltoBlockList.Add(new HBDF.TEXSBlock.PLTOBlock(er, out OK1));
                break;
            }
          }
          this.TEXOBlocks = texoBlockList.ToArray();
          this.IMGOBlocks = imgoBlockList.ToArray();
          this.PLTOBlocks = pltoBlockList.ToArray();
          OK = true;
        }
      }

      public HBDF.TEXSBlock.IMGOBlock GetIMGOByName(string Name)
      {
        foreach (HBDF.TEXSBlock.IMGOBlock imgoBlock in this.IMGOBlocks)
        {
          if (imgoBlock.ToString() == Name)
            return imgoBlock;
        }
        return (HBDF.TEXSBlock.IMGOBlock) null;
      }

      public HBDF.TEXSBlock.PLTOBlock GetPLTOByName(string Name)
      {
        foreach (HBDF.TEXSBlock.PLTOBlock pltoBlock in this.PLTOBlocks)
        {
          if (pltoBlock.ToString() == Name)
            return pltoBlock;
        }
        return (HBDF.TEXSBlock.PLTOBlock) null;
      }

      public class TEXOBlock
      {
        public const string Signature = "TEXO";
        public DataBlockHeader Header;
        public byte[] Padding;
        public HBDF.TEXSBlock.NAMEBlock TexName;
        public HBDF.TEXSBlock.NAMEBlock PalName;

        public TEXOBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "TEXO", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Padding = er.ReadBytes(8);
            this.TexName = new HBDF.TEXSBlock.NAMEBlock(er, out OK1);
            if (!OK1)
            {
              OK = false;
            }
            else
            {
              if (this.Header.size > 16U + this.TexName.Header.size)
              {
                this.PalName = new HBDF.TEXSBlock.NAMEBlock(er, out OK1);
                if (!OK1)
                {
                  OK = false;
                  return;
                }
              }
              OK = true;
            }
          }
        }
      }

      public class IMGOBlock
      {
        public const string Signature = "IMGO";
        public DataBlockHeader Header;
        public HBDF.TEXSBlock.NAMEBlock Name;
        public Graphic.GXTexFmt TexFmt;
        public short Width;
        public short Height;
        public uint texImageParam;
        public ushort S;
        public ushort T;
        public Graphic.GXTexFmt Fmt;
        public bool TransparentColor;
        public uint TexSize;
        public uint Tex4x4Size;
        public bool DataLZ77;
        public byte[] Data;
        public byte[] Data4x4;

        public IMGOBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "IMGO", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Name = new HBDF.TEXSBlock.NAMEBlock(er, out OK1);
            if (!OK1)
            {
              OK = false;
            }
            else
            {
              this.TexFmt = (Graphic.GXTexFmt) er.ReadUInt32();
              this.Width = er.ReadInt16();
              this.Height = er.ReadInt16();
              this.texImageParam = er.ReadUInt32();
              this.S = (ushort) (8U << (int) ((this.texImageParam & 7340032U) >> 20));
              this.T = (ushort) (8U << (int) ((this.texImageParam & 58720256U) >> 23));
              this.Fmt = (Graphic.GXTexFmt) ((this.texImageParam & 469762048U) >> 26);
              this.TransparentColor = ((int) (this.texImageParam >> 29) & 1) == 1;
              this.TexSize = er.ReadUInt32();
              this.Tex4x4Size = er.ReadUInt32();
              this.DataLZ77 = er.ReadUInt32() == 1U;
              this.Data = er.ReadBytes((int) this.TexSize);
              if (this.DataLZ77)
                this.Data = Compression.LZ77Decompress(this.Data);
              this.Data4x4 = er.ReadBytes((int) this.Tex4x4Size);
              OK = true;
            }
          }
        }

        public System.Drawing.Bitmap ToBitmap(HBDF.TEXSBlock.PLTOBlock Palette)
        {
          return Graphic.ConvertData(this.Data, Palette == null ? (byte[]) null : Palette.Data, this.Data4x4, 0, (int) this.S, (int) this.T, this.Fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, this.TransparentColor, true);
        }

        public override string ToString()
        {
          return this.Name.Name;
        }
      }

      public class PLTOBlock
      {
        public const string Signature = "PLTO";
        public DataBlockHeader Header;
        public HBDF.TEXSBlock.NAMEBlock Name;
        public uint PalSize;
        public bool DataLZ77;
        public byte[] Data;

        public PLTOBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "PLTO", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Name = new HBDF.TEXSBlock.NAMEBlock(er, out OK1);
            if (!OK1)
            {
              OK = false;
            }
            else
            {
              this.PalSize = er.ReadUInt32();
              this.DataLZ77 = er.ReadUInt32() == 1U;
              this.Data = er.ReadBytes((int) this.PalSize);
              if (this.DataLZ77)
                this.Data = Compression.LZ77Decompress(this.Data);
              OK = true;
            }
          }
        }

        public override string ToString()
        {
          return this.Name.Name;
        }
      }

      public class NAMEBlock
      {
        public const string Signature = "NAME";
        public DataBlockHeader Header;
        public string Name;

        public NAMEBlock(EndianBinaryReader er, out bool OK)
        {
          bool OK1;
          this.Header = new DataBlockHeader(er, "NAME", out OK1);
          if (!OK1)
          {
            OK = false;
          }
          else
          {
            this.Name = er.ReadString(Encoding.ASCII, (int) this.Header.size).Replace("\0", "");
            OK = true;
          }
        }

        public override string ToString()
        {
          return this.Name;
        }
      }
    }

    public class ANMFBlock
    {
      public const string Signature = "ANMF";
      public DataBlockHeader Header;
      public ushort Unknown1;
      public ushort Unknown2;
      public uint Unknown3;
      public uint NrAnimatedObjects;
      public uint Unknown4;
      public HBDF.ANMFBlock.AnimatedObjectEntry[] AnimatedObjectEntries;

      public ANMFBlock(EndianBinaryReader er, out bool OK)
      {
        long position = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "ANMF", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadUInt32();
          this.NrAnimatedObjects = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          this.AnimatedObjectEntries = new HBDF.ANMFBlock.AnimatedObjectEntry[this.NrAnimatedObjects];
          for (int index = 0; (long) index < (long) this.NrAnimatedObjects; ++index)
            this.AnimatedObjectEntries[index] = new HBDF.ANMFBlock.AnimatedObjectEntry(er, position);
          OK = true;
        }
      }

      public class AnimatedObjectEntry
      {
        public byte Unknown1;
        public ushort UnknownOffset;
        public byte Padding;
        public uint ObjectNameOffset;
        public string Name;

        public AnimatedObjectEntry(EndianBinaryReader er, long BaseOffset)
        {
          this.Unknown1 = er.ReadByte();
          this.UnknownOffset = er.ReadUInt16();
          this.Padding = er.ReadByte();
          this.ObjectNameOffset = er.ReadUInt32();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = BaseOffset + 24L + (long) this.ObjectNameOffset;
          this.Name = er.ReadStringNT(Encoding.ASCII);
          er.BaseStream.Position = position;
        }

        public override string ToString()
        {
          return this.Name;
        }
      }
    }
  }
}
