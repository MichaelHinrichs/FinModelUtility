// Decompiled with JetBrains decompiler
// Type: System.Extensions
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace System
{
  public static class Extensions
  {
    public static Dictionary<string, long> Markers = new Dictionary<string, long>();

    public static void SetMarkerOnCurrentOffset(this EndianBinaryReader er, string Name)
    {
      Extensions.Markers.Add(Name, er.BaseStream.Position);
    }

    public static void SetMarker(this EndianBinaryReader er, string Name, long Offset)
    {
      Extensions.Markers.Add(Name, Offset);
    }

    public static long GetMarker(this EndianBinaryReader er, string Name)
    {
      return Extensions.Markers[Name];
    }

    public static void RemoveMarker(this EndianBinaryReader er, string Name)
    {
      Extensions.Markers.Remove(Name);
    }

    public static void ClearMarkers(this EndianBinaryReader er)
    {
      Extensions.Markers.Clear();
    }

    public static float ReadSingleInt32Exp12(this EndianBinaryReader er)
    {
      return (float) er.ReadInt32() / 4096f;
    }

    public static float[] ReadSingleInt32Exp12s(this EndianBinaryReader er, int count)
    {
      float[] numArray = new float[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = er.ReadSingleInt32Exp12();
      return numArray;
    }

    public static float ReadSingleInt16Exp12(this EndianBinaryReader er)
    {
      return (float) er.ReadInt16() / 4096f;
    }

    public static float[] ReadSingleInt16Exp12s(this EndianBinaryReader er, int count)
    {
      float[] numArray = new float[count];
      for (int index = 0; index < count; ++index)
        numArray[index] = er.ReadSingleInt16Exp12();
      return numArray;
    }

    public static int ReadVariableLength(this EndianBinaryReader er)
    {
      byte num1 = er.ReadByte();
      int num2 = (int) num1 & (int) sbyte.MaxValue;
      while (er.BaseStream.Position < er.BaseStream.Length && ((int) num1 & 128) != 0)
      {
        int num3 = num2 << 7;
        num1 = er.ReadByte();
        num2 = num3 | (int) num1 & (int) sbyte.MaxValue;
      }
      return num2;
    }

    public static Color ReadColor4Singles(this EndianBinaryReader er)
    {
      int red = (int) ((double) er.ReadSingle() * (double) byte.MaxValue);
      int green = (int) ((double) er.ReadSingle() * (double) byte.MaxValue);
      int blue = (int) ((double) er.ReadSingle() * (double) byte.MaxValue);
      return Color.FromArgb((int) ((double) er.ReadSingle() * (double) byte.MaxValue), red, green, blue);
    }

    public static Color ReadColor16(this EndianBinaryReader er)
    {
      short num1 = er.ReadInt16();
      short num2 = er.ReadInt16();
      short num3 = er.ReadInt16();
      return Color.FromArgb((int) Math.Abs(er.ReadInt16()) & (int) byte.MaxValue, (int) Math.Abs(num1) & (int) byte.MaxValue, (int) Math.Abs(num2) & (int) byte.MaxValue, (int) Math.Abs(num3) & (int) byte.MaxValue);
    }

    public static Color ReadColor8(this EndianBinaryReader er)
    {
      int red = (int) er.ReadByte();
      int green = (int) er.ReadByte();
      int blue = (int) er.ReadByte();
      return Color.FromArgb((int) er.ReadByte(), red, green, blue);
    }

    private static int smfGetVarLengthSize(int value)
    {
      int num = 1;
      for (int index = value; index > (int) sbyte.MaxValue && num < 4; index >>= 7)
        ++num;
      return num;
    }

    public static unsafe long IndexOf(this byte[] Haystack, byte[] Needle)
    {
      fixed (byte* numPtr1 = Haystack)
        fixed (byte* numPtr2 = Needle)
        {
          long num = 0;
          byte* numPtr3 = numPtr1;
          for (byte* numPtr4 = numPtr1 + Haystack.LongLength; numPtr3 < numPtr4; ++numPtr3)
          {
            bool flag = true;
            byte* numPtr5 = numPtr3;
            byte* numPtr6 = numPtr2;
            byte* numPtr7 = numPtr2 + Needle.LongLength;
            while (flag && numPtr6 < numPtr7)
            {
              flag = (int) *numPtr6 == (int) *numPtr5;
              ++numPtr6;
              ++numPtr5;
            }
            if (flag)
              return num;
            ++num;
          }
          return -1;
        }
    }

    public static unsafe List<long> IndexesOf(this byte[] Haystack, byte[] Needle)
    {
      List<long> longList = new List<long>();
      fixed (byte* numPtr1 = Haystack)
        fixed (byte* numPtr2 = Needle)
        {
          long num = 0;
          byte* numPtr3 = numPtr1;
          for (byte* numPtr4 = numPtr1 + Haystack.LongLength; numPtr3 < numPtr4; ++numPtr3)
          {
            bool flag = true;
            byte* numPtr5 = numPtr3;
            byte* numPtr6 = numPtr2;
            byte* numPtr7 = numPtr2 + Needle.LongLength;
            while (flag && numPtr6 < numPtr7)
            {
              flag = (int) *numPtr6 == (int) *numPtr5;
              ++numPtr6;
              ++numPtr5;
            }
            if (flag)
              longList.Add(num);
            ++num;
          }
          return longList;
        }
    }

    public static Vector3 ToVector3(this Color c)
    {
      return new Vector3((float) c.R / (float) byte.MaxValue, (float) c.G / (float) byte.MaxValue, (float) c.B / (float) byte.MaxValue);
    }
  }
}
