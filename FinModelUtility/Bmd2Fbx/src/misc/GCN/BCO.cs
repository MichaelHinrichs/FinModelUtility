// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.GCN.BCO
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System.Collections.Generic;
using System.IO;

namespace bmd.GCN
{
  public class BCO
  {
    public static void Read(string infile, string outfile)
    {
      EndianBinaryReader endianBinaryReader = new EndianBinaryReader((Stream) File.OpenRead(infile), Endianness.BigEndian);
      endianBinaryReader.BaseStream.Position = 36L;
      uint num1 = endianBinaryReader.ReadUInt32();
      uint num2 = endianBinaryReader.ReadUInt32();
      endianBinaryReader.BaseStream.Position = (long) num1;
      List<Vector3> vector3List1 = new List<Vector3>();
      List<ushort> ushortList = new List<ushort>();
      while (endianBinaryReader.BaseStream.Position != (long) num2)
        vector3List1.Add(new Vector3(endianBinaryReader.ReadSingle(), endianBinaryReader.ReadSingle(), endianBinaryReader.ReadSingle()));
      endianBinaryReader.BaseStream.Position = 32L;
      uint num3 = endianBinaryReader.ReadUInt32();
      uint num4 = endianBinaryReader.ReadUInt32();
      endianBinaryReader.BaseStream.Position = (long) num3;
      List<Vector3> vector3List2 = new List<Vector3>();
      while (endianBinaryReader.BaseStream.Position != (long) num4)
      {
        vector3List2.Add(new Vector3((float) endianBinaryReader.ReadUInt32(), (float) endianBinaryReader.ReadUInt32(), (float) endianBinaryReader.ReadUInt32()));
        endianBinaryReader.ReadBytes(10);
        ushort num5 = endianBinaryReader.ReadUInt16();
        ushortList.Add(num5);
        endianBinaryReader.ReadBytes(12);
      }
      endianBinaryReader.Close();
      File.Create(outfile).Close();
      TextWriter textWriter1 = (TextWriter) new StreamWriter(outfile);
      textWriter1.WriteLine("# Created by MKDS Course Modifier");
      try
      {
        for (int index = 0; index < vector3List1.Count; ++index)
        {
          TextWriter textWriter2 = textWriter1;
          float num5 = vector3List1[index].X / 7.5f;
          string str1 = num5.ToString().Replace(",", ".");
          num5 = vector3List1[index].Y / 7.5f;
          string str2 = num5.ToString().Replace(",", ".");
          num5 = vector3List1[index].Z / 7.5f;
          string str3 = num5.ToString().Replace(",", ".");
          textWriter2.WriteLine("v {0} {1} {2}", (object) str1, (object) str2, (object) str3);
        }
        for (int index = 0; index < vector3List2.Count; ++index)
        {
          textWriter1.WriteLine("usemtl " + ushortList[index].ToString());
          textWriter1.WriteLine("f {0} {1} {2}", (object) (float) ((double) vector3List2[index].X + 1.0), (object) (float) ((double) vector3List2[index].Y + 1.0), (object) (float) ((double) vector3List2[index].Z + 1.0));
        }
        textWriter1.Close();
      }
      catch
      {
        textWriter1.Close();
      }
    }
  }
}
