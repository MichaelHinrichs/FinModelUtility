// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NCLR
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class NCLR : Form
  {
    private IContainer components = (IContainer) null;
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private NCLREditor nclrEditor1;
    private Panel panel1;
    private NumericUpDown numericUpDown3;
    private Label label3;
    private TrackBar trackBar3;
    private NumericUpDown numericUpDown2;
    private Label label2;
    private TrackBar trackBar2;
    private NumericUpDown numericUpDown1;
    private Label label1;
    private Panel panel2;
    private TrackBar trackBar1;
    private ToolStripButton toolStripButton1;
    private GroupBox groupBox1;
    private Label label6;
    private TrackBar trackBar6;
    private Label label5;
    private TrackBar trackBar5;
    private Label label4;
    private TrackBar trackBar4;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR file;
    private int oldhue;
    private int oldsaturation;
    private int oldluminosity;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (NCLR));
      this.splitContainer1 = new SplitContainer();
      this.panel1 = new Panel();
      this.groupBox1 = new GroupBox();
      this.label6 = new Label();
      this.trackBar6 = new TrackBar();
      this.label5 = new Label();
      this.trackBar5 = new TrackBar();
      this.label4 = new Label();
      this.trackBar4 = new TrackBar();
      this.numericUpDown3 = new NumericUpDown();
      this.label3 = new Label();
      this.trackBar3 = new TrackBar();
      this.numericUpDown2 = new NumericUpDown();
      this.label2 = new Label();
      this.trackBar2 = new TrackBar();
      this.numericUpDown1 = new NumericUpDown();
      this.label1 = new Label();
      this.panel2 = new Panel();
      this.trackBar1 = new TrackBar();
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.nclrEditor1 = new NCLREditor();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.trackBar6.BeginInit();
      this.trackBar5.BeginInit();
      this.trackBar4.BeginInit();
      this.numericUpDown3.BeginInit();
      this.trackBar3.BeginInit();
      this.numericUpDown2.BeginInit();
      this.trackBar2.BeginInit();
      this.numericUpDown1.BeginInit();
      this.trackBar1.BeginInit();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel2;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.nclrEditor1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.panel1);
      this.splitContainer1.Panel2MinSize = 262;
      this.splitContainer1.Size = new Size(665, 344);
      this.splitContainer1.SplitterDistance = 399;
      this.splitContainer1.TabIndex = 1;
      this.panel1.Controls.Add((Control) this.groupBox1);
      this.panel1.Controls.Add((Control) this.numericUpDown3);
      this.panel1.Controls.Add((Control) this.label3);
      this.panel1.Controls.Add((Control) this.trackBar3);
      this.panel1.Controls.Add((Control) this.numericUpDown2);
      this.panel1.Controls.Add((Control) this.label2);
      this.panel1.Controls.Add((Control) this.trackBar2);
      this.panel1.Controls.Add((Control) this.numericUpDown1);
      this.panel1.Controls.Add((Control) this.label1);
      this.panel1.Controls.Add((Control) this.panel2);
      this.panel1.Controls.Add((Control) this.trackBar1);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(262, 344);
      this.panel1.TabIndex = 0;
      this.groupBox1.Controls.Add((Control) this.label6);
      this.groupBox1.Controls.Add((Control) this.trackBar6);
      this.groupBox1.Controls.Add((Control) this.label5);
      this.groupBox1.Controls.Add((Control) this.trackBar5);
      this.groupBox1.Controls.Add((Control) this.label4);
      this.groupBox1.Controls.Add((Control) this.trackBar4);
      this.groupBox1.Location = new Point(3, 104);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(256, 237);
      this.groupBox1.TabIndex = 11;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "All Colors";
      this.label6.AutoSize = true;
      this.label6.Location = new Point(6, 134);
      this.label6.Name = "label6";
      this.label6.Size = new Size(59, 13);
      this.label6.TabIndex = 15;
      this.label6.Text = "Luminosity:";
      this.trackBar6.Location = new Point(71, 124);
      this.trackBar6.Maximum = 240;
      this.trackBar6.Minimum = -240;
      this.trackBar6.Name = "trackBar6";
      this.trackBar6.Size = new Size(176, 45);
      this.trackBar6.TabIndex = 14;
      this.trackBar6.TickFrequency = 24;
      this.trackBar6.Scroll += new EventHandler(this.trackBar6_Scroll);
      this.label5.AutoSize = true;
      this.label5.Location = new Point(7, 83);
      this.label5.Name = "label5";
      this.label5.Size = new Size(58, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "Saturation:";
      this.trackBar5.Location = new Point(71, 73);
      this.trackBar5.Maximum = 240;
      this.trackBar5.Minimum = -240;
      this.trackBar5.Name = "trackBar5";
      this.trackBar5.Size = new Size(176, 45);
      this.trackBar5.TabIndex = 12;
      this.trackBar5.TickFrequency = 24;
      this.trackBar5.Scroll += new EventHandler(this.trackBar5_Scroll);
      this.label4.AutoSize = true;
      this.label4.Location = new Point(35, 32);
      this.label4.Name = "label4";
      this.label4.Size = new Size(30, 13);
      this.label4.TabIndex = 11;
      this.label4.Text = "Hue:";
      this.trackBar4.Location = new Point(71, 22);
      this.trackBar4.Maximum = 240;
      this.trackBar4.Minimum = -240;
      this.trackBar4.Name = "trackBar4";
      this.trackBar4.Size = new Size(176, 45);
      this.trackBar4.TabIndex = 10;
      this.trackBar4.TickFrequency = 24;
      this.trackBar4.Scroll += new EventHandler(this.trackBar4_Scroll);
      this.numericUpDown3.Enabled = false;
      this.numericUpDown3.Location = new Point(81, 78);
      this.numericUpDown3.Maximum = new Decimal(new int[4]
      {
        (int) byte.MaxValue,
        0,
        0,
        0
      });
      this.numericUpDown3.Name = "numericUpDown3";
      this.numericUpDown3.Size = new Size(44, 20);
      this.numericUpDown3.TabIndex = 9;
      this.numericUpDown3.ValueChanged += new EventHandler(this.numericUpDown3_ValueChanged);
      this.label3.AutoSize = true;
      this.label3.Location = new Point(57, 80);
      this.label3.Name = "label3";
      this.label3.Size = new Size(17, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "B:";
      this.trackBar3.Enabled = false;
      this.trackBar3.Location = new Point(131, 75);
      this.trackBar3.Maximum = (int) byte.MaxValue;
      this.trackBar3.MaximumSize = new Size(125, 30);
      this.trackBar3.MinimumSize = new Size(125, 30);
      this.trackBar3.Name = "trackBar3";
      this.trackBar3.Size = new Size(125, 45);
      this.trackBar3.TabIndex = 7;
      this.trackBar3.TickFrequency = 16;
      this.trackBar3.Scroll += new EventHandler(this.trackBar3_Scroll);
      this.numericUpDown2.Enabled = false;
      this.numericUpDown2.Location = new Point(81, 42);
      this.numericUpDown2.Maximum = new Decimal(new int[4]
      {
        (int) byte.MaxValue,
        0,
        0,
        0
      });
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new Size(44, 20);
      this.numericUpDown2.TabIndex = 6;
      this.numericUpDown2.ValueChanged += new EventHandler(this.numericUpDown2_ValueChanged);
      this.label2.AutoSize = true;
      this.label2.Location = new Point(57, 44);
      this.label2.Name = "label2";
      this.label2.Size = new Size(18, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "G:";
      this.trackBar2.Enabled = false;
      this.trackBar2.Location = new Point(131, 39);
      this.trackBar2.Maximum = (int) byte.MaxValue;
      this.trackBar2.MaximumSize = new Size(125, 30);
      this.trackBar2.MinimumSize = new Size(125, 30);
      this.trackBar2.Name = "trackBar2";
      this.trackBar2.Size = new Size(125, 45);
      this.trackBar2.TabIndex = 4;
      this.trackBar2.TickFrequency = 16;
      this.trackBar2.Scroll += new EventHandler(this.trackBar2_Scroll);
      this.numericUpDown1.Enabled = false;
      this.numericUpDown1.Location = new Point(81, 6);
      this.numericUpDown1.Maximum = new Decimal(new int[4]
      {
        (int) byte.MaxValue,
        0,
        0,
        0
      });
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new Size(44, 20);
      this.numericUpDown1.TabIndex = 3;
      this.numericUpDown1.ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
      this.label1.AutoSize = true;
      this.label1.Location = new Point(57, 8);
      this.label1.Name = "label1";
      this.label1.Size = new Size(18, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "R:";
      this.panel2.BackColor = Color.Black;
      this.panel2.BorderStyle = BorderStyle.FixedSingle;
      this.panel2.Location = new Point(3, 3);
      this.panel2.Name = "panel2";
      this.panel2.Size = new Size(48, 48);
      this.panel2.TabIndex = 1;
      this.trackBar1.Enabled = false;
      this.trackBar1.Location = new Point(131, 3);
      this.trackBar1.Maximum = (int) byte.MaxValue;
      this.trackBar1.MaximumSize = new Size(125, 30);
      this.trackBar1.MinimumSize = new Size(125, 30);
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new Size(125, 45);
      this.trackBar1.TabIndex = 0;
      this.trackBar1.TickFrequency = 16;
      this.trackBar1.Scroll += new EventHandler(this.trackBar1_Scroll);
      this.toolStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripButton1
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(665, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.nclrEditor1.BackColor = SystemColors.Control;
      this.nclrEditor1.BorderStyle = BorderStyle.FixedSingle;
      this.nclrEditor1.Colors = new Color[0];
      this.nclrEditor1.Dock = DockStyle.Fill;
      this.nclrEditor1.Location = new Point(0, 0);
      this.nclrEditor1.Name = "nclrEditor1";
      this.nclrEditor1.Size = new Size(399, 344);
      this.nclrEditor1.TabIndex = 0;
      this.nclrEditor1.Use16ColorStyle = true;
      this.nclrEditor1.OnSelectedColorChanged += new NCLREditor.SelectedColorChanged(this.nclrEditor1_OnSelectedColorChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(665, 369);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (NCLR);
      this.Text = nameof (NCLR);
      this.Load += new EventHandler(this.NCLR_Load);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.trackBar6.EndInit();
      this.trackBar5.EndInit();
      this.trackBar4.EndInit();
      this.numericUpDown3.EndInit();
      this.trackBar3.EndInit();
      this.numericUpDown2.EndInit();
      this.trackBar2.EndInit();
      this.numericUpDown1.EndInit();
      this.trackBar1.EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public NCLR(MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR NCLR)
    {
      this.file = NCLR;
      this.InitializeComponent();
      this.toolStripButton1.Text = LanguageHandler.GetString("base.save");
      this.label4.Text = LanguageHandler.GetString("color.hue");
      this.label5.Text = LanguageHandler.GetString("color.saturation");
      this.label6.Text = LanguageHandler.GetString("color.luminosity");
      this.groupBox1.Text = LanguageHandler.GetString("color.allcolors");
    }

    private void NCLR_Load(object sender, EventArgs e)
    {
      this.nclrEditor1.Use16ColorStyle = this.file.PaletteData.fmt == Graphic.GXTexFmt.GX_TEXFMT_PLTT16;
      this.nclrEditor1.Colors = this.file.PaletteData.ToColorArray();
    }

    private void nclrEditor1_OnSelectedColorChanged(Color c)
    {
      this.numericUpDown1.Enabled = true;
      this.numericUpDown2.Enabled = true;
      this.numericUpDown3.Enabled = true;
      this.trackBar1.Enabled = true;
      this.trackBar2.Enabled = true;
      this.trackBar3.Enabled = true;
      this.numericUpDown1.Value = (Decimal) c.R;
      this.numericUpDown2.Value = (Decimal) c.G;
      this.numericUpDown3.Value = (Decimal) c.B;
      this.panel2.BackColor = c;
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
      this.trackBar1.Value = (int) this.numericUpDown1.Value;
      this.panel2.BackColor = this.nclrEditor1.SelectedColor = Color.FromArgb((int) this.numericUpDown1.Value, (int) this.numericUpDown2.Value, (int) this.numericUpDown3.Value);
    }

    private void numericUpDown2_ValueChanged(object sender, EventArgs e)
    {
      this.trackBar2.Value = (int) this.numericUpDown2.Value;
      this.panel2.BackColor = this.nclrEditor1.SelectedColor = Color.FromArgb((int) this.numericUpDown1.Value, (int) this.numericUpDown2.Value, (int) this.numericUpDown3.Value);
    }

    private void numericUpDown3_ValueChanged(object sender, EventArgs e)
    {
      this.trackBar3.Value = (int) this.numericUpDown3.Value;
      this.panel2.BackColor = this.nclrEditor1.SelectedColor = Color.FromArgb((int) this.numericUpDown1.Value, (int) this.numericUpDown2.Value, (int) this.numericUpDown3.Value);
    }

    private void trackBar1_Scroll(object sender, EventArgs e)
    {
      this.numericUpDown1.Value = (Decimal) this.trackBar1.Value;
      this.panel2.BackColor = this.nclrEditor1.SelectedColor = Color.FromArgb((int) this.numericUpDown1.Value, (int) this.numericUpDown2.Value, (int) this.numericUpDown3.Value);
    }

    private void trackBar2_Scroll(object sender, EventArgs e)
    {
      this.numericUpDown2.Value = (Decimal) this.trackBar2.Value;
      this.panel2.BackColor = this.nclrEditor1.SelectedColor = Color.FromArgb((int) this.numericUpDown1.Value, (int) this.numericUpDown2.Value, (int) this.numericUpDown3.Value);
    }

    private void trackBar3_Scroll(object sender, EventArgs e)
    {
      this.numericUpDown3.Value = (Decimal) this.trackBar3.Value;
      this.panel2.BackColor = this.nclrEditor1.SelectedColor = Color.FromArgb((int) this.numericUpDown1.Value, (int) this.numericUpDown2.Value, (int) this.numericUpDown3.Value);
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      this.file.PaletteData.Data = Graphic.ToABGR1555(this.nclrEditor1.Colors);
      FileHandler.Save(this.file.Write(), 0, false);
    }

    private void trackBar4_Scroll(object sender, EventArgs e)
    {
      float num1 = (float) this.trackBar4.Value / 240f;
      float num2 = (float) this.trackBar5.Value / 240f;
      float num3 = (float) this.trackBar6.Value / 240f;
      Color[] colorArray = this.file.PaletteData.ToColorArray();
      for (int index = 0; index < colorArray.Length; ++index)
      {
        NCLR.HSLColor hslColor = (NCLR.HSLColor) colorArray[index];
        hslColor.Hue += hslColor.Hue * (double) num1;
        hslColor.Saturation += hslColor.Saturation * (double) num2;
        hslColor.Luminosity += hslColor.Luminosity * (double) num3;
        colorArray[index] = (Color) hslColor;
      }
      this.nclrEditor1.Colors = colorArray;
    }

    private void trackBar5_Scroll(object sender, EventArgs e)
    {
      float num1 = (float) this.trackBar4.Value / 240f;
      float num2 = (float) this.trackBar5.Value / 240f;
      float num3 = (float) this.trackBar6.Value / 240f;
      Color[] colorArray = this.file.PaletteData.ToColorArray();
      for (int index = 0; index < colorArray.Length; ++index)
      {
        NCLR.HSLColor hslColor = (NCLR.HSLColor) colorArray[index];
        hslColor.Hue += hslColor.Hue * (double) num1;
        hslColor.Saturation += hslColor.Saturation * (double) num2;
        hslColor.Luminosity += hslColor.Luminosity * (double) num3;
        colorArray[index] = (Color) hslColor;
      }
      this.nclrEditor1.Colors = colorArray;
    }

    private void trackBar6_Scroll(object sender, EventArgs e)
    {
      float num1 = (float) this.trackBar4.Value / 240f;
      float num2 = (float) this.trackBar5.Value / 240f;
      float num3 = (float) this.trackBar6.Value / 240f;
      Color[] colorArray = this.file.PaletteData.ToColorArray();
      for (int index = 0; index < colorArray.Length; ++index)
      {
        NCLR.HSLColor hslColor = (NCLR.HSLColor) colorArray[index];
        hslColor.Hue += hslColor.Hue * (double) num1;
        hslColor.Saturation += hslColor.Saturation * (double) num2;
        hslColor.Luminosity += hslColor.Luminosity * (double) num3;
        colorArray[index] = (Color) hslColor;
      }
      this.nclrEditor1.Colors = colorArray;
    }

    private class HSLColor
    {
      private double hue = 1.0;
      private double saturation = 1.0;
      private double luminosity = 1.0;
      private const double scale = 240.0;

      public double Hue
      {
        get
        {
          return this.hue * 240.0;
        }
        set
        {
          this.hue = this.CheckRange(value / 240.0);
        }
      }

      public double Saturation
      {
        get
        {
          return this.saturation * 240.0;
        }
        set
        {
          this.saturation = this.CheckRange(value / 240.0);
        }
      }

      public double Luminosity
      {
        get
        {
          return this.luminosity * 240.0;
        }
        set
        {
          this.luminosity = this.CheckRange(value / 240.0);
        }
      }

      private double CheckRange(double value)
      {
        if (value < 0.0)
          value = 0.0;
        else if (value > 1.0)
          value = 1.0;
        return value;
      }

      public override string ToString()
      {
        return string.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", (object) this.Hue, (object) this.Saturation, (object) this.Luminosity);
      }

      public string ToRGBString()
      {
        Color color = (Color) this;
        return string.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", (object) color.R, (object) color.G, (object) color.B);
      }

      public static implicit operator Color(NCLR.HSLColor hslColor)
      {
        double num1 = 0.0;
        double num2 = 0.0;
        double num3 = 0.0;
        if (hslColor.luminosity != 0.0)
        {
          if (hslColor.saturation == 0.0)
          {
            double luminosity;
            num3 = luminosity = hslColor.luminosity;
            num2 = luminosity;
            num1 = luminosity;
          }
          else
          {
            double temp2 = NCLR.HSLColor.GetTemp2(hslColor);
            double temp1 = 2.0 * hslColor.luminosity - temp2;
            num1 = NCLR.HSLColor.GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
            num2 = NCLR.HSLColor.GetColorComponent(temp1, temp2, hslColor.hue);
            num3 = NCLR.HSLColor.GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
          }
        }
        return Color.FromArgb((int) ((double) byte.MaxValue * num1), (int) ((double) byte.MaxValue * num2), (int) ((double) byte.MaxValue * num3));
      }

      private static double GetColorComponent(double temp1, double temp2, double temp3)
      {
        temp3 = NCLR.HSLColor.MoveIntoRange(temp3);
        if (temp3 < 1.0 / 6.0)
          return temp1 + (temp2 - temp1) * 6.0 * temp3;
        if (temp3 < 0.5)
          return temp2;
        return temp3 < 2.0 / 3.0 ? temp1 + (temp2 - temp1) * (2.0 / 3.0 - temp3) * 6.0 : temp1;
      }

      private static double MoveIntoRange(double temp3)
      {
        if (temp3 < 0.0)
          ++temp3;
        else if (temp3 > 1.0)
          --temp3;
        return temp3;
      }

      private static double GetTemp2(NCLR.HSLColor hslColor)
      {
        return hslColor.luminosity >= 0.5 ? hslColor.luminosity + hslColor.saturation - hslColor.luminosity * hslColor.saturation : hslColor.luminosity * (1.0 + hslColor.saturation);
      }

      public static implicit operator NCLR.HSLColor(Color color)
      {
        return new NCLR.HSLColor()
        {
          hue = (double) color.GetHue() / 360.0,
          luminosity = (double) color.GetBrightness(),
          saturation = (double) color.GetSaturation()
        };
      }

      public void SetRGB(int red, int green, int blue)
      {
        NCLR.HSLColor hslColor = (NCLR.HSLColor) Color.FromArgb(red, green, blue);
        this.hue = hslColor.hue;
        this.saturation = hslColor.saturation;
        this.luminosity = hslColor.luminosity;
      }

      public HSLColor()
      {
      }

      public HSLColor(Color color)
      {
        this.SetRGB((int) color.R, (int) color.G, (int) color.B);
      }

      public HSLColor(int red, int green, int blue)
      {
        this.SetRGB(red, green, blue);
      }

      public HSLColor(double hue, double saturation, double luminosity)
      {
        this.Hue = hue;
        this.Saturation = saturation;
        this.Luminosity = luminosity;
      }
    }
  }
}
