namespace UoT.ui.common.component.fields {
  partial class DiscreteFieldControl {
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
      this.dropdown_ = new System.Windows.Forms.ComboBox();
      this.group_ = new System.Windows.Forms.GroupBox();
      this.group_.SuspendLayout();
      this.SuspendLayout();
      // 
      // dropdown_
      // 
      this.dropdown_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dropdown_.FormattingEnabled = true;
      this.dropdown_.Location = new System.Drawing.Point(3, 16);
      this.dropdown_.Name = "dropdown_";
      this.dropdown_.Size = new System.Drawing.Size(286, 21);
      this.dropdown_.TabIndex = 0;
      this.dropdown_.SelectedIndexChanged += new System.EventHandler(this.dropdown_SelectedIndexChanged_);
      // 
      // group_
      // 
      this.group_.Controls.Add(this.dropdown_);
      this.group_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.group_.Location = new System.Drawing.Point(0, 0);
      this.group_.Name = "group_";
      this.group_.Size = new System.Drawing.Size(292, 40);
      this.group_.TabIndex = 17;
      this.group_.TabStop = false;
      this.group_.Text = "[Field Name]";
      // 
      // DiscreteFieldControl
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this.group_);
      this.MinimumSize = new System.Drawing.Size(0, 40);
      this.Name = "DiscreteFieldControl";
      this.Size = new System.Drawing.Size(292, 40);
      this.group_.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.ComboBox dropdown_;
    private System.Windows.Forms.GroupBox group_;
  }
}
