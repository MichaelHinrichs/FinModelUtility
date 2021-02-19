// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Form1
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using Microsoft.VisualBasic;
using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.Archive_Format;
using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Converters._3D;
using MKDS_Course_Modifier.Converters.Colission;
using MKDS_Course_Modifier.G2D_Binary_File_Format;
using MKDS_Course_Modifier.GCN;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.Misc;
using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.Properties;
using MKDS_Course_Modifier.Sound;
using MKDS_Course_Modifier.UI;
using MKDS_Course_Modifier.UI.MKDS;
using MKDS_Course_Modifier.UI.MPDS;
using NAudio.Wave;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tao.OpenGl;
using wyDay.Controls;

namespace MKDS_Course_Modifier
{
  public class Form1 : Form
  {
    private IContainer components = (IContainer) null;
    private WaveOut d = (WaveOut) null;
    private bool UseSwar = false;
    private SWAR Swar = (SWAR) null;
    private bool UseSsar = false;
    private SSAR Ssar = (SSAR) null;
    private MenuItem menuItem1;
    private MenuItem menuItem2;
    private MenuItem menuItem3;
    private MenuItem menuItem4;
    private SplitContainer splitContainer1;
    private TreeView treeView1;
    private SplitContainer splitContainer2;
    private SplitContainer splitContainer3;
    private Form1.ListViewNF listView1;
    private Panel panel1;
    private PropertyGrid propertyGrid1;
    private ToolStrip toolStrip1;
    private ToolStripButton openToolStripButton;
    private ToolStripButton saveToolStripButton;
    private MenuItem menuItem5;
    private MenuItem menuItem6;
    private MenuItem menuItem7;
    private MenuItem menuItem8;
    private MKDS_Course_Modifier.UI.MainMenu mainMenu3;
    private MenuItem menuItem9;
    private MenuItem menuItem10;
    private MenuItem menuItem11;
    private MenuItem menuItem12;
    private VistaMenu vistaMenu1;
    private ToolStripButton newToolStripButton;
    private MenuItem menuItem13;
    private MenuItem menuItem21;
    private MenuItem menuItem20;
    private MenuItem menuItem14;
    private MenuItem menuItem19;
    private MenuItem menuItem15;
    private MenuItem menuItem16;
    private MenuItem menuItem18;
    private MenuItem menuItem17;
    private ImageList imageList1;
    private ImageList imageList2;
    private ToolStrip toolStrip2;
    private ToolStripButton toolStripButton1;
    private ToolStripButton toolStripButton2;
    private MenuItem menuItem22;
    private MenuItem menuItem24;
    private MenuItem menuItem23;
    private MenuItem menuItem25;
    private OpenFileDialog openFileDialog1;
    private SaveFileDialog saveFileDialog1;
    public ContextMenu contextMenu1;
    private PictureBox pictureBox1;
    private ContextMenu contextMenu2;
    private MenuItem menuItem26;
    private ContextMenu contextMenu3;
    private MenuItem menuItem27;
    private MenuItem menuItem29;
    private MenuItem menuItem28;
    private OpenFileDialog openFileDialog2;
    private MenuItem menuItem30;
    private MenuItem menuItem31;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private MenuItem menuItem32;
    private MenuItem menuItem33;
    private MenuItem menuItem35;
    private MenuItem menuItem36;
    private MenuItem menuItem37;
    private MenuItem menuItem38;
    private MenuItem menuItem39;
    private MenuItem menuItem41;
    private MenuItem menuItem42;
    private MenuItem menuItem40;
    private MenuItem menuItem43;
    private MenuItem menuItem44;
    private MenuItem menuItem45;
    private MenuItem menuItem46;
    private MenuItem menuItem47;
    private MenuItem menuItem48;
    private MenuItem menuItem49;
    private MenuItem menuItem50;
    private MenuItem menuItem51;
    private MenuItem menuItem52;
    private MenuItem menuItem53;
    private MenuItem menuItem54;
    private MenuItem menuItem34;
    private MenuItem menuItem55;
    private MenuItem menuItem56;
    private MenuItem menuItem57;
    private MenuItem menuItem58;
    private MenuItem menuItem59;
    private MenuItem menuItem60;
    private MenuItem menuItem61;
    private MenuItem menuItem62;
    private MenuItem menuItem63;
    private MenuItem menuItem64;
    private MenuItem menuItem65;
    private MenuItem menuItem66;
    private MenuItem menuItem67;
    private MenuItem menuItem68;
    private MenuItem menuItem69;
    private MenuItem menuItem70;
    private MenuItem menuItem71;
    private MenuItem menuItem72;
    private MenuItem menuItem73;
    private MenuItem menuItem74;
    private MenuItem menuItem75;
    private MenuItem menuItem76;
    private MenuItem menuItem77;
    private MenuItem menuItem78;
    private MenuItem menuItem79;
    private MenuItem menuItem80;
    public NDS Rom;
    public NARC.DirectoryEntry Root;
    public PAZ Paz;
    private bool nds;
    public SDAT SDAT;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      ListViewGroup listViewGroup1 = new ListViewGroup("Folders", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup2 = new ListViewGroup("Models", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup3 = new ListViewGroup("Textures", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup4 = new ListViewGroup("Palettes", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup5 = new ListViewGroup("Graphics", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup6 = new ListViewGroup("Animations", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup7 = new ListViewGroup("Screens", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup8 = new ListViewGroup("MKDS", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup9 = new ListViewGroup("Particles", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup10 = new ListViewGroup("Strings", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup11 = new ListViewGroup("Cells", System.Windows.Forms.HorizontalAlignment.Left);
      ListViewGroup listViewGroup12 = new ListViewGroup("Sound", System.Windows.Forms.HorizontalAlignment.Left);
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.menuItem1 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.splitContainer1 = new SplitContainer();
      this.treeView1 = new TreeView();
      this.imageList1 = new ImageList(this.components);
      this.splitContainer2 = new SplitContainer();
      this.splitContainer3 = new SplitContainer();
      this.listView1 = new Form1.ListViewNF();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader2 = new ColumnHeader();
      this.imageList2 = new ImageList(this.components);
      this.toolStrip2 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.panel1 = new Panel();
      this.pictureBox1 = new PictureBox();
      this.propertyGrid1 = new PropertyGrid();
      this.toolStrip1 = new ToolStrip();
      this.newToolStripButton = new ToolStripButton();
      this.openToolStripButton = new ToolStripButton();
      this.saveToolStripButton = new ToolStripButton();
      this.menuItem5 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.menuItem7 = new MenuItem();
      this.menuItem8 = new MenuItem();
      this.mainMenu3 = new MKDS_Course_Modifier.UI.MainMenu(this.components);
      this.menuItem9 = new MenuItem();
      this.menuItem13 = new MenuItem();
      this.menuItem21 = new MenuItem();
      this.menuItem58 = new MenuItem();
      this.menuItem67 = new MenuItem();
      this.menuItem68 = new MenuItem();
      this.menuItem69 = new MenuItem();
      this.menuItem43 = new MenuItem();
      this.menuItem44 = new MenuItem();
      this.menuItem46 = new MenuItem();
      this.menuItem60 = new MenuItem();
      this.menuItem64 = new MenuItem();
      this.menuItem20 = new MenuItem();
      this.menuItem14 = new MenuItem();
      this.menuItem19 = new MenuItem();
      this.menuItem15 = new MenuItem();
      this.menuItem16 = new MenuItem();
      this.menuItem18 = new MenuItem();
      this.menuItem17 = new MenuItem();
      this.menuItem10 = new MenuItem();
      this.menuItem11 = new MenuItem();
      this.menuItem30 = new MenuItem();
      this.menuItem31 = new MenuItem();
      this.menuItem32 = new MenuItem();
      this.menuItem33 = new MenuItem();
      this.menuItem54 = new MenuItem();
      this.menuItem34 = new MenuItem();
      this.menuItem55 = new MenuItem();
      this.menuItem35 = new MenuItem();
      this.menuItem36 = new MenuItem();
      this.menuItem37 = new MenuItem();
      this.menuItem56 = new MenuItem();
      this.menuItem57 = new MenuItem();
      this.menuItem38 = new MenuItem();
      this.menuItem39 = new MenuItem();
      this.menuItem42 = new MenuItem();
      this.menuItem41 = new MenuItem();
      this.menuItem40 = new MenuItem();
      this.menuItem49 = new MenuItem();
      this.menuItem50 = new MenuItem();
      this.menuItem80 = new MenuItem();
      this.menuItem45 = new MenuItem();
      this.menuItem47 = new MenuItem();
      this.menuItem48 = new MenuItem();
      this.menuItem59 = new MenuItem();
      this.menuItem61 = new MenuItem();
      this.menuItem65 = new MenuItem();
      this.menuItem66 = new MenuItem();
      this.menuItem77 = new MenuItem();
      this.menuItem51 = new MenuItem();
      this.menuItem52 = new MenuItem();
      this.menuItem53 = new MenuItem();
      this.menuItem62 = new MenuItem();
      this.menuItem63 = new MenuItem();
      this.menuItem70 = new MenuItem();
      this.menuItem71 = new MenuItem();
      this.menuItem72 = new MenuItem();
      this.menuItem73 = new MenuItem();
      this.menuItem74 = new MenuItem();
      this.menuItem75 = new MenuItem();
      this.menuItem78 = new MenuItem();
      this.menuItem79 = new MenuItem();
      this.menuItem76 = new MenuItem();
      this.menuItem12 = new MenuItem();
      this.vistaMenu1 = new VistaMenu(this.components);
      this.contextMenu1 = new ContextMenu();
      this.menuItem22 = new MenuItem();
      this.menuItem23 = new MenuItem();
      this.menuItem24 = new MenuItem();
      this.menuItem25 = new MenuItem();
      this.openFileDialog1 = new OpenFileDialog();
      this.saveFileDialog1 = new SaveFileDialog();
      this.contextMenu2 = new ContextMenu();
      this.menuItem26 = new MenuItem();
      this.contextMenu3 = new ContextMenu();
      this.menuItem27 = new MenuItem();
      this.menuItem29 = new MenuItem();
      this.menuItem28 = new MenuItem();
      this.openFileDialog2 = new OpenFileDialog();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.splitContainer3.BeginInit();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      this.toolStrip2.SuspendLayout();
      this.panel1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.toolStrip1.SuspendLayout();
      ((ISupportInitialize) this.vistaMenu1).BeginInit();
      this.SuspendLayout();
      this.menuItem1.Index = -1;
      this.menuItem1.Text = "";
      this.menuItem2.Index = -1;
      this.menuItem2.Text = "";
      this.menuItem3.Index = -1;
      this.menuItem3.Text = "";
      this.menuItem4.Index = -1;
      this.menuItem4.Text = "";
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.treeView1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Size = new Size(874, 384);
      this.splitContainer1.SplitterDistance = 172;
      this.splitContainer1.TabIndex = 0;
      this.treeView1.Dock = DockStyle.Fill;
      this.treeView1.HideSelection = false;
      this.treeView1.HotTracking = true;
      this.treeView1.ImageIndex = 0;
      this.treeView1.ImageList = this.imageList1;
      this.treeView1.Location = new Point(0, 0);
      this.treeView1.Name = "treeView1";
      this.treeView1.SelectedImageIndex = 0;
      this.treeView1.ShowLines = false;
      this.treeView1.Size = new Size(172, 384);
      this.treeView1.TabIndex = 0;
      this.treeView1.AfterSelect += new TreeViewEventHandler(this.treeView1_AfterSelect);
      this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList1.ImageSize = new Size(16, 16);
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.FixedPanel = FixedPanel.Panel2;
      this.splitContainer2.Location = new Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Panel1.Controls.Add((Control) this.splitContainer3);
      this.splitContainer2.Panel2.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer2.Size = new Size(698, 384);
      this.splitContainer2.SplitterDistance = 505;
      this.splitContainer2.TabIndex = 0;
      this.splitContainer3.Dock = DockStyle.Fill;
      this.splitContainer3.FixedPanel = FixedPanel.Panel2;
      this.splitContainer3.Location = new Point(0, 0);
      this.splitContainer3.Name = "splitContainer3";
      this.splitContainer3.Orientation = Orientation.Horizontal;
      this.splitContainer3.Panel1.Controls.Add((Control) this.listView1);
      this.splitContainer3.Panel1.Controls.Add((Control) this.toolStrip2);
      this.splitContainer3.Panel2.Controls.Add((Control) this.panel1);
      this.splitContainer3.Size = new Size(505, 384);
      this.splitContainer3.SplitterDistance = 251;
      this.splitContainer3.TabIndex = 0;
      this.listView1.Columns.AddRange(new ColumnHeader[2]
      {
        this.columnHeader1,
        this.columnHeader2
      });
      this.listView1.Dock = DockStyle.Fill;
      listViewGroup1.Header = "Folders";
      listViewGroup1.Name = "listViewGroup1";
      listViewGroup2.Header = "Models";
      listViewGroup2.Name = "listViewGroup2";
      listViewGroup3.Header = "Textures";
      listViewGroup3.Name = "listViewGroup3";
      listViewGroup4.Header = "Palettes";
      listViewGroup4.Name = "listViewGroup4";
      listViewGroup5.Header = "Graphics";
      listViewGroup5.Name = "listViewGroup5";
      listViewGroup6.Header = "Animations";
      listViewGroup6.Name = "listViewGroup6";
      listViewGroup7.Header = "Screens";
      listViewGroup7.Name = "listViewGroup7";
      listViewGroup8.Header = "MKDS";
      listViewGroup8.Name = "listViewGroup8";
      listViewGroup9.Header = "Particles";
      listViewGroup9.Name = "listViewGroup9";
      listViewGroup10.Header = "Strings";
      listViewGroup10.Name = "listViewGroup10";
      listViewGroup11.Header = "Cells";
      listViewGroup11.Name = "listViewGroup11";
      listViewGroup12.Header = "Sound";
      listViewGroup12.Name = "listViewGroup12";
      this.listView1.Groups.AddRange(new ListViewGroup[12]
      {
        listViewGroup1,
        listViewGroup2,
        listViewGroup3,
        listViewGroup4,
        listViewGroup5,
        listViewGroup6,
        listViewGroup7,
        listViewGroup8,
        listViewGroup9,
        listViewGroup10,
        listViewGroup11,
        listViewGroup12
      });
      this.listView1.HideSelection = false;
      this.listView1.LargeImageList = this.imageList2;
      this.listView1.Location = new Point(0, 25);
      this.listView1.MultiSelect = false;
      this.listView1.Name = "listView1";
      this.listView1.Size = new Size(505, 226);
      this.listView1.TabIndex = 0;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Tile;
      this.listView1.ItemActivate += new EventHandler(this.listView1_ItemActivate);
      this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
      this.listView1.MouseClick += new MouseEventHandler(this.listView1_MouseClick);
      this.columnHeader1.Text = "Filename";
      this.columnHeader2.Text = "Filesize";
      this.imageList2.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList2.ImageSize = new Size(32, 32);
      this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
      this.toolStrip2.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2
      });
      this.toolStrip2.Location = new Point(0, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new Size(505, 25);
      this.toolStrip2.TabIndex = 1;
      this.toolStrip2.Text = "toolStrip2";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Enabled = false;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Enabled = false;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.panel1.AutoScroll = true;
      this.panel1.Controls.Add((Control) this.pictureBox1);
      this.panel1.Dock = DockStyle.Fill;
      this.panel1.Location = new Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(505, 129);
      this.panel1.TabIndex = 0;
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(505, 129);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(189, 384);
      this.propertyGrid1.TabIndex = 0;
      this.toolStrip1.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.newToolStripButton,
        (ToolStripItem) this.openToolStripButton,
        (ToolStripItem) this.saveToolStripButton
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(874, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.newToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.newToolStripButton.Enabled = false;
      this.newToolStripButton.Image = (Image) componentResourceManager.GetObject("newToolStripButton.Image");
      this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.newToolStripButton.Name = "newToolStripButton";
      this.newToolStripButton.Size = new Size(23, 22);
      this.newToolStripButton.Text = "&New";
      this.openToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.openToolStripButton.Image = (Image) componentResourceManager.GetObject("openToolStripButton.Image");
      this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.openToolStripButton.Name = "openToolStripButton";
      this.openToolStripButton.Size = new Size(23, 22);
      this.openToolStripButton.Text = "&Open";
      this.openToolStripButton.Click += new EventHandler(this.openToolStripButton_Click);
      this.saveToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.saveToolStripButton.Enabled = false;
      this.saveToolStripButton.Image = (Image) componentResourceManager.GetObject("saveToolStripButton.Image");
      this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.saveToolStripButton.Name = "saveToolStripButton";
      this.saveToolStripButton.Size = new Size(23, 22);
      this.saveToolStripButton.Text = "&Save";
      this.saveToolStripButton.Click += new EventHandler(this.saveToolStripButton_Click);
      this.menuItem5.Index = -1;
      this.menuItem5.Text = "";
      this.menuItem6.Index = -1;
      this.menuItem6.Text = "";
      this.menuItem7.Index = -1;
      this.menuItem7.Text = "";
      this.menuItem8.Index = -1;
      this.menuItem8.Text = "";
      this.mainMenu3.MenuItems.AddRange(new MenuItem[4]
      {
        this.menuItem9,
        this.menuItem10,
        this.menuItem11,
        this.menuItem12
      });
      this.menuItem9.Index = 0;
      this.menuItem9.MenuItems.AddRange(new MenuItem[8]
      {
        this.menuItem13,
        this.menuItem20,
        this.menuItem14,
        this.menuItem19,
        this.menuItem15,
        this.menuItem16,
        this.menuItem18,
        this.menuItem17
      });
      this.menuItem9.Text = "File";
      this.vistaMenu1.SetImage(this.menuItem13, (Image) componentResourceManager.GetObject("menuItem13.Image"));
      this.menuItem13.Index = 0;
      this.menuItem13.MenuItems.AddRange(new MenuItem[5]
      {
        this.menuItem21,
        this.menuItem58,
        this.menuItem67,
        this.menuItem43,
        this.menuItem64
      });
      this.menuItem13.Text = "New";
      this.vistaMenu1.SetImage(this.menuItem21, (Image) componentResourceManager.GetObject("menuItem21.Image"));
      this.menuItem21.Index = 0;
      this.menuItem21.Text = "KCL";
      this.menuItem21.Click += new EventHandler(this.menuItem21_Click);
      this.vistaMenu1.SetImage(this.menuItem58, (Image) componentResourceManager.GetObject("menuItem58.Image"));
      this.menuItem58.Index = 1;
      this.menuItem58.Text = "ZCB";
      this.menuItem58.Click += new EventHandler(this.menuItem58_Click);
      this.menuItem67.Index = 2;
      this.menuItem67.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem68,
        this.menuItem69
      });
      this.menuItem67.Text = "NCLR + NCGR";
      this.menuItem68.Index = 0;
      this.menuItem68.Text = "4bpp";
      this.menuItem68.Click += new EventHandler(this.menuItem68_Click);
      this.menuItem69.Index = 1;
      this.menuItem69.Text = "8bpp";
      this.menuItem69.Click += new EventHandler(this.menuItem69_Click);
      this.menuItem43.Index = 3;
      this.menuItem43.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem44,
        this.menuItem46,
        this.menuItem60
      });
      this.menuItem43.Text = "NCLR + NCGR + NSCR";
      this.menuItem44.Index = 0;
      this.menuItem44.Text = "8bpp - Single";
      this.menuItem44.Click += new EventHandler(this.menuItem44_Click);
      this.menuItem46.Index = 1;
      this.menuItem46.Text = "8bpp - Double";
      this.menuItem46.Click += new EventHandler(this.menuItem46_Click);
      this.menuItem60.Index = 2;
      this.menuItem60.Text = "8bpp - No Simplify";
      this.menuItem60.Click += new EventHandler(this.menuItem60_Click);
      this.vistaMenu1.SetImage(this.menuItem64, (Image) componentResourceManager.GetObject("menuItem64.Image"));
      this.menuItem64.Index = 4;
      this.menuItem64.Text = "NSBMD";
      this.menuItem64.Click += new EventHandler(this.menuItem64_Click);
      this.menuItem20.Index = 1;
      this.menuItem20.Text = "-";
      this.vistaMenu1.SetImage(this.menuItem14, (Image) componentResourceManager.GetObject("menuItem14.Image"));
      this.menuItem14.Index = 2;
      this.menuItem14.Text = "Open";
      this.menuItem14.Click += new EventHandler(this.openToolStripButton_Click);
      this.menuItem19.Index = 3;
      this.menuItem19.Text = "-";
      this.menuItem15.Enabled = false;
      this.vistaMenu1.SetImage(this.menuItem15, (Image) componentResourceManager.GetObject("menuItem15.Image"));
      this.menuItem15.Index = 4;
      this.menuItem15.Text = "Save";
      this.menuItem15.Click += new EventHandler(this.saveToolStripButton_Click);
      this.menuItem16.Enabled = false;
      this.menuItem16.Index = 5;
      this.menuItem16.Text = "Save As";
      this.menuItem18.Index = 6;
      this.menuItem18.Text = "-";
      this.vistaMenu1.SetImage(this.menuItem17, (Image) componentResourceManager.GetObject("menuItem17.Image"));
      this.menuItem17.Index = 7;
      this.menuItem17.Text = "Exit";
      this.menuItem17.Click += new EventHandler(this.menuItem17_Click);
      this.menuItem10.Index = 1;
      this.menuItem10.Text = "Edit";
      this.menuItem11.Index = 2;
      this.menuItem11.MenuItems.AddRange(new MenuItem[16]
      {
        this.menuItem30,
        this.menuItem32,
        this.menuItem33,
        this.menuItem35,
        this.menuItem38,
        this.menuItem45,
        this.menuItem47,
        this.menuItem51,
        this.menuItem52,
        this.menuItem53,
        this.menuItem62,
        this.menuItem63,
        this.menuItem70,
        this.menuItem71,
        this.menuItem73,
        this.menuItem76
      });
      this.menuItem11.Text = "Tools";
      this.menuItem30.Index = 0;
      this.menuItem30.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem31
      });
      this.menuItem30.Text = "Audio";
      this.menuItem30.Visible = false;
      this.menuItem31.Index = 0;
      this.menuItem31.Text = "Raw ADPCM";
      this.menuItem31.Visible = false;
      this.menuItem31.Click += new EventHandler(this.menuItem31_Click);
      this.menuItem32.Index = 1;
      this.menuItem32.Text = "Test";
      this.menuItem32.Visible = false;
      this.menuItem32.Click += new EventHandler(this.menuItem32_Click);
      this.vistaMenu1.SetImage(this.menuItem33, (Image) componentResourceManager.GetObject("menuItem33.Image"));
      this.menuItem33.Index = 2;
      this.menuItem33.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem54,
        this.menuItem55
      });
      this.menuItem33.Text = "Nitro Intermediate";
      this.menuItem54.Index = 0;
      this.menuItem54.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem34
      });
      this.menuItem54.Text = "IMD";
      this.menuItem34.Index = 0;
      this.menuItem34.Text = "UV Fix";
      this.menuItem34.Click += new EventHandler(this.menuItem34_Click);
      this.menuItem55.Index = 1;
      this.menuItem55.Text = "G3DCVTR Gui";
      this.menuItem55.Click += new EventHandler(this.menuItem55_Click);
      this.vistaMenu1.SetImage(this.menuItem35, (Image) componentResourceManager.GetObject("menuItem35.Image"));
      this.menuItem35.Index = 3;
      this.menuItem35.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem36,
        this.menuItem56
      });
      this.menuItem35.Text = "Compression";
      this.menuItem36.Index = 0;
      this.menuItem36.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem37
      });
      this.menuItem36.Text = "Arm9";
      this.menuItem37.Index = 0;
      this.menuItem37.Text = "Decompress";
      this.menuItem37.Click += new EventHandler(this.menuItem37_Click);
      this.menuItem56.Index = 1;
      this.menuItem56.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem57
      });
      this.menuItem56.Text = "Overlay";
      this.menuItem57.Index = 0;
      this.menuItem57.Text = "Decompress";
      this.menuItem57.Click += new EventHandler(this.menuItem57_Click);
      this.menuItem38.Index = 4;
      this.menuItem38.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem39,
        this.menuItem49
      });
      this.menuItem38.Text = "GCN";
      this.menuItem39.Index = 0;
      this.menuItem39.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem42,
        this.menuItem41,
        this.menuItem40
      });
      this.menuItem39.Text = "3D Bones";
      this.menuItem42.Index = 0;
      this.menuItem42.Text = "BMD + BCA to MA";
      this.menuItem42.Click += new EventHandler(this.menuItem42_Click);
      this.menuItem41.Index = 1;
      this.menuItem41.Text = "BMD + BCK to MA";
      this.menuItem41.Click += new EventHandler(this.menuItem41_Click);
      this.menuItem40.Index = 2;
      this.menuItem40.Text = "BMD to MA";
      this.menuItem40.Click += new EventHandler(this.menuItem40_Click);
      this.menuItem49.Index = 1;
      this.menuItem49.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem50,
        this.menuItem80
      });
      this.menuItem49.Text = "MKDD";
      this.menuItem50.Index = 0;
      this.menuItem50.Text = "BOL -> NKM";
      this.menuItem50.Click += new EventHandler(this.menuItem50_Click);
      this.menuItem80.Index = 1;
      this.menuItem80.Text = "BCO -> OBJ";
      this.menuItem80.Click += new EventHandler(this.menuItem80_Click);
      this.menuItem45.Index = 5;
      this.menuItem45.Text = "MPDS Board Viewer";
      this.menuItem45.Click += new EventHandler(this.menuItem45_Click);
      this.menuItem47.Index = 6;
      this.menuItem47.MenuItems.AddRange(new MenuItem[6]
      {
        this.menuItem48,
        this.menuItem59,
        this.menuItem61,
        this.menuItem65,
        this.menuItem66,
        this.menuItem77
      });
      this.menuItem47.Text = "MKDS";
      this.menuItem48.Index = 0;
      this.menuItem48.Text = "CoursePictureGenerator";
      this.menuItem48.Click += new EventHandler(this.menuItem48_Click);
      this.menuItem59.Index = 1;
      this.menuItem59.Text = "GlobalMapGenerator";
      this.menuItem59.Click += new EventHandler(this.menuItem59_Click);
      this.menuItem61.Index = 2;
      this.menuItem61.Text = "Object Database Editor";
      this.menuItem61.Click += new EventHandler(this.menuItem61_Click);
      this.menuItem65.Index = 3;
      this.menuItem65.Text = "Decrypt Save";
      this.menuItem65.Visible = false;
      this.menuItem65.Click += new EventHandler(this.menuItem65_Click);
      this.menuItem66.Index = 4;
      this.menuItem66.Text = "Encrypt Save";
      this.menuItem66.Visible = false;
      this.menuItem66.Click += new EventHandler(this.menuItem66_Click);
      this.menuItem77.Index = 5;
      this.menuItem77.Text = "Nitro Character Course Picture NSCR Patcher";
      this.menuItem77.Click += new EventHandler(this.menuItem77_Click);
      this.menuItem51.Index = 7;
      this.menuItem51.Text = "Picture";
      this.menuItem51.Visible = false;
      this.menuItem51.Click += new EventHandler(this.menuItem51_Click);
      this.menuItem52.Index = 8;
      this.menuItem52.Text = "Fragment Shader Applier";
      this.menuItem52.Click += new EventHandler(this.menuItem52_Click);
      this.menuItem53.Index = 9;
      this.menuItem53.Text = "ASM Patcher";
      this.menuItem53.Click += new EventHandler(this.menuItem53_Click);
      this.menuItem62.Index = 10;
      this.menuItem62.Text = "Obj -> Ma";
      this.menuItem62.Visible = false;
      this.menuItem62.Click += new EventHandler(this.menuItem62_Click);
      this.menuItem63.Index = 11;
      this.menuItem63.Text = "OBJ UV Patcher";
      this.menuItem63.Click += new EventHandler(this.menuItem63_Click);
      this.menuItem70.Index = 12;
      this.menuItem70.Text = "ASM Editor";
      this.menuItem70.Visible = false;
      this.menuItem70.Click += new EventHandler(this.menuItem70_Click);
      this.menuItem71.Index = 13;
      this.menuItem71.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem72
      });
      this.menuItem71.Text = "MK64";
      this.menuItem71.Visible = false;
      this.menuItem72.Index = 0;
      this.menuItem72.Text = "Level Viewer";
      this.menuItem72.Click += new EventHandler(this.menuItem72_Click);
      this.menuItem73.Index = 14;
      this.menuItem73.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem74
      });
      this.menuItem73.Text = "Nitro System";
      this.menuItem74.Index = 0;
      this.menuItem74.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem75,
        this.menuItem78,
        this.menuItem79
      });
      this.menuItem74.Text = "NSBMD";
      this.menuItem75.Index = 0;
      this.menuItem75.Text = "Remove Normals";
      this.menuItem75.Visible = false;
      this.menuItem75.Click += new EventHandler(this.menuItem75_Click);
      this.menuItem78.Index = 1;
      this.menuItem78.Text = "NSBMD -> NSBMD + NSBTX";
      this.menuItem78.Click += new EventHandler(this.menuItem78_Click);
      this.menuItem79.Index = 2;
      this.menuItem79.Text = "NSBMD + NSBTX -> NSBMD";
      this.menuItem79.Click += new EventHandler(this.menuItem79_Click);
      this.menuItem76.Index = 15;
      this.menuItem76.Text = "OBJ Scaler";
      this.menuItem76.Click += new EventHandler(this.menuItem76_Click);
      this.menuItem12.Index = 3;
      this.menuItem12.Text = "Help";
      this.vistaMenu1.ContainerControl = (ContainerControl) this;
      this.contextMenu1.MenuItems.AddRange(new MenuItem[4]
      {
        this.menuItem22,
        this.menuItem23,
        this.menuItem24,
        this.menuItem25
      });
      this.menuItem22.Index = 0;
      this.menuItem22.Text = "Replace";
      this.menuItem22.Click += new EventHandler(this.menuItem22_Click);
      this.menuItem23.Index = 1;
      this.menuItem23.Text = "Export";
      this.menuItem23.Click += new EventHandler(this.menuItem23_Click);
      this.menuItem24.Index = 2;
      this.menuItem24.Text = "-";
      this.menuItem25.Index = 3;
      this.menuItem25.Text = "Rename";
      this.menuItem25.Click += new EventHandler(this.menuItem25_Click);
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "All Files(*.*)|*.*";
      this.contextMenu2.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem26
      });
      this.menuItem26.Index = 0;
      this.menuItem26.Text = "Convert to DLS";
      this.menuItem26.Click += new EventHandler(this.menuItem26_Click);
      this.contextMenu3.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem27,
        this.menuItem29,
        this.menuItem28
      });
      this.menuItem27.Index = 0;
      this.menuItem27.Text = "Convert to WAV";
      this.menuItem27.Click += new EventHandler(this.menuItem27_Click);
      this.menuItem29.Index = 1;
      this.menuItem29.Text = "-";
      this.menuItem28.Enabled = false;
      this.menuItem28.Index = 2;
      this.menuItem28.Text = "From WAV";
      this.openFileDialog2.FileName = "openFileDialog2";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(874, 409);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.DoubleBuffered = true;
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu3;
      this.Name = nameof (Form1);
      this.Text = "MKDS Course Modifier";
      this.Load += new EventHandler(this.Form1_Load);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel1.PerformLayout();
      this.splitContainer3.Panel2.ResumeLayout(false);
      this.splitContainer3.EndInit();
      this.splitContainer3.ResumeLayout(false);
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      this.panel1.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((ISupportInitialize) this.vistaMenu1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr hWnd, string appName, string partList);

    public Form1(string Path)
      : this()
    {
      FileHandler.Open(Path, this);
    }

    public Form1()
    {
      try
      {
        FileHandler.SetMusicPlayer(new MusicPlayer());
      }
      catch
      {
      }
      this.InitializeComponent();
      Form1.SetWindowTheme(this.treeView1.Handle, "explorer", (string) null);
      Form1.SetWindowTheme(this.listView1.Handle, "explorer", (string) null);
      this.imageList1.Images.Add(this.openToolStripButton.Image);
      this.imageList1.Images.Add((Image) Resources.speaker_volume_pack);
      this.imageList1.Images.Add((Image) Resources.music_beam_16_pack);
      FileHandler.SetBigImageList(ref this.imageList2);
      FileHandler.OnSave += new EventHandler(this.FileHandler_OnSave);
      this.newToolStripButton.Text = this.menuItem13.Text = LanguageHandler.GetString("base.new");
      this.openToolStripButton.Text = this.menuItem14.Text = LanguageHandler.GetString("base.open");
      this.saveToolStripButton.Text = this.menuItem15.Text = LanguageHandler.GetString("base.save");
      this.menuItem9.Text = LanguageHandler.GetString("main.file");
      this.menuItem10.Text = LanguageHandler.GetString("main.edit");
      this.menuItem11.Text = LanguageHandler.GetString("main.tools");
      this.menuItem12.Text = LanguageHandler.GetString("main.help");
      this.menuItem16.Text = LanguageHandler.GetString("base.saveas");
      this.menuItem17.Text = LanguageHandler.GetString("main.exit");
      this.menuItem30.Text = LanguageHandler.GetString("type.audio");
      this.toolStripButton1.Text = LanguageHandler.GetString("base.add");
      this.toolStripButton2.Text = LanguageHandler.GetString("base.remove");
      this.contextMenu1.MenuItems[0].Text = LanguageHandler.GetString("base.replace");
      this.contextMenu1.MenuItems[1].Text = LanguageHandler.GetString("base.export");
      this.contextMenu1.MenuItems[3].Text = LanguageHandler.GetString("base.rename");
    }

    private bool CompareData(byte[] Data, int Offset, byte[] Cmp)
    {
      for (int index = 0; index < Cmp.Length; ++index)
      {
        if ((int) Data[Offset + index] != (int) Cmp[index])
          return false;
      }
      return true;
    }

    private void FileHandler_OnSave(object sender, EventArgs e)
    {
      this.RefreshListview(false);
    }

    public void OpenNarc(NARC.DirectoryEntry Root)
    {
      this.nds = false;
      this.Rom = (NDS) null;
      this.Paz = (PAZ) null;
      this.Root = Root;
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      TreeNodeCollection nodes = this.treeView1.Nodes;
      this.Root.GetDirectoryTree(ref nodes);
      nodes[0].Text = "\\";
      nodes[0].Toggle();
      this.treeView1.EndUpdate();
      this.treeView1.SelectedNode = nodes[0];
      this.saveToolStripButton.Enabled = this.menuItem15.Enabled = this.toolStripButton1.Enabled = this.toolStripButton2.Enabled = true;
    }

    public void OpenNDS(NDS Rom)
    {
      this.nds = true;
      this.Paz = (PAZ) null;
      this.Rom = Rom;
      this.Root = this.Rom.Root;
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      TreeNodeCollection nodes = this.treeView1.Nodes;
      this.Root.GetDirectoryTree(ref nodes);
      nodes[0].Text = "\\";
      nodes[0].Toggle();
      this.treeView1.EndUpdate();
      this.treeView1.SelectedNode = nodes[0];
      this.saveToolStripButton.Enabled = this.menuItem15.Enabled = this.toolStripButton1.Enabled = this.toolStripButton2.Enabled = false;
    }

    public void OpenPAZ(PAZ Arc)
    {
      this.nds = false;
      this.Rom = (NDS) null;
      this.Root = (NARC.DirectoryEntry) null;
      this.Paz = Arc;
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      this.treeView1.Nodes.Add("\\");
      this.treeView1.Nodes[0].Toggle();
      this.treeView1.EndUpdate();
      this.treeView1.SelectedNode = this.treeView1.Nodes[0];
      this.saveToolStripButton.Enabled = this.menuItem15.Enabled = this.toolStripButton1.Enabled = this.toolStripButton2.Enabled = false;
    }

    public void OpenSDAT(SDAT SDAT)
    {
      this.SDAT = SDAT;
      this.Paz = (PAZ) null;
      this.nds = false;
      this.Root = (NARC.DirectoryEntry) null;
      this.treeView1.BeginUpdate();
      this.treeView1.Nodes.Clear();
      TreeNode treeNode1 = this.treeView1.Nodes.Add("Seq");
      int index1 = 0;
      TreeNode treeNode2 = this.treeView1.Nodes.Add("SeqArc");
      foreach (SDAT.InfoBlock.SEQARCInfo entry in SDAT.INFO.SEQARCRecord.Entries)
      {
        if (SDAT.SYMB != null)
        {
          TreeNode treeNode3 = treeNode2.Nodes.Add(SDAT.SYMB.SEQARCRecord.Entries[index1].GroupName);
          treeNode3.ImageIndex = 2;
          treeNode3.SelectedImageIndex = 2;
        }
        else
        {
          TreeNode treeNode3 = treeNode2.Nodes.Add("SeqArc " + (object) index1);
          treeNode3.ImageIndex = 2;
          treeNode3.SelectedImageIndex = 2;
        }
        ++index1;
      }
      treeNode1 = this.treeView1.Nodes.Add("Bank");
      TreeNode treeNode4 = this.treeView1.Nodes.Add("WaveArc");
      int index2 = 0;
      foreach (SDAT.InfoBlock.WAVEARCInfo entry in SDAT.INFO.WAVEARCRecord.Entries)
      {
        if (SDAT.SYMB != null)
        {
          TreeNode treeNode3 = treeNode4.Nodes.Add(SDAT.SYMB.WAVEARCRecord.Names[index2]);
          treeNode3.ImageIndex = 1;
          treeNode3.SelectedImageIndex = 1;
        }
        else
        {
          TreeNode treeNode3 = treeNode4.Nodes.Add("WaveArc " + (object) index2);
          treeNode3.ImageIndex = 1;
          treeNode3.SelectedImageIndex = 1;
        }
        ++index2;
      }
      treeNode1 = this.treeView1.Nodes.Add("Strm");
      this.treeView1.EndUpdate();
      this.treeView1.SelectedNode = this.treeView1.Nodes[0];
      this.saveToolStripButton.Enabled = true;
      this.menuItem15.Enabled = this.toolStripButton1.Enabled = this.toolStripButton2.Enabled = false;
    }

    private void openToolStripButton_Click(object sender, EventArgs e)
    {
      if (!FileHandler.ShowFileDialog(this))
        ;
    }

    private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (e.Node.Parent != null && e.Node.Parent.Text == "WaveArc")
      {
        this.UseSsar = false;
        this.UseSwar = true;
        this.Swar = new SWAR(this.SDAT.FAT.Records[(int) this.SDAT.INFO.WAVEARCRecord[e.Node.Index].fileID].Data);
        this.RefreshListview(true);
      }
      else if (e.Node.Parent != null && e.Node.Parent.Text == "SeqArc")
      {
        this.UseSwar = false;
        this.UseSsar = true;
        this.Ssar = new SSAR(this.SDAT.FAT.Records[(int) this.SDAT.INFO.SEQARCRecord[e.Node.Index].fileID].Data);
        this.RefreshListview(true);
      }
      else
      {
        this.UseSwar = false;
        this.UseSsar = false;
        this.Ssar = (SSAR) null;
        this.Swar = (SWAR) null;
        this.propertyGrid1.SelectedObject = e.Node.Parent != null || !this.nds ? (object) null : (object) this.Rom;
        this.RefreshListview(false);
      }
    }

    private void listView1_ItemActivate(object sender, EventArgs e)
    {
      if (this.Paz != null)
      {
        PAZ.PAZFileEntry fileByName = this.Paz.GetFileByName(this.listView1.SelectedItems[0].Text);
        if (fileByName == null)
          return;
        FileHandler.Open(new ByteFileInfo(fileByName), this, (object) null);
      }
      else if (this.Root != null)
      {
        NARC.FileEntry fileByPath = this.Root.GetFileByPath(this.treeView1.SelectedNode.FullPath + "\\" + this.listView1.SelectedItems[0].Text);
        if (fileByPath == null)
          this.treeView1.SelectedNode = this.FindNode(this.treeView1.SelectedNode.Nodes, this.treeView1.SelectedNode.FullPath + "\\" + this.listView1.SelectedItems[0].Text);
        else if (!FileHandler.Open(new ByteFileInfo(fileByPath), this, (object) null))
          ;
      }
      else if (this.UseSwar)
      {
        int num1 = (int) new MKDS_Course_Modifier.UI.WAV(this.Swar[this.listView1.SelectedIndices[0]].ToWave()).ShowDialog();
      }
      else if (this.UseSsar)
      {
        int bnk = (int) this.Ssar.Data.Records[this.listView1.SelectedIndices[0]].bnk;
        int fileId = (int) this.SDAT.INFO.BANKRecord[bnk].fileID;
        ushort[] wavearc = this.SDAT.INFO.BANKRecord[bnk].wavearc;
        List<SWAR> swarList = new List<SWAR>();
        for (int index = 0; index < 4; ++index)
        {
          if (wavearc[index] == ushort.MaxValue)
            swarList.Add((SWAR) null);
          else
            swarList.Add(new SWAR(this.SDAT.FAT.Records[(IntPtr) this.SDAT.INFO.WAVEARCRecord[(int) wavearc[index]].fileID].Data));
        }
        FileHandler.Open(new ByteFileInfo(this.Ssar.Data.Data, this.listView1.SelectedItems[0].Text + ".ssar"), this, (object) (int) this.Ssar.Data.Records[this.listView1.SelectedIndices[0]].Offset);
        FileHandler.Open(new ByteFileInfo(this.SDAT.FAT.Records[fileId].Data, ""), this, (object) swarList.ToArray());
      }
      else if (this.listView1.SelectedItems[0].Text.EndsWith(".sseq"))
      {
        int tag = (int) this.listView1.SelectedItems[0].Tag;
        int fileId1 = (int) this.SDAT.INFO.SEQRecord[tag].fileID;
        int bank = (int) this.SDAT.INFO.SEQRecord[tag].bank;
        int fileId2 = (int) this.SDAT.INFO.BANKRecord[bank].fileID;
        ushort[] wavearc = this.SDAT.INFO.BANKRecord[bank].wavearc;
        List<SWAR> swarList = new List<SWAR>();
        for (int index = 0; index < 4; ++index)
        {
          if (wavearc[index] == ushort.MaxValue)
            swarList.Add((SWAR) null);
          else
            swarList.Add(new SWAR(this.SDAT.FAT.Records[(IntPtr) this.SDAT.INFO.WAVEARCRecord[(int) wavearc[index]].fileID].Data));
        }
        FileHandler.Open(new ByteFileInfo(this.SDAT.FAT.Records[fileId1].Data, this.listView1.SelectedItems[0].Text), this, (object) null);
        FileHandler.Open(new ByteFileInfo(this.SDAT.FAT.Records[fileId2].Data, ""), this, (object) swarList.ToArray());
      }
      else if (this.listView1.SelectedItems[0].Text.EndsWith(".swar"))
      {
        this.UseSsar = false;
        this.UseSwar = true;
        this.Swar = new SWAR(this.SDAT.FAT.Records[(int) this.SDAT.INFO.WAVEARCRecord[(int) this.listView1.SelectedItems[0].Tag].fileID].Data);
        this.treeView1.SelectedNode = this.treeView1.SelectedNode.Nodes[this.listView1.SelectedIndices[0]];
        this.RefreshListview(true);
      }
      else if (this.listView1.SelectedItems[0].Text.EndsWith(".ssar"))
      {
        this.UseSwar = false;
        this.UseSsar = true;
        this.Ssar = new SSAR(this.SDAT.FAT.Records[(int) this.SDAT.INFO.SEQARCRecord[(int) this.listView1.SelectedItems[0].Tag].fileID].Data);
        this.treeView1.SelectedNode = this.treeView1.SelectedNode.Nodes[this.listView1.SelectedIndices[0]];
        this.RefreshListview(true);
      }
      else if (this.listView1.SelectedItems[0].Text.EndsWith(".strm"))
      {
        int num2 = (int) new MKDS_Course_Modifier.UI.WAV(new STRM(this.SDAT.FAT.Records[(int) this.SDAT.INFO.STREAMRecord[(int) this.listView1.SelectedItems[0].Tag].fileID].Data).ToWave()).ShowDialog();
      }
    }

    private TreeNode FindNode(TreeNodeCollection tncoll, string strText)
    {
      foreach (TreeNode treeNode in tncoll)
      {
        if (treeNode.FullPath == strText)
          return treeNode;
        TreeNode node = this.FindNode(treeNode.Nodes, strText);
        if (node != null)
          return node;
      }
      return (TreeNode) null;
    }

    private void menuItem22_Click(object sender, EventArgs e)
    {
      if (this.listView1.SelectedItems[0].Text.EndsWith(".sseq"))
      {
        if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
          return;
        int fileId = (int) this.SDAT.INFO.SEQRecord[(int) this.listView1.SelectedItems[0].Tag].fileID;
        this.SDAT.FAT.Records[fileId].Data = System.IO.File.ReadAllBytes(this.openFileDialog1.FileName);
        this.SDAT.FAT.Records[fileId].nSize = (uint) this.SDAT.FAT.Records[fileId].Data.Length;
      }
      else if (this.Paz != null)
      {
        PAZ.PAZFileEntry fileByName = this.Paz.GetFileByName(this.listView1.SelectedItems[0].Text);
        if (fileByName == null || (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0))
          return;
        fileByName.Data = System.IO.File.ReadAllBytes(this.openFileDialog1.FileName);
        this.RefreshListview(false);
      }
      else
      {
        if (this.Root == null)
          return;
        NARC.FileEntry fileByPath = this.Root.GetFileByPath(this.treeView1.SelectedNode.FullPath + "\\" + this.listView1.SelectedItems[0].Text);
        if (fileByPath != null && (this.openFileDialog1.ShowDialog() == DialogResult.OK && this.openFileDialog1.FileName.Length > 0))
        {
          fileByPath.Content = System.IO.File.ReadAllBytes(this.openFileDialog1.FileName);
          this.RefreshListview(false);
        }
      }
    }

    private void menuItem23_Click(object sender, EventArgs e)
    {
      if (this.listView1.SelectedItems[0].Tag is int)
      {
        this.saveFileDialog1.Filter = Path.GetExtension(this.listView1.SelectedItems[0].Text).Replace(".", "").ToUpper() + "(*" + Path.GetExtension(this.listView1.SelectedItems[0].Text) + ")|*" + Path.GetExtension(this.listView1.SelectedItems[0].Text);
        this.saveFileDialog1.FileName = this.listView1.SelectedItems[0].Text;
        if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
          return;
        int tag = (int) this.listView1.SelectedItems[0].Tag;
        int index = -1;
        if (this.listView1.SelectedItems[0].Text.EndsWith(".sseq"))
          index = (int) this.SDAT.INFO.SEQRecord[tag].fileID;
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, this.SDAT.FAT.Records[index].Data);
      }
      else if (this.Paz != null)
      {
        PAZ.PAZFileEntry fileByName = this.Paz.GetFileByName(this.listView1.SelectedItems[0].Text);
        if (fileByName == null)
          return;
        this.saveFileDialog1.Filter = Path.GetExtension(fileByName.Name).Replace(".", "").ToUpper() + "(*" + Path.GetExtension(fileByName.Name) + ")|*" + Path.GetExtension(fileByName.Name);
        this.saveFileDialog1.FileName = fileByName.Name;
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          System.IO.File.Create(this.saveFileDialog1.FileName).Close();
          System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, fileByName.Data);
        }
      }
      else
      {
        if (this.Root == null)
          return;
        NARC.FileEntry fileByPath = this.Root.GetFileByPath(this.treeView1.SelectedNode.FullPath + "\\" + this.listView1.SelectedItems[0].Text);
        if (fileByPath != null)
        {
          this.saveFileDialog1.Filter = Path.GetExtension(fileByPath.Name).Replace(".", "").ToUpper() + "(*" + Path.GetExtension(fileByPath.Name) + ")|*" + Path.GetExtension(fileByPath.Name);
          this.saveFileDialog1.FileName = fileByPath.Name;
          if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
          {
            System.IO.File.Create(this.saveFileDialog1.FileName).Close();
            System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, fileByPath.Content);
          }
        }
      }
    }

    private void menuItem25_Click(object sender, EventArgs e)
    {
      if (this.Paz != null)
      {
        PAZ.PAZFileEntry fileByName = this.Paz.GetFileByName(this.listView1.SelectedItems[0].Text);
        if (fileByName == null)
          return;
        string str = Interaction.InputBox("Type the name you want:", "Rename", fileByName.Name, -1, -1);
        if (str != "" && str != null)
        {
          this.listView1.SelectedItems[0].Text = str;
          fileByName.Name = str;
          this.RefreshListview(false);
        }
      }
      else
      {
        if (this.Root == null)
          return;
        NARC.FileEntry fileByPath = this.Root.GetFileByPath(this.treeView1.SelectedNode.FullPath + "\\" + this.listView1.SelectedItems[0].Text);
        if (fileByPath != null)
        {
          string str = Interaction.InputBox("Type the name you want:", "Rename", fileByPath.Name, -1, -1);
          if (str != "" && str != null)
          {
            this.listView1.SelectedItems[0].Text = str;
            fileByPath.Name = str;
            this.RefreshListview(false);
          }
        }
      }
    }

    private void RefreshListview(bool swar = false)
    {
      this.listView1.BeginUpdate();
      this.listView1.Items.Clear();
      if (this.Paz != null)
        this.Paz.GetDirectoryContents((ListView) this.listView1);
      else if (this.Root != null)
        this.Root.GetDirectoryByPath(this.treeView1.SelectedNode.FullPath).GetDirectoryContents((ListView) this.listView1);
      else if (this.UseSwar && swar)
      {
        for (int index = 0; (long) index < (long) this.Swar.Data.nSample; ++index)
          this.listView1.Items.Add("Swav " + (object) index + ".swav", 18);
      }
      else if (this.UseSsar && swar)
      {
        for (int index = 0; (long) index < (long) this.Ssar.Data.NrRecord; ++index)
        {
          if (this.SDAT.SYMB != null)
            this.listView1.Items.Add(this.SDAT.SYMB.SEQARCRecord.Entries[this.treeView1.SelectedNode.Index].SubRecords.Names[index]).ImageIndex = 14;
          else
            this.listView1.Items.Add("Seq " + (object) index).ImageIndex = 14;
        }
      }
      else
      {
        this.UseSwar = false;
        this.Swar = (SWAR) null;
        int index = 0;
        switch (this.treeView1.SelectedNode.Text)
        {
          case "Seq":
            using (List<SDAT.InfoBlock.SEQInfo>.Enumerator enumerator = this.SDAT.INFO.SEQRecord.Entries.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                if (enumerator.Current.bank != (ushort) 26196)
                {
                  if (this.SDAT.SYMB != null)
                  {
                    if (this.SDAT.SYMB.SEQRecord.Names[index] == null)
                    {
                      ++index;
                      continue;
                    }
                    ListViewItem listViewItem = this.listView1.Items.Add(this.SDAT.SYMB.SEQRecord.Names[index] + ".sseq");
                    listViewItem.Tag = (object) index;
                    listViewItem.ImageIndex = 14;
                  }
                  else
                  {
                    ListViewItem listViewItem = this.listView1.Items.Add("Seq " + (object) index + ".sseq");
                    listViewItem.Tag = (object) index;
                    listViewItem.ImageIndex = 14;
                  }
                }
                ++index;
              }
              break;
            }
          case "SeqArc":
            using (List<SDAT.InfoBlock.SEQARCInfo>.Enumerator enumerator = this.SDAT.INFO.SEQARCRecord.Entries.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                SDAT.InfoBlock.SEQARCInfo current = enumerator.Current;
                if (this.SDAT.SYMB != null)
                {
                  ListViewItem listViewItem = this.listView1.Items.Add(this.SDAT.SYMB.SEQARCRecord.Entries[index].GroupName + ".ssar");
                  listViewItem.Tag = (object) index;
                  listViewItem.ImageIndex = 17;
                }
                else
                {
                  ListViewItem listViewItem = this.listView1.Items.Add("SeqArc " + (object) index + ".ssar");
                  listViewItem.Tag = (object) index;
                  listViewItem.ImageIndex = 17;
                }
                ++index;
              }
              break;
            }
          case "Bank":
            using (List<SDAT.InfoBlock.BANKInfo>.Enumerator enumerator = this.SDAT.INFO.BANKRecord.Entries.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                SDAT.InfoBlock.BANKInfo current = enumerator.Current;
                if (this.SDAT.SYMB != null)
                {
                  if (this.SDAT.SYMB.BANKRecord.Names[index] != null)
                  {
                    ListViewItem listViewItem = this.listView1.Items.Add(this.SDAT.SYMB.BANKRecord.Names[index] + ".sbnk");
                    listViewItem.Tag = (object) index;
                    listViewItem.ImageIndex = 15;
                  }
                  else
                    continue;
                }
                else
                {
                  ListViewItem listViewItem = this.listView1.Items.Add("Bank " + (object) index + ".sbnk");
                  listViewItem.Tag = (object) index;
                  listViewItem.ImageIndex = 15;
                }
                ++index;
              }
              break;
            }
          case "WaveArc":
            using (List<SDAT.InfoBlock.WAVEARCInfo>.Enumerator enumerator = this.SDAT.INFO.WAVEARCRecord.Entries.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                SDAT.InfoBlock.WAVEARCInfo current = enumerator.Current;
                if (this.SDAT.SYMB != null)
                {
                  if (this.SDAT.SYMB.WAVEARCRecord.Names[index] != null)
                  {
                    ListViewItem listViewItem = this.listView1.Items.Add(this.SDAT.SYMB.WAVEARCRecord.Names[index] + ".swar");
                    listViewItem.Tag = (object) index;
                    listViewItem.ImageIndex = 19;
                  }
                  else
                    continue;
                }
                else
                {
                  ListViewItem listViewItem = this.listView1.Items.Add("WaveArc " + (object) index + ".swar");
                  listViewItem.Tag = (object) index;
                  listViewItem.ImageIndex = 19;
                }
                ++index;
              }
              break;
            }
          case "Strm":
            using (List<SDAT.InfoBlock.STREAMInfo>.Enumerator enumerator = this.SDAT.INFO.STREAMRecord.Entries.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                if (enumerator.Current.fileID != 1330007625U)
                {
                  if (this.SDAT.SYMB != null)
                  {
                    if (this.SDAT.SYMB.STRMRecord.Names[index] != null)
                    {
                      ListViewItem listViewItem = this.listView1.Items.Add(this.SDAT.SYMB.STRMRecord.Names[index] + ".strm");
                      listViewItem.ImageIndex = 16;
                      listViewItem.Tag = (object) index;
                    }
                    else
                      continue;
                  }
                  else
                  {
                    ListViewItem listViewItem = this.listView1.Items.Add("Strm " + (object) index + ".strm");
                    listViewItem.ImageIndex = 16;
                    listViewItem.Tag = (object) index;
                  }
                }
                ++index;
              }
              break;
            }
        }
      }
      this.listView1.EndUpdate();
    }

    private void listView1_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Right)
        return;
      ListViewItem itemAt = this.listView1.GetItemAt(e.X, e.Y);
      if ((this.Root != null || this.Paz != null) && itemAt != null && itemAt.Group != this.listView1.Groups[0])
        this.contextMenu1.Show((Control) this.listView1, e.Location);
      else if (itemAt.Text.EndsWith(".sseq"))
        this.contextMenu1.Show((Control) this.listView1, e.Location);
      else if (itemAt.Text.EndsWith(".sbnk"))
        this.contextMenu2.Show((Control) this.listView1, e.Location);
      else if (itemAt.Text.EndsWith(".swav") || itemAt.Text.EndsWith(".strm"))
        this.contextMenu3.Show((Control) this.listView1, e.Location);
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.listView1.SelectedIndices.Count != 0)
      {
        if (this.Paz != null)
        {
          PAZ.PAZFileEntry fileByName = this.Paz.GetFileByName(this.listView1.SelectedItems[0].Text);
          if (fileByName != null)
            this.pictureBox1.Image = (Image) FileHandler.GetPreview(new ByteFileInfo(fileByName));
          else
            this.pictureBox1.Image = (Image) null;
        }
        else if (this.Root != null)
        {
          NARC.FileEntry fileByPath = this.Root.GetFileByPath(this.treeView1.SelectedNode.FullPath + "\\" + this.listView1.SelectedItems[0].Text);
          if (fileByPath != null)
            this.pictureBox1.Image = (Image) FileHandler.GetPreview(new ByteFileInfo(fileByPath));
          else
            this.pictureBox1.Image = (Image) null;
        }
        else
          this.propertyGrid1.SelectedObject = !this.listView1.SelectedItems[0].Text.EndsWith(".sseq") ? (!this.listView1.SelectedItems[0].Text.EndsWith(".sbnk") ? (!this.listView1.SelectedItems[0].Text.EndsWith(".swar") ? (!this.listView1.SelectedItems[0].Text.EndsWith(".strm") ? (object) null : (object) this.SDAT.INFO.STREAMRecord[(int) this.listView1.SelectedItems[0].Tag]) : (object) this.SDAT.INFO.WAVEARCRecord[(int) this.listView1.SelectedItems[0].Tag]) : (object) this.SDAT.INFO.BANKRecord[(int) this.listView1.SelectedItems[0].Tag]) : (object) this.SDAT.INFO.SEQRecord[(int) this.listView1.SelectedItems[0].Tag];
      }
      else
        this.propertyGrid1.SelectedObject = (object) null;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void saveToolStripButton_Click(object sender, EventArgs e)
    {
      if (this.Root != null && !this.nds)
        FileHandler.Save(NARC.Pack(this.Root), 0, true);
      else if (this.SDAT != null)
      {
        FileHandler.Save(this.SDAT.Write(), 0, true);
      }
      else
      {
        int num = (int) System.Windows.Forms.MessageBox.Show("This format can't be saved yet.");
      }
    }

    private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
    {
      this.RefreshListview(false);
    }

    private void menuItem26_Click(object sender, EventArgs e)
    {
      this.saveFileDialog1.Filter = "DLS Soundbank(*.dls)|*.dls";
      this.saveFileDialog1.DefaultExt = "dls";
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      int fileId = (int) this.SDAT.INFO.BANKRecord[this.listView1.SelectedIndices[0]].fileID;
      ushort[] wavearc = this.SDAT.INFO.BANKRecord[this.listView1.SelectedIndices[0]].wavearc;
      List<SWAR> swarList = new List<SWAR>();
      for (int index = 0; index < 4; ++index)
      {
        if (wavearc[index] == ushort.MaxValue)
          swarList.Add((SWAR) null);
        else
          swarList.Add(new SWAR(this.SDAT.FAT.Records[(IntPtr) this.SDAT.INFO.WAVEARCRecord[(int) wavearc[index]].fileID].Data));
      }
      SBNK s = SBNK.InitDLS(new SBNK(this.SDAT.FAT.Records[fileId].Data), swarList.ToArray());
      System.IO.File.Create(this.saveFileDialog1.FileName).Close();
      System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, SBNK.ToDLS(s));
    }

    private void menuItem27_Click(object sender, EventArgs e)
    {
      if (this.listView1.SelectedItems[0].Text.EndsWith(".swav"))
      {
        this.saveFileDialog1.Filter = "WAV sound(*.wav)|*.wav";
        this.saveFileDialog1.DefaultExt = "wav";
        if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
          return;
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, this.Swar[this.listView1.SelectedIndices[0]].ToWave().Write());
      }
      else
      {
        this.saveFileDialog1.Filter = "WAV sound(*.wav)|*.wav";
        this.saveFileDialog1.DefaultExt = "wav";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          System.IO.File.Create(this.saveFileDialog1.FileName).Close();
          System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, new STRM(this.SDAT.FAT.Records[(IntPtr) this.SDAT.INFO.STREAMRecord[(int) this.listView1.SelectedItems[0].Tag].fileID].Data).ToWave().Write());
        }
      }
    }

    private void menuItem21_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      this.saveFileDialog1.Filter = "Nintendo Collision Files(*.kcl)|*.kcl";
      this.saveFileDialog1.DefaultExt = "kcl";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        Obj2Kcl.ConvertToKcl(this.openFileDialog2.FileName, this.saveFileDialog1.FileName);
    }

    private void menuItem31_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0 || (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0))
        return;
      sfxbin sfxbin = new sfxbin(System.IO.File.ReadAllBytes(this.openFileDialog1.FileName));
      for (int index = 0; index < sfxbin.Waves.Length; ++index)
      {
        System.IO.File.Create(Path.GetDirectoryName(this.saveFileDialog1.FileName) + "\\" + index.ToString() + ".wav").Close();
        System.IO.File.WriteAllBytes(Path.GetDirectoryName(this.saveFileDialog1.FileName) + "\\" + index.ToString() + ".wav", sfxbin.Waves[index].ToWave().Write());
      }
    }

    private void menuItem32_Click(object sender, EventArgs e)
    {
      int num = (int) new Test().ShowDialog();
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "All Files(*.*)|*.*";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      NARC.DirectoryEntry directoryByPath = this.Root.GetDirectoryByPath(this.treeView1.SelectedNode.FullPath);
      directoryByPath.Files.Add(new NARC.FileEntry(Path.GetFileName(this.openFileDialog2.FileName), -1));
      directoryByPath.Files.Last<NARC.FileEntry>().Content = System.IO.File.ReadAllBytes(this.openFileDialog2.FileName);
      int startidx = 0;
      this.Root.RefreshIds(ref startidx);
      this.RefreshListview(false);
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      if (this.listView1.SelectedItems.Count == 0 || this.listView1.SelectedItems[0].Group == this.listView1.Groups[0])
        return;
      NARC.DirectoryEntry directoryByPath = this.Root.GetDirectoryByPath(this.treeView1.SelectedNode.FullPath);
      directoryByPath.Files.RemoveAt(this.listView1.SelectedIndices[0] - directoryByPath.Subdirs.Count);
      int startidx = 0;
      this.Root.RefreshIds(ref startidx);
      this.RefreshListview(false);
    }

    private void menuItem34_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Intermediate Model Files(*.imd)|*.imd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      IMD.FixUV(this.openFileDialog2.FileName);
    }

    private void menuItem17_Click(object sender, EventArgs e)
    {
      System.Windows.Forms.Application.Exit();
    }

    private void menuItem37_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Arm 9(*.bin)|*.bin";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      byte[] bytes = ARM9.Decompress(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      if (bytes != null)
      {
        this.saveFileDialog1.Filter = "Arm 9(*.bin)|*.bin";
        this.saveFileDialog1.DefaultExt = "bin";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          System.IO.File.Create(this.saveFileDialog1.FileName).Close();
          System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, bytes);
        }
      }
    }

    private void menuItem41_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Basic Model(*.bmd)|*.bmd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.GCN.BMD b = new MKDS_Course_Modifier.GCN.BMD(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      this.openFileDialog2.Filter = "Basic Character Key Animation(*.bck)|*.bck";
      if (this.openFileDialog2.ShowDialog() == DialogResult.OK && this.openFileDialog2.FileName.Length > 0)
        System.IO.File.WriteAllBytes("C:\\tmpbone.ma", new BCK(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName)).ANK1.ExportBonesAnimation(b));
    }

    private void menuItem42_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Basic Model(*.bmd)|*.bmd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.GCN.BMD b = new MKDS_Course_Modifier.GCN.BMD(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      this.openFileDialog2.Filter = "Basic Character Animation(*.bca)|*.bca";
      if (this.openFileDialog2.ShowDialog() == DialogResult.OK && this.openFileDialog2.FileName.Length > 0)
        System.IO.File.WriteAllBytes("C:\\tmpbone.ma", new BCA(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName)).ANF1.ExportBonesAnimation(b));
    }

    private void menuItem40_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Basic Model(*.bmd)|*.bmd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.IO.File.WriteAllBytes("C:\\tmpbone.ma", new MKDS_Course_Modifier.GCN.BMD(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName)).ExportBones());
    }

    private void menuItem44_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Portable Network Graphics(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.Drawing.Bitmap b = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
      this.saveFileDialog1.Filter = "Nitro Color Palette for Runtime(*.nclr)|*.nclr";
      this.saveFileDialog1.DefaultExt = "nclr";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        string fileName1 = this.saveFileDialog1.FileName;
        this.saveFileDialog1.Filter = "Nitro Character Graphics for Runtime(*.ncgr)|*.ncgr";
        this.saveFileDialog1.DefaultExt = "ncgr";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          string fileName2 = this.saveFileDialog1.FileName;
          this.saveFileDialog1.Filter = "Nitro Screen for Runtime(*.nscr)|*.nscr";
          this.saveFileDialog1.DefaultExt = "nscr";
          if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
          {
            string fileName3 = this.saveFileDialog1.FileName;
            byte[] Palette;
            byte[] Tilemap;
            byte[] Screendata;
            Graphic.ConvertBitmap(b, out Palette, out Tilemap, out Screendata, Graphic.GXTexFmt.GX_TEXFMT_PLTT256, true);
            MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
            NCGR ncgr = new NCGR(Tilemap, Tilemap.Length / 64 * 8, 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
            MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(Screendata, b.Width, b.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
            System.IO.File.Create(fileName1).Close();
            System.IO.File.WriteAllBytes(fileName1, nclr.Write());
            System.IO.File.Create(fileName2).Close();
            System.IO.File.WriteAllBytes(fileName2, ncgr.Write());
            System.IO.File.Create(fileName3).Close();
            System.IO.File.WriteAllBytes(fileName3, nscr.Write());
          }
        }
      }
      b.Dispose();
    }

    private void menuItem45_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Mario Party DS Rom(*.nds)|*.nds";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      int num = (int) new BoardSelector(new NDS(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName))).ShowDialog();
    }

    private void menuItem46_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Portable Network Graphics(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.Drawing.Bitmap a = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
      if (this.openFileDialog2.ShowDialog() == DialogResult.OK && this.openFileDialog2.FileName.Length > 0)
      {
        System.Drawing.Bitmap b = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
        this.saveFileDialog1.Filter = "Nitro Color Palette for Runtime(*.nclr)|*.nclr";
        this.saveFileDialog1.DefaultExt = "nclr";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          string fileName1 = this.saveFileDialog1.FileName;
          this.saveFileDialog1.Filter = "Nitro Character Graphics for Runtime(*.ncgr)|*.ncgr";
          this.saveFileDialog1.DefaultExt = "ncgr";
          if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
          {
            string fileName2 = this.saveFileDialog1.FileName;
            this.saveFileDialog1.Filter = "Nitro Screen for Runtime(*.nscr)|*.nscr";
            this.saveFileDialog1.DefaultExt = "nscr";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
            {
              string fileName3 = this.saveFileDialog1.FileName;
              if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
              {
                string fileName4 = this.saveFileDialog1.FileName;
                byte[] Palette;
                byte[] Tilemap;
                byte[] ScreendataA;
                byte[] ScreendataB;
                Graphic.ConvertBitmap(a, b, out Palette, out Tilemap, out ScreendataA, out ScreendataB, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                NCGR ncgr = new NCGR(Tilemap, Tilemap.Length / 64 * 8, 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
                MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr1 = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(ScreendataA, b.Width, b.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
                MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr2 = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(ScreendataB, b.Width, b.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
                System.IO.File.Create(fileName1).Close();
                System.IO.File.WriteAllBytes(fileName1, nclr.Write());
                System.IO.File.Create(fileName2).Close();
                System.IO.File.WriteAllBytes(fileName2, ncgr.Write());
                System.IO.File.Create(fileName3).Close();
                System.IO.File.WriteAllBytes(fileName3, nscr1.Write());
                System.IO.File.Create(fileName4).Close();
                System.IO.File.WriteAllBytes(fileName4, nscr2.Write());
              }
            }
          }
        }
        b.Dispose();
      }
      a.Dispose();
    }

    private void menuItem48_Click(object sender, EventArgs e)
    {
      int num = (int) new CoursePictureGenerator().ShowDialog();
    }

    private void menuItem50_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Basic Object Libary(*.bol)|*.bol";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      BOL bol = new BOL(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      this.saveFileDialog1.Filter = "Nitro Kart Model(*.nkm)|*.nkm";
      this.saveFileDialog1.DefaultExt = "nkm";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, bol.ToNKM().Save());
      }
    }

    private void menuItem51_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "PNG Images(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.Drawing.Bitmap bitmap1 = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
      System.Drawing.Bitmap bitmap2 = (System.Drawing.Bitmap) bitmap1.Clone();
      bitmap1.Dispose();
      System.Drawing.Color A = System.Drawing.Color.FromArgb(240, 0, 83, (int) byte.MaxValue);
      System.Drawing.Color B = System.Drawing.Color.FromArgb(165, 165, 165);
      for (int y = 0; y < bitmap2.Height; ++y)
      {
        for (int x = 0; x < bitmap2.Width; ++x)
          bitmap2.SetPixel(x, y, System.Drawing.Color.FromArgb((int) byte.MaxValue, this.add(this.mix(A, B, bitmap2.GetPixel(x, y)), bitmap2.GetPixel(x, y))));
      }
      bitmap2.Save(this.openFileDialog2.FileName + "_new.png", ImageFormat.Png);
      bitmap2.Dispose();
    }

    private System.Drawing.Color mix(System.Drawing.Color A, System.Drawing.Color B, System.Drawing.Color C)
    {
      return System.Drawing.Color.FromArgb((int) ((double) A.R * (1.0 - (double) C.R / (double) byte.MaxValue) + (double) B.R * ((double) C.R / (double) byte.MaxValue)), (int) ((double) A.G * (1.0 - (double) C.G / (double) byte.MaxValue) + (double) B.G * ((double) C.G / (double) byte.MaxValue)), (int) ((double) A.B * (1.0 - (double) C.B / (double) byte.MaxValue) + (double) B.B * ((double) C.B / (double) byte.MaxValue)));
    }

    private System.Drawing.Color add(System.Drawing.Color A, System.Drawing.Color B)
    {
      int red = (int) A.R + (int) B.R;
      int green = (int) A.G + (int) B.G;
      int blue = (int) A.B + (int) B.B;
      if (red > (int) byte.MaxValue)
        red = (int) byte.MaxValue;
      if (green > (int) byte.MaxValue)
        green = (int) byte.MaxValue;
      if (blue > (int) byte.MaxValue)
        blue = (int) byte.MaxValue;
      return System.Drawing.Color.FromArgb(red, green, blue);
    }

    private void menuItem52_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "OpenGl Fragment Shaders(*.txt)|*.txt";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      string fileName = this.openFileDialog2.FileName;
      this.openFileDialog2.Filter = "PNG Images(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() == DialogResult.OK && this.openFileDialog2.FileName.Length > 0)
      {
        int num = (int) new FragmentShaderApplier(fileName, this.openFileDialog2.FileName).ShowDialog();
      }
    }

    private void menuItem53_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Arm 9(*.bin)|*.bin";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      ARM9 arM9 = new ARM9(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      arM9.AddCustomCode(Path.GetDirectoryName(this.openFileDialog2.FileName));
      System.IO.File.WriteAllBytes(Path.GetDirectoryName(this.openFileDialog2.FileName) + "\\" + Path.GetFileNameWithoutExtension(this.openFileDialog2.FileName) + "_new.bin", arM9.Write());
    }

    private void menuItem55_Click(object sender, EventArgs e)
    {
      int num = (int) new G3DCVTR().ShowDialog();
    }

    private void menuItem57_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Compressed Overlay(*.bin)|*.bin";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      byte[] bytes = Compression.OverlayDecompress(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      this.saveFileDialog1.Filter = "Compressed Overlay(*.bin)|*.bin";
      this.saveFileDialog1.DefaultExt = "bin";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, bytes);
      }
    }

    private void menuItem58_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      this.saveFileDialog1.Filter = "Zelda Collision Base(*.zcb)|*.zcb";
      this.saveFileDialog1.DefaultExt = "zcb";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        Obj2Zcb.ConvertToZcb(this.openFileDialog2.FileName, this.saveFileDialog1.FileName);
    }

    private void menuItem59_Click(object sender, EventArgs e)
    {
      int num = (int) new GlobalMapGenerator().ShowDialog();
    }

    private void menuItem60_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Portable Network Graphics(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.Drawing.Bitmap b = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
      this.saveFileDialog1.Filter = "Nitro Color Palette for Runtime(*.nclr)|*.nclr";
      this.saveFileDialog1.DefaultExt = "nclr";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        string fileName1 = this.saveFileDialog1.FileName;
        this.saveFileDialog1.Filter = "Nitro Character Graphics for Runtime(*.ncgr)|*.ncgr";
        this.saveFileDialog1.DefaultExt = "ncgr";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          string fileName2 = this.saveFileDialog1.FileName;
          this.saveFileDialog1.Filter = "Nitro Screen for Runtime(*.nscr)|*.nscr";
          this.saveFileDialog1.DefaultExt = "nscr";
          if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
          {
            string fileName3 = this.saveFileDialog1.FileName;
            byte[] Palette;
            byte[] Tilemap;
            byte[] Screendata;
            Graphic.ConvertBitmap(b, out Palette, out Tilemap, out Screendata, Graphic.GXTexFmt.GX_TEXFMT_PLTT256, false);
            MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
            NCGR ncgr = new NCGR(Tilemap, Tilemap.Length / 64 * 8, 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
            MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(Screendata, b.Width, b.Height, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
            System.IO.File.Create(fileName1).Close();
            System.IO.File.WriteAllBytes(fileName1, nclr.Write());
            System.IO.File.Create(fileName2).Close();
            System.IO.File.WriteAllBytes(fileName2, ncgr.Write());
            System.IO.File.Create(fileName3).Close();
            System.IO.File.WriteAllBytes(fileName3, nscr.Write());
          }
        }
      }
      b.Dispose();
    }

    private void menuItem61_Click(object sender, EventArgs e)
    {
      int num = (int) new ObjectDbEditor().ShowDialog();
    }

    private void menuItem62_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      this.saveFileDialog1.Filter = "Maya ASCII(*.ma)|*.ma";
      this.saveFileDialog1.DefaultExt = "ma";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, MA.Obj2Ma(new MKDS_Course_Modifier._3D_Formats.OBJ(this.openFileDialog2.FileName)));
      }
    }

    private void menuItem63_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier._3D_Formats.OBJ obj = MKDS_Course_Modifier._3D_Formats.OBJ.FixNitroUV(new MKDS_Course_Modifier._3D_Formats.OBJ(this.openFileDialog2.FileName));
      if (obj != null)
      {
        this.saveFileDialog1.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
        this.saveFileDialog1.DefaultExt = "obj";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
          obj.Write(this.saveFileDialog1.FileName);
      }
    }

    private void menuItem64_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      this.saveFileDialog1.Filter = "Nitro System Basic Model(*.nsbmd)|*.nsbmd";
      this.saveFileDialog1.DefaultExt = "nsbmd";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        Obj2Nsbmd.ConvertToNsbmd(this.openFileDialog2.FileName, this.saveFileDialog1.FileName);
    }

    private void menuItem65_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Encrypted MKDS Raw Save File(*.*)|*.*";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      byte[] bytes = MKDS_Const.DecryptSave(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      this.saveFileDialog1.Filter = "Decrypted MKDS Raw Save File(*.*)|*.*";
      this.saveFileDialog1.DefaultExt = "";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, bytes);
      }
    }

    private void menuItem66_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Decrypted MKDS Raw Save File(*.*)|*.*";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      byte[] bytes = MKDS_Const.EncryptSave(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      this.saveFileDialog1.Filter = "Encrypted MKDS Raw Save File(*.*)|*.*";
      this.saveFileDialog1.DefaultExt = "";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, bytes);
      }
    }

    private void menuItem68_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Portable Network Graphics(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.Drawing.Bitmap b = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
      this.saveFileDialog1.Filter = "Nitro Color Palette for Runtime(*.nclr)|*.nclr";
      this.saveFileDialog1.DefaultExt = "nclr";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        string fileName1 = this.saveFileDialog1.FileName;
        this.saveFileDialog1.Filter = "Nitro Character Graphics for Runtime(*.ncgr)|*.ncgr";
        this.saveFileDialog1.DefaultExt = "ncgr";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          string fileName2 = this.saveFileDialog1.FileName;
          byte[] Data;
          byte[] Palette;
          Graphic.ConvertBitmap(b, out Data, out Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out bool _, false);
          MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
          NCGR ncgr = new NCGR(Data, b.Width / 8, b.Height / 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
          System.IO.File.Create(fileName1).Close();
          System.IO.File.WriteAllBytes(fileName1, nclr.Write());
          System.IO.File.Create(fileName2).Close();
          System.IO.File.WriteAllBytes(fileName2, ncgr.Write());
        }
      }
      b.Dispose();
    }

    private void menuItem69_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Portable Network Graphics(*.png)|*.png";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      System.Drawing.Bitmap b = (System.Drawing.Bitmap) Image.FromFile(this.openFileDialog2.FileName);
      this.saveFileDialog1.Filter = "Nitro Color Palette for Runtime(*.nclr)|*.nclr";
      this.saveFileDialog1.DefaultExt = "nclr";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        string fileName1 = this.saveFileDialog1.FileName;
        this.saveFileDialog1.Filter = "Nitro Character Graphics for Runtime(*.ncgr)|*.ncgr";
        this.saveFileDialog1.DefaultExt = "ncgr";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          string fileName2 = this.saveFileDialog1.FileName;
          byte[] Data;
          byte[] Palette;
          Graphic.ConvertBitmap(b, out Data, out Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, out bool _, false);
          MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(Palette, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
          NCGR ncgr = new NCGR(Data, b.Width / 8, b.Height / 8, Graphic.GXTexFmt.GX_TEXFMT_PLTT256);
          System.IO.File.Create(fileName1).Close();
          System.IO.File.WriteAllBytes(fileName1, nclr.Write());
          System.IO.File.Create(fileName2).Close();
          System.IO.File.WriteAllBytes(fileName2, ncgr.Write());
        }
      }
      b.Dispose();
    }

    private void menuItem70_Click(object sender, EventArgs e)
    {
      int num = (int) new ASMEditor().ShowDialog();
    }

    private void menuItem72_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Mario Kart 64 [U](*.rom)|*.rom";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.N64.MK64.MK64 mk64 = new MKDS_Course_Modifier.N64.MK64.MK64(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
    }

    private void menuItem75_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Nitro System Basic Model(*.nsbmd)|*.nsbmd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      foreach (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model model in nsbmd.modelSet.models)
      {
        foreach (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD.ModelSet.Model.ShapeSet.Shape shape in model.shapes.shape)
          shape.DL = GlNitro.RemoveNormals(shape.DL);
      }
      System.IO.File.Create(this.openFileDialog2.FileName).Close();
      System.IO.File.WriteAllBytes(this.openFileDialog2.FileName, nsbmd.Write());
    }

    private void menuItem76_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier._3D_Formats.OBJ obj = new MKDS_Course_Modifier._3D_Formats.OBJ(this.openFileDialog2.FileName);
      ScaleDialog scaleDialog = new ScaleDialog();
      int num = (int) scaleDialog.ShowDialog();
      for (int index = 0; index < obj.Vertices.Count; ++index)
        obj.Vertices[index] = Vector3.Multiply(obj.Vertices[index], scaleDialog.scale);
      this.saveFileDialog1.Filter = "Wavefront OBJ Files(*.obj)|*.obj";
      this.saveFileDialog1.DefaultExt = "obj";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        obj.Write(this.saveFileDialog1.FileName);
    }

    private void menuItem77_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Nitro Screen for Runtime(*.nscr)|*.nscr";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
      if (nscr.ScreenData.Data.Length != 2048)
      {
        int num = (int) System.Windows.MessageBox.Show("Wrong Size!");
      }
      ++nscr.ScreenData.Data[556];
      for (int index = 558; index < nscr.ScreenData.Data.Length; index += 2)
      {
        if (nscr.ScreenData.Data[index] != (byte) 0)
          ++nscr.ScreenData.Data[index];
      }
      this.saveFileDialog1.Filter = "Nitro Screen for Runtime(*.nscr)|*.nscr";
      this.saveFileDialog1.DefaultExt = "nscr";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        System.IO.File.Create(this.saveFileDialog1.FileName).Close();
        System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, nscr.Write());
      }
    }

    private void menuItem78_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Nitro System Basic Model(*.nsbmd)|*.nsbmd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      this.saveFileDialog1.Filter = "Nitro System Basic Model(*.nsbmd)|*.nsbmd";
      this.saveFileDialog1.DefaultExt = "nsbmd";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
      {
        string fileName = this.saveFileDialog1.FileName;
        this.saveFileDialog1.Filter = "Nitro System Basic Texture(*.nsbtx)|*.nsbtx";
        this.saveFileDialog1.DefaultExt = "nsbtx";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX nsbtx = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX();
          nsbtx.TexPlttSet = nsbmd.TexPlttSet;
          nsbmd.TexPlttSet = (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet) null;
          System.IO.File.Create(fileName).Close();
          System.IO.File.WriteAllBytes(fileName, nsbmd.Write());
          System.IO.File.Create(this.saveFileDialog1.FileName).Close();
          System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, nsbtx.Write());
        }
      }
    }

    private void menuItem79_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Nitro System Basic Model(*.nsbmd)|*.nsbmd";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      string fileName = this.openFileDialog2.FileName;
      this.openFileDialog2.Filter = "Nitro System Basic Texture(*.nsbtx)|*.nsbtx";
      if (this.openFileDialog2.ShowDialog() == DialogResult.OK && this.openFileDialog2.FileName.Length > 0)
      {
        this.saveFileDialog1.Filter = "Nitro System Basic Model(*.nsbmd)|*.nsbmd";
        this.saveFileDialog1.DefaultExt = "nsbmd";
        if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        {
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD nsbmd = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBMD(System.IO.File.ReadAllBytes(fileName));
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX nsbtx = new MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX(System.IO.File.ReadAllBytes(this.openFileDialog2.FileName));
          nsbmd.TexPlttSet = nsbtx.TexPlttSet;
          System.IO.File.Create(this.saveFileDialog1.FileName).Close();
          System.IO.File.WriteAllBytes(this.saveFileDialog1.FileName, nsbmd.Write());
        }
      }
    }

    private void menuItem80_Click(object sender, EventArgs e)
    {
      this.openFileDialog2.Filter = "Basic Collision(*.bco)|*.bco";
      if (this.openFileDialog2.ShowDialog() != DialogResult.OK || this.openFileDialog2.FileName.Length <= 0)
        return;
      this.saveFileDialog1.Filter = "Wavefront OBJ(*.obj)|*.obj";
      this.saveFileDialog1.DefaultExt = "obj";
      if (this.saveFileDialog1.ShowDialog() == DialogResult.OK && this.saveFileDialog1.FileName.Length > 0)
        BCO.Read(this.openFileDialog2.FileName, this.saveFileDialog1.FileName);
    }

    public class ListViewNF : ListView
    {
      public ListViewNF()
      {
        this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        this.SetStyle(ControlStyles.EnableNotifyMessage, true);
      }

      protected override void OnNotifyMessage(Message m)
      {
        if (m.Msg == 20)
          return;
        base.OnNotifyMessage(m);
      }
    }
  }
}
