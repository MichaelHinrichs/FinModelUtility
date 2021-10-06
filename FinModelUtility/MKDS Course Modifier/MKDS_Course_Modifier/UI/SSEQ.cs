// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.SSEQ
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Properties;
using MKDS_Course_Modifier.Sound;
using NAudio.Midi;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class SSEQ : Form
  {
    private IContainer components = (IContainer) null;
    private bool first = true;
    private string midi = "";
    private string dlss = "";
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private CheckedListBox checkedListBox1;
    private TreeView treeView1;
    private Panel panel1;
    private SaveFileDialog saveFileDialog1;
    private ToolStripButton toolStripButton1;
    private ToolStripButton toolStripButton2;
    private ToolStripButton toolStripButton3;
    private ImageList imageList1;
    private MusicPlayer m;
    private byte[] dls;
    private MKDS_Course_Modifier.Sound.SSEQ file;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SSEQ));
      this.toolStrip1 = new ToolStrip();
      this.splitContainer1 = new SplitContainer();
      this.splitContainer2 = new SplitContainer();
      this.panel1 = new Panel();
      this.checkedListBox1 = new CheckedListBox();
      this.treeView1 = new TreeView();
      this.saveFileDialog1 = new SaveFileDialog();
      this.imageList1 = new ImageList(this.components);
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStrip1.SuspendLayout();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripButton3
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(663, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel2;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Panel2.Controls.Add((Control) this.treeView1);
      this.splitContainer1.Size = new Size(663, 364);
      this.splitContainer1.SplitterDistance = 435;
      this.splitContainer1.TabIndex = 1;
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.FixedPanel = FixedPanel.Panel2;
      this.splitContainer2.Location = new Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = Orientation.Horizontal;
      this.splitContainer2.Panel1.Controls.Add((Control) this.panel1);
      this.splitContainer2.Panel2.Controls.Add((Control) this.checkedListBox1);
      this.splitContainer2.Size = new Size(435, 364);
      this.splitContainer2.SplitterDistance = 244;
      this.splitContainer2.TabIndex = 0;
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(435, 244);
      this.panel1.TabIndex = 0;
      this.checkedListBox1.CheckOnClick = true;
      this.checkedListBox1.Dock = DockStyle.Fill;
      this.checkedListBox1.FormattingEnabled = true;
      this.checkedListBox1.Location = new Point(0, 0);
      this.checkedListBox1.MultiColumn = true;
      this.checkedListBox1.Name = "checkedListBox1";
      this.checkedListBox1.Size = new Size(435, 116);
      this.checkedListBox1.TabIndex = 0;
      this.treeView1.Dock = DockStyle.Fill;
      this.treeView1.HotTracking = true;
      this.treeView1.ImageIndex = 0;
      this.treeView1.ImageList = this.imageList1;
      this.treeView1.Location = new Point(0, 0);
      this.treeView1.Name = "treeView1";
      this.treeView1.SelectedImageIndex = 0;
      this.treeView1.Size = new Size(224, 364);
      this.treeView1.TabIndex = 0;
      this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList1.ImageSize = new Size(16, 16);
      this.imageList1.TransparentColor = Color.Transparent;
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton3.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton2_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(663, 389);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (SSEQ);
      this.Text = nameof (SSEQ);
      this.FormClosing += new FormClosingEventHandler(this.sseq_FormClosing);
      this.Load += new EventHandler(this.SSEQ_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

    public SSEQ(MKDS_Course_Modifier.Sound.SSEQ file, MusicPlayer m)
    {
      this.m = m;
      this.file = file;
      this.InitializeComponent();
      SSEQ.SetWindowTheme(this.treeView1.Handle, "explorer", (string) null);
      this.imageList1.Images.Add((Image) Resources.metronome);
      this.imageList1.Images.Add((Image) Resources.music);
      this.imageList1.Images.Add((Image) Resources.speaker_network);
      this.imageList1.Images.Add((Image) Resources.clock_select);
      this.imageList1.Images.Add((Image) Resources.speaker_new);
      this.imageList1.Images.Add((Image) Resources.guitar_small);
      this.imageList1.Images.Add((Image) Resources.arrow_return_180);
      this.imageList1.Images.Add((Image) Resources.arrow_return_180_left);
      this.imageList1.Images.Add((Image) Resources.speaker_volume);
      this.imageList1.Images.Add((Image) Resources.question_white);
      TreeNode[] treeNodeArray = new TreeNode[file.Data.GetTreeNodes().Count];
      file.Data.GetTreeNodes().CopyTo((Array) treeNodeArray, 0);
      foreach (TreeNode treeNode in treeNodeArray)
        this.treeView1.Nodes.Add((TreeNode) treeNode.Clone());
      this.treeView1.Nodes[0].Expand();
      this.checkedListBox1.Items.AddRange((ListBox.ObjectCollection) file.Data.GetCheckedListboxItems());
      for (int index = 0; index < this.checkedListBox1.Items.Count; ++index)
        this.checkedListBox1.SetItemChecked(index, true);
    }

    public void SetDLS(byte[] DLS)
    {
      this.dls = DLS;
    }

    private void SSEQ_Load(object sender, EventArgs e)
    {
    }

    private void sseq_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.m.Stop();
      this.m.Unload();
      if (!File.Exists(this.midi))
        return;
      File.Delete(this.midi);
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      MidiEventCollection midi = this.file.Data.Midi;
      if (this.first)
      {
        midi.PrepareForExport();
        this.midi = Path.GetTempFileName();
        MidiFile.Export(this.midi, midi);
      }
      if (this.first && this.dls != null)
      {
        this.dlss = Path.GetTempFileName();
        File.WriteAllBytes(this.dlss, this.dls);
      }
      this.m.SetMidi(this.midi);
      if (this.first && this.dls != null)
        this.m.SetDLS(this.dlss);
      if (this.file.Data.GetLoopStart() != -1 && this.file.Data.GetLoopEnd() != -1 && this.first)
      {
        int length = this.m.GetLength();
        int Start = this.file.Data.GetLoopStart() * 16;
        int End = this.file.Data.GetLoopEnd() * 16;
        if (Start <= length && End <= length && End >= Start)
          this.m.SetLoop(Start, End, this.file.Data.GetNrLoop());
      }
      if (this.file.Data.GetLoopStart() == -1 || this.file.Data.GetLoopEnd() == -1)
        ;
      if (this.first)
        this.first = false;
      this.m.Play();
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you want to export all channels?", "Export", MessageBoxButtons.YesNo) == DialogResult.No)
      {
        MidiEventCollection events = new MidiEventCollection(1, 48);
        for (int index = 0; index < this.file.Data.Midi.Tracks; ++index)
        {
          if (this.checkedListBox1.GetItemChecked(index))
            events.AddTrack(this.file.Data.Midi[index]);
        }
        if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
          return;
        MidiFile.Export(this.saveFileDialog1.FileName, events);
      }
      else if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        MidiFile.Export(this.saveFileDialog1.FileName, this.file.Data.Midi);
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      this.m.Stop();
    }
  }
}
