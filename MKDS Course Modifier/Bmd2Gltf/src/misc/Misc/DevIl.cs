// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Misc.DevIl
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Tao.DevIl;

namespace MKDS_Course_Modifier.Misc
{
  public class DevIl
  {
    public static void SaveAsTGA(Bitmap b, string FilePath)
    {
      Il.ilInit();
      int Images;
      Il.ilGenImages(1, out Images);
      Il.ilBindImage(Images);
      MemoryStream memoryStream = new MemoryStream();
      b.Save((Stream) memoryStream, ImageFormat.Png);
      Il.ilLoadL(1066, memoryStream.ToArray(), (int) memoryStream.Length);
      Il.ilEnable(1568);
      Il.ilSave(1069, FilePath);
      Il.ilDeleteImages(1, ref Images);
    }

    public static byte[] GetDXT1Data(Bitmap b)
    {
      Il.ilInit();
      int Images;
      Il.ilGenImages(1, out Images);
      Il.ilBindImage(Images);
      MemoryStream memoryStream = new MemoryStream();
      b.Save((Stream) memoryStream, ImageFormat.Png);
      Il.ilLoadL(1066, memoryStream.ToArray(), (int) memoryStream.Length);
      int dxtcData = Il.ilGetDXTCData(IntPtr.Zero, 0, 1798);
      byte[] destination = new byte[dxtcData];
      IntPtr num = Marshal.AllocHGlobal(dxtcData);
      Il.ilGetDXTCData(num, dxtcData, 1798);
      Marshal.Copy(num, destination, 0, dxtcData);
      Il.ilDeleteImages(1, ref Images);
      return destination;
    }
  }
}
