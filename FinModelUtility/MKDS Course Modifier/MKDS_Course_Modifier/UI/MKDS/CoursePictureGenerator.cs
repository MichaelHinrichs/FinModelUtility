// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.CoursePictureGenerator
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.G2D_Binary_File_Format;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class CoursePictureGenerator : Form
  {
    private bool zoom = true;
    private List<KeyValuePair<int, int>> selected = new List<KeyValuePair<int, int>>();
    private byte[,] selectedpalettes = new byte[10, 11];
    private IContainer components = (IContainer) null;
    private byte[] PaletteData;
    private byte[] ImageData;
    private byte[] ScreenData;
    private ToolStrip toolStrip1;
    private PictureBox pictureBox1;
    private ToolStripButton toolStripButton1;
    private OpenFileDialog openFileDialog1;
    private ToolStripButton toolStripButton2;
    private ToolStripButton toolStripButton3;
    private ToolStripButton toolStripButton4;
    private SaveFileDialog saveFileDialog1;
    private SaveFileDialog saveFileDialog2;
    private SaveFileDialog saveFileDialog3;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton toolStripButton5;
    private Panel panel1;
    private PictureBox pictureBox2;
    private Button button1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStripProgressBar toolStripProgressBar1;
    private StatusStrip statusStrip1;
    private ToolStripButton toolStripButton6;
    private ToolStripSeparator toolStripSeparator2;

    public CoursePictureGenerator()
    {
      this.InitializeComponent();
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      this.toolStripButton2.Enabled = true;
      this.toolStripButton3.Enabled = true;
      this.toolStripButton4.Enabled = true;
      this.toolStripButton5.Enabled = true;
      this.toolStripButton6.Enabled = true;
      this.button1.Enabled = true;
      Bitmap bitmap = new Bitmap(80, 88);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap))
      {
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.Clear(Color.FromArgb(240, 152, 192));
        graphics.DrawImage(Image.FromFile(this.openFileDialog1.FileName), new Rectangle(4, 0, 70, 88), new Rectangle(0, 0, 70, 88), GraphicsUnit.Pixel);
      }
      this.pictureBox2.Image = (Image) bitmap;
    }

    private void toolStripButton4_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog3.ShowDialog() != DialogResult.OK || this.saveFileDialog3.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR nscr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NSCR(this.ScreenData, 256, 256, Graphic.NNSG2dColorMode.NNS_G2D_SCREENCOLORMODE_16x16);
      File.Create(this.saveFileDialog3.FileName).Close();
      File.WriteAllBytes(this.saveFileDialog3.FileName, nscr.Write());
    }

    private void toolStripButton5_Click(object sender, EventArgs e)
    {
      this.toolStripButton2.Enabled = true;
      this.toolStripButton3.Enabled = true;
      this.toolStripButton4.Enabled = true;
      this.Generate();
    }

    public void Generate()
    {
      Bitmap b1 = new Bitmap(80, 88);
      using (Graphics graphics = Graphics.FromImage((Image) b1))
      {
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.DrawImage(Image.FromFile(this.openFileDialog1.FileName), new Rectangle(4, 0, 70, 88), new Rectangle(0, 0, 70, 88), GraphicsUnit.Pixel);
      }
      Graphic.Tiles8x8 tiles8x8 = new Graphic.Tiles8x8(b1, false);
      List<Graphic.Tile8x8>[] tile8x8ListArray = new List<Graphic.Tile8x8>[7]
      {
        new List<Graphic.Tile8x8>(),
        new List<Graphic.Tile8x8>(),
        new List<Graphic.Tile8x8>(),
        new List<Graphic.Tile8x8>(),
        new List<Graphic.Tile8x8>(),
        new List<Graphic.Tile8x8>(),
        new List<Graphic.Tile8x8>()
      };
      for (int index1 = 0; index1 < 11; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
          tile8x8ListArray[(int) this.selectedpalettes[index2, index1]].Add(tiles8x8.AllTiles[index1 * 10 + index2]);
      }
      Color[][] colorArray = new Color[16][];
      List<byte> byteList1 = new List<byte>();
      for (int index1 = 0; index1 < 7; ++index1)
      {
        if (tile8x8ListArray[index1].Count != 0)
        {
          Bitmap b2 = new Bitmap(8 * tile8x8ListArray[index1].Count, 8);
          using (Graphics graphics = Graphics.FromImage((Image) b2))
          {
            int num = 0;
            foreach (Graphic.Tile8x8 tile8x8 in tile8x8ListArray[index1])
            {
              graphics.DrawImage((Image) tile8x8.ToBitmap(), num * 8, 0);
              ++num;
            }
          }
          Color[] palette = Graphic.GeneratePalette(b2, 16, true, true);
          palette[0] = Color.FromArgb(240, 152, 192);
          colorArray[index1] = palette;
        }
        else
        {
          colorArray[index1] = new Color[16];
          colorArray[index1][0] = Color.FromArgb(240, 152, 192);
          for (int index2 = 1; index2 < 16; ++index2)
            colorArray[index1][index2] = Color.Black;
        }
      }
      for (int index1 = 0; index1 < 9; ++index1)
      {
        colorArray[index1 + 7] = new Color[16];
        for (int index2 = 0; index2 < 16; ++index2)
          colorArray[index1 + 7][index2] = Color.Black;
      }
      for (int index1 = 0; index1 < 11; ++index1)
      {
        for (int index2 = 0; index2 < 10; ++index2)
          byteList1.AddRange((IEnumerable<byte>) tiles8x8.AllTiles[index1 * 10 + index2].GetData(colorArray[(int) this.selectedpalettes[index2, index1]], Graphic.GXTexFmt.GX_TEXFMT_PLTT16, true));
      }
      List<Color> colorList = new List<Color>();
      for (int index = 0; index < 16; ++index)
        colorList.AddRange((IEnumerable<Color>) colorArray[index]);
      this.PaletteData = Graphic.ToABGR1555(colorList.ToArray());
      this.ImageData = byteList1.ToArray();
      MemoryStream memoryStream = new MemoryStream();
      EndianBinaryWriter endianBinaryWriter = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
      for (int index = 0; index < 278; ++index)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      int index3 = 0;
      int index4 = 0;
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index3, index4] & 15) << 12) + 0 + 0 + (index4 * 10 + index3 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index3;
      }
      int index5 = 0;
      int index6 = index4 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index5, index6] & 15) << 12) + 0 + 0 + (index6 * 10 + index5 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index5;
      }
      int index7 = 0;
      int index8 = index6 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index7, index8] & 15) << 12) + 0 + 0 + (index8 * 10 + index7 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index7;
      }
      int index9 = 0;
      int index10 = index8 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index9, index10] & 15) << 12) + 0 + 0 + (index10 * 10 + index9 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index9;
      }
      int index11 = 0;
      int index12 = index10 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index11, index12] & 15) << 12) + 0 + 0 + (index12 * 10 + index11 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index11;
      }
      int index13 = 0;
      int index14 = index12 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index13, index14] & 15) << 12) + 0 + 0 + (index14 * 10 + index13 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index13;
      }
      int index15 = 0;
      int index16 = index14 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index15, index16] & 15) << 12) + 0 + 0 + (index16 * 10 + index15 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index15;
      }
      int index17 = 0;
      int index18 = index16 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index17, index18] & 15) << 12) + 0 + 0 + (index18 * 10 + index17 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index17;
      }
      int index19 = 0;
      int index20 = index18 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index19, index20] & 15) << 12) + 0 + 0 + (index20 * 10 + index19 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index19;
      }
      int index21 = 0;
      int index22 = index20 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index21, index22] & 15) << 12) + 0 + 0 + (index22 * 10 + index21 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index21;
      }
      int index23 = 0;
      int index24 = index22 + 1;
      for (int index1 = 0; index1 < 22; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      for (int index1 = 0; index1 < 10; ++index1)
      {
        int num = (((int) this.selectedpalettes[index23, index24] & 15) << 12) + 0 + 0 + (index24 * 10 + index23 + 1 & 1023);
        endianBinaryWriter.Write((ushort) num);
        ++index23;
      }
      for (int index1 = 0; index1 < 416; ++index1)
      {
        int num = 0 + 0 + 0;
        endianBinaryWriter.Write((ushort) num);
      }
      this.ScreenData = memoryStream.ToArray();
      endianBinaryWriter.Close();
      List<byte> byteList2 = new List<byte>();
      byteList2.AddRange((IEnumerable<byte>) new byte[32]);
      byteList2.AddRange((IEnumerable<byte>) this.ImageData);
      this.pictureBox1.Image = (Image) Graphic.ConvertData(byteList2.ToArray(), 80, 88, this.PaletteData, this.ScreenData, 256, 256, Graphic.GXTexFmt.GX_TEXFMT_PLTT16, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_CHAR);
    }

    private void toolStripTextBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (char.IsNumber(e.KeyChar))
        return;
      e.Handled = false;
    }

    private void pictureBox2_Paint(object sender, PaintEventArgs e)
    {
      if (this.pictureBox2.Image == null)
        return;
      e.Graphics.Clear(this.pictureBox2.BackColor);
      e.Graphics.SmoothingMode = SmoothingMode.None;
      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
      e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
      e.Graphics.DrawImage(this.pictureBox2.Image, 0, 0, this.zoom ? 160 : 80, this.zoom ? 176 : 88);
      e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
      for (int index1 = 0; index1 < 10; ++index1)
      {
        for (int index2 = 0; index2 < 11; ++index2)
        {
          e.Graphics.DrawRectangle(Pens.Black, index1 * (this.zoom ? 16 : 8), index2 * (this.zoom ? 16 : 8), this.zoom ? 16 : 8, this.zoom ? 16 : 8);
          e.Graphics.DrawString(this.selectedpalettes[index1, index2].ToString(), new Font(SystemFonts.DefaultFont.Name, this.zoom ? 10f : 5f), (Brush) new SolidBrush(Color.Black), (float) (index1 * (this.zoom ? 16 : 8)), (float) (index2 * (this.zoom ? 16 : 8)));
        }
      }
      if (this.selected.Count != 0)
      {
        for (int index = 0; index < this.selected.Count; ++index)
        {
          Graphics graphics = e.Graphics;
          Pen white = Pens.White;
          KeyValuePair<int, int> keyValuePair = this.selected[index];
          int x = keyValuePair.Key * (this.zoom ? 16 : 8);
          keyValuePair = this.selected[index];
          int y = keyValuePair.Value * (this.zoom ? 16 : 8);
          int width = this.zoom ? 16 : 8;
          int height = this.zoom ? 16 : 8;
          graphics.DrawRectangle(white, x, y, width, height);
        }
      }
    }

    private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
    {
      this.pictureBox2.Focus();
      bool flag = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
      if (e.X >= (this.zoom ? 160 : 80) || e.Y >= (this.zoom ? 176 : 88) || e.Button != MouseButtons.Left)
        return;
      if (flag && !this.selected.Contains(new KeyValuePair<int, int>(e.X / (this.zoom ? 16 : 8), e.Y / (this.zoom ? 16 : 8))))
        this.selected.Add(new KeyValuePair<int, int>(e.X / (this.zoom ? 16 : 8), e.Y / (this.zoom ? 16 : 8)));
      else if (flag && this.selected.Contains(new KeyValuePair<int, int>(e.X / (this.zoom ? 16 : 8), e.Y / (this.zoom ? 16 : 8))) && this.selected.Count > 1)
        this.selected.Remove(new KeyValuePair<int, int>(e.X / (this.zoom ? 16 : 8), e.Y / (this.zoom ? 16 : 8)));
      else if (!flag)
      {
        this.selected.Clear();
        this.selected.Add(new KeyValuePair<int, int>(e.X / (this.zoom ? 16 : 8), e.Y / (this.zoom ? 16 : 8)));
      }
      this.pictureBox2.Refresh();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.zoom = !this.zoom;
      this.pictureBox2.Refresh();
    }

    private void CoursePictureGenerator_KeyDown(object sender, KeyEventArgs e)
    {
    }

    private void CoursePictureGenerator_KeyUp(object sender, KeyEventArgs e)
    {
    }

    private void CoursePictureGenerator_KeyPress(object sender, KeyPressEventArgs e)
    {
    }

    private void pictureBox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
    {
      char keyValue = (char) e.KeyValue;
      if (this.selected.Count == 0 || !char.IsNumber(keyValue) || Convert.ToInt32(keyValue.ToString()) >= 7)
        return;
      for (int index1 = 0; index1 < this.selected.Count; ++index1)
      {
        byte[,] selectedpalettes = this.selectedpalettes;
        KeyValuePair<int, int> keyValuePair = this.selected[index1];
        int key = keyValuePair.Key;
        keyValuePair = this.selected[index1];
        int index2 = keyValuePair.Value;
        int int32 = (int) (byte) Convert.ToInt32(keyValue.ToString());
        selectedpalettes[key, index2] = (byte) int32;
      }
      this.pictureBox2.Refresh();
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog2.ShowDialog() != DialogResult.OK || this.saveFileDialog2.FileName.Length <= 0)
        return;
      NCGR ncgr = new NCGR(this.ImageData, 80, 88, Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
      File.Create(this.saveFileDialog2.FileName).Close();
      File.WriteAllBytes(this.saveFileDialog2.FileName, ncgr.Write());
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR nclr = new MKDS_Course_Modifier.G2D_Binary_File_Format.NCLR(this.PaletteData, Graphic.GXTexFmt.GX_TEXFMT_PLTT16);
      File.Create(this.saveFileDialog1.FileName).Close();
      File.WriteAllBytes(this.saveFileDialog1.FileName, nclr.Write());
    }

    private void toolStripButton6_Click(object sender, EventArgs e)
    {
      Bitmap b = new Bitmap(80, 88);
      using (Graphics graphics = Graphics.FromImage((Image) b))
      {
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.DrawImage(Image.FromFile(this.openFileDialog1.FileName), new Rectangle(4, 0, 70, 88), new Rectangle(0, 0, 70, 88), GraphicsUnit.Pixel);
      }
      Graphic.Tiles8x8 tiles8x8 = new Graphic.Tiles8x8(b, false);
      List<Color> colorList = new List<Color>();
      foreach (Graphic.Tile8x8 allTile in tiles8x8.AllTiles)
        colorList.Add(Graphic.GeneratePalette(allTile.ToBitmap(), 1, false, false)[0]);
      Color[] palette = Graphic.GeneratePalette(colorList.ToArray(), 7, false);
      int num = 0;
      foreach (Color a in colorList)
      {
        this.selectedpalettes[num % 10, num / 10] = (byte) Graphic.NearestColor(a, palette);
        ++num;
      }
      this.pictureBox2.Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (CoursePictureGenerator));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripButton2 = new ToolStripButton();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripButton4 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton5 = new ToolStripButton();
      this.pictureBox1 = new PictureBox();
      this.openFileDialog1 = new OpenFileDialog();
      this.saveFileDialog1 = new SaveFileDialog();
      this.saveFileDialog2 = new SaveFileDialog();
      this.saveFileDialog3 = new SaveFileDialog();
      this.panel1 = new Panel();
      this.button1 = new Button();
      this.pictureBox2 = new PictureBox();
      this.toolStripStatusLabel1 = new ToolStripStatusLabel();
      this.toolStripProgressBar1 = new ToolStripProgressBar();
      this.statusStrip1 = new StatusStrip();
      this.toolStripButton6 = new ToolStripButton();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStrip1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.panel1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox2).BeginInit();
      this.statusStrip1.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripButton2,
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripButton4,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton5,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton6
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(759, 25);
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
      this.toolStripButton2.Enabled = false;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(23, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.Click += new EventHandler(this.toolStripButton2_Click);
      this.toolStripButton3.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Enabled = false;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripButton4.Alignment = ToolStripItemAlignment.Right;
      this.toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton4.Enabled = false;
      this.toolStripButton4.Image = (Image) componentResourceManager.GetObject("toolStripButton4.Image");
      this.toolStripButton4.ImageTransparentColor = Color.Magenta;
      this.toolStripButton4.Name = "toolStripButton4";
      this.toolStripButton4.Size = new Size(23, 22);
      this.toolStripButton4.Text = "toolStripButton4";
      this.toolStripButton4.Click += new EventHandler(this.toolStripButton4_Click);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new Size(6, 25);
      this.toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton5.Enabled = false;
      this.toolStripButton5.Image = (Image) componentResourceManager.GetObject("toolStripButton5.Image");
      this.toolStripButton5.ImageTransparentColor = Color.Magenta;
      this.toolStripButton5.Name = "toolStripButton5";
      this.toolStripButton5.Size = new Size(23, 22);
      this.toolStripButton5.Text = "Generate";
      this.toolStripButton5.Click += new EventHandler(this.toolStripButton5_Click);
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 25);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(759, 307);
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      this.openFileDialog1.FileName = "openFileDialog1";
      this.saveFileDialog1.DefaultExt = "nclr";
      this.saveFileDialog1.Filter = "Nitro Color Palette for Runtime(*.nclr)|*.nclr";
      this.saveFileDialog2.DefaultExt = "ncgr";
      this.saveFileDialog2.Filter = "Nitro Character Graphics for Runtime(*.ncgr)|*.ncgr";
      this.saveFileDialog3.DefaultExt = "nscr";
      this.saveFileDialog3.Filter = "Nitro Screen for Runtime(*.nscr)|*.nscr";
      this.panel1.Controls.Add((Control) this.button1);
      this.panel1.Controls.Add((Control) this.pictureBox2);
      this.panel1.Dock = DockStyle.Right;
      this.panel1.Location = new Point(550, 25);
      this.panel1.Name = "panel1";
      this.panel1.Size = new Size(209, 285);
      this.panel1.TabIndex = 3;
      this.button1.Enabled = false;
      this.button1.Image = (Image) componentResourceManager.GetObject("button1.Image");
      this.button1.Location = new Point(182, 182);
      this.button1.Name = "button1";
      this.button1.Size = new Size(24, 24);
      this.button1.TabIndex = 1;
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.pictureBox2.Dock = DockStyle.Top;
      this.pictureBox2.Location = new Point(0, 0);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new Size(209, 176);
      this.pictureBox2.TabIndex = 0;
      this.pictureBox2.TabStop = false;
      this.pictureBox2.Paint += new PaintEventHandler(this.pictureBox2_Paint);
      this.pictureBox2.MouseUp += new MouseEventHandler(this.pictureBox2_MouseUp);
      this.pictureBox2.PreviewKeyDown += new PreviewKeyDownEventHandler(this.pictureBox2_PreviewKeyDown);
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new Size(118, 17);
      this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
      this.toolStripProgressBar1.Name = "toolStripProgressBar1";
      this.toolStripProgressBar1.Size = new Size(100, 16);
      this.statusStrip1.Items.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.toolStripStatusLabel1,
        (ToolStripItem) this.toolStripProgressBar1
      });
      this.statusStrip1.Location = new Point(0, 310);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new Size(759, 22);
      this.statusStrip1.TabIndex = 2;
      this.statusStrip1.Text = "statusStrip1";
      this.toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton6.Enabled = false;
      this.toolStripButton6.Image = (Image) componentResourceManager.GetObject("toolStripButton6.Image");
      this.toolStripButton6.ImageTransparentColor = Color.Magenta;
      this.toolStripButton6.Name = "toolStripButton6";
      this.toolStripButton6.Size = new Size(23, 22);
      this.toolStripButton6.Text = "Calculate Palettes";
      this.toolStripButton6.Click += new EventHandler(this.toolStripButton6_Click);
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(759, 332);
      this.Controls.Add((Control) this.panel1);
      this.Controls.Add((Control) this.statusStrip1);
      this.Controls.Add((Control) this.pictureBox1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (CoursePictureGenerator);
      this.Text = nameof (CoursePictureGenerator);
      this.KeyDown += new KeyEventHandler(this.CoursePictureGenerator_KeyDown);
      this.KeyPress += new KeyPressEventHandler(this.CoursePictureGenerator_KeyPress);
      this.KeyUp += new KeyEventHandler(this.CoursePictureGenerator_KeyUp);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.panel1.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox2).EndInit();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
