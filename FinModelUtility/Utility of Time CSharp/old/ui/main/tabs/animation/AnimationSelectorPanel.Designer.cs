namespace UoT.ui.main.tabs.animation {
  partial class AnimationSelectorPanel {
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
      System.Windows.Forms.GroupBox group_;
      System.Windows.Forms.Label bankLabel_;
      this.showBonesCheckBox_ = new System.Windows.Forms.CheckBox();
      this.animationsListBox_ = new System.Windows.Forms.ListBox();
      this.bankComboBox_ = new System.Windows.Forms.ComboBox();
      group_ = new System.Windows.Forms.GroupBox();
      bankLabel_ = new System.Windows.Forms.Label();
      group_.SuspendLayout();
      this.SuspendLayout();
      // 
      // group_
      // 
      group_.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      group_.Controls.Add(bankLabel_);
      group_.Controls.Add(this.showBonesCheckBox_);
      group_.Controls.Add(this.animationsListBox_);
      group_.Controls.Add(this.bankComboBox_);
      group_.Dock = System.Windows.Forms.DockStyle.Fill;
      group_.Location = new System.Drawing.Point(0, 0);
      group_.Name = "group_";
      group_.Size = new System.Drawing.Size(216, 146);
      group_.TabIndex = 14;
      group_.TabStop = false;
      group_.Text = "Animation sets";
      // 
      // bankLabel_
      // 
      bankLabel_.AutoSize = true;
      bankLabel_.Location = new System.Drawing.Point(3, 20);
      bankLabel_.Name = "bankLabel_";
      bankLabel_.Size = new System.Drawing.Size(32, 13);
      bankLabel_.TabIndex = 10;
      bankLabel_.Text = "Bank";
      // 
      // showBonesCheckBox_
      // 
      this.showBonesCheckBox_.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showBonesCheckBox_.AutoSize = true;
      this.showBonesCheckBox_.Location = new System.Drawing.Point(6, 117);
      this.showBonesCheckBox_.Name = "showBonesCheckBox_";
      this.showBonesCheckBox_.Size = new System.Drawing.Size(85, 17);
      this.showBonesCheckBox_.TabIndex = 4;
      this.showBonesCheckBox_.Text = "Show bones";
      this.showBonesCheckBox_.UseVisualStyleBackColor = true;
      // 
      // animationsListBox_
      // 
      this.animationsListBox_.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.animationsListBox_.FormattingEnabled = true;
      this.animationsListBox_.Location = new System.Drawing.Point(6, 42);
      this.animationsListBox_.Name = "animationsListBox_";
      this.animationsListBox_.Size = new System.Drawing.Size(204, 69);
      this.animationsListBox_.TabIndex = 0;
      this.animationsListBox_.SelectedIndexChanged += new System.EventHandler(this.animationsListBox__SelectedIndexChanged);
      // 
      // bankComboBox_
      // 
      this.bankComboBox_.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.bankComboBox_.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.bankComboBox_.FormattingEnabled = true;
      this.bankComboBox_.Items.AddRange(new object[] {
            "Inline with object"});
      this.bankComboBox_.Location = new System.Drawing.Point(41, 16);
      this.bankComboBox_.Name = "bankComboBox_";
      this.bankComboBox_.Size = new System.Drawing.Size(169, 21);
      this.bankComboBox_.TabIndex = 9;
      this.bankComboBox_.SelectedIndexChanged += new System.EventHandler(this.bankComboBox__SelectedIndexChanged_);
      // 
      // AnimationSetsPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(group_);
      this.Name = "AnimationSetsPanel";
      this.Size = new System.Drawing.Size(216, 146);
      group_.ResumeLayout(false);
      group_.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.CheckBox showBonesCheckBox_;
    private System.Windows.Forms.ListBox animationsListBox_;
    private System.Windows.Forms.ComboBox bankComboBox_;
  }
}
