// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.MA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Color = System.Drawing.Color;

namespace MKDS_Course_Modifier._3D_Formats
{
  public class MA
  {
    public static byte[] Obj2Ma(OBJ WaveFrontOBJ)
    {
      MA.MAWriter maWriter = new MA.MAWriter();
      Dictionary<string, List<OBJ.Face>> dictionary = new Dictionary<string, List<OBJ.Face>>();
      foreach (OBJ.Face face in WaveFrontOBJ.Faces)
      {
        if (!dictionary.ContainsKey(face.MaterialName))
          dictionary.Add(face.MaterialName, new List<OBJ.Face>());
        dictionary[face.MaterialName].Add(face);
      }
      foreach (KeyValuePair<string, List<OBJ.Face>> keyValuePair in dictionary)
      {
        List<OpenTK.Vector3> vector3List1 = new List<OpenTK.Vector3>();
        List<OpenTK.Vector2> vector2List = new List<OpenTK.Vector2>();
        List<OpenTK.Vector3> vector3List2 = new List<OpenTK.Vector3>();
        foreach (OBJ.Face face in keyValuePair.Value)
        {
          vector3List1.Add(WaveFrontOBJ.Vertices[face.VertexIndieces[0]]);
          vector3List1.Add(WaveFrontOBJ.Vertices[face.VertexIndieces[1]]);
          vector3List1.Add(WaveFrontOBJ.Vertices[face.VertexIndieces[2]]);
          vector2List.Add(WaveFrontOBJ.TexCoords[face.TexCoordIndieces[0]]);
          vector2List.Add(WaveFrontOBJ.TexCoords[face.TexCoordIndieces[1]]);
          vector2List.Add(WaveFrontOBJ.TexCoords[face.TexCoordIndieces[2]]);
          vector3List2.Add(WaveFrontOBJ.Normals[face.NormalIndieces[0]]);
          vector3List2.Add(WaveFrontOBJ.Normals[face.NormalIndieces[1]]);
          vector3List2.Add(WaveFrontOBJ.Normals[face.NormalIndieces[2]]);
        }
        maWriter.AddPhong(keyValuePair.Key);
        maWriter.AddMesh((string) null, keyValuePair.Key + "_p", keyValuePair.Key, vector3List1.ToArray(), vector2List.ToArray(), (Color[]) null, vector3List2.ToArray(), PolygonType.Triangle);
      }
      return maWriter.Close();
    }

    public static byte[] WriteBones(MA.Node[] Joints)
    {
      MA.MAWriter maWriter = new MA.MAWriter();
      foreach (MA.Node joint in Joints)
        maWriter.AddJoint(joint.Name, joint.Trans.X, joint.Trans.Y, joint.Trans.Z, joint.Rot.X, joint.Rot.Y, joint.Rot.Z, joint.Scale.X, joint.Scale.Y, joint.Scale.Z, joint.Parent);
      return maWriter.Close();
    }

    public static byte[] WriteAnimation(MA.Node[] Joints, MA.AnimatedNode[] AnimatedJoints)
    {
      MemoryStream memoryStream = new MemoryStream();
      TextWriter textWriter = (TextWriter) new StreamWriter((Stream) memoryStream);
      textWriter.WriteLine("//Maya ASCII 3.0 scene");
      textWriter.WriteLine("//Created by MKDS Course Modifier");
      textWriter.WriteLine("");
      textWriter.WriteLine("requires maya \"3.0\";");
      textWriter.WriteLine("currentUnit -l centimeter -a degree -t film;");
      int index1 = 0;
      foreach (MA.Node joint in Joints)
      {
        if (joint.Parent != null)
          textWriter.WriteLine("createNode joint -n \"{0}\" -p \"{1}\";", (object) joint.Name, (object) joint.Parent);
        else
          textWriter.WriteLine("createNode joint -n \"{0}\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".t\" -type \"double3\" {0} {1} {2};", (object) joint.Trans.X.ToString().Replace(",", "."), (object) joint.Trans.Y.ToString().Replace(",", "."), (object) joint.Trans.Z.ToString().Replace(",", "."));
        textWriter.WriteLine("setAttr \".r\" -type \"double3\" {0} {1} {2};", (object) joint.Rot.X.ToString().Replace(",", "."), (object) joint.Rot.Y.ToString().Replace(",", "."), (object) joint.Rot.Z.ToString().Replace(",", "."));
        textWriter.WriteLine("setAttr \".s\" -type \"double3\" {0} {1} {2};", (object) joint.Scale.X.ToString().Replace(",", "."), (object) joint.Scale.Y.ToString().Replace(",", "."), (object) joint.Scale.Z.ToString().Replace(",", "."));
        textWriter.WriteLine("createNode animCurveTL -n \"{0}_translateX\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Tx[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTL -n \"{0}_translateY\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Ty[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTL -n \"{0}_translateZ\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Tz[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTA -n \"{0}_rotateX\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Rx[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTA -n \"{0}_rotateY\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Ry[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTA -n \"{0}_rotateZ\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Rz[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTU -n \"{0}_scaleX\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Sx[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTU -n \"{0}_scaleY\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Sy[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("createNode animCurveTU -n \"{0}_scaleZ\";", (object) joint.Name);
        textWriter.WriteLine("setAttr \".tan\" 10;");
        textWriter.WriteLine("setAttr \".wgt\" no;");
        textWriter.Write("setAttr -s {0} \".ktv[0:{1}]\" ", (object) AnimatedJoints[index1].NrFrames, (object) (AnimatedJoints[index1].NrFrames - 1));
        for (int index2 = 0; index2 < AnimatedJoints[index1].NrFrames; ++index2)
          textWriter.Write(" {0} {1}", (object) (index2 + 1), (object) AnimatedJoints[index1].Sz[index2].ToString().Replace(",", "."));
        textWriter.Write(";" + textWriter.NewLine);
        textWriter.WriteLine("connectAttr \"{0}_translateX.o\" \"{0}.tx\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_rotateX.o\" \"{0}.rx\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_scaleX.o\" \"{0}.sx\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_translateY.o\" \"{0}.ty\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_rotateY.o\" \"{0}.ry\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_scaleY.o\" \"{0}.sy\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_translateZ.o\" \"{0}.tz\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_rotateZ.o\" \"{0}.rz\";", (object) joint.Name);
        textWriter.WriteLine("connectAttr \"{0}_scaleZ.o\" \"{0}.sz\";", (object) joint.Name);
        ++index1;
      }
      textWriter.Flush();
      byte[] array = memoryStream.ToArray();
      textWriter.Close();
      return array;
    }

