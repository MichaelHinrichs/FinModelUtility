// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.MR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.IO;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class MR : Form
  {
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.MKDS.MR Mission;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton3;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripComboBox toolStripComboBox1;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripButton toolStripButton1;
    private ToolStripButton toolStripButton2;
    private PropertyGrid propertyGrid1;

    public MR(MKDS_Course_Modifier.MKDS.MR Mission)
    {
      this.Mission = Mission;
      this.InitializeComponent();
    }

    private void MR_Load(object sender, EventArgs e)
    {
      foreach (MKDS_Course_Modifier.MKDS.MR.MREntry entry in this.Mission.Entries)
        this.toolStripComboBox1.Items.Add((object) entry.NameOfMission);
      this.toolStripComboBox1.SelectedIndex = 0;
    }

    private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.propertyGrid1.SelectedObject = (object) (MRProperties.MREntry) this.Mission.Entries[this.toolStripComboBox1.SelectedIndex];
    }

    private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      this.Mission.Entries[this.toolStripComboBox1.SelectedIndex] = (MKDS_Course_Modifier.MKDS.MR.MREntry) (MRProperties.MREntry) this.propertyGrid1.SelectedObject;
      this.toolStripComboBox1.Items[this.toolStripComboBox1.SelectedIndex] = (object) this.Mission.Entries[this.toolStripComboBox1.SelectedIndex].NameOfMission;
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      this.toolStripComboBox1.Items.Add((object) ("mr" + (this.toolStripComboBox1.Items.Count + 1).ToString("##")));
      this.Mission.Entries.Add(new MKDS_Course_Modifier.MKDS.MR.MREntry());
      this.Mission.Entries[this.Mission.Entries.Count - 1].NameOfMission = "mr" + this.toolStripComboBox1.Items.Count.ToString("##");
      this.Mission.Header.NrEntry = (uint) this.Mission.Entries.Count;
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      int selectedIndex = this.toolStripComboBox1.SelectedIndex;
      this.Mission.Entries.RemoveAt(this.toolStripComboBox1.SelectedIndex);
      this.toolStripComboBox1.Items.RemoveAt(this.toolStripComboBox1.SelectedIndex);
      this.toolStripComboBox1.SelectedIndex = selectedIndex >= this.toolStripComboBox1.Items.Count ? selectedIndex - 1 : selectedIndex;
      this.Mission.Header.NrEntry = (uint) this.Mission.Entries.Count;
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      FileHandler.Save(this.Mission.Write(), 0, false);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (MR));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripComboBox1 = new ToolStripComboBox();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.propertyGrid1 = new PropertyGrid();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripComboBox1,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(462, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripComboBox1.Name = "toolStripComboBox1";
      this.toolStripComboBox1.Size = new Size(121, 25);
      this.toolStripComboBox1.SelectedIndexChanged += new EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
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
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 25);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(462, 322);
      this.propertyGrid1.TabIndex = 1;
      this.propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(462, 347);
      this.Controls.Add((Control) this.propertyGrid1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (MR);
      this.Text = nameof (MR);
      this.Load += new EventHandler(this.MR_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
