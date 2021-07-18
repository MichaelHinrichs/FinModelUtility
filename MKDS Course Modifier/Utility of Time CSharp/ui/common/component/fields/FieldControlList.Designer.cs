namespace UoT.ui.common.component.fields {
  partial class FieldControlList {
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
      this.group_ = new System.Windows.Forms.GroupBox();
      this.fieldControlListContainer_ = new System.Windows.Forms.FlowLayoutPanel();
      this.group_.SuspendLayout();
      this.SuspendLayout();
      // 
      // group_
      // 
      this.group_.Controls.Add(this.fieldControlListContainer_);
      this.group_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.group_.Location = new System.Drawing.Point(0, 0);
      this.group_.Name = "group_";
      this.group_.Size = new System.Drawing.Size(266, 73);
      this.group_.TabIndex = 17;
      this.group_.TabStop = false;
      this.group_.Text = "Fields";
      // 
      // fieldControlListContainer_
      // 
      this.fieldControlListContainer_.AutoScroll = true;
      this.fieldControlListContainer_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.fieldControlListContainer_.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.fieldControlListContainer_.Location = new System.Drawing.Point(3, 16);
      this.fieldControlListContainer_.Name = "fieldControlListContainer_";
      this.fieldControlListContainer_.Size = new System.Drawing.Size(260, 54);
      this.fieldControlListContainer_.TabIndex = 0;
      // 
      // FieldControlList
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this.group_);
      this.Name = "FieldControlList";
      this.Size = new System.Drawing.Size(266, 73);
      this.group_.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.GroupBox group_;
    private System.Windows.Forms.FlowLayoutPanel fieldControlListContainer_;
  }
}
