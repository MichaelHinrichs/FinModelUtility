// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.BMG
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class BMG : Form
  {
    private IContainer components = (IContainer) null;
    public MKDS_Course_Modifier.Misc.BMG File;
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private DataGridView dataGridView1;
    private DataGridViewTextBoxColumn TextColumn;
    private TextBox textBox1;
    private ToolStripButton toolStripButton1;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton toolStripButton2;
    private ToolStripButton toolStripButton3;
    private OpenFileDialog openFileDialog1;
    private SaveFileDialog saveFileDialog1;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripButton toolStripButton4;
    private ToolStripButton toolStripButton5;
    private ToolStripButton toolStripButton6;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripButton toolStripButton7;
    private ToolStripButton toolStripButton8;

    public BMG(MKDS_Course_Modifier.Misc.BMG File)
    {
      this.File = File;
      this.InitializeComponent();
      this.toolStripButton1.Text = LanguageHandler.GetString("base.save");
      this.toolStripButton2.Text = LanguageHandler.GetString("base.import");
      this.toolStripButton3.Text = LanguageHandler.GetString("base.export");
      this.toolStripButton4.Text = LanguageHandler.GetString("base.add");
      this.toolStripButton5.Text = LanguageHandler.GetString("base.remove");
    }

    private void BMG_Load(object sender, EventArgs e)
    {
      foreach (string str in this.File.DAT1.Strings)
        this.dataGridView1.Rows.Add((object) str);
    }

    private void dataGridView1_SelectionChanged(object sender, EventArgs e)
    {
      if (this.dataGridView1.SelectedCells.Count == 0)
        return;
      this.textBox1.Text = this.File.DAT1.Strings[this.dataGridView1.SelectedCells[0].RowIndex];
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      this.dataGridView1.Rows[this.dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value = (object) (this.File.DAT1.Strings[this.dataGridView1.SelectedCells[0].RowIndex] = this.textBox1.Text);
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      FileHandler.Save(this.File.Save(), 0, false);
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      this.File.DAT1.FromTxt(System.IO.File.ReadAllBytes(this.openFileDialog1.FileName));
      this.dataGridView1.Rows.Clear();
      foreach (string str in this.File.DAT1.Strings)
        this.dataGridView1.Rows.Add((object) str);
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      System.IO.File.Create(this.saveFileDialog1.FileName).Close();
      System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, this.File.DAT1.ToTxt());
    }

    private void toolStripButton4_Click(object sender, EventArgs e)
    {
      ++this.File.INF1.NrOffset;
      this.File.DAT1.Strings = new List<string>((IEnumerable<string>) this.File.DAT1.Strings)
      {
        "Dummy Text"
      }.ToArray();
      this.dataGridView1.Rows.Add((object) "Dummy Text");
      this.dataGridView1.Rows[this.dataGridView1.SelectedCells[0].RowIndex - 1].Selected = false;
      this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
    }

    private void toolStripButton5_Click(object sender, EventArgs e)
    {
      --this.File.INF1.NrOffset;
      List<string> stringList = new List<string>((IEnumerable<string>) this.File.DAT1.Strings);
      stringList.RemoveAt(this.dataGridView1.SelectedCells[0].RowIndex);
      this.File.DAT1.Strings = stringList.ToArray();
      this.dataGridView1.Rows.RemoveAt(this.dataGridView1.SelectedCells[0].RowIndex);
    }

    private void toolStripButton6_Click(object sender, EventArgs e)
    {
    }

    private void toolStripButton6_Click_1(object sender, EventArgs e)
    {
      ++this.File.INF1.NrOffset;
      List<string> stringList = new List<string>((IEnumerable<string>) this.File.DAT1.Strings);
      stringList.Insert(this.dataGridView1.SelectedCells[0].RowIndex, "Dummy Text");
      this.File.DAT1.Strings = stringList.ToArray();
      this.dataGridView1.Rows.Insert(this.dataGridView1.SelectedCells[0].RowIndex, (object) "Dummy Text");
      int index = this.dataGridView1.SelectedCells[0].RowIndex - 1;
      this.dataGridView1.Rows[index + 1].Selected = false;
      this.dataGridView1.Rows[index].Selected = true;
    }

    private void toolStripButton7_Click(object sender, EventArgs e)
    {
      int rowIndex = this.dataGridView1.SelectedCells[0].RowIndex;
      if (rowIndex == 0)
        return;
      string str1 = this.File.DAT1.Strings[rowIndex];
      string str2 = this.File.DAT1.Strings[rowIndex - 1];
      this.File.DAT1.Strings[rowIndex] = str2;
      this.File.DAT1.Strings[rowIndex - 1] = str1;
      this.dataGridView1.Rows[rowIndex].SetValues((object) str2);
      this.dataGridView1.Rows[rowIndex - 1].SetValues((object) str1);
      this.dataGridView1.Rows[rowIndex].Selected = false;
      this.dataGridView1.Rows[rowIndex - 1].Selected = true;
    }

    private void toolStripButton8_Click(object sender, EventArgs e)
    {
      int rowIndex = this.dataGridView1.SelectedCells[0].RowIndex;
      if (rowIndex == this.dataGridView1.Rows.Count - 1)
        return;
      string str1 = this.File.DAT1.Strings[rowIndex];
      string str2 = this.File.DAT1.Strings[rowIndex + 1];
      this.File.DAT1.Strings[rowIndex] = str2;
      this.File.DAT1.Strings[rowIndex + 1] = str1;
      this.dataGridView1.Rows[rowIndex].SetValues((object) str2);
      this.dataGridView1.Rows[rowIndex + 1].SetValues((object) str1);
      this.dataGridView1.Rows[rowIndex].Selected = false;
      this.dataGridView1.Rows[rowIndex + 1].Selected = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (BMG));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton2 = new ToolStripButton();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStripButton4 = new ToolStripButton();
      this.toolStripButton5 = new ToolStripButton();
      this.splitContainer1 = new SplitContainer();
      this.dataGridView1 = new DataGridView();
      this.TextColumn = new DataGridViewTextBoxColumn();
      this.textBox1 = new TextBox();
      this.openFileDialog1 = new OpenFileDialog();
      this.saveFileDialog1 = new SaveFileDialog();
      this.toolStripButton6 = new ToolStripButton();
      this.toolStripSeparator3 = new ToolStripSeparator();
      this.toolStripButton7 = new ToolStripButton();
      this.toolStripButton8 = new ToolStripButton();
      this.toolStrip1.SuspendLayout();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[11]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton4,
        (ToolStripItem) this.toolStripButton6,
        (ToolStripItem) this.toolStripButton5,
        (ToolStripItem) this.toolStripSeparator3,
        (ToolStripItem) this.toolStripButton7,
        (ToolStripItem) this.toolStripButton8
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(513, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
      this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton4.Image = (Image) componentResourceManager.GetObject("toolStripButton4.Image");
      this.toolStripButton4.ImageTransparentColor = Color.Magenta;
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.Size = new Size(23, 22);
      this.toolStripButton4.Text = "toolStripButton4";
      this.toolStripButton4.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton5.Image = (Image) componentResourceManager.GetObject("toolStripButton5.Image");
      this.toolStripButton5.ImageTransparentColor = Color.Magenta;
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.Size = new Size(23, 22);
      this.toolStripButton5.Text = "toolStripButton5";
      this.toolStripButton5.Click += new EventHandler(this.toolStripButton5_Click);
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel2;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = Orientation.Horizontal;
      this.splitContainer1.Panel1.Controls.Add((Control) this.dataGridView1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.textBox1);
      this.splitContainer1.Size = new Size(513, 350);
      this.splitContainer1.SplitterDistance = 247;
      this.splitContainer1.TabIndex = 1;
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeColumns = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange((DataGridViewColumn) this.TextColumn);
      this.dataGridView1.Dock = DockStyle.Fill;
      this.dataGridView1.Location = new Point(0, 0);
      this.dataGridView1.MultiSelect = false;
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.Size = new Size(513, 247);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.SelectionChanged += new EventHandler(this.dataGridView1_SelectionChanged);
      this.TextColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.TextColumn.HeaderText = "Text";
      this.TextColumn.Name = "TextColumn";
      this.TextColumn.ReadOnly = true;
      this.TextColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.textBox1.Dock = DockStyle.Fill;
      this.textBox1.Location = new Point(0, 0);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new Size(513, 99);
      this.textBox1.TabIndex = 0;
      this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
      this.openFileDialog1.DefaultExt = "txt";
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "TXT Files(*.txt)|*.txt";
      this.saveFileDialog1.DefaultExt = "txt";
      this.saveFileDialog1.Filter = "TXT Files(*.txt)|*.txt";
      this.toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton6.Image = (Image) componentResourceManager.GetObject("toolStripButton6.Image");
      this.toolStripButton6.ImageTransparentColor = Color.Magenta;
      this.toolStripButton6.Name = "toolStripButton6";
      this.toolStripButton6.Size = new Size(23, 22);
      this.toolStripButton6.Text = "toolStripButton6";
      this.toolStripButton6.Click += new EventHandler(this.toolStripButton6_Click_1);
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new Size(6, 25);
      this.toolStripButton7.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton7.Image = (Image) Resources.arrow_090;
      this.toolStripButton7.ImageTransparentColor = Color.Magenta;
      this.toolStripButton7.Name = "toolStripButton7";
      this.toolStripButton7.Size = new Size(23, 22);
      this.toolStripButton7.Text = "toolStripButton7";
      this.toolStripButton7.Click += new EventHandler(this.toolStripButton7_Click);
      this.toolStripButton8.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton8.Image = (Image) Resources.arrow_270;
      this.toolStripButton8.ImageTransparentColor = Color.Magenta;
      this.toolStripButton8.Name = "toolStripButton8";
      this.toolStripButton8.Size = new Size(23, 22);
      this.toolStripButton8.Text = "toolStripButton8";
      this.toolStripButton8.Click += new EventHandler(this.toolStripButton8_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(513, 375);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (BMG);
      this.Text = "BMG Editor";
      this.Load += new EventHandler(this.BMG_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
