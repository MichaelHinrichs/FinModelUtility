// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.GlobalMapGenerator
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G2D_Binary_File_Format;
using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.Properties;
using OpenTK;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class GlobalMapGenerator : Form
  {
    private IContainer components = (IContainer) null;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private OpenFileDialog openFileDialog1;
    private ToolStripButton toolStripButton2;
    private ToolStripButton toolStripButton3;
    private ToolStripButton toolStripButton4;
    private ToolStripButton toolStripButton5;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private TextBox textBox1;
    private TextBox textBox2;
    private TextBox textBox3;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private TextBox textBox4;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton toolStripButton6;
    private ToolStripButton toolStripButton7;
    private SaveFileDialog saveFileDialog1;
    private SaveFileDialog saveFileDialog2;
    private ToolStripButton toolStripButton8;
    private ToolStripSeparator toolStripSeparator2;
    private OpenFileDialog openFileDialog2;
    private KCL k;
    private MKDS_Course_Modifier.MKDS.NKM m;
    private byte[] img;
    private byte[] map;
    private Color[] pal;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (GlobalMapGenerator));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripButton4 = new ToolStripButton();
      this.toolStripButton5 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton6 = new ToolStripButton();
      this.toolStripButton7 = new ToolStripButton();
      this.openFileDialog1 = new OpenFileDialog();
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.textBox1 = new TextBox();
      this.textBox2 = new TextBox();
      this.textBox3 = new TextBox();
      this.label1 = new Label();
      this.label2 = new Label();
      this.label3 = new Label();
      this.label4 = new Label();
      this.textBox4 = new TextBox();
      this.saveFileDialog1 = new SaveFileDialog();
      this.saveFileDialog2 = new SaveFileDialog();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStripButton8 = new ToolStripButton();
      this.openFileDialog2 = new OpenFileDialog();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[10]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripButton8,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripButton4,
        (ToolStripItem) this.toolStripButton5,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton6,
        (ToolStripItem) this.toolStripButton7
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(430, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripButton2.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.toolStripButton3.CheckOnClick = true;
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Enabled = false;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton4.CheckOnClick = true;
      this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton4.Enabled = false;
      this.toolStripButton4.Image = (Image) componentResourceManager.GetObject("toolStripButton4.Image");
      this.toolStripButton4.ImageTransparentColor = Color.Magenta;
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.Size = new Size(23, 22);
      this.toolStripButton4.Text = "toolStripButton4";
      this.toolStripButton5.CheckOnClick = true;
      this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton5.Enabled = false;
      this.toolStripButton5.Image = (Image) componentResourceManager.GetObject("toolStripButton5.Image");
      this.toolStripButton5.ImageTransparentColor = Color.Magenta;
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.Size = new Size(23, 22);
      this.toolStripButton5.Text = "toolStripButton5";
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton6.Image = (Image) componentResourceManager.GetObject("toolStripButton6.Image");
      this.toolStripButton6.ImageTransparentColor = Color.Magenta;
      this.toolStripButton6.Name = "toolStripButton6";
      this.toolStripButton6.Size = new Size(23, 22);
      this.toolStripButton6.Text = "toolStripButton6";
      this.toolStripButton6.Click += new EventHandler(this.toolStripButton6_Click);
      this.toolStripButton7.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton7.Image = (Image) componentResourceManager.GetObject("toolStripButton7.Image");
      this.toolStripButton7.ImageTransparentColor = Color.Magenta;
      this.toolStripButton7.Name = "toolStripButton7";
      this.toolStripButton7.Size = new Size(23, 22);
      this.toolStripButton7.Text = "toolStripButton7";
      this.toolStripButton7.Click += new EventHandler(this.toolStripButton7_Click);
      this.openFileDialog1.DefaultExt = "kcl";
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "Nintendo Collision Files(*.kcl)|*.kcl";
      this.simpleOpenGlControl1.AccumBits = (byte) 0;
      this.simpleOpenGlControl1.AutoCheckErrors = false;
      this.simpleOpenGlControl1.AutoFinish = false;
      this.simpleOpenGlControl1.AutoMakeCurrent = true;
      this.simpleOpenGlControl1.AutoSwapBuffers = true;
      this.simpleOpenGlControl1.BackColor = SystemColors.Control;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 24;
      this.simpleOpenGlControl1.Location = new Point(158, 28);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(256, 192);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 2;
      this.textBox1.Location = new Point(52, 28);
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.Size = new Size(100, 20);
      this.textBox1.TabIndex = 3;
      this.textBox2.Location = new Point(52, 89);
      this.textBox2.Name = "textBox2";
      this.textBox2.ReadOnly = true;
      this.textBox2.Size = new Size(100, 20);
      this.textBox2.TabIndex = 4;
      this.textBox3.Location = new Point(52, 54);
      this.textBox3.Name = "textBox3";
      this.textBox3.ReadOnly = true;
      this.textBox3.Size = new Size(100, 20);
      this.textBox3.TabIndex = 5;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(12, 31);
      this.label1.Name = "label1";
      this.label1.Size = new Size(34, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Min X";
      this.label2.AutoSize = true;
      this.label2.Location = new Point(9, 57);
      this.label2.Name = "label2";
      this.label2.Size = new Size(37, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Max X";
      this.label3.AutoSize = true;
      this.label3.Location = new Point(12, 92);
      this.label3.Name = "label3";
      this.label3.Size = new Size(34, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Min Y";
      this.label4.AutoSize = true;
      this.label4.Location = new Point(9, 118);
      this.label4.Name = "label4";
      this.label4.Size = new Size(37, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Max Y";
      this.textBox4.Location = new Point(52, 115);
      this.textBox4.Name = "textBox4";
      this.textBox4.ReadOnly = true;
      this.textBox4.Size = new Size(100, 20);
      this.textBox4.TabIndex = 10;
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
      this.toolStripButton8.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton8.Enabled = false;
      this.toolStripButton8.Image = (Image) componentResourceManager.GetObject("toolStripButton8.Image");
      this.toolStripButton8.ImageTransparentColor = Color.Magenta;
      this.toolStripButton8.Name = "toolStripButton8";
      this.toolStripButton8.Size = new Size(23, 22);
      this.toolStripButton8.Text = "toolStripButton8";
      this.toolStripButton8.Click += new EventHandler(this.toolStripButton8_Click);
      this.openFileDialog2.DefaultExt = "nkm";
      this.openFileDialog2.FileName = "openFileDialog2";
      this.openFileDialog2.Filter = "Nitro Kart Model(*.nkm)|*.nkm";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(430, 252);
      this.Controls.Add((Control) this.textBox4);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.textBox3);
      this.Controls.Add((Control) this.textBox2);
      this.Controls.Add((Control) this.textBox1);
      this.Controls.Add((Control) this.simpleOpenGlControl1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (GlobalMapGenerator);
      this.Text = nameof (GlobalMapGenerator);
      this.Load += new EventHandler(this.GlobalMapGenerator_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public GlobalMapGenerator()
    {
      this.InitializeComponent();
      this.simpleOpenGlControl1.InitializeContexts();
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      try
      {
        this.k = new KCL(System.IO.File.ReadAllBytes(this.openFileDialog1.FileName));
      }
      catch
      {
      }
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      float num1 = float.MaxValue;
      float num2 = float.MaxValue;
      float num3 = float.MinValue;
      float num4 = float.MinValue;
      Vector3 PositionA;
      Vector3 PositionB;
      Vector3 PositionC;
      for (int PlaneIdx = 0; PlaneIdx < this.k.Planes.Length; ++PlaneIdx)
      {
        if (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 0 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 1 || (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 3 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 7) || (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 8 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 18) || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 19)
        {
          KCL.GetTriangle(this.k, PlaneIdx, out PositionA, out PositionB, out PositionC);
          if ((double) PositionA.X < (double) num1 && (double) PositionA.X > (double) short.MinValue)
            num1 = PositionA.X;
          if ((double) PositionA.X > (double) num3 && (double) PositionA.X < (double) short.MaxValue)
            num3 = PositionA.X;
          if ((double) PositionB.X < (double) num1 && (double) PositionB.X > (double) short.MinValue)
            num1 = PositionB.X;
          if ((double) PositionB.X > (double) num3 && (double) PositionB.X < (double) short.MaxValue)
            num3 = PositionB.X;
          if ((double) PositionC.X < (double) num1 && (double) PositionC.X > (double) short.MinValue)
            num1 = PositionC.X;
          if ((double) PositionC.X > (double) num3 && (double) PositionC.X < (double) short.MaxValue)
            num3 = PositionC.X;
          if ((double) PositionA.Z < (double) num2 && (double) PositionA.Z > (double) short.MinValue)
            num2 = PositionA.Z;
          if ((double) PositionA.Z > (double) num4 && (double) PositionA.Z < (double) short.MaxValue)
            num4 = PositionA.Z;
          if ((double) PositionB.Z < (double) num2 && (double) PositionB.Z > (double) short.MinValue)
            num2 = PositionB.Z;
          if ((double) PositionB.Z > (double) num4 && (double) PositionB.Z < (double) short.MaxValue)
            num4 = PositionB.Z;
          if ((double) PositionC.Z < (double) num2 && (double) PositionC.Z > (double) short.MinValue)
            num2 = PositionC.Z;
          if ((double) PositionC.Z > (double) num4 && (double) PositionC.Z < (double) short.MaxValue)
            num4 = PositionC.Z;
        }
      }
      float num5 = (float) (int) num1;
      float num6 = (float) (int) num2;
      float num7 = (float) (int) num3;
      float num8 = (float) (int) num4;
      float val1_1 = num5 - 512f;
      float val2_1 = num6 - 512f;
      float val1_2 = num7 + 512f;
      float val2_2 = num8 + 512f;
      this.textBox1.Text = ((int) ((double) Math.Min(val1_1, val2_1) * 1.33333337306976)).ToString();
      this.textBox3.Text = ((int) ((double) Math.Max(val1_2, val2_2) * 1.33333337306976)).ToString();
      TextBox textBox2 = this.textBox2;
      float num9 = Math.Min(val1_1, val2_1);
      string str1 = num9.ToString();
      textBox2.Text = str1;
      TextBox textBox4 = this.textBox4;
      num9 = Math.Max(val1_2, val2_2);
      string str2 = num9.ToString();
      textBox4.Text = str2;
      Gl.glOrtho((double) Math.Min(val1_1, val2_1) * 1.33333337306976, (double) Math.Max(val1_2, val2_2) * 1.33333337306976, (double) Math.Max(val1_2, val2_2), (double) Math.Min(val1_1, val2_1), -8192.0, 8192.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glClearColor(0.0f, 0.0627451f, 0.3764706f, 1f);
      Gl.glClear(16640);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glEnable(3553);
      Gl.glBindTexture(3553, 0);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glDisable(2884);
      Gl.glEnable(3008);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glAlphaFunc(519, 0.0f);
      Gl.glDepthFunc(515);
      Gl.glPolygonMode(1032, 6913);
      Gl.glLineWidth(2f);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      for (int PlaneIdx = 0; PlaneIdx < this.k.Planes.Length; ++PlaneIdx)
      {
        if (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 0 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 1 || (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 3 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 7) || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 18 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 19)
        {
          KCL.GetTriangle(this.k, PlaneIdx, out PositionA, out PositionB, out PositionC);
          Gl.glBegin(4);
          Gl.glVertex3f(PositionA.X, PositionA.Z, PositionA.Y - 100f);
          Gl.glVertex3f(PositionB.X, PositionB.Z, PositionB.Y - 100f);
          Gl.glVertex3f(PositionC.X, PositionC.Z, PositionC.Y - 100f);
          Gl.glEnd();
        }
      }
      Gl.glDisable(10754);
      Gl.glPolygonMode(1032, 6914);
      Gl.glColor3f(0.4705882f, 0.4705882f, 0.4705882f);
      for (int PlaneIdx = 0; PlaneIdx < this.k.Planes.Length; ++PlaneIdx)
      {
        if (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 0 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 1 || (((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 3 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 7) || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 18 || ((int) this.k.Planes[PlaneIdx].Type >> 8 & 31) == 19)
        {
          KCL.GetTriangle(this.k, PlaneIdx, out PositionA, out PositionB, out PositionC);
          Gl.glBegin(4);
          Gl.glVertex3f(PositionA.X, PositionA.Z, PositionA.Y);
          Gl.glVertex3f(PositionB.X, PositionB.Z, PositionB.Y);
          Gl.glVertex3f(PositionC.X, PositionC.Z, PositionC.Y);
          Gl.glEnd();
        }
      }
      this.img = (byte[]) null;
      this.map = (byte[]) null;
      System.Drawing.Bitmap b = GlNitro2.ScreenShot(this.simpleOpenGlControl1);
      b.MakeTransparent(Color.FromArgb(0, 16, 96));
      Graphic.ConvertBitmap(b, this.pal, out this.img, out this.map, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, true);
      b.Dispose();
      this.simpleOpenGlControl1.Refresh();
    }

    private void GlobalMapGenerator_Load(object sender, EventArgs e)
    {
      this.pal = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(Resources.map1).PaletteData.ToColorArray();
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glDepthFunc(519);
      Gl.glEnable(3057);
      Gl.glDisable(2884);
      Gl.glEnable(3553);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
    }

    private void toolStripButton6_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      System.IO.File.Create(this.saveFileDialog1.FileName).Close();
      System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, new NCGR(this.img, 0, 0, Graphic.GXTexFmt.GX_TEXFMT_PLTT16).Write());
    }

    private void toolStripButton7_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog2.ShowDialog() != DialogResult.OK || this.saveFileDialog2.FileName.Length <= 0)
        return;
      System.IO.File.Create(this.saveFileDialog2.FileName).Close();
      System.IO.File.WriteAllBytes(this.saveFileDialog2.FileName, new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(this.map, 256, 256, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16).Write());
    }

    private void toolStripButton8_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      try
      {
        this.m = new MKDS_Course_Modifier.MKDS.NKM(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      }
      catch
      {
      }
    }
  }
}
