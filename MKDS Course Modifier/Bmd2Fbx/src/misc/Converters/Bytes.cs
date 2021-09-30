// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Bytes
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace bmd.Converters
{
  public class Bytes
  {
    public static ushort Read2BytesAsUInt16(byte[] bytes, int offset)
    {
      int num = 0;
      for (int index = 0; index < 2; ++index)
        num |= (int) bytes[offset + index] << 8 * index;
      return (ushort) num;
    }

    public static short Read2BytesAsInt16(byte[] bytes, int offset)
    {
      int num = 0;
      for (int index = 0; index < 2; ++index)
        num |= (int) bytes[offset + index] << 8 * index;
      return (short) num;
    }

    public static int Read3BytesAsInt24(byte[] bytes, int offset)
    {
      int num = 0;
      for (int index = 0; index < 3; ++index)
        num |= (int) bytes[offset + index] << 8 * index;
      return num;
    }

    public static int Read4BytesAsInt32(byte[] bytes, int offset)
    {
      int num = 0;
      for (int index = 0; index < 4; ++index)
        num |= (int) bytes[offset + index] << 8 * index;
      return num;
    }

    public static uint Read4BytesAsUInt32(byte[] bytes, int offset)
    {
      uint num = 0;
      for (int index = 0; index < 4; ++index)
        num |= (uint) bytes[offset + index] << 8 * index;
      return num;
    }

    public static uint[] ReadMultipleBytesAsUInt32s(byte[] bytes, int offset, int nr)
    {
      List<uint> uintList = new List<uint>();
      for (int index = 0; index < nr; ++index)
        uintList.Add(Bytes.Read4BytesAsUInt32(bytes, offset + index * 4));
      return uintList.ToArray();
    }

    private static string DecToHex(double DecNum)
    {
      string str = "";
      for (; DecNum != 0.0; DecNum /= 16.0)
      {
        double num = DecNum % 16.0;
        str = num > 9.0 ? Strings.Chr(Strings.Asc("A") + (int) num - 10).ToString() + str : Strings.Chr(Strings.Asc(num.ToString())).ToString() + str;
      }
      if (string.IsNullOrEmpty(str))
        str = "0";
      return str;
    }

    public static byte[] HexConverter(float ValueRead)
    {
      double num1 = (double) ValueRead;
      bool flag1 = num1 >= 0.0;
      double num2 = Math.Abs(num1);
      bool flag2 = false;
      if (num2 >= 4096.0)
      {
        flag2 = true;
        num2 -= 4096.0;
      }
      double num3 = num2 * 4096.0;
      if (!flag1)
        num3 = 16777216.0 - num3;
      string str = Conversion.Hex((object) num3);
      if (Strings.Len(str) == 1)
        str = "00000" + str;
      if (Strings.Len(str) == 2)
        str = "0000" + str;
      if (Strings.Len(str) == 3)
        str = "000" + str;
      if (Strings.Len(str) == 4)
        str = "00" + str;
      if (Strings.Len(str) == 5)
        str = "0" + str;
      return Bytes.GetBytes(Strings.Mid(str, 5, 2) + Strings.Mid(str, 3, 2) + Strings.Mid(str, 1, 2) + (!flag1 ? (!flag2 ? "FF" : "FE") : (!flag2 ? "00" : "01")), out int _);
    }

    public static byte[] StringToByte(string a)
    {
      return Bytes.GetBytes(a, out int _);
    }

    private static byte[] GetBytes(string hexString, out int discarded)
    {
      discarded = 0;
      string str = "";
      for (int index = 0; index < hexString.Length; ++index)
      {
        char c = hexString[index];
        if (Bytes.IsHexDigit(c))
          str += (string) (object) c;
        else
          ++discarded;
      }
      if (str.Length % 2 != 0)
      {
        ++discarded;
        str = str.Substring(0, str.Length - 1);
      }
      byte[] numArray = new byte[str.Length / 2];
      int index1 = 0;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        string hex = new string(new char[2]
        {
          str[index1],
          str[index1 + 1]
        });
        numArray[index2] = Bytes.HexToByte(hex);
        index1 += 2;
      }
      return numArray;
    }

    private static bool IsHexDigit(char c)
    {
      int int32_1 = Convert.ToInt32('A');
      int int32_2 = Convert.ToInt32('0');
      c = char.ToUpper(c);
      int int32_3 = Convert.ToInt32(c);
      return int32_3 >= int32_1 && int32_3 < int32_1 + 6 || int32_3 >= int32_2 && int32_3 < int32_2 + 10;
    }

    private static byte HexToByte(string hex)
    {
      if (hex.Length > 2 || hex.Length <= 0)
        throw new ArgumentException("hex must be 1 or 2 characters in length");
      return byte.Parse(hex, NumberStyles.HexNumber);
    }
  }
}
