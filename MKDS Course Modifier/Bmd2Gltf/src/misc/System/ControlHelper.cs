// Decompiled with JetBrains decompiler
// Type: System.ControlHelper
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace System
{
  public static class ControlHelper
  {
    private const int WM_SETREDRAW = 11;

    [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

    public static void SuspendDrawing(this Control target)
    {
      ControlHelper.SendMessage(target.Handle, 11, 0, 0);
    }

    public static void ResumeDrawing(this Control target)
    {
      target.ResumeDrawing(true);
    }

    public static void ResumeDrawing(this Control target, bool redraw)
    {
      ControlHelper.SendMessage(target.Handle, 11, 1, 0);
      if (!redraw)
        return;
      target.Refresh();
    }
  }
}
