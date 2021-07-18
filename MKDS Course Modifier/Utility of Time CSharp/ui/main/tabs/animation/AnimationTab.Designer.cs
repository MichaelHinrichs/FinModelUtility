namespace UoT.ui.main.tabs.animation {
  partial class AnimationTab {
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
      System.Windows.Forms.SplitContainer splitContainer_;
      this.animationSelectorPanel_ = new UoT.ui.main.tabs.animation.AnimationSelectorPanel();
      this.animationPlaybackPanel_ = new UoT.ui.main.tabs.animation.AnimationPlaybackPanel();
      splitContainer_ = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(splitContainer_)).BeginInit();
      splitContainer_.Panel1.SuspendLayout();
      splitContainer_.Panel2.SuspendLayout();
      splitContainer_.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer_
      // 
      splitContainer_.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer_.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      splitContainer_.IsSplitterFixed = true;
      splitContainer_.Location = new System.Drawing.Point(0, 0);
      splitContainer_.Name = "splitContainer_";
      splitContainer_.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer_.Panel1
      // 
      splitContainer_.Panel1.Controls.Add(this.animationSelectorPanel_);
      // 
      // splitContainer_.Panel2
      // 
      splitContainer_.Panel2.Controls.Add(this.animationPlaybackPanel_);
      splitContainer_.Panel2MinSize = 147;
      splitContainer_.Size = new System.Drawing.Size(236, 458);
      splitContainer_.SplitterDistance = 307;
      splitContainer_.TabIndex = 16;
      // 
      // animationSelectorPanel_
      // 
      this.animationSelectorPanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.animationSelectorPanel_.Location = new System.Drawing.Point(0, 0);
      this.animationSelectorPanel_.Name = "animationSelectorPanel_";
      this.animationSelectorPanel_.Size = new System.Drawing.Size(236, 307);
      this.animationSelectorPanel_.TabIndex = 11;
      // 
      // animationPlaybackPanel_
      // 
      this.animationPlaybackPanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.animationPlaybackPanel_.Frame = 0D;
      this.animationPlaybackPanel_.FrameRate = 20;
      this.animationPlaybackPanel_.IsPlaying = false;
      this.animationPlaybackPanel_.Location = new System.Drawing.Point(0, 0);
      this.animationPlaybackPanel_.Name = "animationPlaybackPanel_";
      this.animationPlaybackPanel_.ShouldLoop = false;
      this.animationPlaybackPanel_.Size = new System.Drawing.Size(236, 147);
      this.animationPlaybackPanel_.TabIndex = 14;
      this.animationPlaybackPanel_.TotalFrames = 0;
      // 
      // AnimationTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(splitContainer_);
      this.Name = "AnimationTab";
      this.Size = new System.Drawing.Size(236, 458);
      splitContainer_.Panel1.ResumeLayout(false);
      splitContainer_.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer_)).EndInit();
      splitContainer_.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private AnimationSelectorPanel animationSelectorPanel_;
    private AnimationPlaybackPanel animationPlaybackPanel_;
  }
}
