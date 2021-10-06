// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.NKM
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters.Colission;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.Properties;
using OpenTK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using Tao.OpenGl;
using Tao.Platform.Windows;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class NKM : BaseForm
  {
    private IContainer components = (IContainer) null;
    private KCL KCL = (KCL) null;
    private int scale = 1;
    private bool first = true;
    private float min = -8192f;
    private float max = 8192f;
    private float mult = 0.0f;
    private Point last = new Point(int.MinValue, int.MinValue);
    private bool objidraw = false;
    private bool pathdraw = false;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private SplitContainer splitContainer1;
    private PropertyGrid propertyGrid1;
    private TabPage tabPage1;
    private SimpleOpenGlControl simpleOpenGlControl1;
    private TabPage tabPage2;
    private TabPage tabPage3;
    private TabPage tabPage4;
    private TabPage tabPage5;
    private TabPage tabPage6;
    private TabPage tabPage7;
    private TabPage tabPage8;
    private TabPage tabPage9;
    private TabPage tabPage10;
    private TabPage tabPage11;
    private TabPage tabPage12;
    private TabPage tabPage13;
    private TabPage tabPage14;
    private TabPage tabPage15;
    private TabPage tabPage16;
    private TabPage tabPage17;
    private TabPage tabPage18;
    private TabPage tabPage19;
    private VScrollBar vScrollBar1;
    private HScrollBar hScrollBar1;
    private ToolStrip toolStrip2;
    private ToolStrip toolStrip3;
    private ToolStrip toolStrip4;
    private ToolStrip toolStrip6;
    private ToolStrip toolStrip7;
    private ToolStrip toolStrip8;
    private ToolStrip toolStrip9;
    private ToolStrip toolStrip10;
    private ToolStrip toolStrip11;
    private ToolStrip toolStrip12;
    private ToolStrip toolStrip13;
    private ToolStrip toolStrip14;
    private ToolStrip toolStrip15;
    private ToolStrip toolStrip16;
    private ToolStrip toolStrip17;
    private ToolStrip toolStrip18;
    private ToolStrip toolStrip19;
    private Form1.ListViewNF listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private ColumnHeader columnHeader4;
    private ColumnHeader columnHeader5;
    private ColumnHeader columnHeader6;
    private ColumnHeader columnHeader7;
    private ColumnHeader columnHeader8;
    private ColumnHeader columnHeader9;
    private ColumnHeader columnHeader10;
    private ColumnHeader columnHeader11;
    private ColumnHeader columnHeader12;
    private ColumnHeader columnHeader13;
    private ColumnHeader columnHeader14;
    private ColumnHeader columnHeader15;
    private ColumnHeader columnHeader16;
    private ColumnHeader columnHeader17;
    private Form1.ListViewNF listViewNF1;
    private ColumnHeader columnHeader18;
    private ColumnHeader columnHeader19;
    private ColumnHeader columnHeader20;
    private ColumnHeader columnHeader21;
    private Form1.ListViewNF listViewNF2;
    private ColumnHeader columnHeader22;
    private ColumnHeader columnHeader23;
    private ColumnHeader columnHeader24;
    private ColumnHeader columnHeader25;
    private ColumnHeader columnHeader26;
    private ColumnHeader columnHeader27;
    private ColumnHeader columnHeader28;
    private Form1.ListViewNF listViewNF3;
    private ColumnHeader columnHeader29;
    private ColumnHeader columnHeader30;
    private ColumnHeader columnHeader31;
    private ColumnHeader columnHeader32;
    private ColumnHeader columnHeader36;
    private ColumnHeader columnHeader37;
    private ColumnHeader columnHeader38;
    private ColumnHeader columnHeader33;
    private ColumnHeader columnHeader35;
    private Form1.ListViewNF listViewNF4;
    private ColumnHeader columnHeader34;
    private ColumnHeader columnHeader39;
    private ColumnHeader columnHeader40;
    private ColumnHeader columnHeader41;
    private ColumnHeader columnHeader42;
    private ColumnHeader columnHeader43;
    private ColumnHeader columnHeader44;
    private ColumnHeader columnHeader45;
    private ColumnHeader columnHeader46;
    private TabPage tabPage20;
    private Form1.ListViewNF listViewNF5;
    private ColumnHeader columnHeader47;
    private ColumnHeader columnHeader48;
    private ColumnHeader columnHeader49;
    private ColumnHeader columnHeader50;
    private ColumnHeader columnHeader51;
    private ColumnHeader columnHeader52;
    private ColumnHeader columnHeader53;
    private ColumnHeader columnHeader54;
    private ColumnHeader columnHeader55;
    private ToolStrip toolStrip20;
    private ToolStripButton toolStripButton2;
    private ColumnHeader columnHeader56;
    private Form1.ListViewNF listViewNF6;
    private ColumnHeader columnHeader57;
    private ColumnHeader columnHeader58;
    private ColumnHeader columnHeader59;
    private ColumnHeader columnHeader60;
    private ColumnHeader columnHeader61;
    private ColumnHeader columnHeader62;
    private ColumnHeader columnHeader63;
    private ColumnHeader columnHeader64;
    private ColumnHeader columnHeader65;
    private Form1.ListViewNF listViewNF7;
    private ColumnHeader columnHeader66;
    private ColumnHeader columnHeader67;
    private ColumnHeader columnHeader68;
    private ColumnHeader columnHeader69;
    private ColumnHeader columnHeader70;
    private ColumnHeader columnHeader71;
    private ColumnHeader columnHeader72;
    private ColumnHeader columnHeader73;
    private ColumnHeader columnHeader74;
    private Form1.ListViewNF listViewNF8;
    private ColumnHeader columnHeader75;
    private ColumnHeader columnHeader76;
    private ColumnHeader columnHeader78;
    private ColumnHeader columnHeader79;
    private ColumnHeader columnHeader80;
    private ColumnHeader columnHeader81;
    private ColumnHeader columnHeader82;
    private ColumnHeader columnHeader83;
    private ColumnHeader columnHeader84;
    private ColumnHeader columnHeader77;
    private ColumnHeader columnHeader85;
    private Form1.ListViewNF listViewNF9;
    private ColumnHeader columnHeader86;
    private ColumnHeader columnHeader87;
    private ColumnHeader columnHeader88;
    private ColumnHeader columnHeader89;
    private ColumnHeader columnHeader90;
    private ColumnHeader columnHeader91;
    private ColumnHeader columnHeader92;
    private ColumnHeader columnHeader93;
    private ColumnHeader columnHeader94;
    private ColumnHeader columnHeader95;
    private Form1.ListViewNF listViewNF10;
    private ColumnHeader columnHeader96;
    private ColumnHeader columnHeader97;
    private ColumnHeader columnHeader98;
    private ColumnHeader columnHeader99;
    private ColumnHeader columnHeader100;
    private ColumnHeader columnHeader101;
    private Form1.ListViewNF listViewNF11;
    private ColumnHeader columnHeader102;
    private ColumnHeader columnHeader103;
    private ColumnHeader columnHeader104;
    private ColumnHeader columnHeader105;
    private ColumnHeader columnHeader106;
    private ColumnHeader columnHeader107;
    private ColumnHeader columnHeader108;
    private ColumnHeader columnHeader109;
    private ColumnHeader columnHeader110;
    private ColumnHeader columnHeader111;
    private Form1.ListViewNF listViewNF12;
    private ColumnHeader columnHeader112;
    private ColumnHeader columnHeader113;
    private ColumnHeader columnHeader114;
    private ColumnHeader columnHeader115;
    private ColumnHeader columnHeader116;
    private ColumnHeader columnHeader118;
    private Form1.ListViewNF listViewNF13;
    private ColumnHeader columnHeader119;
    private ColumnHeader columnHeader120;
    private ColumnHeader columnHeader121;
    private ColumnHeader columnHeader122;
    private ColumnHeader columnHeader123;
    private ColumnHeader columnHeader124;
    private ColumnHeader columnHeader125;
    private ColumnHeader columnHeader126;
    private ColumnHeader columnHeader127;
    private ColumnHeader columnHeader128;
    private ColumnHeader columnHeader129;
    private Form1.ListViewNF listViewNF14;
    private ColumnHeader columnHeader117;
    private ColumnHeader columnHeader130;
    private ColumnHeader columnHeader131;
    private ColumnHeader columnHeader132;
    private ColumnHeader columnHeader133;
    private ColumnHeader columnHeader134;
    private ColumnHeader columnHeader135;
    private Form1.ListViewNF listViewNF15;
    private ColumnHeader columnHeader136;
    private ColumnHeader columnHeader137;
    private ColumnHeader columnHeader138;
    private ColumnHeader columnHeader139;
    private ColumnHeader columnHeader140;
    private ColumnHeader columnHeader141;
    private ColumnHeader columnHeader142;
    private ColumnHeader columnHeader143;
    private ColumnHeader columnHeader144;
    private ColumnHeader columnHeader146;
    private ColumnHeader columnHeader147;
    private ColumnHeader columnHeader148;
    private ColumnHeader columnHeader149;
    private ColumnHeader columnHeader150;
    private ColumnHeader columnHeader151;
    private ColumnHeader columnHeader152;
    private ColumnHeader columnHeader153;
    private ColumnHeader columnHeader154;
    private ColumnHeader columnHeader155;
    private Form1.ListViewNF listViewNF16;
    private ColumnHeader columnHeader145;
    private ColumnHeader columnHeader156;
    private ColumnHeader columnHeader157;
    private ColumnHeader columnHeader158;
    private ColumnHeader columnHeader159;
    private ColumnHeader columnHeader160;
    private ColumnHeader columnHeader161;
    private ColumnHeader columnHeader162;
    private ColumnHeader columnHeader163;
    private ColumnHeader columnHeader164;
    private ColumnHeader columnHeader165;
    private ColumnHeader columnHeader166;
    private ColumnHeader columnHeader167;
    private ColumnHeader columnHeader168;
    private ColumnHeader columnHeader169;
    private ColumnHeader columnHeader170;
    private ColumnHeader columnHeader171;
    private ColumnHeader columnHeader172;
    private ColumnHeader columnHeader173;
    private ColumnHeader columnHeader174;
    private ColumnHeader columnHeader175;
    private Form1.ListViewNF listViewNF17;
    private ColumnHeader columnHeader176;
    private ColumnHeader columnHeader177;
    private ColumnHeader columnHeader178;
    private ColumnHeader columnHeader179;
    private ColumnHeader columnHeader180;
    private ColumnHeader columnHeader181;
    private ColumnHeader columnHeader182;
    private ColumnHeader columnHeader183;
    private ColumnHeader columnHeader184;
    private ColumnHeader columnHeader185;
    private ColumnHeader columnHeader186;
    private ColumnHeader columnHeader187;
    private ColumnHeader columnHeader188;
    private ColumnHeader columnHeader189;
    private ColumnHeader columnHeader190;
    private ColumnHeader columnHeader191;
    private ColumnHeader columnHeader192;
    private ColumnHeader columnHeader193;
    private ColumnHeader columnHeader194;
    private ColumnHeader columnHeader195;
    private ColumnHeader columnHeader196;
    private ColumnHeader columnHeader197;
    private ColumnHeader columnHeader198;
    private ToolStripButton toolStripButton3;
    private ToolStripButton toolStripButton4;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton toolStripButton5;
    private ToolStripButton toolStripButton6;
    private ToolStripButton toolStripButton7;
    private ToolStripButton toolStripButton8;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripButton toolStripButton9;
    private ToolStripButton toolStripButton10;
    private ToolStripButton toolStripButton11;
    private ToolStripButton toolStripButton12;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripButton toolStripButton13;
    private ToolStripButton toolStripButton14;
    private ToolStripButton toolStripButton15;
    private ToolStripButton toolStripButton16;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripButton toolStripButton17;
    private ToolStripButton toolStripButton18;
    private ToolStripButton toolStripButton19;
    private ToolStripButton toolStripButton20;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripButton toolStripButton21;
    private ToolStripButton toolStripButton22;
    private ToolStripButton toolStripButton23;
    private ToolStripButton toolStripButton24;
    private ToolStripSeparator toolStripSeparator6;
    private ToolStripButton toolStripButton25;
    private ToolStripButton toolStripButton26;
    private ToolStripButton toolStripButton27;
    private ToolStripButton toolStripButton28;
    private ToolStripSeparator toolStripSeparator7;
    private ToolStripButton toolStripButton29;
    private ToolStripButton toolStripButton30;
    private ToolStripButton toolStripButton31;
    private ToolStripButton toolStripButton32;
    private ToolStripSeparator toolStripSeparator8;
    private ToolStripButton toolStripButton33;
    private ToolStripButton toolStripButton34;
    private ToolStripButton toolStripButton35;
    private ToolStripButton toolStripButton36;
    private ToolStripSeparator toolStripSeparator9;
    private ToolStripButton toolStripButton37;
    private ToolStripButton toolStripButton38;
    private ToolStripButton toolStripButton39;
    private ToolStripButton toolStripButton40;
    private ToolStripSeparator toolStripSeparator10;
    private ToolStripButton toolStripButton41;
    private ToolStripButton toolStripButton42;
    private ToolStripButton toolStripButton43;
    private ToolStripButton toolStripButton44;
    private ToolStripSeparator toolStripSeparator11;
    private ToolStripButton toolStripButton45;
    private ToolStripButton toolStripButton46;
    private ToolStripButton toolStripButton47;
    private ToolStripButton toolStripButton48;
    private ToolStripSeparator toolStripSeparator12;
    private ToolStripButton toolStripButton49;
    private ToolStripButton toolStripButton50;
    private ToolStripButton toolStripButton51;
    private ToolStripButton toolStripButton52;
    private ToolStripSeparator toolStripSeparator13;
    private ToolStripButton toolStripButton53;
    private ToolStripButton toolStripButton54;
    private ToolStripButton toolStripButton55;
    private ToolStripButton toolStripButton56;
    private ToolStripSeparator toolStripSeparator14;
    private ToolStripButton toolStripButton57;
    private ToolStripButton toolStripButton58;
    private ToolStripButton toolStripButton59;
    private ToolStripButton toolStripButton60;
    private ToolStripSeparator toolStripSeparator15;
    private ToolStripButton toolStripButton61;
    private ToolStripButton toolStripButton62;
    private ToolStripButton toolStripButton63;
    private ToolStripButton toolStripButton64;
    private ToolStripSeparator toolStripSeparator16;
    private ToolStripButton toolStripButton65;
    private ToolStripButton toolStripButton66;
    private ToolStripButton toolStripButton67;
    private ToolStripButton toolStripButton68;
    private ToolStripSeparator toolStripSeparator17;
    private ToolStripButton toolStripButton69;
    private ToolStripButton toolStripButton70;
    private ToolStripButton toolStripButton71;
    private ToolStripButton toolStripButton72;
    private ToolStripSeparator toolStripSeparator18;
    private ToolStripButton toolStripButton73;
    private ToolStripButton toolStripButton74;
    private GroupBox groupBox1;
    private Button button4;
    private Panel panel4;
    private Label label5;
    private Button button3;
    private Panel panel3;
    private Label label4;
    private Button button2;
    private Panel panel2;
    private Label label3;
    private Button button1;
    private Panel panel1;
    private Label label2;
    private Label label1;
    private ComboBox comboBox1;
    private ColorDialog colorDialog1;
    private ToolStrip toolStrip5;
    private ToolStripButton toolStripButton75;
    private ToolStripSeparator toolStripSeparator19;
    private ToolStripButton toolStripButton76;
    private ToolStripButton toolStripButton77;
    private ToolStripSeparator toolStripSeparator20;
    private ToolStripButton toolStripButton78;
    private Button button5;
    private Panel panel5;
    private Label label6;
    private Label label7;
    private NumericUpDown numericUpDown1;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStripSplitButton toolStripSplitButton1;
    private ToolStripMenuItem oBJIToolStripMenuItem;
    private ToolStripMenuItem pOITToolStripMenuItem;
    private ToolStripMenuItem kTPSToolStripMenuItem;
    private ToolStripMenuItem kTPJToolStripMenuItem;
    private ToolStripMenuItem kTP2ToolStripMenuItem;
    private ToolStripMenuItem kTPCToolStripMenuItem;
    private ToolStripMenuItem kTPMToolStripMenuItem;
    private ToolStripMenuItem cPOIToolStripMenuItem;
    private ToolStripMenuItem iPOIToolStripMenuItem;
    private ToolStripMenuItem ePOIToolStripMenuItem;
    private ToolStripMenuItem aREAToolStripMenuItem;
    private ToolStripMenuItem cAMEToolStripMenuItem;
    public TabControl tabControl1;
    private ColumnHeader columnHeader199;
    private ColumnHeader columnHeader200;
    private GroupBox groupBox2;
    private NumericUpDown numericUpDown2;
    private Label label8;
    private TabPage tabPage21;
    private Label label11;
    private TextBox textBox2;
    private Label label10;
    private TextBox textBox1;
    private Label label9;
    private Label label12;
    private TextBox textBox3;
    private Label label13;
    private ToolStripButton toolStripButton79;
    private Label label14;
    private CheckBox checkBox1;
    private ComboBox comboBox3;
    private ComboBox comboBox2;
    private Label label15;
    private MKDS_Course_Modifier.MKDS.NKM File;
    private byte[] pic;
    public int SelType;
    public int SelIdx;
    public object tmpobj;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (NKM));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.splitContainer1 = new SplitContainer();
      this.propertyGrid1 = new PropertyGrid();
      this.tabControl1 = new TabControl();
      this.tabPage1 = new TabPage();
      this.simpleOpenGlControl1 = new SimpleOpenGlControl();
      this.toolStrip5 = new ToolStrip();
      this.toolStripButton75 = new ToolStripButton();
      this.toolStripSeparator19 = new ToolStripSeparator();
      this.toolStripButton76 = new ToolStripButton();
      this.toolStripButton77 = new ToolStripButton();
      this.toolStripSeparator20 = new ToolStripSeparator();
      this.toolStripButton78 = new ToolStripButton();
      this.toolStripSplitButton1 = new ToolStripSplitButton();
      this.oBJIToolStripMenuItem = new ToolStripMenuItem();
      this.pOITToolStripMenuItem = new ToolStripMenuItem();
      this.kTPSToolStripMenuItem = new ToolStripMenuItem();
      this.kTPJToolStripMenuItem = new ToolStripMenuItem();
      this.kTP2ToolStripMenuItem = new ToolStripMenuItem();
      this.kTPCToolStripMenuItem = new ToolStripMenuItem();
      this.kTPMToolStripMenuItem = new ToolStripMenuItem();
      this.cPOIToolStripMenuItem = new ToolStripMenuItem();
      this.iPOIToolStripMenuItem = new ToolStripMenuItem();
      this.ePOIToolStripMenuItem = new ToolStripMenuItem();
      this.aREAToolStripMenuItem = new ToolStripMenuItem();
      this.cAMEToolStripMenuItem = new ToolStripMenuItem();
      this.vScrollBar1 = new VScrollBar();
      this.hScrollBar1 = new HScrollBar();
      this.tabPage2 = new TabPage();
      this.listView1 = new Form1.ListViewNF();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.columnHeader3 = new ColumnHeader();
      this.columnHeader4 = new ColumnHeader();
      this.columnHeader5 = new ColumnHeader();
      this.columnHeader6 = new ColumnHeader();
      this.columnHeader7 = new ColumnHeader();
      this.columnHeader8 = new ColumnHeader();
      this.columnHeader9 = new ColumnHeader();
      this.columnHeader10 = new ColumnHeader();
      this.columnHeader11 = new ColumnHeader();
      this.columnHeader12 = new ColumnHeader();
      this.columnHeader13 = new ColumnHeader();
      this.columnHeader14 = new ColumnHeader();
      this.columnHeader15 = new ColumnHeader();
      this.columnHeader16 = new ColumnHeader();
      this.columnHeader17 = new ColumnHeader();
      this.toolStrip2 = new ToolStrip();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripButton4 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton5 = new ToolStripButton();
      this.toolStripButton6 = new ToolStripButton();
      this.tabPage3 = new TabPage();
      this.listViewNF1 = new Form1.ListViewNF();
      this.columnHeader18 = new ColumnHeader();
      this.columnHeader19 = new ColumnHeader();
      this.columnHeader20 = new ColumnHeader();
      this.columnHeader21 = new ColumnHeader();
      this.toolStrip3 = new ToolStrip();
      this.toolStripButton7 = new ToolStripButton();
      this.toolStripButton8 = new ToolStripButton();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStripButton9 = new ToolStripButton();
      this.toolStripButton10 = new ToolStripButton();
      this.tabPage4 = new TabPage();
      this.listViewNF2 = new Form1.ListViewNF();
      this.columnHeader22 = new ColumnHeader();
      this.columnHeader23 = new ColumnHeader();
      this.columnHeader24 = new ColumnHeader();
      this.columnHeader25 = new ColumnHeader();
      this.columnHeader26 = new ColumnHeader();
      this.columnHeader27 = new ColumnHeader();
      this.columnHeader28 = new ColumnHeader();
      this.toolStrip4 = new ToolStrip();
      this.toolStripButton11 = new ToolStripButton();
      this.toolStripButton12 = new ToolStripButton();
      this.toolStripSeparator3 = new ToolStripSeparator();
      this.toolStripButton13 = new ToolStripButton();
      this.toolStripButton14 = new ToolStripButton();
      this.tabPage5 = new TabPage();
      this.groupBox2 = new GroupBox();
      this.numericUpDown2 = new NumericUpDown();
      this.label8 = new Label();
      this.label7 = new Label();
      this.numericUpDown1 = new NumericUpDown();
      this.label6 = new Label();
      this.button5 = new Button();
      this.panel5 = new Panel();
      this.groupBox1 = new GroupBox();
      this.button4 = new Button();
      this.panel4 = new Panel();
      this.label5 = new Label();
      this.button3 = new Button();
      this.panel3 = new Panel();
      this.label4 = new Label();
      this.button2 = new Button();
      this.panel2 = new Panel();
      this.label3 = new Label();
      this.button1 = new Button();
      this.panel1 = new Panel();
      this.label2 = new Label();
      this.label1 = new Label();
      this.comboBox1 = new ComboBox();
      this.tabPage6 = new TabPage();
      this.listViewNF3 = new Form1.ListViewNF();
      this.columnHeader29 = new ColumnHeader();
      this.columnHeader30 = new ColumnHeader();
      this.columnHeader31 = new ColumnHeader();
      this.columnHeader32 = new ColumnHeader();
      this.columnHeader36 = new ColumnHeader();
      this.columnHeader37 = new ColumnHeader();
      this.columnHeader38 = new ColumnHeader();
      this.columnHeader33 = new ColumnHeader();
      this.columnHeader35 = new ColumnHeader();
      this.toolStrip6 = new ToolStrip();
      this.toolStripButton15 = new ToolStripButton();
      this.toolStripButton16 = new ToolStripButton();
      this.toolStripSeparator4 = new ToolStripSeparator();
      this.toolStripButton17 = new ToolStripButton();
      this.toolStripButton18 = new ToolStripButton();
      this.tabPage7 = new TabPage();
      this.listViewNF4 = new Form1.ListViewNF();
      this.columnHeader34 = new ColumnHeader();
      this.columnHeader39 = new ColumnHeader();
      this.columnHeader40 = new ColumnHeader();
      this.columnHeader41 = new ColumnHeader();
      this.columnHeader42 = new ColumnHeader();
      this.columnHeader43 = new ColumnHeader();
      this.columnHeader44 = new ColumnHeader();
      this.columnHeader45 = new ColumnHeader();
      this.columnHeader56 = new ColumnHeader();
      this.columnHeader46 = new ColumnHeader();
      this.toolStrip7 = new ToolStrip();
      this.toolStripButton19 = new ToolStripButton();
      this.toolStripButton20 = new ToolStripButton();
      this.toolStripSeparator5 = new ToolStripSeparator();
      this.toolStripButton21 = new ToolStripButton();
      this.toolStripButton22 = new ToolStripButton();
      this.tabPage20 = new TabPage();
      this.listViewNF5 = new Form1.ListViewNF();
      this.columnHeader47 = new ColumnHeader();
      this.columnHeader48 = new ColumnHeader();
      this.columnHeader49 = new ColumnHeader();
      this.columnHeader50 = new ColumnHeader();
      this.columnHeader51 = new ColumnHeader();
      this.columnHeader52 = new ColumnHeader();
      this.columnHeader53 = new ColumnHeader();
      this.columnHeader54 = new ColumnHeader();
      this.columnHeader55 = new ColumnHeader();
      this.toolStrip20 = new ToolStrip();
      this.toolStripButton23 = new ToolStripButton();
      this.toolStripButton24 = new ToolStripButton();
      this.toolStripSeparator6 = new ToolStripSeparator();
      this.toolStripButton25 = new ToolStripButton();
      this.toolStripButton26 = new ToolStripButton();
      this.tabPage8 = new TabPage();
      this.listViewNF6 = new Form1.ListViewNF();
      this.columnHeader57 = new ColumnHeader();
      this.columnHeader58 = new ColumnHeader();
      this.columnHeader59 = new ColumnHeader();
      this.columnHeader60 = new ColumnHeader();
      this.columnHeader61 = new ColumnHeader();
      this.columnHeader62 = new ColumnHeader();
      this.columnHeader63 = new ColumnHeader();
      this.columnHeader64 = new ColumnHeader();
      this.columnHeader65 = new ColumnHeader();
      this.toolStrip8 = new ToolStrip();
      this.toolStripButton27 = new ToolStripButton();
      this.toolStripButton28 = new ToolStripButton();
      this.toolStripSeparator7 = new ToolStripSeparator();
      this.toolStripButton29 = new ToolStripButton();
      this.toolStripButton30 = new ToolStripButton();
      this.tabPage9 = new TabPage();
      this.listViewNF7 = new Form1.ListViewNF();
      this.columnHeader66 = new ColumnHeader();
      this.columnHeader67 = new ColumnHeader();
      this.columnHeader68 = new ColumnHeader();
      this.columnHeader69 = new ColumnHeader();
      this.columnHeader70 = new ColumnHeader();
      this.columnHeader71 = new ColumnHeader();
      this.columnHeader72 = new ColumnHeader();
      this.columnHeader73 = new ColumnHeader();
      this.columnHeader74 = new ColumnHeader();
      this.toolStrip9 = new ToolStrip();
      this.toolStripButton31 = new ToolStripButton();
      this.toolStripButton32 = new ToolStripButton();
      this.toolStripSeparator8 = new ToolStripSeparator();
      this.toolStripButton33 = new ToolStripButton();
      this.toolStripButton34 = new ToolStripButton();
      this.tabPage10 = new TabPage();
      this.listViewNF8 = new Form1.ListViewNF();
      this.columnHeader75 = new ColumnHeader();
      this.columnHeader76 = new ColumnHeader();
      this.columnHeader78 = new ColumnHeader();
      this.columnHeader79 = new ColumnHeader();
      this.columnHeader80 = new ColumnHeader();
      this.columnHeader77 = new ColumnHeader();
      this.columnHeader85 = new ColumnHeader();
      this.columnHeader81 = new ColumnHeader();
      this.columnHeader82 = new ColumnHeader();
      this.columnHeader83 = new ColumnHeader();
      this.columnHeader84 = new ColumnHeader();
      this.toolStrip10 = new ToolStrip();
      this.toolStripButton35 = new ToolStripButton();
      this.toolStripButton36 = new ToolStripButton();
      this.toolStripSeparator9 = new ToolStripSeparator();
      this.toolStripButton37 = new ToolStripButton();
      this.toolStripButton38 = new ToolStripButton();
      this.tabPage11 = new TabPage();
      this.listViewNF9 = new Form1.ListViewNF();
      this.columnHeader86 = new ColumnHeader();
      this.columnHeader87 = new ColumnHeader();
      this.columnHeader88 = new ColumnHeader();
      this.columnHeader89 = new ColumnHeader();
      this.columnHeader90 = new ColumnHeader();
      this.columnHeader91 = new ColumnHeader();
      this.columnHeader92 = new ColumnHeader();
      this.columnHeader93 = new ColumnHeader();
      this.columnHeader94 = new ColumnHeader();
      this.columnHeader95 = new ColumnHeader();
      this.toolStrip11 = new ToolStrip();
      this.toolStripButton39 = new ToolStripButton();
      this.toolStripButton40 = new ToolStripButton();
      this.toolStripSeparator10 = new ToolStripSeparator();
      this.toolStripButton41 = new ToolStripButton();
      this.toolStripButton42 = new ToolStripButton();
      this.tabPage12 = new TabPage();
      this.listViewNF10 = new Form1.ListViewNF();
      this.columnHeader96 = new ColumnHeader();
      this.columnHeader97 = new ColumnHeader();
      this.columnHeader98 = new ColumnHeader();
      this.columnHeader99 = new ColumnHeader();
      this.columnHeader100 = new ColumnHeader();
      this.columnHeader101 = new ColumnHeader();
      this.toolStrip12 = new ToolStrip();
      this.toolStripButton43 = new ToolStripButton();
      this.toolStripButton44 = new ToolStripButton();
      this.toolStripSeparator11 = new ToolStripSeparator();
      this.toolStripButton45 = new ToolStripButton();
      this.toolStripButton46 = new ToolStripButton();
      this.tabPage13 = new TabPage();
      this.listViewNF11 = new Form1.ListViewNF();
      this.columnHeader102 = new ColumnHeader();
      this.columnHeader103 = new ColumnHeader();
      this.columnHeader104 = new ColumnHeader();
      this.columnHeader105 = new ColumnHeader();
      this.columnHeader106 = new ColumnHeader();
      this.columnHeader107 = new ColumnHeader();
      this.columnHeader108 = new ColumnHeader();
      this.columnHeader109 = new ColumnHeader();
      this.columnHeader110 = new ColumnHeader();
      this.columnHeader111 = new ColumnHeader();
      this.toolStrip13 = new ToolStrip();
      this.toolStripButton47 = new ToolStripButton();
      this.toolStripButton48 = new ToolStripButton();
      this.toolStripSeparator12 = new ToolStripSeparator();
      this.toolStripButton49 = new ToolStripButton();
      this.toolStripButton50 = new ToolStripButton();
      this.tabPage14 = new TabPage();
      this.listViewNF12 = new Form1.ListViewNF();
      this.columnHeader112 = new ColumnHeader();
      this.columnHeader113 = new ColumnHeader();
      this.columnHeader114 = new ColumnHeader();
      this.columnHeader115 = new ColumnHeader();
      this.columnHeader118 = new ColumnHeader();
      this.columnHeader116 = new ColumnHeader();
      this.columnHeader200 = new ColumnHeader();
      this.columnHeader129 = new ColumnHeader();
      this.toolStrip14 = new ToolStrip();
      this.toolStripButton51 = new ToolStripButton();
      this.toolStripButton52 = new ToolStripButton();
      this.toolStripSeparator13 = new ToolStripSeparator();
      this.toolStripButton53 = new ToolStripButton();
      this.toolStripButton54 = new ToolStripButton();
      this.tabPage15 = new TabPage();
      this.listViewNF13 = new Form1.ListViewNF();
      this.columnHeader119 = new ColumnHeader();
      this.columnHeader120 = new ColumnHeader();
      this.columnHeader121 = new ColumnHeader();
      this.columnHeader122 = new ColumnHeader();
      this.columnHeader123 = new ColumnHeader();
      this.columnHeader124 = new ColumnHeader();
      this.columnHeader125 = new ColumnHeader();
      this.columnHeader126 = new ColumnHeader();
      this.columnHeader127 = new ColumnHeader();
      this.columnHeader128 = new ColumnHeader();
      this.toolStrip15 = new ToolStrip();
      this.toolStripButton55 = new ToolStripButton();
      this.toolStripButton56 = new ToolStripButton();
      this.toolStripSeparator14 = new ToolStripSeparator();
      this.toolStripButton57 = new ToolStripButton();
      this.toolStripButton58 = new ToolStripButton();
      this.tabPage16 = new TabPage();
      this.listViewNF14 = new Form1.ListViewNF();
      this.columnHeader117 = new ColumnHeader();
      this.columnHeader130 = new ColumnHeader();
      this.columnHeader131 = new ColumnHeader();
      this.columnHeader132 = new ColumnHeader();
      this.columnHeader133 = new ColumnHeader();
      this.columnHeader134 = new ColumnHeader();
      this.columnHeader135 = new ColumnHeader();
      this.toolStrip16 = new ToolStrip();
      this.toolStripButton59 = new ToolStripButton();
      this.toolStripButton60 = new ToolStripButton();
      this.toolStripSeparator15 = new ToolStripSeparator();
      this.toolStripButton61 = new ToolStripButton();
      this.toolStripButton62 = new ToolStripButton();
      this.tabPage17 = new TabPage();
      this.listViewNF15 = new Form1.ListViewNF();
      this.columnHeader136 = new ColumnHeader();
      this.columnHeader137 = new ColumnHeader();
      this.columnHeader138 = new ColumnHeader();
      this.columnHeader139 = new ColumnHeader();
      this.columnHeader140 = new ColumnHeader();
      this.columnHeader141 = new ColumnHeader();
      this.columnHeader142 = new ColumnHeader();
      this.columnHeader143 = new ColumnHeader();
      this.columnHeader144 = new ColumnHeader();
      this.columnHeader146 = new ColumnHeader();
      this.columnHeader147 = new ColumnHeader();
      this.columnHeader148 = new ColumnHeader();
      this.columnHeader149 = new ColumnHeader();
      this.columnHeader150 = new ColumnHeader();
      this.columnHeader151 = new ColumnHeader();
      this.columnHeader152 = new ColumnHeader();
      this.columnHeader153 = new ColumnHeader();
      this.columnHeader154 = new ColumnHeader();
      this.columnHeader155 = new ColumnHeader();
      this.toolStrip17 = new ToolStrip();
      this.toolStripButton63 = new ToolStripButton();
      this.toolStripButton64 = new ToolStripButton();
      this.toolStripSeparator16 = new ToolStripSeparator();
      this.toolStripButton65 = new ToolStripButton();
      this.toolStripButton66 = new ToolStripButton();
      this.tabPage18 = new TabPage();
      this.listViewNF16 = new Form1.ListViewNF();
      this.columnHeader145 = new ColumnHeader();
      this.columnHeader156 = new ColumnHeader();
      this.columnHeader157 = new ColumnHeader();
      this.columnHeader158 = new ColumnHeader();
      this.columnHeader159 = new ColumnHeader();
      this.columnHeader160 = new ColumnHeader();
      this.columnHeader161 = new ColumnHeader();
      this.columnHeader162 = new ColumnHeader();
      this.columnHeader163 = new ColumnHeader();
      this.columnHeader164 = new ColumnHeader();
      this.columnHeader165 = new ColumnHeader();
      this.columnHeader166 = new ColumnHeader();
      this.columnHeader167 = new ColumnHeader();
      this.columnHeader168 = new ColumnHeader();
      this.columnHeader169 = new ColumnHeader();
      this.columnHeader170 = new ColumnHeader();
      this.columnHeader171 = new ColumnHeader();
      this.columnHeader172 = new ColumnHeader();
      this.columnHeader173 = new ColumnHeader();
      this.columnHeader174 = new ColumnHeader();
      this.columnHeader175 = new ColumnHeader();
      this.toolStrip18 = new ToolStrip();
      this.toolStripButton67 = new ToolStripButton();
      this.toolStripButton68 = new ToolStripButton();
      this.toolStripSeparator17 = new ToolStripSeparator();
      this.toolStripButton69 = new ToolStripButton();
      this.toolStripButton70 = new ToolStripButton();
      this.tabPage19 = new TabPage();
      this.listViewNF17 = new Form1.ListViewNF();
      this.columnHeader176 = new ColumnHeader();
      this.columnHeader177 = new ColumnHeader();
      this.columnHeader178 = new ColumnHeader();
      this.columnHeader179 = new ColumnHeader();
      this.columnHeader180 = new ColumnHeader();
      this.columnHeader181 = new ColumnHeader();
      this.columnHeader182 = new ColumnHeader();
      this.columnHeader183 = new ColumnHeader();
      this.columnHeader184 = new ColumnHeader();
      this.columnHeader185 = new ColumnHeader();
      this.columnHeader186 = new ColumnHeader();
      this.columnHeader187 = new ColumnHeader();
      this.columnHeader188 = new ColumnHeader();
      this.columnHeader189 = new ColumnHeader();
      this.columnHeader190 = new ColumnHeader();
      this.columnHeader191 = new ColumnHeader();
      this.columnHeader199 = new ColumnHeader();
      this.columnHeader192 = new ColumnHeader();
      this.columnHeader193 = new ColumnHeader();
      this.columnHeader194 = new ColumnHeader();
      this.columnHeader195 = new ColumnHeader();
      this.columnHeader196 = new ColumnHeader();
      this.columnHeader197 = new ColumnHeader();
      this.columnHeader198 = new ColumnHeader();
      this.toolStrip19 = new ToolStrip();
      this.toolStripButton71 = new ToolStripButton();
      this.toolStripButton72 = new ToolStripButton();
      this.toolStripSeparator18 = new ToolStripSeparator();
      this.toolStripButton73 = new ToolStripButton();
      this.toolStripButton74 = new ToolStripButton();
      this.tabPage21 = new TabPage();
      this.textBox3 = new TextBox();
      this.label13 = new Label();
      this.label12 = new Label();
      this.label11 = new Label();
      this.textBox2 = new TextBox();
      this.label10 = new Label();
      this.textBox1 = new TextBox();
      this.label9 = new Label();
      this.colorDialog1 = new ColorDialog();
      this.statusStrip1 = new StatusStrip();
      this.toolStripStatusLabel1 = new ToolStripStatusLabel();
      this.toolStripButton79 = new ToolStripButton();
      this.checkBox1 = new CheckBox();
      this.label14 = new Label();
      this.label15 = new Label();
      this.comboBox2 = new ComboBox();
      this.comboBox3 = new ComboBox();
      this.toolStrip1.SuspendLayout();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tabPage1.SuspendLayout();
      this.toolStrip5.SuspendLayout();
      this.tabPage2.SuspendLayout();
      this.toolStrip2.SuspendLayout();
      this.tabPage3.SuspendLayout();
      this.toolStrip3.SuspendLayout();
      this.tabPage4.SuspendLayout();
      this.toolStrip4.SuspendLayout();
      this.tabPage5.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.numericUpDown2.BeginInit();
      this.numericUpDown1.BeginInit();
      this.groupBox1.SuspendLayout();
      this.tabPage6.SuspendLayout();
      this.toolStrip6.SuspendLayout();
      this.tabPage7.SuspendLayout();
      this.toolStrip7.SuspendLayout();
      this.tabPage20.SuspendLayout();
      this.toolStrip20.SuspendLayout();
      this.tabPage8.SuspendLayout();
      this.toolStrip8.SuspendLayout();
      this.tabPage9.SuspendLayout();
      this.toolStrip9.SuspendLayout();
      this.tabPage10.SuspendLayout();
      this.toolStrip10.SuspendLayout();
      this.tabPage11.SuspendLayout();
      this.toolStrip11.SuspendLayout();
      this.tabPage12.SuspendLayout();
      this.toolStrip12.SuspendLayout();
      this.tabPage13.SuspendLayout();
      this.toolStrip13.SuspendLayout();
      this.tabPage14.SuspendLayout();
      this.toolStrip14.SuspendLayout();
      this.tabPage15.SuspendLayout();
      this.toolStrip15.SuspendLayout();
      this.tabPage16.SuspendLayout();
      this.toolStrip16.SuspendLayout();
      this.tabPage17.SuspendLayout();
      this.toolStrip17.SuspendLayout();
      this.tabPage18.SuspendLayout();
      this.toolStrip18.SuspendLayout();
      this.tabPage19.SuspendLayout();
      this.toolStrip19.SuspendLayout();
      this.tabPage21.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(739, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripButton2.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.tabControl1);
      this.splitContainer1.Size = new Size(739, 332);
      this.splitContainer1.SplitterDistance = 177;
      this.splitContainer1.TabIndex = 1;
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(177, 332);
      this.propertyGrid1.TabIndex = 0;
      this.propertyGrid1.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
      this.tabControl1.Controls.Add((Control) this.tabPage1);
      this.tabControl1.Controls.Add((Control) this.tabPage2);
      this.tabControl1.Controls.Add((Control) this.tabPage3);
      this.tabControl1.Controls.Add((Control) this.tabPage4);
      this.tabControl1.Controls.Add((Control) this.tabPage5);
      this.tabControl1.Controls.Add((Control) this.tabPage6);
      this.tabControl1.Controls.Add((Control) this.tabPage7);
      this.tabControl1.Controls.Add((Control) this.tabPage20);
      this.tabControl1.Controls.Add((Control) this.tabPage8);
      this.tabControl1.Controls.Add((Control) this.tabPage9);
      this.tabControl1.Controls.Add((Control) this.tabPage10);
      this.tabControl1.Controls.Add((Control) this.tabPage11);
      this.tabControl1.Controls.Add((Control) this.tabPage12);
      this.tabControl1.Controls.Add((Control) this.tabPage13);
      this.tabControl1.Controls.Add((Control) this.tabPage14);
      this.tabControl1.Controls.Add((Control) this.tabPage15);
      this.tabControl1.Controls.Add((Control) this.tabPage16);
      this.tabControl1.Controls.Add((Control) this.tabPage17);
      this.tabControl1.Controls.Add((Control) this.tabPage18);
      this.tabControl1.Controls.Add((Control) this.tabPage19);
      this.tabControl1.Controls.Add((Control) this.tabPage21);
      this.tabControl1.Dock = DockStyle.Fill;
      this.tabControl1.Location = new Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new Size(558, 332);
      this.tabControl1.TabIndex = 0;
      this.tabControl1.Selecting += new TabControlCancelEventHandler(this.tabControl1_Selecting);
      this.tabPage1.Controls.Add((Control) this.simpleOpenGlControl1);
      this.tabPage1.Controls.Add((Control) this.toolStrip5);
      this.tabPage1.Controls.Add((Control) this.vScrollBar1);
      this.tabPage1.Controls.Add((Control) this.hScrollBar1);
      this.tabPage1.Location = new Point(4, 22);
      this.tabPage1.Margin = new Padding(0);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Size = new Size(550, 306);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Map";
      this.tabPage1.UseVisualStyleBackColor = true;
      this.simpleOpenGlControl1.AccumBits = (byte) 0;
      this.simpleOpenGlControl1.AutoCheckErrors = false;
      this.simpleOpenGlControl1.AutoFinish = false;
      this.simpleOpenGlControl1.AutoMakeCurrent = true;
      this.simpleOpenGlControl1.AutoSwapBuffers = true;
      this.simpleOpenGlControl1.BackColor = Color.Black;
      this.simpleOpenGlControl1.ColorBits = (byte) 32;
      this.simpleOpenGlControl1.DepthBits = (byte) 16;
      this.simpleOpenGlControl1.Dock = DockStyle.Fill;
      this.simpleOpenGlControl1.Location = new Point(0, 25);
      this.simpleOpenGlControl1.Name = "simpleOpenGlControl1";
      this.simpleOpenGlControl1.Size = new Size(533, 264);
      this.simpleOpenGlControl1.StencilBits = (byte) 0;
      this.simpleOpenGlControl1.TabIndex = 0;
      this.simpleOpenGlControl1.MouseDown += new MouseEventHandler(this.simpleOpenGlControl1_MouseDown);
      this.simpleOpenGlControl1.MouseMove += new MouseEventHandler(this.simpleOpenGlControl1_MouseMove);
      this.simpleOpenGlControl1.MouseUp += new MouseEventHandler(this.simpleOpenGlControl1_MouseUp);
      this.simpleOpenGlControl1.Resize += new EventHandler(this.simpleOpenGlControl1_Resize);
      this.toolStrip5.Items.AddRange(new ToolStripItem[7]
      {
        (ToolStripItem) this.toolStripButton75,
        (ToolStripItem) this.toolStripSeparator19,
        (ToolStripItem) this.toolStripButton76,
        (ToolStripItem) this.toolStripButton77,
        (ToolStripItem) this.toolStripSeparator20,
        (ToolStripItem) this.toolStripButton78,
        (ToolStripItem) this.toolStripSplitButton1
      });
      this.toolStrip5.Location = new Point(0, 0);
      this.toolStrip5.Name = "toolStrip5";
      this.toolStrip5.Size = new Size(533, 25);
      this.toolStrip5.TabIndex = 3;
      this.toolStrip5.Text = "toolStrip5";
      this.toolStripButton75.Checked = true;
      this.toolStripButton75.CheckState = CheckState.Checked;
      this.toolStripButton75.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton75.Image = (Image) componentResourceManager.GetObject("toolStripButton75.Image");
      this.toolStripButton75.ImageTransparentColor = Color.Magenta;
      this.toolStripButton75.Name = "toolStripButton75";
      this.toolStripButton75.Size = new Size(23, 22);
      this.toolStripButton75.Text = "Selection Mode";
      this.toolStripButton75.Click += new EventHandler(this.toolStripButton75_Click);
      this.toolStripSeparator19.Name = "toolStripSeparator19";
      this.toolStripSeparator19.Size = new Size(6, 25);
      this.toolStripButton76.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton76.Image = (Image) componentResourceManager.GetObject("toolStripButton76.Image");
      this.toolStripButton76.ImageTransparentColor = Color.Magenta;
      this.toolStripButton76.Name = "toolStripButton76";
      this.toolStripButton76.Size = new Size(23, 22);
      this.toolStripButton76.Text = "OBJI Drawmode";
      this.toolStripButton76.Click += new EventHandler(this.toolStripButton76_Click);
      this.toolStripButton77.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton77.Enabled = false;
      this.toolStripButton77.Image = (Image) componentResourceManager.GetObject("toolStripButton77.Image");
      this.toolStripButton77.ImageTransparentColor = Color.Magenta;
      this.toolStripButton77.Name = "toolStripButton77";
      this.toolStripButton77.Size = new Size(23, 22);
      this.toolStripButton77.Text = "PATH Drawmode";
      this.toolStripSeparator20.Name = "toolStripSeparator20";
      this.toolStripSeparator20.Size = new Size(6, 25);
      this.toolStripButton78.CheckOnClick = true;
      this.toolStripButton78.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripButton78.Image = (Image) componentResourceManager.GetObject("toolStripButton78.Image");
      this.toolStripButton78.ImageTransparentColor = Color.Magenta;
      this.toolStripButton78.Name = "toolStripButton78";
      this.toolStripButton78.Size = new Size(142, 22);
      this.toolStripButton78.Text = "Auto Y (Drawmode only)";
      this.toolStripSplitButton1.Alignment = ToolStripItemAlignment.Right;
      this.toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
      this.toolStripSplitButton1.DropDownItems.AddRange(new ToolStripItem[12]
      {
        (ToolStripItem) this.oBJIToolStripMenuItem,
        (ToolStripItem) this.pOITToolStripMenuItem,
        (ToolStripItem) this.kTPSToolStripMenuItem,
        (ToolStripItem) this.kTPJToolStripMenuItem,
        (ToolStripItem) this.kTP2ToolStripMenuItem,
        (ToolStripItem) this.kTPCToolStripMenuItem,
        (ToolStripItem) this.kTPMToolStripMenuItem,
        (ToolStripItem) this.cPOIToolStripMenuItem,
        (ToolStripItem) this.iPOIToolStripMenuItem,
        (ToolStripItem) this.ePOIToolStripMenuItem,
        (ToolStripItem) this.aREAToolStripMenuItem,
        (ToolStripItem) this.cAMEToolStripMenuItem
      });
      this.toolStripSplitButton1.Image = (Image) componentResourceManager.GetObject("toolStripSplitButton1.Image");
      this.toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripSplitButton1.Name = "toolStripSplitButton1";
      this.toolStripSplitButton1.Size = new Size(62, 22);
      this.toolStripSplitButton1.Text = "Legend";
      this.toolStripSplitButton1.ToolTipText = "Legend";
      this.oBJIToolStripMenuItem.Checked = true;
      this.oBJIToolStripMenuItem.CheckOnClick = true;
      this.oBJIToolStripMenuItem.CheckState = CheckState.Checked;
      this.oBJIToolStripMenuItem.Image = (Image) componentResourceManager.GetObject("oBJIToolStripMenuItem.Image");
      this.oBJIToolStripMenuItem.Name = "oBJIToolStripMenuItem";
      this.oBJIToolStripMenuItem.Size = new Size(143, 22);
      this.oBJIToolStripMenuItem.Text = "OBJI";
      this.oBJIToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.pOITToolStripMenuItem.Checked = true;
      this.pOITToolStripMenuItem.CheckOnClick = true;
      this.pOITToolStripMenuItem.CheckState = CheckState.Checked;
      this.pOITToolStripMenuItem.Image = (Image) componentResourceManager.GetObject("pOITToolStripMenuItem.Image");
      this.pOITToolStripMenuItem.Name = "pOITToolStripMenuItem";
      this.pOITToolStripMenuItem.Size = new Size(143, 22);
      this.pOITToolStripMenuItem.Text = "POIT";
      this.pOITToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.kTPSToolStripMenuItem.Checked = true;
      this.kTPSToolStripMenuItem.CheckOnClick = true;
      this.kTPSToolStripMenuItem.CheckState = CheckState.Checked;
      this.kTPSToolStripMenuItem.Image = (Image) componentResourceManager.GetObject("kTPSToolStripMenuItem.Image");
      this.kTPSToolStripMenuItem.Name = "kTPSToolStripMenuItem";
      this.kTPSToolStripMenuItem.Size = new Size(143, 22);
      this.kTPSToolStripMenuItem.Text = "KTPS";
      this.kTPSToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.kTPJToolStripMenuItem.Checked = true;
      this.kTPJToolStripMenuItem.CheckOnClick = true;
      this.kTPJToolStripMenuItem.CheckState = CheckState.Checked;
      this.kTPJToolStripMenuItem.Image = (Image) componentResourceManager.GetObject("kTPJToolStripMenuItem.Image");
      this.kTPJToolStripMenuItem.Name = "kTPJToolStripMenuItem";
      this.kTPJToolStripMenuItem.Size = new Size(143, 22);
      this.kTPJToolStripMenuItem.Text = "KTPJ";
      this.kTPJToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.kTP2ToolStripMenuItem.Checked = true;
      this.kTP2ToolStripMenuItem.CheckOnClick = true;
      this.kTP2ToolStripMenuItem.CheckState = CheckState.Checked;
      this.kTP2ToolStripMenuItem.Image = (Image) componentResourceManager.GetObject("kTP2ToolStripMenuItem.Image");
      this.kTP2ToolStripMenuItem.Name = "kTP2ToolStripMenuItem";
      this.kTP2ToolStripMenuItem.Size = new Size(143, 22);
      this.kTP2ToolStripMenuItem.Text = "KTP2";
      this.kTP2ToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.kTPCToolStripMenuItem.Checked = true;
      this.kTPCToolStripMenuItem.CheckOnClick = true;
      this.kTPCToolStripMenuItem.CheckState = CheckState.Checked;
      this.kTPCToolStripMenuItem.Name = "kTPCToolStripMenuItem";
      this.kTPCToolStripMenuItem.Size = new Size(143, 22);
      this.kTPCToolStripMenuItem.Text = "KTPC";
      this.kTPCToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.kTPMToolStripMenuItem.Checked = true;
      this.kTPMToolStripMenuItem.CheckOnClick = true;
      this.kTPMToolStripMenuItem.CheckState = CheckState.Checked;
      this.kTPMToolStripMenuItem.Name = "kTPMToolStripMenuItem";
      this.kTPMToolStripMenuItem.Size = new Size(143, 22);
      this.kTPMToolStripMenuItem.Text = "KTPM";
      this.kTPMToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.cPOIToolStripMenuItem.Checked = true;
      this.cPOIToolStripMenuItem.CheckOnClick = true;
      this.cPOIToolStripMenuItem.CheckState = CheckState.Checked;
      this.cPOIToolStripMenuItem.Name = "cPOIToolStripMenuItem";
      this.cPOIToolStripMenuItem.Size = new Size(143, 22);
      this.cPOIToolStripMenuItem.Text = "CPOI";
      this.cPOIToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.iPOIToolStripMenuItem.Checked = true;
      this.iPOIToolStripMenuItem.CheckOnClick = true;
      this.iPOIToolStripMenuItem.CheckState = CheckState.Checked;
      this.iPOIToolStripMenuItem.Name = "iPOIToolStripMenuItem";
      this.iPOIToolStripMenuItem.Size = new Size(143, 22);
      this.iPOIToolStripMenuItem.Text = "IPOI";
      this.iPOIToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.ePOIToolStripMenuItem.Checked = true;
      this.ePOIToolStripMenuItem.CheckOnClick = true;
      this.ePOIToolStripMenuItem.CheckState = CheckState.Checked;
      this.ePOIToolStripMenuItem.Name = "ePOIToolStripMenuItem";
      this.ePOIToolStripMenuItem.Size = new Size(143, 22);
      this.ePOIToolStripMenuItem.Text = "EPOI / MEPO";
      this.ePOIToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.aREAToolStripMenuItem.Checked = true;
      this.aREAToolStripMenuItem.CheckOnClick = true;
      this.aREAToolStripMenuItem.CheckState = CheckState.Checked;
      this.aREAToolStripMenuItem.Name = "aREAToolStripMenuItem";
      this.aREAToolStripMenuItem.Size = new Size(143, 22);
      this.aREAToolStripMenuItem.Text = "AREA";
      this.aREAToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.cAMEToolStripMenuItem.Checked = true;
      this.cAMEToolStripMenuItem.CheckOnClick = true;
      this.cAMEToolStripMenuItem.CheckState = CheckState.Checked;
      this.cAMEToolStripMenuItem.Name = "cAMEToolStripMenuItem";
      this.cAMEToolStripMenuItem.Size = new Size(143, 22);
      this.cAMEToolStripMenuItem.Text = "CAME";
      this.cAMEToolStripMenuItem.CheckedChanged += new EventHandler(this.oBJIToolStripMenuItem_CheckedChanged);
      this.vScrollBar1.Dock = DockStyle.Right;
      this.vScrollBar1.Enabled = false;
      this.vScrollBar1.LargeChange = 1;
      this.vScrollBar1.Location = new Point(533, 0);
      this.vScrollBar1.Maximum = 0;
      this.vScrollBar1.Name = "vScrollBar1";
      this.vScrollBar1.Size = new Size(17, 289);
      this.vScrollBar1.TabIndex = 2;
      this.vScrollBar1.Scroll += new ScrollEventHandler(this.vScrollBar1_Scroll);
      this.hScrollBar1.Dock = DockStyle.Bottom;
      this.hScrollBar1.Enabled = false;
      this.hScrollBar1.LargeChange = 1;
      this.hScrollBar1.Location = new Point(0, 289);
      this.hScrollBar1.Maximum = 0;
      this.hScrollBar1.Name = "hScrollBar1";
      this.hScrollBar1.Size = new Size(550, 17);
      this.hScrollBar1.TabIndex = 1;
      this.hScrollBar1.Scroll += new ScrollEventHandler(this.vScrollBar1_Scroll);
      this.tabPage2.Controls.Add((Control) this.listView1);
      this.tabPage2.Controls.Add((Control) this.toolStrip2);
      this.tabPage2.Location = new Point(4, 22);
      this.tabPage2.Margin = new Padding(0);
      this.tabPage2.Name = "tabPage2";
      this.tabPage2.Size = new Size(550, 306);
      this.tabPage2.TabIndex = 1;
      this.tabPage2.Text = "OBJI";
      this.tabPage2.UseVisualStyleBackColor = true;
      this.listView1.Columns.AddRange(new ColumnHeader[17]
      {
        this.columnHeader1,
        this.columnHeader2,
        this.columnHeader3,
        this.columnHeader4,
        this.columnHeader5,
        this.columnHeader6,
        this.columnHeader7,
        this.columnHeader8,
        this.columnHeader9,
        this.columnHeader10,
        this.columnHeader11,
        this.columnHeader12,
        this.columnHeader13,
        this.columnHeader14,
        this.columnHeader15,
        this.columnHeader16,
        this.columnHeader17
      });
      this.listView1.Dock = DockStyle.Fill;
      this.listView1.FullRowSelect = true;
      this.listView1.GridLines = true;
      this.listView1.HideSelection = false;
      this.listView1.Location = new Point(0, 25);
      this.listView1.Name = "listView1";
      this.listView1.Size = new Size(550, 281);
      this.listView1.TabIndex = 1;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Details;
      this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader1.Text = "ID";
      this.columnHeader2.Text = "X";
      this.columnHeader3.Text = "Y";
      this.columnHeader4.Text = "Z";
      this.columnHeader5.Text = "X Angle";
      this.columnHeader6.Text = "Y Angle";
      this.columnHeader7.Text = "Z Angle";
      this.columnHeader8.Text = "X Scale";
      this.columnHeader9.Text = "Y Scale";
      this.columnHeader10.Text = "Z Scale";
      this.columnHeader11.Text = "Object ID";
      this.columnHeader12.Text = "Route ID";
      this.columnHeader13.Text = "?";
      this.columnHeader14.Text = "?";
      this.columnHeader15.Text = "?";
      this.columnHeader16.Text = "?";
      this.columnHeader17.Text = "Show in TT";
      this.toolStrip2.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripButton4,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton5,
        (ToolStripItem) this.toolStripButton6
      });
      this.toolStrip2.Location = new Point(0, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new Size(550, 25);
      this.toolStrip2.TabIndex = 0;
      this.toolStrip2.Text = "toolStrip2";
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = (Image) Resources.plus;
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton4.Image = (Image) Resources.minus;
      this.toolStripButton4.ImageTransparentColor = Color.Magenta;
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.Size = new Size(23, 22);
      this.toolStripButton4.Text = "toolStripButton4";
      this.toolStripButton4.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton5.Image = (Image) Resources.arrow_090;
      this.toolStripButton5.ImageTransparentColor = Color.Magenta;
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.Size = new Size(23, 22);
      this.toolStripButton5.Text = "toolStripButton5";
      this.toolStripButton5.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton6.Image = (Image) Resources.arrow_270;
      this.toolStripButton6.ImageTransparentColor = Color.Magenta;
      this.toolStripButton6.Name = "toolStripButton6";
      this.toolStripButton6.Size = new Size(23, 22);
      this.toolStripButton6.Text = "toolStripButton6";
      this.toolStripButton6.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage3.Controls.Add((Control) this.listViewNF1);
      this.tabPage3.Controls.Add((Control) this.toolStrip3);
      this.tabPage3.Location = new Point(4, 22);
      this.tabPage3.Name = "tabPage3";
      this.tabPage3.Size = new Size(550, 306);
      this.tabPage3.TabIndex = 2;
      this.tabPage3.Text = "PATH";
      this.tabPage3.UseVisualStyleBackColor = true;
      this.listViewNF1.Columns.AddRange(new ColumnHeader[4]
      {
        this.columnHeader18,
        this.columnHeader19,
        this.columnHeader20,
        this.columnHeader21
      });
      this.listViewNF1.Dock = DockStyle.Fill;
      this.listViewNF1.FullRowSelect = true;
      this.listViewNF1.GridLines = true;
      this.listViewNF1.HideSelection = false;
      this.listViewNF1.Location = new Point(0, 25);
      this.listViewNF1.Name = "listViewNF1";
      this.listViewNF1.Size = new Size(550, 281);
      this.listViewNF1.TabIndex = 2;
      this.listViewNF1.UseCompatibleStateImageBehavior = false;
      this.listViewNF1.View = View.Details;
      this.listViewNF1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader18.Text = "ID";
      this.columnHeader19.Text = "Index";
      this.columnHeader20.Text = "Loop";
      this.columnHeader21.Text = "Nr Poit";
      this.toolStrip3.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton7,
        (ToolStripItem) this.toolStripButton8,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton9,
        (ToolStripItem) this.toolStripButton10
      });
      this.toolStrip3.Location = new Point(0, 0);
      this.toolStrip3.Name = "toolStrip3";
      this.toolStrip3.Size = new Size(550, 25);
      this.toolStrip3.TabIndex = 0;
      this.toolStrip3.Text = "toolStrip3";
      this.toolStripButton7.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton7.Image = (Image) Resources.plus;
      this.toolStripButton7.ImageTransparentColor = Color.Magenta;
      this.toolStripButton7.Name = "toolStripButton7";
      this.toolStripButton7.Size = new Size(23, 22);
      this.toolStripButton7.Text = "toolStripButton3";
      this.toolStripButton7.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton8.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton8.Image = (Image) Resources.minus;
      this.toolStripButton8.ImageTransparentColor = Color.Magenta;
      this.toolStripButton8.Name = "toolStripButton8";
      this.toolStripButton8.Size = new Size(23, 22);
      this.toolStripButton8.Text = "toolStripButton4";
      this.toolStripButton8.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
      this.toolStripButton9.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton9.Image = (Image) Resources.arrow_090;
      this.toolStripButton9.ImageTransparentColor = Color.Magenta;
      this.toolStripButton9.Name = "toolStripButton9";
      this.toolStripButton9.Size = new Size(23, 22);
      this.toolStripButton9.Text = "toolStripButton5";
      this.toolStripButton9.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton10.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton10.Image = (Image) Resources.arrow_270;
      this.toolStripButton10.ImageTransparentColor = Color.Magenta;
      this.toolStripButton10.Name = "toolStripButton10";
      this.toolStripButton10.Size = new Size(23, 22);
      this.toolStripButton10.Text = "toolStripButton6";
      this.toolStripButton10.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage4.Controls.Add((Control) this.listViewNF2);
      this.tabPage4.Controls.Add((Control) this.toolStrip4);
      this.tabPage4.Location = new Point(4, 22);
      this.tabPage4.Name = "tabPage4";
      this.tabPage4.Size = new Size(550, 306);
      this.tabPage4.TabIndex = 3;
      this.tabPage4.Text = "POIT";
      this.tabPage4.UseVisualStyleBackColor = true;
      this.listViewNF2.Columns.AddRange(new ColumnHeader[7]
      {
        this.columnHeader22,
        this.columnHeader23,
        this.columnHeader24,
        this.columnHeader25,
        this.columnHeader26,
        this.columnHeader27,
        this.columnHeader28
      });
      this.listViewNF2.Dock = DockStyle.Fill;
      this.listViewNF2.FullRowSelect = true;
      this.listViewNF2.GridLines = true;
      this.listViewNF2.HideSelection = false;
      this.listViewNF2.Location = new Point(0, 25);
      this.listViewNF2.Name = "listViewNF2";
      this.listViewNF2.Size = new Size(550, 281);
      this.listViewNF2.TabIndex = 3;
      this.listViewNF2.UseCompatibleStateImageBehavior = false;
      this.listViewNF2.View = View.Details;
      this.listViewNF2.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader22.Text = "ID";
      this.columnHeader23.Text = "X";
      this.columnHeader24.Text = "Y";
      this.columnHeader25.Text = "Z";
      this.columnHeader26.Text = "Index";
      this.columnHeader27.Text = "Duration";
      this.columnHeader28.Text = "?";
      this.toolStrip4.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton11,
        (ToolStripItem) this.toolStripButton12,
        (ToolStripItem) this.toolStripSeparator3,
        (ToolStripItem) this.toolStripButton13,
        (ToolStripItem) this.toolStripButton14
      });
      this.toolStrip4.Location = new Point(0, 0);
      this.toolStrip4.Name = "toolStrip4";
      this.toolStrip4.Size = new Size(550, 25);
      this.toolStrip4.TabIndex = 0;
      this.toolStrip4.Text = "toolStrip4";
      this.toolStripButton11.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton11.Image = (Image) Resources.plus;
      this.toolStripButton11.ImageTransparentColor = Color.Magenta;
      this.toolStripButton11.Name = "toolStripButton11";
      this.toolStripButton11.Size = new Size(23, 22);
      this.toolStripButton11.Text = "toolStripButton3";
      this.toolStripButton11.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton12.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton12.Image = (Image) Resources.minus;
      this.toolStripButton12.ImageTransparentColor = Color.Magenta;
      this.toolStripButton12.Name = "toolStripButton12";
      this.toolStripButton12.Size = new Size(23, 22);
      this.toolStripButton12.Text = "toolStripButton4";
      this.toolStripButton12.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new Size(6, 25);
      this.toolStripButton13.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton13.Image = (Image) Resources.arrow_090;
      this.toolStripButton13.ImageTransparentColor = Color.Magenta;
      this.toolStripButton13.Name = "toolStripButton13";
      this.toolStripButton13.Size = new Size(23, 22);
      this.toolStripButton13.Text = "toolStripButton5";
      this.toolStripButton13.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton14.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton14.Image = (Image) Resources.arrow_270;
      this.toolStripButton14.ImageTransparentColor = Color.Magenta;
      this.toolStripButton14.Name = "toolStripButton14";
      this.toolStripButton14.Size = new Size(23, 22);
      this.toolStripButton14.Text = "toolStripButton6";
      this.toolStripButton14.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage5.Controls.Add((Control) this.groupBox2);
      this.tabPage5.Controls.Add((Control) this.groupBox1);
      this.tabPage5.Location = new Point(4, 22);
      this.tabPage5.Name = "tabPage5";
      this.tabPage5.Size = new Size(550, 306);
      this.tabPage5.TabIndex = 4;
      this.tabPage5.Text = "STAG";
      this.tabPage5.UseVisualStyleBackColor = true;
      this.groupBox2.Controls.Add((Control) this.comboBox3);
      this.groupBox2.Controls.Add((Control) this.comboBox2);
      this.groupBox2.Controls.Add((Control) this.label15);
      this.groupBox2.Controls.Add((Control) this.label14);
      this.groupBox2.Controls.Add((Control) this.checkBox1);
      this.groupBox2.Controls.Add((Control) this.numericUpDown2);
      this.groupBox2.Controls.Add((Control) this.label8);
      this.groupBox2.Controls.Add((Control) this.label7);
      this.groupBox2.Controls.Add((Control) this.numericUpDown1);
      this.groupBox2.Controls.Add((Control) this.label6);
      this.groupBox2.Controls.Add((Control) this.button5);
      this.groupBox2.Controls.Add((Control) this.panel5);
      this.groupBox2.Location = new Point(270, 3);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(272, 180);
      this.groupBox2.TabIndex = 3;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Fog";
      this.numericUpDown2.Location = new Point(145, 144);
      this.numericUpDown2.Maximum = new Decimal(new int[4]
      {
        31,
        0,
        0,
        0
      });
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new Size(121, 20);
      this.numericUpDown2.TabIndex = 20;
      this.numericUpDown2.ValueChanged += new EventHandler(this.numericUpDown2_ValueChanged);
      this.label8.AutoSize = true;
      this.label8.Location = new Point(105, 146);
      this.label8.Name = "label8";
      this.label8.Size = new Size(34, 13);
      this.label8.TabIndex = 19;
      this.label8.Text = "Alpha";
      this.label7.AutoSize = true;
      this.label7.Location = new Point(97, 96);
      this.label7.MinimumSize = new Size(0, 16);
      this.label7.Name = "label7";
      this.label7.Size = new Size(42, 16);
      this.label7.TabIndex = 18;
      this.label7.Text = "Density";
      this.label7.TextAlign = ContentAlignment.MiddleCenter;
      this.numericUpDown1.DecimalPlaces = 12;
      this.numericUpDown1.Location = new Point(145, 96);
      this.numericUpDown1.Maximum = new Decimal(new int[4]
      {
        8192,
        0,
        0,
        0
      });
      this.numericUpDown1.Minimum = new Decimal(new int[4]
      {
        8192,
        0,
        0,
        int.MinValue
      });
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new Size(121, 20);
      this.numericUpDown1.TabIndex = 17;
      this.numericUpDown1.ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
      this.label6.AutoSize = true;
      this.label6.Location = new Point(108, 122);
      this.label6.MinimumSize = new Size(0, 16);
      this.label6.Name = "label6";
      this.label6.Size = new Size(31, 16);
      this.label6.TabIndex = 14;
      this.label6.Text = "Color";
      this.label6.TextAlign = ContentAlignment.MiddleCenter;
      this.button5.FlatStyle = FlatStyle.System;
      this.button5.Location = new Point(167, 122);
      this.button5.Name = "button5";
      this.button5.Size = new Size(99, 16);
      this.button5.TabIndex = 16;
      this.button5.Text = "Change";
      this.button5.TextAlign = ContentAlignment.BottomCenter;
      this.button5.UseVisualStyleBackColor = true;
      this.button5.Click += new EventHandler(this.button5_Click);
      this.panel5.BorderStyle = BorderStyle.FixedSingle;
      this.panel5.Location = new Point(145, 122);
      this.panel5.Name = "panel5";
      this.panel5.Size = new Size(16, 16);
      this.panel5.TabIndex = 15;
      this.groupBox1.Controls.Add((Control) this.button4);
      this.groupBox1.Controls.Add((Control) this.panel4);
      this.groupBox1.Controls.Add((Control) this.label5);
      this.groupBox1.Controls.Add((Control) this.button3);
      this.groupBox1.Controls.Add((Control) this.panel3);
      this.groupBox1.Controls.Add((Control) this.label4);
      this.groupBox1.Controls.Add((Control) this.button2);
      this.groupBox1.Controls.Add((Control) this.panel2);
      this.groupBox1.Controls.Add((Control) this.label3);
      this.groupBox1.Controls.Add((Control) this.button1);
      this.groupBox1.Controls.Add((Control) this.panel1);
      this.groupBox1.Controls.Add((Control) this.label2);
      this.groupBox1.Controls.Add((Control) this.label1);
      this.groupBox1.Controls.Add((Control) this.comboBox1);
      this.groupBox1.Location = new Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(261, 180);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Track";
      this.button4.FlatStyle = FlatStyle.System;
      this.button4.Location = new Point(142, 134);
      this.button4.Name = "button4";
      this.button4.Size = new Size(99, 16);
      this.button4.TabIndex = 13;
      this.button4.Text = "Change";
      this.button4.TextAlign = ContentAlignment.BottomCenter;
      this.button4.UseVisualStyleBackColor = true;
      this.button4.Click += new EventHandler(this.button4_Click);
      this.panel4.BorderStyle = BorderStyle.FixedSingle;
      this.panel4.Location = new Point(120, 134);
      this.panel4.Name = "panel4";
      this.panel4.Size = new Size(16, 16);
      this.panel4.TabIndex = 12;
      this.label5.AutoSize = true;
      this.label5.Location = new Point(33, 133);
      this.label5.MinimumSize = new Size(0, 16);
      this.label5.Name = "label5";
      this.label5.Size = new Size(81, 16);
      this.label5.TabIndex = 11;
      this.label5.Text = "Collision Color 4";
      this.label5.TextAlign = ContentAlignment.MiddleCenter;
      this.button3.FlatStyle = FlatStyle.System;
      this.button3.Location = new Point(142, 112);
      this.button3.Name = "button3";
      this.button3.Size = new Size(99, 16);
      this.button3.TabIndex = 10;
      this.button3.Text = "Change";
      this.button3.TextAlign = ContentAlignment.BottomCenter;
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new EventHandler(this.button3_Click);
      this.panel3.BorderStyle = BorderStyle.FixedSingle;
      this.panel3.Location = new Point(120, 112);
      this.panel3.Name = "panel3";
      this.panel3.Size = new Size(16, 16);
      this.panel3.TabIndex = 9;
      this.label4.AutoSize = true;
      this.label4.Location = new Point(33, 112);
      this.label4.MinimumSize = new Size(0, 16);
      this.label4.Name = "label4";
      this.label4.Size = new Size(81, 16);
      this.label4.TabIndex = 8;
      this.label4.Text = "Collision Color 3";
      this.label4.TextAlign = ContentAlignment.MiddleCenter;
      this.button2.FlatStyle = FlatStyle.System;
      this.button2.Location = new Point(142, 90);
      this.button2.Name = "button2";
      this.button2.Size = new Size(99, 16);
      this.button2.TabIndex = 7;
      this.button2.Text = "Change";
      this.button2.TextAlign = ContentAlignment.BottomCenter;
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new EventHandler(this.button2_Click);
      this.panel2.BorderStyle = BorderStyle.FixedSingle;
      this.panel2.Location = new Point(120, 90);
      this.panel2.Name = "panel2";
      this.panel2.Size = new Size(16, 16);
      this.panel2.TabIndex = 6;
      this.label3.AutoSize = true;
      this.label3.Location = new Point(33, 90);
      this.label3.MinimumSize = new Size(0, 16);
      this.label3.Name = "label3";
      this.label3.Size = new Size(81, 16);
      this.label3.TabIndex = 5;
      this.label3.Text = "Collision Color 2";
      this.label3.TextAlign = ContentAlignment.MiddleCenter;
      this.button1.FlatStyle = FlatStyle.System;
      this.button1.Location = new Point(142, 68);
      this.button1.Name = "button1";
      this.button1.Size = new Size(99, 16);
      this.button1.TabIndex = 4;
      this.button1.Text = "Change";
      this.button1.TextAlign = ContentAlignment.BottomCenter;
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.panel1.BorderStyle = BorderStyle.FixedSingle;
      this.panel1.Location = new Point(120, 68);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(16, 16);
      this.panel1.TabIndex = 3;
      this.label2.AutoSize = true;
      this.label2.Location = new Point(33, 68);
      this.label2.MinimumSize = new Size(0, 16);
      this.label2.Name = "label2";
      this.label2.Size = new Size(81, 16);
      this.label2.TabIndex = 2;
      this.label2.Text = "Collision Color 1";
      this.label2.TextAlign = ContentAlignment.MiddleCenter;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(6, 22);
      this.label1.Name = "label1";
      this.label1.Size = new Size(108, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Nr Laps / Battle Time";
      this.comboBox1.FormatString = "N0";
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[11]
      {
        (object) "1",
        (object) "2",
        (object) "3",
        (object) "4",
        (object) "5",
        (object) "6",
        (object) "30",
        (object) "60",
        (object) "90",
        (object) "120",
        (object) "180"
      });
      this.comboBox1.Location = new Point(120, 19);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new Size(121, 21);
      this.comboBox1.TabIndex = 0;
      this.comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_SelectedIndexChanged);
      this.tabPage6.Controls.Add((Control) this.listViewNF3);
      this.tabPage6.Controls.Add((Control) this.toolStrip6);
      this.tabPage6.Location = new Point(4, 22);
      this.tabPage6.Name = "tabPage6";
      this.tabPage6.Size = new Size(550, 306);
      this.tabPage6.TabIndex = 5;
      this.tabPage6.Text = "KTPS";
      this.tabPage6.UseVisualStyleBackColor = true;
      this.listViewNF3.Columns.AddRange(new ColumnHeader[9]
      {
        this.columnHeader29,
        this.columnHeader30,
        this.columnHeader31,
        this.columnHeader32,
        this.columnHeader36,
        this.columnHeader37,
        this.columnHeader38,
        this.columnHeader33,
        this.columnHeader35
      });
      this.listViewNF3.Dock = DockStyle.Fill;
      this.listViewNF3.FullRowSelect = true;
      this.listViewNF3.GridLines = true;
      this.listViewNF3.HideSelection = false;
      this.listViewNF3.Location = new Point(0, 25);
      this.listViewNF3.Name = "listViewNF3";
      this.listViewNF3.Size = new Size(550, 281);
      this.listViewNF3.TabIndex = 4;
      this.listViewNF3.UseCompatibleStateImageBehavior = false;
      this.listViewNF3.View = View.Details;
      this.listViewNF3.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader29.Text = "ID";
      this.columnHeader30.Text = "X";
      this.columnHeader31.Text = "Y";
      this.columnHeader32.Text = "Z";
      this.columnHeader36.Text = "X Angle";
      this.columnHeader37.Text = "Y Angle";
      this.columnHeader38.Text = "Z Angle";
      this.columnHeader33.Text = "Padding";
      this.columnHeader35.Text = "Index";
      this.toolStrip6.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton15,
        (ToolStripItem) this.toolStripButton16,
        (ToolStripItem) this.toolStripSeparator4,
        (ToolStripItem) this.toolStripButton17,
        (ToolStripItem) this.toolStripButton18
      });
      this.toolStrip6.Location = new Point(0, 0);
      this.toolStrip6.Name = "toolStrip6";
      this.toolStrip6.Size = new Size(550, 25);
      this.toolStrip6.TabIndex = 0;
      this.toolStrip6.Text = "toolStrip6";
      this.toolStripButton15.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton15.Image = (Image) Resources.plus;
      this.toolStripButton15.ImageTransparentColor = Color.Magenta;
      this.toolStripButton15.Name = "toolStripButton15";
      this.toolStripButton15.Size = new Size(23, 22);
      this.toolStripButton15.Text = "toolStripButton3";
      this.toolStripButton15.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton16.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton16.Image = (Image) Resources.minus;
      this.toolStripButton16.ImageTransparentColor = Color.Magenta;
      this.toolStripButton16.Name = "toolStripButton16";
      this.toolStripButton16.Size = new Size(23, 22);
      this.toolStripButton16.Text = "toolStripButton4";
      this.toolStripButton16.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new Size(6, 25);
      this.toolStripButton17.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton17.Image = (Image) Resources.arrow_090;
      this.toolStripButton17.ImageTransparentColor = Color.Magenta;
      this.toolStripButton17.Name = "toolStripButton17";
      this.toolStripButton17.Size = new Size(23, 22);
      this.toolStripButton17.Text = "toolStripButton5";
      this.toolStripButton17.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton18.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton18.Image = (Image) Resources.arrow_270;
      this.toolStripButton18.ImageTransparentColor = Color.Magenta;
      this.toolStripButton18.Name = "toolStripButton18";
      this.toolStripButton18.Size = new Size(23, 22);
      this.toolStripButton18.Text = "toolStripButton6";
      this.toolStripButton18.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage7.Controls.Add((Control) this.listViewNF4);
      this.tabPage7.Controls.Add((Control) this.toolStrip7);
      this.tabPage7.Location = new Point(4, 22);
      this.tabPage7.Name = "tabPage7";
      this.tabPage7.Size = new Size(550, 306);
      this.tabPage7.TabIndex = 6;
      this.tabPage7.Text = "KTPJ";
      this.tabPage7.UseVisualStyleBackColor = true;
      this.listViewNF4.Columns.AddRange(new ColumnHeader[10]
      {
        this.columnHeader34,
        this.columnHeader39,
        this.columnHeader40,
        this.columnHeader41,
        this.columnHeader42,
        this.columnHeader43,
        this.columnHeader44,
        this.columnHeader45,
        this.columnHeader56,
        this.columnHeader46
      });
      this.listViewNF4.Dock = DockStyle.Fill;
      this.listViewNF4.FullRowSelect = true;
      this.listViewNF4.GridLines = true;
      this.listViewNF4.HideSelection = false;
      this.listViewNF4.Location = new Point(0, 25);
      this.listViewNF4.Name = "listViewNF4";
      this.listViewNF4.Size = new Size(550, 281);
      this.listViewNF4.TabIndex = 5;
      this.listViewNF4.UseCompatibleStateImageBehavior = false;
      this.listViewNF4.View = View.Details;
      this.listViewNF4.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader34.Text = "ID";
      this.columnHeader39.Text = "X";
      this.columnHeader40.Text = "Y";
      this.columnHeader41.Text = "Z";
      this.columnHeader42.Text = "X Angle";
      this.columnHeader43.Text = "Y Angle";
      this.columnHeader44.Text = "Z Angle";
      this.columnHeader45.Text = "Enemy Pos ID";
      this.columnHeader56.Text = "Item Pos ID";
      this.columnHeader46.Text = "Index";
      this.toolStrip7.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton19,
        (ToolStripItem) this.toolStripButton20,
        (ToolStripItem) this.toolStripSeparator5,
        (ToolStripItem) this.toolStripButton21,
        (ToolStripItem) this.toolStripButton22
      });
      this.toolStrip7.Location = new Point(0, 0);
      this.toolStrip7.Name = "toolStrip7";
      this.toolStrip7.Size = new Size(550, 25);
      this.toolStrip7.TabIndex = 0;
      this.toolStrip7.Text = "toolStrip7";
      this.toolStripButton19.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton19.Image = (Image) Resources.plus;
      this.toolStripButton19.ImageTransparentColor = Color.Magenta;
      this.toolStripButton19.Name = "toolStripButton19";
      this.toolStripButton19.Size = new Size(23, 22);
      this.toolStripButton19.Text = "toolStripButton3";
      this.toolStripButton19.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton20.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton20.Image = (Image) Resources.minus;
      this.toolStripButton20.ImageTransparentColor = Color.Magenta;
      this.toolStripButton20.Name = "toolStripButton20";
      this.toolStripButton20.Size = new Size(23, 22);
      this.toolStripButton20.Text = "toolStripButton4";
      this.toolStripButton20.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new Size(6, 25);
      this.toolStripButton21.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton21.Image = (Image) Resources.arrow_090;
      this.toolStripButton21.ImageTransparentColor = Color.Magenta;
      this.toolStripButton21.Name = "toolStripButton21";
      this.toolStripButton21.Size = new Size(23, 22);
      this.toolStripButton21.Text = "toolStripButton5";
      this.toolStripButton21.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton22.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton22.Image = (Image) Resources.arrow_270;
      this.toolStripButton22.ImageTransparentColor = Color.Magenta;
      this.toolStripButton22.Name = "toolStripButton22";
      this.toolStripButton22.Size = new Size(23, 22);
      this.toolStripButton22.Text = "toolStripButton6";
      this.toolStripButton22.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage20.Controls.Add((Control) this.listViewNF5);
      this.tabPage20.Controls.Add((Control) this.toolStrip20);
      this.tabPage20.Location = new Point(4, 22);
      this.tabPage20.Name = "tabPage20";
      this.tabPage20.Size = new Size(550, 306);
      this.tabPage20.TabIndex = 19;
      this.tabPage20.Text = "KTP2";
      this.tabPage20.UseVisualStyleBackColor = true;
      this.listViewNF5.Columns.AddRange(new ColumnHeader[9]
      {
        this.columnHeader47,
        this.columnHeader48,
        this.columnHeader49,
        this.columnHeader50,
        this.columnHeader51,
        this.columnHeader52,
        this.columnHeader53,
        this.columnHeader54,
        this.columnHeader55
      });
      this.listViewNF5.Dock = DockStyle.Fill;
      this.listViewNF5.FullRowSelect = true;
      this.listViewNF5.GridLines = true;
      this.listViewNF5.HideSelection = false;
      this.listViewNF5.Location = new Point(0, 25);
      this.listViewNF5.Name = "listViewNF5";
      this.listViewNF5.Size = new Size(550, 281);
      this.listViewNF5.TabIndex = 5;
      this.listViewNF5.UseCompatibleStateImageBehavior = false;
      this.listViewNF5.View = View.Details;
      this.listViewNF5.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader47.Text = "ID";
      this.columnHeader48.Text = "X";
      this.columnHeader49.Text = "Y";
      this.columnHeader50.Text = "Z";
      this.columnHeader51.Text = "X Angle";
      this.columnHeader52.Text = "Y Angle";
      this.columnHeader53.Text = "Z Angle";
      this.columnHeader54.Text = "Padding";
      this.columnHeader55.Text = "Index";
      this.toolStrip20.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton23,
        (ToolStripItem) this.toolStripButton24,
        (ToolStripItem) this.toolStripSeparator6,
        (ToolStripItem) this.toolStripButton25,
        (ToolStripItem) this.toolStripButton26
      });
      this.toolStrip20.Location = new Point(0, 0);
      this.toolStrip20.Name = "toolStrip20";
      this.toolStrip20.Size = new Size(550, 25);
      this.toolStrip20.TabIndex = 6;
      this.toolStrip20.Text = "toolStrip20";
      this.toolStripButton23.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton23.Image = (Image) Resources.plus;
      this.toolStripButton23.ImageTransparentColor = Color.Magenta;
      this.toolStripButton23.Name = "toolStripButton23";
      this.toolStripButton23.Size = new Size(23, 22);
      this.toolStripButton23.Text = "toolStripButton3";
      this.toolStripButton23.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton24.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton24.Image = (Image) Resources.minus;
      this.toolStripButton24.ImageTransparentColor = Color.Magenta;
      this.toolStripButton24.Name = "toolStripButton24";
      this.toolStripButton24.Size = new Size(23, 22);
      this.toolStripButton24.Text = "toolStripButton4";
      this.toolStripButton24.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new Size(6, 25);
      this.toolStripButton25.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton25.Image = (Image) Resources.arrow_090;
      this.toolStripButton25.ImageTransparentColor = Color.Magenta;
      this.toolStripButton25.Name = "toolStripButton25";
      this.toolStripButton25.Size = new Size(23, 22);
      this.toolStripButton25.Text = "toolStripButton5";
      this.toolStripButton25.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton26.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton26.Image = (Image) Resources.arrow_270;
      this.toolStripButton26.ImageTransparentColor = Color.Magenta;
      this.toolStripButton26.Name = "toolStripButton26";
      this.toolStripButton26.Size = new Size(23, 22);
      this.toolStripButton26.Text = "toolStripButton6";
      this.toolStripButton26.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage8.Controls.Add((Control) this.listViewNF6);
      this.tabPage8.Controls.Add((Control) this.toolStrip8);
      this.tabPage8.Location = new Point(4, 22);
      this.tabPage8.Name = "tabPage8";
      this.tabPage8.Size = new Size(550, 306);
      this.tabPage8.TabIndex = 7;
      this.tabPage8.Text = "KTPC";
      this.tabPage8.UseVisualStyleBackColor = true;
      this.listViewNF6.Columns.AddRange(new ColumnHeader[9]
      {
        this.columnHeader57,
        this.columnHeader58,
        this.columnHeader59,
        this.columnHeader60,
        this.columnHeader61,
        this.columnHeader62,
        this.columnHeader63,
        this.columnHeader64,
        this.columnHeader65
      });
      this.listViewNF6.Dock = DockStyle.Fill;
      this.listViewNF6.FullRowSelect = true;
      this.listViewNF6.GridLines = true;
      this.listViewNF6.HideSelection = false;
      this.listViewNF6.Location = new Point(0, 25);
      this.listViewNF6.Name = "listViewNF6";
      this.listViewNF6.Size = new Size(550, 281);
      this.listViewNF6.TabIndex = 6;
      this.listViewNF6.UseCompatibleStateImageBehavior = false;
      this.listViewNF6.View = View.Details;
      this.listViewNF6.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader57.Text = "ID";
      this.columnHeader58.Text = "X";
      this.columnHeader59.Text = "Y";
      this.columnHeader60.Text = "Z";
      this.columnHeader61.Text = "X Angle";
      this.columnHeader62.Text = "Y Angle";
      this.columnHeader63.Text = "Z Angle";
      this.columnHeader64.Text = "?";
      this.columnHeader65.Text = "Index";
      this.toolStrip8.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton27,
        (ToolStripItem) this.toolStripButton28,
        (ToolStripItem) this.toolStripSeparator7,
        (ToolStripItem) this.toolStripButton29,
        (ToolStripItem) this.toolStripButton30
      });
      this.toolStrip8.Location = new Point(0, 0);
      this.toolStrip8.Name = "toolStrip8";
      this.toolStrip8.Size = new Size(550, 25);
      this.toolStrip8.TabIndex = 0;
      this.toolStrip8.Text = "toolStrip8";
      this.toolStripButton27.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton27.Image = (Image) Resources.plus;
      this.toolStripButton27.ImageTransparentColor = Color.Magenta;
      this.toolStripButton27.Name = "toolStripButton27";
      this.toolStripButton27.Size = new Size(23, 22);
      this.toolStripButton27.Text = "toolStripButton3";
      this.toolStripButton27.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton28.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton28.Image = (Image) Resources.minus;
      this.toolStripButton28.ImageTransparentColor = Color.Magenta;
      this.toolStripButton28.Name = "toolStripButton28";
      this.toolStripButton28.Size = new Size(23, 22);
      this.toolStripButton28.Text = "toolStripButton4";
      this.toolStripButton28.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator7.Name = "toolStripSeparator7";
      this.toolStripSeparator7.Size = new Size(6, 25);
      this.toolStripButton29.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton29.Image = (Image) Resources.arrow_090;
      this.toolStripButton29.ImageTransparentColor = Color.Magenta;
      this.toolStripButton29.Name = "toolStripButton29";
      this.toolStripButton29.Size = new Size(23, 22);
      this.toolStripButton29.Text = "toolStripButton5";
      this.toolStripButton29.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton30.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton30.Image = (Image) Resources.arrow_270;
      this.toolStripButton30.ImageTransparentColor = Color.Magenta;
      this.toolStripButton30.Name = "toolStripButton30";
      this.toolStripButton30.Size = new Size(23, 22);
      this.toolStripButton30.Text = "toolStripButton6";
      this.toolStripButton30.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage9.Controls.Add((Control) this.listViewNF7);
      this.tabPage9.Controls.Add((Control) this.toolStrip9);
      this.tabPage9.Location = new Point(4, 22);
      this.tabPage9.Name = "tabPage9";
      this.tabPage9.Size = new Size(550, 306);
      this.tabPage9.TabIndex = 8;
      this.tabPage9.Text = "KTPM";
      this.tabPage9.UseVisualStyleBackColor = true;
      this.listViewNF7.Columns.AddRange(new ColumnHeader[9]
      {
        this.columnHeader66,
        this.columnHeader67,
        this.columnHeader68,
        this.columnHeader69,
        this.columnHeader70,
        this.columnHeader71,
        this.columnHeader72,
        this.columnHeader73,
        this.columnHeader74
      });
      this.listViewNF7.Dock = DockStyle.Fill;
      this.listViewNF7.FullRowSelect = true;
      this.listViewNF7.GridLines = true;
      this.listViewNF7.HideSelection = false;
      this.listViewNF7.Location = new Point(0, 25);
      this.listViewNF7.Name = "listViewNF7";
      this.listViewNF7.Size = new Size(550, 281);
      this.listViewNF7.TabIndex = 6;
      this.listViewNF7.UseCompatibleStateImageBehavior = false;
      this.listViewNF7.View = View.Details;
      this.listViewNF7.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader66.Text = "ID";
      this.columnHeader67.Text = "X";
      this.columnHeader68.Text = "Y";
      this.columnHeader69.Text = "Z";
      this.columnHeader70.Text = "X Angle";
      this.columnHeader71.Text = "Y Angle";
      this.columnHeader72.Text = "Z Angle";
      this.columnHeader73.Text = "Padding";
      this.columnHeader74.Text = "Index";
      this.toolStrip9.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton31,
        (ToolStripItem) this.toolStripButton32,
        (ToolStripItem) this.toolStripSeparator8,
        (ToolStripItem) this.toolStripButton33,
        (ToolStripItem) this.toolStripButton34
      });
      this.toolStrip9.Location = new Point(0, 0);
      this.toolStrip9.Name = "toolStrip9";
      this.toolStrip9.Size = new Size(550, 25);
      this.toolStrip9.TabIndex = 0;
      this.toolStrip9.Text = "toolStrip9";
      this.toolStripButton31.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton31.Image = (Image) Resources.plus;
      this.toolStripButton31.ImageTransparentColor = Color.Magenta;
      this.toolStripButton31.Name = "toolStripButton31";
      this.toolStripButton31.Size = new Size(23, 22);
      this.toolStripButton31.Text = "toolStripButton3";
      this.toolStripButton31.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton32.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton32.Image = (Image) Resources.minus;
      this.toolStripButton32.ImageTransparentColor = Color.Magenta;
      this.toolStripButton32.Name = "toolStripButton32";
      this.toolStripButton32.Size = new Size(23, 22);
      this.toolStripButton32.Text = "toolStripButton4";
      this.toolStripButton32.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator8.Name = "toolStripSeparator8";
      this.toolStripSeparator8.Size = new Size(6, 25);
      this.toolStripButton33.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton33.Image = (Image) Resources.arrow_090;
      this.toolStripButton33.ImageTransparentColor = Color.Magenta;
      this.toolStripButton33.Name = "toolStripButton33";
      this.toolStripButton33.Size = new Size(23, 22);
      this.toolStripButton33.Text = "toolStripButton5";
      this.toolStripButton33.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton34.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton34.Image = (Image) Resources.arrow_270;
      this.toolStripButton34.ImageTransparentColor = Color.Magenta;
      this.toolStripButton34.Name = "toolStripButton34";
      this.toolStripButton34.Size = new Size(23, 22);
      this.toolStripButton34.Text = "toolStripButton6";
      this.toolStripButton34.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage10.Controls.Add((Control) this.listViewNF8);
      this.tabPage10.Controls.Add((Control) this.toolStrip10);
      this.tabPage10.Location = new Point(4, 22);
      this.tabPage10.Name = "tabPage10";
      this.tabPage10.Size = new Size(550, 306);
      this.tabPage10.TabIndex = 9;
      this.tabPage10.Text = "CPOI";
      this.tabPage10.UseVisualStyleBackColor = true;
      this.listViewNF8.Columns.AddRange(new ColumnHeader[11]
      {
        this.columnHeader75,
        this.columnHeader76,
        this.columnHeader78,
        this.columnHeader79,
        this.columnHeader80,
        this.columnHeader77,
        this.columnHeader85,
        this.columnHeader81,
        this.columnHeader82,
        this.columnHeader83,
        this.columnHeader84
      });
      this.listViewNF8.Dock = DockStyle.Fill;
      this.listViewNF8.FullRowSelect = true;
      this.listViewNF8.GridLines = true;
      this.listViewNF8.HideSelection = false;
      this.listViewNF8.Location = new Point(0, 25);
      this.listViewNF8.Name = "listViewNF8";
      this.listViewNF8.Size = new Size(550, 281);
      this.listViewNF8.TabIndex = 7;
      this.listViewNF8.UseCompatibleStateImageBehavior = false;
      this.listViewNF8.View = View.Details;
      this.listViewNF8.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader75.Text = "ID";
      this.columnHeader76.Text = "X1";
      this.columnHeader78.Text = "Z1";
      this.columnHeader79.Text = "X2";
      this.columnHeader80.Text = "Z2";
      this.columnHeader77.Text = "Sinus";
      this.columnHeader85.Text = "Cosinus";
      this.columnHeader81.Text = "Distance";
      this.columnHeader82.Text = "Section Data";
      this.columnHeader83.Text = "Keypoint";
      this.columnHeader84.Text = "Respawn";
      this.toolStrip10.Items.AddRange(new ToolStripItem[6]
      {
        (ToolStripItem) this.toolStripButton35,
        (ToolStripItem) this.toolStripButton36,
        (ToolStripItem) this.toolStripSeparator9,
        (ToolStripItem) this.toolStripButton37,
        (ToolStripItem) this.toolStripButton38,
        (ToolStripItem) this.toolStripButton79
      });
      this.toolStrip10.Location = new Point(0, 0);
      this.toolStrip10.Name = "toolStrip10";
      this.toolStrip10.Size = new Size(550, 25);
      this.toolStrip10.TabIndex = 0;
      this.toolStrip10.Text = "toolStrip10";
      this.toolStripButton35.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton35.Image = (Image) Resources.plus;
      this.toolStripButton35.ImageTransparentColor = Color.Magenta;
      this.toolStripButton35.Name = "toolStripButton35";
      this.toolStripButton35.Size = new Size(23, 22);
      this.toolStripButton35.Text = "toolStripButton3";
      this.toolStripButton35.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton36.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton36.Image = (Image) Resources.minus;
      this.toolStripButton36.ImageTransparentColor = Color.Magenta;
      this.toolStripButton36.Name = "toolStripButton36";
      this.toolStripButton36.Size = new Size(23, 22);
      this.toolStripButton36.Text = "toolStripButton4";
      this.toolStripButton36.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator9.Name = "toolStripSeparator9";
      this.toolStripSeparator9.Size = new Size(6, 25);
      this.toolStripButton37.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton37.Image = (Image) Resources.arrow_090;
      this.toolStripButton37.ImageTransparentColor = Color.Magenta;
      this.toolStripButton37.Name = "toolStripButton37";
      this.toolStripButton37.Size = new Size(23, 22);
      this.toolStripButton37.Text = "toolStripButton5";
      this.toolStripButton37.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton38.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton38.Image = (Image) Resources.arrow_270;
      this.toolStripButton38.ImageTransparentColor = Color.Magenta;
      this.toolStripButton38.Name = "toolStripButton38";
      this.toolStripButton38.Size = new Size(23, 22);
      this.toolStripButton38.Text = "toolStripButton6";
      this.toolStripButton38.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage11.Controls.Add((Control) this.listViewNF9);
      this.tabPage11.Controls.Add((Control) this.toolStrip11);
      this.tabPage11.Location = new Point(4, 22);
      this.tabPage11.Name = "tabPage11";
      this.tabPage11.Size = new Size(550, 306);
      this.tabPage11.TabIndex = 10;
      this.tabPage11.Text = "CPAT";
      this.tabPage11.UseVisualStyleBackColor = true;
      this.listViewNF9.Columns.AddRange(new ColumnHeader[10]
      {
        this.columnHeader86,
        this.columnHeader87,
        this.columnHeader88,
        this.columnHeader89,
        this.columnHeader90,
        this.columnHeader91,
        this.columnHeader92,
        this.columnHeader93,
        this.columnHeader94,
        this.columnHeader95
      });
      this.listViewNF9.Dock = DockStyle.Fill;
      this.listViewNF9.FullRowSelect = true;
      this.listViewNF9.GridLines = true;
      this.listViewNF9.HideSelection = false;
      this.listViewNF9.Location = new Point(0, 25);
      this.listViewNF9.Name = "listViewNF9";
      this.listViewNF9.Size = new Size(550, 281);
      this.listViewNF9.TabIndex = 7;
      this.listViewNF9.UseCompatibleStateImageBehavior = false;
      this.listViewNF9.View = View.Details;
      this.listViewNF9.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader86.Text = "ID";
      this.columnHeader87.Text = "Start Idx";
      this.columnHeader88.Text = "Length";
      this.columnHeader89.Text = "Goes To 1";
      this.columnHeader90.Text = "Goes To 2";
      this.columnHeader91.Text = "Goes To 3";
      this.columnHeader92.Text = "Comes From 1";
      this.columnHeader93.Text = "Comes From 2";
      this.columnHeader94.Text = "Comes From 3";
      this.columnHeader95.Text = "Section Order";
      this.toolStrip11.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton39,
        (ToolStripItem) this.toolStripButton40,
        (ToolStripItem) this.toolStripSeparator10,
        (ToolStripItem) this.toolStripButton41,
        (ToolStripItem) this.toolStripButton42
      });
      this.toolStrip11.Location = new Point(0, 0);
      this.toolStrip11.Name = "toolStrip11";
      this.toolStrip11.Size = new Size(550, 25);
      this.toolStrip11.TabIndex = 0;
      this.toolStrip11.Text = "toolStrip11";
      this.toolStripButton39.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton39.Image = (Image) Resources.plus;
      this.toolStripButton39.ImageTransparentColor = Color.Magenta;
      this.toolStripButton39.Name = "toolStripButton39";
      this.toolStripButton39.Size = new Size(23, 22);
      this.toolStripButton39.Text = "toolStripButton3";
      this.toolStripButton39.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton40.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton40.Image = (Image) Resources.minus;
      this.toolStripButton40.ImageTransparentColor = Color.Magenta;
      this.toolStripButton40.Name = "toolStripButton40";
      this.toolStripButton40.Size = new Size(23, 22);
      this.toolStripButton40.Text = "toolStripButton4";
      this.toolStripButton40.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator10.Name = "toolStripSeparator10";
      this.toolStripSeparator10.Size = new Size(6, 25);
      this.toolStripButton41.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton41.Image = (Image) Resources.arrow_090;
      this.toolStripButton41.ImageTransparentColor = Color.Magenta;
      this.toolStripButton41.Name = "toolStripButton41";
      this.toolStripButton41.Size = new Size(23, 22);
      this.toolStripButton41.Text = "toolStripButton5";
      this.toolStripButton41.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton42.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton42.Image = (Image) Resources.arrow_270;
      this.toolStripButton42.ImageTransparentColor = Color.Magenta;
      this.toolStripButton42.Name = "toolStripButton42";
      this.toolStripButton42.Size = new Size(23, 22);
      this.toolStripButton42.Text = "toolStripButton6";
      this.toolStripButton42.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage12.Controls.Add((Control) this.listViewNF10);
      this.tabPage12.Controls.Add((Control) this.toolStrip12);
      this.tabPage12.Location = new Point(4, 22);
      this.tabPage12.Name = "tabPage12";
      this.tabPage12.Size = new Size(550, 306);
      this.tabPage12.TabIndex = 11;
      this.tabPage12.Text = "IPOI";
      this.tabPage12.UseVisualStyleBackColor = true;
      this.listViewNF10.Columns.AddRange(new ColumnHeader[6]
      {
        this.columnHeader96,
        this.columnHeader97,
        this.columnHeader98,
        this.columnHeader99,
        this.columnHeader100,
        this.columnHeader101
      });
      this.listViewNF10.Dock = DockStyle.Fill;
      this.listViewNF10.FullRowSelect = true;
      this.listViewNF10.GridLines = true;
      this.listViewNF10.HideSelection = false;
      this.listViewNF10.Location = new Point(0, 25);
      this.listViewNF10.Name = "listViewNF10";
      this.listViewNF10.Size = new Size(550, 281);
      this.listViewNF10.TabIndex = 7;
      this.listViewNF10.UseCompatibleStateImageBehavior = false;
      this.listViewNF10.View = View.Details;
      this.listViewNF10.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader96.Text = "ID";
      this.columnHeader97.Text = "X";
      this.columnHeader98.Text = "Y";
      this.columnHeader99.Text = "Z";
      this.columnHeader100.Text = "?";
      this.columnHeader101.Text = "?";
      this.toolStrip12.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton43,
        (ToolStripItem) this.toolStripButton44,
        (ToolStripItem) this.toolStripSeparator11,
        (ToolStripItem) this.toolStripButton45,
        (ToolStripItem) this.toolStripButton46
      });
      this.toolStrip12.Location = new Point(0, 0);
      this.toolStrip12.Name = "toolStrip12";
      this.toolStrip12.Size = new Size(550, 25);
      this.toolStrip12.TabIndex = 0;
      this.toolStrip12.Text = "toolStrip12";
      this.toolStripButton43.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton43.Image = (Image) Resources.plus;
      this.toolStripButton43.ImageTransparentColor = Color.Magenta;
      this.toolStripButton43.Name = "toolStripButton43";
      this.toolStripButton43.Size = new Size(23, 22);
      this.toolStripButton43.Text = "toolStripButton3";
      this.toolStripButton43.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton44.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton44.Image = (Image) Resources.minus;
      this.toolStripButton44.ImageTransparentColor = Color.Magenta;
      this.toolStripButton44.Name = "toolStripButton44";
      this.toolStripButton44.Size = new Size(23, 22);
      this.toolStripButton44.Text = "toolStripButton4";
      this.toolStripButton44.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator11.Name = "toolStripSeparator11";
      this.toolStripSeparator11.Size = new Size(6, 25);
      this.toolStripButton45.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton45.Image = (Image) Resources.arrow_090;
      this.toolStripButton45.ImageTransparentColor = Color.Magenta;
      this.toolStripButton45.Name = "toolStripButton45";
      this.toolStripButton45.Size = new Size(23, 22);
      this.toolStripButton45.Text = "toolStripButton5";
      this.toolStripButton45.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton46.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton46.Image = (Image) Resources.arrow_270;
      this.toolStripButton46.ImageTransparentColor = Color.Magenta;
      this.toolStripButton46.Name = "toolStripButton46";
      this.toolStripButton46.Size = new Size(23, 22);
      this.toolStripButton46.Text = "toolStripButton6";
      this.toolStripButton46.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage13.Controls.Add((Control) this.listViewNF11);
      this.tabPage13.Controls.Add((Control) this.toolStrip13);
      this.tabPage13.Location = new Point(4, 22);
      this.tabPage13.Name = "tabPage13";
      this.tabPage13.Size = new Size(550, 306);
      this.tabPage13.TabIndex = 12;
      this.tabPage13.Text = "IPAT";
      this.tabPage13.UseVisualStyleBackColor = true;
      this.listViewNF11.Columns.AddRange(new ColumnHeader[10]
      {
        this.columnHeader102,
        this.columnHeader103,
        this.columnHeader104,
        this.columnHeader105,
        this.columnHeader106,
        this.columnHeader107,
        this.columnHeader108,
        this.columnHeader109,
        this.columnHeader110,
        this.columnHeader111
      });
      this.listViewNF11.Dock = DockStyle.Fill;
      this.listViewNF11.FullRowSelect = true;
      this.listViewNF11.GridLines = true;
      this.listViewNF11.HideSelection = false;
      this.listViewNF11.Location = new Point(0, 25);
      this.listViewNF11.Name = "listViewNF11";
      this.listViewNF11.Size = new Size(550, 281);
      this.listViewNF11.TabIndex = 8;
      this.listViewNF11.UseCompatibleStateImageBehavior = false;
      this.listViewNF11.View = View.Details;
      this.listViewNF11.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader102.Text = "ID";
      this.columnHeader103.Text = "Start Idx";
      this.columnHeader104.Text = "Length";
      this.columnHeader105.Text = "Goes To 1";
      this.columnHeader106.Text = "Goes To 2";
      this.columnHeader107.Text = "Goes To 3";
      this.columnHeader108.Text = "Comes From 1";
      this.columnHeader109.Text = "Comes From 2";
      this.columnHeader110.Text = "Comes From 3";
      this.columnHeader111.Text = "Section Order";
      this.toolStrip13.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton47,
        (ToolStripItem) this.toolStripButton48,
        (ToolStripItem) this.toolStripSeparator12,
        (ToolStripItem) this.toolStripButton49,
        (ToolStripItem) this.toolStripButton50
      });
      this.toolStrip13.Location = new Point(0, 0);
      this.toolStrip13.Name = "toolStrip13";
      this.toolStrip13.Size = new Size(550, 25);
      this.toolStrip13.TabIndex = 0;
      this.toolStrip13.Text = "toolStrip13";
      this.toolStripButton47.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton47.Image = (Image) Resources.plus;
      this.toolStripButton47.ImageTransparentColor = Color.Magenta;
      this.toolStripButton47.Name = "toolStripButton47";
      this.toolStripButton47.Size = new Size(23, 22);
      this.toolStripButton47.Text = "toolStripButton3";
      this.toolStripButton47.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton48.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton48.Image = (Image) Resources.minus;
      this.toolStripButton48.ImageTransparentColor = Color.Magenta;
      this.toolStripButton48.Name = "toolStripButton48";
      this.toolStripButton48.Size = new Size(23, 22);
      this.toolStripButton48.Text = "toolStripButton4";
      this.toolStripButton48.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator12.Name = "toolStripSeparator12";
      this.toolStripSeparator12.Size = new Size(6, 25);
      this.toolStripButton49.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton49.Image = (Image) Resources.arrow_090;
      this.toolStripButton49.ImageTransparentColor = Color.Magenta;
      this.toolStripButton49.Name = "toolStripButton49";
      this.toolStripButton49.Size = new Size(23, 22);
      this.toolStripButton49.Text = "toolStripButton5";
      this.toolStripButton49.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton50.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton50.Image = (Image) Resources.arrow_270;
      this.toolStripButton50.ImageTransparentColor = Color.Magenta;
      this.toolStripButton50.Name = "toolStripButton50";
      this.toolStripButton50.Size = new Size(23, 22);
      this.toolStripButton50.Text = "toolStripButton6";
      this.toolStripButton50.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage14.Controls.Add((Control) this.listViewNF12);
      this.tabPage14.Controls.Add((Control) this.toolStrip14);
      this.tabPage14.Location = new Point(4, 22);
      this.tabPage14.Name = "tabPage14";
      this.tabPage14.Size = new Size(550, 306);
      this.tabPage14.TabIndex = 13;
      this.tabPage14.Text = "EPOI";
      this.tabPage14.UseVisualStyleBackColor = true;
      this.listViewNF12.Columns.AddRange(new ColumnHeader[8]
      {
        this.columnHeader112,
        this.columnHeader113,
        this.columnHeader114,
        this.columnHeader115,
        this.columnHeader118,
        this.columnHeader116,
        this.columnHeader200,
        this.columnHeader129
      });
      this.listViewNF12.Dock = DockStyle.Fill;
      this.listViewNF12.FullRowSelect = true;
      this.listViewNF12.GridLines = true;
      this.listViewNF12.HideSelection = false;
      this.listViewNF12.Location = new Point(0, 25);
      this.listViewNF12.Name = "listViewNF12";
      this.listViewNF12.Size = new Size(550, 281);
      this.listViewNF12.TabIndex = 8;
      this.listViewNF12.UseCompatibleStateImageBehavior = false;
      this.listViewNF12.View = View.Details;
      this.listViewNF12.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader112.Text = "ID";
      this.columnHeader113.Text = "X";
      this.columnHeader114.Text = "Y";
      this.columnHeader115.Text = "Z";
      this.columnHeader118.Text = "Point Size";
      this.columnHeader116.Text = "CPU Drifting";
      this.columnHeader200.Text = "?";
      this.columnHeader129.Text = "?";
      this.toolStrip14.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton51,
        (ToolStripItem) this.toolStripButton52,
        (ToolStripItem) this.toolStripSeparator13,
        (ToolStripItem) this.toolStripButton53,
        (ToolStripItem) this.toolStripButton54
      });
      this.toolStrip14.Location = new Point(0, 0);
      this.toolStrip14.Name = "toolStrip14";
      this.toolStrip14.Size = new Size(550, 25);
      this.toolStrip14.TabIndex = 0;
      this.toolStrip14.Text = "toolStrip14";
      this.toolStripButton51.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton51.Image = (Image) Resources.plus;
      this.toolStripButton51.ImageTransparentColor = Color.Magenta;
      this.toolStripButton51.Name = "toolStripButton51";
      this.toolStripButton51.Size = new Size(23, 22);
      this.toolStripButton51.Text = "toolStripButton3";
      this.toolStripButton51.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton52.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton52.Image = (Image) Resources.minus;
      this.toolStripButton52.ImageTransparentColor = Color.Magenta;
      this.toolStripButton52.Name = "toolStripButton52";
      this.toolStripButton52.Size = new Size(23, 22);
      this.toolStripButton52.Text = "toolStripButton4";
      this.toolStripButton52.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator13.Name = "toolStripSeparator13";
      this.toolStripSeparator13.Size = new Size(6, 25);
      this.toolStripButton53.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton53.Image = (Image) Resources.arrow_090;
      this.toolStripButton53.ImageTransparentColor = Color.Magenta;
      this.toolStripButton53.Name = "toolStripButton53";
      this.toolStripButton53.Size = new Size(23, 22);
      this.toolStripButton53.Text = "toolStripButton5";
      this.toolStripButton53.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton54.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton54.Image = (Image) Resources.arrow_270;
      this.toolStripButton54.ImageTransparentColor = Color.Magenta;
      this.toolStripButton54.Name = "toolStripButton54";
      this.toolStripButton54.Size = new Size(23, 22);
      this.toolStripButton54.Text = "toolStripButton6";
      this.toolStripButton54.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage15.Controls.Add((Control) this.listViewNF13);
      this.tabPage15.Controls.Add((Control) this.toolStrip15);
      this.tabPage15.Location = new Point(4, 22);
      this.tabPage15.Name = "tabPage15";
      this.tabPage15.Size = new Size(550, 306);
      this.tabPage15.TabIndex = 14;
      this.tabPage15.Text = "EPAT";
      this.tabPage15.UseVisualStyleBackColor = true;
      this.listViewNF13.Columns.AddRange(new ColumnHeader[10]
      {
        this.columnHeader119,
        this.columnHeader120,
        this.columnHeader121,
        this.columnHeader122,
        this.columnHeader123,
        this.columnHeader124,
        this.columnHeader125,
        this.columnHeader126,
        this.columnHeader127,
        this.columnHeader128
      });
      this.listViewNF13.Dock = DockStyle.Fill;
      this.listViewNF13.FullRowSelect = true;
      this.listViewNF13.GridLines = true;
      this.listViewNF13.HideSelection = false;
      this.listViewNF13.Location = new Point(0, 25);
      this.listViewNF13.Name = "listViewNF13";
      this.listViewNF13.Size = new Size(550, 281);
      this.listViewNF13.TabIndex = 9;
      this.listViewNF13.UseCompatibleStateImageBehavior = false;
      this.listViewNF13.View = View.Details;
      this.listViewNF13.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader119.Text = "ID";
      this.columnHeader120.Text = "Start Idx";
      this.columnHeader121.Text = "Length";
      this.columnHeader122.Text = "Goes To 1";
      this.columnHeader123.Text = "Goes To 2";
      this.columnHeader124.Text = "Goes To 3";
      this.columnHeader125.Text = "Comes From 1";
      this.columnHeader126.Text = "Comes From 2";
      this.columnHeader127.Text = "Comes From 3";
      this.columnHeader128.Text = "Section Order";
      this.toolStrip15.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton55,
        (ToolStripItem) this.toolStripButton56,
        (ToolStripItem) this.toolStripSeparator14,
        (ToolStripItem) this.toolStripButton57,
        (ToolStripItem) this.toolStripButton58
      });
      this.toolStrip15.Location = new Point(0, 0);
      this.toolStrip15.Name = "toolStrip15";
      this.toolStrip15.Size = new Size(550, 25);
      this.toolStrip15.TabIndex = 0;
      this.toolStrip15.Text = "toolStrip15";
      this.toolStripButton55.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton55.Image = (Image) Resources.plus;
      this.toolStripButton55.ImageTransparentColor = Color.Magenta;
      this.toolStripButton55.Name = "toolStripButton55";
      this.toolStripButton55.Size = new Size(23, 22);
      this.toolStripButton55.Text = "toolStripButton3";
      this.toolStripButton55.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton56.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton56.Image = (Image) Resources.minus;
      this.toolStripButton56.ImageTransparentColor = Color.Magenta;
      this.toolStripButton56.Name = "toolStripButton56";
      this.toolStripButton56.Size = new Size(23, 22);
      this.toolStripButton56.Text = "toolStripButton4";
      this.toolStripButton56.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator14.Name = "toolStripSeparator14";
      this.toolStripSeparator14.Size = new Size(6, 25);
      this.toolStripButton57.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton57.Image = (Image) Resources.arrow_090;
      this.toolStripButton57.ImageTransparentColor = Color.Magenta;
      this.toolStripButton57.Name = "toolStripButton57";
      this.toolStripButton57.Size = new Size(23, 22);
      this.toolStripButton57.Text = "toolStripButton5";
      this.toolStripButton57.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton58.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton58.Image = (Image) Resources.arrow_270;
      this.toolStripButton58.ImageTransparentColor = Color.Magenta;
      this.toolStripButton58.Name = "toolStripButton58";
      this.toolStripButton58.Size = new Size(23, 22);
      this.toolStripButton58.Text = "toolStripButton6";
      this.toolStripButton58.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage16.Controls.Add((Control) this.listViewNF14);
      this.tabPage16.Controls.Add((Control) this.toolStrip16);
      this.tabPage16.Location = new Point(4, 22);
      this.tabPage16.Name = "tabPage16";
      this.tabPage16.Size = new Size(550, 306);
      this.tabPage16.TabIndex = 15;
      this.tabPage16.Text = "MEPO";
      this.tabPage16.UseVisualStyleBackColor = true;
      this.listViewNF14.Columns.AddRange(new ColumnHeader[7]
      {
        this.columnHeader117,
        this.columnHeader130,
        this.columnHeader131,
        this.columnHeader132,
        this.columnHeader133,
        this.columnHeader134,
        this.columnHeader135
      });
      this.listViewNF14.Dock = DockStyle.Fill;
      this.listViewNF14.FullRowSelect = true;
      this.listViewNF14.GridLines = true;
      this.listViewNF14.HideSelection = false;
      this.listViewNF14.Location = new Point(0, 25);
      this.listViewNF14.Name = "listViewNF14";
      this.listViewNF14.Size = new Size(550, 281);
      this.listViewNF14.TabIndex = 9;
      this.listViewNF14.UseCompatibleStateImageBehavior = false;
      this.listViewNF14.View = View.Details;
      this.listViewNF14.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader117.Text = "ID";
      this.columnHeader130.Text = "X";
      this.columnHeader131.Text = "Y";
      this.columnHeader132.Text = "Z";
      this.columnHeader133.Text = "Point Size";
      this.columnHeader134.Text = "CPU Drifting";
      this.columnHeader135.Text = "?";
      this.toolStrip16.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton59,
        (ToolStripItem) this.toolStripButton60,
        (ToolStripItem) this.toolStripSeparator15,
        (ToolStripItem) this.toolStripButton61,
        (ToolStripItem) this.toolStripButton62
      });
      this.toolStrip16.Location = new Point(0, 0);
      this.toolStrip16.Name = "toolStrip16";
      this.toolStrip16.Size = new Size(550, 25);
      this.toolStrip16.TabIndex = 0;
      this.toolStrip16.Text = "toolStrip16";
      this.toolStripButton59.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton59.Image = (Image) Resources.plus;
      this.toolStripButton59.ImageTransparentColor = Color.Magenta;
      this.toolStripButton59.Name = "toolStripButton59";
      this.toolStripButton59.Size = new Size(23, 22);
      this.toolStripButton59.Text = "toolStripButton3";
      this.toolStripButton59.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton60.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton60.Image = (Image) Resources.minus;
      this.toolStripButton60.ImageTransparentColor = Color.Magenta;
      this.toolStripButton60.Name = "toolStripButton60";
      this.toolStripButton60.Size = new Size(23, 22);
      this.toolStripButton60.Text = "toolStripButton4";
      this.toolStripButton60.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator15.Name = "toolStripSeparator15";
      this.toolStripSeparator15.Size = new Size(6, 25);
      this.toolStripButton61.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton61.Image = (Image) Resources.arrow_090;
      this.toolStripButton61.ImageTransparentColor = Color.Magenta;
      this.toolStripButton61.Name = "toolStripButton61";
      this.toolStripButton61.Size = new Size(23, 22);
      this.toolStripButton61.Text = "toolStripButton5";
      this.toolStripButton61.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton62.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton62.Image = (Image) Resources.arrow_270;
      this.toolStripButton62.ImageTransparentColor = Color.Magenta;
      this.toolStripButton62.Name = "toolStripButton62";
      this.toolStripButton62.Size = new Size(23, 22);
      this.toolStripButton62.Text = "toolStripButton6";
      this.toolStripButton62.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage17.Controls.Add((Control) this.listViewNF15);
      this.tabPage17.Controls.Add((Control) this.toolStrip17);
      this.tabPage17.Location = new Point(4, 22);
      this.tabPage17.Name = "tabPage17";
      this.tabPage17.Size = new Size(550, 306);
      this.tabPage17.TabIndex = 16;
      this.tabPage17.Text = "MEPA";
      this.tabPage17.UseVisualStyleBackColor = true;
      this.listViewNF15.Columns.AddRange(new ColumnHeader[19]
      {
        this.columnHeader136,
        this.columnHeader137,
        this.columnHeader138,
        this.columnHeader139,
        this.columnHeader140,
        this.columnHeader141,
        this.columnHeader142,
        this.columnHeader143,
        this.columnHeader144,
        this.columnHeader146,
        this.columnHeader147,
        this.columnHeader148,
        this.columnHeader149,
        this.columnHeader150,
        this.columnHeader151,
        this.columnHeader152,
        this.columnHeader153,
        this.columnHeader154,
        this.columnHeader155
      });
      this.listViewNF15.Dock = DockStyle.Fill;
      this.listViewNF15.FullRowSelect = true;
      this.listViewNF15.GridLines = true;
      this.listViewNF15.HideSelection = false;
      this.listViewNF15.Location = new Point(0, 25);
      this.listViewNF15.Name = "listViewNF15";
      this.listViewNF15.Size = new Size(550, 281);
      this.listViewNF15.TabIndex = 10;
      this.listViewNF15.UseCompatibleStateImageBehavior = false;
      this.listViewNF15.View = View.Details;
      this.listViewNF15.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader136.Text = "ID";
      this.columnHeader137.Text = "Start Idx";
      this.columnHeader138.Text = "Length";
      this.columnHeader139.Text = "Goes To 1";
      this.columnHeader140.Text = "Goes To 2";
      this.columnHeader141.Text = "Goes To 3";
      this.columnHeader142.Text = "Goes To 4";
      this.columnHeader143.Text = "Goes To 5";
      this.columnHeader144.Text = "Goes To 6";
      this.columnHeader146.Text = "Goes To 7";
      this.columnHeader147.Text = "Goes To 8";
      this.columnHeader148.Text = "Comes From 1";
      this.columnHeader149.Text = "Comes From 2";
      this.columnHeader150.Text = "Comes From 3";
      this.columnHeader151.Text = "Comes From 4";
      this.columnHeader152.Text = "Comes From 5";
      this.columnHeader153.Text = "Comes From 6";
      this.columnHeader154.Text = "Comes From 7";
      this.columnHeader155.Text = "Comes From 8";
      this.toolStrip17.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton63,
        (ToolStripItem) this.toolStripButton64,
        (ToolStripItem) this.toolStripSeparator16,
        (ToolStripItem) this.toolStripButton65,
        (ToolStripItem) this.toolStripButton66
      });
      this.toolStrip17.Location = new Point(0, 0);
      this.toolStrip17.Name = "toolStrip17";
      this.toolStrip17.Size = new Size(550, 25);
      this.toolStrip17.TabIndex = 0;
      this.toolStrip17.Text = "toolStrip17";
      this.toolStripButton63.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton63.Image = (Image) Resources.plus;
      this.toolStripButton63.ImageTransparentColor = Color.Magenta;
      this.toolStripButton63.Name = "toolStripButton63";
      this.toolStripButton63.Size = new Size(23, 22);
      this.toolStripButton63.Text = "toolStripButton3";
      this.toolStripButton63.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton64.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton64.Image = (Image) Resources.minus;
      this.toolStripButton64.ImageTransparentColor = Color.Magenta;
      this.toolStripButton64.Name = "toolStripButton64";
      this.toolStripButton64.Size = new Size(23, 22);
      this.toolStripButton64.Text = "toolStripButton4";
      this.toolStripButton64.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator16.Name = "toolStripSeparator16";
      this.toolStripSeparator16.Size = new Size(6, 25);
      this.toolStripButton65.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton65.Image = (Image) Resources.arrow_090;
      this.toolStripButton65.ImageTransparentColor = Color.Magenta;
      this.toolStripButton65.Name = "toolStripButton65";
      this.toolStripButton65.Size = new Size(23, 22);
      this.toolStripButton65.Text = "toolStripButton5";
      this.toolStripButton65.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton66.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton66.Image = (Image) Resources.arrow_270;
      this.toolStripButton66.ImageTransparentColor = Color.Magenta;
      this.toolStripButton66.Name = "toolStripButton66";
      this.toolStripButton66.Size = new Size(23, 22);
      this.toolStripButton66.Text = "toolStripButton6";
      this.toolStripButton66.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage18.Controls.Add((Control) this.listViewNF16);
      this.tabPage18.Controls.Add((Control) this.toolStrip18);
      this.tabPage18.Location = new Point(4, 22);
      this.tabPage18.Name = "tabPage18";
      this.tabPage18.Size = new Size(550, 306);
      this.tabPage18.TabIndex = 17;
      this.tabPage18.Text = "AREA";
      this.tabPage18.UseVisualStyleBackColor = true;
      this.listViewNF16.Columns.AddRange(new ColumnHeader[21]
      {
        this.columnHeader145,
        this.columnHeader156,
        this.columnHeader157,
        this.columnHeader158,
        this.columnHeader159,
        this.columnHeader160,
        this.columnHeader161,
        this.columnHeader162,
        this.columnHeader163,
        this.columnHeader164,
        this.columnHeader165,
        this.columnHeader166,
        this.columnHeader167,
        this.columnHeader168,
        this.columnHeader169,
        this.columnHeader170,
        this.columnHeader171,
        this.columnHeader172,
        this.columnHeader173,
        this.columnHeader174,
        this.columnHeader175
      });
      this.listViewNF16.Dock = DockStyle.Fill;
      this.listViewNF16.FullRowSelect = true;
      this.listViewNF16.GridLines = true;
      this.listViewNF16.HideSelection = false;
      this.listViewNF16.Location = new Point(0, 25);
      this.listViewNF16.Name = "listViewNF16";
      this.listViewNF16.Size = new Size(550, 281);
      this.listViewNF16.TabIndex = 11;
      this.listViewNF16.UseCompatibleStateImageBehavior = false;
      this.listViewNF16.View = View.Details;
      this.listViewNF16.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader145.Text = "ID";
      this.columnHeader156.Text = "X";
      this.columnHeader157.Text = "Y";
      this.columnHeader158.Text = "Z";
      this.columnHeader159.Text = "?";
      this.columnHeader160.Text = "?";
      this.columnHeader161.Text = "?";
      this.columnHeader162.Text = "?";
      this.columnHeader163.Text = "?";
      this.columnHeader164.Text = "?";
      this.columnHeader165.Text = "?";
      this.columnHeader166.Text = "?";
      this.columnHeader167.Text = "?";
      this.columnHeader168.Text = "?";
      this.columnHeader169.Text = "?";
      this.columnHeader170.Text = "?";
      this.columnHeader171.Text = "?";
      this.columnHeader172.Text = "?";
      this.columnHeader173.Text = "?";
      this.columnHeader174.Text = "Linked Came";
      this.columnHeader175.Text = "?";
      this.toolStrip18.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton67,
        (ToolStripItem) this.toolStripButton68,
        (ToolStripItem) this.toolStripSeparator17,
        (ToolStripItem) this.toolStripButton69,
        (ToolStripItem) this.toolStripButton70
      });
      this.toolStrip18.Location = new Point(0, 0);
      this.toolStrip18.Name = "toolStrip18";
      this.toolStrip18.Size = new Size(550, 25);
      this.toolStrip18.TabIndex = 0;
      this.toolStrip18.Text = "toolStrip18";
      this.toolStripButton67.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton67.Image = (Image) Resources.plus;
      this.toolStripButton67.ImageTransparentColor = Color.Magenta;
      this.toolStripButton67.Name = "toolStripButton67";
      this.toolStripButton67.Size = new Size(23, 22);
      this.toolStripButton67.Text = "toolStripButton3";
      this.toolStripButton67.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton68.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton68.Image = (Image) Resources.minus;
      this.toolStripButton68.ImageTransparentColor = Color.Magenta;
      this.toolStripButton68.Name = "toolStripButton68";
      this.toolStripButton68.Size = new Size(23, 22);
      this.toolStripButton68.Text = "toolStripButton4";
      this.toolStripButton68.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator17.Name = "toolStripSeparator17";
      this.toolStripSeparator17.Size = new Size(6, 25);
      this.toolStripButton69.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton69.Image = (Image) Resources.arrow_090;
      this.toolStripButton69.ImageTransparentColor = Color.Magenta;
      this.toolStripButton69.Name = "toolStripButton69";
      this.toolStripButton69.Size = new Size(23, 22);
      this.toolStripButton69.Text = "toolStripButton5";
      this.toolStripButton69.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton70.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton70.Image = (Image) Resources.arrow_270;
      this.toolStripButton70.ImageTransparentColor = Color.Magenta;
      this.toolStripButton70.Name = "toolStripButton70";
      this.toolStripButton70.Size = new Size(23, 22);
      this.toolStripButton70.Text = "toolStripButton6";
      this.toolStripButton70.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage19.Controls.Add((Control) this.listViewNF17);
      this.tabPage19.Controls.Add((Control) this.toolStrip19);
      this.tabPage19.Location = new Point(4, 22);
      this.tabPage19.Name = "tabPage19";
      this.tabPage19.Size = new Size(550, 306);
      this.tabPage19.TabIndex = 18;
      this.tabPage19.Text = "CAME";
      this.tabPage19.UseVisualStyleBackColor = true;
      this.listViewNF17.Columns.AddRange(new ColumnHeader[24]
      {
        this.columnHeader176,
        this.columnHeader177,
        this.columnHeader178,
        this.columnHeader179,
        this.columnHeader180,
        this.columnHeader181,
        this.columnHeader182,
        this.columnHeader183,
        this.columnHeader184,
        this.columnHeader185,
        this.columnHeader186,
        this.columnHeader187,
        this.columnHeader188,
        this.columnHeader189,
        this.columnHeader190,
        this.columnHeader191,
        this.columnHeader199,
        this.columnHeader192,
        this.columnHeader193,
        this.columnHeader194,
        this.columnHeader195,
        this.columnHeader196,
        this.columnHeader197,
        this.columnHeader198
      });
      this.listViewNF17.Dock = DockStyle.Fill;
      this.listViewNF17.FullRowSelect = true;
      this.listViewNF17.GridLines = true;
      this.listViewNF17.HideSelection = false;
      this.listViewNF17.Location = new Point(0, 25);
      this.listViewNF17.Name = "listViewNF17";
      this.listViewNF17.Size = new Size(550, 281);
      this.listViewNF17.TabIndex = 12;
      this.listViewNF17.UseCompatibleStateImageBehavior = false;
      this.listViewNF17.View = View.Details;
      this.listViewNF17.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.columnHeader176.Text = "ID";
      this.columnHeader177.Text = "X1";
      this.columnHeader178.Text = "Y1";
      this.columnHeader179.Text = "Z1";
      this.columnHeader180.Text = "X Rotation";
      this.columnHeader181.Text = "Y Rotation";
      this.columnHeader182.Text = "Z Rotation";
      this.columnHeader183.Text = "X2";
      this.columnHeader184.Text = "Y2";
      this.columnHeader185.Text = "Z2";
      this.columnHeader186.Text = "X3";
      this.columnHeader187.Text = "Y3";
      this.columnHeader188.Text = "Z3";
      this.columnHeader189.Text = "?";
      this.columnHeader190.Text = "?";
      this.columnHeader191.Text = "?";
      this.columnHeader199.Text = "Camera Zoom";
      this.columnHeader192.Text = "Camera Type";
      this.columnHeader193.Text = "Linked Route";
      this.columnHeader194.Text = "Route Speed";
      this.columnHeader195.Text = "Point Speed";
      this.columnHeader196.Text = "Total Length";
      this.columnHeader197.Text = "Next Came";
      this.columnHeader198.Text = "?";
      this.toolStrip19.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton71,
        (ToolStripItem) this.toolStripButton72,
        (ToolStripItem) this.toolStripSeparator18,
        (ToolStripItem) this.toolStripButton73,
        (ToolStripItem) this.toolStripButton74
      });
      this.toolStrip19.Location = new Point(0, 0);
      this.toolStrip19.Name = "toolStrip19";
      this.toolStrip19.Size = new Size(550, 25);
      this.toolStrip19.TabIndex = 0;
      this.toolStrip19.Text = "toolStrip19";
      this.toolStripButton71.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton71.Image = (Image) Resources.plus;
      this.toolStripButton71.ImageTransparentColor = Color.Magenta;
      this.toolStripButton71.Name = "toolStripButton71";
      this.toolStripButton71.Size = new Size(23, 22);
      this.toolStripButton71.Text = "toolStripButton3";
      this.toolStripButton71.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton72.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton72.Image = (Image) Resources.minus;
      this.toolStripButton72.ImageTransparentColor = Color.Magenta;
      this.toolStripButton72.Name = "toolStripButton72";
      this.toolStripButton72.Size = new Size(23, 22);
      this.toolStripButton72.Text = "toolStripButton4";
      this.toolStripButton72.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator18.Name = "toolStripSeparator18";
      this.toolStripSeparator18.Size = new Size(6, 25);
      this.toolStripButton73.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton73.Image = (Image) Resources.arrow_090;
      this.toolStripButton73.ImageTransparentColor = Color.Magenta;
      this.toolStripButton73.Name = "toolStripButton73";
      this.toolStripButton73.Size = new Size(23, 22);
      this.toolStripButton73.Text = "toolStripButton5";
      this.toolStripButton73.Click += new EventHandler(this.toolStripButton5_Click);
      this.toolStripButton74.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton74.Image = (Image) Resources.arrow_270;
      this.toolStripButton74.ImageTransparentColor = Color.Magenta;
      this.toolStripButton74.Name = "toolStripButton74";
      this.toolStripButton74.Size = new Size(23, 22);
      this.toolStripButton74.Text = "toolStripButton6";
      this.toolStripButton74.Click += new EventHandler(this.toolStripButton6_Click);
      this.tabPage21.Controls.Add((Control) this.textBox3);
      this.tabPage21.Controls.Add((Control) this.label13);
      this.tabPage21.Controls.Add((Control) this.label12);
      this.tabPage21.Controls.Add((Control) this.label11);
      this.tabPage21.Controls.Add((Control) this.textBox2);
      this.tabPage21.Controls.Add((Control) this.label10);
      this.tabPage21.Controls.Add((Control) this.textBox1);
      this.tabPage21.Controls.Add((Control) this.label9);
      this.tabPage21.Location = new Point(4, 22);
      this.tabPage21.Name = "tabPage21";
      this.tabPage21.Padding = new Padding(3);
      this.tabPage21.Size = new Size(550, 306);
      this.tabPage21.TabIndex = 20;
      this.tabPage21.Text = "Track Info";
      this.tabPage21.UseVisualStyleBackColor = true;
      this.textBox3.Location = new Point(89, 29);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new Size(453, 20);
      this.textBox3.TabIndex = 8;
      this.textBox3.TextChanged += new EventHandler(this.textBox3_TextChanged);
      this.label13.AutoSize = true;
      this.label13.Location = new Point(38, 32);
      this.label13.Name = "label13";
      this.label13.Size = new Size(45, 13);
      this.label13.TabIndex = 7;
      this.label13.Text = "Version:";
      this.label12.AutoSize = true;
      this.label12.Location = new Point(89, 84);
      this.label12.Name = "label12";
      this.label12.Size = new Size(56, 13);
      this.label12.TabIndex = 6;
      this.label12.Text = "Undefined";
      this.label11.AutoSize = true;
      this.label11.Location = new Point(6, 84);
      this.label11.Name = "label11";
      this.label11.Size = new Size(77, 13);
      this.label11.TabIndex = 4;
      this.label11.Text = "Last Edit Date:";
      this.textBox2.Location = new Point(89, 55);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new Size(453, 20);
      this.textBox2.TabIndex = 3;
      this.textBox2.TextChanged += new EventHandler(this.textBox2_TextChanged);
      this.label10.AutoSize = true;
      this.label10.Location = new Point(42, 58);
      this.label10.Name = "label10";
      this.label10.Size = new Size(41, 13);
      this.label10.TabIndex = 2;
      this.label10.Text = "Author:";
      this.textBox1.Location = new Point(89, 3);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new Size(453, 20);
      this.textBox1.TabIndex = 1;
      this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
      this.label9.AutoSize = true;
      this.label9.Location = new Point(14, 6);
      this.label9.Name = "label9";
      this.label9.Size = new Size(69, 13);
      this.label9.TabIndex = 0;
      this.label9.Text = "Track Name:";
      this.statusStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripStatusLabel1
      });
      this.statusStrip1.Location = new Point(0, 357);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(739, 22);
      this.statusStrip1.TabIndex = 2;
      this.statusStrip1.Text = "statusStrip1";
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new Size(0, 17);
      this.toolStripButton79.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton79.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton79.Enabled = false;
      this.toolStripButton79.Image = (Image) componentResourceManager.GetObject("toolStripButton79.Image");
      this.toolStripButton79.ImageTransparentColor = Color.Magenta;
      this.toolStripButton79.Name = "toolStripButton79";
      this.toolStripButton79.Size = new Size(23, 22);
      this.toolStripButton79.Text = "Calculate Distances";
      this.toolStripButton79.Click += new EventHandler(this.toolStripButton79_Click);
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new Point(145, 19);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new Size(65, 17);
      this.checkBox1.TabIndex = 22;
      this.checkBox1.Text = "Enabled";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new EventHandler(this.checkBox1_CheckedChanged);
      this.label14.AutoSize = true;
      this.label14.Location = new Point(20, 45);
      this.label14.MinimumSize = new Size(0, 16);
      this.label14.Name = "label14";
      this.label14.Size = new Size(119, 16);
      this.label14.TabIndex = 23;
      this.label14.Text = "Table Generation Mode";
      this.label15.AutoSize = true;
      this.label15.Location = new Point(105, 72);
      this.label15.MinimumSize = new Size(0, 16);
      this.label15.Name = "label15";
      this.label15.Size = new Size(34, 16);
      this.label15.TabIndex = 24;
      this.label15.Text = "Slope";
      this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBox2.FormattingEnabled = true;
      this.comboBox2.Items.AddRange(new object[3]
      {
        (object) "0",
        (object) "1",
        (object) "2"
      });
      this.comboBox2.Location = new Point(145, 42);
      this.comboBox2.Name = "comboBox2";
      this.comboBox2.Size = new Size(121, 21);
      this.comboBox2.TabIndex = 25;
      this.comboBox2.SelectedIndexChanged += new EventHandler(this.comboBox2_SelectedIndexChanged);
      this.comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBox3.FormattingEnabled = true;
      this.comboBox3.Items.AddRange(new object[11]
      {
        (object) "0x8000",
        (object) "0x4000",
        (object) "0x2000",
        (object) "0x1000",
        (object) "0x0800",
        (object) "0x0400",
        (object) "0x0200",
        (object) "0x0100",
        (object) "0x0080",
        (object) "0x0040",
        (object) "0x0020"
      });
      this.comboBox3.Location = new Point(145, 69);
      this.comboBox3.Name = "comboBox3";
      this.comboBox3.Size = new Size(121, 21);
      this.comboBox3.TabIndex = 26;
      this.comboBox3.SelectedIndexChanged += new EventHandler(this.comboBox3_SelectedIndexChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(739, 379);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Controls.Add((Control) this.statusStrip1);
      this.Name = nameof (NKM);
      this.Text = nameof (NKM);
      this.Load += new EventHandler(this.NKM_Load);
      this.Shown += new EventHandler(this.NKM_Shown);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.toolStrip5.ResumeLayout(false);
      this.toolStrip5.PerformLayout();
      this.tabPage2.ResumeLayout(false);
      this.tabPage2.PerformLayout();
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      this.tabPage3.ResumeLayout(false);
      this.tabPage3.PerformLayout();
      this.toolStrip3.ResumeLayout(false);
      this.toolStrip3.PerformLayout();
      this.tabPage4.ResumeLayout(false);
      this.tabPage4.PerformLayout();
      this.toolStrip4.ResumeLayout(false);
      this.toolStrip4.PerformLayout();
      this.tabPage5.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.numericUpDown2.EndInit();
      this.numericUpDown1.EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.tabPage6.ResumeLayout(false);
      this.tabPage6.PerformLayout();
      this.toolStrip6.ResumeLayout(false);
      this.toolStrip6.PerformLayout();
      this.tabPage7.ResumeLayout(false);
      this.tabPage7.PerformLayout();
      this.toolStrip7.ResumeLayout(false);
      this.toolStrip7.PerformLayout();
      this.tabPage20.ResumeLayout(false);
      this.tabPage20.PerformLayout();
      this.toolStrip20.ResumeLayout(false);
      this.toolStrip20.PerformLayout();
      this.tabPage8.ResumeLayout(false);
      this.tabPage8.PerformLayout();
      this.toolStrip8.ResumeLayout(false);
      this.toolStrip8.PerformLayout();
      this.tabPage9.ResumeLayout(false);
      this.tabPage9.PerformLayout();
      this.toolStrip9.ResumeLayout(false);
      this.toolStrip9.PerformLayout();
      this.tabPage10.ResumeLayout(false);
      this.tabPage10.PerformLayout();
      this.toolStrip10.ResumeLayout(false);
      this.toolStrip10.PerformLayout();
      this.tabPage11.ResumeLayout(false);
      this.tabPage11.PerformLayout();
      this.toolStrip11.ResumeLayout(false);
      this.toolStrip11.PerformLayout();
      this.tabPage12.ResumeLayout(false);
      this.tabPage12.PerformLayout();
      this.toolStrip12.ResumeLayout(false);
      this.toolStrip12.PerformLayout();
      this.tabPage13.ResumeLayout(false);
      this.tabPage13.PerformLayout();
      this.toolStrip13.ResumeLayout(false);
      this.toolStrip13.PerformLayout();
      this.tabPage14.ResumeLayout(false);
      this.tabPage14.PerformLayout();
      this.toolStrip14.ResumeLayout(false);
      this.toolStrip14.PerformLayout();
      this.tabPage15.ResumeLayout(false);
      this.tabPage15.PerformLayout();
      this.toolStrip15.ResumeLayout(false);
      this.toolStrip15.PerformLayout();
      this.tabPage16.ResumeLayout(false);
      this.tabPage16.PerformLayout();
      this.toolStrip16.ResumeLayout(false);
      this.toolStrip16.PerformLayout();
      this.tabPage17.ResumeLayout(false);
      this.tabPage17.PerformLayout();
      this.toolStrip17.ResumeLayout(false);
      this.toolStrip17.PerformLayout();
      this.tabPage18.ResumeLayout(false);
      this.tabPage18.PerformLayout();
      this.toolStrip18.ResumeLayout(false);
      this.toolStrip18.PerformLayout();
      this.tabPage19.ResumeLayout(false);
      this.tabPage19.PerformLayout();
      this.toolStrip19.ResumeLayout(false);
      this.toolStrip19.PerformLayout();
      this.tabPage21.ResumeLayout(false);
      this.tabPage21.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

    public NKM(MKDS_Course_Modifier.MKDS.NKM File)
    {
      this.File = File;
      this.InitializeComponent();
      this.Text = "NKM Editor [" + File.NKMI.TrackName + " (" + File.NKMI.Version + ") by " + File.NKMI.Author + "]";
      this.simpleOpenGlControl1.InitializeContexts();
      this.simpleOpenGlControl1.MouseWheel += new MouseEventHandler(this.simpleOpenGlControl1_MouseWheel);
      this.toolStripButton1.Text = LanguageHandler.GetString("base.save");
      this.toolStripButton2.Text = LanguageHandler.GetString("nkm.check");
    }

    public void SetKCL(KCL KCL)
    {
      this.KCL = KCL;
      this.Render(false, new Point(), false);
    }

    private void simpleOpenGlControl1_MouseWheel(object sender, MouseEventArgs e)
    {
      if (e.Delta < 0 && this.scale == 1 || e.Delta > 0 && this.scale == 32)
        return;
      this.scale = (int) ((double) this.scale * (e.Delta < 0 ? 0.5 : 2.0));
      this.vScrollBar1.Maximum = this.scale - 1;
      this.vScrollBar1.Minimum = -(this.scale - 1);
      this.hScrollBar1.Maximum = this.scale - 1;
      this.hScrollBar1.Minimum = -(this.scale - 1);
      if (this.scale == 1)
      {
        this.vScrollBar1.Enabled = false;
        this.hScrollBar1.Enabled = false;
      }
      else
      {
        this.vScrollBar1.Enabled = true;
        this.hScrollBar1.Enabled = true;
      }
      this.Render(false, new Point(), false);
    }

    private void NKM_Load(object sender, EventArgs e)
    {
      Gl.glEnable(2903);
      Gl.glEnable(2929);
      Gl.glDepthFunc(519);
      Gl.glEnable(3057);
      Gl.glDisable(2884);
      Gl.glEnable(3553);
      Gl.glEnable(2848);
      Gl.glEnable(3042);
      Gl.glBlendFunc(770, 771);
      int index1 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.OBJIEntry objiEntry in this.File.OBJI)
      {
        this.listView1.Items.Add((ListViewItem) objiEntry);
        this.listView1.Items[index1].Text = index1.ToString();
        ++index1;
      }
      int index2 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.PATHEntry pathEntry in this.File.PATH)
      {
        this.listViewNF1.Items.Add((ListViewItem) pathEntry);
        this.listViewNF1.Items[index2].Text = index2.ToString();
        ++index2;
      }
      int index3 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.POITEntry poitEntry in this.File.POIT)
      {
        this.listViewNF2.Items.Add((ListViewItem) poitEntry);
        this.listViewNF2.Items[index3].Text = index3.ToString();
        ++index3;
      }
      this.comboBox1.Text = this.File.STAG.NrLaps.ToString();
      this.checkBox1.Checked = this.File.STAG.FogEnabled;
      this.comboBox2.SelectedIndex = (int) this.File.STAG.FogTableGenMode;
      this.comboBox3.SelectedIndex = (int) this.File.STAG.FogSlope;
      this.numericUpDown1.Value = (Decimal) this.File.STAG.FogDensity;
      this.panel5.BackColor = this.File.STAG.FogColor;
      this.numericUpDown2.Value = (Decimal) this.File.STAG.FogAlpha;
      this.panel1.BackColor = this.File.STAG.KclColor1;
      this.panel2.BackColor = this.File.STAG.KclColor2;
      this.panel3.BackColor = this.File.STAG.KclColor3;
      this.panel4.BackColor = this.File.STAG.KclColor4;
      int index4 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.KTPSEntry ktpsEntry in this.File.KTPS)
      {
        this.listViewNF3.Items.Add((ListViewItem) ktpsEntry);
        this.listViewNF3.Items[index4].Text = index4.ToString();
        ++index4;
      }
      int index5 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.KTPJEntry ktpjEntry in this.File.KTPJ)
      {
        this.listViewNF4.Items.Add((ListViewItem) ktpjEntry);
        this.listViewNF4.Items[index5].Text = index5.ToString();
        ++index5;
      }
      int index6 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.KTP2Entry ktP2Entry in this.File.KTP2)
      {
        this.listViewNF5.Items.Add((ListViewItem) ktP2Entry);
        this.listViewNF5.Items[index6].Text = index6.ToString();
        ++index6;
      }
      int index7 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.KTPCEntry ktpcEntry in this.File.KTPC)
      {
        this.listViewNF6.Items.Add((ListViewItem) ktpcEntry);
        this.listViewNF6.Items[index7].Text = index7.ToString();
        ++index7;
      }
      int index8 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.KTPMEntry ktpmEntry in this.File.KTPM)
      {
        this.listViewNF7.Items.Add((ListViewItem) ktpmEntry);
        this.listViewNF7.Items[index8].Text = index8.ToString();
        ++index8;
      }
      int index9 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.CPOIEntry cpoiEntry in this.File.CPOI)
      {
        this.listViewNF8.Items.Add((ListViewItem) cpoiEntry);
        this.listViewNF8.Items[index9].Text = index9.ToString();
        ++index9;
      }
      int index10 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.CPATEntry cpatEntry in this.File.CPAT)
      {
        this.listViewNF9.Items.Add((ListViewItem) cpatEntry);
        this.listViewNF9.Items[index10].Text = index10.ToString();
        ++index10;
      }
      int index11 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.IPOIEntry ipoiEntry in this.File.IPOI)
      {
        this.listViewNF10.Items.Add((ListViewItem) ipoiEntry);
        this.listViewNF10.Items[index11].Text = index11.ToString();
        ++index11;
      }
      int index12 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.IPATEntry ipatEntry in this.File.IPAT)
      {
        this.listViewNF11.Items.Add((ListViewItem) ipatEntry);
        this.listViewNF11.Items[index12].Text = index12.ToString();
        ++index12;
      }
      if (this.File.EPOI != null)
      {
        this.tabControl1.TabPages.RemoveAt(16);
        this.tabControl1.TabPages.RemoveAt(16);
        int index13 = 0;
        foreach (MKDS_Course_Modifier.MKDS.NKM.EPOIEntry epoiEntry in this.File.EPOI)
        {
          this.listViewNF12.Items.Add((ListViewItem) epoiEntry);
          this.listViewNF12.Items[index13].Text = index13.ToString();
          ++index13;
        }
        int index14 = 0;
        foreach (MKDS_Course_Modifier.MKDS.NKM.EPATEntry epatEntry in this.File.EPAT)
        {
          this.listViewNF13.Items.Add((ListViewItem) epatEntry);
          this.listViewNF13.Items[index14].Text = index14.ToString();
          ++index14;
        }
      }
      else
      {
        this.tabControl1.TabPages.RemoveAt(14);
        this.tabControl1.TabPages.RemoveAt(14);
        int index13 = 0;
        foreach (MKDS_Course_Modifier.MKDS.NKM.MEPOEntry mepoEntry in this.File.MEPO)
        {
          this.listViewNF14.Items.Add((ListViewItem) mepoEntry);
          this.listViewNF14.Items[index13].Text = index13.ToString();
          ++index13;
        }
        int index14 = 0;
        foreach (MKDS_Course_Modifier.MKDS.NKM.MEPAEntry mepaEntry in this.File.MEPA)
        {
          this.listViewNF15.Items.Add((ListViewItem) mepaEntry);
          this.listViewNF15.Items[index14].Text = index14.ToString();
          ++index14;
        }
      }
      int index15 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.AREAEntry areaEntry in this.File.AREA)
      {
        this.listViewNF16.Items.Add((ListViewItem) areaEntry);
        this.listViewNF16.Items[index15].Text = index15.ToString();
        ++index15;
      }
      int index16 = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.CAMEEntry cameEntry in this.File.CAME)
      {
        this.listViewNF17.Items.Add((ListViewItem) cameEntry);
        this.listViewNF17.Items[index16].Text = index16.ToString();
        ++index16;
      }
      this.textBox1.Text = this.File.NKMI.TrackName;
      this.textBox2.Text = this.File.NKMI.Author;
      this.textBox3.Text = this.File.NKMI.Version;
      this.label12.Text = this.File.NKMI.LastEditDate;
    }

    private void Render(bool pick = false, Point mousepoint = default (Point), bool kclpick = false)
    {
      Gl.glMatrixMode(5889);
      Gl.glLoadIdentity();
      Gl.glViewport(0, 0, this.simpleOpenGlControl1.Width, this.simpleOpenGlControl1.Height);
      float num1 = 8192f / (float) this.scale / (float) this.simpleOpenGlControl1.Width * 2f;
      float num2 = 8192f / (float) this.scale / (float) this.simpleOpenGlControl1.Height * 2f;
      if ((double) num1 > (double) num2)
      {
        Gl.glOrtho(-((double) num1 * (double) this.simpleOpenGlControl1.Width) / 2.0 + (double) this.hScrollBar1.Value * (8192.0 / (double) this.scale), (double) num1 * (double) this.simpleOpenGlControl1.Width / 2.0 + (double) this.hScrollBar1.Value * (8192.0 / (double) this.scale), (double) num1 * (double) this.simpleOpenGlControl1.Height / 2.0 + (double) this.vScrollBar1.Value * (8192.0 / (double) this.scale), -((double) num1 * (double) this.simpleOpenGlControl1.Height) / 2.0 + (double) this.vScrollBar1.Value * (8192.0 / (double) this.scale), -8192.0, 8192.0);
        this.mult = num1;
      }
      else
      {
        Gl.glOrtho(-((double) num2 * (double) this.simpleOpenGlControl1.Width) / 2.0 + (double) this.hScrollBar1.Value * (8192.0 / (double) this.scale), (double) num2 * (double) this.simpleOpenGlControl1.Width / 2.0 + (double) this.hScrollBar1.Value * (8192.0 / (double) this.scale), (double) num2 * (double) this.simpleOpenGlControl1.Height / 2.0 + (double) this.vScrollBar1.Value * (8192.0 / (double) this.scale), -((double) num2 * (double) this.simpleOpenGlControl1.Height) / 2.0 + (double) this.vScrollBar1.Value * (8192.0 / (double) this.scale), -8192.0, 8192.0);
        this.mult = num2;
      }
      Gl.glMatrixMode(5888);
      Gl.glLoadIdentity();
      Gl.glClearColor(1f, 1f, 1f, 1f);
      Gl.glClear(16640);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glEnable(3553);
      Gl.glBindTexture(3553, 0);
      Gl.glColor4f(1f, 1f, 1f, 1f);
      Gl.glDisable(2884);
      Gl.glEnable(3008);
      Gl.glEnable(3042);
      Gl.glEnable(2832);
      Gl.glBlendFunc(770, 771);
      Gl.glAlphaFunc(519, 0.0f);
      if (pick)
      {
        Gl.glLoadIdentity();
        Gl.glDisable(2881);
        Gl.glDisable(2832);
        if (!kclpick)
        {
          this.RenderNKM(true);
        }
        else
        {
          Gl.glDepthFunc(515);
          this.KCL.Render(true);
          Gl.glDepthFunc(519);
        }
        this.pic = new byte[4];
        Gl.glReadPixels(mousepoint.X, this.simpleOpenGlControl1.Height - mousepoint.Y, 1, 1, 32993, 5121, (object) this.pic);
        Gl.glClear(16640);
        this.Render(false, new Point(), false);
      }
      else
      {
        Gl.glLoadIdentity();
        this.RenderNKM(false);
        this.simpleOpenGlControl1.Refresh();
      }
    }

    private void RenderNKM(bool picking = false)
    {
      if (this.first && this.KCL != null)
        this.first = false;
      if (!picking)
        Gl.glEnable(2881);
      if (!picking && this.KCL != null)
      {
        Gl.glDepthFunc(515);
        this.KCL.Render(false);
        Gl.glDepthFunc(519);
      }
      Gl.glPointSize(picking ? 6f : 5f);
      Gl.glBegin(0);
      if (!picking)
        Gl.glColor3f(0.0f, 0.0f, 0.5f);
      int num1 = 1;
      if (this.pOITToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.POITEntry poitEntry in this.File.POIT)
        {
          if (picking)
          {
            Color color = Color.FromArgb(num1 | 524288);
            double num2 = (double) color.R / (double) byte.MaxValue;
            color = Color.FromArgb(num1 | 524288);
            double num3 = (double) color.G / (double) byte.MaxValue;
            color = Color.FromArgb(num1 | 524288);
            double num4 = (double) color.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num1;
          }
          Gl.glVertex2f(poitEntry.Position.X, poitEntry.Position.Z);
        }
      }
      Gl.glEnd();
      Gl.glLineWidth(1.5f);
      int index1 = 0;
      if (this.pOITToolStripMenuItem.Checked && !picking)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.PATHEntry pathEntry in this.File.PATH)
        {
          if ((long) this.File.POIT.NrEntries >= (long) ((int) pathEntry.NrPoit + index1))
          {
            Gl.glBegin(3);
            for (int index2 = 0; index2 < (int) pathEntry.NrPoit; ++index2)
            {
              Gl.glVertex2f(this.File.POIT[index1 + index2].Position.X, this.File.POIT[index1 + index2].Position.Z);
              if (index2 + 1 >= (int) pathEntry.NrPoit && pathEntry.Loop)
                Gl.glVertex2f(this.File.POIT[index1].Position.X, this.File.POIT[index1].Position.Z);
            }
            Gl.glEnd();
            index1 += (int) pathEntry.NrPoit;
          }
          else
            break;
        }
      }
      Gl.glBegin(0);
      if (!picking)
        Gl.glColor3f(1f, 0.0f, 0.0f);
      int argb = 1;
      if (this.oBJIToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.OBJIEntry objiEntry in this.File.OBJI)
        {
          if (picking)
          {
            Color color = Color.FromArgb(argb);
            double num2 = (double) color.R / (double) byte.MaxValue;
            color = Color.FromArgb(argb);
            double num3 = (double) color.G / (double) byte.MaxValue;
            color = Color.FromArgb(argb);
            double num4 = (double) color.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++argb;
          }
          System.Drawing.Bitmap bitmap;
          if ((bitmap = (System.Drawing.Bitmap) OBJI.ResourceManager.GetObject("OBJ_" + BitConverter.ToString(BitConverter.GetBytes(objiEntry.ObjectID), 0).Replace("-", ""))) == null)
          {
            Gl.glVertex2f(objiEntry.Position.X, objiEntry.Position.Z);
          }
          else
          {
            Gl.glEnd();
            if (!picking)
            {
              Gl.glColor3f(1f, 1f, 1f);
              Gl.glBindTexture(3553, (int) objiEntry.ObjectID);
            }
            Gl.glPushMatrix();
            Gl.glTranslatef(objiEntry.Position.X, objiEntry.Position.Z, 0.0f);
            Gl.glRotatef(objiEntry.Rotation.Y, 0.0f, 0.0f, 1f);
            Gl.glScalef(this.mult, this.mult, 1f);
            Gl.glBegin(7);
            Gl.glTexCoord2f(0.0f, 0.0f);
            Gl.glVertex2f((float) -bitmap.Width / 2f, (float) -bitmap.Height / 2f);
            Gl.glTexCoord2f(1f, 0.0f);
            Gl.glVertex2f((float) bitmap.Width / 2f, (float) -bitmap.Height / 2f);
            Gl.glTexCoord2f(1f, 1f);
            Gl.glVertex2f((float) bitmap.Width / 2f, (float) bitmap.Height / 2f);
            Gl.glTexCoord2f(0.0f, 1f);
            Gl.glVertex2f((float) -bitmap.Width / 2f, (float) bitmap.Height / 2f);
            Gl.glEnd();
            Gl.glPopMatrix();
            if (!picking)
            {
              Gl.glColor3f(1f, 0.0f, 0.0f);
              Gl.glBindTexture(3553, 0);
            }
            Gl.glBegin(0);
          }
        }
      }
      if (!picking)
        Gl.glColor3f(0.0f, 0.0f, 0.0f);
      int num5 = 1;
      if (this.kTPSToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.KTPSEntry ktpsEntry in this.File.KTPS)
        {
          if (picking)
          {
            Color color = Color.FromArgb(num5 | 1048576);
            double num2 = (double) color.R / (double) byte.MaxValue;
            color = Color.FromArgb(num5 | 1048576);
            double num3 = (double) color.G / (double) byte.MaxValue;
            color = Color.FromArgb(num5 | 1048576);
            double num4 = (double) color.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num5;
          }
          System.Drawing.Bitmap bitmap;
          if ((bitmap = (System.Drawing.Bitmap) OBJI.ResourceManager.GetObject("start")) == null)
          {
            Gl.glVertex2f(ktpsEntry.Position.X, ktpsEntry.Position.Z);
          }
          else
          {
            Gl.glEnd();
            if (!picking)
            {
              Gl.glColor3f(1f, 1f, 1f);
              Gl.glBindTexture(3553, -1);
            }
            Gl.glPushMatrix();
            Gl.glTranslatef(ktpsEntry.Position.X, ktpsEntry.Position.Z, 0.0f);
            Gl.glRotatef(ktpsEntry.Rotation.Y, 0.0f, 0.0f, 1f);
            Gl.glScalef(this.mult, this.mult, 1f);
            Gl.glBegin(7);
            Gl.glTexCoord2f(0.0f, 0.0f);
            Gl.glVertex2f((float) -bitmap.Width / 2f, (float) -bitmap.Height / 2f);
            Gl.glTexCoord2f(1f, 0.0f);
            Gl.glVertex2f((float) bitmap.Width / 2f, (float) -bitmap.Height / 2f);
            Gl.glTexCoord2f(1f, 1f);
            Gl.glVertex2f((float) bitmap.Width / 2f, (float) bitmap.Height / 2f);
            Gl.glTexCoord2f(0.0f, 1f);
            Gl.glVertex2f((float) -bitmap.Width / 2f, (float) bitmap.Height / 2f);
            Gl.glEnd();
            Gl.glPopMatrix();
            if (!picking)
            {
              Gl.glColor3f(1f, 0.0f, 0.0f);
              Gl.glBindTexture(3553, 0);
            }
            Gl.glBegin(0);
          }
        }
      }
      if (!picking)
        Gl.glColor3f(1f, 0.0f, 0.5f);
      int num6 = 1;
      if (this.kTPCToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.KTPCEntry ktpcEntry in this.File.KTPC)
        {
          if (picking)
          {
            Color color = Color.FromArgb(num6 | 1835008);
            double num2 = (double) color.R / (double) byte.MaxValue;
            color = Color.FromArgb(num6 | 1835008);
            double num3 = (double) color.G / (double) byte.MaxValue;
            color = Color.FromArgb(num6 | 1835008);
            double num4 = (double) color.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num6;
          }
          Gl.glVertex2f(ktpcEntry.Position.X, ktpcEntry.Position.Z);
        }
      }
      if (!picking)
        Gl.glColor3f(0.0f, 0.9f, 1f);
      int num7 = 1;
      if (this.kTP2ToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.KTP2Entry ktP2Entry in this.File.KTP2)
        {
          if (picking)
          {
            Color color = Color.FromArgb(num7 | 1572864);
            double num2 = (double) color.R / (double) byte.MaxValue;
            color = Color.FromArgb(num7 | 1572864);
            double num3 = (double) color.G / (double) byte.MaxValue;
            color = Color.FromArgb(num7 | 1572864);
            double num4 = (double) color.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num7;
          }
          Gl.glVertex2f(ktP2Entry.Position.X, ktP2Entry.Position.Z);
        }
      }
      Color color1;
      if (!picking)
      {
        color1 = Color.MediumPurple;
        double num2 = (double) color1.R / (double) byte.MaxValue;
        color1 = Color.MediumPurple;
        double num3 = (double) color1.G / (double) byte.MaxValue;
        color1 = Color.MediumPurple;
        double num4 = (double) color1.B / (double) byte.MaxValue;
        Gl.glColor3f((float) num2, (float) num3, (float) num4);
      }
      int num8 = 1;
      if (this.kTPMToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.KTPMEntry ktpmEntry in this.File.KTPM)
        {
          if (picking)
          {
            color1 = Color.FromArgb(num8 | 2097152);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num8 | 2097152);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num8 | 2097152);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num8;
          }
          Gl.glVertex2f(ktpmEntry.Position.X, ktpmEntry.Position.Z);
        }
      }
      if (!picking)
        Gl.glColor3f(1f, 0.6f, 0.0f);
      int num9 = 1;
      if (this.kTPJToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.KTPJEntry ktpjEntry in this.File.KTPJ)
        {
          if (picking)
          {
            color1 = Color.FromArgb(num9 | 1310720);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num9 | 1310720);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num9 | 1310720);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num9;
          }
          Gl.glVertex2f(ktpjEntry.Position.X, ktpjEntry.Position.Z);
        }
      }
      if (!picking)
        Gl.glColor3f(0.0f, 0.8f, 0.0f);
      int num10 = 1;
      if (this.ePOIToolStripMenuItem.Checked)
      {
        if (this.File.EPOI != null)
        {
          foreach (MKDS_Course_Modifier.MKDS.NKM.EPOIEntry epoiEntry in this.File.EPOI)
          {
            if (picking)
            {
              color1 = Color.FromArgb(num10 | 3407872);
              double num2 = (double) color1.R / (double) byte.MaxValue;
              color1 = Color.FromArgb(num10 | 3407872);
              double num3 = (double) color1.G / (double) byte.MaxValue;
              color1 = Color.FromArgb(num10 | 3407872);
              double num4 = (double) color1.B / (double) byte.MaxValue;
              Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
              ++num10;
            }
            Gl.glVertex2f(epoiEntry.Position.X, epoiEntry.Position.Z);
          }
        }
        else
        {
          foreach (MKDS_Course_Modifier.MKDS.NKM.MEPOEntry mepoEntry in this.File.MEPO)
          {
            if (picking)
            {
              color1 = Color.FromArgb(num10 | 3932160);
              double num2 = (double) color1.R / (double) byte.MaxValue;
              color1 = Color.FromArgb(num10 | 3932160);
              double num3 = (double) color1.G / (double) byte.MaxValue;
              color1 = Color.FromArgb(num10 | 3932160);
              double num4 = (double) color1.B / (double) byte.MaxValue;
              Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
              ++num10;
            }
            Gl.glVertex2f(mepoEntry.Position.X, mepoEntry.Position.Z);
          }
        }
      }
      if (!picking)
        Gl.glColor3f(1f, 0.9f, 0.0f);
      int num11 = 1;
      if (this.iPOIToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.IPOIEntry ipoiEntry in this.File.IPOI)
        {
          if (picking)
          {
            color1 = Color.FromArgb(num11 | 2883584);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num11 | 2883584);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num11 | 2883584);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num11;
          }
          Gl.glVertex2f(ipoiEntry.Position.X, ipoiEntry.Position.Z);
        }
      }
      if (!picking)
      {
        color1 = Color.CornflowerBlue;
        double num2 = (double) color1.R / (double) byte.MaxValue;
        color1 = Color.CornflowerBlue;
        double num3 = (double) color1.G / (double) byte.MaxValue;
        color1 = Color.CornflowerBlue;
        double num4 = (double) color1.B / (double) byte.MaxValue;
        Gl.glColor3f((float) num2, (float) num3, (float) num4);
      }
      int num12 = 1;
      if (this.aREAToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.AREAEntry areaEntry in this.File.AREA)
        {
          if (picking)
          {
            color1 = Color.FromArgb(num12 | 4456448);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num12 | 4456448);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num12 | 4456448);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num12;
          }
          Gl.glVertex2f(areaEntry.Position.X, areaEntry.Position.Z);
        }
      }
      if (!picking)
      {
        color1 = Color.BurlyWood;
        double num2 = (double) color1.R / (double) byte.MaxValue;
        color1 = Color.BurlyWood;
        double num3 = (double) color1.G / (double) byte.MaxValue;
        color1 = Color.BurlyWood;
        double num4 = (double) color1.B / (double) byte.MaxValue;
        Gl.glColor3f((float) num2, (float) num3, (float) num4);
      }
      int num13 = 1;
      if (this.cAMEToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.CAMEEntry cameEntry in this.File.CAME)
        {
          if (picking)
          {
            color1 = Color.FromArgb(num13 | 4718592);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num13 | 4718592);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num13 | 4718592);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
          }
          Gl.glVertex2f(cameEntry.Position1.X, cameEntry.Position1.Z);
          if (picking)
          {
            color1 = Color.FromArgb(num13 | 4980736);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num13 | 4980736);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num13 | 4980736);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
          }
          Gl.glVertex2f(cameEntry.Position2.X, cameEntry.Position2.Z);
          if (picking)
          {
            color1 = Color.FromArgb(num13 | 5242880);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num13 | 5242880);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num13 | 5242880);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num13;
          }
          Gl.glVertex2f(cameEntry.Position3.X, cameEntry.Position3.Z);
        }
      }
      Gl.glEnd();
      if (this.cPOIToolStripMenuItem.Checked && !picking)
      {
        Gl.glBegin(1);
        foreach (MKDS_Course_Modifier.MKDS.NKM.CPOIEntry cpoiEntry in this.File.CPOI)
        {
          Gl.glColor3f(0.0f, 0.6666667f, 0.0f);
          Gl.glVertex2f(cpoiEntry.Position1.X, cpoiEntry.Position1.Y);
          Gl.glColor3f(0.6666667f, 0.0f, 0.0f);
          Gl.glVertex2f(cpoiEntry.Position2.X, cpoiEntry.Position2.Y);
        }
        for (int index2 = 0; (long) index2 < (long) this.File.CPAT.NrEntries && (long) this.File.CPOI.NrEntries >= (long) ((int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length); ++index2)
        {
          for (int startIdx = (int) this.File.CPAT[index2].StartIdx; startIdx < (int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1; ++startIdx)
          {
            Gl.glColor3f(0.0f, 0.6666667f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[startIdx].Position1.X, this.File.CPOI[startIdx].Position1.Y);
            Gl.glVertex2f(this.File.CPOI[startIdx + 1].Position1.X, this.File.CPOI[startIdx + 1].Position1.Y);
            Gl.glColor3f(0.6666667f, 0.0f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[startIdx].Position2.X, this.File.CPOI[startIdx].Position2.Y);
            Gl.glVertex2f(this.File.CPOI[startIdx + 1].Position2.X, this.File.CPOI[startIdx + 1].Position2.Y);
          }
          if (this.File.CPAT[index2].GoesTo1 != (sbyte) -1)
          {
            Gl.glColor3f(0.0f, 0.6666667f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position1.X, this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position1.Y);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo1].StartIdx].Position1.X, this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo1].StartIdx].Position1.Y);
            Gl.glColor3f(0.6666667f, 0.0f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position2.X, this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position2.Y);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo1].StartIdx].Position2.X, this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo1].StartIdx].Position2.Y);
          }
          if (this.File.CPAT[index2].GoesTo2 != (sbyte) -1)
          {
            Gl.glColor3f(0.0f, 0.6666667f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position1.X, this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position1.Y);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo2].StartIdx].Position1.X, this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo2].StartIdx].Position1.Y);
            Gl.glColor3f(0.6666667f, 0.0f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position2.X, this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position2.Y);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo2].StartIdx].Position2.X, this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo2].StartIdx].Position2.Y);
          }
          if (this.File.CPAT[index2].GoesTo3 != (sbyte) -1)
          {
            Gl.glColor3f(0.0f, 0.6666667f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position1.X, this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position1.Y);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo3].StartIdx].Position1.X, this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo3].StartIdx].Position1.Y);
            Gl.glColor3f(0.6666667f, 0.0f, 0.0f);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position2.X, this.File.CPOI[(int) this.File.CPAT[index2].StartIdx + (int) this.File.CPAT[index2].Length - 1].Position2.Y);
            Gl.glVertex2f(this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo3].StartIdx].Position2.X, this.File.CPOI[(int) this.File.CPAT[(int) this.File.CPAT[index2].GoesTo3].StartIdx].Position2.Y);
          }
        }
        Gl.glEnd();
      }
      Gl.glBegin(0);
      int num14 = 1;
      if (this.cPOIToolStripMenuItem.Checked)
      {
        foreach (MKDS_Course_Modifier.MKDS.NKM.CPOIEntry cpoiEntry in this.File.CPOI)
        {
          if (!picking)
            Gl.glColor3f(0.0f, 0.6666667f, 0.0f);
          if (picking)
          {
            color1 = Color.FromArgb(num14 | 5505024);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num14 | 5505024);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num14 | 5505024);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
          }
          Gl.glVertex2f(cpoiEntry.Position1.X, cpoiEntry.Position1.Y);
          if (!picking)
            Gl.glColor3f(0.6666667f, 0.0f, 0.0f);
          if (picking)
          {
            color1 = Color.FromArgb(num14 | 5767168);
            double num2 = (double) color1.R / (double) byte.MaxValue;
            color1 = Color.FromArgb(num14 | 5767168);
            double num3 = (double) color1.G / (double) byte.MaxValue;
            color1 = Color.FromArgb(num14 | 5767168);
            double num4 = (double) color1.B / (double) byte.MaxValue;
            Gl.glColor4f((float) num2, (float) num3, (float) num4, 1f);
            ++num14;
          }
          Gl.glVertex2f(cpoiEntry.Position2.X, cpoiEntry.Position2.Y);
        }
      }
      Gl.glEnd();
    }

    private void simpleOpenGlControl1_Resize(object sender, EventArgs e)
    {
      this.Render(false, new Point(), false);
    }

    private void NKM_Shown(object sender, EventArgs e)
    {
      System.Drawing.Bitmap obj2D01 = OBJI.OBJ_2D01;
      foreach (object resource in OBJI.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, false, false))
      {
        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap) ((DictionaryEntry) resource).Value;
        BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        Gl.glTexEnvf(8960, 8704, 8448f);
        if ((string) ((DictionaryEntry) resource).Key != "start")
          Gl.glBindTexture(3553, (int) BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(ushort.Parse(((string) ((DictionaryEntry) resource).Key).Split('_')[1], NumberStyles.HexNumber))).Reverse<byte>().ToArray<byte>(), 0));
        else
          Gl.glBindTexture(3553, -1);
        Gl.glTexImage2D(3553, 0, 32856, bitmap.Width, bitmap.Height, 0, 32993, 5121, bitmapdata.Scan0);
        bitmap.UnlockBits(bitmapdata);
        Gl.glTexParameteri(3553, 10242, 10496);
        Gl.glTexParameteri(3553, 10243, 10496);
        Gl.glTexParameteri(3553, 10241, 9728);
        Gl.glTexParameteri(3553, 10240, 9728);
      }
      this.Render(false, new Point(), false);
      this.Render(false, new Point(), false);
    }

    private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      this.Render(false, new Point(), false);
    }

    private void simpleOpenGlControl1_MouseUp(object sender, MouseEventArgs e)
    {
      if (!this.objidraw)
      {
        this.Render(true, e.Location, false);
        int num = Color.FromArgb((int) this.pic[2], (int) this.pic[1], (int) this.pic[0]).ToArgb() & 16777215;
        this.SelectObject(num >> 18, (num & 262143) - 1);
        if (e.Button != MouseButtons.Middle || this.KCL == null)
          return;
        switch (this.SelType)
        {
          case 0:
            float height1 = this.GetHeight(((NKMProperties.OBJI) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.OBJI) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height1))
              ((NKMProperties.OBJI) this.propertyGrid1.SelectedObject).Ty = height1;
            this.SetObject();
            break;
          case 2:
            float height2 = this.GetHeight(((NKMProperties.POIT) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.POIT) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height2))
              ((NKMProperties.POIT) this.propertyGrid1.SelectedObject).Ty = height2;
            this.SetObject();
            break;
          case 4:
            float height3 = this.GetHeight(((NKMProperties.KTPS) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.KTPS) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height3))
              ((NKMProperties.KTPS) this.propertyGrid1.SelectedObject).Ty = height3;
            this.SetObject();
            break;
          case 5:
            float height4 = this.GetHeight(((NKMProperties.KTPJ) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.KTPJ) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height4))
              ((NKMProperties.KTPJ) this.propertyGrid1.SelectedObject).Ty = height4;
            this.SetObject();
            break;
          case 6:
            float height5 = this.GetHeight(((NKMProperties.KTP2) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.KTP2) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height5))
              ((NKMProperties.KTP2) this.propertyGrid1.SelectedObject).Ty = height5;
            this.SetObject();
            break;
          case 7:
            float height6 = this.GetHeight(((NKMProperties.KTPC) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.KTPC) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height6))
              ((NKMProperties.KTPC) this.propertyGrid1.SelectedObject).Ty = height6;
            this.SetObject();
            break;
          case 8:
            float height7 = this.GetHeight(((NKMProperties.KTPM) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.KTPM) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height7))
              ((NKMProperties.KTPM) this.propertyGrid1.SelectedObject).Ty = height7;
            this.SetObject();
            break;
          case 11:
            float height8 = this.GetHeight(((NKMProperties.IPOI) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.IPOI) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height8))
              ((NKMProperties.IPOI) this.propertyGrid1.SelectedObject).Ty = height8;
            this.SetObject();
            break;
          case 13:
            float height9 = this.GetHeight(((NKMProperties.EPOI) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.EPOI) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height9))
              ((NKMProperties.EPOI) this.propertyGrid1.SelectedObject).Ty = height9;
            this.SetObject();
            break;
          case 15:
            float height10 = this.GetHeight(((NKMProperties.MEPO) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.MEPO) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height10))
              ((NKMProperties.MEPO) this.propertyGrid1.SelectedObject).Ty = height10;
            this.SetObject();
            break;
          case 17:
            float height11 = this.GetHeight(((NKMProperties.AREA) this.propertyGrid1.SelectedObject).Tx, ((NKMProperties.AREA) this.propertyGrid1.SelectedObject).Tz, e.Location);
            if (!float.IsNaN(height11))
              ((NKMProperties.AREA) this.propertyGrid1.SelectedObject).Ty = height11;
            this.SetObject();
            break;
          case 18:
            float height12 = this.GetHeight(((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tx1, ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tz1, e.Location);
            if (!float.IsNaN(height12))
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Ty1 = height12;
            this.SetObject();
            break;
          case 19:
            float height13 = this.GetHeight(((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tx2, ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tz2, e.Location);
            if (!float.IsNaN(height13))
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Ty2 = height13;
            this.SetObject();
            break;
          case 20:
            float height14 = this.GetHeight(((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tx3, ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tz3, e.Location);
            if (!float.IsNaN(height14))
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Ty3 = height14;
            this.SetObject();
            break;
        }
      }
      else
      {
        float num1 = (float) ((double) e.X * (double) this.mult + -((double) this.mult * (double) this.simpleOpenGlControl1.Width) / 2.0 + (double) this.hScrollBar1.Value * (8192.0 / (double) this.scale));
        float num2 = (float) ((double) e.Y * (double) this.mult + -((double) this.mult * (double) this.simpleOpenGlControl1.Height) / 2.0 + (double) this.vScrollBar1.Value * (8192.0 / (double) this.scale));
        float num3 = !this.toolStripButton78.Checked || this.KCL == null ? 0.0f : this.GetHeight(num1, num2, e.Location);
        if (float.IsNaN(num3))
          num3 = 0.0f;
        this.File.OBJI.Add(new MKDS_Course_Modifier.MKDS.NKM.OBJIEntry()
        {
          Position = new Vector3(num1, num3, num2)
        });
        ++this.File.OBJI.NrEntries;
        this.listView1.BeginUpdate();
        this.listView1.Items.Clear();
        int index = 0;
        foreach (MKDS_Course_Modifier.MKDS.NKM.NKMEntry nkmEntry in this.File.OBJI)
        {
          this.listView1.Items.Add(nkmEntry.ToListViewItem());
          this.listView1.Items[index].Text = index.ToString();
          ++index;
        }
        this.listView1.EndUpdate();
        this.SelectObject(this.SelType, this.SelIdx);
        this.Render(false, new Point(), false);
      }
    }

    public void SelectObject(int Type, int Idx)
    {
      switch (Type)
      {
        case 0:
          this.toolStripStatusLabel1.Text = "OBJI " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.OBJI) this.File.OBJI[Idx];
          this.listView1.SelectedIndices.Clear();
          this.listView1.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listView1);
          break;
        case 1:
          this.toolStripStatusLabel1.Text = "PATH " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.PATH) this.File.PATH[Idx];
          this.listViewNF1.SelectedIndices.Clear();
          this.listViewNF1.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF1);
          break;
        case 2:
          this.toolStripStatusLabel1.Text = "POIT " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.POIT) this.File.POIT[Idx];
          this.listViewNF2.SelectedIndices.Clear();
          this.listViewNF2.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF2);
          break;
        case 4:
          this.toolStripStatusLabel1.Text = "KTPS " + (object) this.SelIdx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.KTPS) this.File.KTPS[Idx];
          this.listViewNF3.SelectedIndices.Clear();
          this.listViewNF3.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF3);
          break;
        case 5:
          this.toolStripStatusLabel1.Text = "KTPJ " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.KTPJ) this.File.KTPJ[Idx];
          this.listViewNF4.SelectedIndices.Clear();
          this.listViewNF4.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF4);
          break;
        case 6:
          this.toolStripStatusLabel1.Text = "KTP2 " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.KTP2) this.File.KTP2[Idx];
          this.listViewNF5.SelectedIndices.Clear();
          this.listViewNF5.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF5);
          break;
        case 7:
          this.toolStripStatusLabel1.Text = "KTPC " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.KTPC) this.File.KTPC[Idx];
          this.listViewNF6.SelectedIndices.Clear();
          this.listViewNF6.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF6);
          break;
        case 8:
          this.toolStripStatusLabel1.Text = "KTPM " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.KTPM) this.File.KTPM[Idx];
          this.listViewNF7.SelectedIndices.Clear();
          this.listViewNF7.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF7);
          break;
        case 10:
          this.toolStripStatusLabel1.Text = "CPAT " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.CPAT) this.File.CPAT[Idx];
          this.listViewNF9.SelectedIndices.Clear();
          this.listViewNF9.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF9);
          break;
        case 11:
          this.toolStripStatusLabel1.Text = "IPOI " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.IPOI) this.File.IPOI[Idx];
          this.listViewNF10.SelectedIndices.Clear();
          this.listViewNF10.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF10);
          break;
        case 12:
          this.toolStripStatusLabel1.Text = "IPAT " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.IPAT) this.File.IPAT[Idx];
          this.listViewNF11.SelectedIndices.Clear();
          this.listViewNF11.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF11);
          break;
        case 13:
          this.toolStripStatusLabel1.Text = "EPOI " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.EPOI) this.File.EPOI[Idx];
          this.listViewNF12.SelectedIndices.Clear();
          this.listViewNF12.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF12);
          break;
        case 14:
          this.toolStripStatusLabel1.Text = "EPAT " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.EPAT) this.File.EPAT[Idx];
          this.listViewNF13.SelectedIndices.Clear();
          this.listViewNF13.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF13);
          break;
        case 15:
          this.toolStripStatusLabel1.Text = "MEPO " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.MEPO) this.File.MEPO[Idx];
          this.listViewNF14.SelectedIndices.Clear();
          this.listViewNF14.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF14);
          break;
        case 16:
          this.toolStripStatusLabel1.Text = "MEPA " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.MEPA) this.File.MEPA[Idx];
          this.listViewNF15.SelectedIndices.Clear();
          this.listViewNF15.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF15);
          break;
        case 17:
          this.toolStripStatusLabel1.Text = "AREA " + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.AREA) this.File.AREA[Idx];
          this.listViewNF16.SelectedIndices.Clear();
          this.listViewNF16.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF16);
          break;
        case 18:
        case 19:
        case 20:
          string str = "";
          if (this.SelType == 18)
            str = "Left) ";
          if (this.SelType == 19)
            str = "Middle) ";
          if (this.SelType == 20)
            str = "Right) ";
          this.toolStripStatusLabel1.Text = "Came(" + str + (object) Idx + ")";
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.CAME) this.File.CAME[Idx];
          this.listViewNF17.SelectedIndices.Clear();
          this.listViewNF17.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF17);
          break;
        case 21:
        case 22:
          this.toolStripStatusLabel1.Text = "CPOI(" + (this.SelType == 21 ? (object) "Left) " : (object) "Right) ") + (object) Idx;
          this.propertyGrid1.SelectedObject = (object) (NKMProperties.CPOI) this.File.CPOI[Idx];
          this.listViewNF8.SelectedIndices.Clear();
          this.listViewNF8.SelectedIndices.Add(Idx);
          this.ListViewSelect(this.listViewNF8);
          break;
        default:
          this.toolStripStatusLabel1.Text = "";
          this.propertyGrid1.SelectedObject = (object) null;
          this.SelType = -1;
          this.SelIdx = -1;
          this.ListViewSelect((Form1.ListViewNF) null);
          return;
      }
      this.SelType = Type;
      this.SelIdx = Idx;
    }

    private void ListViewSelect(Form1.ListViewNF view)
    {
      foreach (Control tabPage in this.tabControl1.TabPages)
      {
        foreach (Control control in (ArrangedElementCollection) tabPage.Controls)
        {
          if (control is Form1.ListViewNF && control != view)
            ((ListView) control).SelectedIndices.Clear();
        }
      }
    }

    private void SetObject()
    {
      switch (this.SelType)
      {
        case 0:
          this.File.OBJI[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.OBJIEntry) (NKMProperties.OBJI) this.propertyGrid1.SelectedObject;
          this.listView1.BeginUpdate();
          this.listView1.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.OBJIEntry) (NKMProperties.OBJI) this.propertyGrid1.SelectedObject;
          this.listView1.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listView1.EndUpdate();
          break;
        case 1:
          this.File.PATH[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.PATHEntry) (NKMProperties.PATH) this.propertyGrid1.SelectedObject;
          this.listViewNF1.BeginUpdate();
          this.listViewNF1.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.PATHEntry) (NKMProperties.PATH) this.propertyGrid1.SelectedObject;
          this.listViewNF1.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF1.EndUpdate();
          break;
        case 2:
          this.File.POIT[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.POITEntry) (NKMProperties.POIT) this.propertyGrid1.SelectedObject;
          this.listViewNF2.BeginUpdate();
          this.listViewNF2.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.POITEntry) (NKMProperties.POIT) this.propertyGrid1.SelectedObject;
          this.listViewNF2.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF2.EndUpdate();
          break;
        case 3:
          return;
        case 4:
          this.File.KTPS[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.KTPSEntry) (NKMProperties.KTPS) this.propertyGrid1.SelectedObject;
          this.listViewNF3.BeginUpdate();
          this.listViewNF3.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.KTPSEntry) (NKMProperties.KTPS) this.propertyGrid1.SelectedObject;
          this.listViewNF3.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF3.EndUpdate();
          break;
        case 5:
          this.File.KTPJ[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.KTPJEntry) (NKMProperties.KTPJ) this.propertyGrid1.SelectedObject;
          this.listViewNF4.BeginUpdate();
          this.listViewNF4.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.KTPJEntry) (NKMProperties.KTPJ) this.propertyGrid1.SelectedObject;
          this.listViewNF4.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF4.EndUpdate();
          break;
        case 6:
          this.File.KTP2[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.KTP2Entry) (NKMProperties.KTP2) this.propertyGrid1.SelectedObject;
          this.listViewNF5.BeginUpdate();
          this.listViewNF5.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.KTP2Entry) (NKMProperties.KTP2) this.propertyGrid1.SelectedObject;
          this.listViewNF5.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF5.EndUpdate();
          break;
        case 7:
          this.File.KTPC[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.KTPCEntry) (NKMProperties.KTPC) this.propertyGrid1.SelectedObject;
          this.listViewNF6.BeginUpdate();
          this.listViewNF6.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.KTPCEntry) (NKMProperties.KTPC) this.propertyGrid1.SelectedObject;
          this.listViewNF6.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF6.EndUpdate();
          break;
        case 8:
          this.File.KTPM[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.KTPMEntry) (NKMProperties.KTPM) this.propertyGrid1.SelectedObject;
          this.listViewNF7.BeginUpdate();
          this.listViewNF7.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.KTPMEntry) (NKMProperties.KTPM) this.propertyGrid1.SelectedObject;
          this.listViewNF7.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF7.EndUpdate();
          break;
        case 9:
          return;
        case 10:
          this.File.CPAT[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.CPATEntry) (NKMProperties.CPAT) this.propertyGrid1.SelectedObject;
          this.listViewNF9.BeginUpdate();
          this.listViewNF9.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.CPATEntry) (NKMProperties.CPAT) this.propertyGrid1.SelectedObject;
          this.listViewNF9.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF9.EndUpdate();
          break;
        case 11:
          this.File.IPOI[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.IPOIEntry) (NKMProperties.IPOI) this.propertyGrid1.SelectedObject;
          this.listViewNF10.BeginUpdate();
          this.listViewNF10.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.IPOIEntry) (NKMProperties.IPOI) this.propertyGrid1.SelectedObject;
          this.listViewNF10.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF10.EndUpdate();
          break;
        case 12:
          this.File.IPAT[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.IPATEntry) (NKMProperties.IPAT) this.propertyGrid1.SelectedObject;
          this.listViewNF11.BeginUpdate();
          this.listViewNF11.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.IPATEntry) (NKMProperties.IPAT) this.propertyGrid1.SelectedObject;
          this.listViewNF11.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF11.EndUpdate();
          break;
        case 13:
          this.File.EPOI[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.EPOIEntry) (NKMProperties.EPOI) this.propertyGrid1.SelectedObject;
          this.listViewNF12.BeginUpdate();
          this.listViewNF12.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.EPOIEntry) (NKMProperties.EPOI) this.propertyGrid1.SelectedObject;
          this.listViewNF12.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF12.EndUpdate();
          break;
        case 14:
          this.File.EPAT[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.EPATEntry) (NKMProperties.EPAT) this.propertyGrid1.SelectedObject;
          this.listViewNF13.BeginUpdate();
          this.listViewNF13.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.EPATEntry) (NKMProperties.EPAT) this.propertyGrid1.SelectedObject;
          this.listViewNF13.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF13.EndUpdate();
          break;
        case 15:
          this.File.MEPO[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.MEPOEntry) (NKMProperties.MEPO) this.propertyGrid1.SelectedObject;
          this.listViewNF14.BeginUpdate();
          this.listViewNF14.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.MEPOEntry) (NKMProperties.MEPO) this.propertyGrid1.SelectedObject;
          this.listViewNF14.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF14.EndUpdate();
          break;
        case 16:
          this.File.MEPA[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.MEPAEntry) (NKMProperties.MEPA) this.propertyGrid1.SelectedObject;
          this.listViewNF15.BeginUpdate();
          this.listViewNF15.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.MEPAEntry) (NKMProperties.MEPA) this.propertyGrid1.SelectedObject;
          this.listViewNF15.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF15.EndUpdate();
          break;
        case 17:
          this.File.AREA[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.AREAEntry) (NKMProperties.AREA) this.propertyGrid1.SelectedObject;
          this.listViewNF16.BeginUpdate();
          this.listViewNF16.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.AREAEntry) (NKMProperties.AREA) this.propertyGrid1.SelectedObject;
          this.listViewNF16.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF16.EndUpdate();
          break;
        case 18:
        case 19:
        case 20:
          this.File.CAME[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.CAMEEntry) (NKMProperties.CAME) this.propertyGrid1.SelectedObject;
          this.listViewNF17.BeginUpdate();
          this.listViewNF17.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.CAMEEntry) (NKMProperties.CAME) this.propertyGrid1.SelectedObject;
          this.listViewNF17.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF17.EndUpdate();
          break;
        case 21:
        case 22:
          this.File.CPOI[this.SelIdx] = (MKDS_Course_Modifier.MKDS.NKM.CPOIEntry) (NKMProperties.CPOI) this.propertyGrid1.SelectedObject;
          this.listViewNF8.BeginUpdate();
          this.listViewNF8.Items[this.SelIdx] = (ListViewItem) (MKDS_Course_Modifier.MKDS.NKM.CPOIEntry) (NKMProperties.CPOI) this.propertyGrid1.SelectedObject;
          this.listViewNF8.Items[this.SelIdx].Text = this.SelIdx.ToString();
          this.listViewNF8.EndUpdate();
          break;
        default:
          return;
      }
      this.Render(false, new Point(), false);
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (((ListView) sender).SelectedIndices.Count == 0)
        return;
      int Type = -1;
      switch (((Control) sender).Name)
      {
        case "listView1":
          Type = 0;
          break;
        case "listViewNF1":
          Type = 1;
          break;
        case "listViewNF2":
          Type = 2;
          break;
        case "listViewNF3":
          Type = 4;
          break;
        case "listViewNF4":
          Type = 5;
          break;
        case "listViewNF5":
          Type = 6;
          break;
        case "listViewNF6":
          Type = 7;
          break;
        case "listViewNF7":
          Type = 8;
          break;
        case "listViewNF8":
          Type = 21;
          break;
        case "listViewNF9":
          Type = 10;
          break;
        case "listViewNF10":
          Type = 11;
          break;
        case "listViewNF11":
          Type = 12;
          break;
        case "listViewNF12":
          Type = 13;
          break;
        case "listViewNF13":
          Type = 14;
          break;
        case "listViewNF14":
          Type = 15;
          break;
        case "listViewNF15":
          Type = 16;
          break;
        case "listViewNF16":
          Type = 17;
          break;
        case "listViewNF17":
          Type = 18;
          break;
      }
      this.SelectObject(Type, ((ListView) sender).SelectedIndices[0]);
    }

    private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
    {
      if (e.TabPageIndex != 0)
        return;
      this.Render(false, new Point(), false);
      this.simpleOpenGlControl1.Focus();
      this.simpleOpenGlControl1.Select();
    }

    private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
      this.SetObject();
      if (!(this.propertyGrid1.SelectedObject is NKMProperties.OBJI))
        return;
      this.SelectObject(this.SelType, this.SelIdx);
    }

    private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left && this.propertyGrid1.SelectedObject != null)
      {
        if (this.last != new Point(int.MinValue, int.MinValue))
        {
          Point point = new Point(-(this.last.X - (int) ((double) e.Location.X * (double) this.mult)), -(this.last.Y - (int) ((double) e.Location.Y * (double) this.mult)));
          switch (this.SelType)
          {
            case 0:
              ((NKMProperties.OBJI) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.OBJI) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 2:
              ((NKMProperties.POIT) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.POIT) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 4:
              ((NKMProperties.KTPS) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.KTPS) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 5:
              ((NKMProperties.KTPJ) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.KTPJ) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 6:
              ((NKMProperties.KTP2) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.KTP2) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 7:
              ((NKMProperties.KTPC) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.KTPC) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 8:
              ((NKMProperties.KTPM) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.KTPM) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 11:
              ((NKMProperties.IPOI) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.IPOI) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 13:
              ((NKMProperties.EPOI) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.EPOI) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 15:
              ((NKMProperties.MEPO) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.MEPO) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 17:
              ((NKMProperties.AREA) this.propertyGrid1.SelectedObject).Tx += (float) point.X;
              ((NKMProperties.AREA) this.propertyGrid1.SelectedObject).Tz += (float) point.Y;
              this.SetObject();
              break;
            case 18:
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tx1 += (float) point.X;
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tz1 += (float) point.Y;
              this.SetObject();
              break;
            case 19:
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tx2 += (float) point.X;
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tz2 += (float) point.Y;
              this.SetObject();
              break;
            case 20:
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tx3 += (float) point.X;
              ((NKMProperties.CAME) this.propertyGrid1.SelectedObject).Tz3 += (float) point.Y;
              this.SetObject();
              break;
            case 21:
              ((NKMProperties.CPOI) this.propertyGrid1.SelectedObject).Tx1 += (float) point.X;
              ((NKMProperties.CPOI) this.propertyGrid1.SelectedObject).Tz1 += (float) point.Y;
              this.SetObject();
              break;
            case 22:
              ((NKMProperties.CPOI) this.propertyGrid1.SelectedObject).Tx2 += (float) point.X;
              ((NKMProperties.CPOI) this.propertyGrid1.SelectedObject).Tz2 += (float) point.Y;
              this.SetObject();
              break;
          }
        }
        this.last = e.Location;
        this.last.X = (int) ((double) this.last.X * (double) this.mult);
        this.last.Y = (int) ((double) this.last.Y * (double) this.mult);
      }
      else
      {
        if (!(this.last != new Point(int.MinValue, int.MinValue)))
          return;
        this.last = new Point(int.MinValue, int.MinValue);
      }
    }

    private void simpleOpenGlControl1_MouseDown(object sender, MouseEventArgs e)
    {
      if (this.objidraw)
        return;
      this.Render(true, e.Location, false);
      int num = Color.FromArgb((int) this.pic[2], (int) this.pic[1], (int) this.pic[0]).ToArgb() & 16777215;
      this.SelectObject(num >> 18, (num & 262143) - 1);
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      new NKMCheck(this.File, this).Show();
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      FileHandler.Save(this.File.Save(), 0, false);
      this.label12.Text = this.File.NKMI.LastEditDate;
      this.Text = "NKM Editor [" + this.File.NKMI.TrackName + " (" + this.File.NKMI.Version + ") by " + this.File.NKMI.Author + "]";
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      switch (((ToolStripItem) sender).Name)
      {
        case "toolStripButton3":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.OBJIEntry>(this.File.OBJI, this.listView1);
          break;
        case "toolStripButton7":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.PATHEntry>(this.File.PATH, this.listViewNF1);
          break;
        case "toolStripButton11":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.POITEntry>(this.File.POIT, this.listViewNF2);
          break;
        case "toolStripButton15":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.KTPSEntry>(this.File.KTPS, this.listViewNF3);
          break;
        case "toolStripButton19":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.KTPJEntry>(this.File.KTPJ, this.listViewNF4);
          break;
        case "toolStripButton23":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.KTP2Entry>(this.File.KTP2, this.listViewNF5);
          break;
        case "toolStripButton27":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.KTPCEntry>(this.File.KTPC, this.listViewNF6);
          break;
        case "toolStripButton31":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.KTPMEntry>(this.File.KTPM, this.listViewNF7);
          break;
        case "toolStripButton35":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.CPOIEntry>(this.File.CPOI, this.listViewNF8);
          break;
        case "toolStripButton39":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.CPATEntry>(this.File.CPAT, this.listViewNF9);
          break;
        case "toolStripButton43":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.IPOIEntry>(this.File.IPOI, this.listViewNF10);
          break;
        case "toolStripButton47":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.IPATEntry>(this.File.IPAT, this.listViewNF11);
          break;
        case "toolStripButton51":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.EPOIEntry>(this.File.EPOI, this.listViewNF12);
          break;
        case "toolStripButton55":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.EPATEntry>(this.File.EPAT, this.listViewNF13);
          break;
        case "toolStripButton59":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.MEPOEntry>(this.File.MEPO, this.listViewNF14);
          break;
        case "toolStripButton63":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.MEPAEntry>(this.File.MEPA, this.listViewNF15);
          break;
        case "toolStripButton67":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.AREAEntry>(this.File.AREA, this.listViewNF16);
          break;
        case "toolStripButton71":
          this.Add<MKDS_Course_Modifier.MKDS.NKM.CAMEEntry>(this.File.CAME, this.listViewNF17);
          break;
      }
    }

    private void toolStripButton4_Click(object sender, EventArgs e)
    {
      switch (((ToolStripItem) sender).Name)
      {
        case "toolStripButton4":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.OBJIEntry>(this.File.OBJI, this.listView1);
          break;
        case "toolStripButton8":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.PATHEntry>(this.File.PATH, this.listViewNF1);
          break;
        case "toolStripButton12":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.POITEntry>(this.File.POIT, this.listViewNF2);
          break;
        case "toolStripButton16":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.KTPSEntry>(this.File.KTPS, this.listViewNF3);
          break;
        case "toolStripButton20":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.KTPJEntry>(this.File.KTPJ, this.listViewNF4);
          break;
        case "toolStripButton24":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.KTP2Entry>(this.File.KTP2, this.listViewNF5);
          break;
        case "toolStripButton28":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.KTPCEntry>(this.File.KTPC, this.listViewNF6);
          break;
        case "toolStripButton32":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.KTPMEntry>(this.File.KTPM, this.listViewNF7);
          break;
        case "toolStripButton36":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.CPOIEntry>(this.File.CPOI, this.listViewNF8);
          break;
        case "toolStripButton40":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.CPATEntry>(this.File.CPAT, this.listViewNF9);
          break;
        case "toolStripButton44":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.IPOIEntry>(this.File.IPOI, this.listViewNF10);
          break;
        case "toolStripButton48":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.IPATEntry>(this.File.IPAT, this.listViewNF11);
          break;
        case "toolStripButton52":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.EPOIEntry>(this.File.EPOI, this.listViewNF12);
          break;
        case "toolStripButton56":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.EPATEntry>(this.File.EPAT, this.listViewNF13);
          break;
        case "toolStripButton60":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.MEPOEntry>(this.File.MEPO, this.listViewNF14);
          break;
        case "toolStripButton64":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.MEPAEntry>(this.File.MEPA, this.listViewNF15);
          break;
        case "toolStripButton68":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.AREAEntry>(this.File.AREA, this.listViewNF16);
          break;
        case "toolStripButton72":
          this.Remove<MKDS_Course_Modifier.MKDS.NKM.CAMEEntry>(this.File.CAME, this.listViewNF17);
          break;
      }
    }

    private void toolStripButton5_Click(object sender, EventArgs e)
    {
      switch (((ToolStripItem) sender).Name)
      {
        case "toolStripButton5":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.OBJIEntry>(this.File.OBJI, this.listView1);
          break;
        case "toolStripButton9":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.PATHEntry>(this.File.PATH, this.listViewNF1);
          break;
        case "toolStripButton13":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.POITEntry>(this.File.POIT, this.listViewNF2);
          break;
        case "toolStripButton17":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.KTPSEntry>(this.File.KTPS, this.listViewNF3);
          break;
        case "toolStripButton21":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.KTPJEntry>(this.File.KTPJ, this.listViewNF4);
          break;
        case "toolStripButton25":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.KTP2Entry>(this.File.KTP2, this.listViewNF5);
          break;
        case "toolStripButton29":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.KTPCEntry>(this.File.KTPC, this.listViewNF6);
          break;
        case "toolStripButton33":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.KTPMEntry>(this.File.KTPM, this.listViewNF7);
          break;
        case "toolStripButton37":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.CPOIEntry>(this.File.CPOI, this.listViewNF8);
          break;
        case "toolStripButton41":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.CPATEntry>(this.File.CPAT, this.listViewNF9);
          break;
        case "toolStripButton45":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.IPOIEntry>(this.File.IPOI, this.listViewNF10);
          break;
        case "toolStripButton49":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.IPATEntry>(this.File.IPAT, this.listViewNF11);
          break;
        case "toolStripButton53":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.EPOIEntry>(this.File.EPOI, this.listViewNF12);
          break;
        case "toolStripButton57":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.EPATEntry>(this.File.EPAT, this.listViewNF13);
          break;
        case "toolStripButton61":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.MEPOEntry>(this.File.MEPO, this.listViewNF14);
          break;
        case "toolStripButton65":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.MEPAEntry>(this.File.MEPA, this.listViewNF15);
          break;
        case "toolStripButton69":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.AREAEntry>(this.File.AREA, this.listViewNF16);
          break;
        case "toolStripButton73":
          this.MoveUp<MKDS_Course_Modifier.MKDS.NKM.CAMEEntry>(this.File.CAME, this.listViewNF17);
          break;
      }
    }

    private void toolStripButton6_Click(object sender, EventArgs e)
    {
      switch (((ToolStripItem) sender).Name)
      {
        case "toolStripButton6":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.OBJIEntry>(this.File.OBJI, this.listView1);
          break;
        case "toolStripButton10":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.PATHEntry>(this.File.PATH, this.listViewNF1);
          break;
        case "toolStripButton14":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.POITEntry>(this.File.POIT, this.listViewNF2);
          break;
        case "toolStripButton18":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.KTPSEntry>(this.File.KTPS, this.listViewNF3);
          break;
        case "toolStripButton22":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.KTPJEntry>(this.File.KTPJ, this.listViewNF4);
          break;
        case "toolStripButton26":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.KTP2Entry>(this.File.KTP2, this.listViewNF5);
          break;
        case "toolStripButton30":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.KTPCEntry>(this.File.KTPC, this.listViewNF6);
          break;
        case "toolStripButton34":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.KTPMEntry>(this.File.KTPM, this.listViewNF7);
          break;
        case "toolStripButton38":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.CPOIEntry>(this.File.CPOI, this.listViewNF8);
          break;
        case "toolStripButton42":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.CPATEntry>(this.File.CPAT, this.listViewNF9);
          break;
        case "toolStripButton46":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.IPOIEntry>(this.File.IPOI, this.listViewNF10);
          break;
        case "toolStripButton50":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.IPATEntry>(this.File.IPAT, this.listViewNF11);
          break;
        case "toolStripButton54":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.EPOIEntry>(this.File.EPOI, this.listViewNF12);
          break;
        case "toolStripButton58":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.EPATEntry>(this.File.EPAT, this.listViewNF13);
          break;
        case "toolStripButton62":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.MEPOEntry>(this.File.MEPO, this.listViewNF14);
          break;
        case "toolStripButton66":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.MEPAEntry>(this.File.MEPA, this.listViewNF15);
          break;
        case "toolStripButton70":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.AREAEntry>(this.File.AREA, this.listViewNF16);
          break;
        case "toolStripButton74":
          this.MoveDown<MKDS_Course_Modifier.MKDS.NKM.CAMEEntry>(this.File.CAME, this.listViewNF17);
          break;
      }
    }

    private void Add<T>(MKDS_Course_Modifier.MKDS.NKM.NKMHeader<T> Objects, Form1.ListViewNF lv) where T : MKDS_Course_Modifier.MKDS.NKM.NKMEntry, new()
    {
      Objects.Add(new T());
      ++Objects.NrEntries;
      lv.BeginUpdate();
      lv.Items.Clear();
      int index = 0;
      foreach (T obj in Objects)
      {
        lv.Items.Add(obj.ToListViewItem());
        lv.Items[index].Text = index.ToString();
        ++index;
      }
      lv.EndUpdate();
      this.SelectObject(this.SelType, this.SelIdx);
      this.Render(false, new Point(), false);
    }

    private void Remove<T>(MKDS_Course_Modifier.MKDS.NKM.NKMHeader<T> Objects, Form1.ListViewNF lv) where T : MKDS_Course_Modifier.MKDS.NKM.NKMEntry, new()
    {
      if (lv.SelectedItems.Count == 0)
        return;
      Objects.RemoveAt(this.SelIdx);
      --Objects.NrEntries;
      lv.BeginUpdate();
      lv.Items.Clear();
      int index = 0;
      foreach (T obj in Objects)
      {
        lv.Items.Add(obj.ToListViewItem());
        lv.Items[index].Text = index.ToString();
        ++index;
      }
      lv.EndUpdate();
      if (Objects.NrEntries != 0U)
        this.SelectObject(this.SelType, (long) this.SelIdx == (long) (Objects.NrEntries - 1U) || this.SelIdx == 0 ? this.SelIdx : this.SelIdx - 1);
      else
        this.SelectObject((int) byte.MaxValue, 0);
      this.Render(false, new Point(), false);
    }

    private void MoveUp<T>(MKDS_Course_Modifier.MKDS.NKM.NKMHeader<T> Objects, Form1.ListViewNF lv) where T : MKDS_Course_Modifier.MKDS.NKM.NKMEntry, new()
    {
      if (lv.SelectedItems.Count == 0 || lv.SelectedIndices[0] <= 0)
        return;
      T obj1 = Objects[this.SelIdx];
      Objects.RemoveAt(this.SelIdx);
      Objects.Insert(this.SelIdx - 1, obj1);
      lv.BeginUpdate();
      lv.Items.Clear();
      int index = 0;
      foreach (T obj2 in Objects)
      {
        lv.Items.Add(obj2.ToListViewItem());
        lv.Items[index].Text = index.ToString();
        ++index;
      }
      lv.EndUpdate();
      this.SelectObject(this.SelType, this.SelIdx - 1);
      this.Render(false, new Point(), false);
    }

    private void MoveDown<T>(MKDS_Course_Modifier.MKDS.NKM.NKMHeader<T> Objects, Form1.ListViewNF lv) where T : MKDS_Course_Modifier.MKDS.NKM.NKMEntry, new()
    {
      if (lv.SelectedItems.Count == 0 || lv.SelectedIndices[0] >= lv.Items.Count - 1)
        return;
      T obj1 = Objects[this.SelIdx];
      Objects.RemoveAt(this.SelIdx);
      Objects.Insert(this.SelIdx + 1, obj1);
      lv.BeginUpdate();
      lv.Items.Clear();
      int index = 0;
      foreach (T obj2 in Objects)
      {
        lv.Items.Add(obj2.ToListViewItem());
        lv.Items[index].Text = index.ToString();
        ++index;
      }
      lv.EndUpdate();
      this.SelectObject(this.SelType, this.SelIdx + 1);
      this.Render(false, new Point(), false);
    }

    private float GetHeight(float X, float Z, Point MousePoint)
    {
      float num1 = float.NaN;
      this.Render(true, MousePoint, true);
      int num2 = Color.FromArgb((int) this.pic[2], (int) this.pic[1], (int) this.pic[0]).ToArgb() & 16777215;
      if (num2 == 16777215)
        return num1;
      int num3 = num2 - 1;
      int PlaneIdx = num3;
      if (num3 == -1)
        return num1;
      Vector3 PositionA;
      Vector3 PositionB;
      Vector3 PositionC;
      KCL.GetTriangle(this.KCL, PlaneIdx, out PositionA, out PositionB, out PositionC);
      Vector3 vector3 = Helpers.unit(Vector3.Cross(PositionB - PositionA, PositionC - PositionA));
      return (float) (((double) vector3.X * ((double) X - (double) PositionA.X) + (double) vector3.Z * ((double) Z - (double) PositionA.Z)) / -(double) vector3.Y) + PositionA.Y;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.colorDialog1.Color = this.panel1.BackColor;
      if (this.colorDialog1.ShowDialog() != DialogResult.OK)
        return;
      this.panel1.BackColor = this.colorDialog1.Color;
      this.File.STAG.KclColor1 = this.colorDialog1.Color;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.colorDialog1.Color = this.panel2.BackColor;
      if (this.colorDialog1.ShowDialog() != DialogResult.OK)
        return;
      this.panel2.BackColor = this.colorDialog1.Color;
      this.File.STAG.KclColor2 = this.colorDialog1.Color;
    }

    private void button3_Click(object sender, EventArgs e)
    {
      this.colorDialog1.Color = this.panel3.BackColor;
      if (this.colorDialog1.ShowDialog() != DialogResult.OK)
        return;
      this.panel3.BackColor = this.colorDialog1.Color;
      this.File.STAG.KclColor3 = this.colorDialog1.Color;
    }

    private void button4_Click(object sender, EventArgs e)
    {
      this.colorDialog1.Color = this.panel4.BackColor;
      if (this.colorDialog1.ShowDialog() != DialogResult.OK)
        return;
      this.panel4.BackColor = this.colorDialog1.Color;
      this.File.STAG.KclColor4 = this.colorDialog1.Color;
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.File.STAG.NrLaps = Convert.ToInt16(this.comboBox1.Text);
    }

    private void toolStripButton75_Click(object sender, EventArgs e)
    {
      this.toolStripButton76.Checked = false;
      this.toolStripButton75.Checked = true;
      this.objidraw = false;
    }

    private void toolStripButton76_Click(object sender, EventArgs e)
    {
      this.toolStripButton75.Checked = false;
      this.toolStripButton76.Checked = true;
      this.objidraw = true;
    }

    private void button5_Click(object sender, EventArgs e)
    {
      this.colorDialog1.Color = this.panel5.BackColor;
      if (this.colorDialog1.ShowDialog() != DialogResult.OK)
        return;
      this.panel5.BackColor = this.colorDialog1.Color;
      this.File.STAG.FogColor = this.colorDialog1.Color;
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
      this.File.STAG.FogDensity = (float) this.numericUpDown1.Value;
    }

    private void oBJIToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
    {
      this.Render(false, new Point(), false);
    }

    private void numericUpDown2_ValueChanged(object sender, EventArgs e)
    {
      this.File.STAG.FogAlpha = (ushort) this.numericUpDown2.Value;
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      this.File.NKMI.TrackName = this.textBox1.Text;
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {
      this.File.NKMI.Author = this.textBox2.Text;
    }

    private void textBox3_TextChanged(object sender, EventArgs e)
    {
      this.File.NKMI.Version = this.textBox3.Text;
    }

    private void toolStripButton79_Click(object sender, EventArgs e)
    {
    }

    private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.File.STAG.FogTableGenMode = (byte) this.comboBox2.SelectedIndex;
    }

    private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.File.STAG.FogSlope = (byte) this.comboBox3.SelectedIndex;
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      this.File.STAG.FogEnabled = this.checkBox1.Checked;
    }
  }
}
