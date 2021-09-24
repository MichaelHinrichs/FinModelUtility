// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.BNCL
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G2D_Binary_File_Format;
using MKDS_Course_Modifier.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class BNCL : Form
  {
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR Palette = (MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR) null;
    private NCGR Graphic = (NCGR) null;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NCER Cell = (MKDS_Course_Modifier.G2D_Binary_File_Format.NCER) null;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private SplitContainer splitContainer1;
    private PropertyGrid propertyGrid1;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private TabPage tabPage2;
    private MKDS_Course_Modifier.Misc.BNCL Bncl;
    private byte[] pic;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (BNCL));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.splitContainer1 = new SplitContainer();
      this.propertyGrid1 = new PropertyGrid();
      this.tabControl1 = new TabControl();
      this.tabPage1 = new TabPage();
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.tabPage2 = new TabPage();
      this.toolStrip1.SuspendLayout();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripButton1
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(671, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.tabControl1);
      this.splitContainer1.Size = new Size(671, 359);
      this.splitContainer1.SplitterDistance = 181;
      this.splitContainer1.TabIndex = 1;
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(181, 359);
      this.propertyGrid1.TabIndex = 0;
      this.tabControl1.Controls.Add((Control) this.tabPage1);
      this.tabControl1.Controls.Add((Control) this.tabPage2);
      this.tabControl1.Dock = DockStyle.Fill;
      this.tabControl1.Location = new Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new Size(486, 359);
      this.tabControl1.TabIndex = 0;
      this.tabPage1.Controls.Add((Control) this.simpleOpenGlControl1);
      this.tabPage1.Location = new Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Size = new Size(478, 333);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Layout";
      this.tabPage1.UseVisualStyleBackColor = true;
      this.simpleOpenGlControl1.AccumBits = (byte) 0;
      this.simpleOpenGlControl1.AutoCheckErrors = false;
      this.simpleOpenGlControl1.AutoFinish = false;
      this.simpleOpenGlControl1.AutoMakeCurrent = true;
      this.simpleOpenGlControl1.AutoSwapBuffers = true;
      this.simpleOpenGlControl1.BackColor = Color.Black;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 16;
      this.simpleOpenGlControl1.Dock = DockStyle.Fill;
      this.simpleOpenGlControl1.Location = new Point(0, 0);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(478, 333);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.Resize += new EventHandler(this.simpleOpenGlControl1_Resize);
      this.tabPage2.Location = new Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new Padding(3);
      this.tabPage2.Size = new Size(478, 333);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = nameof (BNCL);
      this.tabPage2.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(671, 384);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (BNCL);
      this.Text = nameof (BNCL);
      this.Load += new EventHandler(this.BNCL_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public BNCL(MKDS_Course_Modifier.Misc.BNCL Bncl)
    {
      this.Bncl = Bncl;
      this.InitializeComponent();
      this.toolStripButton1.Text = LanguageHandler.GetString("base.save");
      this.simpleOpenGlControl1.InitializeContexts();
    }

    public void SetNCLR(MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR Palette)
    {
      this.Palette = Palette;
      if (Palette == null || this.Graphic == null || this.Cell == null)
        return;
      this.UploadCells();
    }

    public void SetNCGR(NCGR Graphic)
    {
      this.Graphic = Graphic;
      if (this.Palette == null || Graphic == null || this.Cell == null)
        return;
      this.UploadCells();
    }

    public void SetNCER(MKDS_Course_Modifier.G2D_Binary_File_Format.NCER Cell)
    {
      this.Cell = Cell;
      if (this.Palette == null || this.Graphic == null || Cell == null)
        return;
      this.UploadCells();
    }

    private void UploadCells()
    {
      for (int Index = 0; Index < (int) this.Cell.CellBankBlock.CellDataBank.numCells; ++Index)
        GlNitro2.glNitroTexImage2D(this.Cell.CellBankBlock.CellDataBank.GetBitmap(Index, this.Graphic, this.Palette), Index + 1, 9728);
      this.Render(false, new Point());
    }

    private void BNCL_Load(object sender, EventArgs e)
    {
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glDepthFunc(519);
      Gl.glEnable(3057);
      Gl.glDisable(2884);
      Gl.glEnable(3553);
      Gl.glEnable(2848);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
    }

    private void Render(bool pick = false, Point mousepoint = default (Point))
    {
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Gl.glOrtho(0.0, (double) this.simpleOpenGlControl1.Width, (double) this.simpleOpenGlControl1.Height, 0.0, -8192.0, 8192.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glClearColor(1f, 1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glEnable(3553);
      Gl.glBindTexture(3553, 0);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glDisable(2884);
      Gl.glEnable(3008);
      Gl.glEnable(3042);
      Gl.glEnable(2832);
      Gl.glBlendFunc(770, 771);
      Gl.glAlphaFunc(519, 0.0f);
      if (pick)
      {
        Gl.glLoadIdentity();
        Gl.glDisable(2881);
        Gl.glDisable(2832);
        this.RenderBNCL(true);
        this.pic = new byte[4];
        Gl.glReadPixels(mousepoint.X, this.simpleOpenGlControl1.Height - mousepoint.Y, 1, 1, 32993, 5121, (object) this.pic);
        Gl.glClear(16640);
        this.Render(false, new Point());
      }
      else
      {
        Gl.glLoadIdentity();
        this.RenderBNCL(false);
        this.simpleOpenGlControl1.Refresh();
      }
    }

    private void RenderBNCL(bool pick = false)
    {
      if (this.Palette == null || this.Graphic == null || this.Cell == null)
        return;
      foreach (MKDS_Course_Modifier.Misc.BNCL.BNCLEntry entry in this.Bncl.Entries)
      {
        if (this.Cell.CellBankBlock.CellDataBank.CellData[entry.CellNr].boundingRect != null)
        {
          int num1 = (int) this.Cell.CellBankBlock.CellDataBank.CellData[entry.CellNr].boundingRect.maxX - (int) this.Cell.CellBankBlock.CellDataBank.CellData[entry.CellNr].boundingRect.minX;
          int num2 = (int) this.Cell.CellBankBlock.CellDataBank.CellData[entry.CellNr].boundingRect.maxY - (int) this.Cell.CellBankBlock.CellDataBank.CellData[entry.CellNr].boundingRect.minY;
          Gl.glBindTexture(3553, entry.CellNr + 1);
          Gl.glPushMatrix();
          Gl.glTranslatef((float) entry.X, (float) entry.Y, 0.0f);
          if (((int) entry.Unknown1 >> 4 & 1) == 1)
            Gl.glTranslatef((float) -num1 / 2f, 0.0f, 0.0f);
          if (((int) entry.Unknown2 >> 4 & 1) == 1)
            Gl.glTranslatef(0.0f, (float) -num2 / 2f, 0.0f);
          Gl.glBegin(7);
          Gl.glTexCoord2f(0.0f, 0.0f);
          Gl.glVertex2f(0.0f, 0.0f);
          Gl.glTexCoord2f(1f, 0.0f);
          Gl.glVertex2f((float) num1, 0.0f);
          Gl.glTexCoord2f(1f, 1f);
          Gl.glVertex2f((float) num1, (float) num2);
          Gl.glTexCoord2f(0.0f, 1f);
          Gl.glVertex2f(0.0f, (float) num2);
          Gl.glEnd();
          Gl.glPopMatrix();
        }
      }
    }

    private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
    {
      this.Render(false, new Point());
    }
  }
}
