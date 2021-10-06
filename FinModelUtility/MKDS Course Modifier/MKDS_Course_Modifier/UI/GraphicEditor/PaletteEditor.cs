// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.GraphicEditor.PaletteEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.GraphicEditor
{
  public class PaletteEditor : Form, IGraphicEditorMDIWindow
  {
    public readonly List<Color> Colors = new List<Color>();
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.UI.MainMenu mainMenu1;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private MenuItem menuItem1;
    private MenuItem menuItem3;
    private MenuItem menuItem4;
    private MenuItem menuItem5;
    private MenuItem menuItem2;
    private MenuItem menuItem6;

    public PaletteEditor()
    {
      this.InitializeComponent();
    }

    private void PaletteEditor_Load(object sender, EventArgs e)
    {
    }

    public string GetExportFilter()
    {
      return "PAL Palette (*.pal)|*.pal";
    }

    public void Export(string FilePath, string Filter)
    {
      switch (Filter)
      {
      }
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
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (PaletteEditor));
      this.mainMenu1 = new MKDS_Course_Modifier.UI.MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.menuItem5 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.mainMenu1.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem1,
        this.menuItem2
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[3]
      {
        this.menuItem3,
        this.menuItem4,
        this.menuItem5
      });
      this.menuItem1.MergeOrder = 1;
      this.menuItem1.Text = "Edit";
      this.menuItem3.Index = 0;
      this.menuItem3.Text = "Cut";
      this.menuItem4.Index = 1;
      this.menuItem4.Text = "Copy";
      this.menuItem5.Index = 2;
      this.menuItem5.Text = "Paste";
      this.menuItem2.Index = 1;
      this.menuItem2.MenuItems.AddRange(new MenuItem[1]
      {
        this.menuItem6
      });
      this.menuItem2.MergeOrder = 3;
      this.menuItem2.Text = "Tools";
      this.menuItem6.Index = 0;
      this.menuItem6.Text = "Do something";
      this.toolStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripButton1
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(434, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(434, 284);
      this.Controls.Add((Control) this.toolStrip1);
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (PaletteEditor);
      this.ShowInTaskbar = false;
      this.Text = nameof (PaletteEditor);
      this.Load += new EventHandler(this.PaletteEditor_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
