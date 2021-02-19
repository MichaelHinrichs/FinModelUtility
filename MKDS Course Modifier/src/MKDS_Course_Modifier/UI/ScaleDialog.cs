// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.ScaleDialog
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class ScaleDialog : Form
  {
    private IContainer components = (IContainer) null;
    public float scale = 1f;
    private Label label1;
    private NumericUpDown numericUpDown1;
    private Button button1;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.label1 = new Label();
      this.numericUpDown1 = new NumericUpDown();
      this.button1 = new Button();
      this.numericUpDown1.BeginInit();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Location = new Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new Size(179, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Type the scale you want (1 = 100%):";
      this.numericUpDown1.DecimalPlaces = 3;
      this.numericUpDown1.Location = new Point(12, 25);
      this.numericUpDown1.Minimum = new Decimal(new int[4]
      {
        1,
        0,
        0,
        196608
      });
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new Size(120, 20);
      this.numericUpDown1.TabIndex = 1;
      this.numericUpDown1.Value = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.button1.Location = new Point(138, 25);
      this.button1.Name = "button1";
      this.button1.Size = new Size(53, 20);
      this.button1.TabIndex = 2;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(203, 57);
      this.Controls.Add((Control) this.button1);
      this.Controls.Add((Control) this.numericUpDown1);
      this.Controls.Add((Control) this.label1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (ScaleDialog);
      this.Text = nameof (ScaleDialog);
      this.Load += new EventHandler(this.ScaleDialog_Load);
      this.numericUpDown1.EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public ScaleDialog()
    {
      this.InitializeComponent();
    }

    private void ScaleDialog_Load(object sender, EventArgs e)
    {
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.scale = (float) this.numericUpDown1.Value;
      this.Close();
    }
  }
}
