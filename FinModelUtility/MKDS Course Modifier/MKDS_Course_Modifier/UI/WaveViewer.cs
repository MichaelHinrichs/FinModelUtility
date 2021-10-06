// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.WaveViewer
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Wave;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class WaveViewer : UserControl
  {
    private Container components = (Container) null;
    private int samplesPerPixel = 128;
    private WaveStream waveStream;
    private long startPosition;
    private int bytesPerSample;

    public WaveViewer()
    {
      this.InitializeComponent();
      this.DoubleBuffered = true;
    }

    public WaveStream WaveStream
    {
      get
      {
        return this.waveStream;
      }
      set
      {
        this.waveStream = value;
        if (this.waveStream != null)
          this.bytesPerSample = this.waveStream.WaveFormat.BitsPerSample / 8 * this.waveStream.WaveFormat.Channels;
        this.Invalidate();
      }
    }

    public Color LineColor { get; set; }

    public Color FillColor { get; set; }

    public int SamplesPerPixel
    {
      get
      {
        return this.samplesPerPixel;
      }
      set
      {
        this.samplesPerPixel = value;
        this.Invalidate();
      }
    }

    public long StartPosition
    {
      get
      {
        return this.startPosition;
      }
      set
      {
        this.startPosition = value;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (this.waveStream != null)
      {
        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        this.waveStream.Position = 0L;
        byte[] buffer = new byte[this.samplesPerPixel * this.bytesPerSample];
        WaveStream waveStream = this.waveStream;
        long startPosition = this.startPosition;
        Rectangle clipRectangle = e.ClipRectangle;
        long num1 = (long) (clipRectangle.Left * this.bytesPerSample * this.samplesPerPixel);
        long num2 = startPosition + num1;
        waveStream.Position = num2;
        clipRectangle = e.ClipRectangle;
        float x = (float) clipRectangle.X;
        while (true)
        {
          double num3 = (double) x;
          clipRectangle = e.ClipRectangle;
          double right = (double) clipRectangle.Right;
          if (num3 < right)
          {
            short num4 = 0;
            short num5 = 0;
            int num6 = this.waveStream.Read(buffer, 0, this.samplesPerPixel * this.bytesPerSample);
            if (num6 != 0)
            {
              for (int startIndex = 0; startIndex < num6; startIndex += 2)
              {
                short int16 = BitConverter.ToInt16(buffer, startIndex);
                if ((int) int16 < (int) num4)
                  num4 = int16;
                if ((int) int16 > (int) num5)
                  num5 = int16;
              }
              float num7 = (float) (((double) num4 - (double) short.MinValue) / (double) ushort.MaxValue);
              float num8 = (float) (((double) num5 - (double) short.MinValue) / (double) ushort.MaxValue);
              short num9 = 0;
              short num10 = 0;
              int num11 = this.waveStream.Read(buffer, 0, this.samplesPerPixel * this.bytesPerSample);
              this.waveStream.Position -= (long) (this.samplesPerPixel * this.bytesPerSample);
              if (num11 != 0)
              {
                for (int startIndex = 0; startIndex < num11; startIndex += 2)
                {
                  short int16 = BitConverter.ToInt16(buffer, startIndex);
                  if ((int) int16 < (int) num9)
                    num9 = int16;
                  if ((int) int16 > (int) num10)
                    num10 = int16;
                }
                float num12 = (float) (((double) num9 - (double) short.MinValue) / (double) ushort.MaxValue);
                float num13 = (float) (((double) num10 - (double) short.MinValue) / (double) ushort.MaxValue);
                e.Graphics.DrawLine(new Pen(this.FillColor), x, (float) this.Height * num7, x, (float) this.Height * num8);
                e.Graphics.DrawLine(new Pen(this.LineColor, 1f), x, (float) this.Height * num7, x + 1f, (float) this.Height * num12);
                e.Graphics.DrawLine(new Pen(this.LineColor, 1f), x, (float) this.Height * num8, x + 1f, (float) this.Height * num13);
                ++x;
              }
              else
                break;
            }
            else
              break;
          }
          else
            break;
        }
      }
      base.OnPaint(e);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.Name = nameof (WaveViewer);
      this.Resize += new EventHandler(this.WaveViewer_Resize);
      this.ResumeLayout(false);
    }

    private void WaveViewer_Resize(object sender, EventArgs e)
    {
      this.Invalidate();
    }
  }
}
