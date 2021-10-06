// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.WAV
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Wave;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class WAV : Form
  {
    private WaveOut d = new WaveOut();
    private IContainer components = (IContainer) null;
    private MKDS_Course_Modifier.Sound.WAV wav;
    private ToolStrip toolStrip1;
    private ToolStripButton toolStripButton1;
    private WaveViewer waveViewer1;

    public WAV(MKDS_Course_Modifier.Sound.WAV wav)
    {
      this.wav = wav;
      this.InitializeComponent();
    }

    private void SWAV_Load(object sender, EventArgs e)
    {
      this.waveViewer1.WaveStream = (WaveStream) new WaveFileReader((Stream) new MemoryStream(this.wav.Write()));
      this.waveViewer1.SamplesPerPixel = (int) this.waveViewer1.WaveStream.Length / (this.waveViewer1.WaveStream.WaveFormat.BitsPerSample / 8) / this.Width / this.waveViewer1.WaveStream.WaveFormat.Channels;
    }

    private void WAV_Resize(object sender, EventArgs e)
    {
      this.waveViewer1.SamplesPerPixel = (int) this.waveViewer1.WaveStream.Length / (this.waveViewer1.WaveStream.WaveFormat.BitsPerSample / 8) / this.Width / this.waveViewer1.WaveStream.WaveFormat.Channels;
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      if (this.d.PlaybackState == PlaybackState.Stopped)
      {
        this.d.Init((IWaveProvider) new WaveFileReader((Stream) new MemoryStream(this.wav.Write())));
        this.d.Play();
      }
      else if (this.d.PlaybackState == PlaybackState.Playing)
      {
        this.d.Pause();
      }
      else
      {
        if (this.d.PlaybackState != PlaybackState.Paused)
          return;
        this.d.Resume();
      }
    }

    private void WAV_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.d.Stop();
      this.d.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (WAV));
      this.toolStrip1 = new ToolStrip();
      this.toolStripButton1 = new ToolStripButton();
      this.waveViewer1 = new WaveViewer();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripButton1
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(549, 25);
      this.toolStrip1.TabIndex = 1;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = (Image) componentResourceManager.GetObject("toolStripButton1.Image");
      this.toolStripButton1.ImageTransparentColor = Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new Size(23, 22);
      this.toolStripButton1.Text = "toolStripButton1";
      this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
      this.waveViewer1.Dock = DockStyle.Fill;
      this.waveViewer1.FillColor = Color.MediumAquamarine;
      this.waveViewer1.LineColor = Color.FromArgb(0, 64, 0);
      this.waveViewer1.Location = new Point(0, 25);
      this.waveViewer1.Name = "waveViewer1";
      this.waveViewer1.SamplesPerPixel = 32;
      this.waveViewer1.Size = new Size(549, 332);
      this.waveViewer1.StartPosition = 0L;
      this.waveViewer1.TabIndex = 2;
      this.waveViewer1.WaveStream = (WaveStream) null;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(549, 357);
      this.Controls.Add((Control) this.waveViewer1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (WAV);
      this.Text = nameof (WAV);
      this.FormClosing += new FormClosingEventHandler(this.WAV_FormClosing);
      this.Load += new EventHandler(this.SWAV_Load);
      this.Resize += new EventHandler(this.WAV_Resize);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
