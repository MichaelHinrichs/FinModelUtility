﻿// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BMD
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using mkds.gcn.bmd;
using Chadsoft.CTools.Image;
using bmd._3D_Formats;
//using MKDS_Course_Modifier.Converters._3D;
//using MKDS_Course_Modifier.Converters.Colission;
using bmd.G3D_Binary_File_Format;
//using MKDS_Course_Modifier.UI;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Tao.OpenGl;
using System.Drawing;

using bmd.formats.inf1;
using bmd.formats.mat3;
using bmd.formats.shp1;
using bmd.formats.vtx1;

using fin.model;
using fin.model.impl;
using fin.util.asserts;
using fin.util.color;
using fin.util.image;

using schema;

#pragma warning disable CS8604

using BlendFactor = mkds.gcn.bmd.BlendFactor;
using LogicOp = mkds.gcn.bmd.LogicOp;

namespace bmd.GCN {
  public partial class BMD {
    private List<BMDShader> Shaders = new List<BMDShader>();
    public const string Signature = "J3D2bmd3";
    public BMD.BMDHeader Header;
    public BMD.INF1Section INF1;
    public BMD.VTX1Section VTX1;
    public BMD.EVP1Section EVP1;
    public BMD.DRW1Section DRW1;
    public BMD.JNT1Section JNT1;
    public BMD.SHP1Section SHP1;
    public BMD.MAT3Section MAT3;
    public BMD.TEX1Section TEX1;

    public BMD(byte[] file)
    {
      EndianBinaryReader er = new EndianBinaryReader((Stream) new MemoryStream(file), Endianness.BigEndian);
      bool OK;
      this.Header = new BMD.BMDHeader(er, "J3D2bmd3", out OK);
      if (!OK)
      {
        // TODO: Message box
        //int num1 = (int) System.Windows.Forms.MessageBox.Show("Error 1");
      }
      else
      {
        while (er.BaseStream.Position != er.BaseStream.Length)
        {
          switch (er.ReadString(Encoding.ASCII, 4))
          {
            case nameof (INF1):
              er.BaseStream.Position -= 4L;
              this.INF1 = new BMD.INF1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 2");
                goto label_21;
              }
              else
                break;
            case nameof (VTX1):
              er.BaseStream.Position -= 4L;
              this.VTX1 = new BMD.VTX1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 4");
                goto label_21;
              }
              else
                break;
            case nameof (EVP1):
              er.BaseStream.Position -= 4L;
              this.EVP1 = new BMD.EVP1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 4");
                goto label_21;
              }
              else
                break;
            case nameof (DRW1):
              er.BaseStream.Position -= 4L;
              this.DRW1 = new BMD.DRW1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 5");
                goto label_21;
              }
              else
                break;
            case nameof (JNT1):
              er.BaseStream.Position -= 4L;
              this.JNT1 = new BMD.JNT1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 6");
                goto label_21;
              }
              else
                break;
            case nameof (SHP1):
              er.BaseStream.Position -= 4L;
              this.SHP1 = new BMD.SHP1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 7");
                goto label_21;
              }
              else
                break;
            case "MAT1":
            case "MAT2":
            case nameof (MAT3):
              er.BaseStream.Position -= 4L;
              this.MAT3 = new BMD.MAT3Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 8");
                goto label_21;
              }
              else
                break;
            case nameof (TEX1):
              er.BaseStream.Position -= 4L;
              this.TEX1 = new BMD.TEX1Section(er, out OK);
              if (!OK)
              {
                // TODO: Message box
                //int num2 = (int) System.Windows.Forms.MessageBox.Show("Error 9");
                goto label_21;
              }
              else
                break;
            default:
              goto label_21;
          }
        }
      }
label_21:
      er.Close();
    }

    public MA.Node[] GetJoints()
    {
      Stack<BMD.Node> nodeStack = new Stack<BMD.Node>();
      nodeStack.Push((BMD.Node) null);
      List<MA.Node> nodeList = new List<MA.Node>();
      BMD.Node node = (BMD.Node) null;
      foreach (Inf1Entry entry in this.INF1.Entries)
      {
        switch (entry.Type)
        {
          case 0:
            goto label_7;
          case 1:
            nodeStack.Push(node);
            break;
          case 2:
            nodeStack.Pop();
            break;
          case 16:
            nodeList.Add(new MA.Node(this.JNT1.Joints[(int) entry.Index].Tx, this.JNT1.Joints[(int) entry.Index].Ty, this.JNT1.Joints[(int) entry.Index].Tz, (float) ((double) this.JNT1.Joints[(int) entry.Index].Rx / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Ry / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Rz / 32768.0 * 180.0), this.JNT1.Joints[(int) entry.Index].Sx, this.JNT1.Joints[(int) entry.Index].Sy, this.JNT1.Joints[(int) entry.Index].Sz, this.JNT1.StringTable[(int) entry.Index], nodeStack.Peek() == null ? (string) null : nodeStack.Peek().Name));
            node = new BMD.Node(this.JNT1.StringTable[(int) entry.Index], nodeStack.Peek() == null ? (BMD.Node) null : nodeStack.Peek());
            break;
        }
      }
label_7:
      return nodeList.ToArray();
    }

    public byte[] ExportBones()
    {
      //ScaleDialog scaleDialog = new ScaleDialog();
      //int num = (int) scaleDialog.ShowDialog();
      float scale = 1; //scaleDialog.scale;
      MA.Node[] joints = this.GetJoints();
      foreach (MA.Node node in joints)
        node.Trans *= scale;
      return MA.WriteBones(joints);
    }

    public void Render()
    {
      Gl.glScalef(1f / 1000f, 1f / 1000f, 1f / 1000f);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glPushMatrix();
      foreach (Inf1Entry entry in this.INF1.Entries)
      {
        switch (entry.Type)
        {
          case 0:
            goto label_35;
          case 16:
            Gl.glTranslatef(this.JNT1.Joints[(int) entry.Index].Tx, this.JNT1.Joints[(int) entry.Index].Ty, this.JNT1.Joints[(int) entry.Index].Tz);
            Gl.glRotatef((float) ((double) this.JNT1.Joints[(int) entry.Index].Rx / 32768.0 * 180.0), 1f, 0.0f, 0.0f);
            Gl.glRotatef((float) ((double) this.JNT1.Joints[(int) entry.Index].Ry / 32768.0 * 180.0), 0.0f, 1f, 0.0f);
            Gl.glRotatef((float) ((double) this.JNT1.Joints[(int) entry.Index].Rz / 32768.0 * 180.0), 0.0f, 0.0f, 1f);
            Gl.glScalef(this.JNT1.Joints[(int) entry.Index].Sx, this.JNT1.Joints[(int) entry.Index].Sy, this.JNT1.Joints[(int) entry.Index].Sz);
            break;
          case 17:
            Gl.glMatrixMode(5890);
            Gl.glLoadIdentity();
            for (int index = 0; index < 8; ++index)
            {
              Gl.glActiveTexture(33984 + index);
              Gl.glLoadIdentity();
              if (this.MAT3.MaterialEntries[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]].TextureIndexes[index] != ushort.MaxValue)
                Gl.glBindTexture(3553, (int) this.MAT3.TextureIndices[(int) this.MAT3.MaterialEntries[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]].TextureIndexes[index]] + 1);
              else
                Gl.glBindTexture(3553, 0);
            }
            Gl.glMatrixMode(5888);
            this.MAT3.glAlphaCompareglBendMode((int) this.MAT3.MaterialEntries[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]].AlphaCompareIndex, (int) this.MAT3.MaterialEntries[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]].BlendModeIndex, (int) this.MAT3.MaterialEntries[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]].UnknownIndex);
            this.Shaders[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]].Enable();
            break;
          case 18:
            foreach (BMD.SHP1Section.Batch.Packet packet in this.SHP1.Batches[(int) entry.Index].Packets)
            {
              foreach (BMD.SHP1Section.Batch.Packet.Primitive primitive in packet.Primitives)
              {
                if (true)
                {
                  Gl.glBegin(primitive.GetGlPrimitive());
                  foreach (BMD.SHP1Section.Batch.Packet.Primitive.Index point in primitive.Points)
                  {
                    MTX44 mtX44 = new MTX44();
                    if (this.SHP1.Batches[(int) entry.Index].HasColors[0])
                      Gl.glColor4f((float) this.VTX1.Colors[0][(int) point.ColorIndex[0]].R / (float) byte.MaxValue, (float) this.VTX1.Colors[0][(int) point.ColorIndex[0]].G / (float) byte.MaxValue, (float) this.VTX1.Colors[0][(int) point.ColorIndex[0]].B / (float) byte.MaxValue, (float) this.VTX1.Colors[0][(int) point.ColorIndex[0]].A / (float) byte.MaxValue);
                    if (this.SHP1.Batches[(int) entry.Index].HasNormals)
                      Gl.glNormal3f(this.VTX1.Normals[(int) point.NormalIndex].X, this.VTX1.Normals[(int) point.NormalIndex].Y, this.VTX1.Normals[(int) point.NormalIndex].Z);
                    for (int index = 0; index < 8; ++index)
                    {
                      if (this.SHP1.Batches[(int) entry.Index].HasTexCoords[index])
                        Gl.glMultiTexCoord2f(33984 + index, this.VTX1.Texcoords[index][(int) point.TexCoordIndex[index]].S, this.VTX1.Texcoords[index][(int) point.TexCoordIndex[index]].T);
                      else
                        Gl.glMultiTexCoord2f(33984 + index, 0.0f, 0.0f);
                    }
                    if (this.SHP1.Batches[(int) entry.Index].HasPositions)
                    {
                      float[] numArray = mtX44.MultVector(new float[3]
                      {
                        this.VTX1.Positions[(int) point.PosIndex].X,
                        this.VTX1.Positions[(int) point.PosIndex].Y,
                        this.VTX1.Positions[(int) point.PosIndex].Z
                      });
                      Gl.glVertex3f(numArray[0], numArray[1], numArray[2]);
                    }
                  }
                  Gl.glEnd();
                }
              }
            }
            break;
        }
      }
label_35:
      Gl.glPopMatrix();
    }

    public byte[] ExportMA()
    {
      List<Group> groupList = new List<Group>();
      List<string> stringList = new List<string>();
      Stack<MTX44> mtX44Stack = new Stack<MTX44>();
      mtX44Stack.Push(new MTX44());
      string str = (string) null;
      foreach (Inf1Entry entry in this.INF1.Entries)
      {
        switch (entry.Type)
        {
          case 0:
            goto label_50;
          case 1:
            mtX44Stack.Push(mtX44Stack.Peek());
            break;
          case 2:
            mtX44Stack.Pop();
            break;
          case 16:
            mtX44Stack.Peek().translate(this.JNT1.Joints[(int) entry.Index].Tx * (1f / 1000f), this.JNT1.Joints[(int) entry.Index].Ty * (1f / 1000f), this.JNT1.Joints[(int) entry.Index].Tz * (1f / 1000f));
            mtX44Stack.Peek().rotate((float) ((double) this.JNT1.Joints[(int) entry.Index].Rx / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Ry / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Rz / 32768.0 * 180.0));
            mtX44Stack.Peek().Scale(this.JNT1.Joints[(int) entry.Index].Sx, this.JNT1.Joints[(int) entry.Index].Sy, this.JNT1.Joints[(int) entry.Index].Sz);
            break;
          case 17:
            str = this.MAT3.MaterialNameTable[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]];
            break;
          case 18:
            foreach (BMD.SHP1Section.Batch.Packet packet in this.SHP1.Batches[(int) entry.Index].Packets)
            {
              stringList.Add(str);
              Group group = new Group();
              foreach (BMD.SHP1Section.Batch.Packet.Primitive primitive in packet.Primitives)
              {
                PolygonType PolyType;
                switch (primitive.Type)
                {
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_QUADS:
                    PolyType = PolygonType.Quad;
                    break;
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLES:
                    PolyType = PolygonType.Triangle;
                    break;
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLESTRIP:
                    PolyType = PolygonType.TriangleStrip;
                    break;
                  default:
                    PolyType = PolygonType.Triangle;
                    break;
                }
                List<Vector3> vector3List1 = new List<Vector3>();
                List<Vector3> vector3List2 = new List<Vector3>();
                List<Vector2> vector2List = new List<Vector2>();
                List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();
                if (!this.SHP1.Batches[(int) entry.Index].HasNormals)
                  vector3List1 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasPositions)
                  vector3List2 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasColors[0])
                  colorList = (List<System.Drawing.Color>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                  vector2List = (List<Vector2>) null;
                foreach (BMD.SHP1Section.Batch.Packet.Primitive.Index point in primitive.Points)
                {
                  MTX44 mtX44 = new MTX44();
                  if (this.SHP1.Batches[(int) entry.Index].HasMatrixIndices)
                  {
                    int index1 = (int) packet.MatrixTable[(int) point.MatrixIndex / 3];
                    int index2 = (int) this.DRW1.Data[index1];
                    if (index2 != 257 && !this.DRW1.IsWeighted[index1])
                    {
                      Matrix4 rotationX1 = Matrix4.CreateRotationX((float) ((double) this.JNT1.Joints[index2].Rx / 32768.0 * 180.0));
                      Matrix4 rotationX2 = Matrix4.CreateRotationX((float) ((double) this.JNT1.Joints[index2].Ry / 32768.0 * 180.0));
                      Matrix4 rotationX3 = Matrix4.CreateRotationX((float) ((double) this.JNT1.Joints[index2].Rz / 32768.0 * 180.0));
                      mtX44.translate(this.JNT1.Joints[index2].Tx * (1f / 1000f), this.JNT1.Joints[index2].Ty * (1f / 1000f), this.JNT1.Joints[index2].Tz * (1f / 1000f));
                      mtX44 = mtX44.MultMatrix((MTX44) this.Matrix4ToFloat(rotationX1)).MultMatrix((MTX44) this.Matrix4ToFloat(rotationX2)).MultMatrix((MTX44) this.Matrix4ToFloat(rotationX3));
                      mtX44.Scale(this.JNT1.Joints[index2].Sx, this.JNT1.Joints[index2].Sy, this.JNT1.Joints[index2].Sz);
                    }
                  }
                  if (this.SHP1.Batches[(int) entry.Index].HasColors[0])
                    colorList.Add((System.Drawing.Color) this.VTX1.Colors[0][(int) point.ColorIndex[0]]);
                  if (this.SHP1.Batches[(int) entry.Index].HasNormals)
                    vector3List1.Add(new Vector3(this.VTX1.Normals[(int) point.NormalIndex].X, this.VTX1.Normals[(int) point.NormalIndex].Y, this.VTX1.Normals[(int) point.NormalIndex].Z));
                  if (this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                    vector2List.Add(new Vector2(this.VTX1.Texcoords[0][(int) point.TexCoordIndex[0]].S, this.VTX1.Texcoords[0][(int) point.TexCoordIndex[0]].T));
                  if (this.SHP1.Batches[(int) entry.Index].HasPositions)
                  {
                    float[] v = mtX44.MultVector(new float[3]
                    {
                      this.VTX1.Positions[(int) point.PosIndex].X,
                      this.VTX1.Positions[(int) point.PosIndex].Y,
                      this.VTX1.Positions[(int) point.PosIndex].Z
                    });
                    float[] numArray = mtX44Stack.Peek().MultVector(v);
                    vector3List2.Add(new Vector3(numArray[0] * 0.25f, numArray[1] * 0.25f, numArray[2] * 0.25f));
                  }
                }
                if (!this.SHP1.Batches[(int) entry.Index].HasNormals)
                  vector3List1 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasPositions)
                  vector3List2 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasColors[0])
                  colorList = (List<System.Drawing.Color>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                  vector2List = (List<Vector2>) null;
                group.Add(new Polygon(PolyType, !this.SHP1.Batches[(int) entry.Index].HasNormals ? (Vector3[]) null : vector3List1.ToArray(), !this.SHP1.Batches[(int) entry.Index].HasTexCoords[0] ? (Vector2[]) null : vector2List.ToArray(), !this.SHP1.Batches[(int) entry.Index].HasPositions ? (Vector3[]) null : vector3List2.ToArray(), !this.SHP1.Batches[(int) entry.Index].HasColors[0] ? (System.Drawing.Color[]) null : colorList.ToArray()));
              }
              groupList.Add(group);
            }
            break;
        }
      }
