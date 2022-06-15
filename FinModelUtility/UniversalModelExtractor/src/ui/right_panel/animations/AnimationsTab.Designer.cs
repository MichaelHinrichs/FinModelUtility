namespace uni.ui.right_panel {
  partial class AnimationsTab {
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
      this.animationPlaybackPanel_ = new uni.ui.right_panel.AnimationPlaybackPanel();
      this.SuspendLayout();
      // 
      // animationPlaybackPanel_
      // 
      this.animationPlaybackPanel_.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.animationPlaybackPanel_.Frame = 0D;
      this.animationPlaybackPanel_.FrameRate = 20;
      this.animationPlaybackPanel_.IsPlaying = false;
      this.animationPlaybackPanel_.Location = new System.Drawing.Point(0, 203);
      this.animationPlaybackPanel_.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.animationPlaybackPanel_.Name = "animationPlaybackPanel_";
      this.animationPlaybackPanel_.ShouldLoop = false;
      this.animationPlaybackPanel_.Size = new System.Drawing.Size(245, 135);
      this.animationPlaybackPanel_.TabIndex = 0;
      this.animationPlaybackPanel_.TotalFrames = 0;
      // 
      // AnimationsTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.animationPlaybackPanel_);
      this.Name = "AnimationsTab";
      this.Size = new System.Drawing.Size(245, 338);
      this.ResumeLayout(false);

    }

    #endregion

    private AnimationPlaybackPanel animationPlaybackPanel_;
  }
}
