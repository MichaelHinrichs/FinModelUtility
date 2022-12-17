using uni.ui.common;
using uni.ui.common.audio;
using uni.ui.common.scene;
using uni.ui.right_panel;


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
      System.Windows.Forms.SplitContainer splitContainer3;
      this.fileBundleTreeView_ = new uni.ui.common.FileBundleTreeView();
      this.splitContainer4 = new System.Windows.Forms.SplitContainer();
      this.sceneViewerPanel_ = new uni.ui.common.scene.SceneViewerPanel();
      this.audioPlayerPanel_ = new uni.ui.common.audio.AudioPlayerPanel();
      this.modelTabs_ = new uni.ui.right_panel.ModelTabs();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.gitHubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.reportAnIssueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.modelToolStrip_ = new uni.ui.top.ModelToolStrip();
      splitContainer1 = new System.Windows.Forms.SplitContainer();
      splitContainer2 = new System.Windows.Forms.SplitContainer();
      menuStrip = new System.Windows.Forms.MenuStrip();
      splitContainer3 = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
      splitContainer1.Panel1.SuspendLayout();
      splitContainer1.Panel2.SuspendLayout();
      splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
      splitContainer2.Panel1.SuspendLayout();
      splitContainer2.Panel2.SuspendLayout();
      splitContainer2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
      this.splitContainer4.Panel1.SuspendLayout();
      this.splitContainer4.Panel2.SuspendLayout();
      this.splitContainer4.SuspendLayout();
      menuStrip.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(splitContainer3)).BeginInit();
      splitContainer3.Panel1.SuspendLayout();
      splitContainer3.Panel2.SuspendLayout();
      splitContainer3.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer1
      // 
      splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      splitContainer1.Location = new System.Drawing.Point(0, 0);
      splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      splitContainer1.Panel1.Controls.Add(this.fileBundleTreeView_);
      // 
      // splitContainer1.Panel2
      // 
      splitContainer1.Panel2.Controls.Add(splitContainer2);
      splitContainer1.Size = new System.Drawing.Size(1013, 488);
      splitContainer1.SplitterDistance = 224;
      splitContainer1.TabIndex = 2;
      // 
      // fileBundleTreeView_
      // 
      this.fileBundleTreeView_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.fileBundleTreeView_.Location = new System.Drawing.Point(0, 0);
      this.fileBundleTreeView_.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.fileBundleTreeView_.Name = "fileBundleTreeView_";
      this.fileBundleTreeView_.Size = new System.Drawing.Size(224, 488);
      this.fileBundleTreeView_.TabIndex = 0;
      // 
      // splitContainer2
      // 
      splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      splitContainer2.Location = new System.Drawing.Point(0, 0);
      splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      splitContainer2.Panel1.Controls.Add(this.splitContainer4);
      // 
      // splitContainer2.Panel2
      // 
      splitContainer2.Panel2.Controls.Add(this.modelTabs_);
      splitContainer2.Size = new System.Drawing.Size(785, 488);
      splitContainer2.SplitterDistance = 573;
      splitContainer2.TabIndex = 1;
      // 
      // splitContainer4
      // 
      this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer4.IsSplitterFixed = true;
      this.splitContainer4.Location = new System.Drawing.Point(0, 0);
      this.splitContainer4.Name = "splitContainer4";
      this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer4.Panel1
      // 
      this.splitContainer4.Panel1.Controls.Add(this.sceneViewerPanel_);
      // 
      // splitContainer4.Panel2
      // 
      this.splitContainer4.Panel2.Controls.Add(this.audioPlayerPanel_);
      this.splitContainer4.Size = new System.Drawing.Size(573, 488);
      this.splitContainer4.SplitterDistance = 430;
      this.splitContainer4.TabIndex = 0;
      // 
      // modelViewerPanel_
      // 
      this.sceneViewerPanel_.Animation = null;
      this.sceneViewerPanel_.BackColor = System.Drawing.Color.Transparent;
      this.sceneViewerPanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.sceneViewerPanel_.Location = new System.Drawing.Point(0, 0);
      this.sceneViewerPanel_.FileBundleAndScene = null;
      this.sceneViewerPanel_.Name = "sceneViewerPanel_";
      this.sceneViewerPanel_.Size = new System.Drawing.Size(573, 430);
      this.sceneViewerPanel_.TabIndex = 0;
      // 
      // audioPlayerPanel_
      // 
      this.audioPlayerPanel_.AudioFileBundles = null;
      this.audioPlayerPanel_.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.audioPlayerPanel_.BackColor = System.Drawing.Color.Transparent;
      this.audioPlayerPanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.audioPlayerPanel_.Location = new System.Drawing.Point(0, 0);
      this.audioPlayerPanel_.Name = "audioPlayerPanel_";
      this.audioPlayerPanel_.Size = new System.Drawing.Size(573, 54);
      this.audioPlayerPanel_.TabIndex = 2;
      // 
      // modelTabs_
      // 
      this.modelTabs_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.modelTabs_.Location = new System.Drawing.Point(0, 0);
      this.modelTabs_.Name = "modelTabs_";
      this.modelTabs_.Size = new System.Drawing.Size(208, 488);
      this.modelTabs_.TabIndex = 0;
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
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // helpToolStripMenuItem
      // 
      this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gitHubToolStripMenuItem,
            this.reportAnIssueToolStripMenuItem});
      this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
      this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.helpToolStripMenuItem.Text = "Help";
      // 
      // gitHubToolStripMenuItem
      // 
      this.gitHubToolStripMenuItem.Name = "gitHubToolStripMenuItem";
      this.gitHubToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.gitHubToolStripMenuItem.Text = "View on GitHub";
      this.gitHubToolStripMenuItem.Click += new System.EventHandler(this.gitHubToolStripMenuItem_Click);
      // 
      // reportAnIssueToolStripMenuItem
      // 
      this.reportAnIssueToolStripMenuItem.Name = "reportAnIssueToolStripMenuItem";
      this.gitHubToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.reportAnIssueToolStripMenuItem.Text = "Report an Issue";
      this.reportAnIssueToolStripMenuItem.Click += new System.EventHandler(this.reportAnIssueToolStripMenuItem_Click);
      // 
      // splitContainer3
      // 
      splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      splitContainer3.IsSplitterFixed = true;
      splitContainer3.Location = new System.Drawing.Point(0, 24);
      splitContainer3.Name = "splitContainer3";
      splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer3.Panel1
      // 
      splitContainer3.Panel1.Controls.Add(this.modelToolStrip_);
      splitContainer3.Panel1MinSize = 28;
      // 
      // splitContainer3.Panel2
      // 
      splitContainer3.Panel2.Controls.Add(splitContainer1);
      splitContainer3.Size = new System.Drawing.Size(1013, 520);
      splitContainer3.SplitterDistance = 28;
      splitContainer3.TabIndex = 4;
      // 
      // modelToolStrip_
      // 
      this.modelToolStrip_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.modelToolStrip_.Location = new System.Drawing.Point(0, 0);
      this.modelToolStrip_.Name = "modelToolStrip_";
      this.modelToolStrip_.Size = new System.Drawing.Size(1013, 28);
      this.modelToolStrip_.TabIndex = 3;
      // 
      // UniversalModelExtractorForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1013, 544);
      this.Controls.Add(splitContainer3);
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
      splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
      splitContainer2.ResumeLayout(false);
      this.splitContainer4.Panel1.ResumeLayout(false);
      this.splitContainer4.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
      this.splitContainer4.ResumeLayout(false);
      menuStrip.ResumeLayout(false);
      menuStrip.PerformLayout();
      splitContainer3.Panel1.ResumeLayout(false);
      splitContainer3.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer3)).EndInit();
      splitContainer3.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private SceneViewerPanel sceneViewerPanel_;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem gitHubToolStripMenuItem;
    private ToolStripMenuItem reportAnIssueToolStripMenuItem;
    private SplitContainer splitContainer1;
    private FileBundleTreeView fileBundleTreeView_;
    private ModelTabs modelTabs_;
    private top.ModelToolStrip modelToolStrip_;
    private SplitContainer splitContainer4;
    private AudioPlayerPanel audioPlayerPanel_;
  }
}