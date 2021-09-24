// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters._3D.NSBMDSettings
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Converters._3D
{
  public class NSBMDSettings : Form
  {
    private IContainer components = (IContainer) null;
    public float Scale = 1f;
    public bool CreateNSBTX = false;
    private GroupBox groupBox1;
    private NumericUpDown numericUpDown1;
    private RadioButton radioButton3;
    private RadioButton radioButton2;
    private RadioButton radioButton1;
    private Button button1;
    private GroupBox groupBox2;
    private CheckBox checkBox1;
    private Button button2;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.groupBox1 = new GroupBox();
      this.numericUpDown1 = new NumericUpDown();
      this.radioButton3 = new RadioButton();
      this.radioButton2 = new RadioButton();
      this.radioButton1 = new RadioButton();
      this.button1 = new Button();
      this.groupBox2 = new GroupBox();
      this.checkBox1 = new CheckBox();
      this.button2 = new Button();
      this.groupBox1.SuspendLayout();
      this.numericUpDown1.BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      this.groupBox1.Controls.Add((Control) this.numericUpDown1);
      this.groupBox1.Controls.Add((Control) this.radioButton3);
      this.groupBox1.Controls.Add((Control) this.radioButton2);
      this.groupBox1.Controls.Add((Control) this.radioButton1);
      this.groupBox1.Location = new Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(225, 95);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Scale";
      this.numericUpDown1.DecimalPlaces = 4;
      this.numericUpDown1.Enabled = false;
      this.numericUpDown1.Location = new Point(75, 65);
      this.numericUpDown1.Maximum = new Decimal(new int[4]
      {
        1000,
        0,
        0,
        0
      });
      this.numericUpDown1.Minimum = new Decimal(new int[4]
      {
        1,
        0,
        0,
        262144
      });
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new Size(144, 20);
      this.numericUpDown1.TabIndex = 3;
      this.numericUpDown1.Value = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numericUpDown1.ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
      this.radioButton3.AutoSize = true;
      this.radioButton3.Location = new Point(6, 65);
      this.radioButton3.Name = "radioButton3";
      this.radioButton3.Size = new Size(63, 17);
      this.radioButton3.TabIndex = 2;
      this.radioButton3.Text = "Custom:";
      this.radioButton3.UseVisualStyleBackColor = true;
      this.radioButton3.CheckedChanged += new EventHandler(this.radioButton2_CheckedChanged);
      this.radioButton2.AutoSize = true;
      this.radioButton2.Checked = true;
      this.radioButton2.Location = new Point(6, 19);
      this.radioButton2.Name = "radioButton2";
      this.radioButton2.Size = new Size(66, 17);
      this.radioButton2.TabIndex = 1;
      this.radioButton2.TabStop = true;
      this.radioButton2.Text = "None (1)";
      this.radioButton2.UseVisualStyleBackColor = true;
      this.radioButton2.CheckedChanged += new EventHandler(this.radioButton2_CheckedChanged);
      this.radioButton1.AutoSize = true;
      this.radioButton1.Location = new Point(6, 42);
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.Size = new Size(169, 17);
      this.radioButton1.TabIndex = 0;
      this.radioButton1.Text = "MKDS Scale (1 / 16 = 0.0625)";
      this.radioButton1.UseVisualStyleBackColor = true;
      this.radioButton1.CheckedChanged += new EventHandler(this.radioButton2_CheckedChanged);
      this.button1.Location = new Point(162, 161);
      this.button1.Name = "button1";
      this.button1.Size = new Size(75, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.groupBox2.Controls.Add((Control) this.checkBox1);
      this.groupBox2.Location = new Point(12, 113);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(225, 42);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Misc";
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new Point(6, 19);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(96, 17);
      this.checkBox1.TabIndex = 0;
      this.checkBox1.Text = "Create NSBTX";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.button2.Enabled = false;
      this.button2.Location = new Point(12, 161);
      this.button2.Name = "button2";
      this.button2.Size = new Size(75, 23);
      this.button2.TabIndex = 3;
      this.button2.Text = "Advanced";
      this.button2.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(249, 196);
      this.ControlBox = false;
      this.Controls.Add((Control) this.button2);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.groupBox1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (NSBMDSettings);
      this.Text = "NSBMD Settings";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.numericUpDown1.EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);
    }

    public NSBMDSettings()
    {
      this.InitializeComponent();
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked)
      {
        this.Scale = 1f;
        this.numericUpDown1.Enabled = false;
      }
      else if (this.radioButton1.Checked)
      {
        this.Scale = 1f / 16f;
        this.numericUpDown1.Enabled = false;
      }
      else
      {
        this.Scale = (float) this.numericUpDown1.Value;
        this.numericUpDown1.Enabled = true;
      }
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
      if (!this.radioButton3.Checked)
        return;
      this.Scale = (float) this.numericUpDown1.Value;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      this.CreateNSBTX = this.checkBox1.Checked;
    }
  }
}
