// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI._3DExportSettings
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class _3DExportSettings : Form
  {
    public string Format = "PNG";
    private IContainer components = (IContainer) null;
    private ComboBox comboBox1;
    private GroupBox groupBox1;
    private Label label1;
    private Button button1;

    public _3DExportSettings()
    {
      this.InitializeComponent();
      this.groupBox1.Text = LanguageHandler.GetString("3d.textures");
      this.label1.Text = LanguageHandler.GetString("3d.imgformat");
    }

    private void _3DExportSettings_Load(object sender, EventArgs e)
    {
      this.comboBox1.SelectedIndex = 0;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Format = (string) this.comboBox1.SelectedItem;
      this.Close();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.comboBox1 = new ComboBox();
      this.groupBox1 = new GroupBox();
      this.label1 = new Label();
      this.button1 = new Button();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBox1.FlatStyle = FlatStyle.System;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[3]
      {
        (object) "PNG",
        (object) "TIFF",
        (object) "TGA"
      });
      this.comboBox1.Location = new Point(125, 19);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new Size(121, 21);
      this.comboBox1.TabIndex = 0;
      this.groupBox1.Controls.Add((Control) this.label1);
      this.groupBox1.Controls.Add((Control) this.comboBox1);
      this.groupBox1.Location = new Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(252, 46);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Textures";
      this.label1.AutoSize = true;
      this.label1.Location = new Point(6, 22);
      this.label1.Name = "label1";
      this.label1.Size = new Size(74, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Image Format:";
      this.button1.Location = new Point(189, 68);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 2;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(276, 103);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.groupBox1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (_3DExportSettings);
      this.Text = "3D Export";
      this.Load += new EventHandler(this._3DExportSettings_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
