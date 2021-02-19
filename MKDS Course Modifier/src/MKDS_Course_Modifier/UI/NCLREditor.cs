// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.NCLREditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class NCLREditor : UserControl
  {
    private IContainer components = (IContainer) null;
    private List<Color> colors = new List<Color>();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Name = nameof (NCLREditor);
      this.Size = new Size(410, 330);
      this.Load += new EventHandler(this.NCLREditor_Load);
      this.Paint += new PaintEventHandler(this.NCLREditor_Paint);
      this.MouseUp += new MouseEventHandler(this.NCLREditor_MouseUp);
      this.Resize += new EventHandler(this.NCLREditor_Resize);
      this.ResumeLayout(false);
    }

    public event NCLREditor.SelectedColorChanged OnSelectedColorChanged;

    [Browsable(false)]
    public Color[] Colors
    {
      get
      {
        return this.colors.ToArray();
      }
      set
      {
        this.colors.Clear();
        this.colors.AddRange((IEnumerable<Color>) value);
        this.Invalidate();
      }
    }

    public bool Use16ColorStyle { get; set; }

    public NCLREditor()
    {
      this.SelectedIndex = -1;
      this.InitializeComponent();
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      this.SetStyle(ControlStyles.UserPaint, true);
    }

    private void NCLREditor_Load(object sender, EventArgs e)
    {
    }

    private void NCLREditor_Paint(object sender, PaintEventArgs e)
    {
      e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
      e.Graphics.SmoothingMode = SmoothingMode.None;
      e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
      e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      if (this.Use16ColorStyle)
      {
        int num1 = 0;
        int num2 = 0;
        for (int index = 0; index < this.colors.Count; ++index)
        {
          e.Graphics.FillRectangle((Brush) new SolidBrush(this.colors[index]), 4 + num1 * 20, 4 + num2 * 20, 16, 16);
          e.Graphics.DrawRectangle(new Pen(Color.Black, 1f), 4 + num1 * 20, 4 + num2 * 20, 16, 16);
          if (this.SelectedIndex == index)
          {
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, 2 + num2 * 20), new Point(4 + num1 * 20, 2 + num2 * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, 2 + num2 * 20), new Point(2 + num1 * 20, 4 + num2 * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + (num1 + 1) * 20, 2 + (num2 + 1) * 20), new Point((num1 + 1) * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + (num1 + 1) * 20, (num2 + 1) * 20), new Point(2 + (num1 + 1) * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, 2 + (num2 + 1) * 20), new Point(4 + num1 * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, (num2 + 1) * 20), new Point(2 + num1 * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point((num1 + 1) * 20, 2 + num2 * 20), new Point(2 + (num1 + 1) * 20, 2 + num2 * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + (num1 + 1) * 20, 2 + num2 * 20), new Point(2 + (num1 + 1) * 20, 4 + num2 * 20));
          }
          if (num1 == 15)
          {
            num1 = 0;
            ++num2;
          }
          else
            ++num1;
        }
      }
      else
      {
        int num1 = 0;
        int num2 = 0;
        for (int index = 0; index < this.colors.Count; ++index)
        {
          e.Graphics.FillRectangle((Brush) new SolidBrush(this.colors[index]), 4 + num1 * 20, 4 + num2 * 20, 16, 16);
          e.Graphics.DrawRectangle(new Pen(Color.Black, 1f), 4 + num1 * 20, 4 + num2 * 20, 16, 16);
          if (this.SelectedIndex == index)
          {
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, 2 + num2 * 20), new Point(5 + num1 * 20, 2 + num2 * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, 2 + num2 * 20), new Point(2 + num1 * 20, 5 + num2 * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + (num1 + 1) * 20, 2 + (num2 + 1) * 20), new Point((num1 + 1) * 20 - 1, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + (num1 + 1) * 20, (num2 + 1) * 20 - 1), new Point(2 + (num1 + 1) * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, 2 + (num2 + 1) * 20), new Point(5 + num1 * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + num1 * 20, (num2 + 1) * 20 - 1), new Point(2 + num1 * 20, 2 + (num2 + 1) * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point((num1 + 1) * 20 - 1, 2 + num2 * 20), new Point(2 + (num1 + 1) * 20, 2 + num2 * 20));
            e.Graphics.DrawLine(new Pen(Color.Black, 1f), new Point(2 + (num1 + 1) * 20, 2 + num2 * 20), new Point(2 + (num1 + 1) * 20, 5 + num2 * 20));
          }
          if (num1 == (this.Width - 4) / 20 - 1)
          {
            num1 = 0;
            ++num2;
          }
          else
            ++num1;
        }
      }
    }

    private void NCLREditor_Resize(object sender, EventArgs e)
    {
      this.Invalidate();
    }

    private void NCLREditor_MouseUp(object sender, MouseEventArgs e)
    {
      int num1 = (e.X - 4) / 20;
      int num2 = (e.Y - 4) / 20;
      int num3 = this.Use16ColorStyle ? 16 : (this.Width - 4) / 20;
      if (e.X - 4 - num1 * 20 < 0 || e.X - 4 - num1 * 20 > 16 || (e.Y - 4 - num2 * 20 < 0 || e.Y - 4 - num2 * 20 > 16) || num1 > num3 - 1 || num2 > this.colors.Count / (num3 - 1) || num1 + num2 * num3 > this.colors.Count - 1)
        return;
      this.SelectedIndex = num1 + num2 * num3;
      if (this.OnSelectedColorChanged != null)
        this.OnSelectedColorChanged(this.SelectedColor);
      this.Invalidate();
    }

    private int SelectedIndex { get; set; }

    public Color SelectedColor
    {
      get
      {
        return this.colors[this.SelectedIndex];
      }
      set
      {
        this.colors[this.SelectedIndex] = value;
        this.Invalidate();
      }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      switch (keyData)
      {
        case Keys.Left:
          if (this.SelectedIndex > 0)
          {
            --this.SelectedIndex;
            if (this.OnSelectedColorChanged != null)
              this.OnSelectedColorChanged(this.SelectedColor);
            this.Invalidate();
          }
          return true;
        case Keys.Up:
          if (this.SelectedIndex > (this.Use16ColorStyle ? 16 : (this.Width - 4) / 20))
          {
            this.SelectedIndex -= this.Use16ColorStyle ? 16 : (this.Width - 4) / 20;
            if (this.OnSelectedColorChanged != null)
              this.OnSelectedColorChanged(this.SelectedColor);
            this.Invalidate();
          }
          return true;
        case Keys.Right:
          if (this.SelectedIndex < this.colors.Count - 1)
          {
            ++this.SelectedIndex;
            if (this.OnSelectedColorChanged != null)
              this.OnSelectedColorChanged(this.SelectedColor);
            this.Invalidate();
          }
          return true;
        case Keys.Down:
          if (this.SelectedIndex < this.colors.Count - (this.Use16ColorStyle ? 16 : (this.Width - 4) / 20))
          {
            this.SelectedIndex += this.Use16ColorStyle ? 16 : (this.Width - 4) / 20;
            if (this.OnSelectedColorChanged != null)
              this.OnSelectedColorChanged(this.SelectedColor);
            this.Invalidate();
          }
          return true;
        case Keys.Delete:
          if (this.SelectedIndex != -1)
          {
            this.colors.RemoveAt(this.SelectedIndex);
            if (this.OnSelectedColorChanged != null)
              this.OnSelectedColorChanged(this.SelectedColor);
            this.Invalidate();
          }
          return true;
        case Keys.C | Keys.Control:
          if (this.SelectedIndex != -1)
            Clipboard.SetData("NCLR Color", (object) this.SelectedColor);
          return true;
        case Keys.V | Keys.Control:
          if (this.SelectedIndex != -1)
          {
            this.SelectedColor = (Color) Clipboard.GetData("NCLR Color");
            if (this.OnSelectedColorChanged != null)
              this.OnSelectedColorChanged(this.SelectedColor);
          }
          return true;
        default:
          return base.ProcessCmdKey(ref msg, keyData);
      }
    }

    public delegate void SelectedColorChanged(Color c);
  }
}