    public class Node
    {
      public Microsoft.Xna.Framework.Vector3 Trans;
      public Microsoft.Xna.Framework.Quaternion Rot;
      public Microsoft.Xna.Framework.Vector3 Scale;

      public Node(float[] Matrix, string Name, string Parent = null)
      {
        new Matrix(Matrix[0], Matrix[1], Matrix[2], Matrix[3], Matrix[4], Matrix[5], Matrix[6], Matrix[7], Matrix[8], Matrix[9], Matrix[10], Matrix[11], Matrix[12], Matrix[13], Matrix[14], Matrix[15]).Decompose(out this.Scale, out this.Rot, out this.Trans);
        this.Name = Name;
        this.Parent = Parent;
      }

      public Node(
        float Tx,
        float Ty,
        float Tz,
        float Rx,
        float Ry,
        float Rz,
        float Sx,
        float Sy,
        float Sz,
        string Name,
        string Parent = null)
      {
        this.Scale = new Microsoft.Xna.Framework.Vector3(Sx, Sy, Sz);
        this.Rot = new Microsoft.Xna.Framework.Quaternion(Rx, Ry, Rz, 0.0f);
        this.Trans = new Microsoft.Xna.Framework.Vector3(Tx, Ty, Tz);
        this.Name = Name;
        this.Parent = Parent;
      }

      public string Name { get; set; }

      public string Parent { get; set; }

      public override string ToString()
      {
        return this.Name;
      }
    }

    public class AnimatedNode
    {
      public float[] Tx;
      public float[] Ty;
      public float[] Tz;
      public float[] Rx;
      public float[] Ry;
      public float[] Rz;
      public float[] Sx;
      public float[] Sy;
      public float[] Sz;
      public int NrFrames;

      public AnimatedNode(
        int NrFrames,
        float[] Tx,
        float[] Ty,
        float[] Tz,
        float[] Rx,
        float[] Ry,
        float[] Rz,
        float[] Sx,
        float[] Sy,
        float[] Sz)
      {
        this.NrFrames = NrFrames;
        this.Tx = Tx;
        this.Ty = Ty;
        this.Tz = Tz;
        this.Rx = Rx;
        this.Ry = Ry;
        this.Rz = Rz;
        this.Sx = Sx;
        this.Sy = Sy;
        this.Sz = Sz;
      }
    }

    public class MAWriter
    {
      private MemoryStream InternalStream;

      public TextWriter BaseStream { get; private set; }

      public MAWriter()
      {
        this.InternalStream = new MemoryStream();
        this.BaseStream = (TextWriter) new StreamWriter((Stream) this.InternalStream);
        this.BaseStream.WriteLine("//Maya ASCII 3.0 scene");
        this.BaseStream.WriteLine("//Created by MKDS Course Modifier");
        this.BaseStream.WriteLine("");
        this.BaseStream.WriteLine("requires maya \"3.0\";");
        this.BaseStream.WriteLine("currentUnit -l centimeter -a degree -t film;");
      }

      public void CreateNode(
        MA.MAWriter.NodeType Type,
        string Name,
        string Parent = null,
        bool Shared = false,
        bool SkipSelect = false)
      {
        this.BaseStream.Write("createNode " + Enum.GetName(typeof (MA.MAWriter.NodeType), (object) Type));
        this.BaseStream.Write(" -n \"" + Name + "\"");
        if (Parent != null)
          this.BaseStream.Write(" -p \"" + Parent + "\"");
        if (Shared)
          this.BaseStream.Write(" -s");
        if (SkipSelect)
          this.BaseStream.Write(" -ss");
        this.BaseStream.Write(";" + this.BaseStream.NewLine);
      }

      public void SetAttribute(
        string Attribute,
        string Arguments,
        MA.MAWriter.AttributeType Type = MA.MAWriter.AttributeType.numericDefault,
        uint Size = 4294967295,
        bool AlteredValue = false,
        bool Caching = false,
        bool ChannelBox = false,
        bool Clamp = false,
        bool Keyable = false,
        bool Lock = false)
      {
        this.BaseStream.Write("setAttr");
        if (Size != uint.MaxValue)
          this.BaseStream.Write(" -s " + (object) Size);
        this.BaseStream.Write(" \"" + Attribute + "\"");
        if (Type != MA.MAWriter.AttributeType.numericDefault)
          this.BaseStream.Write(" -type \"" + Enum.GetName(typeof (MA.MAWriter.AttributeType), (object) Type).Replace("@", "") + "\"");
        this.BaseStream.Write(" " + Arguments);
        this.BaseStream.Write(";" + this.BaseStream.NewLine);
      }

      public void SetAttribute(
        string Attribute,
        MA.MAWriter.AttributeType Type,
        params string[] Arguments)
      {
        this.SetAttribute(Attribute, Type, -1, Arguments);
      }

      public void SetAttribute(
        string Attribute,
        MA.MAWriter.AttributeType Type,
        int Size,
        params string[] Arguments)
      {
        this.BaseStream.Write("setAttr");
        if (Size != -1)
          this.BaseStream.Write(" -s " + (object) Size);
        this.BaseStream.Write(" \"" + Attribute + "\"");
        if (Type != MA.MAWriter.AttributeType.numericDefault)
          this.BaseStream.Write(" -type \"" + Enum.GetName(typeof (MA.MAWriter.AttributeType), (object) Type).Replace("@", "") + "\"");
        foreach (string str in Arguments)
          this.BaseStream.Write(" " + str);
        this.BaseStream.Write(";" + this.BaseStream.NewLine);
      }

      public void ConnectAttribute(string FirstNode, string SecondNode, bool NextAvailable = false)
      {
        this.BaseStream.WriteLine("connectAttr \"" + FirstNode + "\" \"" + SecondNode + "\"" + (NextAvailable ? " -na" : "") + ";");
      }

      public void AddJoint(
        string Name,
        float Tx,
        float Ty,
        float Tz,
        float Rx,
        float Ry,
        float Rz,
        float Sx,
        float Sy,
        float Sz,
        string Parent = null)
      {
        this.CreateNode(MA.MAWriter.NodeType.joint, Name, Parent, false, false);
        this.SetAttribute(".t", MA.MAWriter.AttributeType.double3, Tx.ToString().Replace(",", "."), Ty.ToString().Replace(",", "."), Tz.ToString().Replace(",", "."));
        this.SetAttribute(".r", MA.MAWriter.AttributeType.double3, Rx.ToString().Replace(",", "."), Ry.ToString().Replace(",", "."), Rz.ToString().Replace(",", "."));
        this.SetAttribute(".s", MA.MAWriter.AttributeType.double3, Sx.ToString().Replace(",", "."), Sy.ToString().Replace(",", "."), Sz.ToString().Replace(",", "."));
      }

