// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.Controls.dataGridViewNF
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.Controls
{
  public class dataGridViewNF : DataGridView
  {
    public dataGridViewNF()
    {
      this.SetStyle(ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
      this.HorizontalScrollBar.SmallChange = 8;
      this.HorizontalScrollBar.LargeChange = 8;
    }
  }
}
