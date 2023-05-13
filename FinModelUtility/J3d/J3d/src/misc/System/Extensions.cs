// Decompiled with JetBrains decompiler
// Type: System.Extensions
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Drawing;
using System.IO;


namespace System {
  public static class Extensions {
    public static Color ReadColor16(this IEndianBinaryReader er) {
      short num1 = er.ReadInt16();
      short num2 = er.ReadInt16();
      short num3 = er.ReadInt16();
      return Color.FromArgb((int)Math.Abs(er.ReadInt16()) & (int)byte.MaxValue,
                            (int)Math.Abs(num1) & (int)byte.MaxValue,
                            (int)Math.Abs(num2) & (int)byte.MaxValue,
                            (int)Math.Abs(num3) & (int)byte.MaxValue);
    }

    public static Color ReadColor8(this IEndianBinaryReader er) {
      int red = (int)er.ReadByte();
      int green = (int)er.ReadByte();
      int blue = (int)er.ReadByte();
      return Color.FromArgb((int)er.ReadByte(), red, green, blue);
    }
  }
}