// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Misc;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Tao.OpenGl;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.G3D_Binary_File_Format
{
  public class NSBMD
  {
    public const string Signature = "BMD0";
    public FileHeader Header;
    public NSBMD.ModelSet modelSet;
    public NSBTX.TexplttSet TexPlttSet;

    public NSBMD(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.LittleEndian);
      bool OK;
      this.Header = new FileHeader(er, "BMD0", out OK);
      if (!OK)
      {
        int num1 = (int) System.Windows.Forms.MessageBox.Show("Error 0");
      }
      else
      {
        er.BaseStream.Position = (long) this.Header[0];
        this.modelSet = new NSBMD.ModelSet(er, out OK);
        if (!OK)
        {
          int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 1");
        }
        else if (this.Header.info.dataBlocks > (ushort) 1)
        {
          er.BaseStream.Position = (long) this.Header[1];
          er.SetMarkerOnCurrentOffset("TexplttSet");
          this.TexPlttSet = new NSBTX.TexplttSet(er, out OK);
          if (!OK)
          {
            int num3 = (int) System.Windows.Forms.MessageBox.Show("Error 2");
          }
        }
      }
      er.ClearMarkers();
      er.Close();
    }

    public NSBMD(bool Textures)
    {
      this.Header = new FileHeader("BMD0", Textures ? (ushort) 2 : (ushort) 1);
      this.Header.info.version = (ushort) 2;
    }

    public byte[] Write()
    {
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      this.Header.info.dataBlocks = this.TexPlttSet != null ? (ushort) 2 : (ushort) 1;
      this.Header.Write(er);
      long position1 = er.BaseStream.Position;
      er.BaseStream.Position = 16L;
      er.Write((uint) position1);
      er.BaseStream.Position = position1;
      this.modelSet.Write(er);
      if (this.TexPlttSet != null)
      {
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = 20L;
        er.Write((uint) position2);
        er.BaseStream.Position = position2;
        this.TexPlttSet.Write(er);
      }
      er.BaseStream.Position = 8L;
      er.Write((uint) er.BaseStream.Length);
      byte[] array = memoryStream.ToArray();
      er.Close();
      return array;
    }

    public class ModelSet
    {
      public const string Signature = "MDL0";
      public DataBlockHeader header;
      public Dictionary<NSBMD.ModelSet.MDL0Data> dict;
      public NSBMD.ModelSet.Model[] models;

      public ModelSet(EndianBinaryReader er, out bool OK)
      {
        er.SetMarkerOnCurrentOffset(nameof (ModelSet));
        bool OK1;
        this.header = new DataBlockHeader(er, "MDL0", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.dict = new Dictionary<NSBMD.ModelSet.MDL0Data>(er);
          this.models = new NSBMD.ModelSet.Model[(int) this.dict.numEntry];
          long position = er.BaseStream.Position;
          for (int index = 0; index < (int) this.dict.numEntry; ++index)
          {
            er.BaseStream.Position = (long) this.dict[index].Value.Offset + er.GetMarker(nameof (ModelSet));
            this.models[index] = new NSBMD.ModelSet.Model(er);
          }
          OK = true;
        }
      }

      public ModelSet()
      {
        this.header = new DataBlockHeader("MDL0", 0U);
        this.dict = new Dictionary<NSBMD.ModelSet.MDL0Data>();
      }

      public void Write(EndianBinaryWriter er)
      {
        long position1 = er.BaseStream.Position;
        this.header.Write(er, 0);
        this.dict.Write(er);
        for (int index = 0; index < this.models.Length; ++index)
        {
          this.dict[index].Value.Offset = (uint) (er.BaseStream.Position - position1);
          this.models[index].Write(er);
        }
        long position2 = er.BaseStream.Position;
        er.BaseStream.Position = position1 + 4L;
        er.Write((uint) (position2 - position1));
        this.dict.Write(er);
        er.BaseStream.Position = position2;
      }

      public class MDL0Data : DictionaryData
      {
        public uint Offset;

        public override ushort GetDataSize()
        {
          return 4;
        }

        public override void Read(EndianBinaryReader er)
        {
          this.Offset = er.ReadUInt32();
        }

        public override void Write(EndianBinaryWriter er)
        {
          er.Write(this.Offset);
        }
      }

      public class Model
      {
        public uint size;
        public uint ofsSbc;
        public uint ofsMat;
        public uint ofsShp;
        public uint ofsEvpMtx;
        public NSBMD.ModelSet.Model.ModelInfo info;
        public NSBMD.ModelSet.Model.NodeSet nodes;
        public byte[] sbc;
        public NSBMD.ModelSet.Model.MaterialSet materials;
        public NSBMD.ModelSet.Model.ShapeSet shapes;
        public NSBMD.ModelSet.Model.EvpMatrices evpMatrices;

        public Model(EndianBinaryReader er)
        {
          er.SetMarkerOnCurrentOffset(nameof (Model));
          this.size = er.ReadUInt32();
          this.ofsSbc = er.ReadUInt32();
          this.ofsMat = er.ReadUInt32();
          this.ofsShp = er.ReadUInt32();
          this.ofsEvpMtx = er.ReadUInt32();
          this.info = new NSBMD.ModelSet.Model.ModelInfo(er);
          this.nodes = new NSBMD.ModelSet.Model.NodeSet(er);
          long position1 = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.ofsSbc + er.GetMarker(nameof (Model));
          this.sbc = er.ReadBytes((int) this.ofsMat - (int) this.ofsSbc);
          er.BaseStream.Position = position1;
          er.BaseStream.Position = (long) this.ofsMat + er.GetMarker(nameof (Model));
          this.materials = new NSBMD.ModelSet.Model.MaterialSet(er);
          er.BaseStream.Position = (long) this.ofsShp + er.GetMarker(nameof (Model));
          this.shapes = new NSBMD.ModelSet.Model.ShapeSet(er);
          if ((int) this.ofsEvpMtx != (int) this.size && this.ofsEvpMtx != 0U)
          {
            er.BaseStream.Position = (long) this.ofsEvpMtx + er.GetMarker(nameof (Model));
            this.evpMatrices = new NSBMD.ModelSet.Model.EvpMatrices(er, (int) this.nodes.dict.numEntry);
          }
          long marker = er.GetMarker(nameof (ModelSet));
          er.ClearMarkers();
          long position2 = er.BaseStream.Position;
          er.BaseStream.Position = marker;
          er.SetMarkerOnCurrentOffset(nameof (ModelSet));
          er.BaseStream.Position = position2;
        }

        public Model()
        {
        }

        public void Write(EndianBinaryWriter er)
        {
          long position1 = er.BaseStream.Position;
          er.Write(0U);
          er.Write(0U);
          er.Write(0U);
          er.Write(0U);
          er.Write(0U);
          this.info.Write(er);
          this.nodes.Write(er);
          long position2 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + 4L;
          er.Write((uint) (position2 - position1));
          er.BaseStream.Position = position2;
          er.Write(this.sbc, 0, this.sbc.Length);
          long position3 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + 8L;
          er.Write((uint) (position3 - position1));
          er.BaseStream.Position = position3;
          this.materials.Write(er);
          long position4 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + 12L;
          er.Write((uint) (position4 - position1));
          er.BaseStream.Position = position4;
          this.shapes.Write(er);
          if (this.evpMatrices != null)
          {
            long position5 = er.BaseStream.Position;
            er.BaseStream.Position = position1 + 16L;
            er.Write((uint) (position5 - position1));
            er.BaseStream.Position = position5;
            this.evpMatrices.Write(er);
          }
          else
          {
            long position5 = er.BaseStream.Position;
            er.BaseStream.Position = position1 + 16L;
            er.Write((uint) (position5 - position1));
            er.BaseStream.Position = position5;
          }
          long position6 = er.BaseStream.Position;
          er.BaseStream.Position = position1;
          er.Write((uint) (position6 - position1));
          er.BaseStream.Position = position6;
        }

        public void ProcessSbc(
          float X,
          float Y,
          float dist,
          float elev,
          float ang,
          bool picking = false,
          int texoffset = 1)
        {
          this.ProcessSbc((NSBTX.TexplttSet) null, (NSBCA) null, 0, 0, (NSBTA) null, 0, 0, (NSBTP) null, 0, 0, (NSBMA) null, 0, 0, (NSBVA) null, 0, 0, X, Y, dist, elev, ang, picking, texoffset);
        }

        public void ProcessSbc(
          NSBTX.TexplttSet Textures,
          NSBCA Bca,
          int BcaAnmNumber,
          int BcaFrameNumber,
          NSBTA Bta,
          int BtaAnmNumber,
          int BtaFrameNumber,
          NSBTP Btp,
          int BtpAnmNumber,
          int BtpFrameNumber,
          NSBMA Bma,
          int BmaAnmNumber,
          int BmaFrameNumber,
          NSBVA Bva,
          int BvaAnmNumber,
          int BvaFrameNumber,
          float X,
          float Y,
          float dist,
          float elev,
          float ang,
          bool picking = false,
          int texoffset = 1)
        {
          int num1 = 0;
          while (true)
          {
            do
            {
              bool flag1 = Bca != null && BcaAnmNumber >= 0;
              bool flag2 = Bta != null && BtaAnmNumber >= 0;
              bool flag3 = Btp != null && BtpAnmNumber >= 0;
              bool flag4 = Bma != null && BmaAnmNumber >= 0;
              bool flag5 = Bva != null && BvaAnmNumber >= 0;
              Gl.glMatrixMode(5888);
              GlNitro.Nitro3DContext Context = new GlNitro.Nitro3DContext();
              int offset1 = 0;
              int posScale = (int) this.info.posScale;
              MTX44 b = new MTX44();
              bool flag6 = true;
              bool flag7 = true;
              while (offset1 != this.sbc.Length)
              {
                byte[] sbc1 = this.sbc;
                int index1 = offset1++;
                byte num2;
                byte num3;
                byte num4;
                switch ((int) (num2 = sbc1[index1]) & 15)
                {
                  case 1:
                    goto label_104;
                  case 2:
                    byte[] sbc2 = this.sbc;
                    int index2 = offset1;
                    int num5 = index2 + 1;
                    byte num6 = sbc2[index2];
                    if (!flag5)
                    {
                      byte[] sbc3 = this.sbc;
                      int index3 = num5;
                      offset1 = index3 + 1;
                      flag6 = sbc3[index3] == (byte) 1;
                      break;
                    }
                    offset1 = num5 + 1;
                    flag6 = Bva.visAnmSet.visAnm[BvaAnmNumber].GetFrame(BvaFrameNumber, (int) num6);
                    break;
                  case 3:
                    if (flag6)
                    {
                      b = Context.MatrixStack[(int) this.sbc[offset1++]].Clone();
                      break;
                    }
                    ++offset1;
                    break;
                  case 4:
                    if (flag6 && !picking)
                    {
                      byte num7 = this.sbc[offset1++];
                      flag7 = true;
                      if (((int) this.materials.materials[(int) num7].texImageParam & (int) ushort.MaxValue) != 0)
                      {
                        int num8 = (int) MessageBox.Show("Texoffset is not 0!!!");
                      }
                      if (this.materials.materials[(int) num7].Fmt != Graphic.GXTexFmt.GX_TEXFMT_NONE)
                      {
                        if ((this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_A3I5 || this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_A5I3 || ((int) (this.materials.materials[(int) num7].polyAttr >> 16) & 31) != 31) && num1 != 1)
                          flag7 = false;
                        if ((this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_NONE || this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT4 || (this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16 || this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT256) || (this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_COMP4x4 || this.materials.materials[(int) num7].Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)) && ((int) (this.materials.materials[(int) num7].polyAttr >> 16) & 31) == 31 && num1 != 0)
                          flag7 = false;
                      }
                      else
                        flag7 = true;
                      if (flag7)
                      {
                        KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.MaterialSetData> keyValuePair;
                        int num9;
                        if (flag3)
                        {
                          Dictionary<NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData> dict = Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          if (dict.Contains(key))
                          {
                            num9 = Textures == null ? 1 : 0;
                            goto label_23;
                          }
                        }
                        num9 = 1;
label_23:
                        if (num9 == 0)
                        {
                          Dictionary<NSBTP.TexPatAnmSet.TexPatAnm.DictTexPatAnmData> dict = Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          int TexIdx;
                          int PlttIdx;
                          dict[key].GetData(out TexIdx, out PlttIdx, BtpFrameNumber);
                          string index3 = (string) Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].texName[TexIdx];
                          string index4 = (string) Btp.texPatAnmSet.texPatAnm[BtpAnmNumber].plttName[PlttIdx];
                          GlNitro.glNitroTexImage2D(Textures.dictTex[index3].ToBitmap(Textures.dictPltt[index4]), this.materials.materials[(int) num7], (int) num7 + texoffset);
                        }
                        Gl.glBindTexture(3553, (int) num7 + texoffset);
                        Gl.glMatrixMode(5890);
                        Gl.glLoadIdentity();
                        Gl.glScalef(1f / (float) this.materials.materials[(int) num7].origWidth, 1f / (float) this.materials.materials[(int) num7].origHeight, 1f);
                        int num10;
                        if (flag2)
                        {
                          NSBTA.TexSRTAnmSet.TexSRTAnm texSrtAnm = Bta.texSRTAnmSet.texSRTAnm[BtaAnmNumber];
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          num10 = texSrtAnm.Contains(key) != -1 ? 1 : 0;
                        }
                        else
                          num10 = 0;
                        if (num10 == 0)
                        {
                          Gl.glMultMatrixf(this.materials.materials[(int) num7].GetMatrix());
                        }
                        else
                        {
                          Dictionary<NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData> dict = Bta.texSRTAnmSet.texSRTAnm[BtaAnmNumber].dict;
                          NSBTA.TexSRTAnmSet.TexSRTAnm texSrtAnm = Bta.texSRTAnmSet.texSRTAnm[BtaAnmNumber];
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          int index3 = texSrtAnm.Contains(key);
                          Gl.glMultMatrixf(dict[index3].Value.GetMatrix(BtaFrameNumber, (int) this.materials.materials[(int) num7].origWidth, (int) this.materials.materials[(int) num7].origHeight));
                        }
                        Context.LightEnabled[0] = ((int) this.materials.materials[(int) num7].polyAttr & 1) == 1;
                        Context.LightEnabled[1] = ((int) (this.materials.materials[(int) num7].polyAttr >> 1) & 1) == 1;
                        Context.LightEnabled[2] = ((int) (this.materials.materials[(int) num7].polyAttr >> 2) & 1) == 1;
                        Context.LightEnabled[3] = ((int) (this.materials.materials[(int) num7].polyAttr >> 3) & 1) == 1;
                        int num11;
                        if (flag4)
                        {
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          num11 = dict.Contains(key) ? 1 : 0;
                        }
                        else
                          num11 = 0;
                        Color color1;
                        if (num11 == 0)
                        {
                          color1 = (this.materials.materials[(int) num7].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_DIFFUSE) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_DIFFUSE ? Color.Black : Graphic.ConvertABGR1555((short) ((int) this.materials.materials[(int) num7].diffAmb & (int) short.MaxValue));
                          Color color2 = (this.materials.materials[(int) num7].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_AMBIENT) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_AMBIENT ? Color.FromArgb(160, 160, 160) : Graphic.ConvertABGR1555((short) ((int) (this.materials.materials[(int) num7].diffAmb >> 16) & (int) short.MaxValue));
                          Context.DiffuseColor = color1;
                          Context.AmbientColor = color2;
                          Color color3 = (this.materials.materials[(int) num7].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SPECULAR) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SPECULAR ? Color.Black : Graphic.ConvertABGR1555((short) ((int) this.materials.materials[(int) num7].specEmi & (int) short.MaxValue));
                          Color color4 = (this.materials.materials[(int) num7].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EMISSION) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EMISSION ? Color.Black : Graphic.ConvertABGR1555((short) ((int) (this.materials.materials[(int) num7].specEmi >> 16) & (int) short.MaxValue));
                          Context.SpecularColor = color3;
                          Context.EmissionColor = color4;
                        }
                        else
                        {
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict1 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key1 = keyValuePair.Key;
                          NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData1 = dict1[key1];
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict2 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key2 = keyValuePair.Key;
                          int tagDiffuse = (int) dict2[key2].tagDiffuse;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict3 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key3 = keyValuePair.Key;
                          int constDiffuse = (int) dict3[key3].ConstDiffuse;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict4 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key4 = keyValuePair.Key;
                          ushort[] diffuse = dict4[key4].Diffuse;
                          int Frame1 = BmaFrameNumber;
                          color1 = Graphic.ConvertABGR1555((short) dictMatColAnmData1.GetValue((uint) tagDiffuse, (ushort) constDiffuse, diffuse, Frame1));
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict5 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key5 = keyValuePair.Key;
                          NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData2 = dict5[key5];
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict6 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key6 = keyValuePair.Key;
                          int tagAmbient = (int) dict6[key6].tagAmbient;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict7 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key7 = keyValuePair.Key;
                          int constAmbient = (int) dict7[key7].ConstAmbient;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict8 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key8 = keyValuePair.Key;
                          ushort[] ambient = dict8[key8].Ambient;
                          int Frame2 = BmaFrameNumber;
                          Color color2 = Graphic.ConvertABGR1555((short) dictMatColAnmData2.GetValue((uint) tagAmbient, (ushort) constAmbient, ambient, Frame2));
                          Context.DiffuseColor = color1;
                          Context.AmbientColor = color2;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict9 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key9 = keyValuePair.Key;
                          NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData3 = dict9[key9];
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict10 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key10 = keyValuePair.Key;
                          int tagSpecular = (int) dict10[key10].tagSpecular;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict11 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key11 = keyValuePair.Key;
                          int constSpecular = (int) dict11[key11].ConstSpecular;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict12 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key12 = keyValuePair.Key;
                          ushort[] specular = dict12[key12].Specular;
                          int Frame3 = BmaFrameNumber;
                          Color color3 = Graphic.ConvertABGR1555((short) dictMatColAnmData3.GetValue((uint) tagSpecular, (ushort) constSpecular, specular, Frame3));
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict13 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key13 = keyValuePair.Key;
                          NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData4 = dict13[key13];
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict14 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key14 = keyValuePair.Key;
                          int tagEmission = (int) dict14[key14].tagEmission;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict15 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key15 = keyValuePair.Key;
                          int constEmission = (int) dict15[key15].ConstEmission;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict16 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key16 = keyValuePair.Key;
                          ushort[] emission = dict16[key16].Emission;
                          int Frame4 = BmaFrameNumber;
                          Color color4 = Graphic.ConvertABGR1555((short) dictMatColAnmData4.GetValue((uint) tagEmission, (ushort) constEmission, emission, Frame4));
                          Context.SpecularColor = color3;
                          Context.EmissionColor = color4;
                        }
                        switch (this.materials.materials[(int) num7].polyAttr >> 14 & 1U)
                        {
                          case 0:
                            Gl.glDepthFunc(513);
                            break;
                          case 1:
                            int num12 = (int) MessageBox.Show("EQUALS!");
                            Gl.glDepthFunc(514);
                            break;
                        }
                        int num13 = -1;
                        switch (this.materials.materials[(int) num7].polyAttr >> 4 & 3U)
                        {
                          case 0:
                            num13 = 8448;
                            break;
                          case 1:
                            num13 = 8449;
                            break;
                          case 2:
                            num13 = 8448;
                            break;
                          case 3:
                            int num14 = (int) MessageBox.Show("SHADOW!");
                            num13 = 8448;
                            break;
                        }
                        Gl.glTexEnvi(8960, 8704, num13);
                        int num15;
                        if (flag4)
                        {
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          num15 = dict.Contains(key) ? 1 : 0;
                        }
                        else
                          num15 = 0;
                        if (num15 == 0)
                        {
                          Context.Alpha = (int) (this.materials.materials[(int) num7].polyAttr >> 16) & 31;
                        }
                        else
                        {
                          GlNitro.Nitro3DContext nitro3Dcontext = Context;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict1 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key1 = keyValuePair.Key;
                          NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData = dict1[key1];
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict2 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key2 = keyValuePair.Key;
                          int tagPolygonAlpha = (int) dict2[key2].tagPolygonAlpha;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict3 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key3 = keyValuePair.Key;
                          int constPolygonAlpha = (int) dict3[key3].ConstPolygonAlpha;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict4 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key4 = keyValuePair.Key;
                          byte[] polygonAlpha = dict4[key4].PolygonAlpha;
                          int Frame = BmaFrameNumber;
                          int num16 = (int) dictMatColAnmData.GetValue((uint) tagPolygonAlpha, (byte) constPolygonAlpha, polygonAlpha, Frame);
                          nitro3Dcontext.Alpha = num16;
                        }
                        int num17;
                        if (flag4)
                        {
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key = keyValuePair.Key;
                          num17 = dict.Contains(key) ? 1 : 0;
                        }
                        else
                          num17 = 0;
                        if (num17 == 0)
                        {
                          if (((int) (this.materials.materials[(int) num7].diffAmb >> 15) & 1) == 1 && (this.materials.materials[(int) num7].flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_VTXCOLOR) == NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_VTXCOLOR)
                          {
                            color1 = Graphic.ConvertABGR1555((short) ((int) this.materials.materials[(int) num7].diffAmb & (int) short.MaxValue));
                            Gl.glColor4f((float) color1.R / (float) byte.MaxValue, (float) color1.G / (float) byte.MaxValue, (float) color1.B / (float) byte.MaxValue, (float) Context.Alpha / 31f);
                          }
                          else
                            Gl.glColor4f(0.0f, 0.0f, 0.0f, (float) Context.Alpha / 31f);
                        }
                        else if (((int) (this.materials.materials[(int) num7].diffAmb >> 15) & 1) == 1)
                        {
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict1 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key1 = keyValuePair.Key;
                          NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData dictMatColAnmData = dict1[key1];
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict2 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key2 = keyValuePair.Key;
                          int tagDiffuse = (int) dict2[key2].tagDiffuse;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict3 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key3 = keyValuePair.Key;
                          int constDiffuse = (int) dict3[key3].ConstDiffuse;
                          Dictionary<NSBMA.MatColAnmSet.MatColAnm.DictMatColAnmData> dict4 = Bma.matColAnmSet.matColAnm[BmaAnmNumber].dict;
                          keyValuePair = this.materials.dict[(int) num7];
                          string key4 = keyValuePair.Key;
                          ushort[] diffuse = dict4[key4].Diffuse;
                          int Frame = BmaFrameNumber;
                          color1 = Graphic.ConvertABGR1555((short) dictMatColAnmData.GetValue((uint) tagDiffuse, (ushort) constDiffuse, diffuse, Frame));
                          Gl.glColor4f((float) color1.R / (float) byte.MaxValue, (float) color1.G / (float) byte.MaxValue, (float) color1.B / (float) byte.MaxValue, (float) Context.Alpha / 31f);
                        }
                        else
                          Gl.glColor4f(0.0f, 0.0f, 0.0f, (float) Context.Alpha / 31f);
                        Context.UseSpecularReflectionTable = ((int) (this.materials.materials[(int) num7].specEmi >> 15) & 1) == 1;
                        int mode = -1;
                        switch (this.materials.materials[(int) num7].polyAttr >> 6 & 3U)
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
                        break;
                      }
                      break;
                    }
                    ++offset1;
                    break;
                  case 5:
                    if (flag6 && flag7)
                    {
                      GlNitro.glNitroGx(this.shapes.shape[(int) this.sbc[offset1++]].DL, b.Clone(), ref Context, posScale, picking);
                      break;
                    }
                    ++offset1;
                    break;
                  case 6:
                    byte[] sbc4 = this.sbc;
                    int index5 = offset1;
                    int num18 = index5 + 1;
                    byte num19 = sbc4[index5];
                    byte[] sbc5 = this.sbc;
                    int index6 = num18;
                    int num20 = index6 + 1;
                    byte num21 = sbc5[index6];
                    byte[] sbc6 = this.sbc;
                    int index7 = num20;
                    offset1 = index7 + 1;
                    byte num22 = sbc6[index7];
                    bool flag8 = ((int) num22 & 1) == 1;
                    bool flag9 = ((int) num22 >> 1 & 1) == 1;
                    bool MayaScale = flag8;
                    int index8 = ((int) num2 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                    int index9 = ((int) num2 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                    if (index9 != -1)
                      b = Context.MatrixStack[index9];
                    if (!flag1)
                    {
                      b = b.MultMatrix((MTX44) this.nodes.data[(int) num19].GetMatrix(MayaScale, posScale));
                    }
                    else
                    {
                      try
                      {
                        b = b.MultMatrix((MTX44) Bca.jntAnmSet.jntAnm[BcaAnmNumber].tagData[(int) num19].GetMatrix(BcaFrameNumber, MayaScale, posScale, this.nodes.data[(int) num19]));
                      }
                      catch
                      {
                        b = b.MultMatrix((MTX44) this.nodes.data[(int) num19].GetMatrix(MayaScale, posScale));
                      }
                    }
                    if (index8 != -1)
                    {
                      Context.MatrixStack[index8] = b;
                      break;
                    }
                    break;
                  case 7:
                    Gl.glCullFace(0);
                    num3 = this.sbc[offset1++];
                    int index10 = ((int) num2 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                    int index11 = ((int) num2 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                    if (index11 != -1)
                      b = Context.MatrixStack[index11];
                    float[] params1 = new float[16];
                    Gl.glGetFloatv(2982, params1);
                    b[0, 0] = params1[0];
                    b[0, 1] = params1[1];
                    b[0, 2] = params1[2];
                    b[1, 0] = params1[4];
                    b[1, 1] = params1[5];
                    b[1, 2] = params1[6];
                    b[2, 0] = params1[8];
                    b[2, 1] = params1[9];
                    b[2, 2] = params1[10];
                    if (index10 != -1)
                    {
                      Context.MatrixStack[index10] = b;
                      break;
                    }
                    break;
                  case 8:
                    Gl.glCullFace(0);
                    num3 = this.sbc[offset1++];
                    int index12 = ((int) num2 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                    int index13 = ((int) num2 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                    if (index13 != -1)
                      b = Context.MatrixStack[index13];
                    float[] params2 = new float[16];
                    Gl.glGetFloatv(2982, params2);
                    b[0, 0] = params2[0];
                    b[0, 2] = params2[2];
                    b[1, 0] = params2[4];
                    b[1, 1] = params2[5];
                    b[1, 2] = params2[6];
                    b[2, 0] = params2[8];
                    b[2, 2] = params2[10];
                    if (index12 != -1)
                    {
                      Context.MatrixStack[index12] = b;
                      break;
                    }
                    break;
                  case 9:
                    MTX44 mtX44_1 = new MTX44();
                    mtX44_1.Zero();
                    MTX44 mtX44_2 = new MTX44();
                    mtX44_2.Zero();
                    MTX44 mtX44_3 = new MTX44();
                    MTX44 mtX44_4 = new MTX44();
                    float num23 = 0.0f;
                    byte[] sbc7 = this.sbc;
                    int index14 = offset1;
                    int num24 = index14 + 1;
                    byte num25 = sbc7[index14];
                    byte[] sbc8 = this.sbc;
                    int index15 = num24;
                    offset1 = index15 + 1;
                    byte num26 = sbc8[index15];
                    for (int index3 = 0; index3 < (int) num26; ++index3)
                    {
                      byte[] sbc3 = this.sbc;
                      int index4 = offset1;
                      int num7 = index4 + 1;
                      byte num8 = sbc3[index4];
                      byte[] sbc9 = this.sbc;
                      int index16 = num7;
                      int num9 = index16 + 1;
                      byte num10 = sbc9[index16];
                      byte[] sbc10 = this.sbc;
                      int index17 = num9;
                      offset1 = index17 + 1;
                      float num11 = (float) sbc10[index17] / 256f;
                      MTX44 mtX44_5 = Context.MatrixStack[(int) num8].MultMatrix(this.evpMatrices.m[(int) num10].invM);
                      if (index3 != 0)
                        mtX44_2 += num23 * mtX44_4;
                      MTX44 mtX44_6 = mtX44_5;
                      MTX44 mtX44_7 = mtX44_5.MultMatrix(this.evpMatrices.m[(int) num10].invN);
                      num23 = num11;
                      mtX44_1 += num23 * mtX44_6;
                      mtX44_4 = mtX44_7;
                    }
                    MTX44 mtX44_8 = mtX44_2 + num23 * mtX44_4;
                    b = mtX44_1;
                    Context.MatrixStack[(int) num25] = b;
                    break;
                  case 10:
                    int num27 = Bytes.Read4BytesAsInt32(this.sbc, offset1);
                    int offset2 = offset1 + 4;
                    int count = Bytes.Read4BytesAsInt32(this.sbc, offset2);
                    offset1 = offset2 + 4;
                    GlNitro.glNitroGx(((IEnumerable<byte>) this.sbc).ToList<byte>().GetRange(offset1 - 9 + num27, count).ToArray(), b.Clone(), ref Context, posScale, false);
                    break;
                  case 11:
                    if ((int) num2 >> 5 != 0)
                      break;
                    break;
                  case 12:
                    byte[] sbc11 = this.sbc;
                    int index18 = offset1;
                    int num28 = index18 + 1;
                    byte num29 = sbc11[index18];
                    byte[] sbc12 = this.sbc;
                    int index19 = num28;
                    offset1 = index19 + 1;
                    num4 = sbc12[index19];
                    if (flag6 && !picking)
                    {
                      float[] numArray = new float[16];
                      Gl.glGetFloatv(2982, numArray);
                      Gl.glMatrixMode(5890);
                      Gl.glLoadIdentity();
                      NSBMD.ModelSet.Model.MaterialSet.Material material = this.materials.materials[(int) num29];
                      Gl.glScalef(0.5f, -0.5f, 1f);
                      Gl.glTranslatef(0.5f, 0.5f, 0.0f);
                      if ((material.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX) != (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
                        Gl.glMultMatrixf(material.effectMtx);
                      MTX44 mtX44_5 = new MTX44();
                      mtX44_5.SetValues(numArray);
                      mtX44_5.MultMatrix(b);
                      mtX44_5[12] = 0.0f;
                      mtX44_5[13] = 0.0f;
                      mtX44_5[14] = 0.0f;
                      Gl.glMultMatrixf((float[]) mtX44_5);
                      Gl.glMatrixMode(5888);
                      Gl.glTexGeni(8192, 9472, 9218);
                      Gl.glTexGeni(8193, 9472, 9218);
                      Gl.glEnable(3168);
                      Gl.glEnable(3169);
                      break;
                    }
                    break;
                  case 13:
                    byte[] sbc13 = this.sbc;
                    int index20 = offset1;
                    int num30 = index20 + 1;
                    byte num31 = sbc13[index20];
                    byte[] sbc14 = this.sbc;
                    int index21 = num30;
                    offset1 = index21 + 1;
                    num4 = sbc14[index21];
                    Gl.glTexGeni(8192, 9472, 9217);
                    Gl.glTexGeni(8193, 9472, 9217);
                    Gl.glEnable(3168);
                    Gl.glEnable(3169);
                    break;
                }
              }
label_104:
              if (num1 == 0)
                num1 = 1;
              else
                goto label_107;
            }
            while (!picking);
            Gl.glDepthMask(1);
          }
label_107:;
        }

        public byte[] ExportBones()
        {
          MTX44[] mtX44Array = new MTX44[31];
          for (int index = 0; index < 31; ++index)
            mtX44Array[index] = new MTX44();
          int offset1 = 0;
          int posScale = (int) this.info.posScale;
          MTX44 mtX44_1 = new MTX44();
          List<MA.Node> nodeList = new List<MA.Node>();
          bool flag1 = true;
          while (offset1 != this.sbc.Length)
          {
            byte[] sbc1 = this.sbc;
            int index1 = offset1++;
            byte num1;
            byte num2;
            int num3;
            int num4;
            byte num5;
            byte num6;
            switch ((int) (num1 = sbc1[index1]) & 15)
            {
              case 1:
                goto label_28;
              case 2:
                byte[] sbc2 = this.sbc;
                int index2 = offset1;
                int num7 = index2 + 1;
                num2 = sbc2[index2];
                byte[] sbc3 = this.sbc;
                int index3 = num7;
                offset1 = index3 + 1;
                flag1 = sbc3[index3] == (byte) 1;
                break;
              case 3:
                if (flag1)
                {
                  mtX44_1 = mtX44Array[(int) this.sbc[offset1++]].Clone();
                  break;
                }
                ++offset1;
                break;
              case 4:
                ++offset1;
                break;
              case 5:
                ++offset1;
                break;
              case 6:
                byte[] sbc4 = this.sbc;
                int index4 = offset1;
                int num8 = index4 + 1;
                byte num9 = sbc4[index4];
                byte[] sbc5 = this.sbc;
                int index5 = num8;
                int num10 = index5 + 1;
                byte num11 = sbc5[index5];
                byte[] sbc6 = this.sbc;
                int index6 = num10;
                offset1 = index6 + 1;
                byte num12 = sbc6[index6];
                bool flag2 = ((int) num12 & 1) == 1;
                bool flag3 = ((int) num12 >> 1 & 1) == 1;
                bool MayaScale = flag2;
                int index7 = ((int) num1 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                int index8 = ((int) num1 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                if (index8 != -1)
                  mtX44_1 = mtX44Array[index8];
                mtX44_1 = mtX44_1.MultMatrix((MTX44) this.nodes.data[(int) num9].GetMatrix(MayaScale, posScale));
                nodeList.Add(new MA.Node((float[]) mtX44_1, this.nodes.dict.names[(int) num9], num11 == byte.MaxValue ? (string) null : this.nodes.dict.names[(int) num11]));
                if (index7 != -1)
                {
                  mtX44Array[index7] = mtX44_1;
                  break;
                }
                break;
              case 7:
                num2 = this.sbc[offset1++];
                num3 = ((int) num1 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                num4 = ((int) num1 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                break;
              case 8:
                num2 = this.sbc[offset1++];
                num3 = ((int) num1 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                num4 = ((int) num1 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                break;
              case 9:
                MTX44 mtX44_2 = new MTX44();
                mtX44_2.Zero();
                MTX44 mtX44_3 = new MTX44();
                mtX44_3.Zero();
                MTX44 mtX44_4 = new MTX44();
                MTX44 mtX44_5 = new MTX44();
                float num13 = 0.0f;
                byte[] sbc7 = this.sbc;
                int index9 = offset1;
                int num14 = index9 + 1;
                byte num15 = sbc7[index9];
                byte[] sbc8 = this.sbc;
                int index10 = num14;
                offset1 = index10 + 1;
                byte num16 = sbc8[index10];
                for (int index11 = 0; index11 < (int) num16; ++index11)
                {
                  byte[] sbc9 = this.sbc;
                  int index12 = offset1;
                  int num17 = index12 + 1;
                  byte num18 = sbc9[index12];
                  byte[] sbc10 = this.sbc;
                  int index13 = num17;
                  int num19 = index13 + 1;
                  byte num20 = sbc10[index13];
                  byte[] sbc11 = this.sbc;
                  int index14 = num19;
                  offset1 = index14 + 1;
                  float num21 = (float) sbc11[index14] / 256f;
                  MTX44 mtX44_6 = mtX44Array[(int) num18].MultMatrix(this.evpMatrices.m[(int) num20].invM);
                  if (index11 != 0)
                    mtX44_3 += num13 * mtX44_5;
                  MTX44 mtX44_7 = mtX44_6;
                  MTX44 mtX44_8 = mtX44_6.MultMatrix(this.evpMatrices.m[(int) num20].invN);
                  num13 = num21;
                  mtX44_2 += num13 * mtX44_7;
                  mtX44_5 = mtX44_8;
                }
                MTX44 mtX44_9 = mtX44_3 + num13 * mtX44_5;
                mtX44_1 = mtX44_2;
                mtX44Array[(int) num15] = mtX44_1;
                break;
              case 10:
                Bytes.Read4BytesAsInt32(this.sbc, offset1);
                int offset2 = offset1 + 4;
                Bytes.Read4BytesAsInt32(this.sbc, offset2);
                offset1 = offset2 + 4;
                break;
              case 12:
                byte[] sbc12 = this.sbc;
                int index15 = offset1;
                int num22 = index15 + 1;
                num5 = sbc12[index15];
                byte[] sbc13 = this.sbc;
                int index16 = num22;
                offset1 = index16 + 1;
                num6 = sbc13[index16];
                break;
              case 13:
                byte[] sbc14 = this.sbc;
                int index17 = offset1;
                int num23 = index17 + 1;
                num5 = sbc14[index17];
                byte[] sbc15 = this.sbc;
                int index18 = num23;
                offset1 = index18 + 1;
                num6 = sbc15[index18];
                break;
            }
          }
label_28:
          return MA.WriteBones(nodeList.ToArray());
        }

        public void ExportMesh(NSBTX.TexplttSet Textures, string FileName, string imageFormat)
        {
          List<Group> groupList = new List<Group>();
          List<string> stringList = new List<string>();
          NSBMD.ModelSet.Model.MaterialSet.Material m = (NSBMD.ModelSet.Model.MaterialSet.Material) null;
          MTX44[] MatrixStack = new MTX44[31];
          for (int index = 0; index < 31; ++index)
            MatrixStack[index] = new MTX44();
          int offset1 = 0;
          int posScale = (int) this.info.posScale;
          MTX44 mtX44_1 = new MTX44();
          bool flag1 = true;
          int Alpha = 31;
          while (offset1 != this.sbc.Length)
          {
            byte[] sbc1 = this.sbc;
            int index1 = offset1++;
            byte num1;
            byte num2;
            int num3;
            int num4;
            byte num5;
            byte num6;
            switch ((int) (num1 = sbc1[index1]) & 15)
            {
              case 1:
                goto label_29;
              case 2:
                byte[] sbc2 = this.sbc;
                int index2 = offset1;
                int num7 = index2 + 1;
                num2 = sbc2[index2];
                byte[] sbc3 = this.sbc;
                int index3 = num7;
                offset1 = index3 + 1;
                flag1 = sbc3[index3] == (byte) 1;
                break;
              case 3:
                if (flag1)
                {
                  mtX44_1 = MatrixStack[(int) this.sbc[offset1++]].Clone();
                  break;
                }
                ++offset1;
                break;
              case 4:
                byte num8 = this.sbc[offset1++];
                m = this.materials.materials[(int) num8];
                stringList.Add(this.materials.dict.names[(int) num8]);
                break;
              case 5:
                groupList.Add(GlNitro.glNitroGxRipper(this.shapes.shape[(int) this.sbc[offset1++]].DL, mtX44_1.Clone(), Alpha, ref MatrixStack, posScale, m));
                break;
              case 6:
                byte[] sbc4 = this.sbc;
                int index4 = offset1;
                int num9 = index4 + 1;
                byte num10 = sbc4[index4];
                byte[] sbc5 = this.sbc;
                int index5 = num9;
                int num11 = index5 + 1;
                byte num12 = sbc5[index5];
                byte[] sbc6 = this.sbc;
                int index6 = num11;
                offset1 = index6 + 1;
                byte num13 = sbc6[index6];
                bool flag2 = ((int) num13 & 1) == 1;
                bool flag3 = ((int) num13 >> 1 & 1) == 1;
                bool MayaScale = flag2;
                int index7 = ((int) num1 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                int index8 = ((int) num1 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                if (index8 != -1)
                  mtX44_1 = MatrixStack[index8];
                mtX44_1 = mtX44_1.MultMatrix((MTX44) this.nodes.data[(int) num10].GetMatrix(MayaScale, posScale));
                if (index7 != -1)
                {
                  MatrixStack[index7] = mtX44_1;
                  break;
                }
                break;
              case 7:
                Gl.glCullFace(0);
                num2 = this.sbc[offset1++];
                num3 = ((int) num1 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                num4 = ((int) num1 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                break;
              case 8:
                Gl.glCullFace(0);
                num2 = this.sbc[offset1++];
                num3 = ((int) num1 >> 5 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                num4 = ((int) num1 >> 6 & 1) == 1 ? (int) this.sbc[offset1++] : -1;
                break;
              case 9:
                MTX44 mtX44_2 = new MTX44();
                mtX44_2.Zero();
                MTX44 mtX44_3 = new MTX44();
                mtX44_3.Zero();
                MTX44 mtX44_4 = new MTX44();
                MTX44 mtX44_5 = new MTX44();
                float num14 = 0.0f;
                byte[] sbc7 = this.sbc;
                int index9 = offset1;
                int num15 = index9 + 1;
                byte num16 = sbc7[index9];
                byte[] sbc8 = this.sbc;
                int index10 = num15;
                offset1 = index10 + 1;
                byte num17 = sbc8[index10];
                for (int index11 = 0; index11 < (int) num17; ++index11)
                {
                  byte[] sbc9 = this.sbc;
                  int index12 = offset1;
                  int num18 = index12 + 1;
                  byte num19 = sbc9[index12];
                  byte[] sbc10 = this.sbc;
                  int index13 = num18;
                  int num20 = index13 + 1;
                  byte num21 = sbc10[index13];
                  byte[] sbc11 = this.sbc;
                  int index14 = num20;
                  offset1 = index14 + 1;
                  float num22 = (float) sbc11[index14] / 256f;
                  MTX44 mtX44_6 = MatrixStack[(int) num19].MultMatrix(this.evpMatrices.m[(int) num21].invM);
                  if (index11 != 0)
                    mtX44_3 += num14 * mtX44_5;
                  MTX44 mtX44_7 = mtX44_6;
                  MTX44 mtX44_8 = mtX44_6.MultMatrix(this.evpMatrices.m[(int) num21].invN);
                  num14 = num22;
                  mtX44_2 += num14 * mtX44_7;
                  mtX44_5 = mtX44_8;
                }
                MTX44 mtX44_9 = mtX44_3 + num14 * mtX44_5;
                mtX44_1 = mtX44_2;
                MatrixStack[(int) num16] = mtX44_1;
                break;
              case 10:
                int num23 = Bytes.Read4BytesAsInt32(this.sbc, offset1);
                int offset2 = offset1 + 4;
                int count = Bytes.Read4BytesAsInt32(this.sbc, offset2);
                offset1 = offset2 + 4;
                groupList.Add(GlNitro.glNitroGxRipper(((IEnumerable<byte>) this.sbc).ToList<byte>().GetRange(offset1 - 9 + num23, count).ToArray(), mtX44_1.Clone(), 31, ref MatrixStack, posScale, m));
                break;
              case 11:
                if ((int) num1 >> 5 != 0)
                  break;
                break;
              case 12:
                byte[] sbc12 = this.sbc;
                int index15 = offset1;
                int num24 = index15 + 1;
                num5 = sbc12[index15];
                byte[] sbc13 = this.sbc;
                int index16 = num24;
                offset1 = index16 + 1;
                num6 = sbc13[index16];
                break;
              case 13:
                byte[] sbc14 = this.sbc;
                int index17 = offset1;
                int num25 = index17 + 1;
                num5 = sbc14[index17];
                byte[] sbc15 = this.sbc;
                int index18 = num25;
                offset1 = index18 + 1;
                num6 = sbc15[index18];
                break;
            }
          }
label_29:
          File.Create(FileName).Close();
          TextWriter textWriter1 = (TextWriter) new StreamWriter(FileName);
          textWriter1.WriteLine("# Created by MKDS Course Modifier");
          textWriter1.WriteLine("mtllib {0}", (object) Path.ChangeExtension(Path.GetFileName(FileName), "mtl"));
          int num26 = 1;
          int index19 = 0;
          foreach (Group group in groupList)
          {
            textWriter1.WriteLine("g {0}", (object) this.shapes.dict.names[index19]);
            textWriter1.WriteLine("usemtl {0}", (object) stringList[index19]);
            foreach (Polygon polygon in group)
            {
              foreach (Vector3 vector3 in polygon.Vertex)
                textWriter1.WriteLine("v {0} {1} {2}", (object) (vector3.X * this.info.posScale).ToString().Replace(",", "."), (object) (vector3.Y * this.info.posScale).ToString().Replace(",", "."), (object) (vector3.Z * this.info.posScale).ToString().Replace(",", "."));
              foreach (Vector3 normal in polygon.Normals)
              {
                TextWriter textWriter2 = textWriter1;
                float num1 = normal.X;
                string str1 = num1.ToString().Replace(",", ".");
                num1 = normal.Y;
                string str2 = num1.ToString().Replace(",", ".");
                num1 = normal.Z;
                string str3 = num1.ToString().Replace(",", ".");
                textWriter2.WriteLine("vn {0} {1} {2}", (object) str1, (object) str2, (object) str3);
              }
              foreach (Vector2 texCoord in polygon.TexCoords)
              {
                TextWriter textWriter2 = textWriter1;
                float num1 = texCoord.X;
                string str1 = num1.ToString().Replace(",", ".");
                num1 = texCoord.Y;
                string str2 = num1.ToString().Replace(",", ".");
                textWriter2.WriteLine("vt {0} {1}", (object) str1, (object) str2);
              }
              switch (polygon.PolyType)
              {
                case PolygonType.Triangle:
                  for (int index1 = 0; index1 < polygon.Vertex.Length; index1 += 3)
                  {
                    textWriter1.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", (object) num26, (object) (num26 + 1), (object) (num26 + 2));
                    num26 += 3;
                  }
                  break;
                case PolygonType.Quad:
                  for (int index1 = 0; index1 < polygon.Vertex.Length; index1 += 4)
                  {
                    textWriter1.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2} {3}/{3}/{3}", (object) num26, (object) (num26 + 1), (object) (num26 + 2), (object) (num26 + 3));
                    num26 += 4;
                  }
                  break;
                case PolygonType.TriangleStrip:
                  for (int index1 = 0; index1 + 2 < polygon.Vertex.Length; index1 += 2)
                  {
                    string str1 = "f" + string.Format(" {0}/{0}/{0}", (object) (num26 + index1)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 1)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 2));
                    textWriter1.WriteLine(str1);
                    if (index1 + 3 < polygon.Vertex.Length)
                    {
                      string str2 = "f" + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 1)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 3)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 2));
                      textWriter1.WriteLine(str2);
                    }
                  }
                  num26 += polygon.Vertex.Length;
                  break;
                case PolygonType.QuadStrip:
                  for (int index1 = 0; index1 + 2 < polygon.Vertex.Length; index1 += 2)
                  {
                    string str1 = "f" + string.Format(" {0}/{0}/{0}", (object) (num26 + index1)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 1)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 2));
                    textWriter1.WriteLine(str1);
                    if (index1 + 3 < polygon.Vertex.Length)
                    {
                      string str2 = "f" + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 1)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 3)) + string.Format(" {0}/{0}/{0}", (object) (num26 + index1 + 2));
                      textWriter1.WriteLine(str2);
                    }
                  }
                  num26 += polygon.Vertex.Length;
                  break;
              }
            }
            ++index19;
          }
          textWriter1.Close();
          File.Create(Path.ChangeExtension(FileName, "mtl")).Close();
          TextWriter textWriter3 = (TextWriter) new StreamWriter(Path.ChangeExtension(FileName, "mtl"));
          int index20 = 0;
          foreach (NSBMD.ModelSet.Model.MaterialSet.Material material in this.materials.materials)
          {
            textWriter3.WriteLine("newmtl {0}", (object) this.materials.dict.names[index20]);
            Color color1 = Graphic.ConvertABGR1555((short) ((int) material.diffAmb & (int) short.MaxValue));
            Color color2 = Graphic.ConvertABGR1555((short) ((int) (material.diffAmb >> 16) & (int) short.MaxValue));
            TextWriter textWriter2 = textWriter3;
            float num1 = (float) color2.R / (float) byte.MaxValue;
            string str1 = num1.ToString().Replace(",", ".");
            num1 = (float) color2.G / (float) byte.MaxValue;
            string str2 = num1.ToString().Replace(",", ".");
            num1 = (float) color2.B / (float) byte.MaxValue;
            string str3 = num1.ToString().Replace(",", ".");
            textWriter2.WriteLine("Ka {0} {1} {2}", (object) str1, (object) str2, (object) str3);
            TextWriter textWriter4 = textWriter3;
            num1 = (float) color1.R / (float) byte.MaxValue;
            string str4 = num1.ToString().Replace(",", ".");
            num1 = (float) color1.G / (float) byte.MaxValue;
            string str5 = num1.ToString().Replace(",", ".");
            num1 = (float) color1.B / (float) byte.MaxValue;
            string str6 = num1.ToString().Replace(",", ".");
            textWriter4.WriteLine("Kd {0} {1} {2}", (object) str4, (object) str5, (object) str6);
            TextWriter textWriter5 = textWriter3;
            num1 = (float) (material.polyAttr >> 16 & 31U) / 31f;
            string str7 = num1.ToString().Replace(",", ".");
            textWriter5.WriteLine("d {0}", (object) str7);
            TextWriter textWriter6 = textWriter3;
            num1 = (float) (material.polyAttr >> 16 & 31U) / 31f;
            string str8 = num1.ToString().Replace(",", ".");
            textWriter6.WriteLine("Tr {0}", (object) str8);
            textWriter3.WriteLine("map_Ka {0}.{1}", (object) this.materials.dict.names[index20], (object)imageFormat.ToLower());
            textWriter3.WriteLine("map_Kd {0}.{1}", (object) this.materials.dict.names[index20], (object)imageFormat.ToLower());
            textWriter3.WriteLine("map_d {0}.{1}", (object) this.materials.dict.names[index20], (object) imageFormat.ToLower());
            ++index20;
          }
          textWriter3.Close();
          if (Textures == null)
            return;
          for (int index1 = 0; index1 < this.materials.materials.Length; ++index1)
          {
            NSBTX.TexplttSet.DictTexData dictTexData = (NSBTX.TexplttSet.DictTexData) null;
            for (int index2 = 0; index2 < (int) this.materials.dictTexToMatList.numEntry; ++index2)
            {
              if (((IEnumerable<int>) this.materials.dictTexToMatList[index2].Value.Materials).Contains<int>(index1))
              {
                int index3 = index2;
                KeyValuePair<string, NSBTX.TexplttSet.DictTexData> keyValuePair;
                for (int index4 = 0; index4 < (int) Textures.dictTex.numEntry; ++index4)
                {
                  keyValuePair = Textures.dictTex[index4];
                  if (keyValuePair.Key == this.materials.dictTexToMatList[index2].Key)
                  {
                    index3 = index4;
                    break;
                  }
                }
                keyValuePair = Textures.dictTex[index3];
                dictTexData = keyValuePair.Value;
                break;
              }
            }
            if (dictTexData != null)
            {
              NSBTX.TexplttSet.DictPlttData Palette = (NSBTX.TexplttSet.DictPlttData) null;
              if (dictTexData.Fmt != Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
              {
                for (int index2 = 0; index2 < (int) this.materials.dictPlttToMatList.numEntry; ++index2)
                {
                  if (((IEnumerable<int>) this.materials.dictPlttToMatList[index2].Value.Materials).Contains<int>(index1))
                  {
                    int index3 = index2;
                    KeyValuePair<string, NSBTX.TexplttSet.DictPlttData> keyValuePair;
                    for (int index4 = 0; index4 < (int) Textures.dictPltt.numEntry; ++index4)
                    {
                      keyValuePair = Textures.dictPltt[index4];
                      if (keyValuePair.Key == this.materials.dictPlttToMatList[index2].Key)
                      {
                        index3 = index4;
                        break;
                      }
                    }
                    keyValuePair = Textures.dictPltt[index3];
                    Palette = keyValuePair.Value;
                    break;
                  }
                }
              }
              System.Drawing.Bitmap bitmap = dictTexData.ToBitmap(Palette);
              System.Drawing.Bitmap b = new System.Drawing.Bitmap((int) ((long) bitmap.Width * (long) (uint) (((int) (this.materials.materials[index1].texImageParam >> 18) & 1) + 1)), (int) ((long) bitmap.Height * (long) (uint) (((int) (this.materials.materials[index1].texImageParam >> 19) & 1) + 1)));
              using (Graphics graphics = Graphics.FromImage((Image) b))
              {
                graphics.DrawImage((Image) bitmap, 0, 0);
                bool flag2 = false;
                bool flag3 = false;
                if (((int) (this.materials.materials[index1].texImageParam >> 16) & 1) == 1 && ((int) (this.materials.materials[index1].texImageParam >> 18) & 1) == 1)
                {
                  graphics.DrawImage((Image) bitmap, bitmap.Width * 2, 0, -bitmap.Width, bitmap.Height);
                  flag2 = true;
                }
                if (((int) (this.materials.materials[index1].texImageParam >> 17) & 1) == 1 && ((int) (this.materials.materials[index1].texImageParam >> 19) & 1) == 1)
                {
                  graphics.DrawImage((Image) bitmap, 0, bitmap.Height * 2, bitmap.Width, -bitmap.Height);
                  flag3 = true;
                }
                if (flag2 && flag3)
                  graphics.DrawImage((Image) bitmap, bitmap.Width * 2, bitmap.Height * 2, -bitmap.Width, -bitmap.Height);
              }
              switch (imageFormat) {
                case "PNG":
                  b.Save(Path.GetDirectoryName(FileName) + "\\" + this.materials.dict.names[index1] + ".png", ImageFormat.Png);
                  break;
                case "TIFF":
                  b.Save(Path.GetDirectoryName(FileName) + "\\" + this.materials.dict.names[index1] + ".tiff", ImageFormat.Tiff);
                  break;
                case "TGA":
                  DevIl.SaveAsTGA(b, Path.GetDirectoryName(FileName) + "\\" + this.materials.dict.names[index1] + ".tga");
                  break;
              }
            }
          }
        }

        public class ModelInfo
        {
          public byte sbcType;
          public byte scalingRule;
          public byte texMtxMode;
          public byte numNode;
          public byte numMat;
          public byte numShp;
          public byte firstUnusedMtxStackID;
          public float posScale;
          public float invPosScale;
          public ushort numVertex;
          public ushort numPolygon;
          public ushort numTriangle;
          public ushort numQuad;
          public float boxX;
          public float boxY;
          public float boxZ;
          public float boxW;
          public float boxH;
          public float boxD;
          public float boxPosScale;
          public float boxInvPosScale;

          public ModelInfo(EndianBinaryReader er)
          {
            this.sbcType = er.ReadByte();
            this.scalingRule = er.ReadByte();
            this.texMtxMode = er.ReadByte();
            this.numNode = er.ReadByte();
            this.numMat = er.ReadByte();
            this.numShp = er.ReadByte();
            this.firstUnusedMtxStackID = er.ReadByte();
            int num = (int) er.ReadByte();
            this.posScale = er.ReadSingleInt32Exp12();
            this.invPosScale = er.ReadSingleInt32Exp12();
            this.numVertex = er.ReadUInt16();
            this.numPolygon = er.ReadUInt16();
            this.numTriangle = er.ReadUInt16();
            this.numQuad = er.ReadUInt16();
            this.boxX = er.ReadSingleInt16Exp12();
            this.boxY = er.ReadSingleInt16Exp12();
            this.boxZ = er.ReadSingleInt16Exp12();
            this.boxW = er.ReadSingleInt16Exp12();
            this.boxH = er.ReadSingleInt16Exp12();
            this.boxD = er.ReadSingleInt16Exp12();
            this.boxPosScale = er.ReadSingleInt32Exp12();
            this.boxInvPosScale = er.ReadSingleInt32Exp12();
          }

          public ModelInfo()
          {
          }

          public void Write(EndianBinaryWriter er)
          {
            er.Write(this.sbcType);
            er.Write(this.scalingRule);
            er.Write(this.texMtxMode);
            er.Write(this.numNode);
            er.Write(this.numMat);
            er.Write(this.numShp);
            er.Write(this.firstUnusedMtxStackID);
            er.Write((byte) 0);
            er.Write((uint) ((double) this.posScale * 4096.0));
            er.Write((uint) ((double) this.invPosScale * 4096.0));
            er.Write(this.numVertex);
            er.Write(this.numPolygon);
            er.Write(this.numTriangle);
            er.Write(this.numQuad);
            er.Write((ushort) ((double) this.boxX * 4096.0));
            er.Write((ushort) ((double) this.boxY * 4096.0));
            er.Write((ushort) ((double) this.boxZ * 4096.0));
            er.Write((ushort) ((double) this.boxW * 4096.0));
            er.Write((ushort) ((double) this.boxH * 4096.0));
            er.Write((ushort) ((double) this.boxD * 4096.0));
            er.Write((uint) ((double) this.boxPosScale * 4096.0));
            er.Write((uint) ((double) this.boxInvPosScale * 4096.0));
          }
        }

        public class NodeSet
        {
          public Dictionary<NSBMD.ModelSet.Model.NodeSet.NodeSetData> dict;
          public NSBMD.ModelSet.Model.NodeSet.NodeData[] data;

          public NodeSet(EndianBinaryReader er)
          {
            er.SetMarkerOnCurrentOffset("NodeInfo");
            this.dict = new Dictionary<NSBMD.ModelSet.Model.NodeSet.NodeSetData>(er);
            this.data = new NSBMD.ModelSet.Model.NodeSet.NodeData[(int) this.dict.numEntry];
            long position = er.BaseStream.Position;
            for (int index = 0; index < (int) this.dict.numEntry; ++index)
            {
              er.BaseStream.Position = er.GetMarker("NodeInfo") + (long) this.dict[index].Value.Offset;
              this.data[index] = new NSBMD.ModelSet.Model.NodeSet.NodeData(er);
            }
            er.BaseStream.Position = position;
          }

          public void Write(EndianBinaryWriter er)
          {
            long position1 = er.BaseStream.Position;
            this.dict.Write(er);
            for (int index = 0; index < this.data.Length; ++index)
            {
              this.dict[index].Value.Offset = (uint) (er.BaseStream.Position - position1);
              this.data[index].Write(er);
            }
            long position2 = er.BaseStream.Position;
            er.BaseStream.Position = position1;
            this.dict.Write(er);
            er.BaseStream.Position = position2;
          }

          public NodeSet()
          {
          }

          public class NodeSetData : DictionaryData
          {
            public uint Offset;

            public override ushort GetDataSize()
            {
              return 4;
            }

            public override void Read(EndianBinaryReader er)
            {
              this.Offset = er.ReadUInt32();
            }

            public override void Write(EndianBinaryWriter er)
            {
              er.Write(this.Offset);
            }
          }

          public class NodeData
          {
            public const ushort NNS_G3D_SRTFLAG_TRANS_ZERO = 1;
            public const ushort NNS_G3D_SRTFLAG_ROT_ZERO = 2;
            public const ushort NNS_G3D_SRTFLAG_SCALE_ONE = 4;
            public const ushort NNS_G3D_SRTFLAG_PIVOT_EXIST = 8;
            public ushort flag;
            public short _00;
            public float Tx;
            public float Ty;
            public float Tz;
            public float _01;
            public float _02;
            public float _10;
            public float _11;
            public float _12;
            public float _20;
            public float _21;
            public float _22;
            public float A;
            public float B;
            public float Sx;
            public float Sy;
            public float Sz;
            public float InvSx;
            public float InvSy;
            public float InvSz;

            public NodeData(EndianBinaryReader er)
            {
              this.flag = er.ReadUInt16();
              this._00 = er.ReadInt16();
              if (((int) this.flag & 1) == 0)
              {
                this.Tx = er.ReadSingleInt32Exp12();
                this.Ty = er.ReadSingleInt32Exp12();
                this.Tz = er.ReadSingleInt32Exp12();
              }
              if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) == 0)
              {
                this._01 = er.ReadSingleInt16Exp12();
                this._02 = er.ReadSingleInt16Exp12();
                this._10 = er.ReadSingleInt16Exp12();
                this._11 = er.ReadSingleInt16Exp12();
                this._12 = er.ReadSingleInt16Exp12();
                this._20 = er.ReadSingleInt16Exp12();
                this._21 = er.ReadSingleInt16Exp12();
                this._22 = er.ReadSingleInt16Exp12();
              }
              if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) != 0)
              {
                this.A = er.ReadSingleInt16Exp12();
                this.B = er.ReadSingleInt16Exp12();
              }
              if (((int) this.flag & 4) != 0)
                return;
              this.Sx = er.ReadSingleInt32Exp12();
              this.Sy = er.ReadSingleInt32Exp12();
              this.Sz = er.ReadSingleInt32Exp12();
              this.InvSx = er.ReadSingleInt32Exp12();
              this.InvSy = er.ReadSingleInt32Exp12();
              this.InvSz = er.ReadSingleInt32Exp12();
            }

            public NodeData()
            {
            }

            public void Write(EndianBinaryWriter er)
            {
              er.Write(this.flag);
              er.Write(this._00);
              if (((int) this.flag & 1) == 0)
              {
                er.Write((uint) ((double) this.Tx * 4096.0));
                er.Write((uint) ((double) this.Ty * 4096.0));
                er.Write((uint) ((double) this.Tz * 4096.0));
              }
              if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) == 0)
              {
                er.Write((ushort) ((double) this._01 * 4096.0));
                er.Write((ushort) ((double) this._02 * 4096.0));
                er.Write((ushort) ((double) this._10 * 4096.0));
                er.Write((ushort) ((double) this._11 * 4096.0));
                er.Write((ushort) ((double) this._12 * 4096.0));
                er.Write((ushort) ((double) this._20 * 4096.0));
                er.Write((ushort) ((double) this._21 * 4096.0));
                er.Write((ushort) ((double) this._22 * 4096.0));
              }
              if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) != 0)
              {
                er.Write((ushort) ((double) this.A * 4096.0));
                er.Write((ushort) ((double) this.B * 4096.0));
              }
              if (((int) this.flag & 4) != 0)
                return;
              er.Write((uint) ((double) this.Sx * 4096.0));
              er.Write((uint) ((double) this.Sy * 4096.0));
              er.Write((uint) ((double) this.Sz * 4096.0));
              er.Write((uint) ((double) this.InvSx * 4096.0));
              er.Write((uint) ((double) this.InvSy * 4096.0));
              er.Write((uint) ((double) this.InvSz * 4096.0));
            }

            public float[] GetMatrix(bool MayaScale, int PosScale)
            {
              float[] numArray1 = this.loadIdentity();
              float[] numArray2 = this.loadIdentity();
              float[] numArray3 = this.loadIdentity();
              float[] numArray4 = this.loadIdentity();
              if (((int) this.flag & 1) == 0)
                numArray4 = this.translate(numArray4, this.Tx / (float) PosScale, this.Ty / (float) PosScale, this.Tz / (float) PosScale);
              if (MayaScale && ((int) this.flag & 4) == 0)
                numArray2 = this.scale(numArray2, this.InvSx / (float) PosScale, this.InvSy / (float) PosScale, this.InvSz / (float) PosScale);
              if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) == 0)
              {
                numArray3[0] = (float) this._00 / 4096f;
                numArray3[1] = this._01;
                numArray3[2] = this._02;
                numArray3[4] = this._10;
                numArray3[5] = this._11;
                numArray3[6] = this._12;
                numArray3[8] = this._20;
                numArray3[9] = this._21;
                numArray3[10] = this._22;
              }
              else if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) != 0)
                numArray3 = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(numArray3, GlNitro.glNitroPivot(new float[2]
                {
                  this.A,
                  this.B
                }, (int) this.flag >> 4 & 15, (int) this.flag >> 8 & 15));
              if (((int) this.flag & 4) == 0)
                numArray1 = this.scale(numArray1, this.Sx / (float) PosScale, this.Sy / (float) PosScale, this.Sz / (float) PosScale);
              float[] a = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix((float[]) new MTX44(), numArray4);
              if (MayaScale)
                a = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, numArray2);
              return NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, numArray3), numArray1);
            }

            public float[] GetRotation()
            {
              float[] a = this.loadIdentity();
              if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) == 0)
              {
                a[0] = (float) this._00 / 4096f;
                a[1] = this._01;
                a[2] = this._02;
                a[4] = this._10;
                a[5] = this._11;
                a[6] = this._12;
                a[8] = this._20;
                a[9] = this._21;
                a[10] = this._22;
              }
              else if (((int) this.flag & 2) == 0 && ((int) this.flag & 8) != 0)
                a = NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, GlNitro.glNitroPivot(new float[2]
                {
                  this.A,
                  this.B
                }, (int) this.flag >> 4 & 15, (int) this.flag >> 8 & 15));
              return a;
            }

            private float[] translate(float[] a, float x, float y, float z)
            {
              float[] b = this.loadIdentity();
              b[12] = x;
              b[13] = y;
              b[14] = z;
              return NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, b);
            }

            private float[] loadIdentity()
            {
              float[] numArray = new float[16];
              numArray[0] = 1f;
              numArray[5] = 1f;
              numArray[10] = 1f;
              numArray[15] = 1f;
              return numArray;
            }

            private static float[] multMatrix(float[] a, float[] b)
            {
              float[] numArray = new float[16];
              for (int index1 = 0; index1 < 4; ++index1)
              {
                for (int index2 = 0; index2 < 4; ++index2)
                {
                  numArray[(index1 << 2) + index2] = 0.0f;
                  for (int index3 = 0; index3 < 4; ++index3)
                    numArray[(index1 << 2) + index2] += a[(index3 << 2) + index2] * b[(index1 << 2) + index3];
                }
              }
              return numArray;
            }

            private float[] scale(float[] a, float x, float y, float z)
            {
              float[] b = this.loadIdentity();
              b[0] = x;
              b[5] = y;
              b[10] = z;
              return NSBMD.ModelSet.Model.NodeSet.NodeData.multMatrix(a, b);
            }
          }
        }

        public class MaterialSet
        {
          public ushort ofsDictTexToMatList;
          public ushort ofsDictPlttToMatList;
          public Dictionary<NSBMD.ModelSet.Model.MaterialSet.MaterialSetData> dict;
          public Dictionary<NSBMD.ModelSet.Model.MaterialSet.TexToMatData> dictTexToMatList;
          public Dictionary<NSBMD.ModelSet.Model.MaterialSet.PlttToMatData> dictPlttToMatList;
          public NSBMD.ModelSet.Model.MaterialSet.Material[] materials;

          public MaterialSet(EndianBinaryReader er)
          {
            er.SetMarkerOnCurrentOffset(nameof (MaterialSet));
            this.ofsDictTexToMatList = er.ReadUInt16();
            this.ofsDictPlttToMatList = er.ReadUInt16();
            this.dict = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.MaterialSetData>(er);
            long position = er.BaseStream.Position;
            this.materials = new NSBMD.ModelSet.Model.MaterialSet.Material[(int) this.dict.numEntry];
            for (int index = 0; index < (int) this.dict.numEntry; ++index)
            {
              er.BaseStream.Position = (long) this.dict[index].Value.Offset + er.GetMarker(nameof (MaterialSet));
              this.materials[index] = new NSBMD.ModelSet.Model.MaterialSet.Material(er);
            }
            er.BaseStream.Position = position;
            this.dictTexToMatList = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.TexToMatData>(er);
            this.dictPlttToMatList = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.PlttToMatData>(er);
            while (er.BaseStream.Position % 4L != 0L)
            {
              int num = (int) er.ReadByte();
            }
          }

          public MaterialSet()
          {
          }

          public void Write(EndianBinaryWriter er)
          {
            long position1 = er.BaseStream.Position;
            er.Write((ushort) 0);
            er.Write((ushort) 0);
            this.dict.Write(er);
            long position2 = er.BaseStream.Position;
            er.BaseStream.Position = position1;
            er.Write((ushort) (position2 - position1));
            er.BaseStream.Position = position2;
            this.dictTexToMatList.Write(er);
            long position3 = er.BaseStream.Position;
            er.BaseStream.Position = position1 + 2L;
            er.Write((ushort) (position3 - position1));
            er.BaseStream.Position = position3;
            this.dictPlttToMatList.Write(er);
            for (int index = 0; index < (int) this.dictTexToMatList.numEntry; ++index)
            {
              KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.TexToMatData> dictTexToMat = this.dictTexToMatList[index];
              dictTexToMat.Value.Offset = (ushort) (er.BaseStream.Position - position1);
              dictTexToMat = this.dictTexToMatList[index];
              foreach (int material in dictTexToMat.Value.Materials)
                er.Write((byte) material);
            }
            for (int index = 0; index < (int) this.dictPlttToMatList.numEntry; ++index)
            {
              KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.PlttToMatData> dictPlttToMat = this.dictPlttToMatList[index];
              dictPlttToMat.Value.Offset = (ushort) (er.BaseStream.Position - position1);
              dictPlttToMat = this.dictPlttToMatList[index];
              foreach (int material in dictPlttToMat.Value.Materials)
                er.Write((byte) material);
            }
            while (er.BaseStream.Position % 4L != 0L)
              er.Write((byte) 0);
            for (int index = 0; index < this.materials.Length; ++index)
            {
              this.dict[index].Value.Offset = (uint) (er.BaseStream.Position - position1);
              this.materials[index].Write(er);
            }
            long position4 = er.BaseStream.Position;
            er.BaseStream.Position = position1 + 4L;
            this.dict.Write(er);
            this.dictTexToMatList.Write(er);
            this.dictPlttToMatList.Write(er);
            er.BaseStream.Position = position4;
          }

          public class MaterialSetData : DictionaryData
          {
            public uint Offset;

            public override ushort GetDataSize()
            {
              return 4;
            }

            public override void Read(EndianBinaryReader er)
            {
              this.Offset = er.ReadUInt32();
            }

            public override void Write(EndianBinaryWriter er)
            {
              er.Write(this.Offset);
            }
          }

          public class TexToMatData : DictionaryData
          {
            public uint Flag;
            public ushort Offset;
            public byte NrMat;
            public byte Bound;
            public int[] Materials;

            public override ushort GetDataSize()
            {
              return 4;
            }

            public override void Read(EndianBinaryReader er)
            {
              this.Flag = er.ReadUInt32();
              this.Offset = (ushort) (this.Flag & (uint) ushort.MaxValue);
              this.NrMat = (byte) (this.Flag >> 16 & (uint) sbyte.MaxValue);
              this.Bound = (byte) (this.Flag >> 24 & (uint) byte.MaxValue);
              this.Materials = new int[(int) this.NrMat];
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.Offset + er.GetMarker(nameof (MaterialSet));
              for (int index = 0; index < (int) this.NrMat; ++index)
                this.Materials[index] = (int) er.ReadByte();
              er.BaseStream.Position = position;
            }

            public override void Write(EndianBinaryWriter er)
            {
              this.Flag = (uint) (((int) this.Bound & (int) byte.MaxValue) << 24 | ((int) this.NrMat & (int) byte.MaxValue) << 16 | (int) this.Offset & (int) ushort.MaxValue);
              er.Write(this.Flag);
            }
          }

          public class PlttToMatData : DictionaryData
          {
            public uint Flag;
            public ushort Offset;
            public byte NrMat;
            public byte Bound;
            public int[] Materials;

            public override ushort GetDataSize()
            {
              return 4;
            }

            public override void Read(EndianBinaryReader er)
            {
              this.Flag = er.ReadUInt32();
              this.Offset = (ushort) (this.Flag & (uint) ushort.MaxValue);
              this.NrMat = (byte) (this.Flag >> 16 & (uint) sbyte.MaxValue);
              this.Bound = (byte) (this.Flag >> 24 & (uint) byte.MaxValue);
              this.Materials = new int[(int) this.NrMat];
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.Offset + er.GetMarker(nameof (MaterialSet));
              for (int index = 0; index < (int) this.NrMat; ++index)
                this.Materials[index] = (int) er.ReadByte();
              er.BaseStream.Position = position;
            }

            public override void Write(EndianBinaryWriter er)
            {
              this.Flag = (uint) (((int) this.Bound & (int) byte.MaxValue) << 24 | ((int) this.NrMat & (int) byte.MaxValue) << 16 | (int) this.Offset & (int) ushort.MaxValue);
              er.Write(this.Flag);
            }
          }

          public class Material
          {
            public ushort itemTag;
            public ushort size;
            public uint diffAmb;
            public uint specEmi;
            public uint polyAttr;
            public uint polyAttrMask;
            public uint texImageParam;
            public uint texImageParamMask;
            public ushort texPlttBase;
            public NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG flag;
            public ushort origWidth;
            public ushort origHeight;
            public float magW;
            public float magH;
            public float scaleS;
            public float scaleT;
            public float rotSin;
            public float rotCos;
            public float transS;
            public float transT;
            public float[] effectMtx;
            public Graphic.GXTexFmt Fmt;

            public Material(EndianBinaryReader er)
            {
              this.itemTag = er.ReadUInt16();
              this.size = er.ReadUInt16();
              this.diffAmb = er.ReadUInt32();
              this.specEmi = er.ReadUInt32();
              this.polyAttr = er.ReadUInt32();
              this.polyAttrMask = er.ReadUInt32();
              this.texImageParam = er.ReadUInt32();
              this.texImageParamMask = er.ReadUInt32();
              this.texPlttBase = er.ReadUInt16();
              this.flag = (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) er.ReadUInt16();
              this.origWidth = er.ReadUInt16();
              this.origHeight = er.ReadUInt16();
              this.magW = er.ReadSingleInt32Exp12();
              this.magH = er.ReadSingleInt32Exp12();
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                this.scaleS = er.ReadSingleInt32Exp12();
                this.scaleT = er.ReadSingleInt32Exp12();
              }
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                this.rotSin = er.ReadSingleInt16Exp12();
                this.rotCos = er.ReadSingleInt16Exp12();
              }
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                this.transS = er.ReadSingleInt32Exp12();
                this.transT = er.ReadSingleInt32Exp12();
              }
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX) != NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX)
                return;
              this.effectMtx = er.ReadSingleInt32Exp12s(16);
            }

            public Material()
            {
            }

            public void Write(EndianBinaryWriter er)
            {
              long position1 = er.BaseStream.Position;
              er.Write(this.itemTag);
              er.Write((ushort) 0);
              er.Write(this.diffAmb);
              er.Write(this.specEmi);
              er.Write(this.polyAttr);
              er.Write(this.polyAttrMask);
              er.Write(this.texImageParam);
              er.Write(this.texImageParamMask);
              er.Write(this.texPlttBase);
              er.Write((ushort) this.flag);
              er.Write(this.origWidth);
              er.Write(this.origHeight);
              er.Write((uint) ((double) this.magW * 4096.0));
              er.Write((uint) ((double) this.magH * 4096.0));
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                er.Write((uint) ((double) this.scaleS * 4096.0));
                er.Write((uint) ((double) this.scaleT * 4096.0));
              }
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                er.Write((ushort) ((double) this.rotSin * 4096.0));
                er.Write((ushort) ((double) this.rotCos * 4096.0));
              }
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                er.Write((uint) ((double) this.transS * 4096.0));
                er.Write((uint) ((double) this.transT * 4096.0));
              }
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EFFECTMTX) != (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                foreach (float num in this.effectMtx)
                  er.Write((uint) ((double) num * 4096.0));
              }
              long position2 = er.BaseStream.Position;
              er.BaseStream.Position = position1 + 2L;
              er.Write((ushort) (position2 - position1));
              er.BaseStream.Position = position2;
            }

            public float[] GetMatrix()
            {
              MTX44 mtX44 = new MTX44();
              mtX44.Zero();
              bool flag1 = (this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0;
              bool flag2 = (this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0;
              if ((this.flag & NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO) == (NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG) 0)
              {
                mtX44[0, 3] = this.transS;
                mtX44[1, 3] = this.transT;
              }
              float num1 = flag1 ? this.scaleS : 1f;
              float num2 = flag1 ? this.scaleT : 1f;
              if (flag2)
              {
                mtX44[0, 0] = this.rotCos * num1;
                mtX44[1, 0] = -this.rotSin;
                mtX44[0, 1] = this.rotSin;
                mtX44[1, 1] = -this.rotCos * num2;
              }
              else
              {
                mtX44[0, 0] = num1;
                mtX44[1, 1] = num2;
              }
              mtX44[2, 2] = 1f;
              mtX44[3, 3] = 1f;
              return (float[]) mtX44;
            }

            [Flags]
            public enum NNS_G3D_MATFLAG : ushort
            {
              NNS_G3D_MATFLAG_TEXMTX_USE = 1,
              NNS_G3D_MATFLAG_TEXMTX_SCALEONE = 2,
              NNS_G3D_MATFLAG_TEXMTX_ROTZERO = 4,
              NNS_G3D_MATFLAG_TEXMTX_TRANSZERO = 8,
              NNS_G3D_MATFLAG_ORIGWH_SAME = 16, // 0x0010
              NNS_G3D_MATFLAG_WIREFRAME = 32, // 0x0020
              NNS_G3D_MATFLAG_DIFFUSE = 64, // 0x0040
              NNS_G3D_MATFLAG_AMBIENT = 128, // 0x0080
              NNS_G3D_MATFLAG_VTXCOLOR = 256, // 0x0100
              NNS_G3D_MATFLAG_SPECULAR = 512, // 0x0200
              NNS_G3D_MATFLAG_EMISSION = 1024, // 0x0400
              NNS_G3D_MATFLAG_SHININESS = 2048, // 0x0800
              NNS_G3D_MATFLAG_TEXPLTTBASE = 4096, // 0x1000
              NNS_G3D_MATFLAG_EFFECTMTX = 8192, // 0x2000
            }
          }
        }

        public class ShapeSet
        {
          public Dictionary<NSBMD.ModelSet.Model.ShapeSet.ShapeSetData> dict;
          public NSBMD.ModelSet.Model.ShapeSet.Shape[] shape;

          public ShapeSet(EndianBinaryReader er)
          {
            er.SetMarkerOnCurrentOffset(nameof (ShapeSet));
            this.dict = new Dictionary<NSBMD.ModelSet.Model.ShapeSet.ShapeSetData>(er);
            this.shape = new NSBMD.ModelSet.Model.ShapeSet.Shape[(int) this.dict.numEntry];
            long position = er.BaseStream.Position;
            for (int index = 0; index < (int) this.dict.numEntry; ++index)
            {
              er.BaseStream.Position = (long) this.dict[index].Value.Offset + er.GetMarker(nameof (ShapeSet));
              this.shape[index] = new NSBMD.ModelSet.Model.ShapeSet.Shape(er);
            }
            er.BaseStream.Position = position;
          }

          public ShapeSet()
          {
          }

          public void Write(EndianBinaryWriter er)
          {
            long position1 = er.BaseStream.Position;
            this.dict.Write(er);
            for (int index = 0; index < this.shape.Length; ++index)
            {
              this.dict[index].Value.Offset = (uint) (er.BaseStream.Position - position1);
              this.shape[index].Write(er);
            }
            for (int index = 0; index < this.shape.Length; ++index)
            {
              this.shape[index].ofsDL = (uint) (er.BaseStream.Position - position1 - (long) this.dict[index].Value.Offset);
              er.Write(this.shape[index].DL, 0, this.shape[index].DL.Length);
            }
            long position2 = er.BaseStream.Position;
            er.BaseStream.Position = position1;
            this.dict.Write(er);
            for (int index = 0; index < this.shape.Length; ++index)
              this.shape[index].Write(er);
            er.BaseStream.Position = position2;
          }

          public class ShapeSetData : DictionaryData
          {
            public uint Offset;

            public override ushort GetDataSize()
            {
              return 4;
            }

            public override void Read(EndianBinaryReader er)
            {
              this.Offset = er.ReadUInt32();
            }

            public override void Write(EndianBinaryWriter er)
            {
              er.Write(this.Offset);
            }
          }

          public class Shape
          {
            public ushort itemTag;
            public ushort size;
            public NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG flag;
            public uint ofsDL;
            public uint sizeDL;
            public byte[] DL;

            public Shape(EndianBinaryReader er)
            {
              long position1 = er.BaseStream.Position;
              this.itemTag = er.ReadUInt16();
              this.size = er.ReadUInt16();
              this.flag = (NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG) er.ReadUInt32();
              this.ofsDL = er.ReadUInt32();
              this.sizeDL = er.ReadUInt32();
              long position2 = er.BaseStream.Position;
              er.BaseStream.Position = position1 + (long) this.ofsDL;
              this.DL = er.ReadBytes((int) this.sizeDL);
              er.BaseStream.Position = position2;
            }

            public Shape()
            {
            }

            public void Write(EndianBinaryWriter er)
            {
              long position1 = er.BaseStream.Position;
              er.Write(this.itemTag);
              er.Write((ushort) 0);
              er.Write((uint) this.flag);
              er.Write(this.ofsDL);
              er.Write((uint) this.DL.Length);
              long position2 = er.BaseStream.Position;
              er.BaseStream.Position = position1 + 2L;
              er.Write((ushort) (position2 - position1));
              er.BaseStream.Position = position2;
            }

            [Flags]
            public enum NNS_G3D_SHPFLAG : uint
            {
              NNS_G3D_SHPFLAG_USE_NORMAL = 1,
              NNS_G3D_SHPFLAG_USE_COLOR = 2,
              NNS_G3D_SHPFLAG_USE_TEXCOORD = 4,
              NNS_G3D_SHPFLAG_USE_RESTOREMTX = 8,
            }
          }
        }

        public class EvpMatrices
        {
          public NSBMD.ModelSet.Model.EvpMatrices.envelope[] m;

          public EvpMatrices(EndianBinaryReader er, int NumNodes)
          {
            this.m = new NSBMD.ModelSet.Model.EvpMatrices.envelope[NumNodes];
            for (int index = 0; index < NumNodes; ++index)
              this.m[index] = new NSBMD.ModelSet.Model.EvpMatrices.envelope(er);
          }

          public void Write(EndianBinaryWriter er)
          {
            foreach (NSBMD.ModelSet.Model.EvpMatrices.envelope envelope in this.m)
              envelope.Write(er);
          }

          public class envelope
          {
            public MTX44 invM;
            public MTX44 invN;

            public envelope(EndianBinaryReader er)
            {
              this.invM = new MTX44();
              for (int index1 = 0; index1 < 4; ++index1)
              {
                for (int index2 = 0; index2 < 3; ++index2)
                  this.invM[index2, index1] = er.ReadSingleInt32Exp12();
              }
              this.invN = new MTX44();
              for (int index1 = 0; index1 < 3; ++index1)
              {
                for (int index2 = 0; index2 < 3; ++index2)
                  this.invN[index2, index1] = er.ReadSingleInt32Exp12();
              }
            }

            public void Write(EndianBinaryWriter er)
            {
              for (int index1 = 0; index1 < 4; ++index1)
              {
                for (int index2 = 0; index2 < 3; ++index2)
                  er.Write((uint) ((double) this.invM[index2, index1] * 4096.0));
              }
              for (int index1 = 0; index1 < 3; ++index1)
              {
                for (int index2 = 0; index2 < 3; ++index2)
                  er.Write((uint) ((double) this.invN[index2, index1] * 4096.0));
              }
            }
          }
        }
      }
    }
  }
}
