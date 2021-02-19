// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MPDS.BoardSelector
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.MPDS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MPDS
{
  public class BoardSelector : Form
  {
    private IContainer components = (IContainer) null;
    private NDS Rom;
    private ToolStrip toolStrip1;
    private ListView listView1;
    private ColumnHeader columnHeader1;

    public BoardSelector(NDS Rom)
    {
      this.Rom = Rom;
      this.InitializeComponent();
    }

    private void CourseSelector_Load(object sender, EventArgs e)
    {
      BMAP bmap = new BMAP(Compression.LZ77Decompress(this.Rom.Root.GetFileByPath("\\data\\UK\\mpd_050_bmap_name_LZ.bin").Content));
      this.listView1.Items.Add("bmap_00");
      for (int index = 0; (long) index < (long) bmap.NrFiles; ++index)
        this.listView1.Items.Add(new string(Encoding.Unicode.GetChars(bmap.Files[index].Data)));
    }

    private void listView1_ItemActivate(object sender, EventArgs e)
    {
      int num = (int) new MPDSEditor(new BMAP(this.Rom.Root.GetFileByPath("\\data\\bmap_0" + (object) this.listView1.SelectedItems[0].Index + "_LZ.bin").Content), new BMAP(this.Rom.Root.GetFileByPath("\\data\\bmap_sys_LZ.bin").Content)).ShowDialog();
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
      this.listView1 = new ListView();
      this.columnHeader1 = new ColumnHeader();
      this.SuspendLayout();
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(442, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.listView1.Columns.AddRange(new ColumnHeader[1]
      {
        this.columnHeader1
      });
      this.listView1.Dock = DockStyle.Fill;
      this.listView1.GridLines = true;
      this.listView1.HeaderStyle = ColumnHeaderStyle.None;
      this.listView1.Location = new Point(0, 25);
      this.listView1.Name = "listView1";
      this.listView1.Size = new Size(442, 315);
      this.listView1.TabIndex = 1;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Details;
      this.listView1.ItemActivate += new EventHandler(this.listView1_ItemActivate);
      this.columnHeader1.Text = "Name";
      this.columnHeader1.Width = 500;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(442, 340);
      this.Controls.Add((Control) this.listView1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = "CourseSelector";
      this.Text = nameof (BoardSelector);
      this.Load += new EventHandler(this.CourseSelector_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
