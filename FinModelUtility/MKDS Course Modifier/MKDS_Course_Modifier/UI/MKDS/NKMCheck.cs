// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.MKDS.NKMCheck
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.MKDS
{
  public class NKMCheck : Form
  {
    private IContainer components = (IContainer) null;
    private List<NKMCheck.Error> Errors = new List<NKMCheck.Error>();
    private ListView listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader3;
    private ColumnHeader columnHeader4;
    private ColumnHeader columnHeader5;
    private ImageList imageList1;
    private NKM Owner;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.listView1 = new ListView();
      this.columnHeader1 = new ColumnHeader();
      this.columnHeader3 = new ColumnHeader();
      this.columnHeader4 = new ColumnHeader();
      this.columnHeader5 = new ColumnHeader();
      this.imageList1 = new ImageList(this.components);
      this.SuspendLayout();
      this.listView1.Columns.AddRange(new ColumnHeader[4]
      {
        this.columnHeader1,
        this.columnHeader3,
        this.columnHeader4,
        this.columnHeader5
      });
      this.listView1.Dock = DockStyle.Fill;
      this.listView1.FullRowSelect = true;
      this.listView1.GridLines = true;
      this.listView1.HideSelection = false;
      this.listView1.Location = new Point(0, 0);
      this.listView1.MultiSelect = false;
      this.listView1.Name = "listView1";
      this.listView1.Size = new Size(564, 231);
      this.listView1.SmallImageList = this.imageList1;
      this.listView1.TabIndex = 0;
      this.listView1.UseCompatibleStateImageBehavior = false;
      this.listView1.View = View.Details;
      this.listView1.ItemActivate += new EventHandler(this.listView1_ItemActivate);
      this.columnHeader1.Text = "";
      this.columnHeader1.Width = 25;
      this.columnHeader3.Text = "Description";
      this.columnHeader3.Width = 445;
      this.columnHeader4.Text = "Section";
      this.columnHeader4.Width = 50;
      this.columnHeader5.Text = "Index";
      this.columnHeader5.Width = 40;
      this.imageList1.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList1.ImageSize = new Size(16, 16);
      this.imageList1.TransparentColor = Color.Transparent;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(564, 231);
      this.Controls.Add((Control) this.listView1);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (NKMCheck);
      this.Text = "Checking Results";
      this.TopMost = true;
      this.Load += new EventHandler(this.NKMCheck_Load);
      this.ResumeLayout(false);
    }

    public NKMCheck(MKDS_Course_Modifier.MKDS.NKM Nkm, NKM Owner)
    {
      this.Owner = Owner;
      this.InitializeComponent();
      this.imageList1.Images.Add((Image) Resources.cross_circle);
      this.imageList1.Images.Add((Image) Resources.exclamation);
      this.imageList1.Images.Add((Image) Resources.information_white);
      int Index = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.OBJIEntry objiEntry in Nkm.OBJI)
      {
        if ((long) objiEntry.RouteID > (long) (Nkm.PATH.NrEntries - 1U))
        {
          this.Errors.Add(new NKMCheck.Error(0, Index, LanguageHandler.GetString("nkm.check.objirouteid"), NKMCheck.Error.ErrorType.Error));
          this.listView1.Items.Add((ListViewItem) this.Errors.Last<NKMCheck.Error>());
        }
        if (objiEntry.ObjectID == (ushort) 101 && objiEntry.TimeTrails || objiEntry.ObjectID == (ushort) 201 && objiEntry.TimeTrails)
        {
          this.Errors.Add(new NKMCheck.Error(0, Index, LanguageHandler.GetString("nkm.check.objiiboxtt"), NKMCheck.Error.ErrorType.Warning));
          this.listView1.Items.Add((ListViewItem) this.Errors.Last<NKMCheck.Error>());
        }
        ObjectDb.Object @object = MKDS_Const.ObjectDatabase.GetObject(objiEntry.ObjectID);
        if (@object != null && @object.RouteRequired && objiEntry.RouteID == (short) -1)
        {
          this.Errors.Add(new NKMCheck.Error(0, Index, LanguageHandler.GetString("nkm.check.objineedroute"), NKMCheck.Error.ErrorType.Error));
          this.listView1.Items.Add((ListViewItem) this.Errors.Last<NKMCheck.Error>());
        }
        ++Index;
      }
      int num = 0;
      foreach (MKDS_Course_Modifier.MKDS.NKM.PATHEntry pathEntry in Nkm.PATH)
        num += (int) pathEntry.NrPoit;
      if ((long) Nkm.POIT.NrEntries != (long) num)
      {
        this.Errors.Add(new NKMCheck.Error(2, -1, LanguageHandler.GetString("nkm.check.pathpoit"), NKMCheck.Error.ErrorType.Error));
        this.listView1.Items.Add((ListViewItem) this.Errors.Last<NKMCheck.Error>());
      }
      if (Nkm.KTPS.Entries.Count != 0 && (Nkm.KTPS.Entries.Count <= 1 || Nkm.KTPS.Entries.Count >= 8))
        return;
      this.Errors.Add(new NKMCheck.Error(4, -1, LanguageHandler.GetString("nkm.check.ktps"), NKMCheck.Error.ErrorType.Warning));
      this.listView1.Items.Add((ListViewItem) this.Errors.Last<NKMCheck.Error>());
    }

    private void NKMCheck_Load(object sender, EventArgs e)
    {
    }

    private void listView1_ItemActivate(object sender, EventArgs e)
    {
      if (this.Errors[this.listView1.SelectedIndices[0]].Index != -1)
        this.Owner.SelectObject(this.Owner.SelType = this.Errors[this.listView1.SelectedIndices[0]].Section, this.Owner.SelIdx = this.Errors[this.listView1.SelectedIndices[0]].Index);
      this.Owner.tabControl1.SelectTab(this.Errors[this.listView1.SelectedIndices[0]].Section + 1);
    }

    private class Error
    {
      private static string[] SectionName = new string[19]
      {
        "OBJI",
        "PATH",
        "POIT",
        "STAG",
        "KTPS",
        "KTPJ",
        "KTP2",
        "KTPC",
        "KTPM",
        "CPOI",
        "CPAT",
        "IPOI",
        "IPAT",
        "EPOI",
        "EPAT",
        "MEPO",
        "MEPA",
        "AREA",
        "CAME"
      };

      public Error(int Section, int Index, string Description, NKMCheck.Error.ErrorType Type)
      {
        this.Section = Section;
        this.Index = Index;
        this.Description = Description;
        this.Type = Type;
      }

      public int Section { get; private set; }

      public int Index { get; private set; }

      public string Description { get; private set; }

      public NKMCheck.Error.ErrorType Type { get; private set; }

      public static implicit operator ListViewItem(NKMCheck.Error e)
      {
        return new ListViewItem(new string[4]
        {
          "",
          e.Description,
          NKMCheck.Error.SectionName[e.Section],
          e.Index == -1 ? "All" : e.Index.ToString()
        }, (int) e.Type);
      }

      public enum ErrorType
      {
        Error,
        Warning,
        Message,
      }
    }
  }
}
