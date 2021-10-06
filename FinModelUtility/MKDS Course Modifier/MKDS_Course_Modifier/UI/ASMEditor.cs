// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.ASMEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using FastColoredTextBoxNS;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class ASMEditor : Form
  {
    private IContainer components = (IContainer) null;
    private TextStyle Function = new TextStyle((Brush) new SolidBrush(Color.FromArgb(43, 145, 175)), (Brush) null, FontStyle.Bold);
    private TextStyle Register = new TextStyle((Brush) new SolidBrush(Color.Blue), (Brush) null, FontStyle.Regular);
    private TextStyle Comment = new TextStyle((Brush) new SolidBrush(Color.FromArgb(0, 128, 0)), (Brush) null, FontStyle.Regular);
    private TextStyle Keyword = new TextStyle((Brush) new SolidBrush(Color.Purple), (Brush) null, FontStyle.Regular);
    private TextStyle Number = new TextStyle((Brush) new SolidBrush(Color.Blue), (Brush) null, FontStyle.Regular);
    private TextStyle TextString = new TextStyle((Brush) new SolidBrush(Color.FromArgb(163, 21, 21)), (Brush) null, FontStyle.Regular);
    private string[] Registers = new string[18]
    {
      "r0",
      "r1",
      "r2",
      "r3",
      "r4",
      "r5",
      "r6",
      "r7",
      "r8",
      "r9",
      "r10",
      "r11",
      "r12",
      "r13",
      "r14",
      "r15",
      "sp",
      "lr"
    };
    private FastColoredTextBox fastColoredTextBox1;
    private ToolStrip toolStrip1;
    private MainMenu mainMenu1;
    private MenuItem menuItem1;
    private MenuItem menuItem2;
    private MenuItem menuItem3;
    private MenuItem menuItem4;
    private OpenFileDialog openFileDialog1;
    private string Text;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.fastColoredTextBox1 = new FastColoredTextBox();
      this.toolStrip1 = new ToolStrip();
      this.mainMenu1 = new MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.openFileDialog1 = new OpenFileDialog();
      ((ISupportInitialize) this.fastColoredTextBox1).BeginInit();
      this.SuspendLayout();
      ((ScrollableControl) this.fastColoredTextBox1).AutoScrollMinSize = new Size(179, 14);
      this.fastColoredTextBox1.BackBrush = (Brush) null;
      this.fastColoredTextBox1.Cursor = Cursors.IBeam;
      this.fastColoredTextBox1.DisabledColor = Color.FromArgb(100, 180, 180, 180);
      this.fastColoredTextBox1.Dock = DockStyle.Fill;
      this.fastColoredTextBox1.IsReplaceMode = false;
      this.fastColoredTextBox1.Location = new Point(0, 25);
      this.fastColoredTextBox1.Name = "fastColoredTextBox1";
      this.fastColoredTextBox1.Paddings = new Padding(0);
      this.fastColoredTextBox1.SelectionColor = Color.FromArgb(50, 0, 0, (int) byte.MaxValue);
      this.fastColoredTextBox1.Size = new Size(422, 273);
      this.fastColoredTextBox1.TabIndex = 0;
      this.fastColoredTextBox1.Text = "fastColoredTextBox1";
      this.fastColoredTextBox1.TextChanged += new EventHandler<TextChangedEventArgs>(this.fastColoredTextBox1_TextChanged);
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(422, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.mainMenu1.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem1
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem2,
        this.menuItem3,
        this.menuItem4
      });
      this.menuItem1.Text = "File";
      this.menuItem2.Index = 0;
      this.menuItem2.Text = "New";
      this.menuItem3.Index = 1;
      this.menuItem3.Text = "Open";
      this.menuItem3.Click += new EventHandler(this.menuItem3_Click);
      this.menuItem4.Index = 2;
      this.menuItem4.Text = "Save";
      this.openFileDialog1.DefaultExt = "s";
      this.openFileDialog1.FileName = "openFileDialog1";
      this.openFileDialog1.Filter = "ASM Files(*.s)|*.s";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(422, 298);
      this.Controls.Add((Control) this.fastColoredTextBox1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (ASMEditor);
      this.Load += new EventHandler(this.ASMEditor_Load);
      ((ISupportInitialize) this.fastColoredTextBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public ASMEditor()
    {
      this.InitializeComponent();
    }

    private void ASMEditor_Load(object sender, EventArgs e)
    {
    }

    private void menuItem3_Click(object sender, EventArgs e)
    {
      if (this.openFileDialog1.ShowDialog() != DialogResult.OK || this.openFileDialog1.FileName.Length <= 0)
        return;
      this.fastColoredTextBox1.Text = File.ReadAllText(this.openFileDialog1.FileName);
    }

    public void HighLight()
    {
      this.fastColoredTextBox1.ClearStyle(StyleIndex.All);
      this.fastColoredTextBox1.Range.SetStyle((Style) this.Comment, "@.*$", RegexOptions.Multiline | RegexOptions.Compiled);
      this.fastColoredTextBox1.Range.SetStyle((Style) this.Comment, "(/\\*.*?\\*/)|(.*\\*/)", RegexOptions.Compiled | RegexOptions.Singleline);
      this.fastColoredTextBox1.Range.SetStyle((Style) this.Keyword, "\\x2E\\w*", RegexOptions.Multiline);
      this.fastColoredTextBox1.Range.SetStyle((Style) this.Function, "\\w*\\x3A", RegexOptions.Multiline);
      this.fastColoredTextBox1.Range.SetStyle((Style) this.Number, "\\x23\\x30\\x78\\d*|\\x23\\d*", RegexOptions.Multiline);
      this.fastColoredTextBox1.Range.SetStyle((Style) this.TextString, "\\x22.*\\x22", RegexOptions.Multiline);
    }

    private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.HighLight();
    }
  }
}
