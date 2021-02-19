// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.CourseSelector
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Archive_Format;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.IO;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class CourseSelector : Form
  {
    private IContainer components = (IContainer) null;
    private ToolStrip toolStrip1;
    private ListView listView1;
    private ColumnHeader columnHeader1;
    private NDS Rom;

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
      this.Name = nameof (CourseSelector);
      this.Text = nameof (CourseSelector);
      this.Load += new EventHandler(this.CourseSelector_Load);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public CourseSelector(NDS Rom)
    {
      this.Rom = Rom;
      this.InitializeComponent();
    }

    private void CourseSelector_Load(object sender, EventArgs e)
    {
      foreach (NARC.FileEntry file in this.Rom.Root.GetDirectoryByPath("\\data\\Course").Files)
      {
        if (!file.Name.EndsWith("Tex.carc"))
          this.listView1.Items.Add(file.Name.Replace(".carc", ""));
      }
    }

    private void listView1_ItemActivate(object sender, EventArgs e)
    {
      int num = (int) new MKDSEditor(NARC.Unpack(Compression.LZ77Decompress(this.Rom.Root.GetFileByPath("\\data\\Course\\" + this.listView1.SelectedItems[0].Text + ".carc").Content)), NARC.Unpack(Compression.LZ77Decompress(this.Rom.Root.GetFileByPath("\\data\\Course\\" + this.listView1.SelectedItems[0].Text + "Tex.carc").Content)), this.Rom.Root).ShowDialog();
    }
  }
}
