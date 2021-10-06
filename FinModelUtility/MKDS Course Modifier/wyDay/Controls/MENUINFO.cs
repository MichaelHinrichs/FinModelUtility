// Decompiled with JetBrains decompiler
// Type: wyDay.Controls.MENUINFO
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Runtime.InteropServices;

namespace wyDay.Controls
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public class MENUINFO
  {
    public int cbSize = Marshal.SizeOf(typeof (MENUINFO));
    public int fMask = 16;
    public int dwStyle = 67108864;
    public IntPtr hbrBack = IntPtr.Zero;
    public IntPtr dwMenuData = IntPtr.Zero;
    public uint cyMax;
    public int dwContextHelpID;
  }
}
