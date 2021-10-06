// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.G3DCVTR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class G3DCVTR : Form
  {
    private IContainer components = (IContainer) null;
    private string G3DCVTRPath;
    private GroupBox groupBox1;
    private Button button3;
    private TextBox textBox3;
    private TextBox textBox2;
    private CheckBox checkBox1;
    private Label label1;
    private Button button2;
    private GroupBox groupBox2;
    private GroupBox groupBox3;
    private Button button1;
    private TextBox textBox1;
    private CheckBox checkBox6;
    private CheckBox checkBox5;
    private CheckBox checkBox4;
    private CheckBox checkBox3;
    private CheckBox checkBox2;
    private System.Windows.Forms.OpenFileDialog openFileDialog1;
    private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private ToolTip toolTip1;
    private RadioButton radioButton3;
    private RadioButton radioButton2;
    private RadioButton radioButton1;
    private System.Windows.Forms.OpenFileDialog openFileDialog2;

    public G3DCVTR()
    {
      this.InitializeComponent();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      this.textBox2.Text = this.openFileDialog1.FileName;
      if (Path.GetExtension(this.openFileDialog1.FileName).StartsWith(".nsb"))
      {
        this.checkBox1.Checked = false;
        this.checkBox1.Enabled = false;
      }
      else
        this.checkBox1.Enabled = true;
      switch (Path.GetExtension(this.openFileDialog1.FileName))
      {
        case ".imd":
          this.toolTip1.RemoveAll();
          this.radioButton1.Text = "etex";
          this.toolTip1.SetToolTip((Control) this.radioButton1, "Output only texture data(.nsbtx)");
          this.radioButton2.Text = "emdl";
          this.toolTip1.SetToolTip((Control) this.radioButton2, "Output only model structures(.nsbmd)");
          this.radioButton3.Text = "eboth";
          this.radioButton3.Checked = true;
          this.toolTip1.SetToolTip((Control) this.radioButton3, "Output model structures and texture data(default, .nsbmd)");
          this.checkBox5.Text = "s";
          this.toolTip1.SetToolTip((Control) this.checkBox5, "Store the all matrices on the matrix stack");
          this.checkBox6.Text = "texsrt";
          this.toolTip1.SetToolTip((Control) this.checkBox6, "Always output data field for a texture matrix");
          this.radioButton1.Visible = true;
          this.radioButton2.Visible = true;
          this.radioButton3.Visible = true;
          this.checkBox2.Visible = false;
          this.checkBox3.Visible = false;
          this.checkBox4.Visible = false;
          this.checkBox5.Visible = true;
          this.checkBox6.Visible = true;
          break;
        case ".ica":
          this.toolTip1.RemoveAll();
          this.checkBox2.Text = "OT";
          this.toolTip1.SetToolTip((Control) this.checkBox2, "Omit translation data");
          this.checkBox3.Text = "OS";
          this.toolTip1.SetToolTip((Control) this.checkBox3, "Omit scaling data");
          this.checkBox4.Text = "OR";
          this.toolTip1.SetToolTip((Control) this.checkBox4, "Omit rotation data");
          this.radioButton1.Visible = false;
          this.radioButton2.Visible = false;
          this.radioButton3.Visible = false;
          this.checkBox2.Visible = true;
          this.checkBox3.Visible = true;
          this.checkBox4.Visible = true;
          this.checkBox5.Visible = false;
          this.checkBox6.Visible = false;
          break;
        default:
          this.radioButton1.Visible = false;
          this.radioButton2.Visible = false;
          this.radioButton3.Visible = false;
          this.checkBox2.Visible = false;
          this.checkBox3.Visible = false;
          this.checkBox4.Visible = false;
          this.checkBox5.Visible = false;
          this.checkBox6.Visible = false;
          break;
      }
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkBox1.Checked)
      {
        this.textBox3.Enabled = true;
        this.button3.Enabled = true;
      }
      else
      {
        this.textBox3.Enabled = false;
        this.button3.Enabled = false;
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Process process = new Process();
      process.StartInfo.FileName = this.G3DCVTRPath;
      process.StartInfo.Arguments = "\"" + this.openFileDialog1.FileName + "\"";
      if (this.checkBox1.Checked)
      {
        ProcessStartInfo startInfo = process.StartInfo;
        startInfo.Arguments = startInfo.Arguments + " -o \"" + this.saveFileDialog1.FileName + "\"";
      }
      switch (Path.GetExtension(this.openFileDialog1.FileName))
      {
        case ".imd":
          if (this.radioButton1.Checked)
            process.StartInfo.Arguments += " -etex";
          else if (this.radioButton2.Checked)
            process.StartInfo.Arguments += " -emdl";
          else
            process.StartInfo.Arguments += " -eboth";
          if (this.checkBox5.Checked)
            process.StartInfo.Arguments += " -s";
          if (this.checkBox6.Checked)
          {
            process.StartInfo.Arguments += " -texsrt";
            break;
          }
          break;
        case ".ica":
          if (this.checkBox2.Checked)
            process.StartInfo.Arguments += " -OT";
          if (this.checkBox3.Checked)
            process.StartInfo.Arguments += " -OS";
          if (this.checkBox4.Checked)
          {
            process.StartInfo.Arguments += " -OR";
            break;
          }
          break;
      }
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.openFileDialog1.FileName);
      process.Start();
      this.textBox1.Text = process.StandardOutput.ReadToEnd();
      process.WaitForExit();
    }

    private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      this.textBox1.Text += e.Data;
    }

    private void G3DCVTR_Load(object sender, EventArgs e)
    {
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("MKDS Course Modifier", true);
      if (!((IEnumerable<string>) registryKey.GetValueNames()).Contains<string>("G3DCVTRPath"))
      {
        if (this.openFileDialog2.ShowDialog() == DialogResult.OK && this.openFileDialog2.FileName.Length > 0)
        {
          registryKey.SetValue("G3DCVTRPath", (object) this.openFileDialog2.FileName);
          this.G3DCVTRPath = this.openFileDialog2.FileName;
        }
        else
          this.Close();
      }
      else
        this.G3DCVTRPath = (string) registryKey.GetValue("G3DCVTRPath");
    }

    private void button3_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      this.textBox3.Text = this.saveFileDialog1.FileName;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (G3DCVTR));
      this.groupBox1 = new GroupBox();
      this.button3 = new Button();
      this.textBox3 = new TextBox();
      this.textBox2 = new TextBox();
      this.checkBox1 = new CheckBox();
      this.label1 = new Label();
      this.button2 = new Button();
      this.groupBox2 = new GroupBox();
      this.radioButton3 = new RadioButton();
      this.radioButton2 = new RadioButton();
      this.radioButton1 = new RadioButton();
      this.checkBox6 = new CheckBox();
      this.checkBox5 = new CheckBox();
      this.checkBox4 = new CheckBox();
      this.checkBox3 = new CheckBox();
      this.checkBox2 = new CheckBox();
      this.groupBox3 = new GroupBox();
      this.button1 = new Button();
      this.textBox1 = new TextBox();
      this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
      this.toolTip1 = new ToolTip(this.components);
      this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      this.groupBox1.Controls.Add((Control) this.button3);
      this.groupBox1.Controls.Add((Control) this.textBox3);
      this.groupBox1.Controls.Add((Control) this.textBox2);
      this.groupBox1.Controls.Add((Control) this.checkBox1);
      this.groupBox1.Controls.Add((Control) this.label1);
      this.groupBox1.Controls.Add((Control) this.button2);
      this.groupBox1.Location = new Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(460, 71);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Files";
      this.button3.Enabled = false;
      this.button3.FlatStyle = FlatStyle.System;
      this.button3.Location = new Point(433, 44);
      this.button3.Name = "button3";
      this.button3.Size = new Size(21, 20);
      this.button3.TabIndex = 5;
      this.button3.Text = "...";
      this.button3.TextAlign = ContentAlignment.BottomCenter;
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new EventHandler(this.button3_Click);
      this.textBox3.Enabled = false;
      this.textBox3.Location = new Point(67, 45);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new Size(360, 20);
      this.textBox3.TabIndex = 4;
      this.textBox2.Location = new Point(67, 19);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new Size(360, 20);
      this.textBox2.TabIndex = 3;
      this.checkBox1.AutoSize = true;
      this.checkBox1.Enabled = false;
      this.checkBox1.Location = new Point(6, 47);
      this.checkBox1.Margin = new Padding(3, 0, 0, 0);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(58, 17);
      this.checkBox1.TabIndex = 2;
      this.checkBox1.Text = "Output";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.label1.AutoSize = true;
      this.label1.Location = new Point(30, 24);
      this.label1.Name = "label1";
      this.label1.Size = new Size(31, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Input";
      this.button2.FlatStyle = FlatStyle.System;
      this.button2.Location = new Point(433, 19);
      this.button2.Name = "button2";
      this.button2.Size = new Size(21, 20);
      this.button2.TabIndex = 0;
      this.button2.Text = "...";
      this.button2.TextAlign = ContentAlignment.BottomCenter;
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.groupBox2.Controls.Add((Control) this.radioButton3);
      this.groupBox2.Controls.Add((Control) this.radioButton2);
      this.groupBox2.Controls.Add((Control) this.radioButton1);
      this.groupBox2.Controls.Add((Control) this.checkBox6);
      this.groupBox2.Controls.Add((Control) this.checkBox5);
      this.groupBox2.Controls.Add((Control) this.checkBox4);
      this.groupBox2.Controls.Add((Control) this.checkBox3);
      this.groupBox2.Controls.Add((Control) this.checkBox2);
      this.groupBox2.Location = new Point(12, 89);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(174, 243);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Options";
      this.radioButton3.AutoSize = true;
      this.radioButton3.Location = new Point(6, 65);
      this.radioButton3.Name = "radioButton3";
      this.radioButton3.Size = new Size(85, 17);
      this.radioButton3.TabIndex = 8;
      this.radioButton3.TabStop = true;
      this.radioButton3.Text = "radioButton3";
      this.radioButton3.UseVisualStyleBackColor = true;
      this.radioButton3.Visible = false;
      this.radioButton2.AutoSize = true;
      this.radioButton2.Location = new Point(6, 42);
      this.radioButton2.Name = "radioButton2";
      this.radioButton2.Size = new Size(85, 17);
      this.radioButton2.TabIndex = 7;
      this.radioButton2.TabStop = true;
      this.radioButton2.Text = "radioButton2";
      this.radioButton2.UseVisualStyleBackColor = true;
      this.radioButton2.Visible = false;
      this.radioButton1.AutoSize = true;
      this.radioButton1.Location = new Point(6, 19);
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.Size = new Size(85, 17);
      this.radioButton1.TabIndex = 6;
      this.radioButton1.TabStop = true;
      this.radioButton1.Text = "radioButton1";
      this.radioButton1.UseVisualStyleBackColor = true;
      this.radioButton1.Visible = false;
      this.checkBox6.AutoSize = true;
      this.checkBox6.Location = new Point(6, 113);
      this.checkBox6.Name = "checkBox6";
      this.checkBox6.Size = new Size(80, 17);
      this.checkBox6.TabIndex = 4;
      this.checkBox6.Text = "checkBox6";
      this.checkBox6.UseVisualStyleBackColor = true;
      this.checkBox6.Visible = false;
      this.checkBox5.AutoSize = true;
      this.checkBox5.Location = new Point(6, 90);
      this.checkBox5.Name = "checkBox5";
      this.checkBox5.Size = new Size(80, 17);
      this.checkBox5.TabIndex = 3;
      this.checkBox5.Text = "checkBox5";
      this.checkBox5.UseVisualStyleBackColor = true;
      this.checkBox5.Visible = false;
      this.checkBox4.AutoSize = true;
      this.checkBox4.Location = new Point(6, 67);
      this.checkBox4.Name = "checkBox4";
      this.checkBox4.Size = new Size(80, 17);
      this.checkBox4.TabIndex = 2;
      this.checkBox4.Text = "checkBox4";
      this.checkBox4.UseVisualStyleBackColor = true;
      this.checkBox4.Visible = false;
      this.checkBox3.AutoSize = true;
      this.checkBox3.Location = new Point(6, 44);
      this.checkBox3.Name = "checkBox3";
      this.checkBox3.Size = new Size(80, 17);
      this.checkBox3.TabIndex = 1;
      this.checkBox3.Text = "checkBox3";
      this.checkBox3.UseVisualStyleBackColor = true;
      this.checkBox3.Visible = false;
      this.checkBox2.AutoSize = true;
      this.checkBox2.Location = new Point(6, 21);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new Size(80, 17);
      this.checkBox2.TabIndex = 0;
      this.checkBox2.Text = "checkBox2";
      this.checkBox2.UseVisualStyleBackColor = true;
      this.checkBox2.Visible = false;
      this.groupBox3.Controls.Add((Control) this.button1);
      this.groupBox3.Controls.Add((Control) this.textBox1);
      this.groupBox3.Location = new Point(192, 89);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new Size(280, 243);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Output";
      this.button1.Location = new Point(6, 214);
      this.button1.Name = "button1";
      this.button1.Size = new Size(268, 23);
      this.button1.TabIndex = 1;
      this.button1.Text = "Convert / Analyze";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.textBox1.Location = new Point(6, 19);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ReadOnly = true;
      this.textBox1.ScrollBars = ScrollBars.Vertical;
      this.textBox1.Size = new Size(268, 189);
      this.textBox1.TabIndex = 0;
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = componentResourceManager.GetString("openFileDialog1.Filter");
      this.openFileDialog2.FileName = "openFileDialog2";
      this.openFileDialog2.Filter = "(G3DCVTR.exe)|G3DCVTR.exe";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(484, 344);
      this.Controls.Add((Control) this.groupBox3);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.Name = nameof (G3DCVTR);
      this.Text = nameof (G3DCVTR);
      this.Load += new EventHandler(this.G3DCVTR_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
