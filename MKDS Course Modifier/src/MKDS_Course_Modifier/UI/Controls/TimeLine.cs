// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.Controls.TimeLine
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI.Controls
{
  public class TimeLine : UserControl
  {
    private readonly ObservableCollection<TimeLine.Group> groups = new ObservableCollection<TimeLine.Group>();
    private int curFrame = 0;
    private int nrFrames = 100;
    private IContainer components = (IContainer) null;
    public TimeLine.AdobeToolStripRender ToolstripRender;
    private dataGridViewNF dataGridView1;
    private ToolStrip toolStrip1;

    public ObservableCollection<TimeLine.Group> Groups
    {
      get
      {
        return this.groups;
      }
    }

    [DisplayName("Current Frame")]
    public uint CurFrame
    {
      get
      {
        return (uint) this.curFrame;
      }
      set
      {
        this.curFrame = (int) value;
        this.ToolstripRender.FrameNr = (int) value + 1;
        this.toolStrip1.Refresh();
        this.dataGridView1.Refresh();
      }
    }

    public uint NrFrames
    {
      get
      {
        return (uint) this.nrFrames;
      }
      set
      {
        if (value < 1U)
        {
          this.NrFrames = 1U;
        }
        else
        {
          this.nrFrames = (int) value;
          this.ToolstripRender.NrFrames = this.nrFrames;
          this.toolStrip1.Refresh();
          this.dataGridView1.SuspendDrawing();
          this.dataGridView1.Columns.Clear();
          for (int index = 0; (long) index < (long) this.NrFrames; ++index)
          {
            this.dataGridView1.Columns.Add((DataGridViewColumn) new TimeLine.DataGridViewFlashFrameColumn());
            this.dataGridView1.Columns[index].Width = 8;
            this.dataGridView1.Columns[index].FillWeight = 8f;
            this.dataGridView1.Columns[index].ReadOnly = true;
          }
          int index1 = 0;
          foreach (TimeLine.Group group in (Collection<TimeLine.Group>) this.Groups)
          {
            this.dataGridView1.Rows.Add((object[]) (string[]) group);
            this.dataGridView1.Rows[index1].HeaderCell = (DataGridViewRowHeaderCell) new TimeLine.FlashHeaderCell();
            this.dataGridView1.Rows[index1].HeaderCell.Value = (object) group.Name;
            ++index1;
          }
          this.dataGridView1.ResumeDrawing(true);
        }
      }
    }

    public TimeLine()
    {
      this.InitializeComponent();
      this.ToolstripRender = new TimeLine.AdobeToolStripRender(100, 0, 1);
      this.toolStrip1.Renderer = (ToolStripRenderer) this.ToolstripRender;
      this.groups.CollectionChanged += new NotifyCollectionChangedEventHandler(this.groups_CollectionChanged);
      for (int index = 0; (long) index < (long) this.NrFrames; ++index)
      {
        this.dataGridView1.Columns.Add((DataGridViewColumn) new TimeLine.DataGridViewFlashFrameColumn());
        this.dataGridView1.Columns[index].Width = 8;
        this.dataGridView1.Columns[index].FillWeight = 8f;
        this.dataGridView1.Columns[index].ReadOnly = true;
      }
      int index1 = 0;
      foreach (TimeLine.Group group in (Collection<TimeLine.Group>) this.Groups)
      {
        this.dataGridView1.Rows.Add((object[]) (string[]) group);
        this.dataGridView1.Rows[index1].HeaderCell = (DataGridViewRowHeaderCell) new TimeLine.FlashHeaderCell();
        this.dataGridView1.Rows[index1].HeaderCell.Value = (object) group.Name;
        ++index1;
      }
    }

    private void groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.dataGridView1.SuspendDrawing();
      this.dataGridView1.Columns.Clear();
      for (int index = 0; (long) index < (long) this.NrFrames; ++index)
      {
        this.dataGridView1.Columns.Add((DataGridViewColumn) new TimeLine.DataGridViewFlashFrameColumn());
        this.dataGridView1.Columns[index].Width = 8;
        this.dataGridView1.Columns[index].FillWeight = 8f;
        this.dataGridView1.Columns[index].ReadOnly = true;
      }
      int index1 = 0;
      foreach (TimeLine.Group group in (Collection<TimeLine.Group>) this.Groups)
      {
        this.dataGridView1.Rows.Add((object[]) (string[]) group);
        this.dataGridView1.Rows[index1].HeaderCell = (DataGridViewRowHeaderCell) new TimeLine.FlashHeaderCell();
        this.dataGridView1.Rows[index1].HeaderCell.Value = (object) group.Name;
        ++index1;
      }
      this.dataGridView1.ResumeDrawing(true);
    }

    private void dataGridView1_Paint(object sender, PaintEventArgs e)
    {
      e.Graphics.DrawLine(new Pen(Color.FromArgb(166, 166, 166)), this.dataGridView1.RowHeadersWidth - 1, 0, this.dataGridView1.RowHeadersWidth - 1, this.dataGridView1.Height);
      if (this.ToolstripRender.StartFrame >= this.ToolstripRender.FrameNr)
        return;
      e.Graphics.DrawLine(new Pen(Color.FromArgb(204, 0, 0)), (float) ((long) (uint) ((int) this.CurFrame * 8 + 3) + (long) this.dataGridView1.RowHeadersWidth - (long) this.dataGridView1.HorizontalScrollingOffset), (float) (this.dataGridView1.RowCount * 18), (float) ((long) (uint) ((int) this.CurFrame * 8 + 3) + (long) this.dataGridView1.RowHeadersWidth - (long) this.dataGridView1.HorizontalScrollingOffset), (float) e.ClipRectangle.Height);
    }

    private void dataGridView1_RowHeadersWidthChanged(object sender, EventArgs e)
    {
      this.toolStrip1.Padding = new Padding(this.dataGridView1.RowHeadersWidth - 1, 0, 0, 0);
    }

    private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
    {
      this.dataGridView1.HorizontalScrollingOffset = e.NewValue / 8 * 8;
      this.dataGridView1.FirstDisplayedScrollingColumnIndex = e.NewValue / 8;
      this.ToolstripRender.StartFrame = this.dataGridView1.FirstDisplayedScrollingColumnIndex;
      this.toolStrip1.Refresh();
    }

    private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
    {
      if ((e.Button & MouseButtons.Left) != MouseButtons.Left)
        return;
      int num = (e.X - this.toolStrip1.Padding.Left) / 8 + this.ToolstripRender.StartFrame;
      if (num < 0)
        num = 0;
      if ((long) num > (long) (this.NrFrames - 1U))
        num = (int) this.NrFrames - 1;
      this.ToolstripRender.FrameNr = num + 1;
      this.CurFrame = (uint) num;
      this.toolStrip1.Refresh();
      this.dataGridView1.Refresh();
      if (this.OnFrameChanged != null)
        this.OnFrameChanged((int) this.CurFrame);
    }

    private void toolStrip1_MouseUp(object sender, MouseEventArgs e)
    {
      if ((e.Button & MouseButtons.Left) != MouseButtons.Left)
        return;
      int num = (e.X - this.toolStrip1.Padding.Left) / 8 + this.ToolstripRender.StartFrame;
      if (num < 0)
        num = 0;
      if ((long) num > (long) (this.NrFrames - 1U))
        num = (int) this.NrFrames - 1;
      this.ToolstripRender.FrameNr = num + 1;
      this.CurFrame = (uint) num;
      this.toolStrip1.Refresh();
      this.dataGridView1.Refresh();
      if (this.OnFrameChanged != null)
        this.OnFrameChanged((int) this.CurFrame);
    }

    private void dataGridView1_SelectionChanged(object sender, EventArgs e)
    {
    }

    private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
    }

    public event TimeLine.OnFrameChangedEventHandler OnFrameChanged;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      DataGridViewCellStyle gridViewCellStyle = new DataGridViewCellStyle();
      this.toolStrip1 = new ToolStrip();
      this.dataGridView1 = new dataGridViewNF();
      ((ISupportInitialize) this.dataGridView1).BeginInit();
      this.SuspendLayout();
      this.toolStrip1.BackColor = Color.FromArgb(214, 214, 214);
      this.toolStrip1.CanOverflow = false;
      this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.MinimumSize = new Size(0, 27);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Padding = new Padding(149, 0, 0, 0);
      this.toolStrip1.RenderMode = ToolStripRenderMode.System;
      this.toolStrip1.Size = new Size(408, 27);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStrip1.MouseMove += new MouseEventHandler(this.toolStrip1_MouseMove);
      this.toolStrip1.MouseUp += new MouseEventHandler(this.toolStrip1_MouseUp);
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeColumns = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.BackgroundColor = Color.FromArgb(229, 229, 229);
      this.dataGridView1.BorderStyle = BorderStyle.None;
      this.dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;
      this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.ColumnHeadersVisible = false;
      this.dataGridView1.Dock = DockStyle.Fill;
      this.dataGridView1.EnableHeadersVisualStyles = false;
      this.dataGridView1.GridColor = Color.FromArgb(166, 166, 166);
      this.dataGridView1.Location = new Point(0, 27);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
      gridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle.BackColor = Color.White;
      gridViewCellStyle.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle.ForeColor = SystemColors.WindowText;
      gridViewCellStyle.Padding = new Padding(15, 0, 0, 0);
      gridViewCellStyle.SelectionBackColor = Color.FromArgb(117, 178, 239);
      gridViewCellStyle.SelectionForeColor = Color.White;
      gridViewCellStyle.WrapMode = DataGridViewTriState.True;
      this.dataGridView1.RowHeadersDefaultCellStyle = gridViewCellStyle;
      this.dataGridView1.RowHeadersWidth = 150;
      this.dataGridView1.RowTemplate.Height = 18;
      this.dataGridView1.ShowCellToolTips = false;
      this.dataGridView1.ShowEditingIcon = false;
      this.dataGridView1.Size = new Size(408, 278);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.RowHeadersWidthChanged += new EventHandler(this.dataGridView1_RowHeadersWidthChanged);
      this.dataGridView1.Scroll += new ScrollEventHandler(this.dataGridView1_Scroll);
      this.dataGridView1.SelectionChanged += new EventHandler(this.dataGridView1_SelectionChanged);
      this.dataGridView1.Paint += new PaintEventHandler(this.dataGridView1_Paint);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.dataGridView1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (TimeLine);
      this.Size = new Size(408, 305);
      ((ISupportInitialize) this.dataGridView1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public class AdobeToolStripRender : ToolStripSystemRenderer
    {
      public AdobeToolStripRender(int NrFrames, int StartFrame, int FrameNr)
      {
        this.NrFrames = NrFrames;
        this.StartFrame = StartFrame;
        this.FrameNr = FrameNr;
      }

      public int FrameNr { get; set; }

      public int NrFrames { get; set; }

      public int StartFrame { get; set; }

      protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
      {
        base.OnRenderToolStripBackground(e);
        e.Graphics.TranslateTransform((float) e.ToolStrip.Padding.Left, 0.0f);
        e.Graphics.DrawLine(new Pen(Color.FromArgb(166, 166, 166)), 0, 0, 0, 27);
        e.Graphics.TranslateTransform(1f, 0.0f);
        e.Graphics.DrawLine(new Pen(Color.FromArgb(166, 166, 166)), 0, 26, e.ToolStrip.Width, 26);
        e.Graphics.DrawLine(new Pen(Color.FromArgb(94, 94, 94)), 0, 0, e.ToolStrip.Width, 0);
        e.Graphics.TranslateTransform((float) (-this.StartFrame * 8), 0.0f);
        if (this.StartFrame < this.FrameNr)
          e.Graphics.FillRectangle((Brush) new SolidBrush(Color.FromArgb((int) byte.MaxValue, 153, 153)), (this.FrameNr - 1) * 8 - 1, 4, 8, 21);
        StringFormat format = new StringFormat();
        format.Alignment = StringAlignment.Near;
        format.LineAlignment = StringAlignment.Center;
        for (int startFrame = this.StartFrame; startFrame < this.NrFrames && startFrame * 8 + 7 - this.StartFrame * 8 <= e.AffectedBounds.Width; ++startFrame)
        {
          e.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), startFrame * 8 + 7, 21, startFrame * 8 + 7, 24);
          if ((startFrame + 1) % 5 == 0 || startFrame == 0)
            e.Graphics.DrawString((startFrame + 1).ToString(), Control.DefaultFont, Brushes.Black, new RectangleF((float) (startFrame * 8 - 3), 0.0f, 30f, 27f), format);
        }
        if (this.StartFrame >= this.FrameNr)
          return;
        e.Graphics.DrawRectangle(new Pen(Color.FromArgb(204, 0, 0)), (this.FrameNr - 1) * 8 - 1, 4, 8, 20);
        e.Graphics.DrawLine(new Pen(Color.FromArgb(204, 0, 0)), (this.FrameNr - 1) * 8 + 3, 25, (this.FrameNr - 1) * 8 + 3, 27);
      }

      protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
      {
        e.Graphics.DrawLine(new Pen(Color.FromArgb(166, 166, 166)), 0, 26, e.ToolStrip.Padding.Left, 26);
        e.Graphics.DrawLine(new Pen(Color.FromArgb(94, 94, 94)), 0, 0, e.ToolStrip.Width, 0);
      }
    }

    public class DataGridViewFlashFrameColumn : DataGridViewColumn
    {
      public DataGridViewFlashFrameColumn()
      {
        this.CellTemplate = (DataGridViewCell) new TimeLine.FlashFrame();
      }
    }

    public class FlashFrame : DataGridViewTextBoxCell
    {
      protected override void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates cellState,
        object value,
        object formattedValue,
        string errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
      {
        if (value == null)
        {
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(51, 153, (int) byte.MaxValue) : ((this.ColumnIndex + 1) % 5 == 0 ? Color.FromArgb(237, 237, 237) : Color.White)), cellBounds.X, cellBounds.Y, 8, 18);
          graphics.DrawLine(new Pen(Color.FromArgb(222, 222, 222)), cellBounds.X + 7, cellBounds.Y, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.DrawLine(new Pen(Color.FromArgb(222, 222, 222)), cellBounds.X, cellBounds.Y + 17, cellBounds.X + 7, cellBounds.Y + 17);
        }
        else if (value == (object) "+")
        {
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(113, 174, 235) : Color.FromArgb(204, 204, 204)), cellBounds.X, cellBounds.Y, 8, 18);
          graphics.DrawLine(Pens.Black, cellBounds.X + 7, cellBounds.Y, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(51, 153, (int) byte.MaxValue) : Color.Black), cellBounds.X, cellBounds.Y + 17, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 2, cellBounds.Y + 10, 3, 5);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 1, cellBounds.Y + 11, 1, 3);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 5, cellBounds.Y + 11, 1, 3);
        }
        else if (value == (object) "+-")
        {
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(113, 174, 235) : Color.FromArgb(204, 204, 204)), cellBounds.X, cellBounds.Y, 8, 18);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(120, 181, 242) : Color.FromArgb(222, 222, 222)), cellBounds.X + 7, cellBounds.Y, cellBounds.X + 7, cellBounds.Y + 1);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(120, 181, 242) : Color.FromArgb(222, 222, 222)), cellBounds.X + 7, cellBounds.Y + 16, cellBounds.X + 7, cellBounds.Y + 15);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(51, 153, (int) byte.MaxValue) : Color.Black), cellBounds.X, cellBounds.Y + 17, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 2, cellBounds.Y + 10, 3, 5);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 1, cellBounds.Y + 11, 1, 3);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 5, cellBounds.Y + 11, 1, 3);
        }
        else if (value == (object) "-")
        {
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(113, 174, 235) : Color.FromArgb(204, 204, 204)), cellBounds.X, cellBounds.Y, 8, 18);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(51, 153, (int) byte.MaxValue) : Color.Black), cellBounds.X, cellBounds.Y + 17, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(120, 181, 242) : Color.FromArgb(222, 222, 222)), cellBounds.X + 7, cellBounds.Y, cellBounds.X + 7, cellBounds.Y + 1);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(120, 181, 242) : Color.FromArgb(222, 222, 222)), cellBounds.X + 7, cellBounds.Y + 16, cellBounds.X + 7, cellBounds.Y + 15);
        }
        else if (value == (object) "-+")
        {
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(113, 174, 235) : Color.FromArgb(204, 204, 204)), cellBounds.X, cellBounds.Y, 8, 18);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(51, 153, (int) byte.MaxValue) : Color.Black), cellBounds.X + 7, cellBounds.Y, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.DrawLine(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(51, 153, (int) byte.MaxValue) : Color.Black), cellBounds.X, cellBounds.Y + 17, cellBounds.X + 7, cellBounds.Y + 17);
          graphics.DrawRectangle(new Pen((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(31, 92, 153) : Color.Black), cellBounds.X + 1, cellBounds.Y + 7, 4, 8);
          graphics.FillRectangle((Brush) new SolidBrush((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected ? Color.FromArgb(133, 194, (int) byte.MaxValue) : Color.White), cellBounds.X + 2, cellBounds.Y + 8, 3, 7);
        }
        if (((TimeLine) this.DataGridView.Parent).ToolstripRender.FrameNr - 1 != this.ColumnIndex)
          return;
        graphics.DrawLine(new Pen(Color.FromArgb(204, 0, 0)), cellBounds.X + 3, cellBounds.Y, cellBounds.X + 3, cellBounds.Y + 17);
      }
    }

    public class FlashHeaderCell : DataGridViewRowHeaderCell
    {
      protected override void Paint(
        Graphics graphics,
        Rectangle clipBounds,
        Rectangle cellBounds,
        int rowIndex,
        DataGridViewElementStates dataGridViewElementState,
        object value,
        object formattedValue,
        string errorText,
        DataGridViewCellStyle cellStyle,
        DataGridViewAdvancedBorderStyle advancedBorderStyle,
        DataGridViewPaintParts paintParts)
      {
        base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground);
        DataGridViewCellStyle cellStyle1 = cellStyle.Clone();
        if (this.Selected || this.DataGridView.SelectedCells.Count != 0 && this.DataGridView.SelectedCells[0].RowIndex == this.RowIndex)
        {
          graphics.FillRectangle((Brush) new SolidBrush(this.DataGridView.RowHeadersDefaultCellStyle.SelectionBackColor), cellBounds);
          cellStyle1.ForeColor = Color.White;
        }
        cellBounds.X -= 33;
        base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle1, advancedBorderStyle, DataGridViewPaintParts.ContentForeground);
        cellBounds.X += 33;
        graphics.DrawLine(new Pen(Color.FromArgb(222, 222, 222)), cellBounds.X, cellBounds.Y + 17, cellBounds.X + cellBounds.Width, cellBounds.Y + 17);
      }
    }

    public class Frame
    {
      public Frame()
      {
        this.IsKeyFrame = true;
      }

      public Frame(bool IsKeyFrame)
      {
        this.IsKeyFrame = IsKeyFrame;
      }

      public bool IsKeyFrame { get; set; }
    }

    public class Group
    {
      private readonly Collection<TimeLine.Frame> frames = new Collection<TimeLine.Frame>();
      private readonly Collection<TimeLine.Group> subGroups = new Collection<TimeLine.Group>();

      public Group()
      {
      }

      public Group(string Name)
      {
        this.Name = Name;
      }

      public string Name { get; set; }

      public Collection<TimeLine.Frame> Frames
      {
        get
        {
          return this.frames;
        }
      }

      public Collection<TimeLine.Group> SubGroups
      {
        get
        {
          return this.subGroups;
        }
      }

      public override string ToString()
      {
        return this.Name;
      }

      public static implicit operator string[](TimeLine.Group g)
      {
        List<string> stringList = new List<string>();
        int num = 0;
        foreach (TimeLine.Frame frame in g.Frames)
        {
          if (num != g.Frames.Count - 1 && frame.IsKeyFrame && !g.Frames[num + 1].IsKeyFrame)
            stringList.Add("+-");
          else if (frame.IsKeyFrame)
            stringList.Add("+");
          else if (num != g.Frames.Count - 1 && g.Frames[num + 1].IsKeyFrame || num == g.Frames.Count - 1)
            stringList.Add("-+");
          else
            stringList.Add("-");
          ++num;
        }
        return stringList.ToArray();
      }
    }

    public delegate void OnFrameChangedEventHandler(int FrameNr);
  }
}
