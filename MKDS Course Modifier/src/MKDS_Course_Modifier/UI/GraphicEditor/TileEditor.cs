// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.GraphicEditor.TileEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.GraphicEditor
{
  public class TileEditor : Form, IGraphicEditorMDIWindow
  {
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.UI.MainMenu mainMenu1;
    private MenuItem menuItem1;
    private MenuItem menuItem2;

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
      this.SuspendLayout();
      this.mainMenu1.MenuItems.AddRange(new MenuItem[2]
      {
        this.menuItem1,
        this.menuItem2
      });
      this.menuItem1.Index = 0;
      this.menuItem1.MergeOrder = 1;
      this.menuItem1.Text = "Edit";
      this.menuItem2.Index = 1;
      this.menuItem2.MergeOrder = 3;
      this.menuItem2.Text = "Tools";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(549, 366);
      this.Menu = (System.Windows.Forms.MainMenu) this.mainMenu1;
      this.Name = nameof (TileEditor);
      this.Text = nameof (TileEditor);
      this.Load += new EventHandler(this.TileEditor_Load);
      this.ResumeLayout(false);
    }

    public TileEditor()
    {
      this.InitializeComponent();
    }

    private void TileEditor_Load(object sender, EventArgs e)
    {
    }

    public string GetExportFilter()
    {
      return "Portable Network Graphics (*.png)|*.png";
    }

    public void Export(string FilePath, string Filter)
    {
      switch (Filter)
      {
      }
    }
  }
}
