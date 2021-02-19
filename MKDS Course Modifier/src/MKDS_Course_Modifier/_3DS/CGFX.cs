// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3DS.CGFX
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Exceptions;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace MKDS_Course_Modifier._3DS
{
  public class CGFX
  {
    public CGFX.CGFXHeader Header;
    public CGFX.DATA Data;

    public CGFX(byte[] data)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(data), Endianness.LittleEndian);
      this.Header = new CGFX.CGFXHeader(er);
      this.Data = new CGFX.DATA(er);
      er.Close();
    }

    public class CGFXHeader
    {
      public string Signature;
      public ushort Endianness;
      public uint HeaderSize;
      public ushort Unknown1;
      public uint FileSize;
      public uint NrBlocks;

      public CGFXHeader(EndianBinaryReader er)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (CGFX))
          throw new SignatureNotCorrectException(this.Signature, nameof (CGFX), er.BaseStream.Position);
        this.Endianness = er.ReadUInt16();
        this.HeaderSize = er.ReadUInt32();
        this.Unknown1 = er.ReadUInt16();
        this.FileSize = er.ReadUInt32();
        this.NrBlocks = er.ReadUInt32();
      }
    }

    public class DATA
    {
      public string Signature;
      public uint SectionSize;
      public CGFX.DATA.DictionaryInfo[] DictionaryEntries;
      public CGFX.DICT[] Dictionaries;
      public CGFX.DATA.CMDL[] Models;
      public CGFX.DATA.TXOB[] Textures;

      public DATA(EndianBinaryReader er)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (DATA))
          throw new SignatureNotCorrectException(this.Signature, nameof (DATA), er.BaseStream.Position);
        this.SectionSize = er.ReadUInt32();
        this.DictionaryEntries = new CGFX.DATA.DictionaryInfo[16];
        for (int index = 0; index < 16; ++index)
          this.DictionaryEntries[index] = new CGFX.DATA.DictionaryInfo(er);
        this.Dictionaries = new CGFX.DICT[16];
        for (int index = 0; index < 16; ++index)
        {
          if (index == 15 && this.DictionaryEntries[index].NrItems == 1413695812U)
          {
            this.DictionaryEntries[index].NrItems = 0U;
            this.DictionaryEntries[index].Offset = 0U;
          }
          if (this.DictionaryEntries[index].Offset != 0U)
          {
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.DictionaryEntries[index].Offset;
            this.Dictionaries[index] = new CGFX.DICT(er);
            er.BaseStream.Position = position;
          }
          else
            this.Dictionaries[index] = (CGFX.DICT) null;
        }
        if (this.Dictionaries[0] != null)
        {
          this.Models = new CGFX.DATA.CMDL[(IntPtr) this.Dictionaries[0].NrEntries];
          for (int index = 0; (long) index < (long) this.Dictionaries[0].NrEntries; ++index)
          {
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.Dictionaries[0].Entries[index].DataOffset;
            this.Models[index] = new CGFX.DATA.CMDL(er);
            er.BaseStream.Position = position;
          }
        }
        if (this.Dictionaries[1] == null)
          return;
        this.Textures = new CGFX.DATA.TXOB[(IntPtr) this.Dictionaries[1].NrEntries];
        for (int index = 0; (long) index < (long) this.Dictionaries[1].NrEntries; ++index)
        {
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.Dictionaries[1].Entries[index].DataOffset;
          this.Textures[index] = new CGFX.DATA.TXOB(er);
          er.BaseStream.Position = position;
        }
      }

      public OBJ ToOBJ()
      {
        if (this.Models.Length == 0)
          return (OBJ) null;
        OBJ obj = new OBJ();
        obj.Faces = new List<OBJ.Face>();
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int index = 0;
        CGFX.DATA.CMDL model = this.Models[0];
        foreach (CGFX.DATA.CMDL.SeparateDataShapeCtr shape in model.Shapes)
        {
          Polygon vertexData = shape.GetVertexData(model);
          CGFX.DATA.CMDL.MTOB material = model.Materials[(IntPtr) model.Meshes[index].MaterialReference];
          foreach (CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr.IndexStreamCtr indexStream in shape.PrimitiveSets[0].Primitives[0].IndexStreams)
          {
            foreach (Vector3 vector3 in indexStream.GetFaceData())
            {
              OBJ.Face face = new OBJ.Face();
              face.MaterialName = material.Name;
              face.VertexIndieces = new List<int>();
              face.NormalIndieces = new List<int>();
              face.TexCoordIndieces = new List<int>();
              face.VertexColorIndieces = new List<int>();
              obj.Vertices.Add(vertexData.Vertex[(int) vector3.X]);
              obj.Vertices.Add(vertexData.Vertex[(int) vector3.Y]);
              obj.Vertices.Add(vertexData.Vertex[(int) vector3.Z]);
              face.VertexIndieces.Add(num1);
              face.VertexIndieces.Add(num1 + 1);
              face.VertexIndieces.Add(num1 + 2);
              num1 += 3;
              if (vertexData.Normals != null)
              {
                obj.Normals.Add(vertexData.Normals[(int) vector3.X]);
                obj.Normals.Add(vertexData.Normals[(int) vector3.Y]);
                obj.Normals.Add(vertexData.Normals[(int) vector3.Z]);
                face.NormalIndieces.Add(num2);
                face.NormalIndieces.Add(num2 + 1);
                face.NormalIndieces.Add(num2 + 2);
                num2 += 3;
              }
              obj.TexCoords.Add(Vector2.Multiply(Vector2.Multiply(vertexData.TexCoords[(int) vector3.X], new Vector2(1f, -1f)), material.Tex0Scale));
              obj.TexCoords.Add(Vector2.Multiply(Vector2.Multiply(vertexData.TexCoords[(int) vector3.Y], new Vector2(1f, -1f)), material.Tex0Scale));
              obj.TexCoords.Add(Vector2.Multiply(Vector2.Multiply(vertexData.TexCoords[(int) vector3.Z], new Vector2(1f, -1f)), material.Tex0Scale));
              face.TexCoordIndieces.Add(num3);
              face.TexCoordIndieces.Add(num3 + 1);
              face.TexCoordIndieces.Add(num3 + 2);
              num3 += 3;
              if (vertexData.Colors != null)
              {
                obj.VertexColors.Add(vertexData.Colors[(int) vector3.X]);
                obj.VertexColors.Add(vertexData.Colors[(int) vector3.Y]);
                obj.VertexColors.Add(vertexData.Colors[(int) vector3.Z]);
                face.VertexColorIndieces.Add(num4);
                face.VertexColorIndieces.Add(num4 + 1);
                face.VertexColorIndieces.Add(num4 + 2);
                num4 += 3;
              }
              obj.Faces.Add(face);
            }
          }
          ++index;
        }
        return obj;
      }

      public class DictionaryInfo
      {
        public uint NrItems;
        public uint Offset;

        public DictionaryInfo(EndianBinaryReader er)
        {
          this.NrItems = er.ReadUInt32();
          long position = er.BaseStream.Position;
          this.Offset = er.ReadUInt32();
          if (this.Offset == 0U)
            return;
          this.Offset += (uint) position;
        }
      }

      public class CMDL
      {
        public uint Flags;
        public string Signature;
        public uint Unknown1;
        public uint NameOffset;
        public uint Unknown2;
        public uint Unknown3;
        public uint Unknown4;
        public uint Unknown5;
        public uint Unknown6;
        public uint Unknown7;
        public uint NrAnimationTypes;
        public uint AnimationInfoDictOffset;
        public Vector3 PosScale;
        public uint Unknown9;
        public uint Unknown10;
        public uint Unknown11;
        public uint Unknown12;
        public uint Unknown13;
        public uint Unknown14;
        public float[] Matrix1;
        public float[] Matrix2;
        public uint NrVertexInfoSOBJ;
        public uint VertexInfoSOBJOffsetsOffset;
        public uint NrMaterials;
        public uint MaterialsDictOffset;
        public uint NrShapes;
        public uint ShapeOffsetsOffset;
        public uint Unknown21;
        public uint Unknown22;
        public uint Unknown23;
        public uint Unknown24;
        public uint Unknown25;
        public uint SkeletonInfoSOBJOffset;
        public uint[] VertexInfoSOBJOffsets;
        public uint[] VertexSOBJOffsets;
        public string Name;
        public CGFX.DICT AnimationInfoDict;
        public CGFX.DICT MaterialsDict;
        public CGFX.DATA.CMDL.GraphicsAnimationGroupDescription[] AnimationGroupDescriptions;
        public CGFX.DATA.CMDL.Mesh[] Meshes;
        public CGFX.DATA.CMDL.SeparateDataShapeCtr[] Shapes;
        public CGFX.DATA.CMDL.Skeleton_ Skeleton;
        public CGFX.DATA.CMDL.MTOB[] Materials;

        public CMDL(EndianBinaryReader er)
        {
          this.Flags = er.ReadUInt32();
          this.Signature = er.ReadString(Encoding.ASCII, 4);
          if (this.Signature != nameof (CMDL))
            throw new SignatureNotCorrectException(this.Signature, nameof (CMDL), er.BaseStream.Position);
          this.Unknown1 = er.ReadUInt32();
          this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.Unknown3 = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          this.Unknown5 = er.ReadUInt32();
          this.Unknown6 = er.ReadUInt32();
          this.Unknown7 = er.ReadUInt32();
          this.NrAnimationTypes = er.ReadUInt32();
          this.AnimationInfoDictOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.PosScale = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
          this.Unknown9 = er.ReadUInt32();
          this.Unknown10 = er.ReadUInt32();
          this.Unknown11 = er.ReadUInt32();
          this.Unknown12 = er.ReadUInt32();
          this.Unknown13 = er.ReadUInt32();
          this.Unknown14 = er.ReadUInt32();
          this.Matrix1 = er.ReadSingles(12);
          this.Matrix2 = er.ReadSingles(12);
          this.NrVertexInfoSOBJ = er.ReadUInt32();
          this.VertexInfoSOBJOffsetsOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.NrMaterials = er.ReadUInt32();
          this.MaterialsDictOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.NrShapes = er.ReadUInt32();
          this.ShapeOffsetsOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.Unknown21 = er.ReadUInt32();
          this.Unknown22 = er.ReadUInt32();
          this.Unknown23 = er.ReadUInt32();
          this.Unknown24 = er.ReadUInt32();
          this.Unknown25 = er.ReadUInt32();
          if (((int) this.Flags & 128) != 0)
            this.SkeletonInfoSOBJOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.NameOffset;
          this.Name = er.ReadStringNT(Encoding.ASCII);
          er.BaseStream.Position = (long) this.AnimationInfoDictOffset;
          this.AnimationInfoDict = new CGFX.DICT(er);
          er.BaseStream.Position = (long) this.VertexInfoSOBJOffsetsOffset;
          this.VertexInfoSOBJOffsets = new uint[(IntPtr) this.NrVertexInfoSOBJ];
          for (int index = 0; (long) index < (long) this.NrVertexInfoSOBJ; ++index)
            this.VertexInfoSOBJOffsets[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
          er.BaseStream.Position = (long) this.MaterialsDictOffset;
          this.MaterialsDict = new CGFX.DICT(er);
          this.Materials = new CGFX.DATA.CMDL.MTOB[(IntPtr) this.NrMaterials];
          for (int index = 0; (long) index < (long) this.NrMaterials; ++index)
          {
            er.BaseStream.Position = (long) this.MaterialsDict.Entries[index].DataOffset;
            this.Materials[index] = new CGFX.DATA.CMDL.MTOB(er);
          }
          er.BaseStream.Position = (long) this.ShapeOffsetsOffset;
          this.VertexSOBJOffsets = new uint[(IntPtr) this.NrShapes];
          for (int index = 0; (long) index < (long) this.NrShapes; ++index)
            this.VertexSOBJOffsets[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.AnimationGroupDescriptions = new CGFX.DATA.CMDL.GraphicsAnimationGroupDescription[(IntPtr) this.NrAnimationTypes];
          for (int index = 0; (long) index < (long) this.NrAnimationTypes; ++index)
          {
            er.BaseStream.Position = (long) this.AnimationInfoDict.Entries[index].DataOffset;
            this.AnimationGroupDescriptions[index] = new CGFX.DATA.CMDL.GraphicsAnimationGroupDescription(er);
          }
          this.Meshes = new CGFX.DATA.CMDL.Mesh[(IntPtr) this.NrVertexInfoSOBJ];
          for (int index = 0; (long) index < (long) this.NrVertexInfoSOBJ; ++index)
          {
            er.BaseStream.Position = (long) this.VertexInfoSOBJOffsets[index];
            this.Meshes[index] = new CGFX.DATA.CMDL.Mesh(er);
          }
          this.Shapes = new CGFX.DATA.CMDL.SeparateDataShapeCtr[(IntPtr) this.NrShapes];
          for (int index = 0; (long) index < (long) this.NrShapes; ++index)
          {
            er.BaseStream.Position = (long) this.VertexSOBJOffsets[index];
            this.Shapes[index] = new CGFX.DATA.CMDL.SeparateDataShapeCtr(er);
          }
          if (((int) this.Flags & 128) != 0)
          {
            er.BaseStream.Position = (long) this.SkeletonInfoSOBJOffset;
            this.Skeleton = new CGFX.DATA.CMDL.Skeleton_(er);
          }
          er.BaseStream.Position = position;
        }

        public override string ToString()
        {
          return this.Name;
        }

        public class GraphicsAnimationGroupDescription
        {
          public uint Unknown1;
          public uint Unknown2;
          public uint NameOffset;
          public uint Unknown3;
          public uint NrDictionaryElements;
          public uint DictionaryOffset;
          public uint NrUnknown;
          public uint Unknown4;
          public uint Unknown5;
          public uint[] Unknowns;
          public string Name;
          public CGFX.DICT Dictionary;

          public GraphicsAnimationGroupDescription(EndianBinaryReader er)
          {
            this.Unknown1 = er.ReadUInt32();
            this.Unknown2 = er.ReadUInt32();
            this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown3 = er.ReadUInt32();
            this.NrDictionaryElements = er.ReadUInt32();
            this.DictionaryOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.NrUnknown = er.ReadUInt32();
            this.Unknown4 = er.ReadUInt32();
            this.Unknown5 = er.ReadUInt32();
            this.Unknowns = er.ReadUInt32s((int) this.NrUnknown);
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.NameOffset;
            this.Name = er.ReadStringNT(Encoding.ASCII);
            er.BaseStream.Position = (long) this.DictionaryOffset;
            this.Dictionary = new CGFX.DICT(er);
            er.BaseStream.Position = position;
          }

          public override string ToString()
          {
            return this.Name;
          }
        }

        public class MTOB
        {
          public uint Flags;
          public string Signature;
          public uint Unknown1;
          public uint NameOffset;
          public uint Unknown2;
          public uint Unknown3;
          public uint Unknown4;
          public uint Unknown5;
          public uint Unknown6;
          public Color Emission_1;
          public Color Ambient_1;
          public Color Diffuse_1;
          public Color Specular0_1;
          public Color Specular1_1;
          public Color Constant0_1;
          public Color Constant1_1;
          public Color Constant2_1;
          public Color Constant3_1;
          public Color Constant4_1;
          public Color Constant5_1;
          public Color Emission_2;
          public Color Ambient_2;
          public Color Diffuse_2;
          public Color Specular0_2;
          public Color Specular1_2;
          public Color Constant0_2;
          public Color Constant1_2;
          public Color Constant2_2;
          public Color Constant3_2;
          public Color Constant4_2;
          public Color Constant5_2;
          public uint Unknown18;
          public uint Unknown19;
          public uint Unknown20;
          public uint Unknown21;
          public uint Unknown22;
          public ushort Unknown23;
          public ushort Unknown24;
          public uint Unknown25;
          public uint Unknown26;
          public uint Unknown27;
          public uint Unknown28;
          public uint Unknown29;
          public uint Unknown30;
          public uint Unknown31;
          public uint Unknown32;
          public uint Unknown33;
          public float Unknown34;
          public uint Unknown35;
          public uint Unknown36;
          public uint Unknown37;
          public uint Unknown38;
          public uint Unknown39;
          public uint Unknown40;
          public uint Unknown41;
          public uint Unknown42;
          public uint Unknown43;
          public uint Unknown44;
          public uint Unknown45;
          public uint Tex0Coord;
          public uint Unknown47;
          public uint Unknown48;
          public uint Unknown49;
          public Vector2 Tex0Scale;
          public Vector3 Tex0Rotation;
          public uint Unknown55;
          public float[] Tex0Matrix;
          public uint Tex1Coord;
          public uint Unknown69;
          public uint Unknown70;
          public uint Unknown71;
          public Vector2 Tex1Scale;
          public Vector3 Tex1Rotation;
          public uint Unknown77;
          public float[] Tex1Matrix;
          public uint Tex2Coord;
          public uint Unknown91;
          public uint Unknown92;
          public uint Unknown93;
          public Vector2 Tex2Scale;
          public Vector3 Tex2Rotation;
          public uint Unknown99;
          public float[] Tex2Matrix;
          public uint Tex0Offset;
          public uint Tex1Offset;
          public uint Tex2Offset;
          public uint Tex3Offset;
          public uint ShaderOffset;
          public uint Unknown105;
          public uint Unknown106;
          public uint Unknown107;
          public uint Unknown108;
          public uint Unknown109;
          public uint Unknown110;
          public string Name;
          public CGFX.DATA.CMDL.MTOB.TexInfo Tex0;
          public CGFX.DATA.CMDL.MTOB.TexInfo Tex1;
          public CGFX.DATA.CMDL.MTOB.TexInfo Tex2;
          public CGFX.DATA.CMDL.MTOB.TexInfo Tex3;
          public CGFX.DATA.CMDL.MTOB.SHDR FragmentShader;

          public MTOB(EndianBinaryReader er)
          {
            this.Flags = er.ReadUInt32();
            this.Signature = er.ReadString(Encoding.ASCII, 4);
            if (this.Signature != nameof (MTOB))
              throw new SignatureNotCorrectException(this.Signature, nameof (MTOB), er.BaseStream.Position);
            this.Unknown1 = er.ReadUInt32();
            this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown2 = er.ReadUInt32();
            this.Unknown3 = er.ReadUInt32();
            this.Unknown4 = er.ReadUInt32();
            this.Unknown5 = er.ReadUInt32();
            this.Unknown6 = er.ReadUInt32();
            this.Emission_1 = er.ReadColor4Singles();
            this.Ambient_1 = er.ReadColor4Singles();
            this.Diffuse_1 = er.ReadColor4Singles();
            this.Specular0_1 = er.ReadColor4Singles();
            this.Specular1_1 = er.ReadColor4Singles();
            this.Constant0_1 = er.ReadColor4Singles();
            this.Constant1_1 = er.ReadColor4Singles();
            this.Constant2_1 = er.ReadColor4Singles();
            this.Constant3_1 = er.ReadColor4Singles();
            this.Constant4_1 = er.ReadColor4Singles();
            this.Constant5_1 = er.ReadColor4Singles();
            this.Emission_2 = er.ReadColor8();
            this.Ambient_2 = er.ReadColor8();
            this.Diffuse_2 = er.ReadColor8();
            this.Specular0_2 = er.ReadColor8();
            this.Specular1_2 = er.ReadColor8();
            this.Constant0_2 = er.ReadColor8();
            this.Constant1_2 = er.ReadColor8();
            this.Constant2_2 = er.ReadColor8();
            this.Constant3_2 = er.ReadColor8();
            this.Constant4_2 = er.ReadColor8();
            this.Constant5_2 = er.ReadColor8();
            this.Unknown18 = er.ReadUInt32();
            this.Unknown19 = er.ReadUInt32();
            this.Unknown20 = er.ReadUInt32();
            this.Unknown21 = er.ReadUInt32();
            this.Unknown22 = er.ReadUInt32();
            this.Unknown23 = er.ReadUInt16();
            this.Unknown24 = er.ReadUInt16();
            this.Unknown25 = er.ReadUInt32();
            this.Unknown26 = er.ReadUInt32();
            this.Unknown27 = er.ReadUInt32();
            this.Unknown28 = er.ReadUInt32();
            this.Unknown29 = er.ReadUInt32();
            this.Unknown30 = er.ReadUInt32();
            this.Unknown31 = er.ReadUInt32();
            this.Unknown32 = er.ReadUInt32();
            this.Unknown33 = er.ReadUInt32();
            this.Unknown34 = er.ReadSingle();
            this.Unknown35 = er.ReadUInt32();
            this.Unknown36 = er.ReadUInt32();
            this.Unknown37 = er.ReadUInt32();
            this.Unknown38 = er.ReadUInt32();
            this.Unknown39 = er.ReadUInt32();
            this.Unknown40 = er.ReadUInt32();
            this.Unknown41 = er.ReadUInt32();
            this.Unknown42 = er.ReadUInt32();
            this.Unknown43 = er.ReadUInt32();
            this.Unknown44 = er.ReadUInt32();
            this.Unknown45 = er.ReadUInt32();
            this.Tex0Coord = er.ReadUInt32();
            this.Unknown47 = er.ReadUInt32();
            this.Unknown48 = er.ReadUInt32();
            this.Unknown49 = er.ReadUInt32();
            this.Tex0Scale = new Vector2(er.ReadSingle(), er.ReadSingle());
            this.Tex0Rotation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
            this.Unknown55 = er.ReadUInt32();
            this.Tex0Matrix = er.ReadSingles(12);
            this.Tex1Coord = er.ReadUInt32();
            this.Unknown69 = er.ReadUInt32();
            this.Unknown70 = er.ReadUInt32();
            this.Unknown71 = er.ReadUInt32();
            this.Tex1Scale = new Vector2(er.ReadSingle(), er.ReadSingle());
            this.Tex1Rotation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
            this.Unknown77 = er.ReadUInt32();
            this.Tex1Matrix = er.ReadSingles(12);
            this.Tex2Coord = er.ReadUInt32();
            this.Unknown91 = er.ReadUInt32();
            this.Unknown92 = er.ReadUInt32();
            this.Unknown93 = er.ReadUInt32();
            this.Tex2Scale = new Vector2(er.ReadSingle(), er.ReadSingle());
            this.Tex2Rotation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
            this.Unknown99 = er.ReadUInt32();
            this.Tex2Matrix = er.ReadSingles(12);
            this.Tex0Offset = er.ReadUInt32();
            if (this.Tex0Offset != 0U)
              this.Tex0Offset += (uint) er.BaseStream.Position - 4U;
            this.Tex1Offset = er.ReadUInt32();
            if (this.Tex1Offset != 0U)
              this.Tex1Offset += (uint) er.BaseStream.Position - 4U;
            this.Tex2Offset = er.ReadUInt32();
            if (this.Tex2Offset != 0U)
              this.Tex2Offset += (uint) er.BaseStream.Position - 4U;
            this.Tex3Offset = er.ReadUInt32();
            if (this.Tex3Offset != 0U)
              this.Tex3Offset += (uint) er.BaseStream.Position - 4U;
            this.ShaderOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown105 = er.ReadUInt32();
            this.Unknown106 = er.ReadUInt32();
            this.Unknown107 = er.ReadUInt32();
            this.Unknown108 = er.ReadUInt32();
            this.Unknown109 = er.ReadUInt32();
            this.Unknown110 = er.ReadUInt32();
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.NameOffset;
            this.Name = er.ReadStringNT(Encoding.ASCII);
            if (this.Tex0Offset != 0U)
            {
              er.BaseStream.Position = (long) this.Tex0Offset;
              this.Tex0 = new CGFX.DATA.CMDL.MTOB.TexInfo(er);
            }
            if (this.Tex1Offset != 0U)
            {
              er.BaseStream.Position = (long) this.Tex1Offset;
              this.Tex1 = new CGFX.DATA.CMDL.MTOB.TexInfo(er);
            }
            if (this.Tex2Offset != 0U)
            {
              er.BaseStream.Position = (long) this.Tex2Offset;
              this.Tex2 = new CGFX.DATA.CMDL.MTOB.TexInfo(er);
            }
            if (this.Tex3Offset != 0U)
            {
              er.BaseStream.Position = (long) this.Tex3Offset;
              this.Tex3 = new CGFX.DATA.CMDL.MTOB.TexInfo(er);
            }
            er.BaseStream.Position = (long) this.ShaderOffset;
            this.FragmentShader = new CGFX.DATA.CMDL.MTOB.SHDR(er);
            er.BaseStream.Position = position;
          }

          public override string ToString()
          {
            return this.Name;
          }

          public class TextureCoordinatorCtr
          {
            public uint SourceCoordinate;
            public uint Unknown1;
            public int ReferenceCamera;
            public uint Unknown2;
            public Vector2 Scale;
            public float Rotate;
            public Vector2 Translate;
            public uint Unknown3;
            public float[] Matrix;
          }

          public class TexInfo
          {
            public uint Unknown1;
            public uint Unknown2;
            public uint TXOBOffset;
            public uint Unknown3;
            public uint Unknown4;
            public ushort Unknown5;
            public ushort Unknown6;
            public uint Unknown7;
            public ushort Unknown8;
            public ushort Unknown9;
            public ushort Height;
            public ushort Width;
            public uint Unknown12;
            public uint Unknown13;
            public uint Unknown14;
            public uint Unknown15;
            public uint Unknown16;
            public uint Unknown17;
            public uint Unknown18;
            public uint Unknown19;
            public uint Unknown20;
            public uint Unknown21;
            public CGFX.DATA.CMDL.MTOB.TexInfo.TXOB TextureObject;

            public TexInfo(EndianBinaryReader er)
            {
              this.Unknown1 = er.ReadUInt32();
              this.Unknown2 = er.ReadUInt32();
              this.TXOBOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Unknown3 = er.ReadUInt32();
              this.Unknown4 = er.ReadUInt32();
              this.Unknown5 = er.ReadUInt16();
              this.Unknown6 = er.ReadUInt16();
              this.Unknown7 = er.ReadUInt32();
              this.Unknown8 = er.ReadUInt16();
              this.Unknown9 = er.ReadUInt16();
              this.Height = er.ReadUInt16();
              this.Width = er.ReadUInt16();
              this.Unknown12 = er.ReadUInt32();
              this.Unknown13 = er.ReadUInt32();
              this.Unknown14 = er.ReadUInt32();
              this.Unknown15 = er.ReadUInt32();
              this.Unknown16 = er.ReadUInt32();
              this.Unknown17 = er.ReadUInt32();
              this.Unknown18 = er.ReadUInt32();
              this.Unknown19 = er.ReadUInt32();
              this.Unknown20 = er.ReadUInt32();
              this.Unknown21 = er.ReadUInt32();
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.TXOBOffset;
              this.TextureObject = new CGFX.DATA.CMDL.MTOB.TexInfo.TXOB(er);
              er.BaseStream.Position = position;
            }

            public class TXOB
            {
              public uint Flags;
              public string Signature;
              public uint Unknown1;
              public uint NameOffset;
              public uint Unknown2;
              public uint Unknown3;
              public uint LinkedTextureNameOffset;
              public uint LinkedTextureOffset;
              public uint Unknown6;
              public uint Unknown7;
              public uint Unknown8;
              public uint Unknown9;
              public uint Unknown10;
              public uint Unknown11;
              public float Unknown12;
              public float Unknown13;
              public string Name;
              public string LinkedTextureName;

              public TXOB(EndianBinaryReader er)
              {
                this.Flags = er.ReadUInt32();
                this.Signature = er.ReadString(Encoding.ASCII, 4);
                if (this.Signature != nameof (TXOB))
                  throw new SignatureNotCorrectException(this.Signature, nameof (TXOB), er.BaseStream.Position);
                this.Unknown1 = er.ReadUInt32();
                this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                this.Unknown2 = er.ReadUInt32();
                this.Unknown3 = er.ReadUInt32();
                this.LinkedTextureNameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                this.LinkedTextureOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                this.Unknown6 = er.ReadUInt32();
                this.Unknown7 = er.ReadUInt32();
                this.Unknown8 = er.ReadUInt32();
                this.Unknown9 = er.ReadUInt32();
                this.Unknown10 = er.ReadUInt32();
                this.Unknown11 = er.ReadUInt32();
                this.Unknown12 = er.ReadSingle();
                this.Unknown13 = er.ReadSingle();
                long position = er.BaseStream.Position;
                er.BaseStream.Position = (long) this.NameOffset;
                this.Name = er.ReadStringNT(Encoding.ASCII);
                er.BaseStream.Position = (long) this.LinkedTextureNameOffset;
                this.LinkedTextureName = er.ReadStringNT(Encoding.ASCII);
                er.BaseStream.Position = position;
              }

              public override string ToString()
              {
                return this.Name;
              }
            }
          }

          public class SHDR
          {
            public uint Flags;
            public string Signature;
            public uint Unknown1;
            public uint NameOffset;
            public uint Unknown2;
            public uint Unknown3;
            public uint LinkedShaderNameOffset;
            public uint Unknown4;
            public Color BufferColor_1;
            public uint Unknown5;
            public uint Unknown6;
            public uint Unknown7;
            public uint Unknown8;
            public uint Unknown9;
            public uint Unknown10;
            public uint FragmentLightingTableOffset;
            public uint Unknown12;
            public CGFX.DATA.CMDL.MTOB.SHDR.TextureCombinerCtr[] TextureCombiners;
            public uint Unknown25;
            public Color BufferColor_2;
            public ushort[] Unknown26;
            public uint Distribution0SamplerOffset;
            public uint Distribution1SamplerOffset;
            public uint FresnelSamplerOffset;
            public uint ReflectanceBSamplerOffset;
            public uint ReflectanceGSamplerOffset;
            public uint ReflectanceRSamplerOffset;
            public string Name;
            public string LinkedShaderName;
            public CGFX.DATA.CMDL.MTOB.SHDR.TableSetInfoEntry TableSetInfoEntry1;
            public CGFX.DATA.CMDL.MTOB.SHDR.TableSetInfoEntry TableSetInfoEntry2;
            public CGFX.DATA.CMDL.MTOB.SHDR.TableSetInfoEntry TableSetInfoEntry3;

            public SHDR(EndianBinaryReader er)
            {
              this.Flags = er.ReadUInt32();
              this.Signature = er.ReadString(Encoding.ASCII, 4);
              if (this.Signature != nameof (SHDR))
                throw new SignatureNotCorrectException(this.Signature, nameof (SHDR), er.BaseStream.Position);
              this.Unknown1 = er.ReadUInt32();
              this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Unknown2 = er.ReadUInt32();
              this.Unknown3 = er.ReadUInt32();
              this.LinkedShaderNameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Unknown4 = er.ReadUInt32();
              this.BufferColor_1 = er.ReadColor4Singles();
              this.Unknown5 = er.ReadUInt32();
              this.Unknown6 = er.ReadUInt32();
              this.Unknown7 = er.ReadUInt32();
              this.Unknown8 = er.ReadUInt32();
              this.Unknown9 = er.ReadUInt32();
              this.Unknown10 = er.ReadUInt32();
              this.FragmentLightingTableOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Unknown12 = er.ReadUInt32();
              this.TextureCombiners = new CGFX.DATA.CMDL.MTOB.SHDR.TextureCombinerCtr[6];
              for (int index = 0; index < 6; ++index)
                this.TextureCombiners[index] = new CGFX.DATA.CMDL.MTOB.SHDR.TextureCombinerCtr(er);
              this.Unknown25 = er.ReadUInt32();
              this.BufferColor_2 = er.ReadColor8();
              this.Unknown26 = er.ReadUInt16s(10);
              this.Distribution0SamplerOffset = er.ReadUInt32();
              if (this.Distribution0SamplerOffset != 0U)
                this.Distribution0SamplerOffset += (uint) er.BaseStream.Position - 4U;
              this.Distribution1SamplerOffset = er.ReadUInt32();
              if (this.Distribution1SamplerOffset != 0U)
                this.Distribution1SamplerOffset += (uint) er.BaseStream.Position - 4U;
              this.FresnelSamplerOffset = er.ReadUInt32();
              if (this.FresnelSamplerOffset != 0U)
                this.FresnelSamplerOffset += (uint) er.BaseStream.Position - 4U;
              this.ReflectanceBSamplerOffset = er.ReadUInt32();
              if (this.ReflectanceBSamplerOffset != 0U)
                this.ReflectanceBSamplerOffset += (uint) er.BaseStream.Position - 4U;
              this.ReflectanceGSamplerOffset = er.ReadUInt32();
              if (this.ReflectanceGSamplerOffset != 0U)
                this.ReflectanceGSamplerOffset += (uint) er.BaseStream.Position - 4U;
              this.ReflectanceRSamplerOffset = er.ReadUInt32();
              if (this.ReflectanceRSamplerOffset != 0U)
                this.ReflectanceRSamplerOffset += (uint) er.BaseStream.Position - 4U;
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.NameOffset;
              this.Name = er.ReadStringNT(Encoding.ASCII);
              er.BaseStream.Position = (long) this.LinkedShaderNameOffset;
              this.LinkedShaderName = er.ReadStringNT(Encoding.ASCII);
              if (this.ReflectanceBSamplerOffset != 0U)
              {
                er.BaseStream.Position = (long) this.ReflectanceBSamplerOffset;
                this.TableSetInfoEntry1 = new CGFX.DATA.CMDL.MTOB.SHDR.TableSetInfoEntry(er);
              }
              if (this.ReflectanceGSamplerOffset != 0U)
              {
                er.BaseStream.Position = (long) this.ReflectanceGSamplerOffset;
                this.TableSetInfoEntry2 = new CGFX.DATA.CMDL.MTOB.SHDR.TableSetInfoEntry(er);
              }
              if (this.ReflectanceRSamplerOffset != 0U)
              {
                er.BaseStream.Position = (long) this.ReflectanceRSamplerOffset;
                this.TableSetInfoEntry3 = new CGFX.DATA.CMDL.MTOB.SHDR.TableSetInfoEntry(er);
              }
              er.BaseStream.Position = position;
            }

            public override string ToString()
            {
              return this.Name;
            }

            public class TextureCombinerCtr
            {
              public ushort SrcRgb;
              public ushort SrcAlpha;
              public uint Address;
              public uint Operands;
              public ushort CombineRgb;
              public ushort CombineAlpha;
              public Color ConstRgba;
              public ushort Unknown2Rgb;
              public ushort Unknown2Alpha;
              public uint Unknown3;

              public TextureCombinerCtr(EndianBinaryReader er)
              {
                this.SrcRgb = er.ReadUInt16();
                this.SrcAlpha = er.ReadUInt16();
                this.Address = er.ReadUInt32();
                this.Operands = er.ReadUInt32();
                this.CombineRgb = er.ReadUInt16();
                this.CombineAlpha = er.ReadUInt16();
                this.ConstRgba = er.ReadColor8();
                this.Unknown2Rgb = er.ReadUInt16();
                this.Unknown2Alpha = er.ReadUInt16();
                this.Unknown3 = er.ReadUInt32();
              }
            }

            public class TableSetInfoEntry
            {
              public uint Unknown1;
              public uint Unknown2;
              public uint Unknown3;
              public uint Unknown4;
              public uint LinkedTableSetNameOffset;
              public uint LinkedTableSetEntryNameOffset;
              public uint Unknown5;
              public string LinkedTableSetName;
              public string LinkedTableSetEntryName;

              public TableSetInfoEntry(EndianBinaryReader er)
              {
                this.Unknown1 = er.ReadUInt32();
                this.Unknown2 = er.ReadUInt32();
                this.Unknown3 = er.ReadUInt32();
                this.Unknown4 = er.ReadUInt32();
                this.LinkedTableSetNameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                this.LinkedTableSetEntryNameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                this.Unknown5 = er.ReadUInt32();
                long position = er.BaseStream.Position;
                er.BaseStream.Position = (long) this.LinkedTableSetNameOffset;
                this.LinkedTableSetName = er.ReadStringNT(Encoding.ASCII);
                er.BaseStream.Position = (long) this.LinkedTableSetEntryNameOffset;
                this.LinkedTableSetEntryName = er.ReadStringNT(Encoding.ASCII);
                er.BaseStream.Position = position;
              }
            }
          }
        }

        public class Mesh
        {
          public uint Flags;
          public string Signature;
          public uint Unknown1;
          public uint NameOffset;
          public uint Unknown2;
          public uint Unknown3;
          public uint SeparateShapeReference;
          public uint MaterialReference;
          public uint Unknown6;
          public uint Unknown7;
          public uint Unknown8;
          public uint Unknown9;
          public uint Unknown10;
          public uint Unknown11;
          public float Unknown12;
          public uint Unknown13;
          public uint Unknown14;
          public float Unknown15;
          public float Unknown16;
          public uint Unknown17;
          public uint Unknown18;
          public uint Unknown19;
          public float Unknown20;
          public uint Unknown21;
          public uint Unknown22;
          public uint Unknown23;
          public uint Unknown24;
          public uint Unknown25;
          public uint Unknown26;
          public uint Unknown27;
          public uint Unknown28;
          public uint Unknown29;
          public uint Unknown30;
          public uint Unknown31;
          public string Name;

          public Mesh(EndianBinaryReader er)
          {
            this.Flags = er.ReadUInt32();
            this.Signature = er.ReadString(Encoding.ASCII, 4);
            if (this.Signature != "SOBJ")
              throw new SignatureNotCorrectException(this.Signature, "SOBJ", er.BaseStream.Position);
            this.Unknown1 = er.ReadUInt32();
            this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown2 = er.ReadUInt32();
            this.Unknown3 = er.ReadUInt32();
            this.SeparateShapeReference = er.ReadUInt32();
            this.MaterialReference = er.ReadUInt32();
            this.Unknown6 = er.ReadUInt32();
            this.Unknown7 = er.ReadUInt32();
            this.Unknown8 = er.ReadUInt32();
            this.Unknown9 = er.ReadUInt32();
            this.Unknown10 = er.ReadUInt32();
            this.Unknown11 = er.ReadUInt32();
            this.Unknown12 = er.ReadSingle();
            this.Unknown13 = er.ReadUInt32();
            this.Unknown14 = er.ReadUInt32();
            this.Unknown15 = er.ReadSingle();
            this.Unknown16 = er.ReadSingle();
            this.Unknown17 = er.ReadUInt32();
            this.Unknown18 = er.ReadUInt32();
            this.Unknown19 = er.ReadUInt32();
            this.Unknown20 = er.ReadSingle();
            this.Unknown21 = er.ReadUInt32();
            this.Unknown22 = er.ReadUInt32();
            this.Unknown23 = er.ReadUInt32();
            this.Unknown24 = er.ReadUInt32();
            this.Unknown25 = er.ReadUInt32();
            this.Unknown26 = er.ReadUInt32();
            this.Unknown27 = er.ReadUInt32();
            this.Unknown28 = er.ReadUInt32();
            this.Unknown29 = er.ReadUInt32();
            this.Unknown30 = er.ReadUInt32();
            this.Unknown31 = er.ReadUInt32();
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.NameOffset;
            this.Name = er.ReadStringNT(Encoding.ASCII);
            er.BaseStream.Position = position;
          }

          public override string ToString()
          {
            return this.Name;
          }
        }

        public class SeparateDataShapeCtr
        {
          public uint Flags;
          public string Signature;
          public uint Unknown1;
          public uint NameOffset;
          public uint Unknown2;
          public uint Unknown3;
          public uint Unknown4;
          public uint MatrixOffset;
          public uint Unknown6;
          public uint Unknown7;
          public uint Unknown8;
          public uint NrPrimitiveSets;
          public uint PrimitiveSetOffsetsArrayOffset;
          public uint Unknown11;
          public uint NrVertexAttributes;
          public uint VertexAttributeOffsetsArrayOffset;
          public uint UnknownOffset1;
          public uint[] PrimitiveSetOffsets;
          public uint[] VertexAttributeOffsets;
          public string Name;
          public CGFX.DATA.CMDL.SeparateDataShapeCtr.MatrixData Matrix;
          public CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr[] PrimitiveSets;
          public CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_[] VertexAttributes;

          public SeparateDataShapeCtr(EndianBinaryReader er)
          {
            this.Flags = er.ReadUInt32();
            this.Signature = er.ReadString(Encoding.ASCII, 4);
            if (this.Signature != "SOBJ")
              throw new SignatureNotCorrectException(this.Signature, "SOBJ", er.BaseStream.Position);
            this.Unknown1 = er.ReadUInt32();
            this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown2 = er.ReadUInt32();
            this.Unknown3 = er.ReadUInt32();
            this.Unknown4 = er.ReadUInt32();
            this.MatrixOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown6 = er.ReadUInt32();
            this.Unknown7 = er.ReadUInt32();
            this.Unknown8 = er.ReadUInt32();
            this.NrPrimitiveSets = er.ReadUInt32();
            this.PrimitiveSetOffsetsArrayOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown11 = er.ReadUInt32();
            this.NrVertexAttributes = er.ReadUInt32();
            this.VertexAttributeOffsetsArrayOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.UnknownOffset1 = (uint) er.BaseStream.Position + er.ReadUInt32();
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.NameOffset;
            this.Name = er.ReadStringNT(Encoding.ASCII);
            er.BaseStream.Position = (long) this.MatrixOffset;
            this.Matrix = new CGFX.DATA.CMDL.SeparateDataShapeCtr.MatrixData(er);
            er.BaseStream.Position = (long) this.PrimitiveSetOffsetsArrayOffset;
            this.PrimitiveSetOffsets = new uint[(IntPtr) this.NrPrimitiveSets];
            for (int index = 0; (long) index < (long) this.NrPrimitiveSets; ++index)
              this.PrimitiveSetOffsets[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
            er.BaseStream.Position = (long) this.VertexAttributeOffsetsArrayOffset;
            this.VertexAttributeOffsets = new uint[(IntPtr) this.NrVertexAttributes];
            for (int index = 0; (long) index < (long) this.NrVertexAttributes; ++index)
              this.VertexAttributeOffsets[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.PrimitiveSets = new CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr[(IntPtr) this.NrPrimitiveSets];
            for (int index = 0; (long) index < (long) this.NrPrimitiveSets; ++index)
            {
              er.BaseStream.Position = (long) this.PrimitiveSetOffsets[index];
              this.PrimitiveSets[index] = new CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr(er);
            }
            this.VertexAttributes = new CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_[(IntPtr) this.NrVertexAttributes];
            for (int index = 0; index < 1; ++index)
            {
              er.BaseStream.Position = (long) this.VertexAttributeOffsets[index];
              this.VertexAttributes[index] = new CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_(er);
            }
            er.BaseStream.Position = position;
          }

          public Polygon GetVertexData(CGFX.DATA.CMDL Model)
          {
            Polygon polygon = new Polygon();
            int length = (int) (this.VertexAttributes[0].VertexDataLength / this.VertexAttributes[0].VertexDataEntrySize);
            int num1 = 0;
            byte[] vertexData = this.VertexAttributes[0].VertexData;
            foreach (CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_.VertexStreamCtr vertexStream in this.VertexAttributes[0].VertexStreams)
            {
              switch (vertexStream.Type)
              {
                case 0:
                  polygon.Vertex = new Vector3[length];
                  break;
                case 1:
                  polygon.Normals = new Vector3[length];
                  break;
                case 3:
                  polygon.Colors = new Color[length];
                  break;
                case 4:
                  polygon.TexCoords = new Vector2[length];
                  break;
                case 5:
                  polygon.TexCoords2 = new Vector2[length];
                  break;
                case 6:
                  polygon.TexCoords3 = new Vector2[length];
                  break;
              }
            }
            for (int index1 = 0; index1 < length; ++index1)
            {
              byte[] numArray1 = new byte[0];
              foreach (CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_.VertexStreamCtr vertexStream in this.VertexAttributes[0].VertexStreams)
              {
                switch (vertexStream.Type)
                {
                  case 0:
                    polygon.Vertex[index1] = new Vector3(BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 4), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 8));
                    break;
                  case 1:
                    polygon.Normals[index1] = new Vector3(BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 4), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 8));
                    break;
                  case 2:
                    Vector3 vector3 = new Vector3(BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 4), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 8));
                    break;
                  case 3:
                    uint num2 = Bytes.Read4BytesAsUInt32(vertexData, num1 + (int) vertexStream.Offset);
                    polygon.Colors[index1] = Math.Round(1.0 / (double) vertexStream.Unknown9) != (double) sbyte.MaxValue ? Color.FromArgb((int) (num2 >> 24) & (int) byte.MaxValue, (int) num2 & (int) byte.MaxValue, (int) (num2 >> 8) & (int) byte.MaxValue, (int) (num2 >> 16) & (int) byte.MaxValue) : Color.FromArgb((int) ((double) (num2 >> 24 & (uint) sbyte.MaxValue) / (double) sbyte.MaxValue * (double) byte.MaxValue), (int) ((double) (num2 & (uint) sbyte.MaxValue) / (double) sbyte.MaxValue * (double) byte.MaxValue), (int) ((double) (num2 >> 8 & (uint) sbyte.MaxValue) / (double) sbyte.MaxValue * (double) byte.MaxValue), (int) ((double) (num2 >> 16 & (uint) sbyte.MaxValue) / (double) sbyte.MaxValue * (double) byte.MaxValue));
                    break;
                  case 4:
                    polygon.TexCoords[index1] = new Vector2(BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 4));
                    break;
                  case 5:
                    polygon.TexCoords2[index1] = new Vector2(BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 4));
                    break;
                  case 6:
                    polygon.TexCoords3[index1] = new Vector2(BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset), BitConverter.ToSingle(vertexData, num1 + (int) vertexStream.Offset + 4));
                    break;
                  case 7:
                    if (vertexStream.NrComponents == 1U)
                    {
                      byte num3 = vertexData[(long) num1 + (long) vertexStream.Offset];
                      polygon.Vertex[index1] = Vector3.Transform(polygon.Vertex[index1], Model.Skeleton.GetMatrix((int) this.PrimitiveSets[0].RelatedBones[(int) num3]));
                      break;
                    }
                    if (vertexStream.NrComponents == 2U)
                    {
                      numArray1 = new byte[(IntPtr) vertexStream.NrComponents];
                      for (int index2 = 0; (long) index2 < (long) vertexStream.NrComponents; ++index2)
                        numArray1[index2] = vertexData[(long) num1 + (long) vertexStream.Offset + (long) index2];
                      break;
                    }
                    break;
                  case 8:
                    if (numArray1.Length != 0)
                    {
                      byte[] numArray2 = new byte[(IntPtr) vertexStream.NrComponents];
                      for (int index2 = 0; (long) index2 < (long) vertexStream.NrComponents; ++index2)
                        numArray2[index2] = vertexData[(long) num1 + (long) vertexStream.Offset + (long) index2];
                      Matrix4 matrix4 = new Matrix4();
                      for (int index2 = 0; (long) index2 < (long) vertexStream.NrComponents; ++index2)
                        matrix4 = this.Add4(matrix4, this.Mult4(Model.Skeleton.GetMatrix((int) this.PrimitiveSets[0].RelatedBones[(int) numArray1[index2]]), (float) numArray2[index2] * vertexStream.Unknown9));
                      polygon.Vertex[index1] = Vector3.Transform(polygon.Vertex[index1], matrix4);
                      break;
                    }
                    break;
                }
              }
              num1 += (int) this.VertexAttributes[0].VertexDataEntrySize;
            }
            return polygon;
          }

          private Matrix4 Mult4(Matrix4 a, float b)
          {
            return new Matrix4(a.Row0 * b, a.Row1 * b, a.Row2 * b, a.Row3 * b);
          }

          private Matrix4 Add4(Matrix4 a, Matrix4 b)
          {
            return new Matrix4(a.Row0 + b.Row0, a.Row1 + b.Row1, a.Row2 + b.Row2, a.Row3 + b.Row3);
          }

          public override string ToString()
          {
            return this.Name;
          }

          public class MatrixData
          {
            public uint Unknown1;
            public Vector3 CenterPosition;
            public float[] OrientationMatrix;
            public Vector3 Size;

            public MatrixData(EndianBinaryReader er)
            {
              this.Unknown1 = er.ReadUInt32();
              this.CenterPosition = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
              this.OrientationMatrix = er.ReadSingles(9);
              this.Size = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
            }
          }

          public class PrimitiveSetCtr
          {
            public uint NrRelatedBones;
            public uint RelatedBonesArrayOffset;
            public uint Unknown1;
            public uint NrPrimitives;
            public uint PrimitiveOffsetsArrayOffset;
            public uint[] RelatedBones;
            public uint[] PrimitiveOffsets;
            public CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr[] Primitives;

            public PrimitiveSetCtr(EndianBinaryReader er)
            {
              this.NrRelatedBones = er.ReadUInt32();
              this.RelatedBonesArrayOffset = er.ReadUInt32();
              if (this.RelatedBonesArrayOffset != 0U)
                this.RelatedBonesArrayOffset += (uint) er.BaseStream.Position - 4U;
              this.Unknown1 = er.ReadUInt32();
              this.NrPrimitives = er.ReadUInt32();
              this.PrimitiveOffsetsArrayOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              long position = er.BaseStream.Position;
              if (this.RelatedBonesArrayOffset != 0U)
              {
                er.BaseStream.Position = (long) this.RelatedBonesArrayOffset;
                this.RelatedBones = er.ReadUInt32s((int) this.NrRelatedBones);
              }
              er.BaseStream.Position = (long) this.PrimitiveOffsetsArrayOffset;
              this.PrimitiveOffsets = new uint[(IntPtr) this.NrPrimitives];
              for (int index = 0; (long) index < (long) this.NrPrimitives; ++index)
                this.PrimitiveOffsets[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Primitives = new CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr[(IntPtr) this.NrPrimitives];
              for (int index = 0; (long) index < (long) this.NrPrimitives; ++index)
              {
                er.BaseStream.Position = (long) this.PrimitiveOffsets[index];
                this.Primitives[index] = new CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr(er);
              }
            }

            public class PrimitiveCtr
            {
              public uint NrFaceInfos;
              public uint FaceInfoOffsetsArrayOffset;
              public uint NrUnknown2;
              public uint Unknown2ArrayOffset;
              public uint[] FaceInfoOffsets;
              public uint[] Unknown2s;
              public CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr.IndexStreamCtr[] IndexStreams;

              public PrimitiveCtr(EndianBinaryReader er)
              {
                this.NrFaceInfos = er.ReadUInt32();
                this.FaceInfoOffsetsArrayOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                this.NrUnknown2 = er.ReadUInt32();
                this.Unknown2ArrayOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                long position = er.BaseStream.Position;
                er.BaseStream.Position = (long) this.FaceInfoOffsetsArrayOffset;
                this.FaceInfoOffsets = new uint[(IntPtr) this.NrFaceInfos];
                for (int index = 0; (long) index < (long) this.NrFaceInfos; ++index)
                  this.FaceInfoOffsets[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
                er.BaseStream.Position = (long) this.Unknown2ArrayOffset;
                this.Unknown2s = er.ReadUInt32s((int) this.NrUnknown2);
                this.IndexStreams = new CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr.IndexStreamCtr[(IntPtr) this.NrFaceInfos];
                for (int index = 0; (long) index < (long) this.NrFaceInfos; ++index)
                {
                  er.BaseStream.Position = (long) this.FaceInfoOffsets[index];
                  this.IndexStreams[index] = new CGFX.DATA.CMDL.SeparateDataShapeCtr.PrimitiveSetCtr.PrimitiveCtr.IndexStreamCtr(er);
                }
              }

              public class IndexStreamCtr
              {
                public byte FaceDataFormat;
                public ushort Unknown14;
                public byte Unknown15;
                public uint Unknown16;
                public uint FaceDataLength;
                public uint FaceDataOffset;
                public uint Unknown17;
                public uint Unknown18;
                public uint Unknown19;
                public uint Unknown20;
                public uint Unknown21;
                public uint Unknown22;
                public uint Unknown23;
                public CGFX.DATA.CMDL.SeparateDataShapeCtr.MatrixData Matrix;
                public byte[] FaceData;

                public IndexStreamCtr(EndianBinaryReader er)
                {
                  this.FaceDataFormat = er.ReadByte();
                  this.Unknown14 = er.ReadUInt16();
                  this.Unknown15 = er.ReadByte();
                  this.Unknown16 = er.ReadUInt32();
                  this.FaceDataLength = er.ReadUInt32();
                  this.FaceDataOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
                  this.Unknown17 = er.ReadUInt32();
                  this.Unknown18 = er.ReadUInt32();
                  this.Unknown19 = er.ReadUInt32();
                  this.Unknown20 = er.ReadUInt32();
                  this.Unknown21 = er.ReadUInt32();
                  this.Unknown22 = er.ReadUInt32();
                  this.Unknown23 = er.ReadUInt32();
                  this.Matrix = new CGFX.DATA.CMDL.SeparateDataShapeCtr.MatrixData(er);
                  long position = er.BaseStream.Position;
                  er.BaseStream.Position = (long) this.FaceDataOffset;
                  this.FaceData = er.ReadBytes((int) this.FaceDataLength);
                  er.BaseStream.Position = position;
                }

                public Vector3[] GetFaceData()
                {
                  int offset = 0;
                  Vector3[] vector3Array;
                  if (this.FaceDataFormat == (byte) 3)
                  {
                    int length = (int) (this.FaceDataLength / 2U / 3U);
                    vector3Array = new Vector3[length];
                    for (int index = 0; index < length; ++index)
                    {
                      vector3Array[index] = new Vector3((float) Bytes.Read2BytesAsUInt16(this.FaceData, offset), (float) Bytes.Read2BytesAsUInt16(this.FaceData, offset + 2), (float) Bytes.Read2BytesAsUInt16(this.FaceData, offset + 4));
                      offset += 6;
                    }
                  }
                  else if (this.FaceDataFormat == (byte) 1)
                  {
                    int length = (int) (this.FaceDataLength / 3U);
                    vector3Array = new Vector3[length];
                    for (int index = 0; index < length; ++index)
                    {
                      vector3Array[index] = new Vector3((float) this.FaceData[offset], (float) this.FaceData[offset + 1], (float) this.FaceData[offset + 2]);
                      offset += 3;
                    }
                  }
                  else
                    vector3Array = new Vector3[0];
                  return vector3Array;
                }
              }
            }
          }

          public class VertexAttributes_
          {
            public uint Unknown1;
            public uint Unknown2;
            public uint Unknown3;
            public uint Unknown4;
            public uint Unknown5;
            public uint VertexDataLength;
            public uint VertexDataOffset;
            public uint Unknown6;
            public uint Unknown7;
            public uint VertexDataEntrySize;
            public uint NrVertexStreams;
            public uint VertexStreamsOffsetArrayOffset;
            public uint[] VertexStreamsOffsetArray;
            public byte[] VertexData;
            public CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_.VertexStreamCtr[] VertexStreams;

            public VertexAttributes_(EndianBinaryReader er)
            {
              this.Unknown1 = er.ReadUInt32();
              this.Unknown2 = er.ReadUInt32();
              this.Unknown3 = er.ReadUInt32();
              this.Unknown4 = er.ReadUInt32();
              this.Unknown5 = er.ReadUInt32();
              this.VertexDataLength = er.ReadUInt32();
              this.VertexDataOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Unknown6 = er.ReadUInt32();
              this.Unknown7 = er.ReadUInt32();
              this.VertexDataEntrySize = er.ReadUInt32();
              this.NrVertexStreams = er.ReadUInt32();
              this.VertexStreamsOffsetArrayOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.VertexStreamsOffsetArrayOffset;
              this.VertexStreamsOffsetArray = new uint[(IntPtr) this.NrVertexStreams];
              for (int index = 0; (long) index < (long) this.NrVertexStreams; ++index)
                this.VertexStreamsOffsetArray[index] = (uint) er.BaseStream.Position + er.ReadUInt32();
              er.BaseStream.Position = (long) this.VertexDataOffset;
              this.VertexData = er.ReadBytes((int) this.VertexDataLength);
              this.VertexStreams = new CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_.VertexStreamCtr[(IntPtr) this.NrVertexStreams];
              for (int index = 0; (long) index < (long) this.NrVertexStreams; ++index)
              {
                er.BaseStream.Position = (long) this.VertexStreamsOffsetArray[index];
                this.VertexStreams[index] = new CGFX.DATA.CMDL.SeparateDataShapeCtr.VertexAttributes_.VertexStreamCtr(er);
              }
            }

            public class VertexStreamCtr
            {
              public uint Flags;
              public uint Type;
              public uint Unknown1;
              public uint Unknown2;
              public uint Unknown3;
              public uint Unknown4;
              public uint Unknown5;
              public uint Unknown6;
              public uint Unknown7;
              public uint Unknown8;
              public uint NrComponents;
              public float Unknown9;
              public uint Offset;

              public VertexStreamCtr(EndianBinaryReader er)
              {
                this.Flags = er.ReadUInt32();
                this.Type = er.ReadUInt32();
                this.Unknown1 = er.ReadUInt32();
                this.Unknown2 = er.ReadUInt32();
                this.Unknown3 = er.ReadUInt32();
                this.Unknown4 = er.ReadUInt32();
                this.Unknown5 = er.ReadUInt32();
                this.Unknown6 = er.ReadUInt32();
                this.Unknown7 = er.ReadUInt32();
                this.Unknown8 = er.ReadUInt32();
                this.NrComponents = er.ReadUInt32();
                this.Unknown9 = er.ReadSingle();
                this.Offset = er.ReadUInt32();
              }
            }
          }
        }

        public class Skeleton_
        {
          public uint Flags;
          public string Signature;
          public uint Unknown1;
          public uint NameOffset;
          public uint Unknown2;
          public uint Unknown3;
          public uint NrBones;
          public uint BoneDictionaryOffset;
          public uint BoneDataOffset;
          public uint Unknown4;
          public uint Unknown5;
          public string Name;
          public CGFX.DICT BoneDictionary;
          public CGFX.DATA.CMDL.Skeleton_.Bone[] Bones;

          public Skeleton_(EndianBinaryReader er)
          {
            this.Flags = er.ReadUInt32();
            this.Signature = er.ReadString(Encoding.ASCII, 4);
            if (this.Signature != "SOBJ")
              throw new SignatureNotCorrectException(this.Signature, "SOBJ", er.BaseStream.Position);
            this.Unknown1 = er.ReadUInt32();
            this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown2 = er.ReadUInt32();
            this.Unknown3 = er.ReadUInt32();
            this.NrBones = er.ReadUInt32();
            this.BoneDictionaryOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.BoneDataOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
            this.Unknown4 = er.ReadUInt32();
            this.Unknown5 = er.ReadUInt32();
            long position = er.BaseStream.Position;
            er.BaseStream.Position = (long) this.NameOffset;
            this.Name = er.ReadStringNT(Encoding.ASCII);
            er.BaseStream.Position = (long) this.BoneDictionaryOffset;
            this.BoneDictionary = new CGFX.DICT(er);
            this.Bones = new CGFX.DATA.CMDL.Skeleton_.Bone[(IntPtr) this.NrBones];
            for (int index = 0; (long) index < (long) this.NrBones; ++index)
            {
              er.BaseStream.Position = (long) this.BoneDictionary.Entries[index].DataOffset;
              this.Bones[index] = new CGFX.DATA.CMDL.Skeleton_.Bone(er);
            }
            er.BaseStream.Position = position;
          }

          public Matrix4 GetMatrix(int JointID)
          {
            int index1 = -1;
            for (int index2 = 0; (long) index2 < (long) this.NrBones; ++index2)
            {
              if ((long) this.Bones[index2].JointID == (long) JointID)
              {
                index1 = index2;
                break;
              }
            }
            if (index1 == -1)
              return Matrix4.Identity;
            Matrix4 left = Matrix4.Identity;
            if (this.Bones[index1].ParentID != -1)
              left = this.GetMatrix(this.Bones[index1].ParentID);
            Matrix4 matrix4 = Matrix4.Identity;
            matrix4.Row0 = new Vector4(this.Bones[index1].Matrix1[0], this.Bones[index1].Matrix1[1], this.Bones[index1].Matrix1[2], 0.0f);
            matrix4.Row1 = new Vector4(this.Bones[index1].Matrix1[4], this.Bones[index1].Matrix1[5], this.Bones[index1].Matrix1[6], 0.0f);
            matrix4.Row2 = new Vector4(this.Bones[index1].Matrix1[8], this.Bones[index1].Matrix1[9], this.Bones[index1].Matrix1[10], 0.0f);
            matrix4.Row3 = new Vector4(0.0f, 0.0f, 0.0f, 1f);
            matrix4 = Matrix4.Mult(matrix4, Matrix4.Scale(this.Bones[index1].Scale));
            Matrix4 right1 = Matrix4.Mult(Matrix4.Rotate(new Vector3(0.0f, 1f, 0.0f), this.Bones[index1].Rotation.Y), Matrix4.Rotate(new Vector3(0.0f, 0.0f, 1f), this.Bones[index1].Rotation.Z));
            Matrix4 right2 = Matrix4.Mult(Matrix4.Rotate(new Vector3(1f, 0.0f, 0.0f), this.Bones[index1].Rotation.X), right1);
            matrix4 = Matrix4.Mult(matrix4, right2);
            matrix4 = Matrix4.Mult(matrix4, Matrix4.Translation(this.Bones[index1].Translation));
            return Matrix4.Mult(left, matrix4);
          }

          private float[] Matrix4ToFloat(Matrix4 mtx)
          {
            return new float[16]
            {
              mtx.M11,
              mtx.M12,
              mtx.M13,
              mtx.M14,
              mtx.M21,
              mtx.M22,
              mtx.M23,
              mtx.M24,
              mtx.M31,
              mtx.M32,
              mtx.M33,
              mtx.M34,
              mtx.M41,
              mtx.M42,
              mtx.M43,
              mtx.M44
            };
          }

          public override string ToString()
          {
            return this.Name;
          }

          public class Bone
          {
            public uint NameOffset;
            public uint Flags;
            public uint JointID;
            public int ParentID;
            public uint ParentOffset;
            public uint Unknown2;
            public uint Unknown3;
            public uint Unknown4;
            public Vector3 Scale;
            public Vector3 Rotation;
            public Vector3 Translation;
            public float[] Matrix1;
            public float[] Matrix2;
            public float[] Matrix3;
            public uint Unknown5;
            public uint Unknown6;
            public uint Unknown7;
            public string Name;

            public Bone(EndianBinaryReader er)
            {
              this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
              this.Flags = er.ReadUInt32();
              this.JointID = er.ReadUInt32();
              this.ParentID = er.ReadInt32();
              this.ParentOffset = this.ParentID != -1 ? (uint) ((ulong) er.BaseStream.Position + (ulong) er.ReadInt32()) : er.ReadUInt32();
              this.Unknown2 = er.ReadUInt32();
              this.Unknown3 = er.ReadUInt32();
              this.Unknown4 = er.ReadUInt32();
              this.Scale = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
              this.Rotation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
              this.Translation = new Vector3(er.ReadSingle(), er.ReadSingle(), er.ReadSingle());
              this.Matrix1 = er.ReadSingles(12);
              this.Matrix2 = er.ReadSingles(12);
              this.Matrix3 = er.ReadSingles(12);
              this.Unknown5 = er.ReadUInt32();
              this.Unknown6 = er.ReadUInt32();
              this.Unknown7 = er.ReadUInt32();
              long position = er.BaseStream.Position;
              er.BaseStream.Position = (long) this.NameOffset;
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

      public class TXOB
      {
        public uint Flags;
        public string Signature;
        public uint Unknown1;
        public uint NameOffset;
        public uint Unknown2;
        public uint Unknown3;
        public uint Height;
        public uint Width;
        public uint Unknown4;
        public uint Unknown5;
        public uint NrLevels;
        public uint Unknown7;
        public uint Unknown8;
        public uint Format;
        public uint Unknown9;
        public uint Height2;
        public uint Width2;
        public uint DataSize;
        public uint DataOffset;
        public uint Unknown10;
        public uint Unknown11;
        public uint Unknown12;
        public string Name;
        public byte[] Data;

        public TXOB()
        {
        }

        public TXOB(EndianBinaryReader er)
        {
          this.Flags = er.ReadUInt32();
          this.Signature = er.ReadString(Encoding.ASCII, 4);
          if (this.Signature != nameof (TXOB))
            throw new SignatureNotCorrectException(this.Signature, nameof (TXOB), er.BaseStream.Position);
          this.Unknown1 = er.ReadUInt32();
          this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.Unknown2 = er.ReadUInt32();
          this.Unknown3 = er.ReadUInt32();
          this.Height = er.ReadUInt32();
          this.Width = er.ReadUInt32();
          this.Unknown4 = er.ReadUInt32();
          this.Unknown5 = er.ReadUInt32();
          this.NrLevels = er.ReadUInt32();
          this.Unknown7 = er.ReadUInt32();
          this.Unknown8 = er.ReadUInt32();
          this.Format = er.ReadUInt32();
          this.Unknown9 = er.ReadUInt32();
          this.Height2 = er.ReadUInt32();
          this.Width2 = er.ReadUInt32();
          this.DataSize = er.ReadUInt32();
          this.DataOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.Unknown10 = er.ReadUInt32();
          this.Unknown11 = er.ReadUInt32();
          this.Unknown12 = er.ReadUInt32();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.NameOffset;
          this.Name = er.ReadStringNT(Encoding.ASCII);
          er.BaseStream.Position = (long) this.DataOffset;
          this.Data = er.ReadBytes((int) this.DataSize);
          er.BaseStream.Position = position;
        }

        public Bitmap GetBitmap(int Level = 0)
        {
          int[] numArray1 = new int[14]
          {
            32,
            24,
            16,
            16,
            16,
            16,
            16,
            8,
            8,
            8,
            4,
            4,
            4,
            8
          };
          int[] numArray2 = new int[64]
          {
            0,
            1,
            8,
            9,
            2,
            3,
            10,
            11,
            16,
            17,
            24,
            25,
            18,
            19,
            26,
            27,
            4,
            5,
            12,
            13,
            6,
            7,
            14,
            15,
            20,
            21,
            28,
            29,
            22,
            23,
            30,
            31,
            32,
            33,
            40,
            41,
            34,
            35,
            42,
            43,
            48,
            49,
            56,
            57,
            50,
            51,
            58,
            59,
            36,
            37,
            44,
            45,
            38,
            39,
            46,
            47,
            52,
            53,
            60,
            61,
            54,
            55,
            62,
            63
          };
          int[,] numArray3 = new int[8, 2]
          {
            {
              2,
              8
            },
            {
              5,
              17
            },
            {
              9,
              29
            },
            {
              13,
              42
            },
            {
              18,
              60
            },
            {
              24,
              80
            },
            {
              33,
              106
            },
            {
              47,
              183
            }
          };
          int num1 = Level;
          uint width = this.Width;
          uint height = this.Height;
          int index1 = 0;
          for (; num1 > 0; --num1)
          {
            index1 += (int) ((double) (width * height) * ((double) numArray1[(IntPtr) this.Format] / 8.0));
            width /= 2U;
            height /= 2U;
          }
          Bitmap bitmap = new Bitmap((int) width, (int) height);
          switch (this.Format)
          {
            case 0:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) this.Data[index1], (int) this.Data[index1 + 3], (int) this.Data[index1 + 2], (int) this.Data[index1 + 1]));
                              index1 += 4;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 2:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              int num2 = (int) this.Data[index1] | (int) this.Data[index1 + 1] << 8;
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((num2 >> 15 & 1) * (int) byte.MaxValue, (num2 >> 10 & 31) * 8, (num2 >> 5 & 31) * 8, (num2 & 31) * 8));
                              index1 += 2;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 3:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              ushort num2 = (ushort) ((uint) this.Data[index1] | (uint) this.Data[index1 + 1] << 8);
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) byte.MaxValue, ((int) num2 >> 11 & 31) * 8, ((int) num2 >> 5 & 63) * 4, ((int) num2 & 31) * 8));
                              index1 += 2;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 4:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              byte num2 = (byte) (((int) this.Data[index1] >> 4) * 17);
                              byte num3 = (byte) (((int) this.Data[index1] & 15) * 17);
                              byte num4 = (byte) (((int) this.Data[index1 + 1] >> 4) * 17);
                              byte num5 = (byte) (((int) this.Data[index1 + 1] & 15) * 17);
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) num3, (int) num4, (int) num5, (int) num2));
                              index1 += 2;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 5:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              byte num2 = this.Data[index1];
                              byte num3 = this.Data[index1 + 1];
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) num2, (int) num3, (int) num3, (int) num3));
                              index1 += 2;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 6:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              byte num2 = this.Data[index1];
                              byte num3 = this.Data[index1 + 1];
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) byte.MaxValue, (int) num3, (int) num3, (int) num3));
                              index1 += 2;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 7:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              byte num2 = this.Data[index1];
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) byte.MaxValue, (int) num2, (int) num2, (int) num2));
                              ++index1;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 9:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; ++index9)
                            {
                              byte num2 = (byte) (((int) this.Data[index1] >> 4) * 17);
                              byte num3 = (byte) (((int) this.Data[index1] & 15) * 17);
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) num3, (int) num2, (int) num2, (int) num2));
                              ++index1;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 10:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; index9 += 2)
                            {
                              byte num2 = (byte) (((int) this.Data[index1] & 15) * 17);
                              byte num3 = (byte) (((int) this.Data[index1] >> 4 & 15) * 17);
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) byte.MaxValue, (int) num2, (int) num2, (int) num2));
                              bitmap.SetPixel(index3 + index5 + index7 + index9 + 1, index2 + index4 + index6 + index8, Color.FromArgb((int) byte.MaxValue, (int) num3, (int) num3, (int) num3));
                              ++index1;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 11:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 8; index4 += 4)
                  {
                    for (int index5 = 0; index5 < 8; index5 += 4)
                    {
                      for (int index6 = 0; index6 < 4; index6 += 2)
                      {
                        for (int index7 = 0; index7 < 4; index7 += 2)
                        {
                          for (int index8 = 0; index8 < 2; ++index8)
                          {
                            for (int index9 = 0; index9 < 2; index9 += 2)
                            {
                              byte num2 = (byte) (((int) this.Data[index1] & 15) * 17);
                              byte num3 = (byte) (((int) this.Data[index1] >> 4 & 15) * 17);
                              bitmap.SetPixel(index3 + index5 + index7 + index9, index2 + index4 + index6 + index8, Color.FromArgb((int) num2, (int) num2, (int) num2, (int) num2));
                              bitmap.SetPixel(index3 + index5 + index7 + index9 + 1, index2 + index4 + index6 + index8, Color.FromArgb((int) num3, (int) num3, (int) num3, (int) num3));
                              ++index1;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
              break;
            case 12:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 2; ++index4)
                  {
                    for (int index5 = 0; index5 < 2; ++index5)
                    {
                      ulong num2 = (ulong) ((long) this.Data[index1] | (long) this.Data[index1 + 1] << 8 | (long) this.Data[index1 + 2] << 16 | (long) this.Data[index1 + 3] << 24 | (long) this.Data[index1 + 4] << 32 | (long) this.Data[index1 + 5] << 40 | (long) this.Data[index1 + 6] << 48 | (long) this.Data[index1 + 7] << 56);
                      bool flag1 = ((long) (num2 >> 33) & 1L) == 1L;
                      bool flag2 = ((long) (num2 >> 32) & 1L) == 1L;
                      Color color1;
                      Color color2;
                      if (flag1)
                      {
                        int num3 = (int) ((long) (num2 >> 59) & 31L);
                        int num4 = (int) ((long) (num2 >> 51) & 31L);
                        int num5 = (int) ((long) (num2 >> 43) & 31L);
                        color1 = Color.FromArgb(num3 << 3 | (num3 & 28) >> 2, num4 << 3 | (num4 & 28) >> 2, num5 << 3 | (num5 & 28) >> 2);
                        int num6 = num3 + ((int) ((long) (num2 >> 56) & 7L) << 29 >> 29);
                        int num7 = num4 + ((int) ((long) (num2 >> 48) & 7L) << 29 >> 29);
                        int num8 = num5 + ((int) ((long) (num2 >> 40) & 7L) << 29 >> 29);
                        color2 = Color.FromArgb(num6 << 3 | (num6 & 28) >> 2, num7 << 3 | (num7 & 28) >> 2, num8 << 3 | (num8 & 28) >> 2);
                      }
                      else
                      {
                        color1 = Color.FromArgb((int) (((long) (num2 >> 60) & 15L) * 17L), (int) (((long) (num2 >> 52) & 15L) * 17L), (int) (((long) (num2 >> 44) & 15L) * 17L));
                        color2 = Color.FromArgb((int) (((long) (num2 >> 56) & 15L) * 17L), (int) (((long) (num2 >> 48) & 15L) * 17L), (int) (((long) (num2 >> 40) & 15L) * 17L));
                      }
                      int index6 = (int) ((long) (num2 >> 37) & 7L);
                      int index7 = (int) ((long) (num2 >> 34) & 7L);
                      for (int index8 = 0; index8 < 4; ++index8)
                      {
                        for (int index9 = 0; index9 < 4; ++index9)
                        {
                          int index10 = (int) ((long) (num2 >> index9 * 4 + index8) & 1L);
                          bool flag3 = ((long) (num2 >> index9 * 4 + index8 + 16) & 1L) == 1L;
                          Color color3;
                          if (flag2 && index8 < 2 || !flag2 && index9 < 2)
                          {
                            int num3 = numArray3[index6, index10] * (flag3 ? -1 : 1);
                            color3 = Color.FromArgb(this.ColorClamp((int) color1.R + num3), this.ColorClamp((int) color1.G + num3), this.ColorClamp((int) color1.B + num3));
                          }
                          else
                          {
                            int num3 = numArray3[index7, index10] * (flag3 ? -1 : 1);
                            color3 = Color.FromArgb(this.ColorClamp((int) color2.R + num3), this.ColorClamp((int) color2.G + num3), this.ColorClamp((int) color2.B + num3));
                          }
                          bitmap.SetPixel(index3 + index5 * 4 + index9, index2 + index4 * 4 + index8, color3);
                        }
                      }
                      index1 += 8;
                    }
                  }
                }
              }
              break;
            case 13:
              for (int index2 = 0; (long) index2 < (long) height; index2 += 8)
              {
                for (int index3 = 0; (long) index3 < (long) width; index3 += 8)
                {
                  for (int index4 = 0; index4 < 2; ++index4)
                  {
                    for (int index5 = 0; index5 < 2; ++index5)
                    {
                      ulong num2 = (ulong) ((long) this.Data[index1] | (long) this.Data[index1 + 1] << 8 | (long) this.Data[index1 + 2] << 16 | (long) this.Data[index1 + 3] << 24 | (long) this.Data[index1 + 4] << 32 | (long) this.Data[index1 + 5] << 40 | (long) this.Data[index1 + 6] << 48 | (long) this.Data[index1 + 7] << 56);
                      ulong num3 = (ulong) ((long) this.Data[index1 + 8] | (long) this.Data[index1 + 9] << 8 | (long) this.Data[index1 + 10] << 16 | (long) this.Data[index1 + 11] << 24 | (long) this.Data[index1 + 12] << 32 | (long) this.Data[index1 + 13] << 40 | (long) this.Data[index1 + 14] << 48 | (long) this.Data[index1 + 15] << 56);
                      bool flag1 = ((long) (num3 >> 33) & 1L) == 1L;
                      bool flag2 = ((long) (num3 >> 32) & 1L) == 1L;
                      Color color1;
                      Color color2;
                      if (flag1)
                      {
                        int num4 = (int) ((long) (num3 >> 59) & 31L);
                        int num5 = (int) ((long) (num3 >> 51) & 31L);
                        int num6 = (int) ((long) (num3 >> 43) & 31L);
                        color1 = Color.FromArgb(num4 << 3 | (num4 & 28) >> 2, num5 << 3 | (num5 & 28) >> 2, num6 << 3 | (num6 & 28) >> 2);
                        int num7 = num4 + ((int) ((long) (num3 >> 56) & 7L) << 29 >> 29);
                        int num8 = num5 + ((int) ((long) (num3 >> 48) & 7L) << 29 >> 29);
                        int num9 = num6 + ((int) ((long) (num3 >> 40) & 7L) << 29 >> 29);
                        color2 = Color.FromArgb(num7 << 3 | (num7 & 28) >> 2, num8 << 3 | (num8 & 28) >> 2, num9 << 3 | (num9 & 28) >> 2);
                      }
                      else
                      {
                        color1 = Color.FromArgb((int) (((long) (num3 >> 60) & 15L) * 17L), (int) (((long) (num3 >> 52) & 15L) * 17L), (int) (((long) (num3 >> 44) & 15L) * 17L));
                        color2 = Color.FromArgb((int) (((long) (num3 >> 56) & 15L) * 17L), (int) (((long) (num3 >> 48) & 15L) * 17L), (int) (((long) (num3 >> 40) & 15L) * 17L));
                      }
                      int index6 = (int) ((long) (num3 >> 37) & 7L);
                      int index7 = (int) ((long) (num3 >> 34) & 7L);
                      for (int index8 = 0; index8 < 4; ++index8)
                      {
                        for (int index9 = 0; index9 < 4; ++index9)
                        {
                          int index10 = (int) ((long) (num3 >> index9 * 4 + index8) & 1L);
                          bool flag3 = ((long) (num3 >> index9 * 4 + index8 + 16) & 1L) == 1L;
                          Color color3;
                          if (flag2 && index8 < 2 || !flag2 && index9 < 2)
                          {
                            int num4 = numArray3[index6, index10] * (flag3 ? -1 : 1);
                            color3 = Color.FromArgb((int) (((long) (num2 >> (index9 * 4 + index8) * 4) & 15L) * 17L), this.ColorClamp((int) color1.R + num4), this.ColorClamp((int) color1.G + num4), this.ColorClamp((int) color1.B + num4));
                          }
                          else
                          {
                            int num4 = numArray3[index7, index10] * (flag3 ? -1 : 1);
                            color3 = Color.FromArgb((int) (((long) (num2 >> (index9 * 4 + index8) * 4) & 15L) * 17L), this.ColorClamp((int) color2.R + num4), this.ColorClamp((int) color2.G + num4), this.ColorClamp((int) color2.B + num4));
                          }
                          bitmap.SetPixel(index3 + index5 * 4 + index9, index2 + index4 * 4 + index8, color3);
                        }
                      }
                      index1 += 16;
                    }
                  }
                }
              }
              break;
            default:
              bitmap = (Bitmap) null;
              break;
          }
          return bitmap;
        }

        private int ColorClamp(int Color)
        {
          if (Color > (int) byte.MaxValue)
            Color = (int) byte.MaxValue;
          if (Color < 0)
            Color = 0;
          return Color;
        }

        public override string ToString()
        {
          return this.Name;
        }
      }
    }

    public class DICT
    {
      public string Signature;
      public uint SectionSize;
      public uint NrEntries;
      public uint Padding;
      public uint Unknown1;
      public uint Unknown2;
      public uint Unknown3;
      public CGFX.DICT.DICTEntry[] Entries;

      public DICT(EndianBinaryReader er)
      {
        this.Signature = er.ReadString(Encoding.ASCII, 4);
        if (this.Signature != nameof (DICT))
          throw new SignatureNotCorrectException(this.Signature, nameof (DICT), er.BaseStream.Position);
        this.SectionSize = er.ReadUInt32();
        this.NrEntries = er.ReadUInt32();
        this.Padding = er.ReadUInt32();
        this.Unknown1 = er.ReadUInt32();
        this.Unknown2 = er.ReadUInt32();
        this.Unknown3 = er.ReadUInt32();
        this.Entries = new CGFX.DICT.DICTEntry[(IntPtr) this.NrEntries];
        for (int index = 0; (long) index < (long) this.NrEntries; ++index)
          this.Entries[index] = new CGFX.DICT.DICTEntry(er);
      }

      public int IndexOf(uint Offset)
      {
        for (int index = 0; (long) index < (long) this.NrEntries; ++index)
        {
          if ((int) this.Entries[index].DataOffset == (int) Offset)
            return index;
        }
        return -1;
      }

      public int IndexOf(string Name)
      {
        for (int index = 0; (long) index < (long) this.NrEntries; ++index)
        {
          if (this.Entries[index].Name == Name)
            return index;
        }
        return -1;
      }

      public class DICTEntry
      {
        public uint Unknown1;
        public ushort Unknown2;
        public ushort Unknown3;
        public uint NameOffset;
        public uint DataOffset;
        public string Name;

        public DICTEntry(EndianBinaryReader er)
        {
          this.Unknown1 = er.ReadUInt32();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadUInt16();
          this.NameOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          this.DataOffset = (uint) er.BaseStream.Position + er.ReadUInt32();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = (long) this.NameOffset;
          this.Name = er.ReadStringNT(Encoding.ASCII);
          er.BaseStream.Position = position;
        }
      }
    }
  }
}