label_50:
      mtX44Stack.Pop();
      MA.MAWriter maWriter = new MA.MAWriter();
      foreach (BMD.Stringtable.StringTableEntry entry in this.MAT3.MaterialNameTable.Entries)
      {
        string Name = (string) entry;
        maWriter.AddPhong(Name);
      }
      int index = 0;
      foreach (Group group in groupList)
      {
        maWriter.CreateNode(MA.MAWriter.NodeType.transform, "G" + (object) index, (string) null, false, false);
        int num = 0;
        foreach (Polygon polygon in group.Polygons)
        {
          maWriter.AddMesh("G" + (object) index, "P" + (object) num, stringList[index], polygon.Vertex, polygon.TexCoords, polygon.Colors, polygon.Normals, polygon.PolyType);
          ++num;
        }
        ++index;
      }
      return maWriter.Close();
    }

    private Vector3 GetSize(float Scale, out Vector3 Min, out Vector3 Max)
    {
      List<Group> groupList = new List<Group>();
      List<string> stringList = new List<string>();
      Stack<MTX44> mtX44Stack = new Stack<MTX44>();
      mtX44Stack.Push(new MTX44());
      string str = (string) null;
      foreach (Inf1Entry entry in this.INF1.Entries)
      {
        switch (entry.Type)
        {
          case 0:
            goto label_46;
          case 1:
            mtX44Stack.Push(mtX44Stack.Peek());
            break;
          case 2:
            mtX44Stack.Pop();
            break;
          case 16:
            mtX44Stack.Peek().translate(this.JNT1.Joints[(int) entry.Index].Tx * (1f / 1000f), this.JNT1.Joints[(int) entry.Index].Ty * (1f / 1000f), this.JNT1.Joints[(int) entry.Index].Tz * (1f / 1000f));
            mtX44Stack.Peek().rotate((float) ((double) this.JNT1.Joints[(int) entry.Index].Rx / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Ry / 32768.0 * 180.0), (float) ((double) this.JNT1.Joints[(int) entry.Index].Rz / 32768.0 * 180.0));
            mtX44Stack.Peek().Scale(this.JNT1.Joints[(int) entry.Index].Sx, this.JNT1.Joints[(int) entry.Index].Sy, this.JNT1.Joints[(int) entry.Index].Sz);
            break;
          case 17:
            str = this.MAT3.MaterialNameTable[(int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]];
            break;
          case 18:
            foreach (BMD.SHP1Section.Batch.Packet packet in this.SHP1.Batches[(int) entry.Index].Packets)
            {
              stringList.Add(str);
              Group group = new Group();
              foreach (BMD.SHP1Section.Batch.Packet.Primitive primitive in packet.Primitives)
              {
                PolygonType PolyType;
                switch (primitive.Type)
                {
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_QUADS:
                    PolyType = PolygonType.Quad;
                    break;
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLES:
                    PolyType = PolygonType.Triangle;
                    break;
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLESTRIP:
                    PolyType = PolygonType.TriangleStrip;
                    break;
                  default:
                    PolyType = PolygonType.Triangle;
                    break;
                }
                List<Vector3> vector3List1 = new List<Vector3>();
                List<Vector3> vector3List2 = new List<Vector3>();
                List<Vector2> vector2List = new List<Vector2>();
                List<System.Drawing.Color> colorList = new List<System.Drawing.Color>();
                if (!this.SHP1.Batches[(int) entry.Index].HasNormals)
                  vector3List1 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasPositions)
                  vector3List2 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasColors[0])
                  colorList = (List<System.Drawing.Color>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                  vector2List = (List<Vector2>) null;
                foreach (BMD.SHP1Section.Batch.Packet.Primitive.Index point in primitive.Points)
                {
                  MTX44 mtX44 = new MTX44();
                  if (this.SHP1.Batches[(int) entry.Index].HasColors[0])
                    colorList.Add((System.Drawing.Color) this.VTX1.Colors[0][(int) point.ColorIndex[0]]);
                  if (this.SHP1.Batches[(int) entry.Index].HasNormals)
                    vector3List1.Add(new Vector3(this.VTX1.Normals[(int) point.NormalIndex].X, this.VTX1.Normals[(int) point.NormalIndex].Y, this.VTX1.Normals[(int) point.NormalIndex].Z));
                  if (this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                    vector2List.Add(new Vector2(this.VTX1.Texcoords[0][(int) point.TexCoordIndex[0]].S, this.VTX1.Texcoords[0][(int) point.TexCoordIndex[0]].T));
                  if (this.SHP1.Batches[(int) entry.Index].HasPositions)
                    vector3List2.Add(this.VTX1.Positions[(int) point.PosIndex]);
                }
                if (!this.SHP1.Batches[(int) entry.Index].HasNormals)
                  vector3List1 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasPositions)
                  vector3List2 = (List<Vector3>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasColors[0])
                  colorList = (List<System.Drawing.Color>) null;
                if (!this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                  vector2List = (List<Vector2>) null;
                group.Add(new Polygon(PolyType, !this.SHP1.Batches[(int) entry.Index].HasNormals ? (Vector3[]) null : vector3List1.ToArray(), !this.SHP1.Batches[(int) entry.Index].HasTexCoords[0] ? (Vector2[]) null : vector2List.ToArray(), !this.SHP1.Batches[(int) entry.Index].HasPositions ? (Vector3[]) null : vector3List2.ToArray(), !this.SHP1.Batches[(int) entry.Index].HasColors[0] ? (System.Drawing.Color[]) null : colorList.ToArray()));
              }
              groupList.Add(group);
            }
            break;
        }
      }
label_46:
      mtX44Stack.Pop();
      Min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
      Max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
      foreach (Group group in groupList)
      {
        foreach (Polygon polygon in group.Polygons)
        {
          for (int index = 0; index < polygon.Vertex.Length; ++index)
          {
            polygon.Vertex[index] = Vector3.Multiply(polygon.Vertex[index], Scale);
            if ((double) polygon.Vertex[index].X < (double) Min.X)
              Min.X = polygon.Vertex[index].X;
            if ((double) polygon.Vertex[index].X > (double) Max.X)
              Max.X = polygon.Vertex[index].X;
            if ((double) polygon.Vertex[index].Y < (double) Min.Y)
              Min.Y = polygon.Vertex[index].Y;
            if ((double) polygon.Vertex[index].Y > (double) Max.Y)
              Max.Y = polygon.Vertex[index].Y;
            if ((double) polygon.Vertex[index].Z < (double) Min.Z)
              Min.Z = polygon.Vertex[index].Z;
            if ((double) polygon.Vertex[index].Z > (double) Max.Z)
              Max.Z = polygon.Vertex[index].Z;
          }
        }
      }
      return Max - Min;
    }

    private MTX44 multiplyAddition(MTX44 r, MTX44 m, float f)
    {
      for (int index1 = 0; index1 < 3; ++index1)
      {
        for (int index2 = 0; index2 < 4; ++index2)
          r[index2, index1] += f * m[index2, index1];
      }
      return r;
    }

    /*public MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD ExportNSBMD(
      float Scale,
      bool ChangeFar)
    {
      MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(false);
      nsbmd.modelSet = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet();
      string Name = "bmd_model";
      if (Name.Length > 16)
        Name = Name.Remove(16);
      nsbmd.modelSet.dict = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.MDL0Data>();
      nsbmd.modelSet.dict.Add(Name, new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.MDL0Data());
      nsbmd.modelSet.models = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model[1];
      nsbmd.modelSet.models[0] = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model();
      Vector3 Min;
      Vector3 size = this.GetSize(Scale, out Min, out Vector3 _);
      float num1 = Helpers.max(size.X, size.Y, size.Z);
      float num2 = 1f;
      int num3 = 0;
      for (; (double) num1 > 7.999755859375; num1 /= 2f)
      {
        ++num3;
        num2 /= 2f;
      }
      nsbmd.modelSet.models[0].info = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ModelInfo();
      nsbmd.modelSet.models[0].info.numNode = (byte) this.JNT1.NrJoints;
      nsbmd.modelSet.models[0].info.numMat = (byte) this.MAT3.NrMaterials;
      nsbmd.modelSet.models[0].info.firstUnusedMtxStackID = (byte) 1;
      nsbmd.modelSet.models[0].info.posScale = (float) (1 << num3);
      nsbmd.modelSet.models[0].info.invPosScale = 1f / nsbmd.modelSet.models[0].info.posScale;
      nsbmd.modelSet.models[0].info.boxX = Min.X * num2;
      nsbmd.modelSet.models[0].info.boxY = Min.Y * num2;
      nsbmd.modelSet.models[0].info.boxZ = Min.Z * num2;
      nsbmd.modelSet.models[0].info.boxW = size.X * num2;
      nsbmd.modelSet.models[0].info.boxH = size.Y * num2;
      nsbmd.modelSet.models[0].info.boxD = size.Z * num2;
      nsbmd.modelSet.models[0].info.boxPosScale = (float) (1 << num3);
      nsbmd.modelSet.models[0].info.boxInvPosScale = 1f / nsbmd.modelSet.models[0].info.boxPosScale;
      nsbmd.modelSet.models[0].nodes = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.NodeSet();
      nsbmd.modelSet.models[0].nodes.dict = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.NodeSet.NodeSetData>();
      nsbmd.modelSet.models[0].nodes.dict.Add("world_root", new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.NodeSet.NodeSetData());
      nsbmd.modelSet.models[0].nodes.data = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.NodeSet.NodeData[1];
      nsbmd.modelSet.models[0].nodes.data[0] = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.NodeSet.NodeData();
      nsbmd.modelSet.models[0].nodes.data[0].flag = (ushort) 7;
      nsbmd.modelSet.models[0].materials = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet();
      nsbmd.modelSet.models[0].materials.dictTexToMatList = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.TexToMatData>();
      nsbmd.modelSet.models[0].materials.dictPlttToMatList = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.PlttToMatData>();
      MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet texplttSet = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet();
      texplttSet.TexInfo = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.texInfo();
      texplttSet.Tex4x4Info = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.tex4x4Info();
      texplttSet.PlttInfo = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.plttInfo();
      texplttSet.dictTex = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData>();
      texplttSet.dictPltt = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData>();
      int index1 = 0;
      int index2 = 0;
      foreach (BMD.TEX1Section.TextureHeader textureHeader in this.TEX1.TextureHeaders)
      {
        Graphic.GXTexFmt gxTexFmt;
        switch (textureHeader.Format)
        {
          case BMD.TEX1Section.TextureFormat.I4:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT16;
            break;
          case BMD.TEX1Section.TextureFormat.I8:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT256;
            break;
          case BMD.TEX1Section.TextureFormat.A4_I4:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_A3I5;
            break;
          case BMD.TEX1Section.TextureFormat.A8_I8:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_A5I3;
            break;
          case BMD.TEX1Section.TextureFormat.R5_G6_B5:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT256;
            break;
          case BMD.TEX1Section.TextureFormat.A3_RGB5:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_A3I5;
            break;
          case BMD.TEX1Section.TextureFormat.ARGB8:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT256;
            break;
          case BMD.TEX1Section.TextureFormat.INDEX4:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT16;
            break;
          case BMD.TEX1Section.TextureFormat.INDEX8:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT256;
            break;
          case BMD.TEX1Section.TextureFormat.INDEX14_X2:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_PLTT256;
            break;
          case BMD.TEX1Section.TextureFormat.S3TC1:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_COMP4x4;
            break;
          default:
            gxTexFmt = Graphic.GXTexFmt.GX_TEXFMT_NONE;
            break;
        }
        if (gxTexFmt != Graphic.GXTexFmt.GX_TEXFMT_NONE)
        {
          System.Drawing.Bitmap bitmap1 = textureHeader.ToBitmap();
          int width = bitmap1.Width / 2;
          int height = bitmap1.Height / 2;
          if (width == 4 || height == 4)
          {
            width *= 2;
            height *= 2;
          }
          System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap((System.Drawing.Image) bitmap1, width, height);
          string str1 = this.TEX1.StringTable[index1];
          if (str1.Length > 12)
            str1.Remove(12);
          texplttSet.dictTex.Add(index2.ToString() + "_t", new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData());
          string str2 = this.TEX1.StringTable[index1];
          if (str2.Length > 10)
            str2.Remove(10);
          texplttSet.dictPltt.Add(index2.ToString() + "_p", new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData());
          nsbmd.modelSet.models[0].materials.dictTexToMatList.Add(index2.ToString() + "_t", new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.TexToMatData());
          nsbmd.modelSet.models[0].materials.dictTexToMatList.entry.data[index2].NrMat = (byte) 0;
          nsbmd.modelSet.models[0].materials.dictTexToMatList.entry.data[index2].Materials = new int[0];
          nsbmd.modelSet.models[0].materials.dictPlttToMatList.Add(index2.ToString() + "_p", new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.PlttToMatData());
          nsbmd.modelSet.models[0].materials.dictPlttToMatList.entry.data[index2].NrMat = (byte) 0;
          nsbmd.modelSet.models[0].materials.dictPlttToMatList.entry.data[index2].Materials = new int[0];
          KeyValuePair<string, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData> keyValuePair = texplttSet.dictTex[index2];
          keyValuePair.Value.Fmt = gxTexFmt;
          keyValuePair = texplttSet.dictTex[index2];
          keyValuePair.Value.S = (ushort) bitmap2.Width;
          keyValuePair = texplttSet.dictTex[index2];
          keyValuePair.Value.T = (ushort) bitmap2.Height;
          System.Drawing.Bitmap b = bitmap2;
          keyValuePair = texplttSet.dictTex[index2];
          ref byte[] local1 = ref keyValuePair.Value.Data;
          ref byte[] local2 = ref texplttSet.dictPltt[index2].Value.Data;
          keyValuePair = texplttSet.dictTex[index2];
          ref byte[] local3 = ref keyValuePair.Value.Data4x4;
          keyValuePair = texplttSet.dictTex[index2];
          int fmt = (int) keyValuePair.Value.Fmt;
          keyValuePair = texplttSet.dictTex[index2];
          ref bool local4 = ref keyValuePair.Value.TransparentColor;
          Graphic.ConvertBitmap(b, out local1, out local2, out local3, (Graphic.GXTexFmt) fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, out local4, false);
          ++index2;
        }
        ++index1;
      }
      nsbmd.TexPlttSet = texplttSet;
      nsbmd.modelSet.models[0].materials.dict = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.MaterialSetData>();
      nsbmd.modelSet.models[0].materials.materials = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material[(int) this.MAT3.NrMaterials];
      int index3 = 0;
      foreach (BMD.MAT3Section.MaterialEntry materialEntry in this.MAT3.MaterialEntries)
      {
        int index4 = -1;
        for (int index5 = 0; index5 < 8; ++index5)
        {
          if (materialEntry.TexStages[index5] != ushort.MaxValue)
          {
            index4 = (int) this.MAT3.TextureIndieces[(int) materialEntry.TexStages[index5]];
            break;
          }
        }
        if (index4 != -1)
        {
          KeyValuePair<string, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.TexToMatData> dictTexToMat = nsbmd.modelSet.models[0].materials.dictTexToMatList[index4];
          ++dictTexToMat.Value.NrMat;
          List<int> intList1 = new List<int>();
          List<int> intList2 = intList1;
          dictTexToMat = nsbmd.modelSet.models[0].materials.dictTexToMatList[index4];
          int[] materials = dictTexToMat.Value.Materials;
          intList2.AddRange((IEnumerable<int>) materials);
          intList1.Add(index3);
          dictTexToMat = nsbmd.modelSet.models[0].materials.dictTexToMatList[index4];
          dictTexToMat.Value.Materials = intList1.ToArray();
          ++nsbmd.modelSet.models[0].materials.dictPlttToMatList[index4].Value.NrMat;
          List<int> intList3 = new List<int>();
          intList3.AddRange((IEnumerable<int>) nsbmd.modelSet.models[0].materials.dictPlttToMatList[index4].Value.Materials);
          intList3.Add(index3);
          nsbmd.modelSet.models[0].materials.dictPlttToMatList[index4].Value.Materials = intList3.ToArray();
        }
        string str = this.MAT3.StringTable[index3];
        if (str.Length > 13)
          str.Remove(13);
        nsbmd.modelSet.models[0].materials.dict.Add(index3.ToString() + "_m", new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.MaterialSetData());
        nsbmd.modelSet.models[0].materials.materials[index3] = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material();
        MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material material1 = nsbmd.modelSet.models[0].materials.materials[index3];
        System.Drawing.Color color = System.Drawing.Color.Black;
        int num4 = (Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue) << 16 | 32768;
        color = System.Drawing.Color.FromArgb(200, 200, 200);
        int num5 = Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue;
        int num6 = num4 | num5;
        material1.diffAmb = (uint) num6;
        MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material material2 = nsbmd.modelSet.models[0].materials.materials[index3];
        color = System.Drawing.Color.Black;
        int num7 = (Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue) << 16;
        color = System.Drawing.Color.Black;
        int num8 = Graphic.encodeColor(color.ToArgb()) & (int) short.MaxValue;
        int num9 = num7 | num8;
        material2.specEmi = (uint) num9;
        uint num10 = 31;
        nsbmd.modelSet.models[0].materials.materials[index3].polyAttr = 0U;
        nsbmd.modelSet.models[0].materials.materials[index3].polyAttr |= 64U;
        nsbmd.modelSet.models[0].materials.materials[index3].polyAttr |= num10 << 16;
        nsbmd.modelSet.models[0].materials.materials[index3].polyAttrMask = 1059059967U;
        nsbmd.modelSet.models[0].materials.materials[index3].texImageParam = 196608U;
        nsbmd.modelSet.models[0].materials.materials[index3].texImageParamMask = uint.MaxValue;
        nsbmd.modelSet.models[0].materials.materials[index3].texPlttBase = (ushort) 0;
        nsbmd.modelSet.models[0].materials.materials[index3].flag = MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_TEXMTX_ROTZERO | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_DIFFUSE | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_AMBIENT | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_VTXCOLOR | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SPECULAR | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_EMISSION | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.MaterialSet.Material.NNS_G3D_MATFLAG.NNS_G3D_MATFLAG_SHININESS;
        nsbmd.modelSet.models[0].materials.materials[index3].transS = -this.MAT3.TextureMatrices[(int) materialEntry.TexMatrices[0]].Unknown2;
        nsbmd.modelSet.models[0].materials.materials[index3].transT = -this.MAT3.TextureMatrices[(int) materialEntry.TexMatrices[0]].Unknown3;
        nsbmd.modelSet.models[0].materials.materials[index3].scaleS = this.MAT3.TextureMatrices[(int) materialEntry.TexMatrices[0]].Unknown5;
        nsbmd.modelSet.models[0].materials.materials[index3].scaleT = this.MAT3.TextureMatrices[(int) materialEntry.TexMatrices[0]].Unknown6;
        if (index4 != -1)
        {
          nsbmd.modelSet.models[0].materials.materials[index3].origWidth = (ushort) ((uint) this.TEX1.TextureHeaders[index4].Width / 2U);
          nsbmd.modelSet.models[0].materials.materials[index3].origHeight = (ushort) ((uint) this.TEX1.TextureHeaders[index4].Height / 2U);
          if (nsbmd.modelSet.models[0].materials.materials[index3].origWidth == (ushort) 4 || nsbmd.modelSet.models[0].materials.materials[index3].origHeight == (ushort) 4)
          {
            nsbmd.modelSet.models[0].materials.materials[index3].origWidth *= (ushort) 2;
            nsbmd.modelSet.models[0].materials.materials[index3].origHeight *= (ushort) 2;
          }
          if (this.TEX1.TextureHeaders[index4].WrapS == BMD.TEX1Section.GX_WRAP_TAG.GX_CLAMP)
            nsbmd.modelSet.models[0].materials.materials[index3].texImageParam &= 4294639615U;
          else if (this.TEX1.TextureHeaders[index4].WrapS == BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT)
            nsbmd.modelSet.models[0].materials.materials[index3].texImageParam |= 65536U;
          else if (this.TEX1.TextureHeaders[index4].WrapS == BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR)
            nsbmd.modelSet.models[0].materials.materials[index3].texImageParam |= 327680U;
          if (this.TEX1.TextureHeaders[index4].WrapT == BMD.TEX1Section.GX_WRAP_TAG.GX_CLAMP)
            nsbmd.modelSet.models[0].materials.materials[index3].texImageParam &= 4294311935U;
          else if (this.TEX1.TextureHeaders[index4].WrapT == BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT)
            nsbmd.modelSet.models[0].materials.materials[index3].texImageParam |= 131072U;
          else if (this.TEX1.TextureHeaders[index4].WrapT == BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR)
            nsbmd.modelSet.models[0].materials.materials[index3].texImageParam |= 655360U;
        }
        nsbmd.modelSet.models[0].materials.materials[index3].magW = 1f;
        nsbmd.modelSet.models[0].materials.materials[index3].magH = 1f;
        ++index3;
      }
      nsbmd.modelSet.models[0].shapes = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet();
      nsbmd.modelSet.models[0].shapes.dict = new Dictionary<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.ShapeSetData>();
      List<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape> shapeList = new List<MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape>();
      Obj2Nsbmd.SBCWriter sbcWriter = new Obj2Nsbmd.SBCWriter();
      sbcWriter.NODEDESC((byte) 0, (byte) 0, false, false, 0, -1);
      sbcWriter.NODE((byte) 0, true);
      sbcWriter.POSSCALE(true);
      MTX44 mtX44_1 = (MTX44) null;
      if (ChangeFar)
      {
        mtX44_1 = new MTX44();
        mtX44_1.Zero();
        float num4 = 0.1455078f;
        float num5 = -0.1455078f;
        float num6 = -0.1940918f;
        float num7 = 0.1940918f;
        float num8 = 0.25f;
        float num9 = 300f;
        mtX44_1[0, 0] = (float) (2.0 * (double) num8 / ((double) num7 - (double) num6));
        mtX44_1[1, 1] = (float) (2.0 * (double) num8 / ((double) num4 - (double) num5));
        mtX44_1[0, 2] = (float) (((double) num7 + (double) num6) / ((double) num7 - (double) num6));
        mtX44_1[1, 2] = (float) (((double) num4 + (double) num5) / ((double) num4 - (double) num5));
        mtX44_1[2, 2] = (float) (((double) num8 + (double) num9) / ((double) num8 - (double) num9));
        mtX44_1[3, 2] = -1f;
        mtX44_1[2, 3] = (float) (2.0 * (double) num8 * (double) num9 / ((double) num8 - (double) num9));
      }
      int index6 = 0;
      int num11 = 0;
      Stack<int> intStack = new Stack<int>();
      int num12 = 0;
      int num13 = 0;
      foreach (Inf1Entry entry in this.INF1.Entries)
      {
        switch (entry.Type)
        {
          case 0:
            sbcWriter.POSSCALE(false);
            sbcWriter.RET();
            sbcWriter.NOP();
            goto label_140;
          case 1:
            intStack.Push(num13);
            break;
          case 2:
            num13 = intStack.Pop();
            break;
          case 16:
            num13 = (int) entry.Index;
            break;
          case 17:
            sbcWriter.MAT((byte) this.MAT3.MaterialEntryIndieces[(int) entry.Index]);
            index6 = (int) this.MAT3.MaterialEntryIndieces[(int) entry.Index];
            break;
          case 18:
            foreach (BMD.SHP1Section.Batch.Packet packet in this.SHP1.Batches[(int) entry.Index].Packets)
            {
              nsbmd.modelSet.models[0].shapes.dict.Add("packet_" + (object) num11, new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.ShapeSetData());
              sbcWriter.SHP((byte) num11);
              ++num11;
              ++nsbmd.modelSet.models[0].info.numShp;
              GlNitro.DisplayListEncoder displayListEncoder = new GlNitro.DisplayListEncoder();
              if (ChangeFar && num11 - 1 == 0)
              {
                displayListEncoder.AddCommand((byte) 16, new uint[1]);
                uint[] numArray = new uint[16];
                for (int index4 = 0; index4 < 16; ++index4)
                  numArray[index4] = (uint) ((double) mtX44_1[index4] * 4096.0);
                displayListEncoder.AddCommand((byte) 22, numArray);
                displayListEncoder.AddCommand((byte) 16, 2U);
              }
              foreach (BMD.SHP1Section.Batch.Packet.Primitive primitive in packet.Primitives)
              {
                ++nsbmd.modelSet.models[0].info.numPolygon;
                switch (primitive.Type)
                {
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_QUADS:
                    displayListEncoder.Begin((byte) 1);
                    nsbmd.modelSet.models[0].info.numQuad += (ushort) (primitive.Points.Length / 4);
                    break;
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLES:
                    displayListEncoder.Begin((byte) 0);
                    nsbmd.modelSet.models[0].info.numTriangle += (ushort) (primitive.Points.Length / 3);
                    break;
                  case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLESTRIP:
                    displayListEncoder.Begin((byte) 2);
                    nsbmd.modelSet.models[0].info.numTriangle += (ushort) (primitive.Points.Length - 2);
                    break;
                  default:
                    int num4 = (int) MessageBox.Show("Unsupported Primitive!");
                    break;
                }
                Vector2[] vector2Array = (Vector2[]) null;
                if (this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                {
                  List<Vector2> vector2List = new List<Vector2>();
                  foreach (BMD.SHP1Section.Batch.Packet.Primitive.Index point in primitive.Points)
                    vector2List.Add(new Vector2(this.VTX1.Texcoords[0][(int) point.TexCoordIndex[0]].S, this.VTX1.Texcoords[0][(int) point.TexCoordIndex[0]].T));
                  vector2Array = vector2List.ToArray();
                  float num5 = 2047.938f / (float) nsbmd.modelSet.models[0].materials.materials[index6].origWidth;
                  float num6 = -2048f / (float) nsbmd.modelSet.models[0].materials.materials[index6].origWidth;
                  float num7 = 2047.938f / (float) nsbmd.modelSet.models[0].materials.materials[index6].origHeight;
                  float num8 = -2048f / (float) nsbmd.modelSet.models[0].materials.materials[index6].origHeight;
                  float num9 = vector2Array[0].X % 1f;
                  float num10 = vector2Array[0].Y % 1f;
                  for (int index4 = 0; index4 < vector2Array.Length - 1; ++index4)
                  {
                    vector2Array[index4 + 1].X = vector2Array[index4 + 1].X - vector2Array[0].X + num9;
                    vector2Array[index4 + 1].Y = vector2Array[index4 + 1].Y - vector2Array[0].Y + num10;
                  }
                  vector2Array[0].X = num9;
                  vector2Array[0].Y = num10;
                  int num14 = 0;
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    while ((double) vector2Array[index4].X + (double) num14 <= (double) num6)
                      ++num14;
                  }
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    while ((double) vector2Array[index4].X + (double) num14 >= (double) num5)
                      --num14;
                  }
                  int num15 = 0;
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    while ((double) vector2Array[index4].Y + (double) num15 <= (double) num8)
                      ++num15;
                  }
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    while ((double) vector2Array[index4].Y + (double) num15 >= (double) num7)
                      --num15;
                  }
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    vector2Array[index4].X += (float) num14;
                    vector2Array[index4].Y += (float) num15;
                  }
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    if ((double) vector2Array[index4].X <= (double) num6 || (double) vector2Array[index4].X >= (double) num5 || (double) vector2Array[index4].Y <= (double) num8 || (double) vector2Array[index4].Y >= (double) num7)
                    {
                      if ((double) vector2Array[index4].X <= (double) num6)
                        vector2Array[index4].X = num6;
                      else if ((double) vector2Array[index4].X >= (double) num5)
                        vector2Array[index4].X = num5;
                      if ((double) vector2Array[index4].Y <= (double) num8)
                        vector2Array[index4].Y = num8;
                      else if ((double) vector2Array[index4].Y >= (double) num7)
                        vector2Array[index4].Y = num7;
                    }
                  }
                  for (int index4 = 0; index4 < vector2Array.Length; ++index4)
                  {
                    vector2Array[index4].X *= (float) nsbmd.modelSet.models[0].materials.materials[index6].origWidth;
                    vector2Array[index4].Y *= (float) nsbmd.modelSet.models[0].materials.materials[index6].origHeight;
                  }
                }
                int num16 = 0;
                foreach (BMD.SHP1Section.Batch.Packet.Primitive.Index point in primitive.Points)
                {
                  MTX44 mtX44_2 = new MTX44();
                  if (this.SHP1.Batches[(int) entry.Index].HasTexCoords[0])
                    displayListEncoder.TexCoord(vector2Array[num16++]);
                  if (this.SHP1.Batches[(int) entry.Index].HasColors[0])
                    displayListEncoder.Color((System.Drawing.Color) this.VTX1.Colors[0][(int) point.ColorIndex[0]]);
                  else
                    displayListEncoder.Color(System.Drawing.Color.White);
                  if (this.SHP1.Batches[(int) entry.Index].HasPositions)
                  {
                    displayListEncoder.BestVertex(this.VTX1.Positions[(int) point.PosIndex] * Scale * num2);
                    ++nsbmd.modelSet.models[0].info.numVertex;
                  }
                }
                displayListEncoder.End();
              }
              MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape shape = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape()
              {
                DL = displayListEncoder.GetDisplayList()
              };
              shape.sizeDL = (uint) shape.DL.Length;
              shape.flag = MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG.NNS_G3D_SHPFLAG_USE_COLOR | MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape.NNS_G3D_SHPFLAG.NNS_G3D_SHPFLAG_USE_TEXCOORD;
              shapeList.Add(shape);
            }
            break;
        }
        ++num12;
      }
label_140:
      nsbmd.modelSet.models[0].shapes.shape = shapeList.ToArray();
      nsbmd.modelSet.models[0].sbc = sbcWriter.GetData();
      return nsbmd;
    }*/

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

    public void BindTextures()
    {
      int num = 1;
      foreach (BMD.TEX1Section.TextureHeader textureHeader in this.TEX1.TextureHeaders)
      {
        Gl.glBindTexture(3553, num++);
        Gl.glColor3f(1f, 1f, 1f);
        System.Drawing.Bitmap bitmap = textureHeader.ToBitmap();
        BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        Gl.glTexParameteri(3553, 33169, 1);
        Gl.glTexImage2D(3553, 0, 32856, bitmap.Width, bitmap.Height, 0, 32993, 5121, bitmapdata.Scan0);
        Gl.glTexParameteri(3553, 33169, 0);
        bitmap.UnlockBits(bitmapdata);
        Gl.glTexParameteri(3553, 10241, textureHeader.GetGlFilterModeMin());
        Gl.glTexParameteri(3553, 10240, textureHeader.GetGlFilterModeMag());
        Gl.glTexParameterf(3553, 10242, (float) textureHeader.GetGlWrapModeS());
        Gl.glTexParameterf(3553, 10243, (float) textureHeader.GetGlWrapModeT());
      }
      for (int MatIdx = 0; MatIdx < (int) this.MAT3.NrMaterials; ++MatIdx)
      {
        this.Shaders.Add(this.MAT3.GetShader(MatIdx));
        this.Shaders.Last<BMDShader>().Compile();
      }
    }

    private float[][] ExtractFrustum()
    {
      float[] params1 = new float[16];
      float[] params2 = new float[16];
      float[] numArray1 = new float[16];
      float[][] numArray2 = new float[6][]
      {
        new float[6],
        new float[6],
        new float[6],
        new float[6],
        new float[6],
        new float[6]
      };
      Gl.glGetFloatv(2983, params1);
      Gl.glGetFloatv(2982, params2);
      numArray1[0] = (float) ((double) params2[0] * (double) params1[0] + (double) params2[1] * (double) params1[4] + (double) params2[2] * (double) params1[8] + (double) params2[3] * (double) params1[12]);
      numArray1[1] = (float) ((double) params2[0] * (double) params1[1] + (double) params2[1] * (double) params1[5] + (double) params2[2] * (double) params1[9] + (double) params2[3] * (double) params1[13]);
      numArray1[2] = (float) ((double) params2[0] * (double) params1[2] + (double) params2[1] * (double) params1[6] + (double) params2[2] * (double) params1[10] + (double) params2[3] * (double) params1[14]);
      numArray1[3] = (float) ((double) params2[0] * (double) params1[3] + (double) params2[1] * (double) params1[7] + (double) params2[2] * (double) params1[11] + (double) params2[3] * (double) params1[15]);
      numArray1[4] = (float) ((double) params2[4] * (double) params1[0] + (double) params2[5] * (double) params1[4] + (double) params2[6] * (double) params1[8] + (double) params2[7] * (double) params1[12]);
      numArray1[5] = (float) ((double) params2[4] * (double) params1[1] + (double) params2[5] * (double) params1[5] + (double) params2[6] * (double) params1[9] + (double) params2[7] * (double) params1[13]);
      numArray1[6] = (float) ((double) params2[4] * (double) params1[2] + (double) params2[5] * (double) params1[6] + (double) params2[6] * (double) params1[10] + (double) params2[7] * (double) params1[14]);
      numArray1[7] = (float) ((double) params2[4] * (double) params1[3] + (double) params2[5] * (double) params1[7] + (double) params2[6] * (double) params1[11] + (double) params2[7] * (double) params1[15]);
      numArray1[8] = (float) ((double) params2[8] * (double) params1[0] + (double) params2[9] * (double) params1[4] + (double) params2[10] * (double) params1[8] + (double) params2[11] * (double) params1[12]);
      numArray1[9] = (float) ((double) params2[8] * (double) params1[1] + (double) params2[9] * (double) params1[5] + (double) params2[10] * (double) params1[9] + (double) params2[11] * (double) params1[13]);
      numArray1[10] = (float) ((double) params2[8] * (double) params1[2] + (double) params2[9] * (double) params1[6] + (double) params2[10] * (double) params1[10] + (double) params2[11] * (double) params1[14]);
      numArray1[11] = (float) ((double) params2[8] * (double) params1[3] + (double) params2[9] * (double) params1[7] + (double) params2[10] * (double) params1[11] + (double) params2[11] * (double) params1[15]);
      numArray1[12] = (float) ((double) params2[12] * (double) params1[0] + (double) params2[13] * (double) params1[4] + (double) params2[14] * (double) params1[8] + (double) params2[15] * (double) params1[12]);
      numArray1[13] = (float) ((double) params2[12] * (double) params1[1] + (double) params2[13] * (double) params1[5] + (double) params2[14] * (double) params1[9] + (double) params2[15] * (double) params1[13]);
      numArray1[14] = (float) ((double) params2[12] * (double) params1[2] + (double) params2[13] * (double) params1[6] + (double) params2[14] * (double) params1[10] + (double) params2[15] * (double) params1[14]);
      numArray1[15] = (float) ((double) params2[12] * (double) params1[3] + (double) params2[13] * (double) params1[7] + (double) params2[14] * (double) params1[11] + (double) params2[15] * (double) params1[15]);
      numArray2[0][0] = numArray1[3] - numArray1[0];
      numArray2[0][1] = numArray1[7] - numArray1[4];
      numArray2[0][2] = numArray1[11] - numArray1[8];
      numArray2[0][3] = numArray1[15] - numArray1[12];
      float num1 = (float) Math.Sqrt((double) numArray2[0][0] * (double) numArray2[0][0] + (double) numArray2[0][1] * (double) numArray2[0][1] + (double) numArray2[0][2] * (double) numArray2[0][2]);
      numArray2[0][0] /= num1;
      numArray2[0][1] /= num1;
      numArray2[0][2] /= num1;
      numArray2[0][3] /= num1;
      numArray2[1][0] = numArray1[3] + numArray1[0];
      numArray2[1][1] = numArray1[7] + numArray1[4];
      numArray2[1][2] = numArray1[11] + numArray1[8];
      numArray2[1][3] = numArray1[15] + numArray1[12];
      float num2 = (float) Math.Sqrt((double) numArray2[1][0] * (double) numArray2[1][0] + (double) numArray2[1][1] * (double) numArray2[1][1] + (double) numArray2[1][2] * (double) numArray2[1][2]);
      numArray2[1][0] /= num2;
      numArray2[1][1] /= num2;
      numArray2[1][2] /= num2;
      numArray2[1][3] /= num2;
      numArray2[2][0] = numArray1[3] + numArray1[1];
      numArray2[2][1] = numArray1[7] + numArray1[5];
      numArray2[2][2] = numArray1[11] + numArray1[9];
      numArray2[2][3] = numArray1[15] + numArray1[13];
      float num3 = (float) Math.Sqrt((double) numArray2[2][0] * (double) numArray2[2][0] + (double) numArray2[2][1] * (double) numArray2[2][1] + (double) numArray2[2][2] * (double) numArray2[2][2]);
      numArray2[2][0] /= num3;
      numArray2[2][1] /= num3;
      numArray2[2][2] /= num3;
      numArray2[2][3] /= num3;
      numArray2[3][0] = numArray1[3] - numArray1[1];
      numArray2[3][1] = numArray1[7] - numArray1[5];
      numArray2[3][2] = numArray1[11] - numArray1[9];
      numArray2[3][3] = numArray1[15] - numArray1[13];
      float num4 = (float) Math.Sqrt((double) numArray2[3][0] * (double) numArray2[3][0] + (double) numArray2[3][1] * (double) numArray2[3][1] + (double) numArray2[3][2] * (double) numArray2[3][2]);
      numArray2[3][0] /= num4;
      numArray2[3][1] /= num4;
      numArray2[3][2] /= num4;
      numArray2[3][3] /= num4;
      numArray2[4][0] = numArray1[3] - numArray1[2];
      numArray2[4][1] = numArray1[7] - numArray1[6];
      numArray2[4][2] = numArray1[11] - numArray1[10];
      numArray2[4][3] = numArray1[15] - numArray1[14];
      float num5 = (float) Math.Sqrt((double) numArray2[4][0] * (double) numArray2[4][0] + (double) numArray2[4][1] * (double) numArray2[4][1] + (double) numArray2[4][2] * (double) numArray2[4][2]);
      numArray2[4][0] /= num5;
      numArray2[4][1] /= num5;
      numArray2[4][2] /= num5;
      numArray2[4][3] /= num5;
      numArray2[5][0] = numArray1[3] + numArray1[2];
      numArray2[5][1] = numArray1[7] + numArray1[6];
      numArray2[5][2] = numArray1[11] + numArray1[10];
      numArray2[5][3] = numArray1[15] + numArray1[14];
      float num6 = (float) Math.Sqrt((double) numArray2[5][0] * (double) numArray2[5][0] + (double) numArray2[5][1] * (double) numArray2[5][1] + (double) numArray2[5][2] * (double) numArray2[5][2]);
      numArray2[5][0] /= num6;
      numArray2[5][1] /= num6;
      numArray2[5][2] /= num6;
      numArray2[5][3] /= num6;
      return numArray2;
    }

    private bool PointInFrustum(float x, float y, float z, float[][] frustum)
    {
      for (int index = 0; index < 6; ++index)
      {
        if ((double) frustum[index][0] * (double) x + (double) frustum[index][1] * (double) y + (double) frustum[index][2] * (double) z + (double) frustum[index][3] <= 0.0)
          return false;
      }
      return true;
    }

    public class BMDHeader
    {
      public string Type;
      public uint FileSize;
      public uint NrSections;
      public byte[] Padding;

      public BMDHeader(EndianBinaryReader er, string Signature, out bool OK)
      {
        this.Type = er.ReadString(Encoding.ASCII, 8);
        if (this.Type != Signature)
        {
          OK = false;
        }
        else
        {
          this.FileSize = er.ReadUInt32();
          this.NrSections = er.ReadUInt32();
          this.Padding = er.ReadBytes(16);
          OK = true;
        }
      }
    }

    public class INF1Section
    {
      public const string Signature = "INF1";
      public DataBlockHeader Header;
      public ushort Unknown1;
      public ushort Padding;
      public uint Unknown2;
      public uint NrVertex;
      public uint EntryOffset;
      public Inf1Entry[] Entries;

      public INF1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "INF1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Unknown1 = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt32();
          this.NrVertex = er.ReadUInt32();
          this.EntryOffset = er.ReadUInt32();
          long position2 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + (long) this.EntryOffset;
          List<Inf1Entry> source = new List<Inf1Entry>();
          source.Add(er.ReadNew<Inf1Entry>());
          while (source.Last<Inf1Entry>().Type != (ushort) 0)
            source.Add(er.ReadNew<Inf1Entry>());
          er.BaseStream.Position = position1 + (long) this.Header.size;
          this.Entries = source.ToArray();
          OK = true;
        }
      }
    }

    public partial class VTX1Section {
      public BMD.VTX1Section.Color[][] Colors = new BMD.VTX1Section.Color[2][];
      public BMD.VTX1Section.Texcoord[][] Texcoords = new BMD.VTX1Section.Texcoord[8][];
      public const string Signature = "VTX1";
      public DataBlockHeader Header;
      public uint ArrayFormatOffset;
      public uint[] Offsets;
      public ArrayFormat[] ArrayFormats;
      public Vector3[] Positions;
      public Vector3[] Normals;

      public VTX1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "VTX1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.ArrayFormatOffset = er.ReadUInt32();
          this.Offsets = er.ReadUInt32s(13);
          long position2 = er.BaseStream.Position;
          int length1 = 0;
          foreach (uint offset in this.Offsets)
          {
            if (offset != 0U)
              ++length1;
          }
          er.BaseStream.Position = position1 + (long) this.ArrayFormatOffset;
          this.ArrayFormats = new ArrayFormat[length1];
          for (int index = 0; index < length1; ++index) {
            this.ArrayFormats[index] = er.ReadNew<ArrayFormat>();
          }
          int index1 = 0;
          for (int k = 0; k < 13; ++k)
          {
            if (this.Offsets[k] != 0U)
            {
              ArrayFormat arrayFormat = this.ArrayFormats[index1];
              int length2 = this.GetLength(k);
              er.BaseStream.Position = position1 + (long) this.Offsets[k];
              if (arrayFormat.ArrayType >= 11 && arrayFormat.ArrayType <= 12) {
                this.ReadColorArray(arrayFormat, length2, er);
              } else {
                this.ReadVertexArray(arrayFormat, length2, er);
              }
              ++index1;
            }
          }
          er.BaseStream.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      private int GetLength(int k)
      {
        int offset = (int) this.Offsets[k];
        for (int index = k + 1; index < 13; ++index)
        {
          if (this.Offsets[index] != 0U)
            return (int) this.Offsets[index] - offset;
        }
        return (int) this.Header.size - offset;
      }

      private void ReadVertexArray(
          ArrayFormat Format,
          int Length,
          EndianBinaryReader er) {
        List<float> floatList = new List<float>();
        switch (Format.DataType) {
          case 3:
            float num1 = (float) Math.Pow(0.5, (double) Format.DecimalPoint);
            for (int index = 0; index < Length / 2; ++index)
              floatList.Add((float) er.ReadInt16() * num1);
            break;
          case 4:
            floatList.AddRange((IEnumerable<float>) er.ReadSingles(Length / 4));
            break;
          default:
            throw new NotImplementedException();
        }
        switch (Format.ArrayType) {
          case 9:
            switch (Format.ComponentCount) {
              case 0:
                this.Positions = new Vector3[floatList.Count / 2];
                for (int index = 0; index < floatList.Count - 1; index += 2)
                  this.Positions[index / 2] =
                      new Vector3(floatList[index], floatList[index + 1], 0.0f);
                return;
              case 1:
                this.Positions = new Vector3[floatList.Count / 3];
                for (int index = 0; index < floatList.Count - 2; index += 3)
                  this.Positions[index / 3] = new Vector3(
                      floatList[index],
                      floatList[index + 1],
                      floatList[index + 2]);
                return;
              default:
                return;
            }
          case 10:
            if (Format.ComponentCount != 0U)
              break;
            this.Normals = new Vector3[floatList.Count / 3];
            for (int index = 0; index < floatList.Count - 2; index += 3)
              this.Normals[index / 3] = new Vector3(
                  floatList[index],
                  floatList[index + 1],
                  floatList[index + 2]);
            break;
          case 13:
          case 14:
          case 15:
          case 16:
          case 17:
          case 18:
          case 19:
          case 20:
            uint num3 = Format.ArrayType - 13U;
            switch (Format.ComponentCount) {
              case 0:
                this.Texcoords[num3] =
                    new BMD.VTX1Section.Texcoord[floatList.Count];
                for (int index = 0; index < floatList.Count; ++index)
                  this.Texcoords[num3][index] =
                      new BMD.VTX1Section.Texcoord(floatList[index], 0.0f);
                return;
              case 1:
                this.Texcoords[num3] =
                    new BMD.VTX1Section.Texcoord[floatList.Count / 2];
                for (int index = 0; index < floatList.Count - 1; index += 2)
                  this.Texcoords[num3][index / 2] =
                      new BMD.VTX1Section.Texcoord(
                          floatList[index],
                          floatList[index + 1]);
                return;
              default:
                return;
            }
        }
      }

      private enum ColorDataType {
        RGB565 = 0,
        RGB8 = 1,
        RGBX8 = 2,
        RGBA4 = 3,
        RGBA6 = 4,
        RGBA8 = 5,
      }

      /// <summary>
      ///   Colors are a special case:
      ///   https://wiki.cloudmodding.com/tww/BMD_and_BDL#Data_Types
      /// </summary>
      private void ReadColorArray(
          ArrayFormat Format,
          int byteLength,
          EndianBinaryReader er) {
        uint num2 = Format.ArrayType - 11U;

        var colorDataType = (ColorDataType) Format.DataType;
        var expectedComponentCount = colorDataType switch {
            ColorDataType.RGB565 => 3,
            ColorDataType.RGB8   => 3,
            ColorDataType.RGBX8  => 4,
            ColorDataType.RGBA4  => 4,
            ColorDataType.RGBA6  => 4,
            ColorDataType.RGBA8  => 4,
            _                    => throw new ArgumentOutOfRangeException()
        };

        var actualComponentCount = (int) (3 + Format.ComponentCount);
        Asserts.Equal(expectedComponentCount, actualComponentCount);

        var colorCount = colorDataType switch {
            ColorDataType.RGB565 => byteLength / 2,
            ColorDataType.RGB8   => byteLength / 3,
            ColorDataType.RGBX8  => byteLength / 4,
            ColorDataType.RGBA4  => byteLength / 2,
            ColorDataType.RGBA6  => byteLength / 3,
            ColorDataType.RGBA8  => byteLength / 4,
            _                    => throw new ArgumentOutOfRangeException()
        };

        var colors = new Color[colorCount];
        for (var i = 0; i < colorCount; ++i) {
          Color color;
          switch (colorDataType) {
            case ColorDataType.RGB565: {
              ColorUtil.SplitRgb565(er.ReadUInt16(), out var r, out var g, out var b);
              color = new Color(r, g, b, 255);
              break;
            }
            case ColorDataType.RGB8: {
              color = new Color(er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte(),
                                255);
              break;
            }
            case ColorDataType.RGBX8: {
              color = new Color(er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte());
              break;
            }
            case ColorDataType.RGBA4: {
              throw new ArgumentOutOfRangeException();
            }
            case ColorDataType.RGBA6: {
              throw new ArgumentOutOfRangeException();
            }
            case ColorDataType.RGBA8: {
              color = new Color(er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte(),
                                er.ReadByte());
              break;
            }
            default: throw new ArgumentOutOfRangeException();
          }

          Asserts.Nonnull(color);

          colors[i] = color;
        }

        this.Colors[num2] = colors;
      }

      public class Color
      {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
          this.R = r;
          this.G = g;
          this.B = g;
          this.A = b;
        }

        public static implicit operator System.Drawing.Color(BMD.VTX1Section.Color c)
        {
          return System.Drawing.Color.FromArgb((int) c.A, (int) c.R, (int) c.G, (int) c.B);
        }
      }

      public class Texcoord
      {
        public float S;
        public float T;

        public Texcoord(float s, float t)
        {
          this.S = s;
          this.T = t;
        }
      }
    }

    public class EVP1Section
    {
      public const string Signature = "EVP1";
      public DataBlockHeader Header;
      public ushort Count;
      public ushort Padding;
      public uint[] Offsets;
      public byte[] Counts;
      public MTX44[] InverseBindMatrices;
      public BMD.EVP1Section.MultiMatrix[] WeightedIndices;

      public EVP1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "EVP1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Count = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.Offsets = er.ReadUInt32s(4);
          long position2 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + (long) this.Offsets[0];
          this.Counts = er.ReadBytes((int) this.Count);

          er.BaseStream.Position = position1 + (long) this.Offsets[1];
          this.WeightedIndices = new BMD.EVP1Section.MultiMatrix[(int) this.Count];
          int val1 = 0;
          for (int index1 = 0; index1 < (int) this.Count; ++index1)
          {
            this.WeightedIndices[index1] = new BMD.EVP1Section.MultiMatrix();
            this.WeightedIndices[index1].Indices = new ushort[(int) this.Counts[index1]];
            for (int index2 = 0; index2 < (int) this.Counts[index1]; ++index2)
            {
              this.WeightedIndices[index1].Indices[index2] = er.ReadUInt16();
              val1 = Math.Max(val1, (int) this.WeightedIndices[index1].Indices[index2] + 1);
            }
          }
          
          er.BaseStream.Position = position1 + (long) this.Offsets[2];
          for (int index1 = 0; index1 < (int) this.Count; ++index1)
          {
            this.WeightedIndices[index1].Weights = new float[(int) this.Counts[index1]];
            for (int index2 = 0; index2 < (int) this.Counts[index1]; ++index2)
              this.WeightedIndices[index1].Weights[index2] = er.ReadSingle();
          }

          er.BaseStream.Position = position1 + (long) this.Offsets[3];
          this.InverseBindMatrices = new MTX44[val1];
          for (int index = 0; index < val1; ++index)
          {
            float[] array = er.ReadSingles(12);
            Array.Resize<float>(ref array, 16);
            array[15] = 1f;
            this.InverseBindMatrices[index] = (MTX44) array;
          }
          er.BaseStream.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public class MultiMatrix
      {
        public float[] Weights;
        public ushort[] Indices;
      }
    }

    public class DRW1Section
    {
      public const string Signature = "DRW1";
      public DataBlockHeader Header;
      public ushort Count;
      public ushort Padding;
      public uint IsWeightedOffset;
      public uint DataOffset;
      public bool[] IsWeighted;
      public ushort[] Data;

      public DRW1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "DRW1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.Count = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.IsWeightedOffset = er.ReadUInt32();
          this.DataOffset = er.ReadUInt32();

          er.BaseStream.Position = position1 + (long) this.IsWeightedOffset;
          this.IsWeighted = new bool[(int) this.Count];
          for (int index = 0; index < (int) this.Count; ++index)
            this.IsWeighted[index] = er.ReadByte() == (byte) 1;
          
          er.BaseStream.Position = position1 + (long) this.DataOffset;
          this.Data = er.ReadUInt16s((int) this.Count);

          er.BaseStream.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }
    }

    public partial class JNT1Section {
      public const string Signature = "JNT1";
      public DataBlockHeader Header;
      public ushort NrJoints;
      public ushort Padding;
      public uint JointEntryOffset;
      public uint UnknownOffset;
      public uint StringTableOffset;
      public BMD.JNT1Section.JNT1Entry[] Joints;
      public BMD.Stringtable StringTable;

      public JNT1Section(EndianBinaryReader er, out bool OK)
      {
        long position = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "JNT1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrJoints = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.JointEntryOffset = er.ReadUInt32();
          this.UnknownOffset = er.ReadUInt32();
          this.StringTableOffset = er.ReadUInt32();
          er.BaseStream.Position = position + (long) this.StringTableOffset;
          this.StringTable = new BMD.Stringtable(er);
          er.BaseStream.Position = position + (long) this.JointEntryOffset;
          this.Joints = new BMD.JNT1Section.JNT1Entry[(int) this.NrJoints];
          for (var index = 0; index < (int) this.NrJoints; ++index) {
            this.Joints[index] = er.ReadNew<JNT1Entry>();
          }
          er.BaseStream.Position = position + (long) this.Header.size;
          OK = true;
        }
      }

      [Schema]
      public partial class JNT1Entry : IDeserializable {
        public ushort Unknown1;
        public byte Unknown2;
        public byte Padding1;
        public float Sx;
        public float Sy;
        public float Sz;
        public short Rx;
        public short Ry;
        public short Rz;
        public ushort Padding2;
        public float Tx;
        public float Ty;
        public float Tz;
        public float Unknown3;
        public readonly float[] BoundingBoxMin = new float[3];
        public readonly float[] BoundingBoxMax = new float[3];
      }
    }

    public partial class SHP1Section {
      public const string Signature = "SHP1";
      public DataBlockHeader Header;
      public ushort NrBatch;
      public ushort Padding;
      public uint BatchesOffset;
      public uint UnknownOffset;
      public uint Zero;
      public uint BatchAttribsOffset;
      public uint MatrixTableOffset;
      public uint DataOffset;
      public uint MatrixDataOffset;
      public uint PacketLocationsOffset;
      public BMD.SHP1Section.Batch[] Batches;

      public SHP1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "SHP1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrBatch = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.BatchesOffset = er.ReadUInt32();
          this.UnknownOffset = er.ReadUInt32();
          this.Zero = er.ReadUInt32();
          this.BatchAttribsOffset = er.ReadUInt32();
          this.MatrixTableOffset = er.ReadUInt32();
          this.DataOffset = er.ReadUInt32();
          this.MatrixDataOffset = er.ReadUInt32();
          this.PacketLocationsOffset = er.ReadUInt32();
          long position2 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + (long) this.BatchesOffset;
          this.Batches = new BMD.SHP1Section.Batch[(int) this.NrBatch];
          for (int index = 0; index < (int) this.NrBatch; ++index)
            this.Batches[index] = new BMD.SHP1Section.Batch(er, position1, this);
          er.BaseStream.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public partial class Batch {
        public bool[] HasColors = new bool[2];
        public bool[] HasTexCoords = new bool[8];
        public byte MatrixType;
        public byte Unknown1;
        public ushort NrPacket;
        public ushort AttribsOffset;
        public ushort FirstMatrixData;
        public ushort FirstPacketLocation;
        public ushort Unknown2;
        public float Unknown3;
        public float[] BoundingBoxMin;
        public float[] BoundingBoxMax;
        public BatchAttribute[] BatchAttributes;
        public bool HasMatrixIndices;
        public bool HasPositions;
        public bool HasNormals;
        public PacketLocation[] PacketLocations;
        public BMD.SHP1Section.Batch.Packet[] Packets;

        public Batch(
            EndianBinaryReader er,
            long baseoffset,
            BMD.SHP1Section Parent) {
          this.MatrixType = er.ReadByte();
          this.Unknown1 = er.ReadByte();
          this.NrPacket = er.ReadUInt16();
          this.AttribsOffset = er.ReadUInt16();
          this.FirstMatrixData = er.ReadUInt16();
          this.FirstPacketLocation = er.ReadUInt16();
          this.Unknown2 = er.ReadUInt16();
          this.Unknown3 = er.ReadSingle();
          this.BoundingBoxMin = er.ReadSingles(3);
          this.BoundingBoxMax = er.ReadSingles(3);
          long position = er.BaseStream.Position;
          er.BaseStream.Position = baseoffset +
                                   (long) Parent.BatchAttribsOffset +
                                   (long) this.AttribsOffset;
          List<BatchAttribute> source = new List<BatchAttribute>();
          {
            var entry = er.ReadNew<BatchAttribute>();
            source.Add(entry);

            while (source.Last<BatchAttribute>()
                         .Attribute !=
                   (uint) byte.MaxValue) {
              entry = new BatchAttribute();
              entry.Read(er);
              source.Add(entry);
            }
          }

        source.Remove(source.Last<BatchAttribute>());
          this.BatchAttributes = source.ToArray();
          for (int index = 0; index < this.BatchAttributes.Length; ++index)
          {
            if (this.BatchAttributes[index].DataType != 1U && this.BatchAttributes[index].DataType != 3U)
              throw new Exception();
            switch (this.BatchAttributes[index].Attribute)
            {
              case 0:
                this.HasMatrixIndices = true;
                break;
              case 9:
                this.HasPositions = true;
                break;
              case 10:
                this.HasNormals = true;
                break;
              case 11:
              case 12:
                this.HasColors[this.BatchAttributes[index].Attribute - 11U] = true;
                break;
              case 13:
              case 14:
              case 15:
              case 16:
              case 17:
              case 18:
              case 19:
              case 20:
                this.HasTexCoords[this.BatchAttributes[index].Attribute - 13U] = true;
                break;
            }
          }
          this.Packets = new BMD.SHP1Section.Batch.Packet[(int) this.NrPacket];
          this.PacketLocations = new PacketLocation[(int) this.NrPacket];
          for (int index = 0; index < (int) this.NrPacket; ++index)
          {
            er.BaseStream.Position = baseoffset + (long) Parent.PacketLocationsOffset + (long) (((int) this.FirstPacketLocation + index) * 8);
            var packetLocation = new PacketLocation();
            packetLocation.Read(er);
            this.PacketLocations[index] = packetLocation;

            er.BaseStream.Position = baseoffset + (long) Parent.DataOffset + (long) this.PacketLocations[index].Offset;
            this.Packets[index] = new BMD.SHP1Section.Batch.Packet(er, (int) this.PacketLocations[index].Size, this.BatchAttributes);
            er.BaseStream.Position = baseoffset + (long) Parent.MatrixDataOffset + (long) (((int) this.FirstMatrixData + index) * 8);
            this.Packets[index].MatrixData = new BMD.SHP1Section.Batch.Packet.Matrixdata(er);
            er.BaseStream.Position = baseoffset + (long) Parent.MatrixTableOffset + (long) (2U * this.Packets[index].MatrixData.FirstIndex);
            this.Packets[index].MatrixTable = er.ReadUInt16s((int) this.Packets[index].MatrixData.Count);
          }
          er.BaseStream.Position = position;
        }

        public class Packet
        {
          public BMD.SHP1Section.Batch.Packet.Primitive[] Primitives;
          public ushort[] MatrixTable;
          public BMD.SHP1Section.Batch.Packet.Matrixdata MatrixData;

          public Packet(
            EndianBinaryReader er,
            int Length,
            BatchAttribute[] Attributes)
          {
            List<BMD.SHP1Section.Batch.Packet.Primitive> primitiveList = new List<BMD.SHP1Section.Batch.Packet.Primitive>();
            bool flag = false;
            int num1 = 0;
            while (!flag)
            {
              BMD.SHP1Section.Batch.Packet.Primitive primitive = new BMD.SHP1Section.Batch.Packet.Primitive();
              primitive.Type = (BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive) er.ReadByte();
              ++num1;
              if (primitive.Type == (BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive) 0 || num1 >= Length)
              {
                flag = true;
              }
              else
              {
                ushort num2 = er.ReadUInt16();
                num1 += 2;
                primitive.Points = new BMD.SHP1Section.Batch.Packet.Primitive.Index[(int) num2];
                for (int index1 = 0; index1 < (int) num2; ++index1)
                {
                  primitive.Points[index1] = new BMD.SHP1Section.Batch.Packet.Primitive.Index();
                  for (int index2 = 0; index2 < Attributes.Length; ++index2)
                  {
                    ushort num3 = 0;
                    switch (Attributes[index2].DataType)
                    {
                      case 1:
                        num3 = (ushort) er.ReadByte();
                        ++num1;
                        break;
                      case 3:
                        num3 = er.ReadUInt16();
                        num1 += 2;
                        break;
                    }
                    switch (Attributes[index2].Attribute)
                    {
                      case 0:
                        primitive.Points[index1].MatrixIndex = num3;
                        break;
                      case 9:
                        primitive.Points[index1].PosIndex = num3;
                        break;
                      case 10:
                        primitive.Points[index1].NormalIndex = num3;
                        break;
                      case 11:
                      case 12:
                        primitive.Points[index1].ColorIndex[(Attributes[index2].Attribute - 11U)] = num3;
                        break;
                      case 13:
                      case 14:
                      case 15:
                      case 16:
                      case 17:
                      case 18:
                      case 19:
                      case 20:
                        primitive.Points[index1].TexCoordIndex[(Attributes[index2].Attribute - 13U)] = num3;
                        break;
                    }
                  }
                }
                primitiveList.Add(primitive);
              }
            }
            this.Primitives = primitiveList.ToArray();
          }

          public class Primitive
          {
            public BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive Type;
            public BMD.SHP1Section.Batch.Packet.Primitive.Index[] Points;

            public int GetGlPrimitive()
            {
              switch (this.Type)
              {
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_QUADS:
                  return 7;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLES:
                  return 4;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLESTRIP:
                  return 5;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_TRIANGLEFAN:
                  return 6;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_LINES:
                  return 1;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_LINESTRIP:
                  return 3;
                case BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive.GX_POINTS:
                  return 0;
                default:
                  return -1;
              }
            }

            public enum GXPrimitive
            {
              GX_QUADS = 128, // 0x00000080
              GX_TRIANGLES = 144, // 0x00000090
              GX_TRIANGLESTRIP = 152, // 0x00000098
              GX_TRIANGLEFAN = 160, // 0x000000A0
              GX_LINES = 168, // 0x000000A8
              GX_LINESTRIP = 176, // 0x000000B0
              GX_POINTS = 184, // 0x000000B8
            }

            public class Index
            {
              public ushort[] ColorIndex = new ushort[2];
              public ushort[] TexCoordIndex = new ushort[8];
              public ushort MatrixIndex;
              public ushort PosIndex;
              public ushort NormalIndex;
            }
          }

          public class Matrixdata
          {
            public ushort Unknown;
            public ushort Count;
            public uint FirstIndex;

            public Matrixdata(EndianBinaryReader er)
            {
              this.Unknown = er.ReadUInt16();
              this.Count = er.ReadUInt16();
              this.FirstIndex = er.ReadUInt32();
            }
          }
        }
      }
    }

    public enum CullMode {
      None = 0,  // Do not cull any primitives
      Front = 1, // Cull front-facing primitives
      Back = 2,  // Cull back-facing primitives
      All = 3    // Cull all primitives
    }

    public enum GxKonstColorSel {
      KCSel_1 = 0x00,     // Constant 1.0
      KCSel_7_8 = 0x01,   // Constant 7/8
      KCSel_3_4 = 0x02,   // Constant 3/4
      KCSel_5_8 = 0x03,   // Constant 5/8
      KCSel_1_2 = 0x04,   // Constant 1/2
      KCSel_3_8 = 0x05,   // Constant 3/8
      KCSel_1_4 = 0x06,   // Constant 1/4
      KCSel_1_8 = 0x07,   // Constant 1/8
      KCSel_K0 = 0x0C,    // K0[RGB] Register
      KCSel_K1 = 0x0D,    // K1[RGB] Register
      KCSel_K2 = 0x0E,    // K2[RGB] Register
      KCSel_K3 = 0x0F,    // K3[RGB] Register
      KCSel_K0_R = 0x10,  // K0[RRR] Register
      KCSel_K1_R = 0x11,  // K1[RRR] Register
      KCSel_K2_R = 0x12,  // K2[RRR] Register
      KCSel_K3_R = 0x13,  // K3[RRR] Register
      KCSel_K0_G = 0x14,  // K0[GGG] Register
      KCSel_K1_G = 0x15,  // K1[GGG] Register
      KCSel_K2_G = 0x16,  // K2[GGG] Register
      KCSel_K3_G = 0x17,  // K3[GGG] Register
      KCSel_K0_B = 0x18,  // K0[BBB] Register
      KCSel_K1_B = 0x19,  // K1[BBB] Register
      KCSel_K2_B = 0x1A,  // K2[BBB] Register
      KCSel_K3_B = 0x1B,  // K3[BBB] Register
      KCSel_K0_A = 0x1C,  // K0[AAA] Register
      KCSel_K1_A = 0x1D,  // K1[AAA] Register
      KCSel_K2_A = 0x1E,  // K2[AAA] Register
      KCSel_K3_A = 0x1F   // K3[AAA] Register
    }

    public enum GxKonstAlphaSel {
      KASel_1 = 0x00,     // Constant 1.0
      KASel_7_8 = 0x01,   // Constant 7/8
      KASel_3_4 = 0x02,   // Constant 3/4
      KASel_5_8 = 0x03,   // Constant 5/8
      KASel_1_2 = 0x04,   // Constant 1/2
      KASel_3_8 = 0x05,   // Constant 3/8
      KASel_1_4 = 0x06,   // Constant 1/4
      KASel_1_8 = 0x07,   // Constant 1/8
      KASel_K0_R = 0x10,  // K0[R] Register
      KASel_K1_R = 0x11,  // K1[R] Register
      KASel_K2_R = 0x12,  // K2[R] Register
      KASel_K3_R = 0x13,  // K3[R] Register
      KASel_K0_G = 0x14,  // K0[G] Register
      KASel_K1_G = 0x15,  // K1[G] Register
      KASel_K2_G = 0x16,  // K2[G] Register
      KASel_K3_G = 0x17,  // K3[G] Register
      KASel_K0_B = 0x18,  // K0[B] Register
      KASel_K1_B = 0x19,  // K1[B] Register
      KASel_K2_B = 0x1A,  // K2[B] Register
      KASel_K3_B = 0x1B,  // K3[B] Register
      KASel_K0_A = 0x1C,  // K0[A] Register
      KASel_K1_A = 0x1D,  // K1[A] Register
      KASel_K2_A = 0x1E,  // K2[A] Register
      KASel_K3_A = 0x1F   // K3[A] Register
    }



    public partial class MAT3Section {
      public const string Signature = "MAT3";
      public DataBlockHeader Header;
      public ushort NrMaterials;
      public uint[] Offsets;
      public BMD.MAT3Section.MaterialEntry[] MaterialEntries;
      public BMD.MAT3Section.PopulatedMaterial[] PopulatedMaterials;
      public ushort[] MaterialEntryIndieces;
      public short[] TextureIndices;
      public CullMode[] CullModes;
      public System.Drawing.Color[] MaterialColor;
      public System.Drawing.Color[] AmbientColors;
      public System.Drawing.Color[] ColorS10;
      public System.Drawing.Color[] Color3;
      public AlphaCompare[] AlphaCompares;
      public BlendFunction[] BlendFunctions;
      public DepthFunction[] DepthFunctions;
      public BMD.MAT3Section.TevStageProps[] TevStages;
      public IList<TexCoordGen> TexCoordGens;
      public TextureMatrixInfo[] TextureMatrices;
      public BMD.MAT3Section.TevOrder[] TevOrders;
      public BMD.Stringtable MaterialNameTable;

      public MAT3Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "MAT3", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrMaterials = er.ReadUInt16();

          er.AssertUInt16(0xffff); // padding

          this.Offsets = er.ReadUInt32s(30);
          int[] sectionLengths = this.GetSectionLengths();
          long position2 = er.BaseStream.Position;

          // TODO: There is a bunch more data that isn't even read yet:
          // https://github.com/RenolY2/SuperBMD/blob/ccc86e21493275bcd9d86f65b516b85d95c83abd/SuperBMDLib/source/Materials/Enums/Mat3OffsetIndex.cs

          er.BaseStream.Position = position1 + (long) this.Offsets[0];
          this.MaterialEntries = new BMD.MAT3Section.MaterialEntry[sectionLengths[0] / 332];
          for (int index = 0; index < sectionLengths[0] / 332; ++index)
            this.MaterialEntries[index] = new BMD.MAT3Section.MaterialEntry(er);
          
          er.BaseStream.Position = position1 + (long) this.Offsets[1];
          this.MaterialEntryIndieces = er.ReadUInt16s((int) this.NrMaterials);
          
          er.BaseStream.Position = position1 + (long) this.Offsets[2];
          this.MaterialNameTable = new BMD.Stringtable(er);

          // TODO: Add support for indirect textures (3)

          er.BaseStream.Position = position1 + (long)this.Offsets[4];
          this.CullModes = new CullMode[sectionLengths[4] / 4];
          for (var index = 0; index < sectionLengths[4] / 4; ++index)
            this.CullModes[index] = (CullMode) er.ReadInt32();

          er.BaseStream.Position = position1 + (long) this.Offsets[5];
          this.MaterialColor = new System.Drawing.Color[sectionLengths[5] / 4];
          for (int index = 0; index < sectionLengths[5] / 4; ++index)
            this.MaterialColor[index] = er.ReadColor8();

          // TODO: Add support for color channel info (7)

          er.BaseStream.Position = position1 + (long) this.Offsets[8];
          this.AmbientColors = new System.Drawing.Color[sectionLengths[8] / 4];
          for (int index = 0; index < sectionLengths[8] / 4; ++index)
            this.AmbientColors[index] = er.ReadColor8();

          // TODO: Add support for light colors (9)
          // TODO: Add support for texgen counts (10)

          er.BaseStream.Position = position1 + this.Offsets[11];
          this.TexCoordGens = new List<TexCoordGen>();
          for (int index = 0; index < sectionLengths[11] / 4; ++index) {
            this.TexCoordGens.Add(er.ReadNew<TexCoordGen>());
          }
 
          // TODO: Add support for post tex coord gens (12)

          er.BaseStream.Position = position1 + (long) this.Offsets[13];
          this.TextureMatrices = new TextureMatrixInfo[sectionLengths[13] / 100];
          for (int index = 0; index < sectionLengths[13] / 100; ++index) {
            this.TextureMatrices[index] = er.ReadNew<TextureMatrixInfo>();
          }

          // TODO: Add support for post tex matrices (14)

          er.BaseStream.Position = position1 + (long) this.Offsets[15];
          this.TextureIndices = er.ReadInt16s(sectionLengths[15] / 2);

          er.BaseStream.Position = position1 + (long) this.Offsets[16];
          this.TevOrders = new BMD.MAT3Section.TevOrder[sectionLengths[16] / 4];
          for (int index = 0; index < sectionLengths[16] / 4; ++index) {
            this.TevOrders[index] = er.ReadNew<TevOrder>();
          }
          
          er.BaseStream.Position = position1 + (long) this.Offsets[17];
          this.ColorS10 = new System.Drawing.Color[sectionLengths[17] / 8];
          for (int index = 0; index < sectionLengths[17] / 8; ++index)
            this.ColorS10[index] = er.ReadColor16();
          
          er.BaseStream.Position = position1 + (long) this.Offsets[18];
          this.Color3 = new System.Drawing.Color[sectionLengths[18] / 4];
          for (int index = 0; index < sectionLengths[18] / 4; ++index)
            this.Color3[index] = er.ReadColor8();

          // TODO: Add support for tev counts (19)

          er.BaseStream.Position = position1 + (long) this.Offsets[20];
          this.TevStages = new BMD.MAT3Section.TevStageProps[sectionLengths[20] / 20];
          for (int index = 0; index < sectionLengths[20] / 20; ++index)
            this.TevStages[index] = new BMD.MAT3Section.TevStageProps(er);

          // TODO: Add support for tev swap modes (21)
          // TODO: Add support for tev swap mode table (22)
          // TODO: Add support for fog modes (23)

          er.BaseStream.Position = position1 + (long) this.Offsets[24];
          this.AlphaCompares = new AlphaCompare[sectionLengths[24] / 8];
          for (int index = 0; index < sectionLengths[24] / 8; ++index) {
            this.AlphaCompares[index] = er.ReadNew<AlphaCompare>();
          }

          er.BaseStream.Position = position1 + (long) this.Offsets[25];
          this.BlendFunctions = new BlendFunction[sectionLengths[25] / 4];
          for (int index = 0; index < sectionLengths[25] / 4; ++index) {
            this.BlendFunctions[index] = er.ReadNew<BlendFunction>();
          }
          er.BaseStream.Position = position1 + (long) this.Offsets[26];
          this.DepthFunctions = new DepthFunction[sectionLengths[26] / 4];
          for (int index = 0; index < sectionLengths[26] / 4; ++index) {
            this.DepthFunctions[index] = er.ReadNew<DepthFunction>();
          }
          er.BaseStream.Position = position1 + (long) this.Header.size;
          OK = true;

          // TODO: Add support for nbt scale (29)

          this.PopulatedMaterials = this.MaterialEntries.Select((entry, index) => new PopulatedMaterial(this, index, entry)).ToArray();
        }
      }

      public BMDShader GetShader(int MatIdx)
      {
        List<BMD.MAT3Section.TevStageProps> source = new List<BMD.MAT3Section.TevStageProps>();
        BMD.MAT3Section.MaterialEntry materialEntry = this.MaterialEntries[MatIdx];
        for (int index = 0; index < 16; ++index)
        {
          if (materialEntry.TevStageInfoIndexes[index] != ushort.MaxValue)
          {
            source.Add(this.TevStages[(int) materialEntry.TevStageInfoIndexes[index]]);
            source.Last<BMD.MAT3Section.TevStageProps>().alpha_constant_sel = materialEntry.KonstAlphaSel[index];
            source.Last<BMD.MAT3Section.TevStageProps>().color_constant_sel = materialEntry.KonstColorSel[index];
            source.Last<BMD.MAT3Section.TevStageProps>().texcoord = this.TevOrders[(int) materialEntry.TevOrderInfoIndexes[index]].TexCoordId;
            source.Last<BMD.MAT3Section.TevStageProps>().texmap = this.TevOrders[(int) materialEntry.TevOrderInfoIndexes[index]].TexMap;
          }
        }
        List<int> intList = new List<int>();
        for (int index = 0; index < 8; ++index)
        {
          if (materialEntry.TextureIndexes[index] != ushort.MaxValue)
            intList.Add((int) this.TextureIndices[(int) materialEntry.TextureIndexes[index]]);
        }
        return new BMDShader(source.ToArray(), intList.ToArray(), new System.Drawing.Color[3]
        {
          this.ColorS10[(int) materialEntry.TevColorIndexes[1]],
          this.ColorS10[(int) materialEntry.TevColorIndexes[2]],
          this.ColorS10[(int) materialEntry.TevColorIndexes[3]]
        }, new System.Drawing.Color[4]
        {
          this.Color3[(int) materialEntry.TevKonstColorIndexes[0]],
          this.Color3[(int) materialEntry.TevKonstColorIndexes[1]],
          this.Color3[(int) materialEntry.TevKonstColorIndexes[2]],
          this.Color3[(int) materialEntry.TevKonstColorIndexes[3]]
        }, (byte) 1, (byte) 1, this.AmbientColors[(int) materialEntry.AmbientColorIndexes[0]], this.AlphaCompares[(int) materialEntry.AlphaCompareIndex]);
      }

      public void glAlphaCompareglBendMode(int idxa, int idxb, int idxd)
      {
        if (this.DepthFunctions[idxd].Enable == (byte) 1)
        {
          Gl.glEnable(2929);
          Gl.glDepthFunc(this.GetGlAlphaFunc((int) this.DepthFunctions[idxd].Func));
          Gl.glDepthMask((int) this.DepthFunctions[idxd].UpdateEnable);
        }
        else
          Gl.glDisable(2929);
        if (this.BlendFunctions[idxb].BlendMode == BmdBlendMode.NONE)
        {
          Gl.glDisable(3042);
        }
        else
        {
          Gl.glEnable(3042);
          Gl.glBlendEquation((int) Blending.BmdToGl(this.BlendFunctions[idxb].BlendMode));
          Gl.glBlendFunc((int) BlendFactor.BmdToGl(this.BlendFunctions[idxb].SrcFactor), (int)BlendFactor.BmdToGl(this.BlendFunctions[idxb].DstFactor));
          Gl.glLogicOp((int) LogicOp.BmdToGl(this.BlendFunctions[idxb].LogicOp));
        }
      }

      private int GetGlAlphaFunc(int func)
      {
        return new int[8]
        {
          512,
          513,
          514,
          515,
          516,
          517,
          518,
          519
        }[func];
      }


      public int[] GetSectionLengths()
      {
        int[] numArray = new int[30];
        for (int index1 = 0; index1 < 30; ++index1)
        {
          int num1 = 0;
          if (this.Offsets[index1] != 0U)
          {
            int num2 = (int) this.Header.size;
            for (int index2 = index1 + 1; index2 < 30; ++index2)
            {
              if (this.Offsets[index2] != 0U)
              {
                num2 = (int) this.Offsets[index2];
                break;
              }
            }
            num1 = num2 - (int) this.Offsets[index1];
          }
          numArray[index1] = num1;
        }
        return numArray;
      }

      public class MaterialEntry
      {
        public byte Flag;
        public byte CullModeIndex;
        public byte ColorChannelControlsCountIndex;
        public byte TexGensCountIndex;
        public byte TevStagesCountIndex;
        public byte ZCompLocIndex;
        public byte ZModeIndex;
        public byte DitherIndex;

        public ushort[] MaterialColorIndexes;
        public ushort[] ColorChannelControlIndexes;
        public ushort[] AmbientColorIndexes;
        public ushort[] LightColorIndexes;

        public ushort[] TexGenInfo;
        public TexGenType[] TexGenTypes;
        
        public ushort[] TexGenInfo2;
        public ushort[] TexMatrices;
        public ushort[] DttMatrices;
        public short[] TextureIndexes;
        public ushort[] TevKonstColorIndexes;
        public GxKonstColorSel[] KonstColorSel;
        public GxKonstAlphaSel[] KonstAlphaSel;
        public short[] TevOrderInfoIndexes;
        public ushort[] TevColorIndexes;
        public short[] TevStageInfoIndexes;
        public ushort[] TevSwapModeInfo;
        public ushort[] TevSwapModeTable;
        public ushort[] Unknown2;
        public short FogInfoIndex;
        public short AlphaCompareIndex;
        public short BlendModeIndex;
        public short UnknownIndex;

        // https://github.com/LordNed/WindEditor/wiki/BMD-and-BDL-Model-Format#material-entry
        public MaterialEntry(EndianBinaryReader er) {
          this.Flag = er.ReadByte();
          this.CullModeIndex = er.ReadByte();
          this.ColorChannelControlsCountIndex = er.ReadByte();
          this.TexGensCountIndex = er.ReadByte();
          this.TevStagesCountIndex = er.ReadByte();
          this.ZCompLocIndex = er.ReadByte();
          this.ZModeIndex = er.ReadByte();
          this.DitherIndex = er.ReadByte();
          
          this.MaterialColorIndexes = er.ReadUInt16s(2);
          this.ColorChannelControlIndexes = er.ReadUInt16s(4);
          this.AmbientColorIndexes = er.ReadUInt16s(2);
          this.LightColorIndexes = er.ReadUInt16s(8);
          
          this.TexGenInfo = er.ReadUInt16s(8);
          this.TexGenTypes =
              this.TexGenInfo
                  .Select(texGenType
                              => texGenType != 65535
                                     ? (TexGenType) texGenType
                                     : TexGenType.UNDEFINED)
                  .ToArray();

          this.TexGenInfo2 = er.ReadUInt16s(8);
          this.TexMatrices = er.ReadUInt16s(10);
          this.DttMatrices = er.ReadUInt16s(20);
          this.TextureIndexes = er.ReadInt16s(8);
          this.TevKonstColorIndexes = er.ReadUInt16s(4);
          this.KonstColorSel =
              er.ReadBytes(16)
                .Select(konstColor => (GxKonstColorSel) konstColor)
                .ToArray();
          this.KonstAlphaSel =
              er.ReadBytes(16)
                .Select(konstAlpha => (GxKonstAlphaSel) konstAlpha)
                .ToArray();
          this.TevOrderInfoIndexes = er.ReadInt16s(16);
          this.TevColorIndexes = er.ReadUInt16s(4);
          this.TevStageInfoIndexes = er.ReadInt16s(16);
          this.TevSwapModeInfo = er.ReadUInt16s(16);
          this.TevSwapModeTable = er.ReadUInt16s(4);
          this.Unknown2 = er.ReadUInt16s(12);
          this.FogInfoIndex = er.ReadInt16();
          this.AlphaCompareIndex = er.ReadInt16();
          this.BlendModeIndex = er.ReadInt16();
          this.UnknownIndex = er.ReadInt16();
        }

        public enum TexGenType {
          GX_TG_MTX3x4,
          GX_TG_MTX2x4,
          GX_TG_BUMP0,
          GX_TG_BUMP1,
          GX_TG_BUMP2,
          GX_TG_BUMP3,
          GX_TG_BUMP4,
          GX_TG_BUMP5,
          GX_TG_BUMP6,
          GX_TG_BUMP7,
          GX_TG_SRTG,
          UNDEFINED,
      }
      }

      public class PopulatedMaterial {
        public string Name;
        public byte Flag;
        public CullMode CullMode;
        public byte ColorChannelControlsCountIndex;
        public byte TexGensCountIndex;
        public byte TevStagesCountIndex;
        public byte ZCompLocIndex;
        public byte ZModeIndex;
        public byte DitherIndex;

        public Color[] MaterialColors;
        public ushort[] ColorChannelControlIndexes;
        public Color[] AmbientColors;
        public ushort[] LightColorIndexes;

        public ushort[] TexGenInfo;
        public MaterialEntry.TexGenType[] TexGenTypes;

        public ushort[] TexGenInfo2;
        public ushort[] TexMatrices;
        public ushort[] DttMatrices;
        public short[] TextureIndices;
        public ushort[] TevKonstColorIndexes;
        public byte[] ConstColorSel;
        public byte[] ConstAlphaSel;

        public TevOrder[] TevOrderInfos;

        public ushort[] TevOrderInfoIndexes;
        public ushort[] TevColorIndexes;
        public TevStageProps[] TevStageInfos;
        public ushort[] TevSwapModeInfo;
        public ushort[] TevSwapModeTable;
        public ushort[] Unknown2;
        public short FogInfoIndex;
        public AlphaCompare AlphaCompare;
        public BlendFunction BlendMode;
        public short UnknownIndex;

        public PopulatedMaterial(MAT3Section mat3, int index, MaterialEntry entry) {
          this.Name = mat3.MaterialNameTable[index];
          this.Flag = entry.Flag;

          this.CullMode = mat3.CullModes[entry.CullModeIndex];

          this.MaterialColors =
              entry.MaterialColorIndexes
                   .Select(i => GetOrNull(mat3.MaterialColor, i))
                   .ToArray();
          this.AmbientColors =
              entry.AmbientColorIndexes
                   .Select(i => GetOrNull(mat3.AmbientColors, i))
                   .ToArray();

          this.TevOrderInfos =
              entry.TevOrderInfoIndexes
                   .Select(i => GetOrNull(mat3.TevOrders, i))
                   .ToArray();

          this.TevStageInfos = 
              entry.TevStageInfoIndexes
                   .Select(i => GetOrNull(mat3.TevStages, i))
                   .ToArray();

          this.TextureIndices =
              entry.TextureIndexes
                   .Select(t => (short) (t != -1 ? mat3.TextureIndices[t] : -1))
                   .ToArray();

          this.AlphaCompare = mat3.AlphaCompares[entry.AlphaCompareIndex];
          this.BlendMode = mat3.BlendFunctions[entry.BlendModeIndex];

          if (this.Name == "eye1") {
            ;
          }
        }

        private static T? GetOrNull<T>(IList<T> array, int i)
            where T : notnull
          => i != -1 ? array[i] : default;
      }

      public class TevStageProps {
        public GxCc color_a;
        public GxCc color_b;
        public GxCc color_c;
        public GxCc color_d;
        public GxCa alpha_a;
        public GxCa alpha_b;
        public GxCa alpha_c;
        public GxCa alpha_d;
        public TevOp color_op;
        public TevOp alpha_op;
        public ColorRegister color_regid;
        public ColorRegister alpha_regid;
        public byte pad;
        public byte texcoord;
        public sbyte texmap;
        public GxKonstColorSel color_constant_sel;
        public GxKonstAlphaSel alpha_constant_sel;
        public TevBias color_bias;
        public TevScale color_scale;
        public TevBias alpha_bias;
        public TevScale alpha_scale;
        public bool color_clamp;
        public bool alpha_clamp;

        public TevStageProps(EndianBinaryReader er) {
          er.ReadByte();
          this.color_a = (GxCc) er.ReadByte();
          this.color_b = (GxCc) er.ReadByte();
          this.color_c = (GxCc) er.ReadByte();
          this.color_d = (GxCc) er.ReadByte();
          this.color_op = (TevOp) er.ReadByte();
          this.color_bias = (TevBias) er.ReadByte();
          this.color_scale = (TevScale) er.ReadByte();
          this.color_clamp = er.ReadByte() == (byte) 1;
          this.color_regid = (ColorRegister) er.ReadByte();
          this.alpha_a = (GxCa) er.ReadByte();
          this.alpha_b = (GxCa) er.ReadByte();
          this.alpha_c = (GxCa) er.ReadByte();
          this.alpha_d = (GxCa) er.ReadByte();
          this.alpha_op = (TevOp) er.ReadByte();
          this.alpha_bias = (TevBias) er.ReadByte();
          this.alpha_scale = (TevScale) er.ReadByte();
          this.alpha_clamp = er.ReadByte() == (byte) 1;
          this.alpha_regid = (ColorRegister) er.ReadByte();
          er.ReadByte();
        }

        public enum GxCc {
          GX_CC_CPREV,
          GX_CC_APREV,
          GX_CC_C0,
          GX_CC_A0,
          GX_CC_C1,
          GX_CC_A1,
          GX_CC_C2,
          GX_CC_A2,
          GX_CC_TEXC,
          GX_CC_TEXA,
          GX_CC_RASC,
          GX_CC_RASA,
          GX_CC_ONE,
          GX_CC_HALF,
          GX_CC_KONST,
          GX_CC_ZERO,
        }

        public enum GxCa {
          GX_CA_APREV,
          GX_CA_A0,
          GX_CA_A1,
          GX_CA_A2,
          GX_CA_TEXA,
          GX_CA_RASA,
          GX_CA_KONST,
          GX_CA_ZERO,
        }

        public enum TevOp {
          GX_TEV_ADD,
          GX_TEV_SUB,
          GX_TEV_COMP_R8_GT,
          GX_TEV_COMP_R8_EQ,
          GX_TEV_COMP_GR16_GT,
          GX_TEV_COMP_GR16_EQ,
          GX_TEV_COMP_BGR24_GT,
          GX_TEV_COMP_BGR24_EQ,
          GX_TEV_COMP_RGB8_GT,
          GX_TEV_COMP_RGB8_EQ
        }

        public enum TevBias {
          GX_TB_ZERO,
          GX_TB_ADDHALF,
          GX_TB_SUBHALF
        }

        public enum TevScale {
          GX_CS_SCALE_1,
          GX_CS_SCALE_2,
          GX_CS_SCALE_4,
          GX_CS_DIVIDE_2
        }

        public enum ColorRegister {
          GX_TEVPREV,
          GX_TEVREG0,
          GX_TEVREG1,
          GX_TEVREG2,
        }
      }

      [Schema]
      public partial class TevOrder : IDeserializable {
        public byte TexCoordId;
        public sbyte TexMap;
        [Format(SchemaNumberType.BYTE)]
        public ColorChannel ColorChannelId;
        private readonly byte padding_ = 0xff;

        public enum ColorChannel {
          GX_COLOR0,
          GX_COLOR1,
          GX_ALPHA0,
          GX_ALPHA1,
          GX_COLOR0A0,
          GX_COLOR1A1,
          GX_COLORZERO,
          GX_BUMP,
          GX_BUMPN,
          GX_COLORNULL,
        }
      }
    }

    public class TEX1Section
    {
      public const string Signature = "TEX1";
      public DataBlockHeader Header;
      public ushort NrTextures;
      public ushort Padding;
      public uint TextureHeaderOffset;
      public uint StringTableOffset;
      public BMD.Stringtable StringTable;
      public BMD.TEX1Section.TextureHeader[] TextureHeaders;

      public TEX1Section(EndianBinaryReader er, out bool OK)
      {
        long position1 = er.BaseStream.Position;
        bool OK1;
        this.Header = new DataBlockHeader(er, "TEX1", out OK1);
        if (!OK1)
        {
          OK = false;
        }
        else
        {
          this.NrTextures = er.ReadUInt16();
          this.Padding = er.ReadUInt16();
          this.TextureHeaderOffset = er.ReadUInt32();
          this.StringTableOffset = er.ReadUInt32();
          long position2 = er.BaseStream.Position;
          er.BaseStream.Position = position1 + (long) this.StringTableOffset;
          this.StringTable = new BMD.Stringtable(er);
          er.BaseStream.Position = position1 + (long) this.TextureHeaderOffset;
          this.TextureHeaders = new BMD.TEX1Section.TextureHeader[(int) this.NrTextures];
          for (int idx = 0; idx < (int) this.NrTextures; ++idx)
            this.TextureHeaders[idx] = new BMD.TEX1Section.TextureHeader(er, position1, idx);
          er.BaseStream.Position = position1 + (long) this.Header.size;
          OK = true;
        }
      }

      public enum TextureFormat : byte {
        I4 = 0,
        I8 = 1,
        A4_I4 = 2,
        A8_I8 = 3,
        R5_G6_B5 = 4,
        A3_RGB5 = 5,
        ARGB8 = 6,
        INDEX4 = 8,
        INDEX8 = 9,
        INDEX14_X2 = 10, // 0x0000000A
        S3TC1 = 14, // 0x0000000E
      }

      public enum PaletteFormat : byte {
        PAL_A8_I8,
        PAL_R5_G6_B5,
        PAL_A3_RGB5,
      }

      public enum GX_WRAP_TAG: byte {
        GX_CLAMP,
        GX_REPEAT,
        GX_MIRROR,
        GX_MAXTEXWRAPMODE,
      }

      public enum GX_TEXTURE_FILTER : byte {
        GX_NEAR,
        GX_LINEAR,
        GX_NEAR_MIP_NEAR,
        GX_LIN_MIP_NEAR,
        GX_NEAR_MIP_LIN,
        GX_LIN_MIP_LIN,
        GX_NEAR2,
        GX_NEAR3,
      }

      [StructLayout(LayoutKind.Sequential, Pack = 1)]
      public class TextureHeader {
        // Do not modify any of these types or the order!
        public BMD.TEX1Section.TextureFormat Format;
        public Byte AlphaSetting;
        public UInt16 Width;
        public UInt16 Height;
        public BMD.TEX1Section.GX_WRAP_TAG WrapS;
        public BMD.TEX1Section.GX_WRAP_TAG WrapT;
        public Byte PalettesEnabled;
        public BMD.TEX1Section.PaletteFormat PaletteFormat;
        public UInt16 NrPaletteEntries;
        public UInt32 PaletteOffset;
        public IColor[] palette;
        public UInt32 BorderColor;
        public BMD.TEX1Section.GX_TEXTURE_FILTER MinFilter;
        public BMD.TEX1Section.GX_TEXTURE_FILTER MagFilter;
        public UInt16 Unknown4;
        public Byte NrMipMap;
        public Byte Unknown5;
        public UInt16 LodBias;
        public UInt32 DataOffset;

        [NonSerialized]
        public byte[] Data;

        public TextureHeader(EndianBinaryReader er, long baseoffset, int idx) {
          var pos = er.Position;

          this.Format = (BMD.TEX1Section.TextureFormat) er.ReadByte();
          this.AlphaSetting = er.ReadByte();
          this.Width = er.ReadUInt16();
          this.Height = er.ReadUInt16();
          this.WrapS = (BMD.TEX1Section.GX_WRAP_TAG) er.ReadByte();
          this.WrapT = (BMD.TEX1Section.GX_WRAP_TAG) er.ReadByte();
          this.PalettesEnabled = er.ReadByte();
          this.PaletteFormat = (BMD.TEX1Section.PaletteFormat) er.ReadByte();
          this.NrPaletteEntries = er.ReadUInt16();
          this.PaletteOffset = er.ReadUInt32();
          this.BorderColor = er.ReadUInt32();
          this.MinFilter = (BMD.TEX1Section.GX_TEXTURE_FILTER) er.ReadByte();
          this.MagFilter = (BMD.TEX1Section.GX_TEXTURE_FILTER) er.ReadByte();
          this.Unknown4 = er.ReadUInt16();
          this.NrMipMap = er.ReadByte();
          this.Unknown5 = er.ReadByte();
          this.LodBias = er.ReadUInt16();
          this.DataOffset = er.ReadUInt32();

          long position = er.BaseStream.Position;
          {
            er.BaseStream.Position = baseoffset + (long)this.DataOffset + (long)(32 * (idx + 1));
            this.Data = er.ReadBytes(this.GetCompressedBufferSize());
          }

          this.palette = new IColor[this.NrPaletteEntries];
          {
            er.Position = pos + this.PaletteOffset;
            for (var i = 0; i < this.NrPaletteEntries; ++i) {

              switch (this.PaletteFormat) {
                case PaletteFormat.PAL_A8_I8: {
                  var alpha = er.ReadByte();
                  var intensity = er.ReadByte();
                  this.palette[i] =
                      ColorImpl.FromRgbaBytes(intensity,
                                              intensity,
                                              intensity,
                                              alpha);
                  break;
                }
                case PaletteFormat.PAL_R5_G6_B5: {
                  this.palette[i] = ColorUtil.ParseRgb565(er.ReadUInt16());
                  break;
                }
                // TODO: There seems to be a bug reading the palette, these colors look weird
                case PaletteFormat.PAL_A3_RGB5: {
                  this.palette[i] = ColorUtil.ParseRgb5A3(er.ReadUInt16());
                  break;
                }
                default: 
                  throw new ArgumentOutOfRangeException();
              }
            }
          }
          er.BaseStream.Position = position;
        }

        // TODO: Share this implementation w/ BTI
        public unsafe System.Drawing.Bitmap ToBitmap() {
          Bitmap bitmap;
          var isIndex4 = this.Format == TextureFormat.INDEX4;
          var isIndex8 = this.Format == TextureFormat.INDEX8;
          if (isIndex4 || isIndex8) {
            bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            BitmapUtil.InvokeAsLocked(
                bitmap,
                bitmapData => {
                  var indices = new byte[this.Width * this.Height];
                  if (isIndex4) {
                    for (var i = 0; i < this.Data.Length; ++i) {
                      var two = this.Data[i];

                      var firstIndex = two >> 4;
                      var secondIndex = two & 0x0F;

                      indices[2 * i + 0] = (byte) firstIndex;
                      indices[2 * i + 1] = (byte) secondIndex;
                    }
                  } else {
                    indices = this.Data;
                  }

                  var blockWidth = 8;
                  var blockHeight = isIndex4 ? 8 : 4;

                  var index = 0;
                  var bytes = (byte*)bitmapData.Scan0.ToPointer();
                  for (var ty = 0; ty < this.Height / blockHeight; ty++) {
                    for (var tx = 0; tx < this.Width / blockWidth; tx++) {

                      for (var y = 0; y < blockHeight; ++y) {
                        for (var x = 0; x < blockWidth; ++x) {
                          var color = this.palette[indices[index++]];

                          var i = (ty * blockHeight + y) * this.Width + tx * blockWidth + x;
                          bytes[4 * i + 0] = color.Bb;
                          bytes[4 * i + 1] = color.Gb;
                          bytes[4 * i + 2] = color.Rb;
                          bytes[4 * i + 3] = color.Ab;
                        }
                      }
                    }
                  }
                });

            return bitmap;
          } 
          
          ImageDataFormat imageDataFormat = (ImageDataFormat)null;
          switch (this.Format) {
            case TextureFormat.I4:
              imageDataFormat = ImageDataFormat.I4;
              break;
            case TextureFormat.I8:
              imageDataFormat = ImageDataFormat.I8;
              break;
            case TextureFormat.A4_I4:
              imageDataFormat = ImageDataFormat.IA4;
              break;
            case TextureFormat.A8_I8:
              imageDataFormat = ImageDataFormat.IA8;
              break;
            case TextureFormat.R5_G6_B5:
              imageDataFormat = ImageDataFormat.RGB565;
              break;
            case TextureFormat.A3_RGB5:
              imageDataFormat = ImageDataFormat.RGB5A3;
              break;
            case TextureFormat.ARGB8:
              imageDataFormat = ImageDataFormat.Rgba32;
              break;
            case TextureFormat.S3TC1:
              imageDataFormat = ImageDataFormat.Cmpr;
              break;
            default:
              throw new NotImplementedException();
          }

          byte[] numArray = imageDataFormat.ConvertFrom(this.Data, (int)this.Width, (int)this.Height, (ProgressChangedEventHandler)null); 
          bitmap = new System.Drawing.Bitmap((int)this.Width, (int)this.Height);
          BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, (int)this.Width, (int)this.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
          for (int ofs = 0; ofs < numArray.Length; ++ofs)
            Marshal.WriteByte(bitmapdata.Scan0, ofs, numArray[ofs]);
          bitmap.UnlockBits(bitmapdata);
          return bitmap;
        }

        public int GetGlWrapModeS() => this.GetGlWrapMode(this.WrapS);
        public int GetGlWrapModeT() => this.GetGlWrapMode(this.WrapT);

        private int GetGlWrapMode(BMD.TEX1Section.GX_WRAP_TAG id) {
          switch (id)
          {
            case BMD.TEX1Section.GX_WRAP_TAG.GX_CLAMP:
              return 33071;
            case BMD.TEX1Section.GX_WRAP_TAG.GX_REPEAT:
              return 10497;
            case BMD.TEX1Section.GX_WRAP_TAG.GX_MIRROR:
              return 33648;
            case BMD.TEX1Section.GX_WRAP_TAG.GX_MAXTEXWRAPMODE:
              return 10496;
            default:
              return -1;
          }
        }

        public int GetGlFilterModeMin() => this.GetGlFilterMode(this.MinFilter);
        public int GetGlFilterModeMag() => this.GetGlFilterMode(this.MagFilter);

        private int GetGlFilterMode(BMD.TEX1Section.GX_TEXTURE_FILTER id) {
          switch (id)
          {
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_NEAR:
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_NEAR2:
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_NEAR3:
              return 9728;
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_LINEAR:
              return 9729;
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_NEAR_MIP_NEAR:
              return 9984;
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_LIN_MIP_NEAR:
              return 9985;
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_NEAR_MIP_LIN:
              return 9986;
            case BMD.TEX1Section.GX_TEXTURE_FILTER.GX_LIN_MIP_LIN:
              return 9987;
            default:
              return -1;
          }
        }

        private int GetCompressedBufferSize() {
          int num1 = (int) this.Width + (8 - (int) this.Width % 8) % 8;
          int num2 = (int) this.Width + (4 - (int) this.Width % 4) % 4;
          int num3 = (int) this.Height + (8 - (int) this.Height % 8) % 8;
          int num4 = (int) this.Height + (4 - (int) this.Height % 4) % 4;
          switch (this.Format)
          {
            case BMD.TEX1Section.TextureFormat.I4:
              return num1 * num3 / 2;
            case BMD.TEX1Section.TextureFormat.I8:
              return num1 * num4;
            case BMD.TEX1Section.TextureFormat.A4_I4:
              return num1 * num4;
            case BMD.TEX1Section.TextureFormat.A8_I8:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.R5_G6_B5:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.A3_RGB5:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.ARGB8:
              return num2 * num4 * 4;
            case BMD.TEX1Section.TextureFormat.INDEX4:
              return num1 * num3 / 2;
            case BMD.TEX1Section.TextureFormat.INDEX8:
              return num1 * num4;
            case BMD.TEX1Section.TextureFormat.INDEX14_X2:
              return num2 * num4 * 2;
            case BMD.TEX1Section.TextureFormat.S3TC1:
              return num2 * num4 / 2;
            default:
              return -1;
          }
        }
      }
    }

    public class Stringtable
    {
      public ushort NrStrings;
      public ushort Padding;
      public BMD.Stringtable.StringTableEntry[] Entries;

      public Stringtable(EndianBinaryReader er)
      {
        long position = er.BaseStream.Position;
        this.NrStrings = er.ReadUInt16();
        this.Padding = er.ReadUInt16();
        this.Entries = new BMD.Stringtable.StringTableEntry[(int) this.NrStrings];
        for (int index = 0; index < (int) this.NrStrings; ++index)
          this.Entries[index] = new BMD.Stringtable.StringTableEntry(er, position);
      }

      public string this[int index]
      {
        get
        {
          return (string) this.Entries[index];
        }
      }

      public int this[string index]
      {
        get
        {
          for (int index1 = 0; index1 < this.Entries.Length; ++index1)
          {
            if ((string) this.Entries[index1] == index)
              return index1;
          }
          return -1;
        }
      }

      public class StringTableEntry
      {
        public ushort Unknown;
        public ushort Offset;
        public string Entry;

        public StringTableEntry(EndianBinaryReader er, long baseoffset)
        {
          this.Unknown = er.ReadUInt16();
          this.Offset = er.ReadUInt16();
          long position = er.BaseStream.Position;
          er.BaseStream.Position = baseoffset + (long) this.Offset;
          this.Entry = er.ReadStringNT(Encoding.ASCII);
          er.BaseStream.Position = position;
        }

        public override string ToString()
        {
          return this.Entry;
        }

        public static implicit operator string(BMD.Stringtable.StringTableEntry e)
        {
          return e.Entry;
        }
      }
    }

    private class Node
    {
      public BMD.Node Parent;
      public string Name;

      public Node(string Name, BMD.Node Parent)
      {
        this.Name = Name;
        this.Parent = Parent;
      }
    }
  }
}