      public void AddMesh(
        string GroupName,
        string Polyname,
        string Material,
        OpenTK.Vector3[] Positions,
        OpenTK.Vector2[] TexCoords,
        Color[] VertexColors,
        OpenTK.Vector3[] Normals,
        PolygonType p)
      {
        this.CreateNode(MA.MAWriter.NodeType.groupId, Polyname + "id1", (string) null, false, false);
        this.SetAttribute(".ihi", MA.MAWriter.AttributeType.numericDefault, "0");
        this.CreateNode(MA.MAWriter.NodeType.groupId, Polyname + "id2", (string) null, false, false);
        this.SetAttribute(".ihi", MA.MAWriter.AttributeType.numericDefault, "0");
        this.CreateNode(MA.MAWriter.NodeType.mesh, Polyname + "Shape", GroupName, false, false);
        this.SetAttribute(".iog[0].og", MA.MAWriter.AttributeType.numericDefault, 2, "");
        this.SetAttribute(".iog[0].og[0].gcl", MA.MAWriter.AttributeType.componentList, "0");
        this.SetAttribute(".iog[0].og[1].gcl", MA.MAWriter.AttributeType.componentList, "1", "\"f[0:1]\"");
        this.SetAttribute(".uvst[0].uvsn", MA.MAWriter.AttributeType.@string, "map1");
        List<string> stringList1 = new List<string>();
        float num1;
        foreach (OpenTK.Vector2 texCoord in TexCoords)
        {
          List<string> stringList2 = stringList1;
          num1 = texCoord.X;
          string str1 = num1.ToString().Replace(",", ".");
          stringList2.Add(str1);
          List<string> stringList3 = stringList1;
          num1 = -texCoord.Y;
          string str2 = num1.ToString().Replace(",", ".");
          stringList3.Add(str2);
        }
        this.SetAttribute(".uvst[0].uvsp[0:" + (object) (TexCoords.Length - 1) + "]", MA.MAWriter.AttributeType.float2, TexCoords.Length, stringList1.ToArray());
        this.SetAttribute(".cuvs", MA.MAWriter.AttributeType.@string, "map1");
        this.SetAttribute(".dcol", MA.MAWriter.AttributeType.numericDefault, "yes");
        this.SetAttribute(".dcc", MA.MAWriter.AttributeType.@string, "Diffuse");
        List<string> stringList4 = new List<string>();
        foreach (OpenTK.Vector3 position in Positions)
        {
          List<string> stringList2 = stringList4;
          num1 = position.X;
          string str1 = num1.ToString().Replace(",", ".");
          stringList2.Add(str1);
          List<string> stringList3 = stringList4;
          num1 = position.Y;
          string str2 = num1.ToString().Replace(",", ".");
          stringList3.Add(str2);
          List<string> stringList5 = stringList4;
          num1 = position.Z;
          string str3 = num1.ToString().Replace(",", ".");
          stringList5.Add(str3);
        }
        this.SetAttribute(".vt[0:" + (object) (Positions.Length - 1) + "]", MA.MAWriter.AttributeType.numericDefault, Positions.Length, stringList4.ToArray());
        int num2 = 0;
        List<string> stringList6 = new List<string>();
        int num3;
        int num4;
        switch (p)
        {
          case PolygonType.Triangle:
            for (int index = 0; index < Positions.Length; index += 3)
            {
              stringList6.Add(num2.ToString());
              List<string> stringList2 = stringList6;
              num3 = num2 + 1;
              string str1 = num3.ToString();
              stringList2.Add(str1);
              stringList6.Add("0");
              List<string> stringList3 = stringList6;
              num3 = num2 + 1;
              string str2 = num3.ToString();
              stringList3.Add(str2);
              List<string> stringList5 = stringList6;
              num3 = num2 + 2;
              string str3 = num3.ToString();
              stringList5.Add(str3);
              stringList6.Add("0");
              stringList6.Add(num2.ToString());
              List<string> stringList7 = stringList6;
              num3 = num2 + 2;
              string str4 = num3.ToString();
              stringList7.Add(str4);
              stringList6.Add("0");
              num2 += 3;
            }
            break;
          case PolygonType.Quad:
            for (int index = 0; index < Positions.Length; index += 4)
            {
              stringList6.Add(num2.ToString());
              List<string> stringList2 = stringList6;
              num3 = num2 + 1;
              string str1 = num3.ToString();
              stringList2.Add(str1);
              stringList6.Add("0");
              List<string> stringList3 = stringList6;
              num3 = num2 + 1;
              string str2 = num3.ToString();
              stringList3.Add(str2);
              List<string> stringList5 = stringList6;
              num3 = num2 + 2;
              string str3 = num3.ToString();
              stringList5.Add(str3);
              stringList6.Add("0");
              List<string> stringList7 = stringList6;
              num3 = num2 + 2;
              string str4 = num3.ToString();
              stringList7.Add(str4);
              List<string> stringList8 = stringList6;
              num3 = num2 + 3;
              string str5 = num3.ToString();
              stringList8.Add(str5);
              stringList6.Add("0");
              stringList6.Add(num2.ToString());
              List<string> stringList9 = stringList6;
              num3 = num2 + 3;
              string str6 = num3.ToString();
              stringList9.Add(str6);
              stringList6.Add("0");
              num2 += 4;
            }
            break;
          case PolygonType.TriangleStrip:
            for (int index = 0; index + 2 < Positions.Length; index += 2)
            {
              List<string> stringList2 = stringList6;
              num3 = num2 + index;
              string str1 = num3.ToString();
              stringList2.Add(str1);
              List<string> stringList3 = stringList6;
              num3 = num2 + index + 1;
              string str2 = num3.ToString();
              stringList3.Add(str2);
              stringList6.Add("0");
              List<string> stringList5 = stringList6;
              num3 = num2 + index + 1;
              string str3 = num3.ToString();
              stringList5.Add(str3);
              List<string> stringList7 = stringList6;
              num3 = num2 + index + 2;
              string str4 = num3.ToString();
              stringList7.Add(str4);
              stringList6.Add("0");
              List<string> stringList8 = stringList6;
              num3 = num2 + index;
              string str5 = num3.ToString();
              stringList8.Add(str5);
              List<string> stringList9 = stringList6;
              num3 = num2 + index + 2;
              string str6 = num3.ToString();
              stringList9.Add(str6);
              stringList6.Add("0");
              if (index + 3 < Positions.Length)
              {
                List<string> stringList10 = stringList6;
                num3 = num2 + index + 1;
                string str7 = num3.ToString();
                stringList10.Add(str7);
                List<string> stringList11 = stringList6;
                num3 = num2 + index + 3;
                string str8 = num3.ToString();
                stringList11.Add(str8);
                stringList6.Add("0");
                List<string> stringList12 = stringList6;
                num3 = num2 + index + 3;
                string str9 = num3.ToString();
                stringList12.Add(str9);
                List<string> stringList13 = stringList6;
                num3 = num2 + index + 2;
                string str10 = num3.ToString();
                stringList13.Add(str10);
                stringList6.Add("0");
                List<string> stringList14 = stringList6;
                num3 = num2 + index + 1;
                string str11 = num3.ToString();
                stringList14.Add(str11);
                List<string> stringList15 = stringList6;
                num3 = num2 + index + 2;
                string str12 = num3.ToString();
                stringList15.Add(str12);
                stringList6.Add("0");
              }
            }
            num4 = num2 + Positions.Length;
            break;
        }
        this.SetAttribute(".ed[0:" + (object) (stringList6.Count / 3 - 1) + "]", MA.MAWriter.AttributeType.numericDefault, stringList6.Count / 3, stringList6.ToArray());
        if (VertexColors != null)
        {
          List<string> stringList2 = new List<string>();
          foreach (Color vertexColor in VertexColors)
          {
            List<string> stringList3 = stringList2;
            num1 = (float) vertexColor.R / (float) byte.MaxValue;
            string str1 = num1.ToString().Replace(",", ".");
            stringList3.Add(str1);
            List<string> stringList5 = stringList2;
            num1 = (float) vertexColor.G / (float) byte.MaxValue;
            string str2 = num1.ToString().Replace(",", ".");
            stringList5.Add(str2);
            List<string> stringList7 = stringList2;
            num1 = (float) vertexColor.B / (float) byte.MaxValue;
            string str3 = num1.ToString().Replace(",", ".");
            stringList7.Add(str3);
            List<string> stringList8 = stringList2;
            num1 = (float) vertexColor.A / (float) byte.MaxValue;
            string str4 = num1.ToString().Replace(",", ".");
            stringList8.Add(str4);
          }
          this.SetAttribute(".clr[0:" + (object) (VertexColors.Length - 1) + "]", MA.MAWriter.AttributeType.numericDefault, VertexColors.Length, stringList2.ToArray());
        }
        if (Normals != null)
        {
          List<string> stringList2 = new List<string>();
          foreach (OpenTK.Vector3 normal in Normals)
          {
            List<string> stringList3 = stringList2;
            num1 = normal.X;
            string str1 = num1.ToString().Replace(",", ".");
            stringList3.Add(str1);
            List<string> stringList5 = stringList2;
            num1 = normal.Y;
            string str2 = num1.ToString().Replace(",", ".");
            stringList5.Add(str2);
            List<string> stringList7 = stringList2;
            num1 = normal.Z;
            string str3 = num1.ToString().Replace(",", ".");
            stringList7.Add(str3);
          }
          this.SetAttribute(".n[0:" + (object) (Normals.Length - 1) + "]", MA.MAWriter.AttributeType.float3, Normals.Length, stringList2.ToArray());
        }
        int num5 = 0;
        int Size = 0;
        int num6 = 0;
        List<string> stringList16 = new List<string>();
        switch (p)
        {
          case PolygonType.Triangle:
            for (int index = 0; index < Positions.Length; index += 3)
            {
              stringList16.Add("f");
              stringList16.Add("3");
              List<string> stringList2 = stringList16;
              int num7 = num6;
              int num8 = num7 + 1;
              num3 = num7;
              string str1 = num3.ToString();
              stringList2.Add(str1);
              List<string> stringList3 = stringList16;
              int num9 = num8;
              int num10 = num9 + 1;
              num3 = num9;
              string str2 = num3.ToString();
              stringList3.Add(str2);
              List<string> stringList5 = stringList16;
              int num11 = num10;
              num6 = num11 + 1;
              num3 = num11;
              string str3 = num3.ToString();
              stringList5.Add(str3);
              stringList16.Add("mu");
              stringList16.Add("0");
              stringList16.Add("3");
              stringList16.Add(num5.ToString());
              List<string> stringList7 = stringList16;
              num3 = num5 + 1;
              string str4 = num3.ToString();
              stringList7.Add(str4);
              List<string> stringList8 = stringList16;
              num3 = num5 + 2;
              string str5 = num3.ToString();
              stringList8.Add(str5);
              if (VertexColors != null)
              {
                stringList16.Add("fc");
                stringList16.Add("3");
                stringList16.Add(num5.ToString());
                List<string> stringList9 = stringList16;
                num3 = num5 + 1;
                string str6 = num3.ToString();
                stringList9.Add(str6);
                List<string> stringList10 = stringList16;
                num3 = num5 + 2;
                string str7 = num3.ToString();
                stringList10.Add(str7);
              }
              num5 += 3;
              ++Size;
            }
            break;
          case PolygonType.Quad:
            for (int index = 0; index < Positions.Length; index += 4)
            {
              stringList16.Add("f");
              stringList16.Add("4");
              List<string> stringList2 = stringList16;
              int num7 = num6;
              int num8 = num7 + 1;
              num3 = num7;
              string str1 = num3.ToString();
              stringList2.Add(str1);
              List<string> stringList3 = stringList16;
              int num9 = num8;
              int num10 = num9 + 1;
              num3 = num9;
              string str2 = num3.ToString();
              stringList3.Add(str2);
              List<string> stringList5 = stringList16;
              int num11 = num10;
              int num12 = num11 + 1;
              num3 = num11;
              string str3 = num3.ToString();
              stringList5.Add(str3);
              List<string> stringList7 = stringList16;
              int num13 = num12;
              num6 = num13 + 1;
              num3 = num13;
              string str4 = num3.ToString();
              stringList7.Add(str4);
              stringList16.Add("mu");
              stringList16.Add("0");
              stringList16.Add("4");
              stringList16.Add(num5.ToString());
              List<string> stringList8 = stringList16;
              num3 = num5 + 1;
              string str5 = num3.ToString();
              stringList8.Add(str5);
              List<string> stringList9 = stringList16;
              num3 = num5 + 2;
              string str6 = num3.ToString();
              stringList9.Add(str6);
              List<string> stringList10 = stringList16;
              num3 = num5 + 3;
              string str7 = num3.ToString();
              stringList10.Add(str7);
              if (VertexColors != null)
              {
                stringList16.Add("fc");
                stringList16.Add("4");
                stringList16.Add(num5.ToString());
                List<string> stringList11 = stringList16;
                num3 = num5 + 1;
                string str8 = num3.ToString();
                stringList11.Add(str8);
                List<string> stringList12 = stringList16;
                num3 = num5 + 2;
                string str9 = num3.ToString();
                stringList12.Add(str9);
                List<string> stringList13 = stringList16;
                num3 = num5 + 3;
                string str10 = num3.ToString();
                stringList13.Add(str10);
              }
              num5 += 4;
              ++Size;
            }
            break;
          case PolygonType.TriangleStrip:
            for (int index = 0; index + 2 < Positions.Length; index += 2)
            {
              stringList16.Add("f");
              stringList16.Add("3");
              List<string> stringList2 = stringList16;
              int num7 = num6;
              int num8 = num7 + 1;
              num3 = num7;
              string str1 = num3.ToString();
              stringList2.Add(str1);
              List<string> stringList3 = stringList16;
              int num9 = num8;
              int num10 = num9 + 1;
              num3 = num9;
              string str2 = num3.ToString();
              stringList3.Add(str2);
              List<string> stringList5 = stringList16;
              int num11 = num10;
              num6 = num11 + 1;
              num3 = num11;
              string str3 = num3.ToString();
              stringList5.Add(str3);
              stringList16.Add("mu");
              stringList16.Add("0");
              stringList16.Add("3");
              List<string> stringList7 = stringList16;
              num3 = num5 + index;
              string str4 = num3.ToString();
              stringList7.Add(str4);
              List<string> stringList8 = stringList16;
              num3 = num5 + index + 1;
              string str5 = num3.ToString();
              stringList8.Add(str5);
              List<string> stringList9 = stringList16;
              num3 = num5 + index + 2;
              string str6 = num3.ToString();
              stringList9.Add(str6);
              if (VertexColors != null)
              {
                stringList16.Add("fc");
                stringList16.Add("3");
                List<string> stringList10 = stringList16;
                num3 = num5 + index;
                string str7 = num3.ToString();
                stringList10.Add(str7);
                List<string> stringList11 = stringList16;
                num3 = num5 + index + 1;
                string str8 = num3.ToString();
                stringList11.Add(str8);
                List<string> stringList12 = stringList16;
                num3 = num5 + index + 2;
                string str9 = num3.ToString();
                stringList12.Add(str9);
              }
              ++Size;
              if (index + 3 < Positions.Length)
              {
                stringList16.Add("f");
                stringList16.Add("3");
                List<string> stringList10 = stringList16;
                int num12 = num6;
                int num13 = num12 + 1;
                num3 = num12;
                string str7 = num3.ToString();
                stringList10.Add(str7);
                List<string> stringList11 = stringList16;
                int num14 = num13;
                int num15 = num14 + 1;
                num3 = num14;
                string str8 = num3.ToString();
                stringList11.Add(str8);
                List<string> stringList12 = stringList16;
                int num16 = num15;
                num6 = num16 + 1;
                num3 = num16;
                string str9 = num3.ToString();
                stringList12.Add(str9);
                stringList16.Add("mu");
                stringList16.Add("0");
                stringList16.Add("3");
                List<string> stringList13 = stringList16;
                num3 = num5 + index + 1;
                string str10 = num3.ToString();
                stringList13.Add(str10);
                List<string> stringList14 = stringList16;
                num3 = num5 + index + 3;
                string str11 = num3.ToString();
                stringList14.Add(str11);
                List<string> stringList15 = stringList16;
                num3 = num5 + index + 2;
                string str12 = num3.ToString();
                stringList15.Add(str12);
                if (VertexColors != null)
                {
                  stringList16.Add("fc");
                  stringList16.Add("3");
                  List<string> stringList17 = stringList16;
                  num3 = num5 + index + 1;
                  string str13 = num3.ToString();
                  stringList17.Add(str13);
                  List<string> stringList18 = stringList16;
                  num3 = num5 + index + 3;
                  string str14 = num3.ToString();
                  stringList18.Add(str14);
                  List<string> stringList19 = stringList16;
                  num3 = num5 + index + 2;
                  string str15 = num3.ToString();
                  stringList19.Add(str15);
                }
                ++Size;
              }
            }
            num4 = num5 + Positions.Length;
            break;
        }
        this.SetAttribute(".fc[0:" + (object) (Size - 1) + "]", MA.MAWriter.AttributeType.polyFace, Size, stringList16.ToArray());
        this.SetAttribute(".matb", MA.MAWriter.AttributeType.numericDefault, "6");
        this.ConnectAttribute(Polyname + "id2.id", Polyname + "Shape.iog.og[1].gid", false);
        this.ConnectAttribute(Material + ".mwc", Polyname + "Shape.iog.og[1].gco", false);
        this.ConnectAttribute(Polyname + "id1.id", Polyname + "Shape.ciog.cog[0].cgid", false);
        this.ConnectAttribute(Polyname + "id2.msg", Material + ".gn", true);
        this.ConnectAttribute(Polyname + "Shape.iog.og[1]", Material + ".dsm", true);
        this.ConnectAttribute(Polyname + "Shape.ciog.cog[0]", ":initialShadingGroup.dsm", true);
        this.ConnectAttribute(Polyname + "id1.msg", ":initialShadingGroup.gn", true);
      }

