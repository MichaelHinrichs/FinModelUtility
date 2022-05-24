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
      System.Windows.Forms.SplitContainer splitContainer1;
      System.Windows.Forms.SplitContainer splitContainer2;
      System.Windows.Forms.MenuStrip menuStrip;
      this.modelFileTreeView_ = new uni.ui.common.ModelFileTreeView();
      this.modelViewerGlPanel_ = new uni.ui.common.ModelViewerGlPanel();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      splitContainer1 = new System.Windows.Forms.SplitContainer();
      splitContainer2 = new System.Windows.Forms.SplitContainer();
      menuStrip = new System.Windows.Forms.MenuStrip();
      ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.SuspendLayout();
      menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer1.Location = new System.Drawing.Point(0, 24);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(this.modelFileTreeView_);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(splitContainer2);
      splitContainer1.Size = new System.Drawing.Size(1013, 520);
      splitContainer1.SplitterDistance = 224;
      splitContainer1.TabIndex = 2;
      // 
      // modelFileTreeView_
      // 
      this.modelFileTreeView_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.modelFileTreeView_.Location = new System.Drawing.Point(0, 0);
      this.modelFileTreeView_.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.modelFileTreeView_.Name = "modelFileTreeView_";
      this.modelFileTreeView_.Size = new System.Drawing.Size(224, 520);
      this.modelFileTreeView_.TabIndex = 0;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer2.Location = new System.Drawing.Point(0, 0);
      splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(this.modelViewerGlPanel_);
      splitContainer2.Size = new System.Drawing.Size(785, 520);
      splitContainer2.SplitterDistance = 573;
      splitContainer2.TabIndex = 1;
      // 
      // modelViewerGlPanel_
      // 
      this.modelViewerGlPanel_.BackColor = System.Drawing.Color.Fuchsia;
      this.modelViewerGlPanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.modelViewerGlPanel_.Location = new System.Drawing.Point(0, 0);
      this.modelViewerGlPanel_.Name = "modelViewerGlPanel_";
      this.modelViewerGlPanel_.Size = new System.Drawing.Size(573, 520);
      this.modelViewerGlPanel_.TabIndex = 0;
      // 
      // menuStrip
      // 
      menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
      menuStrip.Location = new System.Drawing.Point(0, 0);
      menuStrip.Name = "menuStrip";
      menuStrip.Size = new System.Drawing.Size(1013, 24);
      menuStrip.TabIndex = 1;
      menuStrip.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // UniversalModelExtractorForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1013, 544);
      this.Controls.Add(splitContainer1);
      this.Controls.Add(menuStrip);
      this.MainMenuStrip = menuStrip;
      this.Name = "UniversalModelExtractorForm";
      this.Text = "Universal Model Extractor";
      this.Load += new System.EventHandler(this.UniversalModelExtractorForm_Load);
      splitContainer1.Panel1.ResumeLayout(false);
      splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
      splitContainer1.ResumeLayout(false);
      splitContainer2.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
      splitContainer2.ResumeLayout(false);
      menuStrip.ResumeLayout(false);
      menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ModelViewerGlPanel modelViewerGlPanel_;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private SplitContainer splitContainer1;
    private ModelFileTreeView modelFileTreeView_;
  }
}