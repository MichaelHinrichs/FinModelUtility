// Decompiled with JetBrains decompiler
// Type: AngleAltitudeControls.AngleAltitudeSelector
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
  public class AngleAltitudeSelector : UserControl
  {
    private IContainer components = (IContainer) null;
    private int altitude = 90;
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
      this.Name = nameof (AngleAltitudeSelector);
      this.Size = new Size(40, 40);
      this.Load += new EventHandler(this.AngleAltitudeSelector_Load);
      this.MouseMove += new MouseEventHandler(this.AngleAltitudeSelector_MouseMove);
      this.MouseDown += new MouseEventHandler(this.AngleAltitudeSelector_MouseDown);
      this.SizeChanged += new EventHandler(this.AngleAltitudeSelector_SizeChanged);
      this.ResumeLayout(false);
    }

    public AngleAltitudeSelector()
    {
      this.InitializeComponent();
      this.DoubleBuffered = true;
    }

    private void AngleAltitudeSelector_Load(object sender, EventArgs e)
    {
      this.setDrawRegion();
    }

    private void AngleAltitudeSelector_SizeChanged(object sender, EventArgs e)
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

    public int Altitude
    {
      get
      {
        return this.altitude;
      }
      set
      {
        this.altitude = value;
        if (!this.DesignMode && this.AltitudeChanged != null)
          this.AltitudeChanged();
        this.Refresh();
      }
    }

    public event AngleAltitudeSelector.AngleChangedDelegate AngleChanged;

    public event AngleAltitudeSelector.AltitudeChangedDelegate AltitudeChanged;

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

    private float getDistance(Point point1, Point point2)
    {
      return (float) Math.Sqrt(Math.Pow((double) (point1.X - point2.X), 2.0) + Math.Pow((double) (point1.Y - point2.Y), 2.0));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics graphics = e.Graphics;
      Pen pen = new Pen(Color.FromArgb(86, 103, 141), 2f);
      SolidBrush solidBrush = new SolidBrush(Color.FromArgb(90, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
      PointF xy = this.DegreesToXY((float) this.angle, (float) ((double) this.origin.X * (90.0 - (double) this.altitude) / 100.0), this.origin);
      Rectangle rect1 = new Rectangle(this.origin.X - 1, this.origin.Y - 1, 3, 3);
      Rectangle rect2 = new Rectangle((int) xy.X, (int) xy.Y, 1, 1);
      graphics.SmoothingMode = SmoothingMode.AntiAlias;
      graphics.DrawEllipse(pen, this.drawRegion);
      graphics.FillEllipse((Brush) solidBrush, this.drawRegion);
      graphics.SmoothingMode = SmoothingMode.HighSpeed;
      graphics.FillRectangle(Brushes.Black, rect2);
      int x1_1 = rect2.X - 3;
      if (x1_1 < 0)
        x1_1 = 0;
      int x2_1 = rect2.X - 2;
      if (x2_1 < 0)
        x2_1 = 0;
      int x1_2 = rect2.X + 2;
      if (x1_2 > this.drawRegion.Right)
        x1_2 = this.drawRegion.Right;
      int x2_2 = rect2.X + 3;
      if (x2_2 > this.drawRegion.Right)
        x2_2 = this.drawRegion.Right;
      int y1_1 = rect2.Y - 3;
      if (y1_1 < 0)
        y1_1 = 0;
      int y2_1 = rect2.Y - 2;
      if (y2_1 < 0)
        y2_1 = 0;
      int y1_2 = rect2.Y + 2;
      if (y1_2 > this.drawRegion.Bottom)
        y1_2 = this.drawRegion.Bottom;
      int y2_2 = rect2.Y + 3;
      if (y2_2 > this.drawRegion.Bottom)
        y2_2 = this.drawRegion.Bottom;
      graphics.DrawLine(Pens.Black, x1_1, rect2.Y, x2_1, rect2.Y);
      graphics.DrawLine(Pens.Black, x1_2, rect2.Y, x2_2, rect2.Y);
      graphics.DrawLine(Pens.Black, rect2.X, y1_1, rect2.X, y2_1);
      graphics.DrawLine(Pens.Black, rect2.X, y1_2, rect2.X, y2_2);
      graphics.FillRectangle(Brushes.Black, rect1);
      solidBrush.Dispose();
      pen.Dispose();
      base.OnPaint(e);
    }

    private void AngleAltitudeSelector_MouseDown(object sender, MouseEventArgs e)
    {
      int nearestAngle = this.findNearestAngle(new Point(e.X, e.Y));
      int altitude = this.findAltitude(new Point(e.X, e.Y));
      if (nearestAngle != -1)
        this.Angle = nearestAngle;
      this.Altitude = altitude;
      this.Refresh();
    }

    private void AngleAltitudeSelector_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
        return;
      int nearestAngle = this.findNearestAngle(new Point(e.X, e.Y));
      int altitude = this.findAltitude(new Point(e.X, e.Y));
      if (nearestAngle != -1)
        this.Angle = nearestAngle;
      this.Altitude = altitude;
      this.Refresh();
    }

    private int findNearestAngle(Point mouseXY)
    {
      int degrees = (int) this.XYToDegrees(mouseXY, this.origin);
      return degrees != 0 ? degrees : -1;
    }

    private int findAltitude(Point mouseXY)
    {
      int num = 90 - (int) (90.0 * ((double) this.getDistance(mouseXY, this.origin) / (double) this.origin.X));
      if (num < 0)
        num = 0;
      return num;
    }

    public delegate void AngleChangedDelegate();

    public delegate void AltitudeChangedDelegate();
  }
}