      public void AddPhong(string Name)
      {
        this.CreateNode(MA.MAWriter.NodeType.shadingEngine, Name, (string) null, false, false);
        this.SetAttribute(".ihi", MA.MAWriter.AttributeType.numericDefault, "0");
        this.SetAttribute(".ro", MA.MAWriter.AttributeType.numericDefault, "yes");
        this.CreateNode(MA.MAWriter.NodeType.materialInfo, Name + "In", (string) null, false, false);
        this.CreateNode(MA.MAWriter.NodeType.phong, Name + "Ph", (string) null, false, false);
        this.SetAttribute(".ambc", MA.MAWriter.AttributeType.float3, "0.78430003", "0.78430003", "0.78430003");
        this.SetAttribute(".sc", MA.MAWriter.AttributeType.float3, "0.30000001", "0.30000001", "0.30000001");
        this.SetAttribute(".cp", MA.MAWriter.AttributeType.numericDefault, "2.059999942779541");
        this.ConnectAttribute(Name + "Ph.oc", Name + ".ss", false);
        this.ConnectAttribute(Name + ".msg", Name + "In.sg", false);
        this.ConnectAttribute(Name + "Ph.msg", Name + "In.m", false);
        this.ConnectAttribute(Name + ".pa", ":renderPartition.st", true);
        this.ConnectAttribute(Name + "Ph.msg", ":defaultShaderList1.s", true);
      }

