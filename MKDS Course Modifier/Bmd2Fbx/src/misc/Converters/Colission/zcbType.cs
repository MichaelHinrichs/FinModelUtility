// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.zcbType
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Converters.Colission
{
  public class zcbType : Form
  {
    private IContainer components = (IContainer) null;
    private DataGridView dataGridView1;
    private DataRepeaterItem ItemTemplate;
    private DataGridViewTextBoxColumn name;
    private DataGridViewCheckBoxColumn Collide;
    private DataGridViewTextBoxColumn Type;
    public Dictionary<string, uint> Mapping;
    public Dictionary<string, bool> Colli;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.dataGridView1 = new DataGridView();
      this.ItemTemplate = new DataRepeaterItem();
      this.name = new DataGridViewTextBoxColumn();
      this.Collide = new DataGridViewCheckBoxColumn();
      this.Type = new DataGridViewTextBoxColumn();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange((DataGridViewColumn) this.name, (DataGridViewColumn) this.Collide, (DataGridViewColumn) this.Type);
      this.dataGridView1.Dock = DockStyle.Fill;
      this.dataGridView1.Location = new Point(0, 0);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ShowEditingIcon = false;
      this.dataGridView1.Size = new Size(497, 304);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.CellEndEdit += new DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
      this.dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
      this.ItemTemplate.Size = new Size(232, 100);
      this.name.HeaderText = "Name";
      this.name.Name = "name";
      this.name.ReadOnly = true;
      this.Collide.FlatStyle = FlatStyle.System;
      this.Collide.HeaderText = "Collide";
      this.Collide.Name = "Collide";
      this.Type.HeaderText = "Type";
      this.Type.MaxInputLength = 8;
      this.Type.Name = "Type";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(497, 304);
      this.Controls.Add((Control) this.dataGridView1);
      this.Name = nameof (zcbType);
      this.Text = "kclType";
      this.FormClosing += new FormClosingEventHandler(this.kclType_FormClosing);
      this.FormClosed += new FormClosedEventHandler(this.kclType_FormClosed);
      this.Load += new EventHandler(this.kclType_Load);
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
    }

    public zcbType(string[] names)
    {
      this.InitializeComponent();
      this.Mapping = new Dictionary<string, uint>();
      this.Colli = new Dictionary<string, bool>();
      for (int index = 0; index < names.Length; ++index)
      {
        this.Mapping.Add(names[index], 124780544U);
        this.Colli.Add(names[index], true);
        this.dataGridView1.Rows.Add((object) names[index], (object) true, (object) "07700000");
      }
      if (this.Mapping.Count != 0)
        return;
      this.Mapping.Add("Default", 124780544U);
      this.Colli.Add("Default", true);
      this.dataGridView1.Rows.Add((object) "Default", (object) true, (object) "07700000");
    }

    private void kclType_Load(object sender, EventArgs e)
    {
    }

    private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
      if (e.ColumnIndex != 2)
        return;
      e.Cancel = !uint.TryParse(e.FormattedValue.ToString(), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out uint _);
    }

    private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      switch (e.ColumnIndex)
      {
        case 1:
          this.Colli[this.dataGridView1.Rows[e.RowIndex].Cells[0].Value as string] = (bool) this.dataGridView1.Rows[e.RowIndex].Cells[1].Value;
          break;
        case 2:
          this.dataGridView1.Rows[e.RowIndex].Cells[2].Value = (object) string.Format("{0:X8}", (object) uint.Parse(this.dataGridView1.Rows[e.RowIndex].Cells[2].Value as string, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture));
          this.Mapping[this.dataGridView1.Rows[e.RowIndex].Cells[0].Value as string] = uint.Parse(this.dataGridView1.Rows[e.RowIndex].Cells[2].Value as string, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
          break;
      }
    }

    private void kclType_FormClosed(object sender, FormClosedEventArgs e)
    {
      this.DialogResult = DialogResult.OK;
    }

    private void kclType_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.dataGridView1.EndEdit();
    }
  }
}
