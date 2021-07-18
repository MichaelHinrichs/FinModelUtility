namespace UoT.ui.common.component.tabs {
  partial class HostPanel {
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
      this.HostGroup = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.HostGroup.SuspendLayout();
      this.SuspendLayout();
      // 
      // HostGroup
      // 
      this.HostGroup.Controls.Add(this.label1);
      this.HostGroup.Dock = System.Windows.Forms.DockStyle.Fill;
      this.HostGroup.Location = new System.Drawing.Point(0, 0);
      this.HostGroup.Name = "HostGroup";
      this.HostGroup.Size = new System.Drawing.Size(292, 162);
      this.HostGroup.TabIndex = 17;
      this.HostGroup.TabStop = false;
      this.HostGroup.Text = "(Empty)";
      this.HostGroup.Enter += new System.EventHandler(this.PlaybackGroup_Enter);
      // 
      // label1
      // 
      this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label1.Location = new System.Drawing.Point(3, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(286, 143);
      this.label1.TabIndex = 0;
      this.label1.Text = "Drag a";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // HostPanel
      // 
      this.AllowDrop = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.HostGroup);
      this.Name = "HostPanel";
      this.Size = new System.Drawing.Size(292, 162);
      this.HostGroup.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox HostGroup;
  }
}
