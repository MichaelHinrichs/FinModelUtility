// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.OBJVertexColor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class OBJVertexColor : Form
  {
    private IContainer components = (IContainer) null;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(536, 361);
      this.Name = nameof (OBJVertexColor);
      this.Text = nameof (OBJVertexColor);
      this.ResumeLayout(false);
    }

    public OBJVertexColor()
    {
      this.InitializeComponent();
    }
  }
}
