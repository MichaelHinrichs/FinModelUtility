namespace uni.src.ui.top {
  partial class ModelToolStrip {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.Windows.Forms.ToolStrip toolStrip;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelToolStrip));
      this.exportSelectedModelButton_ = new System.Windows.Forms.ToolStripButton();
      this.exportAllModelsButton_ = new System.Windows.Forms.ToolStripButton();
      toolStrip = new System.Windows.Forms.ToolStrip();
      toolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip
      // 
      toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSelectedModelButton_,
            this.exportAllModelsButton_});
      toolStrip.Location = new System.Drawing.Point(0, 0);
      toolStrip.Name = "toolStrip";
      toolStrip.Size = new System.Drawing.Size(451, 25);
      toolStrip.Stretch = true;
      toolStrip.TabIndex = 0;
      toolStrip.Text = "toolStrip1";
      // 
      // exportSelectedModelButton_
      // 
      this.exportSelectedModelButton_.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.exportSelectedModelButton_.Enabled = false;
      this.exportSelectedModelButton_.Image = ((System.Drawing.Image)(resources.GetObject("exportSelectedModelButton_.Image")));
      this.exportSelectedModelButton_.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.exportSelectedModelButton_.Name = "exportSelectedModelButton_";
      this.exportSelectedModelButton_.Size = new System.Drawing.Size(23, 22);
      this.exportSelectedModelButton_.Text = "Export";
      this.exportSelectedModelButton_.ToolTipText = "Export Selected Model";
      // 
      // exportAllModelsButton_
      // 
      this.exportAllModelsButton_.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.exportAllModelsButton_.Enabled = false;
      this.exportAllModelsButton_.Image = ((System.Drawing.Image)(resources.GetObject("exportAllModelsButton_.Image")));
      this.exportAllModelsButton_.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.exportAllModelsButton_.Name = "exportAllModelsButton_";
      this.exportAllModelsButton_.Size = new System.Drawing.Size(23, 22);
      this.exportAllModelsButton_.Text = "Export All";
      this.exportAllModelsButton_.ToolTipText = "Export All Models";
      // 
      // UserControl1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(toolStrip);
      this.Name = "UserControl1";
      this.Size = new System.Drawing.Size(451, 25);
      toolStrip.ResumeLayout(false);
      toolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ToolStrip toolStrip;
    private ToolStripButton exportSelectedModelButton_;
    private ToolStripButton exportAllModelsButton_;
  }
}
