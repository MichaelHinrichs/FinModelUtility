// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.ObjectDbEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.VisualBasic;
using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class ObjectDbEditor : Form
  {
    private IContainer components = (IContainer) null;
    private bool changed = false;
    private SplitContainer splitContainer1;
    private TreeView treeView1;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private ToolStripButton toolStripButton2;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton toolStripButton3;
    private ToolStripButton toolStripButton4;
    private SplitContainer splitContainer2;
    private PropertyGrid propertyGrid1;
    private Label label2;
    private PictureBox pictureBox1;
    private Label label1;
    private TableLayoutPanel tableLayoutPanel1;
    private ToolStripButton toolStripButton5;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (ObjectDbEditor));
      this.splitContainer1 = new SplitContainer();
      this.treeView1 = new TreeView();
      this.splitContainer2 = new SplitContainer();
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.pictureBox1 = new PictureBox();
      this.label2 = new Label();
      this.label1 = new Label();
      this.propertyGrid1 = new PropertyGrid();
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripButton4 = new ToolStripButton();
      this.toolStripButton5 = new ToolStripButton();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.treeView1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Size = new Size(722, 325);
      this.splitContainer1.SplitterDistance = 161;
      this.splitContainer1.TabIndex = 0;
      this.treeView1.Dock = DockStyle.Fill;
      this.treeView1.HideSelection = false;
      this.treeView1.HotTracking = true;
      this.treeView1.Location = new Point(0, 0);
      this.treeView1.Name = "treeView1";
      this.treeView1.Size = new Size(161, 325);
      this.treeView1.TabIndex = 0;
      this.treeView1.AfterSelect += new TreeViewEventHandler(this.treeView1_AfterSelect);
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.FixedPanel = FixedPanel.Panel2;
      this.splitContainer2.Location = new Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Panel1.Controls.Add((Control) this.tableLayoutPanel1);
      this.splitContainer2.Panel1.Controls.Add((Control) this.label1);
      this.splitContainer2.Panel2.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer2.Size = new Size(557, 325);
      this.splitContainer2.SplitterDistance = 368;
      this.splitContainer2.TabIndex = 0;
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.Controls.Add((Control) this.pictureBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label2, 1, 0);
      this.tableLayoutPanel1.Dock = DockStyle.Fill;
      this.tableLayoutPanel1.Location = new Point(0, 13);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.Size = new Size(368, 312);
      this.tableLayoutPanel1.TabIndex = 3;
      this.pictureBox1.BackColor = Color.White;
      this.pictureBox1.BackgroundImage = (Image) Resources.preview_background;
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.ErrorImage = (Image) null;
      this.pictureBox1.Location = new Point(3, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(178, 150);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      this.label2.Dock = DockStyle.Fill;
      this.label2.Location = new Point(187, 0);
      this.label2.Name = "label2";
      this.tableLayoutPanel1.SetRowSpan((Control) this.label2, 2);
      this.label2.Size = new Size(178, 312);
      this.label2.TabIndex = 2;
      this.label2.Text = "Description";
      this.label1.Dock = DockStyle.Top;
      this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(0, 0);
      this.label1.Name = "label1";
      this.label1.Size = new Size(368, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Object Name (Object ID)";
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(185, 325);
      this.propertyGrid1.TabIndex = 0;
      this.propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
      this.toolStrip1.Items.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripButton4,
        (ToolStripItem) this.toolStripButton5
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(722, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "Add Category";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Enabled = false;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "Remove Category";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Enabled = false;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "Add Object";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton4.Enabled = false;
      this.toolStripButton4.Image = (Image) componentResourceManager.GetObject("toolStripButton4.Image");
      this.toolStripButton4.ImageTransparentColor = Color.Magenta;
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.Size = new Size(23, 22);
      this.toolStripButton4.Text = "Remove Object";
      this.toolStripButton4.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripButton5.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton5.Enabled = false;
      this.toolStripButton5.Image = (Image) componentResourceManager.GetObject("toolStripButton5.Image");
      this.toolStripButton5.ImageTransparentColor = Color.Magenta;
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.Size = new Size(23, 22);
      this.toolStripButton5.Text = "toolStripButton5";
      this.toolStripButton5.Click += new EventHandler(this.toolStripButton5_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(722, 350);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (ObjectDbEditor);
      this.Text = nameof (ObjectDbEditor);
      this.FormClosing += new FormClosingEventHandler(this.ObjectDbEditor_FormClosing);
      this.Load += new EventHandler(this.ObjectDbEditor_Load);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

    public ObjectDbEditor()
    {
      this.InitializeComponent();
      ObjectDbEditor.SetWindowTheme(this.treeView1.Handle, "explorer", (string) null);
    }

    private void ObjectDbEditor_Load(object sender, EventArgs e)
    {
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, (string) null);
      this.treeView1.Sort();
      this.treeView1.EndUpdate();
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      string text = this.treeView1.SelectedNode.Text;
      string Name = Interaction.InputBox("Give the name of the category:", "", "", -1, -1);
      if (Name == "")
        return;
      this.changed = true;
      MKDS_Const.ObjectDatabase.AddCategory(this.treeView1.SelectedNode.FullPath, Name);
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, text);
      this.treeView1.Sort();
      this.treeView1.EndUpdate();
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      this.changed = true;
      string text = this.treeView1.SelectedNode.Parent.Text;
      MKDS_Const.ObjectDatabase.RemoveCategory(this.treeView1.SelectedNode.FullPath);
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, text);
      this.treeView1.Sort();
      this.treeView1.EndUpdate();
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      string text = this.treeView1.SelectedNode.Text;
      string str = Interaction.InputBox("Give the object id:", "", "", -1, -1);
      if (str == "")
        return;
      ushort ObjectId = ushort.Parse(str[2].ToString() + (object) str[3] + (object) str[0] + (object) str[1], NumberStyles.HexNumber);
      ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(ObjectId);
      if (@object != null)
      {
        int num = (int) MessageBox.Show("This object id already exist in the database:\r\n" + @object.Name, "This object already exist", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
      {
        string Name = Interaction.InputBox("Give the name of the object:", "", "", -1, -1);
        if (Name == "")
          return;
        this.changed = true;
        MKDS_Const.ObjectDatabase.AddObject(this.treeView1.SelectedNode.FullPath, Name, ObjectId);
        this.treeView1.BeginUpdate();
        this.treeView1.Nodes.Clear();
        MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, text);
        this.treeView1.Sort();
        this.treeView1.EndUpdate();
      }
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (e.Node.FullPath != "Root" && !(bool) e.Node.Tag)
      {
        this.toolStripButton1.Enabled = true;
        this.toolStripButton2.Enabled = true;
        this.toolStripButton3.Enabled = true;
        this.toolStripButton4.Enabled = false;
        this.toolStripButton5.Enabled = false;
        this.propertyGrid1.SelectedObject = (object) null;
        this.label1.Text = "";
        this.label2.Text = "";
        this.pictureBox1.Image = (Image) null;
      }
      else if ((bool) e.Node.Tag)
      {
        this.toolStripButton1.Enabled = false;
        this.toolStripButton2.Enabled = false;
        this.toolStripButton3.Enabled = false;
        this.toolStripButton4.Enabled = true;
        this.toolStripButton5.Enabled = true;
        ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(e.Node.FullPath);
        this.propertyGrid1.SelectedObject = (object) @object;
        this.label1.Text = @object.ToString();
        this.label2.Text = @object.Description;
        @object.GetPictureAsyc(this.pictureBox1);
      }
      else
      {
        this.toolStripButton1.Enabled = true;
        this.toolStripButton2.Enabled = false;
        this.toolStripButton3.Enabled = false;
        this.toolStripButton4.Enabled = false;
        this.toolStripButton5.Enabled = false;
        this.propertyGrid1.SelectedObject = (object) null;
        this.label1.Text = "";
        this.label2.Text = "";
        this.pictureBox1.Image = (Image) null;
      }
    }

    private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      this.changed = true;
      if (e.ChangedItem.Label == "Name")
      {
        this.treeView1.BeginUpdate();
        this.treeView1.Nodes.Clear();
        MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, (string) null);
        this.treeView1.Sort();
        this.treeView1.EndUpdate();
      }
      ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(((ObjectDb.Object) this.propertyGrid1.SelectedObject).ObjectId);
      this.label1.Text = @object.ToString();
      this.label2.Text = @object.Description;
      this.pictureBox1.Image = (Image) @object.GetPicture();
    }

    private void toolStripButton4_Click(object sender, EventArgs e)
    {
      this.changed = true;
      string text = this.treeView1.SelectedNode.Parent.Text;
      MKDS_Const.ObjectDatabase.RemoveObject(this.treeView1.SelectedNode.FullPath);
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, text);
      this.treeView1.Sort();
      this.treeView1.EndUpdate();
    }

    private void ObjectDbEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!this.changed)
        return;
      switch (MessageBox.Show("Do you want to save?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
      {
        case DialogResult.Cancel:
          e.Cancel = true;
          break;
        case DialogResult.Yes:
          System.IO.File.Create("ObjectDb.xml").Close();
          System.IO.File.WriteAllBytes("ObjectDb.xml", MKDS_Const.ObjectDatabase.Write());
          break;
        case DialogResult.No:
          if (MessageBox.Show("Are you sure you don't want to save?", "Don't Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
          {
            MKDS_Const.ObjectDatabase = new ObjectDb(System.IO.File.ReadAllBytes("ObjectDb.xml"), System.IO.File.ReadAllBytes("ObjectDb.xsd"));
            break;
          }
          e.Cancel = true;
          break;
      }
    }

    private void toolStripButton5_Click(object sender, EventArgs e)
    {
      int num = (int) new ObjectDbWikiTextGenerator(MKDS_Const.ObjectDatabase.GetObject(((ObjectDb.Object) this.propertyGrid1.SelectedObject).ObjectId)).ShowDialog();
    }
  }
}
