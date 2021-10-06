// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.ObjectSelector
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.MKDS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class ObjectSelector : Form
  {
    private IContainer components = (IContainer) null;
    public ushort ObjectID;
    private SplitContainer splitContainer1;
    private TreeView treeView1;
    private TableLayoutPanel tableLayoutPanel1;
    private PictureBox pictureBox1;
    private Label label2;
    private Label label1;
    private Panel panel1;
    private Button button2;
    private Button button1;

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

    public ObjectSelector(ushort ObjectID)
    {
      this.ObjectID = ObjectID;
      this.InitializeComponent();
      ObjectSelector.SetWindowTheme(this.treeView1.Handle, "explorer", (string) null);
    }

    private void ObjectSelector_Load(object sender, EventArgs e)
    {
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      MKDS_Const.ObjectDatabase.GetTreeNodes(this.treeView1.Nodes, (string) null);
      this.treeView1.Sort();
      this.treeView1.EndUpdate();
      ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(this.ObjectID);
      if (@object != null)
      {
        this.label1.Text = @object.ToString();
        this.label2.Text = @object.Description;
        @object.GetPictureAsyc(this.pictureBox1);
      }
      else
        this.label1.Text = "Unknown (" + BitConverter.ToString(BitConverter.GetBytes(this.ObjectID)).Replace("-", "") + ")";
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (e.Node.FullPath != "Root" && !(bool) e.Node.Tag || !(bool) e.Node.Tag)
        return;
      ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(e.Node.FullPath);
      this.label1.Text = @object.ToString();
      this.label2.Text = @object.Description;
      @object.GetPictureAsyc(this.pictureBox1);
      this.ObjectID = @object.ObjectId;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.splitContainer1 = new SplitContainer();
      this.treeView1 = new TreeView();
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.pictureBox1 = new PictureBox();
      this.label2 = new Label();
      this.label1 = new Label();
      this.panel1 = new Panel();
      this.button1 = new Button();
      this.button2 = new Button();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.Location = new Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.treeView1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.tableLayoutPanel1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.label1);
      this.splitContainer1.Size = new Size(603, 359);
      this.splitContainer1.SplitterDistance = 201;
      this.splitContainer1.TabIndex = 0;
      this.treeView1.Dock = DockStyle.Fill;
      this.treeView1.Location = new Point(0, 0);
      this.treeView1.Name = "treeView1";
      this.treeView1.Size = new Size(201, 359);
      this.treeView1.TabIndex = 0;
      this.treeView1.AfterSelect += new TreeViewEventHandler(this.treeView1_AfterSelect);
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.Controls.Add((Control) this.pictureBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label2, 1, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.panel1, 0, 2);
      this.tableLayoutPanel1.Dock = DockStyle.Fill;
      this.tableLayoutPanel1.Location = new Point(0, 13);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel1.Size = new Size(398, 346);
      this.tableLayoutPanel1.TabIndex = 5;
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(3, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(193, 152);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      this.label2.Dock = DockStyle.Fill;
      this.label2.Location = new Point(202, 0);
      this.label2.Name = "label2";
      this.tableLayoutPanel1.SetRowSpan((Control) this.label2, 2);
      this.label2.Size = new Size(193, 316);
      this.label2.TabIndex = 2;
      this.label2.Text = "Description";
      this.label1.Dock = DockStyle.Top;
      this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(0, 0);
      this.label1.Name = "label1";
      this.label1.Size = new Size(398, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Object Name (Object ID)";
      this.tableLayoutPanel1.SetColumnSpan((Control) this.panel1, 2);
      this.panel1.Controls.Add((Control) this.button2);
      this.panel1.Controls.Add((Control) this.button1);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(3, 319);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(392, 24);
      this.panel1.TabIndex = 3;
      this.button1.DialogResult = DialogResult.OK;
      this.button1.Location = new Point(314, 0);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button2.DialogResult = DialogResult.Cancel;
      this.button2.Location = new Point(233, 0);
      this.button2.Name = "button2";
      this.button2.Size = new Size(75, 23);
      this.button2.TabIndex = 1;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(603, 359);
      this.Controls.Add((Control) this.splitContainer1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (ObjectSelector);
      this.Text = nameof (ObjectSelector);
      this.Load += new EventHandler(this.ObjectSelector_Load);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
