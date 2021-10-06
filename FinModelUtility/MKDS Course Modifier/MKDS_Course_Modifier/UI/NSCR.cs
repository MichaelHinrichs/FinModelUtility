// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NSCR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G2D_Binary_File_Format;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class NSCR : Form
  {
    private IContainer components = (IContainer) null;
    private ToolStrip toolStrip1;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR file;
    private NCGR NCGR;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR NCLR;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.toolStrip1 = new ToolStrip();
      this.SuspendLayout();
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(554, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(554, 416);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (NSCR);
      this.Text = nameof (NSCR);
      this.Load += new EventHandler(this.NSCR_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public NSCR(MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR NSCR, NCGR NCGR, MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR NCLR)
    {
      this.file = NSCR;
      this.NCGR = NCGR;
      this.NCLR = NCLR;
      this.InitializeComponent();
    }

    private void NSCR_Load(object sender, EventArgs e)
    {
    }
  }
}
