// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.Test
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.UI.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class Test : Form
  {
    private IContainer components = (IContainer) null;
    private TimeLine timeLine1;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.timeLine1 = new TimeLine();
      this.SuspendLayout();
      this.timeLine1.CurFrame = 0U;
      this.timeLine1.Dock = DockStyle.Fill;
      this.timeLine1.Location = new Point(0, 0);
      this.timeLine1.Name = "timeLine1";
      this.timeLine1.NrFrames = 100U;
      this.timeLine1.Size = new Size(645, 262);
      this.timeLine1.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(645, 262);
      this.Controls.Add((Control) this.timeLine1);
      this.Name = nameof (Test);
      this.Text = nameof (Test);
      this.ResumeLayout(false);
    }

    public Test()
    {
      this.InitializeComponent();
    }

    private void timeLine1_Load(object sender, EventArgs e)
    {
    }
  }
}
