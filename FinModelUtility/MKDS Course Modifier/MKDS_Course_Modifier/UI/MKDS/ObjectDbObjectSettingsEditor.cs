// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.ObjectDbObjectSettingsEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.MKDS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class ObjectDbObjectSettingsEditor : Form
  {
    private IContainer components = (IContainer) null;
    private bool start = true;
    private TableLayoutPanel tableLayoutPanel1;
    private Label label32;
    private Label label31;
    private Label label30;
    private Label label29;
    private Label label28;
    private Label label27;
    private Label label26;
    private Label label25;
    private Label label24;
    private Label label23;
    private Label label22;
    private Label label21;
    private Label label20;
    private Label label19;
    private Label label18;
    private Label label17;
    private Label label16;
    private Label label15;
    private Label label14;
    private Label label13;
    private Label label12;
    private Label label11;
    private Label label10;
    private Label label9;
    private Label label8;
    private Label label7;
    private Label label6;
    private Label label5;
    private Label label4;
    private Label label3;
    private Label label2;
    private Label label1;
    private TableLayoutPanel tableLayoutPanel2;
    private Panel panel3;
    private CheckBox checkBox5;
    private CheckBox checkBox6;
    private Panel panel2;
    private CheckBox checkBox3;
    private CheckBox checkBox4;
    private RadioButton radioButton1;
    private TableLayoutPanel tableLayoutPanel3;
    private Panel panel1;
    private CheckBox checkBox2;
    private CheckBox checkBox1;
    private RadioButton radioButton2;
    private TextBox textBox2;
    private TextBox textBox1;
    private TextBox textBox3;
    public List<ObjectDb.Object.Setting.SettingData> Setting;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.label32 = new Label();
      this.label31 = new Label();
      this.label30 = new Label();
      this.label29 = new Label();
      this.label28 = new Label();
      this.label27 = new Label();
      this.label26 = new Label();
      this.label25 = new Label();
      this.label24 = new Label();
      this.label23 = new Label();
      this.label22 = new Label();
      this.label21 = new Label();
      this.label20 = new Label();
      this.label19 = new Label();
      this.label18 = new Label();
      this.label17 = new Label();
      this.label16 = new Label();
      this.label15 = new Label();
      this.label14 = new Label();
      this.label13 = new Label();
      this.label12 = new Label();
      this.label11 = new Label();
      this.label10 = new Label();
      this.label9 = new Label();
      this.label8 = new Label();
      this.label7 = new Label();
      this.label6 = new Label();
      this.label5 = new Label();
      this.label4 = new Label();
      this.label3 = new Label();
      this.label2 = new Label();
      this.label1 = new Label();
      this.tableLayoutPanel2 = new TableLayoutPanel();
      this.panel3 = new Panel();
      this.textBox2 = new TextBox();
      this.checkBox5 = new CheckBox();
      this.checkBox6 = new CheckBox();
      this.panel2 = new Panel();
      this.textBox1 = new TextBox();
      this.checkBox3 = new CheckBox();
      this.checkBox4 = new CheckBox();
      this.radioButton1 = new RadioButton();
      this.tableLayoutPanel3 = new TableLayoutPanel();
      this.panel1 = new Panel();
      this.textBox3 = new TextBox();
      this.checkBox2 = new CheckBox();
      this.checkBox1 = new CheckBox();
      this.radioButton2 = new RadioButton();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.panel3.SuspendLayout();
      this.panel2.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.tableLayoutPanel1.BackColor = SystemColors.InactiveBorder;
      this.tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
      this.tableLayoutPanel1.ColumnCount = 32;
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3.125f));
      this.tableLayoutPanel1.Controls.Add((Control) this.label32, 31, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label31, 30, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label30, 29, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label29, 28, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label28, 27, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label27, 26, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label26, 25, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label25, 24, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label24, 23, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label23, 22, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label22, 21, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label21, 20, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label20, 19, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label19, 18, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label18, 17, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label17, 16, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label16, 15, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label15, 14, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label14, 13, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label13, 12, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label12, 11, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label11, 10, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label10, 9, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label9, 8, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label8, 7, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label7, 6, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label6, 5, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label5, 4, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label4, 3, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label3, 2, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label2, 1, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label1, 0, 0);
      this.tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
      this.tableLayoutPanel1.Location = new Point(12, 12);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 1;
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel1.Size = new Size(450, 25);
      this.tableLayoutPanel1.TabIndex = 0;
      this.label32.Dock = DockStyle.Fill;
      this.label32.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label32.ImageAlign = ContentAlignment.BottomRight;
      this.label32.Location = new Point(435, 1);
      this.label32.Margin = new Padding(0);
      this.label32.Name = "label32";
      this.label32.Size = new Size(14, 23);
      this.label32.TabIndex = 32;
      this.label32.Text = "0";
      this.label31.Dock = DockStyle.Fill;
      this.label31.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label31.ImageAlign = ContentAlignment.BottomRight;
      this.label31.Location = new Point(421, 1);
      this.label31.Margin = new Padding(0);
      this.label31.Name = "label31";
      this.label31.Size = new Size(13, 23);
      this.label31.TabIndex = 31;
      this.label31.Text = "1";
      this.label30.Dock = DockStyle.Fill;
      this.label30.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label30.ImageAlign = ContentAlignment.BottomRight;
      this.label30.Location = new Point(407, 1);
      this.label30.Margin = new Padding(0);
      this.label30.Name = "label30";
      this.label30.Size = new Size(13, 23);
      this.label30.TabIndex = 30;
      this.label30.Text = "2";
      this.label29.Dock = DockStyle.Fill;
      this.label29.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label29.ImageAlign = ContentAlignment.BottomRight;
      this.label29.Location = new Point(393, 1);
      this.label29.Margin = new Padding(0);
      this.label29.Name = "label29";
      this.label29.Size = new Size(13, 23);
      this.label29.TabIndex = 29;
      this.label29.Text = "3";
      this.label28.Dock = DockStyle.Fill;
      this.label28.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label28.ImageAlign = ContentAlignment.BottomRight;
      this.label28.Location = new Point(379, 1);
      this.label28.Margin = new Padding(0);
      this.label28.Name = "label28";
      this.label28.Size = new Size(13, 23);
      this.label28.TabIndex = 28;
      this.label28.Text = "4";
      this.label27.Dock = DockStyle.Fill;
      this.label27.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label27.ImageAlign = ContentAlignment.BottomRight;
      this.label27.Location = new Point(365, 1);
      this.label27.Margin = new Padding(0);
      this.label27.Name = "label27";
      this.label27.Size = new Size(13, 23);
      this.label27.TabIndex = 27;
      this.label27.Text = "5";
      this.label26.Dock = DockStyle.Fill;
      this.label26.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label26.ImageAlign = ContentAlignment.BottomRight;
      this.label26.Location = new Point(351, 1);
      this.label26.Margin = new Padding(0);
      this.label26.Name = "label26";
      this.label26.Size = new Size(13, 23);
      this.label26.TabIndex = 26;
      this.label26.Text = "6";
      this.label25.Dock = DockStyle.Fill;
      this.label25.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label25.ImageAlign = ContentAlignment.BottomRight;
      this.label25.Location = new Point(337, 1);
      this.label25.Margin = new Padding(0);
      this.label25.Name = "label25";
      this.label25.Size = new Size(13, 23);
      this.label25.TabIndex = 25;
      this.label25.Text = "7";
      this.label24.Dock = DockStyle.Fill;
      this.label24.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label24.ImageAlign = ContentAlignment.BottomRight;
      this.label24.Location = new Point(323, 1);
      this.label24.Margin = new Padding(0);
      this.label24.Name = "label24";
      this.label24.Size = new Size(13, 23);
      this.label24.TabIndex = 24;
      this.label24.Text = "8";
      this.label23.Dock = DockStyle.Fill;
      this.label23.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label23.ImageAlign = ContentAlignment.BottomRight;
      this.label23.Location = new Point(309, 1);
      this.label23.Margin = new Padding(0);
      this.label23.Name = "label23";
      this.label23.Size = new Size(13, 23);
      this.label23.TabIndex = 23;
      this.label23.Text = "9";
      this.label22.Dock = DockStyle.Fill;
      this.label22.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label22.ImageAlign = ContentAlignment.BottomRight;
      this.label22.Location = new Point(295, 1);
      this.label22.Margin = new Padding(0);
      this.label22.Name = "label22";
      this.label22.Size = new Size(13, 23);
      this.label22.TabIndex = 22;
      this.label22.Text = "10";
      this.label21.Dock = DockStyle.Fill;
      this.label21.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label21.ImageAlign = ContentAlignment.BottomRight;
      this.label21.Location = new Point(281, 1);
      this.label21.Margin = new Padding(0);
      this.label21.Name = "label21";
      this.label21.Size = new Size(13, 23);
      this.label21.TabIndex = 21;
      this.label21.Text = "11";
      this.label20.Dock = DockStyle.Fill;
      this.label20.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label20.ImageAlign = ContentAlignment.BottomRight;
      this.label20.Location = new Point(267, 1);
      this.label20.Margin = new Padding(0);
      this.label20.Name = "label20";
      this.label20.Size = new Size(13, 23);
      this.label20.TabIndex = 20;
      this.label20.Text = "12";
      this.label19.Dock = DockStyle.Fill;
      this.label19.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label19.ImageAlign = ContentAlignment.BottomRight;
      this.label19.Location = new Point(253, 1);
      this.label19.Margin = new Padding(0);
      this.label19.Name = "label19";
      this.label19.Size = new Size(13, 23);
      this.label19.TabIndex = 19;
      this.label19.Text = "13";
      this.label18.Dock = DockStyle.Fill;
      this.label18.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label18.ImageAlign = ContentAlignment.BottomRight;
      this.label18.Location = new Point(239, 1);
      this.label18.Margin = new Padding(0);
      this.label18.Name = "label18";
      this.label18.Size = new Size(13, 23);
      this.label18.TabIndex = 18;
      this.label18.Text = "14";
      this.label17.Dock = DockStyle.Fill;
      this.label17.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label17.ImageAlign = ContentAlignment.BottomRight;
      this.label17.Location = new Point(225, 1);
      this.label17.Margin = new Padding(0);
      this.label17.Name = "label17";
      this.label17.Size = new Size(13, 23);
      this.label17.TabIndex = 17;
      this.label17.Text = "15";
      this.label16.Dock = DockStyle.Fill;
      this.label16.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label16.ImageAlign = ContentAlignment.BottomRight;
      this.label16.Location = new Point(211, 1);
      this.label16.Margin = new Padding(0);
      this.label16.Name = "label16";
      this.label16.Size = new Size(13, 23);
      this.label16.TabIndex = 16;
      this.label16.Text = "16";
      this.label15.Dock = DockStyle.Fill;
      this.label15.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label15.ImageAlign = ContentAlignment.BottomRight;
      this.label15.Location = new Point(197, 1);
      this.label15.Margin = new Padding(0);
      this.label15.Name = "label15";
      this.label15.Size = new Size(13, 23);
      this.label15.TabIndex = 15;
      this.label15.Text = "17";
      this.label14.Dock = DockStyle.Fill;
      this.label14.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label14.ImageAlign = ContentAlignment.BottomRight;
      this.label14.Location = new Point(183, 1);
      this.label14.Margin = new Padding(0);
      this.label14.Name = "label14";
      this.label14.Size = new Size(13, 23);
      this.label14.TabIndex = 14;
      this.label14.Text = "18";
      this.label13.Dock = DockStyle.Fill;
      this.label13.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label13.ImageAlign = ContentAlignment.BottomRight;
      this.label13.Location = new Point(169, 1);
      this.label13.Margin = new Padding(0);
      this.label13.Name = "label13";
      this.label13.Size = new Size(13, 23);
      this.label13.TabIndex = 13;
      this.label13.Text = "19";
      this.label12.Dock = DockStyle.Fill;
      this.label12.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label12.ImageAlign = ContentAlignment.BottomRight;
      this.label12.Location = new Point(155, 1);
      this.label12.Margin = new Padding(0);
      this.label12.Name = "label12";
      this.label12.Size = new Size(13, 23);
      this.label12.TabIndex = 12;
      this.label12.Text = "20";
      this.label11.Dock = DockStyle.Fill;
      this.label11.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label11.ImageAlign = ContentAlignment.BottomRight;
      this.label11.Location = new Point(141, 1);
      this.label11.Margin = new Padding(0);
      this.label11.Name = "label11";
      this.label11.Size = new Size(13, 23);
      this.label11.TabIndex = 11;
      this.label11.Text = "21";
      this.label10.Dock = DockStyle.Fill;
      this.label10.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label10.ImageAlign = ContentAlignment.BottomRight;
      this.label10.Location = new Point((int) sbyte.MaxValue, 1);
      this.label10.Margin = new Padding(0);
      this.label10.Name = "label10";
      this.label10.Size = new Size(13, 23);
      this.label10.TabIndex = 10;
      this.label10.Text = "22";
      this.label9.Dock = DockStyle.Fill;
      this.label9.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label9.ImageAlign = ContentAlignment.BottomRight;
      this.label9.Location = new Point(113, 1);
      this.label9.Margin = new Padding(0);
      this.label9.Name = "label9";
      this.label9.Size = new Size(13, 23);
      this.label9.TabIndex = 9;
      this.label9.Text = "23";
      this.label8.Dock = DockStyle.Fill;
      this.label8.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label8.ImageAlign = ContentAlignment.BottomRight;
      this.label8.Location = new Point(99, 1);
      this.label8.Margin = new Padding(0);
      this.label8.Name = "label8";
      this.label8.Size = new Size(13, 23);
      this.label8.TabIndex = 8;
      this.label8.Text = "24";
      this.label7.Dock = DockStyle.Fill;
      this.label7.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label7.ImageAlign = ContentAlignment.BottomRight;
      this.label7.Location = new Point(85, 1);
      this.label7.Margin = new Padding(0);
      this.label7.Name = "label7";
      this.label7.Size = new Size(13, 23);
      this.label7.TabIndex = 7;
      this.label7.Text = "25";
      this.label6.Dock = DockStyle.Fill;
      this.label6.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label6.ImageAlign = ContentAlignment.BottomRight;
      this.label6.Location = new Point(71, 1);
      this.label6.Margin = new Padding(0);
      this.label6.Name = "label6";
      this.label6.Size = new Size(13, 23);
      this.label6.TabIndex = 6;
      this.label6.Text = "26";
      this.label5.Dock = DockStyle.Fill;
      this.label5.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label5.ImageAlign = ContentAlignment.BottomRight;
      this.label5.Location = new Point(57, 1);
      this.label5.Margin = new Padding(0);
      this.label5.Name = "label5";
      this.label5.Size = new Size(13, 23);
      this.label5.TabIndex = 5;
      this.label5.Text = "27";
      this.label4.Dock = DockStyle.Fill;
      this.label4.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label4.ImageAlign = ContentAlignment.BottomRight;
      this.label4.Location = new Point(43, 1);
      this.label4.Margin = new Padding(0);
      this.label4.Name = "label4";
      this.label4.Size = new Size(13, 23);
      this.label4.TabIndex = 4;
      this.label4.Text = "28";
      this.label3.Dock = DockStyle.Fill;
      this.label3.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label3.ImageAlign = ContentAlignment.BottomRight;
      this.label3.Location = new Point(29, 1);
      this.label3.Margin = new Padding(0);
      this.label3.Name = "label3";
      this.label3.Size = new Size(13, 23);
      this.label3.TabIndex = 3;
      this.label3.Text = "29";
      this.label2.Dock = DockStyle.Fill;
      this.label2.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label2.ImageAlign = ContentAlignment.BottomRight;
      this.label2.Location = new Point(15, 1);
      this.label2.Margin = new Padding(0);
      this.label2.Name = "label2";
      this.label2.Size = new Size(13, 23);
      this.label2.TabIndex = 2;
      this.label2.Text = "30";
      this.label1.Dock = DockStyle.Fill;
      this.label1.Font = new Font("Microsoft Sans Serif", 6f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label1.ImageAlign = ContentAlignment.BottomRight;
      this.label1.Location = new Point(1, 1);
      this.label1.Margin = new Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new Size(13, 23);
      this.label1.TabIndex = 1;
      this.label1.Text = "31";
      this.tableLayoutPanel2.BackColor = SystemColors.InactiveBorder;
      this.tableLayoutPanel2.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
      this.tableLayoutPanel2.Controls.Add((Control) this.panel3, 1, 0);
      this.tableLayoutPanel2.Controls.Add((Control) this.panel2, 0, 0);
      this.tableLayoutPanel2.Location = new Point(12, 76);
      this.tableLayoutPanel2.Margin = new Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel2.Size = new Size(450, 25);
      this.tableLayoutPanel2.TabIndex = 1;
      this.panel3.Controls.Add((Control) this.textBox2);
      this.panel3.Controls.Add((Control) this.checkBox5);
      this.panel3.Controls.Add((Control) this.checkBox6);
      this.panel3.Dock = DockStyle.Fill;
      this.panel3.Location = new Point(228, 1);
      this.panel3.Margin = new Padding(3, 0, 3, 0);
      this.panel3.Name = "panel3";
      this.panel3.Size = new Size(218, 23);
      this.panel3.TabIndex = 1;
      this.textBox2.BackColor = Color.White;
      this.textBox2.BorderStyle = BorderStyle.None;
      this.textBox2.Dock = DockStyle.Fill;
      this.textBox2.Location = new Point(116, 0);
      this.textBox2.Margin = new Padding(0);
      this.textBox2.MaximumSize = new Size(0, 23);
      this.textBox2.MinimumSize = new Size(0, 23);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new Size(102, 23);
      this.textBox2.TabIndex = 5;
      this.textBox2.TextAlign = HorizontalAlignment.Right;
      this.textBox2.TextChanged += new EventHandler(this.textBox2_TextChanged);
      this.checkBox5.AutoSize = true;
      this.checkBox5.Dock = DockStyle.Left;
      this.checkBox5.Location = new Point(71, 0);
      this.checkBox5.Name = "checkBox5";
      this.checkBox5.Size = new Size(45, 23);
      this.checkBox5.TabIndex = 3;
      this.checkBox5.Text = "Hex";
      this.checkBox5.UseVisualStyleBackColor = true;
      this.checkBox5.CheckedChanged += new EventHandler(this.checkBox5_CheckedChanged);
      this.checkBox6.AutoSize = true;
      this.checkBox6.Dock = DockStyle.Left;
      this.checkBox6.Location = new Point(0, 0);
      this.checkBox6.Name = "checkBox6";
      this.checkBox6.Size = new Size(71, 23);
      this.checkBox6.TabIndex = 2;
      this.checkBox6.Text = "Unsigned";
      this.checkBox6.UseVisualStyleBackColor = true;
      this.checkBox6.CheckedChanged += new EventHandler(this.checkBox6_CheckedChanged);
      this.panel2.Controls.Add((Control) this.textBox1);
      this.panel2.Controls.Add((Control) this.checkBox3);
      this.panel2.Controls.Add((Control) this.checkBox4);
      this.panel2.Dock = DockStyle.Fill;
      this.panel2.Location = new Point(4, 1);
      this.panel2.Margin = new Padding(3, 0, 3, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new Size(217, 23);
      this.panel2.TabIndex = 0;
      this.textBox1.BackColor = Color.White;
      this.textBox1.BorderStyle = BorderStyle.None;
      this.textBox1.Dock = DockStyle.Fill;
      this.textBox1.Location = new Point(116, 0);
      this.textBox1.Margin = new Padding(0);
      this.textBox1.MaximumSize = new Size(0, 23);
      this.textBox1.MinimumSize = new Size(0, 23);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new Size(101, 23);
      this.textBox1.TabIndex = 4;
      this.textBox1.TextAlign = HorizontalAlignment.Right;
      this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
      this.checkBox3.AutoSize = true;
      this.checkBox3.Dock = DockStyle.Left;
      this.checkBox3.Location = new Point(71, 0);
      this.checkBox3.Name = "checkBox3";
      this.checkBox3.Size = new Size(45, 23);
      this.checkBox3.TabIndex = 3;
      this.checkBox3.Text = "Hex";
      this.checkBox3.UseVisualStyleBackColor = true;
      this.checkBox3.CheckedChanged += new EventHandler(this.checkBox3_CheckedChanged);
      this.checkBox4.AutoSize = true;
      this.checkBox4.Dock = DockStyle.Left;
      this.checkBox4.Location = new Point(0, 0);
      this.checkBox4.Name = "checkBox4";
      this.checkBox4.Size = new Size(71, 23);
      this.checkBox4.TabIndex = 2;
      this.checkBox4.Text = "Unsigned";
      this.checkBox4.UseVisualStyleBackColor = true;
      this.checkBox4.CheckedChanged += new EventHandler(this.checkBox4_CheckedChanged);
      this.radioButton1.Location = new Point(468, 45);
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.Size = new Size(55, 25);
      this.radioButton1.TabIndex = 2;
      this.radioButton1.TabStop = true;
      this.radioButton1.Text = "32 Bit";
      this.radioButton1.UseVisualStyleBackColor = true;
      this.radioButton1.CheckedChanged += new EventHandler(this.radioButton1_CheckedChanged);
      this.tableLayoutPanel3.BackColor = SystemColors.InactiveBorder;
      this.tableLayoutPanel3.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel3.Controls.Add((Control) this.panel1, 0, 0);
      this.tableLayoutPanel3.Location = new Point(12, 45);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 1;
      this.tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
      this.tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
      this.tableLayoutPanel3.Size = new Size(450, 25);
      this.tableLayoutPanel3.TabIndex = 3;
      this.panel1.Controls.Add((Control) this.textBox3);
      this.panel1.Controls.Add((Control) this.checkBox2);
      this.panel1.Controls.Add((Control) this.checkBox1);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(4, 1);
      this.panel1.Margin = new Padding(3, 0, 3, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(442, 23);
      this.panel1.TabIndex = 0;
      this.textBox3.BackColor = Color.White;
      this.textBox3.BorderStyle = BorderStyle.None;
      this.textBox3.Dock = DockStyle.Fill;
      this.textBox3.Location = new Point(116, 0);
      this.textBox3.Margin = new Padding(0);
      this.textBox3.MaximumSize = new Size(0, 23);
      this.textBox3.MinimumSize = new Size(0, 23);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new Size(326, 23);
      this.textBox3.TabIndex = 5;
      this.textBox3.TextAlign = HorizontalAlignment.Right;
      this.textBox3.TextChanged += new EventHandler(this.textBox3_TextChanged);
      this.checkBox2.AutoSize = true;
      this.checkBox2.Dock = DockStyle.Left;
      this.checkBox2.Location = new Point(71, 0);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new Size(45, 23);
      this.checkBox2.TabIndex = 1;
      this.checkBox2.Text = "Hex";
      this.checkBox2.UseVisualStyleBackColor = true;
      this.checkBox2.CheckedChanged += new EventHandler(this.checkBox2_CheckedChanged);
      this.checkBox1.AutoSize = true;
      this.checkBox1.Dock = DockStyle.Left;
      this.checkBox1.Location = new Point(0, 0);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(71, 23);
      this.checkBox1.TabIndex = 0;
      this.checkBox1.Text = "Unsigned";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.radioButton2.Location = new Point(468, 76);
      this.radioButton2.Name = "radioButton2";
      this.radioButton2.Size = new Size(55, 25);
      this.radioButton2.TabIndex = 4;
      this.radioButton2.TabStop = true;
      this.radioButton2.Text = "16 Bit";
      this.radioButton2.UseVisualStyleBackColor = true;
      this.radioButton2.CheckedChanged += new EventHandler(this.radioButton2_CheckedChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(535, 116);
      this.Controls.Add((Control) this.radioButton2);
      this.Controls.Add((Control) this.tableLayoutPanel3);
      this.Controls.Add((Control) this.radioButton1);
      this.Controls.Add((Control) this.tableLayoutPanel2);
      this.Controls.Add((Control) this.tableLayoutPanel1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (ObjectDbObjectSettingsEditor);
      this.Text = "Object Setting Config";
      this.FormClosing += new FormClosingEventHandler(this.ObjectDbObjectSettingsEditor_FormClosing);
      this.Load += new EventHandler(this.ObjectDbObjectSettingsEditor_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.panel3.ResumeLayout(false);
      this.panel3.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
    }

    public ObjectDbObjectSettingsEditor(List<ObjectDb.Object.Setting.SettingData> s)
    {
      this.Setting = s;
      this.InitializeComponent();
      if (this.Setting.Count == 1)
      {
        this.tableLayoutPanel3.Enabled = true;
        this.tableLayoutPanel2.Enabled = false;
        this.radioButton1.Checked = true;
        this.radioButton2.Checked = false;
        this.textBox3.Text = this.Setting[0].Name;
        this.checkBox2.Checked = this.Setting[0].Hex;
        if (this.Setting[0] is ObjectDb.Object.Setting.U32)
          this.checkBox1.Checked = true;
        this.textBox1.Text = "Unknown";
        this.textBox2.Text = "Unknown";
        this.checkBox4.Checked = true;
        this.checkBox6.Checked = true;
        this.checkBox3.Checked = true;
        this.checkBox5.Checked = true;
      }
      else
      {
        this.tableLayoutPanel3.Enabled = false;
        this.tableLayoutPanel2.Enabled = true;
        this.radioButton1.Checked = false;
        this.radioButton2.Checked = true;
        this.textBox1.Text = this.Setting[0].Name;
        this.textBox2.Text = this.Setting[1].Name;
        this.checkBox3.Checked = this.Setting[0].Hex;
        this.checkBox5.Checked = this.Setting[1].Hex;
        if (this.Setting[0] is ObjectDb.Object.Setting.U16)
          this.checkBox4.Checked = true;
        if (this.Setting[1] is ObjectDb.Object.Setting.U16)
          this.checkBox6.Checked = true;
        this.textBox3.Text = "Unknown";
        this.checkBox2.Checked = true;
        this.checkBox1.Checked = true;
      }
      this.start = false;
    }

    private void ObjectDbObjectSettingsEditor_Load(object sender, EventArgs e)
    {
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.radioButton1.Checked || this.start)
        return;
      this.tableLayoutPanel3.Enabled = true;
      this.tableLayoutPanel2.Enabled = false;
      this.Setting.Clear();
      ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
      settingData.Name = this.textBox3.Text;
      this.Setting.Add(settingData);
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.radioButton2.Checked || this.start)
        return;
      this.tableLayoutPanel3.Enabled = false;
      this.tableLayoutPanel2.Enabled = true;
      this.Setting.Clear();
      ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
      settingData1.Name = this.textBox1.Text;
      this.Setting.Add(settingData1);
      ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
      settingData2.Name = this.textBox2.Text;
      this.Setting.Add(settingData2);
    }

    private void checkBox5_CheckedChanged(object sender, EventArgs e)
    {
      if (!this.radioButton2.Checked || this.start)
        return;
      this.Setting.Clear();
      ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
      settingData1.Name = this.textBox1.Text;
      this.Setting.Add(settingData1);
      ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
      settingData2.Name = this.textBox2.Text;
      this.Setting.Add(settingData2);
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void checkBox3_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void checkBox6_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void checkBox4_CheckedChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void textBox3_TextChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {
      if (this.radioButton2.Checked && !this.start)
      {
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData1 = !this.checkBox4.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox3.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox3.Checked);
        settingData1.Name = this.textBox1.Text;
        this.Setting.Add(settingData1);
        ObjectDb.Object.Setting.SettingData settingData2 = !this.checkBox6.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S16(this.checkBox5.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U16(this.checkBox5.Checked);
        settingData2.Name = this.textBox2.Text;
        this.Setting.Add(settingData2);
      }
      else
      {
        if (!this.radioButton1.Checked || this.start)
          return;
        this.Setting.Clear();
        ObjectDb.Object.Setting.SettingData settingData = !this.checkBox1.Checked ? (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.S32(this.checkBox2.Checked) : (ObjectDb.Object.Setting.SettingData) new ObjectDb.Object.Setting.U32(this.checkBox2.Checked);
        settingData.Name = this.textBox3.Text;
        this.Setting.Add(settingData);
      }
    }

    private void ObjectDbObjectSettingsEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.DialogResult = DialogResult.OK;
    }
  }
}
