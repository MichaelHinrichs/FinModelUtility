// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NSBTA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Properties;
using MKDS_Course_Modifier.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI
{
  public class NSBTA : Form
  {
    private MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData selected = (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData) null;
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA file;
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private SplitContainer splitContainer3;
    private PropertyGrid propertyGrid1;
    private TimeLine timeLine1;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private TreeView treeView1;

    public NSBTA(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA file)
    {
      this.file = file;
      this.InitializeComponent();
      this.simpleOpenGlControl1.InitializeContexts();
    }

    private void NSBTA_Load(object sender, EventArgs e)
    {
      for (int index1 = 0; index1 < (int) this.file.texSRTAnmSet.dict.numEntry; ++index1)
      {
        TreeNode treeNode = this.treeView1.Nodes.Add(this.file.texSRTAnmSet.dict[index1].Key);
        for (int index2 = 0; index2 < (int) this.file.texSRTAnmSet.texSRTAnm[index1].dict.numEntry; ++index2)
          treeNode.Nodes.Add(this.file.texSRTAnmSet.texSRTAnm[index1].dict[index2].Key);
      }
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glDepthFunc(519);
      Gl.glDisable(2884);
      Gl.glEnable(3553);
      Gl.glBlendFunc(770, 771);
      Gl.glEnable(3553);
      GlNitro2.glNitroTexImage2D(Resources.preview_background, 0, 10497);
      GlNitro2.glNitroTexImage2D(Resources.example, 1, 10497);
      this.Render((float[]) null);
    }

    public void Render(float[] Matrix = null)
    {
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      Gl.glOrtho(-((double) this.simpleOpenGlControl1.Width / 2.0), (double) this.simpleOpenGlControl1.Width / 2.0, (double) this.simpleOpenGlControl1.Height / 2.0, -((double) this.simpleOpenGlControl1.Height / 2.0), -1.0, 1.0);
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glClearColor(1f, 1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glEnable(3553);
      Gl.glBindTexture(3553, 0);
      Gl.glBegin(7);
      Gl.glTexCoord2f(0.0f, 0.0f);
      Gl.glVertex2f((float) -((double) this.simpleOpenGlControl1.Width / 2.0), (float) -((double) this.simpleOpenGlControl1.Height / 2.0));
      Gl.glTexCoord2f((float) this.simpleOpenGlControl1.Width / 16f, 0.0f);
      Gl.glVertex2f((float) this.simpleOpenGlControl1.Width / 2f, (float) -((double) this.simpleOpenGlControl1.Height / 2.0));
      Gl.glTexCoord2f((float) this.simpleOpenGlControl1.Width / 16f, (float) this.simpleOpenGlControl1.Height / 16f);
      Gl.glVertex2f((float) this.simpleOpenGlControl1.Width / 2f, (float) this.simpleOpenGlControl1.Height / 2f);
      Gl.glTexCoord2f(0.0f, (float) this.simpleOpenGlControl1.Height / 16f);
      Gl.glVertex2f((float) -((double) this.simpleOpenGlControl1.Width / 2.0), (float) this.simpleOpenGlControl1.Height / 2f);
      Gl.glEnd();
      Gl.glBindTexture(3553, 1);
      if (Matrix != null)
      {
        Gl.glMatrixMode(5890);
        Gl.glLoadMatrixf(Matrix);
      }
      Gl.glMatrixMode(5888);
      Gl.glBegin(7);
      Gl.glTexCoord2f(0.0f, 0.0f);
      Gl.glVertex2f((float) sbyte.MinValue, (float) sbyte.MinValue);
      Gl.glTexCoord2f(1f, 0.0f);
      Gl.glVertex2f(128f, (float) sbyte.MinValue);
      Gl.glTexCoord2f(1f, 1f);
      Gl.glVertex2f(128f, 128f);
      Gl.glTexCoord2f(0.0f, 1f);
      Gl.glVertex2f((float) sbyte.MinValue, 128f);
      Gl.glEnd();
      if (Matrix != null)
      {
        Gl.glMatrixMode(5890);
        Gl.glLoadIdentity();
      }
      this.simpleOpenGlControl1.Refresh();
    }

    private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
    {
      this.Render((float[]) null);
    }

    private void NSBTA_Shown(object sender, EventArgs e)
    {
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      if (this.treeView1.SelectedNode == null || e.Node.Parent == null)
        return;
      this.timeLine1.SuspendLayout();
      this.timeLine1.Groups.Clear();
      this.timeLine1.NrFrames = (uint) this.file.texSRTAnmSet.texSRTAnm[e.Node.Parent.Index].numFrame;
      string[] strArray = new string[5]
      {
        "Scale S",
        "Scale T",
        "Rotation",
        "Translation S",
        "Translation T"
      };
      KeyValuePair<string, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTA.TexSRTAnmSet.TexSRTAnm.TexSRTAnmData> keyValuePair;
      for (int index1 = 0; index1 < 5; ++index1)
      {
        TimeLine.Group group = new TimeLine.Group(strArray[index1]);
        keyValuePair = this.file.texSRTAnmSet.texSRTAnm[e.Node.Parent.Index].dict[e.Node.Index];
        switch (keyValuePair.Value.GetInterpolation(strArray[index1]))
        {
          case 0:
            group.Frames.Add(new TimeLine.Frame());
            for (int index2 = 0; (long) index2 < (long) (this.timeLine1.NrFrames - 1U); ++index2)
              group.Frames.Add(new TimeLine.Frame(false));
            break;
          case 1:
            for (int index2 = 0; (long) index2 < (long) this.timeLine1.NrFrames; ++index2)
              group.Frames.Add(new TimeLine.Frame());
            break;
          case 2:
            for (int index2 = 0; (long) index2 < (long) (this.timeLine1.NrFrames / 2U); ++index2)
            {
              group.Frames.Add(new TimeLine.Frame());
              group.Frames.Add(new TimeLine.Frame(false));
            }
            break;
          default:
            for (int index2 = 0; (long) index2 < (long) (this.timeLine1.NrFrames / 4U); ++index2)
            {
              group.Frames.Add(new TimeLine.Frame());
              group.Frames.Add(new TimeLine.Frame(false));
              group.Frames.Add(new TimeLine.Frame(false));
              group.Frames.Add(new TimeLine.Frame(false));
            }
            break;
        }
        this.timeLine1.Groups.Add(group);
      }
      this.timeLine1.ResumeLayout();
      keyValuePair = this.file.texSRTAnmSet.texSRTAnm[e.Node.Parent.Index].dict[e.Node.Index];
      this.selected = keyValuePair.Value;
      this.Render(this.selected.GetMatrix(0, 1, 1));
    }

    private void timeLine1_OnFrameChanged_1(int FrameNr)
    {
      this.Render(this.selected.GetMatrix(FrameNr, 1, 1));
    }

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
      this.treeView1 = new TreeView();
      this.splitContainer2 = new SplitContainer();
      this.splitContainer3 = new SplitContainer();
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.propertyGrid1 = new PropertyGrid();
      this.timeLine1 = new TimeLine();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.splitContainer3.BeginInit();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(745, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.treeView1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Size = new Size(745, 389);
      this.splitContainer1.SplitterDistance = 143;
      this.splitContainer1.TabIndex = 1;
      this.treeView1.Dock = DockStyle.Fill;
      this.treeView1.HideSelection = false;
      this.treeView1.Location = new Point(0, 0);
      this.treeView1.Name = "treeView1";
      this.treeView1.Size = new Size(143, 389);
      this.treeView1.TabIndex = 0;
      this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.Location = new Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = Orientation.Horizontal;
      this.splitContainer2.Panel1.Controls.Add((Control) this.splitContainer3);
      this.splitContainer2.Panel2.Controls.Add((Control) this.timeLine1);
      this.splitContainer2.Size = new Size(598, 389);
      this.splitContainer2.SplitterDistance = 230;
      this.splitContainer2.TabIndex = 0;
      this.splitContainer3.Dock = DockStyle.Fill;
      this.splitContainer3.Location = new Point(0, 0);
      this.splitContainer3.Name = "splitContainer3";
      this.splitContainer3.Panel1.Controls.Add((Control) this.simpleOpenGlControl1);
      this.splitContainer3.Panel2.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer3.Size = new Size(598, 230);
      this.splitContainer3.SplitterDistance = 435;
      this.splitContainer3.TabIndex = 0;
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
      this.simpleOpenGlControl1.Size = new Size(435, 230);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.Resize += new EventHandler(this.simpleOpenGlControl1_Resize);
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(159, 230);
      this.propertyGrid1.TabIndex = 0;
      this.timeLine1.CurFrame = 0U;
      this.timeLine1.Dock = DockStyle.Fill;
      this.timeLine1.Location = new Point(0, 0);
      this.timeLine1.Name = "timeLine1";
      this.timeLine1.NrFrames = 100U;
      this.timeLine1.Size = new Size(598, 155);
      this.timeLine1.TabIndex = 0;
      this.timeLine1.OnFrameChanged += new TimeLine.OnFrameChangedEventHandler(this.timeLine1_OnFrameChanged_1);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(745, 414);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.DoubleBuffered = true;
      this.Name = nameof (NSBTA);
      this.Text = nameof (NSBTA);
      this.Load += new EventHandler(this.NSBTA_Load);
      this.Shown += new EventHandler(this.NSBTA_Shown);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel2.ResumeLayout(false);
      this.splitContainer3.EndInit();
      this.splitContainer3.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
