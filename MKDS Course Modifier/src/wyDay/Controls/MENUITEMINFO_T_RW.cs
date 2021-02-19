// Decompiled with JetBrains decompiler
// Type: wyDay.Controls.MENUITEMINFO_T_RW
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Runtime.InteropServices;

namespace wyDay.Controls
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public class MENUITEMINFO_T_RW
  {
    public int cbSize = Marshal.SizeOf(typeof (MENUITEMINFO_T_RW));
    public int fMask = 128;
    public IntPtr hSubMenu = IntPtr.Zero;
    public IntPtr hbmpChecked = IntPtr.Zero;
    public IntPtr hbmpUnchecked = IntPtr.Zero;
    public IntPtr dwItemData = IntPtr.Zero;
    public IntPtr dwTypeData = IntPtr.Zero;
    public IntPtr hbmpItem = IntPtr.Zero;
    public int fType;
    public int fState;
    public int wID;
    public int cch;
  }
}
