// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.GraphicEditor.GraphicEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.GraphicEditor
{
  public class GraphicEditor : Form
  {
    private IContainer components = (IContainer) null;
    private PaletteEditor PaletteE;
    private TileEditor TileE;
    private MKDS_Course_Modifier.UI.MainMenu mainMenu1;
    private MenuItem menuItem1;
    private MenuItem menuItem4;
    private MenuItem menuItem5;
    private MenuItem menuItem2;
    private MenuItem menuItem3;
    private MenuItem menuItem6;

    public GraphicEditor()
    {
      this.InitializeComponent();
    }

    private void GraphicEditor_Load(object sender, EventArgs e)
    {
      this.PaletteE = new PaletteEditor();
      this.PaletteE.MdiParent = (Form) this;
      this.PaletteE.Show();
      this.TileE = new TileEditor();
      this.TileE.MdiParent = (Form) this;
      this.TileE.Show();
    }

    private void menuItem6_Click(object sender, EventArgs e)
    {
      int num = (int) MessageBox.Show(((IGraphicEditorMDIWindow) this.ActiveMdiChild).GetExportFilter());
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
      this.mainMenu1 = new MKDS_Course_Modifier.UI.MainMenu(this.components);
      this.menuItem1 = new MenuItem();
      this.menuItem2 = new MenuItem();
      this.menuItem3 = new MenuItem();
      this.menuItem4 = new MenuItem();
      this.menuItem5 = new MenuItem();
      this.menuItem6 = new MenuItem();
      this.SuspendLayout();
      this.mainMenu1.MenuItems.AddRange(new MenuItem[4]
      {
        this.menuItem1,
        this.menuItem3,
        this.menuItem4,
        this.menuItem5
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem2,
        this.menuItem6
      });
      this.menuItem1.Text = "File";
      this.menuItem2.Index = 0;
      this.menuItem2.Text = "New";
      this.menuItem3.Index = 1;
      this.menuItem3.MergeOrder = 2;
      this.menuItem3.Text = "View";
      this.menuItem4.Index = 2;
      this.menuItem4.MdiList = true;
      this.menuItem4.MergeOrder = 4;
      this.menuItem4.Text = "Window";
      this.menuItem5.Index = 3;
      this.menuItem5.MergeOrder = 5;
      this.menuItem5.Text = "Help";
      this.menuItem6.Index = 1;
      this.menuItem6.Text = "Export";
      this.menuItem6.Click += new EventHandler(this.menuItem6_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(650, 426);
      this.IsMdiContainer = true;
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (GraphicEditor);
      this.Text = nameof (GraphicEditor);
      this.Load += new EventHandler(this.GraphicEditor_Load);
      this.ResumeLayout(false);
    }
  }
}
