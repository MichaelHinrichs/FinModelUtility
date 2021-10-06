// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MPDS.MPDSEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.MPDS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI.MPDS
{
  public class MPDSEditor : Form
  {
    public static float X = 0.0f;
    public static float Y = 0.0f;
    public static float ang = 0.0f;
    public static float dist = 0.0f;
    public static float elev = 0.0f;
    private IContainer components = (IContainer) null;
    public HBDF[] Objects = new HBDF[3];
    private bool wire = false;
    private bool licht = true;
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private PropertyGrid propertyGrid1;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private TabPage tabPage2;
    private PictureBox pictureBox1;
    public BMAP BoardData;
    public BMAP SystemData;
    public HBDF Model;
    public HBDF.TEXSBlock Spaces;
    public BoardLayout Layout;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.toolStrip1 = new ToolStrip();
      this.splitContainer1 = new SplitContainer();
      this.propertyGrid1 = new PropertyGrid();
      this.tabControl1 = new TabControl();
      this.tabPage1 = new TabPage();
      this.tabPage2 = new TabPage();
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.pictureBox1 = new PictureBox();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.tabPage2.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(726, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.tabControl1);
      this.splitContainer1.Size = new Size(726, 334);
      this.splitContainer1.SplitterDistance = 154;
      this.splitContainer1.TabIndex = 2;
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(154, 334);
      this.propertyGrid1.TabIndex = 0;
      this.tabControl1.Controls.Add((Control) this.tabPage1);
      this.tabControl1.Controls.Add((Control) this.tabPage2);
      this.tabControl1.Dock = DockStyle.Fill;
      this.tabControl1.Location = new Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new Size(568, 334);
      this.tabControl1.TabIndex = 0;
      this.tabPage1.Controls.Add((Control) this.simpleOpenGlControl1);
      this.tabPage1.Location = new Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Size = new Size(560, 308);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Board Layout";
      this.tabPage1.UseVisualStyleBackColor = true;
      this.tabPage2.Controls.Add((Control) this.pictureBox1);
      this.tabPage2.Location = new Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new Padding(3);
      this.tabPage2.Size = new Size(560, 308);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "Board Information";
      this.tabPage2.UseVisualStyleBackColor = true;
      this.simpleOpenGlControl1.AccumBits = (byte) 0;
      this.simpleOpenGlControl1.AutoCheckErrors = false;
      this.simpleOpenGlControl1.AutoFinish = false;
      this.simpleOpenGlControl1.AutoMakeCurrent = true;
      this.simpleOpenGlControl1.AutoSwapBuffers = true;
      this.simpleOpenGlControl1.BackColor = Color.Black;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 24;
      this.simpleOpenGlControl1.Dock = DockStyle.Fill;
      this.simpleOpenGlControl1.Location = new Point(0, 0);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(560, 308);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.Resize += new EventHandler(this.MPDSEditor_Resize);
      this.pictureBox1.Location = new Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(256, 256);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(726, 359);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (MPDSEditor);
      this.Text = nameof (MPDSEditor);
      this.Load += new EventHandler(this.MPDSEditor_Load);
      this.Resize += new EventHandler(this.MPDSEditor_Resize);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage2.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public MPDSEditor(BMAP BoardData, BMAP SystemData)
    {
      this.BoardData = BoardData;
      this.SystemData = SystemData;
      this.InitializeComponent();
      this.simpleOpenGlControl1.InitializeContexts();
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
    }

    private void MPDSEditor_Load(object sender, EventArgs e)
    {
      byte[] numArray1 = Compression.LZ77Decompress(this.BoardData.Files[1].Data);
      byte[] numArray2 = Compression.LZ77Decompress(this.BoardData.Files[2].Data);
      byte[] numArray3 = Compression.LZ77Decompress(this.BoardData.Files[0].Data);
      byte[] array1 = ((IEnumerable<byte>) numArray1).ToList<byte>().GetRange(4, numArray1.Length - 4).ToArray();
      byte[] array2 = ((IEnumerable<byte>) numArray2).ToList<byte>().GetRange(4, numArray2.Length - 4).ToArray();
      byte[] array3 = ((IEnumerable<byte>) numArray3).ToList<byte>().GetRange(4, numArray3.Length - 4).ToArray();
      this.pictureBox1.Image = (Image) Graphic.ConvertData(array2, 0, 0, array1, array3, 256, 256, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR);
      this.Layout = new BoardLayout(Compression.LZ77Decompress(this.BoardData.Files[3].Data));
      int index1 = -1;
      int num = 0;
      for (int index2 = 4; (long) index2 < (long) this.BoardData.NrFiles; ++index2)
      {
        int length = this.BoardData.Files[index2].Data.Length;
        if (length > num)
        {
          num = length;
          index1 = index2;
        }
      }
      this.Model = new HBDF(Compression.LZ77Decompress(this.BoardData.Files[index1].Data));
      this.Spaces = new HBDF.TEXSBlock(Compression.LZ77Decompress(this.SystemData.Files[69].Data));
      this.Objects[0] = new HBDF(Compression.LZ77Decompress(this.SystemData.Files[32].Data));
      this.Objects[1] = new HBDF(Compression.LZ77Decompress(this.SystemData.Files[31].Data));
      this.Objects[2] = new HBDF(Compression.LZ77Decompress(this.SystemData.Files[33].Data));
      Gl.ReloadFunctions();
      Gl.glEnable(2896);
      Gl.glEnable(32826);
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glEnable(2977);
      Gl.glDisable(2884);
      Gl.glFrontFace(2305);
      Gl.glEnable(3553);
      Gl.glClearDepth(1.0);
      Gl.glEnable(3008);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glShadeModel(7425);
      Gl.glAlphaFunc(516, 0.0f);
      Gl.glClearColor(0.2f, 0.2f, 0.2f, 1f);
      GlNitro2.glNitroBindTextures(this.Model, 1);
      GlNitro2.glNitroBindTextures(this.Spaces, (int) this.Model.MDLFBlocks[0].NrTextures + 1);
      GlNitro2.glNitroBindTextures(this.Objects[0], (int) this.Model.MDLFBlocks[0].NrTextures + 1 + this.Spaces.TEXOBlocks.Length);
      GlNitro2.glNitroBindTextures(this.Objects[1], (int) this.Model.MDLFBlocks[0].NrTextures + 1 + this.Spaces.TEXOBlocks.Length + (int) this.Objects[0].MDLFBlocks[0].NrTextures);
      GlNitro2.glNitroBindTextures(this.Objects[2], (int) this.Model.MDLFBlocks[0].NrTextures + 1 + this.Spaces.TEXOBlocks.Length + (int) this.Objects[0].MDLFBlocks[0].NrTextures + (int) this.Objects[1].MDLFBlocks[0].NrTextures);
      this.Render();
    }

    public void Render()
    {
      float num = (float) this.simpleOpenGlControl1.Width / (float) this.simpleOpenGlControl1.Height;
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Glu.gluPerspective(30.0, (double) num, 0.100000001490116, 204800.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glBindTexture(3553, 0);
      Gl.glColor3f(1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glRotatef(MPDSEditor.elev, 1f, 0.0f, 0.0f);
      Gl.glLightfv(16384, 4611, new float[4]
      {
        0.0f,
        1f,
        -1f,
        0.0f
      });
      Gl.glLightfv(16385, 4611, new float[4]
      {
        0.998047f,
        1f,
        0.0f,
        0.0f
      });
      Gl.glLightfv(16386, 4611, new float[4]
      {
        0.0f,
        1f,
        0.998047f,
        0.0f
      });
      Gl.glLightfv(16387, 4611, new float[4]
      {
        -1f,
        1f,
        0.0f,
        0.0f
      });
      Gl.glLightfv(16384, 4609, new float[4]
      {
        1f,
        1f,
        1f,
        1f
      });
      Gl.glLightfv(16385, 4609, new float[4]
      {
        1f,
        1f,
        1f,
        1f
      });
      Gl.glLightfv(16386, 4609, new float[4]
      {
        1f,
        1f,
        1f,
        1f
      });
      Gl.glLightfv(16387, 4609, new float[4]
      {
        1f,
        1f,
        1f,
        1f
      });
      Gl.glRotatef(-MPDSEditor.elev, 1f, 0.0f, 0.0f);
      Gl.glTranslatef(MPDSEditor.X, MPDSEditor.Y, -MPDSEditor.dist);
      Gl.glRotatef(MPDSEditor.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(MPDSEditor.ang, 0.0f, 1f, 0.0f);
      Gl.glPushMatrix();
      this.Model.MDLFBlocks[0].Render(this.Model.TEXSBlocks[0], 1);
      Gl.glPopMatrix();
      Gl.glDisable(2896);
      Gl.glColor3f(1f, 1f, 1f);
      for (int index = 0; (long) index < (long) this.Layout.NrObjects; ++index)
      {
        int texture;
        switch (this.Layout.Entries[index].ObjectID)
        {
          case 17:
            texture = (int) this.Model.MDLFBlocks[0].NrTextures + 1;
            break;
          case 18:
            texture = (int) this.Model.MDLFBlocks[0].NrTextures + 2;
            break;
          case 19:
            texture = (int) this.Model.MDLFBlocks[0].NrTextures + 5;
            break;
          case 20:
            texture = (int) this.Model.MDLFBlocks[0].NrTextures + 3;
            break;
          case 21:
            Gl.glPushMatrix();
            Gl.glTranslatef((float) this.Layout.Entries[index].X, (float) this.Layout.Entries[index].Y, (float) this.Layout.Entries[index].Z);
            this.Objects[1].MDLFBlocks[0].Render(this.Objects[1].TEXSBlocks[0], (int) this.Model.MDLFBlocks[0].NrTextures + 1 + this.Spaces.TEXOBlocks.Length + (int) this.Objects[0].MDLFBlocks[0].NrTextures);
            Gl.glPopMatrix();
            Gl.glColor3f(1f, 1f, 1f);
            continue;
          case 22:
            texture = (int) this.Model.MDLFBlocks[0].NrTextures + 4;
            break;
          case 23:
            texture = (int) this.Model.MDLFBlocks[0].NrTextures + 7;
            break;
          case 25:
            Gl.glPushMatrix();
            Gl.glTranslatef((float) this.Layout.Entries[index].X, (float) this.Layout.Entries[index].Y, (float) this.Layout.Entries[index].Z);
            this.Objects[0].MDLFBlocks[0].Render(this.Objects[0].TEXSBlocks[0], (int) this.Model.MDLFBlocks[0].NrTextures + 1 + this.Spaces.TEXOBlocks.Length);
            Gl.glPopMatrix();
            Gl.glColor3f(1f, 1f, 1f);
            continue;
          case 32:
            Gl.glPushMatrix();
            Gl.glTranslatef((float) this.Layout.Entries[index].X, (float) this.Layout.Entries[index].Y, (float) this.Layout.Entries[index].Z);
            Gl.glTranslatef(0.0f, 20.5f, 24f);
            this.Objects[2].MDLFBlocks[0].Render((HBDF.TEXSBlock) null, (int) this.Model.MDLFBlocks[0].NrTextures + 1 + this.Spaces.TEXOBlocks.Length + (int) this.Objects[0].MDLFBlocks[0].NrTextures + (int) this.Objects[1].MDLFBlocks[0].NrTextures);
            Gl.glPopMatrix();
            Gl.glColor3f(1f, 1f, 1f);
            continue;
          default:
            texture = 0;
            break;
        }
        Gl.glPushMatrix();
        Gl.glTranslatef((float) this.Layout.Entries[index].X, (float) this.Layout.Entries[index].Y, (float) this.Layout.Entries[index].Z);
        Gl.glMatrixMode(5890);
        Gl.glBindTexture(3553, texture);
        Gl.glLoadIdentity();
        Gl.glMatrixMode(5888);
        Gl.glBegin(7);
        Gl.glTexCoord2f(0.0f, 1f);
        Gl.glVertex3f(-8f, 0.0f, 8f);
        Gl.glTexCoord2f(1f, 1f);
        Gl.glVertex3f(8f, 0.0f, 8f);
        Gl.glTexCoord2f(1f, 0.0f);
        Gl.glVertex3f(8f, 0.0f, -8f);
        Gl.glTexCoord2f(0.0f, 0.0f);
        Gl.glVertex3f(-8f, 0.0f, -8f);
        Gl.glEnd();
        Gl.glPopMatrix();
      }
      this.simpleOpenGlControl1.Refresh();
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
        MPDSEditor.dist += (float) e.Delta / 1f;
      else
        MPDSEditor.dist += (float) e.Delta / 10f;
      this.Render();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData & ~Keys.Shift)
      {
        case Keys.Escape:
          MPDSEditor.X = 0.0f;
          MPDSEditor.Y = 0.0f;
          MPDSEditor.ang = 0.0f;
          MPDSEditor.dist = 0.0f;
          MPDSEditor.elev = 0.0f;
          this.Render();
          return true;
        case Keys.Left:
          --MPDSEditor.ang;
          this.Render();
          return true;
        case Keys.Up:
          ++MPDSEditor.elev;
          this.Render();
          return true;
        case Keys.Right:
          ++MPDSEditor.ang;
          this.Render();
          return true;
        case Keys.Down:
          --MPDSEditor.elev;
          this.Render();
          return true;
        case Keys.A:
          MPDSEditor.Y -= (float) (5.0 * ((Control.ModifierKeys & Keys.Shift) == Keys.Shift ? 10.0 : 1.0));
          this.Render();
          return true;
        case Keys.L:
          this.licht = !this.licht;
          this.Render();
          return true;
        case Keys.S:
          MPDSEditor.Y += (float) (5.0 * ((Control.ModifierKeys & Keys.Shift) == Keys.Shift ? 10.0 : 1.0));
          this.Render();
          return true;
        case Keys.T:
          MPDSEditor.elev = 90f;
          MPDSEditor.ang = 0.0f;
          this.Render();
          return true;
        case Keys.W:
          this.wire = !this.wire;
          if (this.wire)
          {
            Gl.glPolygonMode(1032, 6913);
            this.Render();
          }
          else
          {
            Gl.glPolygonMode(1032, 6914);
            this.Render();
          }
          return true;
        case Keys.X:
          MPDSEditor.X += (float) (5.0 * ((Control.ModifierKeys & Keys.Shift) == Keys.Shift ? 10.0 : 1.0));
          this.Render();
          return true;
        case Keys.Z:
          MPDSEditor.X -= (float) (5.0 * ((Control.ModifierKeys & Keys.Shift) == Keys.Shift ? 10.0 : 1.0));
          this.Render();
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }

    private void MPDSEditor_Resize(object sender, EventArgs e)
    {
      this.Render();
    }
  }
}
