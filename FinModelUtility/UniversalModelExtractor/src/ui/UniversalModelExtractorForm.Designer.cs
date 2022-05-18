using uni.ui.common;


namespace uni.ui {
  partial class UniversalModelExtractorForm {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.glPanel1 = new uni.ui.common.GlPanel();
      this.glPanel_ = new uni.ui.common.GlPanel();
      this.SuspendLayout();
      // 
      // glPanel1
      // 
      this.glPanel1.BackColor = System.Drawing.Color.Fuchsia;
      this.glPanel1.Location = new System.Drawing.Point(2118, 473);
      this.glPanel1.Name = "glPanel1";
      this.glPanel1.Size = new System.Drawing.Size(511, 331);
      this.glPanel1.TabIndex = 0;
      // 
      // glPanel_
      // 
      this.glPanel_.BackColor = System.Drawing.Color.Fuchsia;
      this.glPanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.glPanel_.Location = new System.Drawing.Point(0, 0);
      this.glPanel_.Name = "glPanel_";
      this.glPanel_.Size = new System.Drawing.Size(800, 450);
      this.glPanel_.TabIndex = 1;
      // 
      // UniversalModelExtractorForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.glPanel_);
      this.Controls.Add(this.glPanel1);
      this.Name = "UniversalModelExtractorForm";
      this.Text = "Universal Model Extractor";
      this.Load += new System.EventHandler(this.UniversalModelExtractorForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private GlPanel glPanel1;
    private GlPanel glPanel_;
  }
}