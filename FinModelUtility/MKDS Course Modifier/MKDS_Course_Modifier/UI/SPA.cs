// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.UI.SPA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.UI
{
  public class SPA : Form
  {
    private IContainer components = (IContainer) null;
    private ToolStrip toolStrip1;
    private ToolStripComboBox toolStripComboBox1;
    private PictureBox pictureBox1;
    public MKDS_Course_Modifier.Particles.SPA Spa;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (SPA));
      this.toolStrip1 = new ToolStrip();
      this.toolStripComboBox1 = new ToolStripComboBox();
      this.pictureBox1 = new PictureBox();
      this.toolStrip1.SuspendLayout();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.toolStrip1.Items.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.toolStripComboBox1
      });
      this.toolStrip1.Location = new Point(0, 0);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new Size(490, 25);
      this.toolStrip1.TabIndex = 0;
      this.toolStrip1.Text = "toolStrip1";
      this.toolStripComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
      this.toolStripComboBox1.FlatStyle = FlatStyle.Standard;
      this.toolStripComboBox1.Name = "toolStripComboBox1";
      this.toolStripComboBox1.Size = new Size(121, 25);
      this.toolStripComboBox1.SelectedIndexChanged += new EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
      this.pictureBox1.BackColor = Color.White;
      this.pictureBox1.BackgroundImage = (Image) componentResourceManager.GetObject("pictureBox1.BackgroundImage");
      this.pictureBox1.Dock = DockStyle.Fill;
      this.pictureBox1.Location = new Point(0, 25);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(490, 344);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(490, 369);
      this.Controls.Add((Control) this.pictureBox1);
      this.Controls.Add((Control) this.toolStrip1);
      this.Name = nameof (SPA);
      this.Text = nameof (SPA);
      this.Load += new EventHandler(this.SPA_Load);
      this.toolStrip1.ResumeLayout(false);
      this.toolStrip1.PerformLayout();
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public SPA(MKDS_Course_Modifier.Particles.SPA Spa)
    {
      this.Spa = Spa;
      this.InitializeComponent();
    }

    private void SPA_Load(object sender, EventArgs e)
    {
      for (int index = 0; index < (int) this.Spa.NrParticleTextures; ++index)
        this.toolStripComboBox1.Items.Add((object) ("Particle " + (object) index));
      this.toolStripComboBox1.SelectedIndex = 0;
    }

    private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) this.Spa.ParticleTextures[this.toolStripComboBox1.SelectedIndex].ToBitmap();
    }
  }
}
