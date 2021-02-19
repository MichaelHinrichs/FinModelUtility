// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NSBTX
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.IO;
using MKDS_Course_Modifier.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class NSBTX : Form
  {
    public byte[] Before = (byte[]) null;
    private int[] Powers = new int[8]
    {
      8,
      16,
      32,
      64,
      128,
      256,
      512,
      1024
    };
    private IContainer components = (IContainer) null;
    public MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX Btx;
    public MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet Tex;
    private ToolStrip toolStrip1;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private PictureBox pictureBox1;
    private ListBox listBox1;
    private ListBox listBox2;
    private ToolStripButton toolStripButton1;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripButton toolStripButton3;
    private ToolStripSeparator toolStripSeparator2;
    private ToolStripSplitButton toolStripButton2;
    private OpenFileDialog openFileDialog1;
    private SaveFileDialog saveFileDialog1;

    public NSBTX(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX Btx)
    {
      this.Btx = Btx;
      this.Tex = Btx.TexPlttSet;
      this.InitializeComponent();
      this.toolStripButton1.Text = LanguageHandler.GetString("base.save");
      this.toolStripButton3.Text = LanguageHandler.GetString("base.export");
      this.toolStripButton2.Text = LanguageHandler.GetString("base.import");
    }

    public NSBTX(MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet Tex, byte[] Before)
    {
      this.Tex = Tex;
      this.Before = Before;
      this.InitializeComponent();
    }

    private void NSBTX_Load(object sender, EventArgs e)
    {
      for (int index = 0; index < (int) this.Tex.dictTex.numEntry; ++index)
        this.listBox1.Items.Add((object) this.Tex.dictTex[index].Key);
      for (int index = 0; index < (int) this.Tex.dictPltt.numEntry; ++index)
        this.listBox2.Items.Add((object) this.Tex.dictPltt[index].Key);
      this.listBox1.SelectedIndex = 0;
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      KeyValuePair<string, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData> keyValuePair;
      if (this.listBox1.SelectedIndex == -1)
      {
        this.listBox1.SelectedIndex = 0;
      }
      else
      {
        int num;
        if (this.listBox2.SelectedIndex == -1)
        {
          keyValuePair = this.Tex.dictTex[this.listBox1.SelectedIndex];
          num = keyValuePair.Value.Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT ? 1 : 0;
        }
        else
          num = 1;
        if (num == 0)
        {
          this.listBox2.SelectedIndex = 0;
        }
        else
        {
          keyValuePair = this.Tex.dictTex[this.listBox1.SelectedIndex];
          if (keyValuePair.Value.Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
            this.listBox2.SelectedIndex = -1;
        }
      }
      this.toolStripButton2.Enabled = true;
      try
      {
        PictureBox pictureBox1 = this.pictureBox1;
        keyValuePair = this.Tex.dictTex[this.listBox1.SelectedIndex];
        MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData dictTexData = keyValuePair.Value;
        keyValuePair = this.Tex.dictTex[this.listBox1.SelectedIndex];
        MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData Palette = keyValuePair.Value.Fmt != Graphic.GXTexFmt.GX_TEXFMT_DIRECT ? this.Tex.dictPltt[this.listBox2.SelectedIndex].Value : (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData) null;
        Bitmap bitmap = dictTexData.ToBitmap(Palette);
        pictureBox1.Image = (Image) bitmap;
      }
      catch
      {
        this.pictureBox1.Image = (Image) null;
      }
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      byte[] Data;
      if (this.Before != null)
      {
        MemoryStream memoryStream = new MemoryStream();
        EndianBinaryWriter er = new EndianBinaryWriter((Stream) memoryStream, Endianness.LittleEndian);
        er.Write(this.Before, 0, this.Before.Length);
        this.Tex.Write(er);
        er.BaseStream.Position = 8L;
        er.Write((int) er.BaseStream.Length);
        byte[] array = memoryStream.ToArray();
        er.Close();
        Data = array;
      }
      else
      {
        this.Btx.TexPlttSet = this.Tex;
        Data = this.Btx.Write();
      }
      FileHandler.Save(Data, 0, false);
    }

    private void toolStripButton2_ButtonClick(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      Bitmap bitmap1 = (Bitmap) Image.FromFile(this.openFileDialog1.FileName);
      if (!((IEnumerable<int>) this.Powers).Contains<int>(bitmap1.Width) || !((IEnumerable<int>) this.Powers).Contains<int>(bitmap1.Height))
      {
        bitmap1.Dispose();
        int num = (int) MessageBox.Show("Your image's width and/or height is not one of these:\r\n8, 16, 32, 64, 128, 256, 512, 1024", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else
      {
        KeyValuePair<string, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData> keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
        if (keyValuePair1.Value.Fmt != Graphic.GXTexFmt.GX_TEXFMT_COMP4x4)
        {
          Bitmap b = bitmap1;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          ref byte[] local1 = ref keyValuePair1.Value.Data;
          byte[] numArray;
          ref byte[] local2 = ref numArray;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          int fmt = (int) keyValuePair1.Value.Fmt;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          ref bool local3 = ref keyValuePair1.Value.TransparentColor;
          Graphic.ConvertBitmap(b, out local1, out local2, (Graphic.GXTexFmt) fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, out local3, false);
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          keyValuePair1.Value.S = (ushort) bitmap1.Width;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          keyValuePair1.Value.T = (ushort) bitmap1.Height;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          KeyValuePair<string, MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData> keyValuePair2;
          if (keyValuePair1.Value.Fmt != Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
          {
            keyValuePair2 = this.Tex.dictPltt[this.listBox2.SelectedIndex];
            keyValuePair2.Value.Data = numArray;
          }
          PictureBox pictureBox1 = this.pictureBox1;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictTexData dictTexData = keyValuePair1.Value;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData Palette;
          if (keyValuePair1.Value.Fmt == Graphic.GXTexFmt.GX_TEXFMT_DIRECT)
          {
            Palette = (MKDS_Course_Modifier.G3D_Binary_File_Format.NSBTX.TexplttSet.DictPlttData) null;
          }
          else
          {
            keyValuePair2 = this.Tex.dictPltt[this.listBox2.SelectedIndex];
            Palette = keyValuePair2.Value;
          }
          Bitmap bitmap2 = dictTexData.ToBitmap(Palette);
          pictureBox1.Image = (Image) bitmap2;
        }
        else
        {
          if (bitmap1.Width == 1024 && bitmap1.Height == 1024)
          {
            bitmap1.Dispose();
            int num = (int) MessageBox.Show("Your image's size is 1024x1024. This is not supported by the current texture type. The maximum sizes are: 1024x512 and 512x1024.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
          }
          Bitmap b = bitmap1;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          ref byte[] local1 = ref keyValuePair1.Value.Data;
          ref byte[] local2 = ref this.Tex.dictPltt[this.listBox2.SelectedIndex].Value.Data;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          ref byte[] local3 = ref keyValuePair1.Value.Data4x4;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          int fmt = (int) keyValuePair1.Value.Fmt;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          ref bool local4 = ref keyValuePair1.Value.TransparentColor;
          Graphic.ConvertBitmap(b, out local1, out local2, out local3, (Graphic.GXTexFmt) fmt, Graphic.NNSG2dCharacterFmt.NNS_G2D_CHARACTER_FMT_BMP, out local4, false);
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          keyValuePair1.Value.S = (ushort) bitmap1.Width;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          keyValuePair1.Value.T = (ushort) bitmap1.Height;
          PictureBox pictureBox1 = this.pictureBox1;
          keyValuePair1 = this.Tex.dictTex[this.listBox1.SelectedIndex];
          Bitmap bitmap2 = keyValuePair1.Value.ToBitmap(this.Tex.dictPltt[this.listBox2.SelectedIndex].Value);
          pictureBox1.Image = (Image) bitmap2;
        }
        bitmap1.Dispose();
      }
    }

    private void toolStripButton3_Click(object sender, EventArgs e)
    {
      if (this.saveFileDialog1.ShowDialog() != DialogResult.OK || this.saveFileDialog1.FileName.Length <= 0)
        return;
      this.pictureBox1.Image.Save(this.saveFileDialog1.FileName, ImageFormat.Png);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (NSBTX));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStripSeparator1 = new ToolStripSeparator();
      this.toolStripButton3 = new ToolStripButton();
      this.toolStripSeparator2 = new ToolStripSeparator();
      this.toolStripButton2 = new ToolStripSplitButton();
      this.splitContainer1 = new SplitContainer();
      this.splitContainer2 = new SplitContainer();
      this.listBox1 = new ListBox();
      this.listBox2 = new ListBox();
      this.pictureBox1 = new PictureBox();
      this.openFileDialog1 = new OpenFileDialog();
      this.saveFileDialog1 = new SaveFileDialog();
      this.toolStrip1.SuspendLayout();
      this.splitContainer1.BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.splitContainer2.BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.toolStripButton1,
        (ToolStripItem) this.toolStripSeparator1,
        (ToolStripItem) this.toolStripButton3,
        (ToolStripItem) this.toolStripSeparator2,
        (ToolStripItem) this.toolStripButton2
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(532, 25);
      this.toolStrip1.TabIndex = 0;
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
      this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = (Image) componentResourceManager.GetObject("toolStripButton3.Image");
      this.toolStripButton3.ImageTransparentColor = Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new Size(23, 22);
      this.toolStripButton3.Text = "toolStripButton3";
      this.toolStripButton3.Click += new EventHandler(this.toolStripButton3_Click);
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new Size(6, 25);
      this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Enabled = false;
      this.toolStripButton2.Image = (Image) componentResourceManager.GetObject("toolStripButton2.Image");
      this.toolStripButton2.ImageTransparentColor = Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new Size(32, 22);
      this.toolStripButton2.Text = "toolStripButton2";
      this.toolStripButton2.ButtonClick += new EventHandler(this.toolStripButton2_ButtonClick);
      this.splitContainer1.Dock = DockStyle.Fill;
      this.splitContainer1.FixedPanel = FixedPanel.Panel1;
      this.splitContainer1.Location = new Point(0, 25);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Panel1.Controls.Add((Control) this.splitContainer2);
      this.splitContainer1.Panel2.Controls.Add((Control) this.pictureBox1);
      this.splitContainer1.Size = new Size(532, 355);
      this.splitContainer1.SplitterDistance = 146;
      this.splitContainer1.TabIndex = 1;
      this.splitContainer2.Dock = DockStyle.Fill;
      this.splitContainer2.Location = new Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = Orientation.Horizontal;
      this.splitContainer2.Panel1.Controls.Add((Control) this.listBox1);
      this.splitContainer2.Panel2.Controls.Add((Control) this.listBox2);
      this.splitContainer2.Size = new Size(146, 355);
      this.splitContainer2.SplitterDistance = 177;
      this.splitContainer2.TabIndex = 0;
      this.listBox1.Dock = DockStyle.Fill;
      this.listBox1.FormattingEnabled = true;
      this.listBox1.Location = new Point(0, 0);
      this.listBox1.Name = "listBox1";
      this.listBox1.Size = new Size(146, 177);
      this.listBox1.TabIndex = 0;
      this.listBox1.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
      this.listBox2.Dock = DockStyle.Fill;
      this.listBox2.FormattingEnabled = true;
      this.listBox2.Location = new Point(0, 0);
      this.listBox2.Name = "listBox2";
      this.listBox2.Size = new Size(146, 174);
      this.listBox2.TabIndex = 0;
      this.listBox2.SelectedIndexChanged += new EventHandler(this.listBox1_SelectedIndexChanged);
      this.pictureBox1.BackColor = Color.White;
      this.pictureBox1.BackgroundImage = (Image) componentResourceManager.GetObject("pictureBox1.BackgroundImage");
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(382, 355);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "Pictures (*.jpg; *.jpeg; *.gif; *.png; *.tiff)|*.jpg; *.jpeg; *.gif; *.png; *.tiff";
      this.saveFileDialog1.Filter = "PNG Files (*.png)|*.png";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(532, 380);
      this.Controls.Add((Control) this.splitContainer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (NSBTX);
      this.Text = nameof (NSBTX);
      this.Load += new EventHandler(this.NSBTX_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.EndInit();
      this.splitContainer2.ResumeLayout(false);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
