// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NCER
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.G2D_Binary_File_Format;
using MKDS_Course_Modifier.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class NCER : Form
  {
    private IContainer components = (IContainer) null;
    private int GraphicId = -1;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR Palette = (MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR) null;
    private NCGR Graphic = (NCGR) null;
    private Size[] Sizes = new Size[12]
    {
      new Size(8, 8),
      new Size(16, 16),
      new Size(32, 32),
      new Size(64, 64),
      new Size(16, 8),
      new Size(32, 8),
      new Size(32, 16),
      new Size(64, 32),
      new Size(8, 16),
      new Size(8, 32),
      new Size(16, 32),
      new Size(32, 64)
    };
    private SplitContainer splitContainer1;
    private ListBox listBox1;
    private SplitContainer splitContainer2;
    private SplitContainer splitContainer3;
    private PictureBox pictureBox1;
    private PictureBox pictureBox2;
    private PropertyGrid propertyGrid1;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStrip toolStrip2;
    private ToolStripComboBox toolStripComboBox1;
    private ToolStripLabel toolStripLabel1;
    private ToolStripButton toolStripButton2;
    private OpenFileDialog openFileDialog1;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripButton toolStripButton3;
    private SaveFileDialog saveFileDialog1;
    private MKDS_Course_Modifier.G2D_Binary_File_Format.NCER Cell;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (NCER));
      this.splitContainer1 = new SplitContainer();
      this.listBox1 = new ListBox();
      this.splitContainer2 = new SplitContainer();
      this.splitContainer3 = new SplitContainer();
      this.pictureBox1 = new PictureBox();
      this.toolStrip2 = new ToolStrip();
      this.toolStripLabel1 = new ToolStripLabel();
      this.toolStripComboBox1 = new ToolStripComboBox();
      this.pictureBox2 = new PictureBox();
      this.propertyGrid1 = new PropertyGrid();
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton2 = new ToolStripButton();
      this.openFileDialog1 = new OpenFileDialog();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStripButton3 = new ToolStripButton();
      this.saveFileDialog1 = new SaveFileDialog();
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
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.toolStrip2.SuspendLayout();
      ((ISupportInitialize) this.pictureBox2).BeginInit();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.listBox1);
      this.splitContainer1.Panel2.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Size = new Size(676, 348);
      this.splitContainer1.SplitterDistance = 149;
      this.splitContainer1.TabIndex = 0;
      this.listBox1.Dock = DockStyle.Fill;
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new Point(0, 0);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new Size(149, 348);
      this.listBox1.TabIndex = 0;
      this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.Location = new Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Panel1.Controls.Add((Control) this.splitContainer3);
      this.splitContainer2.Panel2.Controls.Add((Control) this.propertyGrid1);
      this.splitContainer2.Size = new Size(523, 348);
      this.splitContainer2.SplitterDistance = 362;
      this.splitContainer2.TabIndex = 0;
      this.splitContainer3.Dock = DockStyle.Fill;
      this.splitContainer3.Location = new Point(0, 0);
      this.splitContainer3.Name = "splitContainer3";
      this.splitContainer3.Orientation = Orientation.Horizontal;
      this.splitContainer3.Panel1.Controls.Add((Control) this.pictureBox1);
      this.splitContainer3.Panel1.Controls.Add((Control) this.toolStrip2);
      this.splitContainer3.Panel2.Controls.Add((Control) this.pictureBox2);
      this.splitContainer3.Size = new Size(362, 348);
      this.splitContainer3.SplitterDistance = 158;
      this.splitContainer3.TabIndex = 0;
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 25);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(362, 133);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.toolStrip2.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripLabel1,
        (ToolStripItem) this.toolStripComboBox1
      });
      this.toolStrip2.Location = new Point(0, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new Size(362, 25);
      this.toolStrip2.TabIndex = 1;
      this.toolStrip2.Text = "toolStrip2";
      this.toolStripLabel1.Name = "toolStripLabel1";
      this.toolStripLabel1.Size = new Size(46, 22);
      this.toolStripLabel1.Text = "Palette:";
      this.toolStripComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
      this.toolStripComboBox1.FlatStyle = FlatStyle.System;
      this.toolStripComboBox1.Name = "toolStripComboBox1";
      this.toolStripComboBox1.Size = new Size(121, 25);
      this.toolStripComboBox1.SelectedIndexChanged += new EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
      this.pictureBox2.Dock = DockStyle.Fill;
      this.pictureBox2.Location = new Point(0, 0);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new Size(362, 186);
      this.pictureBox2.TabIndex = 0;
      this.pictureBox2.TabStop = false;
      this.propertyGrid1.Dock = DockStyle.Fill;
      this.propertyGrid1.Location = new Point(0, 0);
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.Size = new Size(157, 348);
      this.propertyGrid1.TabIndex = 0;
      this.toolStrip1.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton3
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(676, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "Add Cell";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.openFileDialog1.FileName = "openFileDialog1";
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "Export";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.saveFileDialog1.DefaultExt = "png";
      this.saveFileDialog1.Filter = "PNG Images (*.png)|*.png";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(676, 373);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (NCER);
      this.Text = nameof (NCER);
      this.Load += new EventHandler(this.NCER_Load);
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
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      ((ISupportInitialize) this.pictureBox2).EndInit();
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public NCER(MKDS_Course_Modifier.G2D_Binary_File_Format.NCER Cell)
    {
      this.Cell = Cell;
      this.InitializeComponent();
    }

    public void SetNCLR(MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR Palette)
    {
      this.Palette = Palette;
      if (Palette == null || this.Graphic == null)
        return;
      this.LoadCells();
    }

    public void SetNCGR(NCGR Graphic, int ID)
    {
      this.Graphic = Graphic;
      if (this.GraphicId != -1)
      {
        FileHandler.CloseFile(this.GraphicId);
        this.GraphicId = ID - 1;
      }
      else
        this.GraphicId = ID;
      if (this.Palette == null || Graphic == null)
        return;
      this.LoadCells();
    }

    private void NCER_Load(object sender, EventArgs e)
    {
    }

    private void LoadCells()
    {
      this.listBox1.Items.Clear();
      for (int index = 0; index < (int) this.Cell.CellBankBlock.CellDataBank.numCells; ++index)
        this.listBox1.Items.Add((object) ("Cell " + (object) index));
      this.toolStripComboBox1.Items.Clear();
      for (int index = 0; index < 16; ++index)
        this.toolStripComboBox1.Items.Add((object) string.Concat((object) index));
      this.listBox1.SetSelected(0, true);
      this.toolStripComboBox1.SelectedIndex = 0;
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.listBox1.SelectedIndices.Count == 0 || this.Palette == null || this.Graphic == null)
        return;
      this.pictureBox2.Image = (Image) this.Cell.CellBankBlock.CellDataBank.GetBitmap(this.listBox1.SelectedIndex, this.Graphic, this.Palette);
    }

    private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) this.Graphic.CharacterData.ToBitmap(this.Palette, this.toolStripComboBox1.SelectedIndex);
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      Bitmap bitmap = new Bitmap(this.openFileDialog1.FileName);
      Bitmap b1 = (Bitmap) bitmap.Clone();
      b1.SetResolution(96f, 96f);
      bitmap.Dispose();
      if (((IEnumerable<Size>) this.Sizes).Contains<Size>(b1.Size))
      {
        ++this.Cell.CellBankBlock.CellDataBank.numCells;
        Array.Resize<MKDS_Course_Modifier.G2D_Binary_File_Format.NCER.cellBankBlock.cellDataBank.cellData>(ref this.Cell.CellBankBlock.CellDataBank.CellData, (int) this.Cell.CellBankBlock.CellDataBank.numCells);
        this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1] = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCER.cellBankBlock.cellDataBank.cellData();
        this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].numOAMAttrs = (ushort) 1;
        if (this.Cell.CellBankBlock.CellDataBank.cellBankAttr == (ushort) 1)
          this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].boundingRect = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCER.cellBankBlock.cellDataBank.cellData.CellBoundingRectS16((short) b1.Width, (short) b1.Height);
        this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].cellAttr = (ushort) 0;
        this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].CellOAMAttrData = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCER.cellBankBlock.cellDataBank.cellData.cellOAMAttrData[1];
        int num1 = ((IEnumerable<Size>) this.Sizes).ToList<Size>().IndexOf(b1.Size) / 4;
        int num2 = ((IEnumerable<Size>) this.Sizes).ToList<Size>().IndexOf(b1.Size) - num1 * 4;
        this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].CellOAMAttrData[0] = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCER.cellBankBlock.cellDataBank.cellData.cellOAMAttrData((short) -(b1.Width / 2), (sbyte) -(b1.Height / 2), (byte) num2, (byte) num1, (byte) this.toolStripComboBox1.SelectedIndex);
        if (this.Cell.CellBankBlock.CellDataBank.mappingMode == MKDS_Course_Modifier.G2D_Binary_File_Format.NCER.cellBankBlock.cellDataBank.CharacterDataMapingType.NNS_G2D_CHARACTERMAPING_2D)
        {
          this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].CellOAMAttrData[0].StartingCharacterName = (short) ((int) this.Graphic.CharacterData.H * (int) this.Graphic.CharacterData.W);
          this.Graphic.CharacterData.H += (ushort) (b1.Height / 8);
          Bitmap b2 = new Bitmap((int) this.Graphic.CharacterData.W * 8, b1.Height);
          using (Graphics graphics = Graphics.FromImage((Image) b2))
            graphics.DrawImage((Image) b1, 0, 0);
          byte[] Data;
          MKDS_Course_Modifier.Converters.Graphic.ConvertBitmap(b2, out Data, ((IEnumerable<Color>) this.Palette.PaletteData.ToColorArray()).ToList<Color>().GetRange(this.toolStripComboBox1.SelectedIndex * 16, 16).ToArray(), MKDS_Course_Modifier.Converters.Graphic.GXTexFmt.GX_TEXFMT_PLTT16, MKDS_Course_Modifier.Converters.Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, true);
          Array.Resize<byte>(ref this.Graphic.CharacterData.Data, this.Graphic.CharacterData.Data.Length + Data.Length);
          Array.Copy((Array) Data, 0, (Array) this.Graphic.CharacterData.Data, this.Graphic.CharacterData.Data.Length - Data.Length, Data.Length);
          this.listBox1.Items.Clear();
          for (int index = 0; index < (int) this.Cell.CellBankBlock.CellDataBank.numCells; ++index)
            this.listBox1.Items.Add((object) ("Cell " + (object) index));
          this.toolStripComboBox1.SelectedIndex = 0;
        }
        else
        {
          this.Cell.CellBankBlock.CellDataBank.CellData[(int) this.Cell.CellBankBlock.CellDataBank.numCells - 1].CellOAMAttrData[0].StartingCharacterName = (short) (this.Graphic.CharacterData.Data.Length / 32);
          byte[] Data;
          MKDS_Course_Modifier.Converters.Graphic.ConvertBitmap(b1, out Data, ((IEnumerable<Color>) this.Palette.PaletteData.ToColorArray()).ToList<Color>().GetRange(this.toolStripComboBox1.SelectedIndex * 16, 16).ToArray(), MKDS_Course_Modifier.Converters.Graphic.GXTexFmt.GX_TEXFMT_PLTT16, MKDS_Course_Modifier.Converters.Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR, true);
          Array.Resize<byte>(ref this.Graphic.CharacterData.Data, this.Graphic.CharacterData.Data.Length + Data.Length);
          Array.Copy((Array) Data, 0, (Array) this.Graphic.CharacterData.Data, this.Graphic.CharacterData.Data.Length - Data.Length, Data.Length);
          this.listBox1.Items.Clear();
          for (int index = 0; index < (int) this.Cell.CellBankBlock.CellDataBank.numCells; ++index)
            this.listBox1.Items.Add((object) ("Cell " + (object) index));
          this.toolStripComboBox1.SelectedIndex = 0;
        }
      }
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      FileHandler.Save(this.Cell.Write(), 0, false);
      FileHandler.Save(this.Graphic.Write(), this.GraphicId, false);
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0 || this.pictureBox2.Image == null)
        return;
      this.pictureBox2.Image.Save(this.saveFileDialog1.FileName, ImageFormat.Png);
    }
  }
}
