// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters._3D.Obj2Nsbmd
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters.Colission;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Tao.OpenGl;

namespace MKDS_Course_Modifier.Converters._3D
{
  public class Obj2Nsbmd
  {
    public static void ConvertToNsbmd(string infile, string outfile)
    {
      OBJ obj = OBJ.FixNitroUV(new OBJ(infile));
      if (obj == null)
        return;
      MLT mlt = new MLT(obj.MLTName);
      NSBMDSettings nsbmdSettings = new NSBMDSettings();
      int num1 = (int) nsbmdSettings.ShowDialog();
      List<string> stringList = new List<string>();
      float scale1 = nsbmdSettings.Scale;
      bool createNsbtx = nsbmdSettings.CreateNSBTX;
      Vector3 vector3_1 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
      Vector3 vector3_2 = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
      for (int index = 0; index < obj.Vertices.Count; ++index)
      {
        obj.Vertices[index] = Vector3.Multiply(obj.Vertices[index], scale1);
        if ((double) obj.Vertices[index].X < (double) vector3_1.X)
          vector3_1.X = obj.Vertices[index].X;
        if ((double) obj.Vertices[index].X > (double) vector3_2.X)
          vector3_2.X = obj.Vertices[index].X;
        if ((double) obj.Vertices[index].Y < (double) vector3_1.Y)
          vector3_1.Y = obj.Vertices[index].Y;
        if ((double) obj.Vertices[index].Y > (double) vector3_2.Y)
          vector3_2.Y = obj.Vertices[index].Y;
        if ((double) obj.Vertices[index].Z < (double) vector3_1.Z)
          vector3_1.Z = obj.Vertices[index].Z;
        if ((double) obj.Vertices[index].Z > (double) vector3_2.Z)
          vector3_2.Z = obj.Vertices[index].Z;
      }
      Vector3 vector3_3 = vector3_2 - vector3_1;
      float num2 = Helpers.max(vector3_3.X, vector3_3.Y, vector3_3.Z);
      float scale2 = 1f;
      int num3 = 0;
      for (; (double) num2 > 7.999755859375; num2 /= 2f)
      {
        ++num3;
        scale2 /= 2f;
      }
      for (int index = 0; index < obj.Vertices.Count; ++index)
        obj.Vertices[index] = Vector3.Multiply(obj.Vertices[index], scale2);
      NSBMD nsbmd = new NSBMD(!createNsbtx);
      nsbmd.modelSet = new NSBMD.ModelSet();
      string Name1 = Path.GetFileNameWithoutExtension(infile);
      if (Name1.Length > 16)
        Name1 = Name1.Remove(16);
      nsbmd.modelSet.dict = new Dictionary<NSBMD.ModelSet.MDL0Data>();
      nsbmd.modelSet.dict.Add(Name1, new NSBMD.ModelSet.MDL0Data());
      nsbmd.modelSet.models = new NSBMD.ModelSet.Model[1];
      nsbmd.modelSet.models[0] = new NSBMD.ModelSet.Model();
      nsbmd.modelSet.models[0].info = new NSBMD.ModelSet.Model.ModelInfo();
      nsbmd.modelSet.models[0].info.numNode = (byte) 1;
      foreach (OBJ.Face face in obj.Faces)
      {
        if (!stringList.Contains(face.MaterialName))
          stringList.Add(face.MaterialName);
        ++nsbmd.modelSet.models[0].info.numTriangle;
      }
      nsbmd.modelSet.models[0].info.numMat = (byte) stringList.Count;
      nsbmd.modelSet.models[0].info.numShp = (byte) stringList.Count;
      nsbmd.modelSet.models[0].info.firstUnusedMtxStackID = (byte) 1;
      nsbmd.modelSet.models[0].info.posScale = (float) (1 << num3);
      nsbmd.modelSet.models[0].info.invPosScale = 1f / nsbmd.modelSet.models[0].info.posScale;
      nsbmd.modelSet.models[0].info.numVertex = (ushort) (byte) obj.Vertices.Count;
      nsbmd.modelSet.models[0].info.numPolygon = (ushort) (byte) obj.Faces.Count;
      nsbmd.modelSet.models[0].info.boxX = vector3_1.X * scale2;
      nsbmd.modelSet.models[0].info.boxY = vector3_1.Y * scale2;
      nsbmd.modelSet.models[0].info.boxZ = vector3_1.Z * scale2;
      nsbmd.modelSet.models[0].info.boxW = vector3_3.X * scale2;
      nsbmd.modelSet.models[0].info.boxH = vector3_3.Y * scale2;
      nsbmd.modelSet.models[0].info.boxD = vector3_3.Z * scale2;
      nsbmd.modelSet.models[0].info.boxPosScale = (float) (1 << num3);
      nsbmd.modelSet.models[0].info.boxInvPosScale = 1f / nsbmd.modelSet.models[0].info.boxPosScale;
      nsbmd.modelSet.models[0].nodes = new NSBMD.ModelSet.Model.NodeSet();
      nsbmd.modelSet.models[0].nodes.dict = new Dictionary<NSBMD.ModelSet.Model.NodeSet.NodeSetData>();
      nsbmd.modelSet.models[0].nodes.dict.Add("world_root", new NSBMD.ModelSet.Model.NodeSet.NodeSetData());
      nsbmd.modelSet.models[0].nodes.data = new NSBMD.ModelSet.Model.NodeSet.NodeData[1];
      nsbmd.modelSet.models[0].nodes.data[0] = new NSBMD.ModelSet.Model.NodeSet.NodeData();
      nsbmd.modelSet.models[0].nodes.data[0].flag = (ushort) 7;
      nsbmd.modelSet.models[0].materials = new NSBMD.ModelSet.Model.MaterialSet();
      nsbmd.modelSet.models[0].materials.dictTexToMatList = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.TexToMatData>();
      nsbmd.modelSet.models[0].materials.dictPlttToMatList = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.PlttToMatData>();
      nsbmd.modelSet.models[0].materials.dict = new Dictionary<NSBMD.ModelSet.Model.MaterialSet.MaterialSetData>();
      nsbmd.modelSet.models[0].materials.materials = new NSBMD.ModelSet.Model.MaterialSet.Material[stringList.Count];
      int index1 = 0;
      int index2 = 0;
      foreach (string Name2 in stringList)
      {
        MLT.Material materialByName = mlt.GetMaterialByName(Name2);
        if (materialByName.DiffuseMap != null)
        {
          nsbmd.modelSet.models[0].materials.dictTexToMatList.Add(index1.ToString() + "_t", new NSBMD.ModelSet.Model.MaterialSet.TexToMatData());
          nsbmd.modelSet.models[0].materials.dictPlttToMatList.Add(index1.ToString() + "_p", new NSBMD.ModelSet.Model.MaterialSet.PlttToMatData());
          nsbmd.modelSet.models[0].materials.dictTexToMatList[index2].Value.NrMat = (byte) 1;
          nsbmd.modelSet.models[0].materials.dictTexToMatList[index2].Value.Materials = new int[1]
          {
            index1
          };
          KeyValuePair<string, NSBMD.ModelSet.Model.MaterialSet.PlttToMatData> dictPlttToMat = nsbmd.modelSet.models[0].materials.dictPlttToMatList[index2];
          dictPlttToMat.Value.NrMat = (byte) 1;
          dictPlttToMat = nsbmd.modelSet.models[0].materials.dictPlttToMatList[index2];
          dictPlttToMat.Value.Materials = new int[1]
          {
            index1
          };
          ++index2;
        }
        nsbmd.modelSet.models[0].materials.dict.Add(index1.ToString() + "_m", new NSBMD.ModelSet.Model.MaterialSet.MaterialSetData());
        nsbmd.modelSet.models[0].materials.materials[index1] = new NSBMD.ModelSet.Model.MaterialSet.Material();
        NSBMD.ModelSet.Model.MaterialSet.Material material1 = nsbmd.modelSet.models[0].materials.materials[index1];
        Color color = Color.Black;
        int num4 = (Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue) << 16 | 32768;
        int argb;
        if (materialByName.DiffuseMap == null)
        {
          argb = materialByName.DiffuseColor.ToArgb();
        }
        else
        {
          color = Color.FromArgb(200, 200, 200);
          argb = color.ToArgb();
        }
        int num5 = Graphic.encodeColor(argb) & (int) short.MaxValue;
        int num6 = num4 | num5;
        material1.diffAmb = (uint) num6;
        NSBMD.ModelSet.Model.MaterialSet.Material material2 = nsbmd.modelSet.models[0].materials.materials[index1];
        color = Color.Black;
        int num7 = (Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue) << 16;
        color = Color.Black;
        int num8 = Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue;
        int num9 = num7 | num8;
        material2.specEmi = (uint) num9;
        uint num10 = (uint) ((double) materialByName.Alpha * 31.0);
        nsbmd.modelSet.models[0].materials.materials[index1].polyAttr = 0U;
        nsbmd.modelSet.models[0].materials.materials[index1].polyAttr |= 192U;
        nsbmd.modelSet.models[0].materials.materials[index1].polyAttr |= num10 << 16;
        nsbmd.modelSet.models[0].materials.materials[index1].polyAttrMask = 1059059967U;
        nsbmd.modelSet.models[0].materials.materials[index1].texImageParam = 196608U;
        nsbmd.modelSet.models[0].materials.materials[index1].texImageParamMask = uint.MaxValue;
        nsbmd.modelSet.models[0].materials.materials[index1].texPlttBase = (ushort) 0;
        nsbmd.modelSet.models[0].materials.materials[index1].flag = NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_SCALEONE | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_TRANSZERO | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_DIFFUSE | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_AMBIENT | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_VTXCOLOR | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SPECULAR | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EMISSION | NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SHININESS;
        if (materialByName.DiffuseMap != null)
        {
          nsbmd.modelSet.models[0].materials.materials[index1].origWidth = (ushort) materialByName.DiffuseMap.Width;
          nsbmd.modelSet.models[0].materials.materials[index1].origHeight = (ushort) materialByName.DiffuseMap.Height;
        }
        nsbmd.modelSet.models[0].materials.materials[index1].magW = 1f;
        nsbmd.modelSet.models[0].materials.materials[index1].magH = 1f;
        ++index1;
      }
      nsbmd.modelSet.models[0].shapes = new NSBMD.ModelSet.Model.ShapeSet();
      nsbmd.modelSet.models[0].shapes.dict = new Dictionary<NSBMD.ModelSet.Model.ShapeSet.ShapeSetData>();
      nsbmd.modelSet.models[0].shapes.shape = new NSBMD.ModelSet.Model.ShapeSet.Shape[stringList.Count];
      int index3 = 0;
      foreach (string Name2 in stringList)
      {
        int num4 = 1;
        int num5 = 1;
        MLT.Material materialByName = mlt.GetMaterialByName(Name2);
        List<Color> colorList = (List<Color>) null;
        if (materialByName.DiffuseMap != null)
        {
          num4 = materialByName.DiffuseMap.Width;
          num5 = -materialByName.DiffuseMap.Height;
        }
        else
          colorList = new List<Color>();
        nsbmd.modelSet.models[0].shapes.dict.Add(index3.ToString() + "_py", new NSBMD.ModelSet.Model.ShapeSet.ShapeSetData());
        List<Vector3> vector3List1 = new List<Vector3>();
        List<Vector2> vector2List = new List<Vector2>();
        List<Vector3> vector3List2 = new List<Vector3>();
        foreach (OBJ.Face face in obj.Faces)
        {
          if (face.MaterialName == Name2)
          {
            vector3List1.AddRange((IEnumerable<Vector3>) new Vector3[3]
            {
              obj.Vertices[face.VertexIndieces[0]],
              obj.Vertices[face.VertexIndieces[1]],
              obj.Vertices[face.VertexIndieces[2]]
            });
            Vector2[] vector2Array = new Vector2[3]
            {
              obj.TexCoords[face.TexCoordIndieces[0]],
              obj.TexCoords[face.TexCoordIndieces[1]],
              obj.TexCoords[face.TexCoordIndieces[2]]
            };
            vector2Array[0].X *= (float) num4;
            vector2Array[0].Y *= (float) num5;
            vector2Array[1].X *= (float) num4;
            vector2Array[1].Y *= (float) num5;
            vector2Array[2].X *= (float) num4;
            vector2Array[2].Y *= (float) num5;
            vector2List.AddRange((IEnumerable<Vector2>) vector2Array);
            if (face.NormalIndieces.Count != 0)
              vector3List2.AddRange((IEnumerable<Vector3>) new Vector3[3]
              {
                obj.Normals[face.NormalIndieces[0]],
                obj.Normals[face.NormalIndieces[1]],
                obj.Normals[face.NormalIndieces[2]]
              });
            colorList?.AddRange((IEnumerable<Color>) new Color[3]
            {
              materialByName.DiffuseColor,
              materialByName.DiffuseColor,
              materialByName.DiffuseColor
            });
          }
        }
        nsbmd.modelSet.models[0].shapes.shape[index3] = new NSBMD.ModelSet.Model.ShapeSet.Shape();
        nsbmd.modelSet.models[0].shapes.shape[index3].DL = GlNitro.GenerateDl(vector3List1.ToArray(), vector2List.ToArray(), vector3List2.ToArray(), colorList?.ToArray());
        nsbmd.modelSet.models[0].shapes.shape[index3].sizeDL = (uint) nsbmd.modelSet.models[0].shapes.shape[index3].DL.Length;
        nsbmd.modelSet.models[0].shapes.shape[index3].flag = NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG.NNS_G3D_SHPFLAG_USE_NORMAL | NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG.NNS_G3D_SHPFLAG_USE_COLOR | NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG.NNS_G3D_SHPFLAG_USE_TEXCOORD;
        ++index3;
      }
      Obj2Nsbmd.SBCWriter sbcWriter = new Obj2Nsbmd.SBCWriter();
      sbcWriter.NODEDESC((byte) 0, (byte) 0, false, false, 0, -1);
      sbcWriter.NODE((byte) 0, true);
      sbcWriter.POSSCALE(true);
      for (int index4 = 0; index4 < stringList.Count; ++index4)
      {
        sbcWriter.MAT((byte) index4);
        sbcWriter.SHP((byte) index4);
      }
      sbcWriter.POSSCALE(false);
      sbcWriter.RET();
      sbcWriter.NOP();
      nsbmd.modelSet.models[0].sbc = sbcWriter.GetData();
      NSBTX.TexplttSet texplttSet = new NSBTX.TexplttSet();
      texplttSet.TexInfo = new NSBTX.TexplttSet.texInfo();
      texplttSet.Tex4x4Info = new NSBTX.TexplttSet.tex4x4Info();
      texplttSet.PlttInfo = new NSBTX.TexplttSet.plttInfo();
      texplttSet.dictTex = new Dictionary<NSBTX.TexplttSet.DictTexData>();
      texplttSet.dictPltt = new Dictionary<NSBTX.TexplttSet.DictPlttData>();
      int num11 = 0;
      int index5 = 0;
      foreach (string Name2 in stringList)
      {
        MLT.Material materialByName = mlt.GetMaterialByName(Name2);
        if (materialByName.DiffuseMap != null)
        {
          BitmapData bitmapdata = materialByName.DiffuseMap.LockBits(new Rectangle(0, 0, materialByName.DiffuseMap.Width, materialByName.DiffuseMap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
          List<Color> source = new List<Color>();
          bool flag = false;
          for (int index4 = 0; index4 < materialByName.DiffuseMap.Width * materialByName.DiffuseMap.Height; ++index4)
          {
            source.Add(Color.FromArgb(Marshal.ReadInt32(bitmapdata.Scan0, index4 * 4)));
            Color color = source.Last<Color>();
            int num4;
            if (color.A != (byte) 0)
            {
              color = source.Last<Color>();
              if (color.A != byte.MaxValue)
              {
                num4 = flag ? 1 : 0;
                goto label_74;
              }
            }
            num4 = 1;
label_74:
            if (num4 == 0)
              flag = true;
          }
          materialByName.DiffuseMap.UnlockBits(bitmapdata);
          List<Color> list = source.Distinct<Color>().ToList<Color>();
          texplttSet.dictTex.Add(num11.ToString() + "_t", new NSBTX.TexplttSet.DictTexData());
          texplttSet.dictPltt.Add(num11.ToString() + "_p", new NSBTX.TexplttSet.DictPlttData());
          KeyValuePair<string, NSBTX.TexplttSet.DictTexData> keyValuePair = texplttSet.dictTex[index5];
          keyValuePair.Value.S = (ushort) materialByName.DiffuseMap.Width;
          keyValuePair = texplttSet.dictTex[index5];
          keyValuePair.Value.T = (ushort) materialByName.DiffuseMap.Height;
          if (flag && list.Count <= 8)
          {
            keyValuePair = texplttSet.dictTex[index5];
            keyValuePair.Value.Fmt = Graphic.GXTexFmt.GX_TEXFMT_A5I3;
          }
          else if (flag)
          {
            keyValuePair = texplttSet.dictTex[index5];
            keyValuePair.Value.Fmt = Graphic.GXTexFmt.GX_TEXFMT_A3I5;
          }
          else if (list.Count <= 16)
          {
            keyValuePair = texplttSet.dictTex[index5];
            keyValuePair.Value.Fmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT16;
          }
          else
          {
            keyValuePair = texplttSet.dictTex[index5];
            keyValuePair.Value.Fmt = Graphic.GXTexFmt.GX_TEXFMT_COMP4x4;
          }
          System.Drawing.Bitmap diffuseMap = materialByName.DiffuseMap;
          keyValuePair = texplttSet.dictTex[index5];
          ref byte[] local1 = ref keyValuePair.Value.Data;
          ref byte[] local2 = ref texplttSet.dictPltt[index5].Value.Data;
          keyValuePair = texplttSet.dictTex[index5];
          ref byte[] local3 = ref keyValuePair.Value.Data4x4;
          keyValuePair = texplttSet.dictTex[index5];
          int fmt = (int) keyValuePair.Value.Fmt;
          keyValuePair = texplttSet.dictTex[index5];
          ref bool local4 = ref keyValuePair.Value.TransparentColor;
          Graphic.ConvertBitmap(diffuseMap, out local1, out local2, out local3, (Graphic.GXTexFmt) fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, out local4, false);
          ++index5;
        }
        ++num11;
      }
      if (!createNsbtx)
      {
        nsbmd.TexPlttSet = texplttSet;
      }
      else
      {
        NSBTX nsbtx = new NSBTX();
        nsbtx.TexPlttSet = texplttSet;
        File.Create(Path.ChangeExtension(outfile, "nsbtx")).Close();
        File.WriteAllBytes(Path.ChangeExtension(outfile, "nsbtx"), nsbtx.Write());
      }
      File.Create(outfile).Close();
      File.WriteAllBytes(outfile, nsbmd.Write());
    }

    public class SBCWriter
    {
      private List<byte> Data = new List<byte>();

      public void NOP()
      {
        this.Data.Add((byte) 0);
      }

      public void RET()
      {
        this.Data.Add((byte) 1);
      }

      public void NODE(byte NodeID, bool V)
      {
        this.Data.Add((byte) 2);
        this.Data.Add(NodeID);
        this.Data.Add(V ? (byte) 1 : (byte) 0);
      }

      public void MTX(byte Idx)
      {
        this.Data.Add((byte) 3);
        this.Data.Add(Idx);
      }

      public void MAT(byte MatID)
      {
        this.Data.Add((byte) 36);
        this.Data.Add(MatID);
      }

      public void SHP(byte ShpID)
      {
        this.Data.Add((byte) 5);
        this.Data.Add(ShpID);
      }

      public void NODEDESC(byte NodeID, byte ParentNodeID, bool S, bool P, byte SrcIdx)
      {
        this.NODEDESC(NodeID, ParentNodeID, S, P, -1, (int) SrcIdx);
      }

      public void NODEDESC(
        byte NodeID,
        byte ParentNodeID,
        bool S,
        bool P,
        int DestIdx = -1,
        int SrcIdx = -1)
      {
        this.Data.Add((byte) (6 | (SrcIdx != -1 ? 1 : 0) << 6 | (DestIdx != -1 ? 1 : 0) << 5));
        this.Data.Add(NodeID);
        this.Data.Add(ParentNodeID);
        this.Data.Add((byte) ((P ? 1 : 0) << 1 | (S ? 1 : 0)));
        if (DestIdx != -1)
          this.Data.Add((byte) DestIdx);
        if (SrcIdx == -1)
          return;
        this.Data.Add((byte) SrcIdx);
      }

      public void BB(byte NodeID, byte SrcIdx)
      {
        this.BB(NodeID, -1, (int) SrcIdx);
      }

      public void BB(byte NodeID, int DestIdx = -1, int SrcIdx = -1)
      {
        this.Data.Add((byte) (7 | (SrcIdx != -1 ? 1 : 0) << 6 | (DestIdx != -1 ? 1 : 0) << 5));
        this.Data.Add(NodeID);
        if (DestIdx != -1)
          this.Data.Add((byte) DestIdx);
        if (SrcIdx == -1)
          return;
        this.Data.Add((byte) SrcIdx);
      }

      public void BBY(byte NodeID, byte SrcIdx)
      {
        this.BB(NodeID, -1, (int) SrcIdx);
      }

      public void BBY(byte NodeID, int DestIdx = -1, int SrcIdx = -1)
      {
        this.Data.Add((byte) (8 | (SrcIdx != -1 ? 1 : 0) << 6 | (DestIdx != -1 ? 1 : 0) << 5));
        this.Data.Add(NodeID);
        if (DestIdx != -1)
          this.Data.Add((byte) DestIdx);
        if (SrcIdx == -1)
          return;
        this.Data.Add((byte) SrcIdx);
      }

      public void NODEMIX(byte DestIdx, byte NumMtx, byte[] SrcIdx, byte[] NodeID, byte[] Ratio)
      {
        this.Data.Add((byte) 9);
        this.Data.Add(DestIdx);
        this.Data.Add(NumMtx);
        for (int index = 0; index < (int) NumMtx; ++index)
        {
          this.Data.Add(SrcIdx[index]);
          this.Data.Add(NodeID[index]);
          this.Data.Add(Ratio[index]);
        }
      }

      public void CALLDL(uint RelAddr, uint Size)
      {
        this.Data.Add((byte) 10);
        this.Data.AddRange((IEnumerable<byte>) BitConverter.GetBytes(RelAddr));
        this.Data.AddRange((IEnumerable<byte>) BitConverter.GetBytes(Size));
      }

      public void POSSCALE(bool OPT)
      {
        this.Data.Add((byte) (11 | (OPT ? 0 : 1) << 5));
      }

      public void ENVMAP(byte MatID, byte Flag)
      {
        this.Data.Add((byte) 12);
        this.Data.Add(MatID);
        this.Data.Add(Flag);
      }

      public void PRJMAP(byte MatID, byte Flag)
      {
        this.Data.Add((byte) 13);
        this.Data.Add(MatID);
        this.Data.Add(Flag);
      }

      public byte[] GetData()
      {
        return this.Data.ToArray();
      }
    }
  }
}