      public byte[] Close()
      {
        this.BaseStream.Flush();
        byte[] array = this.InternalStream.ToArray();
        this.BaseStream.Close();
        return array;
      }

      public enum NodeType
      {
        abstractBaseCreate,
        abstractBaseNurbsConversion,
        addDoubleLinear,
        addMatrix,
        aimConstraint,
        airField,
        alignCurve,
        alignSurface,
        ambientLight,
        angleBetween,
        animBlend,
        animBlendInOut,
        animBlendNodeAdditive,
        animBlendNodeAdditiveDA,
        animBlendNodeAdditiveDL,
        animBlendNodeAdditiveF,
        animBlendNodeAdditiveFA,
        animBlendNodeAdditiveFL,
        animBlendNodeAdditiveI16,
        animBlendNodeAdditiveI32,
        animBlendNodeAdditiveRotation,
        animBlendNodeAdditiveScale,
        animBlendNodeBase,
        animBlendNodeBoolean,
        animBlendNodeEnum,
        animClip,
        animCurve,
        animCurveTA,
        animCurveTL,
        animCurveTT,
        animCurveTU,
        animCurveUA,
        animCurveUL,
        animCurveUT,
        animCurveUU,
        animLayer,
        anisotropic,
        annotationShape,
        arcLengthDimension,
        areaLight,
        arrayMapper,
        attachCurve,
        attachSurface,
        audio,
        avgCurves,
        avgNurbsSurfacePoints,
        avgSurfacePoints,
        bakeSet,
        baseGeometryVarGroup,
        baseLattice,
        baseShadingSwitch,
        bevel,
        bevelPlus,
        birailSrf,
        blend,
        blendColors,
        blendColorSets,
        blendDevice,
        blendShape,
        blendTwoAttr,
        blendWeighted,
        blindDataTemplate,
        blinn,
        boneLattice,
        boolean,
        boundary,
        boundaryBase,
        brownian,
        brush,
        bulge,
        bump2d,
        bump3d,
        cacheBase,
        cacheBlend,
        cacheFile,
        camera,
        cameraView,
        character,
        characterMap,
        characterOffset,
        checker,
        choice,
        chooser,
        clamp,
        clipLibrary,
        clipScheduler,
        closeCurve,
        closestPointOnMesh,
        closestPointOnSurface,
        closeSurface,
        cloth,
        cloud,
        cluster,
        clusterHandle,
        condition,
        constraint,
        container,
        contour_composite,
        contour_contrast_function_levels,
        contour_contrast_function_simple,
        contour_only,
        contour_ps,
        contour_shader_combi,
        contour_shader_curvature,
        contour_shader_depthfade,
        contour_shader_factorcolor,
        contour_shader_framefade,
        contour_shader_layerthinner,
        contour_shader_maxcolor,
        contour_shader_silhouette,
        contour_shader_simple,
        contour_shader_widthfromcolor,
        contour_shader_widthfromlight,
        contour_shader_widthfromlightdir,
        contour_store_function,
        contour_store_function_simple,
        contrast,
        controlPoint,
        copyColorSet,
        copyUVSet,
        crater,
        createColorSet,
        createUVSet,
        curveFromMesh,
        curveFromMeshCoM,
        curveFromMeshEdge,
        curveFromSubdiv,
        curveFromSubdivEdge,
        curveFromSubdivFace,
        curveFromSurface,
        curveFromSurfaceBnd,
        curveFromSurfaceCoS,
        curveFromSurfaceIso,
        curveInfo,
        curveIntersect,
        curveNormalizer,
        curveNormalizerAngle,
        curveNormalizerLinear,
        curveRange,
        curveShape,
        curveVarGroup,
        dagNode,
        dagPose,
        defaultLightList,
        defaultRenderUtilityList,
        defaultShaderList,
        defaultTextureList,
        deformableShape,
        deformBend,
        deformFlare,
        deformFunc,
        deformSine,
        deformSquash,
        deformTwist,
        deformWave,
        deleteColorSet,
        deleteComponent,
        deleteUVSet,
        dependNode,
        detachCurve,
        detachSurface,
        dgs_material,
        dgs_material_photon,
        dielectric_material,
        dielectric_material_photon,
        dimensionShape,
        directedDisc,
        directionalLight,
        diskCache,
        displacementShader,
        displayLayer,
        displayLayerManager,
        distanceBetween,
        distanceDimShape,
        dof,
        doubleShadingSwitch,
        dpBirailSrf,
        dragField,
        dropoffLocator,
        dynamicConstraint,
        dynBase,
        dynGlobals,
        entity,
        envBall,
        envChrome,
        envCube,
        envFacade,
        envFog,
        environmentFog,
        envSky,
        envSphere,
        explodeNurbsShell,
        expression,
        extendCurve,
        extendSurface,
        extrude,
        facade,
        ffBlendSrf,
        ffBlendSrfObsolete,
        ffd,
        ffFilletSrf,
        field,
        file,
        filletCurve,
        filter,
        filterClosestSample,
        filterEuler,
        filterResample,
        filterSimplify,
        fitBspline,
        flexorShape,
        flow,
        fluidEmitter,
        fluidShape,
        fluidTexture2D,
        fluidTexture3D,
        follicle,
        fourByFourMatrix,
        fractal,
        frameCache,
        FurAttractors,
        FurCurveAttractors,
        FurDescription,
        FurFeedback,
        FurGlobals,
        furPointOnMeshInfo,
        furPointOnSubd,
        gammaCorrect,
        geoConnectable,
        geoConnector,
        geometryConstraint,
        geometryFilter,
        geometryShape,
        geometryVarGroup,
        globalCacheControl,
        globalStitch,
        granite,
        gravityField,
        grid,
        groupId,
        groupParts,
        guide,
        hairConstraint,
        hairSystem,
        hairTubeShader,
        hardenPoint,
        hardwareRenderGlobals,
        heightField,
        hikEffector,
        hikFloorContactMarker,
        hikGroundPlane,
        hikHandle,
        hikSolver,
        historySwitch,
        holdMatrix,
        hsvToRgb,
        hwReflectionMap,
        hwRenderGlobals,
        hwShader,
        hyperGraphInfo,
        hyperLayout,
        hyperView,
        ikEffector,
        ikHandle,
        ikMCsolver,
        ikPASolver,
        ikRPsolver,
        ikSCsolver,
        ikSolver,
        ikSplineSolver,
        ikSystem,
        imagePlane,
        implicitBox,
        implicitCone,
        implicitSphere,
        insertKnotCurve,
        insertKnotSurface,
        instancer,
        intersectSurface,
        jiggle,
        joint,
        jointCluster,
        jointFfd,
        jointLattice,
        lambert,
        lattice,
        layeredShader,
        layeredTexture,
        leastSquaresModifier,
        leather,
        light,
        lightFog,
        lightInfo,
        lightLinker,
        lightList,
        lightlists,
        lineModifier,
        locator,
        lodGroup,
        lodThresholds,
        loft,
        lookAt,
        luminance,
        makeCircularArc,
        makeGroup,
        makeIllustratorCurves,
        makeNurbCircle,
        makeNurbCone,
        makeNurbCube,
        makeNurbCylinder,
        makeNurbPlane,
        makeNurbSphere,
        makeNurbsSquare,
        makeNurbTorus,
        makeTextCurves,
        makeThreePointCircularArc,
        makeTwoPointCircularArc,
        marble,
        materialFacade,
        materialInfo,
        membrane,
        mentalrayTexture,
        mesh,
        meshVarGroup,
        mi_bump_flakes,
        mi_car_paint_phen,
        mi_metallic_paint,
        mia_envblur,
        mia_exposure_photographic,
        mia_exposure_simple,
        mia_lens_bokeh,
        mia_light_surface,
        mia_material,
        mia_material_x,
        mia_physicalsky,
        mia_physicalsun,
        mia_portal_light,
        mia_roundcorners,
        mib_amb_occlusion,
        mib_bent_normal_env,
        mib_blackbody,
        mib_bump_basis,
        mib_bump_map,
        mib_cie_d,
        mib_color_alpha,
        mib_color_average,
        mib_color_intensity,
        mib_color_interpolate,
        mib_color_mix,
        mib_color_spread,
        mib_continue,
        mib_dielectric,
        mib_fast_occlusion,
        mib_fg_occlusion,
        mib_geo_add_uv_texsurf,
        mib_geo_cone,
        mib_geo_cube,
        mib_geo_cylinder,
        mib_geo_instance,
        mib_geo_instance_mlist,
        mib_geo_sphere,
        mib_geo_square,
        mib_geo_torus,
        mib_glossy_reflection,
        mib_glossy_refraction,
        mib_illum_blinn,
        mib_illum_cooktorr,
        mib_illum_hair,
        mib_illum_lambert,
        mib_illum_phong,
        mib_illum_ward,
        mib_illum_ward_deriv,
        mib_lens_clamp,
        mib_lens_stencil,
        mib_light_infinite,
        mib_light_photometric,
        mib_light_point,
        mib_light_spot,
        mib_lightmap_sample,
        mib_lightmap_write,
        mib_lookup_background,
        mib_lookup_cylindrical,
        mib_lookup_spherical,
        mib_opacity,
        mib_passthrough_bump_map,
        mib_photon_basic,
        mib_ray_marcher,
        mib_reflect,
        mib_refract,
        mib_refraction_index,
        mib_shadow_transparency,
        mib_texture_checkerboard,
        mib_texture_filter_lookup,
        mib_texture_lookup,
        mib_texture_polkadot,
        mib_texture_polkasphere,
        mib_texture_remap,
        mib_texture_rotate,
        mib_texture_turbulence,
        mib_texture_vector,
        mib_texture_wave,
        mib_transparency,
        mib_twosided,
        mib_volume,
        misss_call_shader,
        misss_fast_shader,
        misss_fast_simple_phen,
        misss_fast_skin_phen,
        misss_fast_skin_phen_d,
        misss_lambert_gamma,
        misss_lightmap_write,
        misss_physical_phen,
        misss_physical_shader,
        misss_skin_specular,
        motionPath,
        mountain,
        movie,
        mpBirailSrf,
        MPxNode,
        multDoubleLinear,
        multilisterLight,
        multiplyDivide,
        multMatrix,
        mute,
        nBase,
        nCloth,
        nComponent,
        network,
        newtonField,
        noise,
        nonAmbientLightShapeNode,
        nonExtendedLightShapeNode,
        nonLinear,
        normalConstraint,
        nParticle,
        nRigid,
        nucleus,
        nurbsCurve,
        nurbsDimShape,
        nurbsSurface,
        nurbsTessellate,
        nurbsToSubdiv,
        nurbsToSubdivProc,
        objectAttrFilter,
        objectBinFilter,
        objectFilter,
        objectMultiFilter,
        objectNameFilter,
        objectRenderFilter,
        objectScriptFilter,
        objectSet,
        objectTypeFilter,
        ocean,
        oceanShader,
        offsetCos,
        offsetCurve,
        offsetSurface,
        oldBlindDataBase,
        opticalFX,
        orientationMarker,
        orientConstraint,
        oversampling_lens,
        pairBlend,
        paramDimension,
        parentConstraint,
        parentTessellate,
        parti_volume,
        parti_volume_photon,
        particle,
        particleAgeMapper,
        particleCloud,
        particleColorMapper,
        particleIncandMapper,
        particleSamplerInfo,
        particleTranspMapper,
        partition,
        passContributionMap,
        passMatrix,
        path_material,
        pfxGeometry,
        pfxHair,
        pfxToon,
        phong,
        phongE,
        physical_lens_dof,
        physical_light,
        place2dTexture,
        place3dTexture,
        planarTrimSurface,
        plane,
        plusMinusAverage,
        pointConstraint,
        pointEmitter,
        pointLight,
        pointMatrixMult,
        pointOnCurveInfo,
        pointOnSurfaceInfo,
        poleVectorConstraint,
        polyAppend,
        polyAppendVertex,
        polyAutoProj,
        polyAverageVertex,
        polyBase,
        polyBevel,
        polyBlindData,
        polyBoolOp,
        polyBridgeEdge,
        polyChipOff,
        polyCloseBorder,
        polyCollapseEdge,
        polyCollapseF,
        polyColorDel,
        polyColorMod,
        polyColorPerVertex,
        polyCone,
        polyCopyUV,
        polyCrease,
        polyCreaseEdge,
        polyCreateFace,
        polyCreator,
        polyCube,
        polyCut,
        polyCylinder,
        polyCylProj,
        polyDelEdge,
        polyDelFacet,
        polyDelVertex,
        polyDuplicateEdge,
        polyEdgeToCurve,
        polyExtrudeEdge,
        polyExtrudeFace,
        polyExtrudeVertex,
        polyFlipEdge,
        polyFlipUV,
        polyHelix,
        polyLayoutUV,
        polyMapCut,
        polyMapDel,
        polyMapSew,
        polyMapSewMove,
        polyMergeEdge,
        polyMergeFace,
        polyMergeUV,
        polyMergeVert,
        polyMirror,
        polyModifier,
        polyModifierUV,
        polyModifierWorld,
        polyMoveEdge,
        polyMoveFace,
        polyMoveFacetUV,
        polyMoveUV,
        polyMoveVertex,
        polyNormal,
        polyNormalizeUV,
        polyNormalPerVertex,
        polyOptUvs,
        polyPipe,
        polyPlanarProj,
        polyPlane,
        polyPlatonicSolid,
        polyPoke,
        polyPrimitive,
        polyPrimitiveMisc,
        polyPrism,
        polyProj,
        polyPyramid,
        polyQuad,
        polyReduce,
        polySeparate,
        polySewEdge,
        polySmooth,
        polySmoothFace,
        polySmoothProxy,
        polySoftEdge,
        polySphere,
        polySphProj,
        polySplit,
        polySplitEdge,
        polySplitRing,
        polySplitVert,
        polyStraightenUVBorder,
        polySubdEdge,
        polySubdFace,
        polyTorus,
        polyToSubdiv,
        polyTransfer,
        polyTriangulate,
        polyTweak,
        polyTweakUV,
        polyUnite,
        polyWedgeFace,
        positionMarker,
        postProcessList,
        precompExport,
        primitive,
        projectCurve,
        projection,
        projectTangent,
        proxyManager,
        psdFileTex,
        quadShadingSwitch,
        radialField,
        ramp,
        rampShader,
        rbfSrf,
        rebuildCurve,
        rebuildSurface,
        record,
        reference,
        reflect,
        remapColor,
        remapHsv,
        remapValue,
        renderBox,
        renderCone,
        renderGlobals,
        renderGlobalsList,
        renderLayer,
        renderLayerManager,
        renderLight,
        renderPass,
        renderPassSet,
        renderQuality,
        renderRect,
        renderSphere,
        resolution,
        resultCurve,
        resultCurveTimeToAngular,
        resultCurveTimeToLinear,
        resultCurveTimeToTime,
        resultCurveTimeToUnitless,
        reverse,
        reverseCurve,
        reverseSurface,
        revolve,
        revolvedPrimitive,
        rgbToHsv,
        rigidBody,
        rigidConstraint,
        rigidSolver,
        rock,
        roundConstantRadius,
        sampler,
        samplerInfo,
        scaleConstraint,
        script,
        sculpt,
        selectionListOperator,
        setRange,
        shaderGlow,
        shadingEngine,
        shadingMap,
        shape,
        shellTessellate,
        simpleVolumeShader,
        singleShadingSwitch,
        sketchPlane,
        skinCluster,
        smear,
        smoothCurve,
        smoothTangentSrf,
        snapshot,
        snapshotShape,
        snow,
        softMod,
        softModHandle,
        solidFractal,
        spBirailSrf,
        spotLight,
        spring,
        squareSrf,
        stencil,
        stereoRigCamera,
        stitchAsNurbsShell,
        stitchSrf,
        stroke,
        strokeGlobals,
        stucco,
        studioClearCoat,
        styleCurve,
        subCurve,
        subdAddTopology,
        subdAutoProj,
        subdBase,
        subdBlindData,
        subdCleanTopology,
        subdHierBlind,
        subdiv,
        subdivCollapse,
        subdivComponentId,
        subdivReverseFaces,
        subdivSurfaceVarGroup,
        subdivToNurbs,
        subdivToPoly,
        subdLayoutUV,
        subdMapCut,
        subdMapSewMove,
        subdModifier,
        subdModifierUV,
        subdModifierWorld,
        subdPlanarProj,
        subdTweak,
        subdTweakUV,
        subSurface,
        surfaceInfo,
        surfaceLuminance,
        surfaceSampler,
        surfaceShader,
        surfaceShape,
        surfaceVarGroup,
        tangentConstraint,
        texture2d,
        texture3d,
        textureBakeSet,
        textureEnv,
        textureToGeom,
        time,
        timeFunction,
        timeToUnitConversion,
        toonLineAttributes,
        transferAttributes,
        transform,
        transformGeometry,
        transmat,
        transmat_photon,
        trim,
        trimWithBoundaries,
        tripleShadingSwitch,
        turbulenceField,
        tweak,
        uniformField,
        unitConversion,
        unitToTimeConversion,
        unknown,
        unknownDag,
        unknownTransform,
        untrim,
        useBackground,
        uvChooser,
        vectorProduct,
        vectorRenderGlobals,
        vertexBakeSet,
        volumeAxisField,
        volumeFog,
        volumeLight,
        volumeNoise,
        volumeShader,
        vortexField,
        water,
        weightGeometryFilter,
        wire,
        wood,
        wrap,
        writeToColorBuffer,
        writeToDepthBuffer,
        writeToFrameBuffer,
        writeToLabelBuffer,
        writeToVectorBuffer,
        wtAddMatrix,
      }

      public enum AttributeType
      {
        numericDefault,
        attributeAlias,
        componentList,
        cone,
        double2,
        double3,
        doubleArray,
        float2,
        float3,
        int32Array,
        lattice,
        long2,
        long3,
        matrix,
        mesh,
        nurbsCurve,
        nurbsSurface,
        nurbsTrimface,
        pointArray,
        polyFace,
        reflectanceRGB,
        short2,
        short3,
        spectrumRGB,
        sphere,
        @string,
        stringArray,
        vectorArray,
      }
    }
  }
}
