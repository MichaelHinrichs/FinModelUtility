namespace uni.ui.right_panel.materials {
  partial class ShaderSection {
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
      System.Windows.Forms.GroupBox groupBox;
      this.richTextBox_ = new System.Windows.Forms.RichTextBox();
      groupBox = new System.Windows.Forms.GroupBox();
      groupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox
      // 
      groupBox.Controls.Add(this.richTextBox_);
      groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      groupBox.Location = new System.Drawing.Point(0, 0);
      groupBox.Name = "groupBox";
      groupBox.Size = new System.Drawing.Size(367, 305);
      groupBox.TabIndex = 0;
      groupBox.TabStop = false;
      groupBox.Text = "Shader";
      // 
      // richTextBox_
      // 
      this.richTextBox_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.richTextBox_.Location = new System.Drawing.Point(3, 19);
      this.richTextBox_.Name = "richTextBox_";
      this.richTextBox_.Size = new System.Drawing.Size(361, 283);
      this.richTextBox_.TabIndex = 0;
      this.richTextBox_.Text = "";
      // 
      // ShaderSection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(groupBox);
      this.Name = "ShaderSection";
      this.Size = new System.Drawing.Size(367, 305);
      groupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private RichTextBox richTextBox_;
  }
}
