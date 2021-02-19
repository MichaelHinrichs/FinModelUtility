// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.MKDSEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Archive_Format;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using MKDS_Course_Modifier.MKDS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class MKDSEditor : Form
  {
    public static float X = 0.0f;
    public static float Y = 0.0f;
    public static float ang = 0.0f;
    public static float dist = 0.0f;
    public static float elev = 0.0f;
    private IContainer components = (IContainer) null;
    private List<ushort> objectstex = new List<ushort>();
    private bool first = true;
    private byte[] pic = new byte[16];
    private object selected = (object) null;
    private bool texoffset2 = true;
    private bool wire = false;
    private bool licht = true;
    private Point Last = new Point(-1, -1);
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private PropertyGrid propertyGrid1;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private TabPage tabPage2;
    private ToolStripSplitButton toolStripSplitButton1;
    private ToolStripMenuItem courseMapToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem1;
    private NARC.DirectoryEntry Root1;
    private NARC.DirectoryEntry Root2;
    private NARC.DirectoryEntry RomRoot;
    private NARC.DirectoryEntry Main;
    private NARC.DirectoryEntry MainRace;
    private MKDS_Course_Modifier.MKDS.NKM nkm;
    private MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd;
    private MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd2;
    private bool picking;
    private int texoffset;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MKDSEditor));
      this.toolStrip1 = new ToolStrip();
      this.toolStripSplitButton1 = new ToolStripSplitButton();
      this.courseMapToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
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
        (ToolStripItem) this.toolStripSplitButton1
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(706, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.courseMapToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem1
      });
      this.toolStripSplitButton1.Image = (Image) componentResourceManager.GetObject("toolStripSplitButton1.Image");
      this.toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripSplitButton1.Name = "toolStripSplitButton1";
      this.toolStripSplitButton1.Size = new Size(50, 22);
      this.toolStripSplitButton1.Text = "NKM";
      this.courseMapToolStripMenuItem.Checked = true;
      this.courseMapToolStripMenuItem.CheckState = CheckState.Checked;
      this.courseMapToolStripMenuItem.Name = "courseMapToolStripMenuItem";
      this.courseMapToolStripMenuItem.Size = new Size(165, 22);
      this.courseMapToolStripMenuItem.Text = "course_map.nkm";
      this.courseMapToolStripMenuItem.Click += new EventHandler(this.NKMClick);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new Size(162, 6);
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.tabControl1);
      this.splitContainer1.Size = new Size(706, 407);
      this.splitContainer1.SplitterDistance = 184;
      this.splitContainer1.TabIndex = 1;
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(184, 407);
      this.propertyGrid1.TabIndex = 0;
      this.tabControl1.Controls.Add((Control) this.tabPage1);
      this.tabControl1.Controls.Add((Control) this.tabPage2);
      this.tabControl1.Dock = DockStyle.Fill;
      this.tabControl1.Location = new Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new Size(518, 407);
      this.tabControl1.TabIndex = 0;
      this.tabPage1.Controls.Add((Control) this.simpleOpenGlControl1);
      this.tabPage1.Location = new Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new Padding(3);
      this.tabPage1.Size = new Size(510, 381);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "tabPage1";
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
      this.simpleOpenGlControl1.Location = new Point(3, 3);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(504, 375);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.KeyUp += new KeyEventHandler(this.simpleOpenGlControl1_KeyUp);
      this.simpleOpenGlControl1.MouseMove += new MouseEventHandler(this.simpleOpenGlControl1_MouseMove);
      this.simpleOpenGlControl1.MouseUp += new MouseEventHandler(this.simpleOpenGlControl1_MouseUp);
      this.simpleOpenGlControl1.Resize += new EventHandler(this.simpleOpenGlControl1_Resize);
      this.tabPage2.Location = new Point(4, 22);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Padding = new Padding(3);
      this.tabPage2.Size = new Size(510, 381);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "tabPage2";
      this.tabPage2.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(706, 432);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (MKDSEditor);
      this.Text = "NKM";
      this.FormClosing += new FormClosingEventHandler(this.MKDSEditor_FormClosing);
      this.Load += new EventHandler(this.NKM_Load);
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

    public MKDSEditor(
      NARC.DirectoryEntry Root1,
      NARC.DirectoryEntry Root2,
      NARC.DirectoryEntry RomRoot)
    {
      this.Root1 = Root1;
      this.Root2 = Root2;
      this.RomRoot = RomRoot;
      this.nkm = new MKDS_Course_Modifier.MKDS.NKM(Root1.GetFileByPath("\\course_map.nkm").Content);
      this.Main = NARC.Unpack(Compression.LZ77Decompress(RomRoot.GetFileByPath("\\data\\Main\\MapObj.carc").Content));
      this.MainRace = NARC.Unpack(Compression.LZ77Decompress(RomRoot.GetFileByPath("\\data\\MainRace.carc").Content));
      this.nsbmd = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(Root1.GetFileByPath("\\course_model.nsbmd").Content);
      if (this.nsbmd.TexPlttSet == null)
      {
        try
        {
          this.nsbmd.TexPlttSet = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX(Root2.GetFileByPath("\\course_model.nsbtx").Content).TexPlttSet;
        }
        catch
        {
        }
      }
      if (Root1.GetFileByPath("\\course_model_V.nsbmd") != null)
      {
        this.nsbmd2 = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(Root1.GetFileByPath("\\course_model_V.nsbmd").Content);
        if (this.nsbmd2.TexPlttSet == null)
          this.nsbmd2.TexPlttSet = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX(Root2.GetFileByPath("\\course_model_V.nsbtx").Content).TexPlttSet;
      }
      this.InitializeComponent();
      if (Root1.GetDirectoryByPath("\\MissionRun") != null)
      {
        foreach (NARC.FileEntry file in Root1.GetDirectoryByPath("\\MissionRun").Files)
          this.toolStripSplitButton1.DropDownItems.Add(file.Name, (Image) null, new EventHandler(this.NKMClick));
      }
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
    }

    private void NKMClick(object sender, EventArgs e)
    {
      ToolStripItem toolStripItem = sender as ToolStripItem;
      this.nkm = !toolStripItem.Text.Contains("tool") ? new MKDS_Course_Modifier.MKDS.NKM(this.Root1.GetFileByPath("\\course_map.nkm").Content) : new MKDS_Course_Modifier.MKDS.NKM(this.Root1.GetFileByPath("\\MissionRun\\" + toolStripItem.Text).Content);
      this.first = true;
      this.Render(false, 0, 0);
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
        MKDSEditor.dist += (float) e.Delta / 100f;
      else
        MKDSEditor.dist += (float) e.Delta / 1000f;
      this.Render(false, 0, 0);
    }

    private void NKM_Load(object sender, EventArgs e)
    {
      this.simpleOpenGlControl1.InitializeContexts();
      Gl.ReloadFunctions();
      Gl.glEnable(32826);
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glEnable(2977);
      Gl.glDisable(2884);
      Gl.glFrontFace(2305);
      Gl.glEnable(3553);
      Gl.glClearDepth(1.0);
      Gl.glEnable(3008);
      Gl.glAlphaFunc(516, 0.0f);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      Gl.glShadeModel(7425);
      Gl.glClearColor(0.2f, 0.2f, 0.2f, 0.0f);
      GlNitro.glNitroBindTextures(this.nsbmd, 1);
      this.Render(false, 0, 0);
    }

    public void Render(bool pick = false, int X1 = 0, int Y1 = 0)
    {
      float num1 = (float) this.simpleOpenGlControl1.Width / (float) this.simpleOpenGlControl1.Height;
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Glu.gluPerspective(30.0, (double) num1, 0.100000001490116, 32768.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glBindTexture(3553, 0);
      Gl.glColor3f(1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glTranslatef(MKDSEditor.X, MKDSEditor.Y, -MKDSEditor.dist);
      Gl.glRotatef(MKDSEditor.elev, 1f, 0.0f, 0.0f);
      Gl.glRotatef(MKDSEditor.ang, 0.0f, 1f, 0.0f);
      Gl.glPushMatrix();
      Gl.glPushMatrix();
      if (this.Root1.GetFileByPath("\\course_model_V.nsbmd") != null)
      {
        if (this.first)
          GlNitro.glNitroBindTextures(this.nsbmd2, 16711426);
        Gl.glScalef(this.nsbmd2.modelSet.models[0].info.posScale / this.nsbmd.modelSet.models[0].info.posScale, this.nsbmd2.modelSet.models[0].info.posScale / this.nsbmd.modelSet.models[0].info.posScale, this.nsbmd2.modelSet.models[0].info.posScale / this.nsbmd.modelSet.models[0].info.posScale);
        Gl.glDisable(2884);
        this.nsbmd2.modelSet.models[0].ProcessSbc(MKDSEditor.X, MKDSEditor.Y, MKDSEditor.dist, MKDSEditor.elev, MKDSEditor.ang, this.picking, 16711426);
      }
      Gl.glPopMatrix();
      this.nsbmd.modelSet.models[0].ProcessSbc(MKDSEditor.X, MKDSEditor.Y, MKDSEditor.dist, MKDSEditor.elev, MKDSEditor.ang, false, 1);
      Gl.glPopMatrix();
      Gl.glPushMatrix();
      Gl.glScalef(0.062f * this.nsbmd.modelSet.models[0].info.invPosScale, 0.062f * this.nsbmd.modelSet.models[0].info.invPosScale, 0.062f * this.nsbmd.modelSet.models[0].info.invPosScale);
      Gl.glPushMatrix();
      this.texoffset = (int) this.nsbmd.modelSet.models[0].materials.dict.numEntry + 1;
      this.picking = false;
      int num2 = -1;
      for (int index = 0; (long) index < (long) this.nkm.OBJI.NrEntries; ++index)
      {
        Gl.glBindTexture(3553, 0);
        Gl.glColor4f(1f, 1f, 1f, 1f);
        Gl.glPushMatrix();
        Gl.glTranslatef(this.nkm.OBJI[index].Position.X, this.nkm.OBJI[index].Position.Y + 16f, this.nkm.OBJI[index].Position.Z);
        Gl.glRotatef(this.nkm.OBJI[index].Rotation.X, 1f, 0.0f, 0.0f);
        Gl.glRotatef(this.nkm.OBJI[index].Rotation.Y, 0.0f, 1f, 0.0f);
        Gl.glRotatef(this.nkm.OBJI[index].Rotation.Z, 0.0f, 0.0f, 1f);
        Gl.glScalef(this.nkm.OBJI[index].Scale.X, this.nkm.OBJI[index].Scale.Y, this.nkm.OBJI[index].Scale.Z);
        if (this.selected == this.nkm.OBJI[index])
          num2 = index;
        this.RenderObject(this.nkm.OBJI[index]);
        Gl.glPopMatrix();
      }
      Gl.glPopMatrix();
      if (num2 != -1)
      {
        int index = num2;
        Gl.glBindTexture(3553, 0);
        Gl.glColor4f(1f, 1f, 1f, 1f);
        Gl.glPushMatrix();
        Gl.glTranslatef(this.nkm.OBJI[index].Position.X, this.nkm.OBJI[index].Position.Y, this.nkm.OBJI[index].Position.Z);
        Gl.glRotatef(this.nkm.OBJI[index].Rotation.X, 1f, 0.0f, 0.0f);
        Gl.glRotatef(this.nkm.OBJI[index].Rotation.Y, 0.0f, 1f, 0.0f);
        Gl.glRotatef(this.nkm.OBJI[index].Rotation.Z, 0.0f, 0.0f, 1f);
        Gl.glScalef(this.nkm.OBJI[index].Scale.X, this.nkm.OBJI[index].Scale.Y, this.nkm.OBJI[index].Scale.Z);
        Gl.glColorMask(0, 0, 0, 0);
        Gl.glEnable(2960);
        Gl.glStencilMask(3);
        Gl.glStencilFunc(519, 1, 3);
        Gl.glStencilOp(7681, 7681, 7681);
        this.picking = true;
        Gl.glDisable(2884);
        this.RenderObject(this.nkm.OBJI[index]);
        this.picking = false;
        Gl.glColorMask(1, 1, 1, 1);
        Gl.glPolygonMode(1032, 6914);
        Gl.glEnable(32823);
        Gl.glStencilFunc(514, 1, 3);
        Gl.glStencilOp(7680, 7680, 7682);
        Gl.glPolygonOffset(-1f, -1f);
        Gl.glColor4f(1f, 1f, 0.5f, 0.1960784f);
        this.picking = true;
        Gl.glDisable(2884);
        this.RenderObject(this.nkm.OBJI[index]);
        Gl.glDisable(32823);
        Gl.glPolygonMode(1032, 6913);
        Gl.glLineWidth(2f);
        Gl.glStencilFunc(514, 0, 3);
        Gl.glStencilOp(7680, 7680, 7681);
        Gl.glDepthFunc(519);
        Gl.glColor4f(1f, 1f, 0.5f, 1f);
        Gl.glDisable(2884);
        this.RenderObject(this.nkm.OBJI[index]);
        this.picking = false;
        Gl.glPolygonMode(1032, 6914);
        Gl.glDepthFunc(515);
        Gl.glColor4f(1f, 1f, 1f, 1f);
        this.picking = false;
        Gl.glPopMatrix();
      }
      Gl.glPopMatrix();
      this.simpleOpenGlControl1.Refresh();
    }

    private void CreateCube(Color Fill, Color Border, bool axes)
    {
      float num = (float) (0.00999999977648258 / (0.061999998986721 * (double) this.nsbmd.modelSet.models[0].info.invPosScale));
      if (!this.picking)
      {
        Gl.glBindTexture(3553, 0);
        Gl.glColor4f((float) Fill.R / (float) byte.MaxValue, (float) Fill.G / (float) byte.MaxValue, (float) Fill.B / (float) byte.MaxValue, (float) Fill.A / (float) byte.MaxValue);
        Gl.glDisable(2896);
      }
      Gl.glBegin(5);
      Gl.glVertex3f(-num, -num, -num);
      Gl.glVertex3f(-num, num, -num);
      Gl.glVertex3f(num, -num, -num);
      Gl.glVertex3f(num, num, -num);
      Gl.glVertex3f(num, -num, num);
      Gl.glVertex3f(num, num, num);
      Gl.glVertex3f(-num, -num, num);
      Gl.glVertex3f(-num, num, num);
      Gl.glVertex3f(-num, -num, -num);
      Gl.glVertex3f(-num, num, -num);
      Gl.glEnd();
      Gl.glBegin(5);
      Gl.glVertex3f(-num, num, -num);
      Gl.glVertex3f(-num, num, num);
      Gl.glVertex3f(num, num, -num);
      Gl.glVertex3f(num, num, num);
      Gl.glEnd();
      Gl.glBegin(5);
      Gl.glVertex3f(-num, -num, -num);
      Gl.glVertex3f(num, -num, -num);
      Gl.glVertex3f(-num, -num, num);
      Gl.glVertex3f(num, -num, num);
      Gl.glEnd();
      if (this.picking)
        return;
      Gl.glLineWidth(1.5f);
      Gl.glColor4f((float) Border.R / (float) byte.MaxValue, (float) Border.G / (float) byte.MaxValue, (float) Border.B / (float) byte.MaxValue, (float) Border.A / (float) byte.MaxValue);
      Gl.glBegin(3);
      Gl.glVertex3f(num, num, num);
      Gl.glVertex3f(-num, num, num);
      Gl.glVertex3f(-num, num, -num);
      Gl.glVertex3f(num, num, -num);
      Gl.glVertex3f(num, num, num);
      Gl.glVertex3f(num, -num, num);
      Gl.glVertex3f(-num, -num, num);
      Gl.glVertex3f(-num, -num, -num);
      Gl.glVertex3f(num, -num, -num);
      Gl.glVertex3f(num, -num, num);
      Gl.glEnd();
      Gl.glBegin(1);
      Gl.glVertex3f(-num, num, num);
      Gl.glVertex3f(-num, -num, num);
      Gl.glVertex3f(-num, num, -num);
      Gl.glVertex3f(-num, -num, -num);
      Gl.glVertex3f(num, num, -num);
      Gl.glVertex3f(num, -num, -num);
      Gl.glEnd();
      if (axes)
      {
        Gl.glBegin(1);
        Gl.glColor3f(1f, 0.0f, 0.0f);
        Gl.glVertex3f(0.0f, 0.0f, 0.0f);
        Gl.glColor3f(1f, 0.0f, 0.0f);
        Gl.glVertex3f(num * 2f, 0.0f, 0.0f);
        Gl.glColor3f(0.0f, 1f, 0.0f);
        Gl.glVertex3f(0.0f, 0.0f, 0.0f);
        Gl.glColor3f(0.0f, 1f, 0.0f);
        Gl.glVertex3f(0.0f, num * 2f, 0.0f);
        Gl.glColor3f(0.0f, 0.0f, 1f);
        Gl.glVertex3f(0.0f, 0.0f, 0.0f);
        Gl.glColor3f(0.0f, 0.0f, 1f);
        Gl.glVertex3f(0.0f, 0.0f, num * 2f);
        Gl.glEnd();
      }
    }

    public bool RenderObject(MKDS_Course_Modifier.MKDS.NKM.OBJIEntry Item)
    {
      this.texoffset = 0;
      ushort objectId = Item.ObjectID;
      ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(objectId);
      if (@object != null)
      {
        List<string> stringList = new List<string>();
        foreach (ObjectDb.Object.File requiredFile in @object.RequiredFiles)
        {
          if (requiredFile.FileName.EndsWith(".nsbmd"))
            stringList.Add(requiredFile.FileName);
        }
        foreach (string str in stringList)
        {
          if (!str.StartsWith("sh_") && !str.EndsWith("_shadow.nsbmd"))
          {
            NARC.FileEntry fileByPath1 = this.Root1.GetFileByPath("\\MapObj\\" + str);
            if (fileByPath1 != null)
            {
              this.RenderObject(objectId, new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(fileByPath1.Content), (NSBTP) null, 0, stringList.IndexOf(str) == stringList.Count - 1);
            }
            else
            {
              NARC.FileEntry fileByPath2 = this.Main.GetFileByPath("\\" + str);
              if (fileByPath2 != null)
              {
                this.RenderObject(objectId, new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(fileByPath2.Content), (NSBTP) null, 0, stringList.IndexOf(str) == stringList.Count - 1);
              }
              else
              {
                NARC.FileEntry fileByPath3 = this.RomRoot.GetFileByPath("\\data\\MissionRun\\" + str);
                if (fileByPath3 != null)
                {
                  this.RenderObject(objectId, new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(fileByPath3.Content), (NSBTP) null, 0, stringList.IndexOf(str) == stringList.Count - 1);
                }
                else
                {
                  this.CreateCube(Color.Red, Color.SkyBlue, true);
                  return false;
                }
              }
            }
          }
        }
        return true;
      }
      this.CreateCube(Color.Blue, Color.SkyBlue, true);
      return false;
    }

    public void RenderObject(ushort id, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD tmp, NSBTP btp = null, int btpidx = 0, bool last = true)
    {
      this.RenderObject(id, tmp, false, btp, btpidx, last);
    }

    public void RenderObject(
      ushort id,
      MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD tmp,
      bool translate,
      NSBTP btp = null,
      int btpidx = 0,
      bool last = true)
    {
      if (!this.objectstex.Contains(id))
      {
        GlNitro.glNitroBindTextures(tmp, (int) id * (int) byte.MaxValue + 1 + this.texoffset * (int) ushort.MaxValue);
        if (last)
          this.objectstex.Add(id);
      }
      Gl.glPushMatrix();
      Gl.glScalef(16.12903f * tmp.modelSet.models[0].info.posScale, 16.12903f * tmp.modelSet.models[0].info.posScale, 16.12903f * tmp.modelSet.models[0].info.posScale);
      if (translate)
        Gl.glTranslatef(0.0f, tmp.modelSet.models[0].info.boxH / 2f - tmp.modelSet.models[0].info.boxY, 0.0f);
      tmp.modelSet.models[0].ProcessSbc(tmp.TexPlttSet, (NSBCA) null, 0, 0, (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA) null, 0, 0, btp, 0, btpidx, (NSBMA) null, 0, 0, (NSBVA) null, 0, 0, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, this.picking, (int) id * (int) byte.MaxValue + 1 + this.texoffset * (int) ushort.MaxValue);
      ++this.texoffset;
      Gl.glPopMatrix();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Escape:
          MKDSEditor.X = 0.0f;
          MKDSEditor.Y = 0.0f;
          MKDSEditor.ang = 0.0f;
          MKDSEditor.dist = 0.0f;
          MKDSEditor.elev = 0.0f;
          this.Render(false, 0, 0);
          return true;
        case Keys.Left:
          --MKDSEditor.ang;
          this.Render(false, 0, 0);
          return true;
        case Keys.Up:
          ++MKDSEditor.elev;
          this.Render(false, 0, 0);
          return true;
        case Keys.Right:
          ++MKDSEditor.ang;
          this.Render(false, 0, 0);
          return true;
        case Keys.Down:
          --MKDSEditor.elev;
          this.Render(false, 0, 0);
          return true;
        case Keys.A:
          MKDSEditor.Y -= 0.05f;
          this.Render(false, 0, 0);
          return true;
        case Keys.L:
          this.licht = !this.licht;
          this.Render(false, 0, 0);
          return true;
        case Keys.S:
          MKDSEditor.Y += 0.05f;
          this.Render(false, 0, 0);
          return true;
        case Keys.T:
          MKDSEditor.elev = 90f;
          MKDSEditor.ang = 0.0f;
          this.Render(false, 0, 0);
          return true;
        case Keys.W:
          this.wire = !this.wire;
          if (this.wire)
          {
            Gl.glPolygonMode(1032, 6913);
            this.Render(false, 0, 0);
          }
          else
          {
            Gl.glPolygonMode(1032, 6914);
            this.Render(false, 0, 0);
          }
          return true;
        case Keys.X:
          MKDSEditor.X += 0.05f;
          this.Render(false, 0, 0);
          return true;
        case Keys.Z:
          MKDSEditor.X -= 0.05f;
          this.Render(false, 0, 0);
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }

    private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
    {
      this.Render(false, 0, 0);
    }

    private void MKDSEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.simpleOpenGlControl1.DestroyContexts();
    }

    private void simpleOpenGlControl1_KeyUp(object sender, KeyEventArgs e)
    {
    }

    private void simpleOpenGlControl1_MouseUp(object sender, MouseEventArgs e)
    {
      this.Render(true, e.X, e.Y);
      try
      {
        this.selected = (object) this.nkm.OBJI[Color.FromArgb(0, (int) this.pic[2], (int) this.pic[1], (int) this.pic[0]).ToArgb() - 1];
        this.Render(false, 0, 0);
      }
      catch
      {
      }
    }

    private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left && this.selected != null)
      {
        if (!(this.selected is MKDS_Course_Modifier.MKDS.NKM.OBJIEntry))
          return;
        double[] numArray1 = new double[16];
        double[] numArray2 = new double[16];
        int[] numArray3 = new int[4];
        Gl.glGetDoublev(2982, numArray1);
        Gl.glGetDoublev(2983, numArray2);
        Gl.glGetIntegerv(2978, numArray3);
        float num1 = -1f;
        Gl.glReadPixels(e.X, e.Y, 1, 1, 6402, 5126, (object) num1);
        double objX;
        double objY;
        Glu.gluUnProject((double) e.X, (double) e.Y, (double) num1, numArray1, numArray2, numArray3, out objX, out objY, out double _);
        double num2 = objX * ((double) this.nsbmd.modelSet.models[0].info.posScale * 10000.0);
        objY *= (double) this.nsbmd.modelSet.models[0].info.posScale * 10000.0;
        if (this.Last != new Point(-1, -1))
        {
          Point point = new Point((int) num2 - this.Last.X, (int) objY - this.Last.Y);
          ((MKDS_Course_Modifier.MKDS.NKM.OBJIEntry) this.selected).Position.X += (float) point.X;
          ((MKDS_Course_Modifier.MKDS.NKM.OBJIEntry) this.selected).Position.Y += (float) -point.Y;
          this.Render(false, 0, 0);
        }
        this.Last = new Point((int) num2, (int) objY);
      }
      else
        this.Last = new Point(-1, -1);
    }
  }
}
