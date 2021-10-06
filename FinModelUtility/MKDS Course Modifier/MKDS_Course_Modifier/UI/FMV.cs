// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.FMV
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class FMV : Form
  {
    private int aa = 0;
    private int bb = 0;
    private bool first = true;
    private Queue<Bitmap> Frames = new Queue<Bitmap>();
    private bool stop = false;
    private bool stopped = false;
    private IContainer components = (IContainer) null;
    private BufferedWaveProvider AudioBuffer;
    private WaveOut Player;
    private MKDS_Course_Modifier.Misc.FMV Video;
    private PictureBox pictureBox1;
    private BackgroundWorker backgroundWorker1;

    public FMV(MKDS_Course_Modifier.Misc.FMV Video)
    {
      this.Video = Video;
      this.InitializeComponent();
    }

    private void FMV_Load(object sender, EventArgs e)
    {
      if (((int) this.Video.Header.Flags & 4) == 4)
      {
        this.AudioBuffer = new BufferedWaveProvider(new WaveFormat((int) this.Video.Header.AudioRate, 16, 1));
        this.AudioBuffer.DiscardOnBufferOverflow = true;
        this.AudioBuffer.BufferLength = 131072;
        this.Player = new WaveOut();
        this.Player.DesiredLatency = 150;
        this.Player.Init((IWaveProvider) this.AudioBuffer);
        this.Player.Play();
      }
      new Thread((ThreadStart) (() =>
      {
        int num = 0;
        while (!this.stop)
        {
          if (this.Frames.Count != 0)
          {
            this.pictureBox1.Image = (Image) this.Frames.Dequeue();
            switch (num)
            {
              case 0:
                Thread.Sleep(TimeSpan.FromTicks(666666L));
                break;
              case 1:
                Thread.Sleep(TimeSpan.FromTicks(666667L));
                break;
              case 2:
                Thread.Sleep(TimeSpan.FromTicks(666667L));
                break;
            }
            num = (num + 1) % 3;
          }
        }
        Thread.CurrentThread.Abort();
      })).Start();
      this.backgroundWorker1.RunWorkerAsync();
    }

    private void FMV_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (!this.stop)
      {
        this.stop = true;
        if (((int) this.Video.Header.Flags & 4) == 4)
        {
          this.Player.Stop();
          this.Player.Dispose();
          this.Player = (WaveOut) null;
          this.AudioBuffer = (BufferedWaveProvider) null;
        }
      }
      this.Video.Close();
    }

    private unsafe void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      while (!this.stop)
      {
        if (this.Frames.Count < 8 || this.AudioBuffer.BufferedBytes < 32768)
        {
          Bitmap nextFrame;
          while (true)
          {
            byte[] Audio;
            nextFrame = this.Video.GetNextFrame(out Audio);
            if (Audio != null)
            {
              byte[] buffer = !this.first ? new byte[Audio.Length * 4] : new byte[(Audio.Length - 4) * 4];
              byte* buf = (byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) Audio, 0);
              byte* outbuffer = (byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) buffer, 0);
              int length = Audio.Length;
              if (this.first)
              {
                this.aa = (int) *(short*) buf;
                this.bb = (int) *(short*) (buf + 2) & (int) sbyte.MaxValue;
                *(short*) outbuffer = (short) this.aa;
                this.first = false;
                buf += 4;
                length -= 4;
                outbuffer += 2;
              }
              Sound.ConvertImaAdpcm(buf, length, outbuffer, ref this.aa, ref this.bb);
              this.AudioBuffer.AddSamples(buffer, 0, buffer.Length);
            }
            else
              break;
          }
          if (nextFrame == null)
          {
            this.stop = true;
            if (((int) this.Video.Header.Flags & 4) == 4)
            {
              this.Player.Stop();
              this.Player.Dispose();
              this.Player = (WaveOut) null;
              this.AudioBuffer = (BufferedWaveProvider) null;
            }
          }
          else
            this.Frames.Enqueue(nextFrame);
        }
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
      this.pictureBox1 = new PictureBox();
      this.backgroundWorker1 = new BackgroundWorker();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 0);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(422, 326);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.backgroundWorker1.DoWork += new DoWorkEventHandler(this.backgroundWorker1_DoWork);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(422, 326);
      this.Controls.Add((Control) this.pictureBox1);
      this.Name = nameof (FMV);
      this.Text = nameof (FMV);
      this.FormClosing += new FormClosingEventHandler(this.FMV_FormClosing);
      this.Load += new EventHandler(this.FMV_Load);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
    }
  }
}
