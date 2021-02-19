// Decompiled with JetBrains decompiler
// Type: AngleAltitudeControls.AngleSelector
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AngleAltitudeControls
{
  public class AngleSelector : UserControl
  {
    private IContainer components = (IContainer) null;
    private int angle;
    private Rectangle drawRegion;
    private Point origin;

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
      this.Name = nameof (AngleSelector);
      this.Size = new Size(40, 40);
      this.Load += new EventHandler(this.AngleSelector_Load);
      this.MouseMove += new MouseEventHandler(this.AngleSelector_MouseMove);
      this.MouseDown += new MouseEventHandler(this.AngleSelector_MouseDown);
      this.SizeChanged += new EventHandler(this.AngleSelector_SizeChanged);
      this.ResumeLayout(false);
    }

    public AngleSelector()
    {
      this.InitializeComponent();
      this.DoubleBuffered = true;
    }

    private void AngleSelector_Load(object sender, EventArgs e)
    {
      this.setDrawRegion();
    }

    private void AngleSelector_SizeChanged(object sender, EventArgs e)
    {
      this.Height = this.Width;
      this.setDrawRegion();
    }

    private void setDrawRegion()
    {
      this.drawRegion = new Rectangle(0, 0, this.Width, this.Height);
      this.drawRegion.X += 2;
      this.drawRegion.Y += 2;
      this.drawRegion.Width -= 4;
      this.drawRegion.Height -= 4;
      int num = 2;
      this.origin = new Point(this.drawRegion.Width / 2 + num, this.drawRegion.Height / 2 + num);
      this.Refresh();
    }

    public int Angle
    {
      get
      {
        return this.angle;
      }
      set
      {
        this.angle = value;
        if (!this.DesignMode && this.AngleChanged != null)
          this.AngleChanged();
        this.Refresh();
      }
    }

    public event AngleSelector.AngleChangedDelegate AngleChanged;

    private PointF DegreesToXY(float degrees, float radius, Point origin)
    {
      PointF pointF = new PointF();
      double d = (double) degrees * Math.PI / 180.0;
      pointF.X = (float) Math.Cos(d) * radius + (float) origin.X;
      pointF.Y = (float) Math.Sin(-d) * radius + (float) origin.Y;
      return pointF;
    }

    private float XYToDegrees(Point xy, Point origin)
    {
      double num = 0.0;
      if (xy.Y < origin.Y)
      {
        if (xy.X > origin.X)
          num = 90.0 - Math.Atan((double) (xy.X - origin.X) / (double) (origin.Y - xy.Y)) * 180.0 / Math.PI;
        else if (xy.X < origin.X)
          num = 90.0 - Math.Atan(-((double) (origin.X - xy.X) / (double) (origin.Y - xy.Y))) * 180.0 / Math.PI;
      }
      else if (xy.Y > origin.Y)
      {
        if (xy.X > origin.X)
          num = 270.0 - Math.Atan(-((double) (xy.X - origin.X) / (double) (xy.Y - origin.Y))) * 180.0 / Math.PI;
        else if (xy.X < origin.X)
          num = 270.0 - Math.Atan((double) (origin.X - xy.X) / (double) (xy.Y - origin.Y)) * 180.0 / Math.PI;
      }
      if (num > 180.0)
        num -= 360.0;
      return (float) num;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      Pen pen = new Pen(Color.FromArgb(86, 103, 141), 2f);
      SolidBrush solidBrush = new SolidBrush(Color.FromArgb(90, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
      PointF xy = this.DegreesToXY((float) this.angle, (float) (this.origin.X - 2), this.origin);
      Rectangle rect = new Rectangle(this.origin.X - 1, this.origin.Y - 1, 3, 3);
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      graphics.DrawEllipse(pen, this.drawRegion);
      graphics.FillEllipse((Brush) solidBrush, this.drawRegion);
      graphics.DrawLine(Pens.Black, (PointF) this.origin, xy);
      graphics.SmoothingMode = SmoothingMode.HighSpeed;
      graphics.FillRectangle(Brushes.Black, rect);
      solidBrush.Dispose();
      pen.Dispose();
      base.OnPaint(e);
    }

    private void AngleSelector_MouseDown(object sender, MouseEventArgs e)
    {
      int nearestAngle = this.findNearestAngle(new Point(e.X, e.Y));
      if (nearestAngle == -1)
        return;
      this.Angle = nearestAngle;
      this.Refresh();
    }

    private void AngleSelector_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
        return;
      int nearestAngle = this.findNearestAngle(new Point(e.X, e.Y));
      if (nearestAngle != -1)
      {
        this.Angle = nearestAngle;
        this.Refresh();
      }
    }

    private int findNearestAngle(Point mouseXY)
    {
      int degrees = (int) this.XYToDegrees(mouseXY, this.origin);
      return degrees != 0 ? degrees : -1;
    }

    public delegate void AngleChangedDelegate();
  }
}
