namespace uni.ui.top {
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
      this.exportAllModelsInSelectedDirectoryButton_ = new System.Windows.Forms.ToolStripButton();
      this.exportSelectedModelButton_ = new System.Windows.Forms.ToolStripButton();
      toolStrip = new System.Windows.Forms.ToolStrip();
      toolStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip
      // 
      toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAllModelsInSelectedDirectoryButton_,
            this.exportSelectedModelButton_});
      toolStrip.Location = new System.Drawing.Point(0, 0);
      toolStrip.Name = "toolStrip";
      toolStrip.Size = new System.Drawing.Size(451, 31);
      toolStrip.Stretch = true;
      toolStrip.TabIndex = 0;
      toolStrip.Text = "toolStrip1";
      // 
      // exportAllModelsInSelectedDirectoryButton_
      // 
      this.exportAllModelsInSelectedDirectoryButton_.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.exportAllModelsInSelectedDirectoryButton_.Enabled = false;
      this.exportAllModelsInSelectedDirectoryButton_.Image = ((System.Drawing.Image)(resources.GetObject("exportAllModelsInSelectedDirectoryButton_.Image")));
      this.exportAllModelsInSelectedDirectoryButton_.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.exportAllModelsInSelectedDirectoryButton_.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.exportAllModelsInSelectedDirectoryButton_.Name = "exportAllModelsInSelectedDirectoryButton_";
      this.exportAllModelsInSelectedDirectoryButton_.Size = new System.Drawing.Size(28, 28);
      this.exportAllModelsInSelectedDirectoryButton_.Text = "Export All";
      this.exportAllModelsInSelectedDirectoryButton_.ToolTipText = "Export all models in selected directory";
      this.exportAllModelsInSelectedDirectoryButton_.Click += new System.EventHandler(this.exportAllModelsInSelectedDirectoryButton__Click);
      // 
      // exportSelectedModelButton_
      // 
      this.exportSelectedModelButton_.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.exportSelectedModelButton_.Enabled = false;
      this.exportSelectedModelButton_.Image = ((System.Drawing.Image)(resources.GetObject("exportSelectedModelButton_.Image")));
      this.exportSelectedModelButton_.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.exportSelectedModelButton_.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.exportSelectedModelButton_.Name = "exportSelectedModelButton_";
      this.exportSelectedModelButton_.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
      this.exportSelectedModelButton_.Size = new System.Drawing.Size(28, 28);
      this.exportSelectedModelButton_.Text = "Export";
      this.exportSelectedModelButton_.ToolTipText = "Export selected model";
      this.exportSelectedModelButton_.Click += new System.EventHandler(this.exportSelectedModelButton__Click);
      // 
      // ModelToolStrip
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(toolStrip);
      this.Name = "ModelToolStrip";
      this.Size = new System.Drawing.Size(451, 31);
      toolStrip.ResumeLayout(false);
      toolStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ToolStrip toolStrip;
    private ToolStripButton exportSelectedModelButton_;
    private ToolStripButton exportAllModelsInSelectedDirectoryButton_;
  }
}
