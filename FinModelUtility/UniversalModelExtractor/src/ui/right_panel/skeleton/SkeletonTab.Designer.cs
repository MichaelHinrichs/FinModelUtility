namespace uni.ui.right_panel {
  partial class SkeletonTab {
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
      this.listView_ = new System.Windows.Forms.ListView();
      this.SuspendLayout();
      // 
      // listView_
      // 
      this.listView_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView_.Location = new System.Drawing.Point(0, 0);
      this.listView_.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.listView_.MultiSelect = false;
      this.listView_.Name = "listView_";
      this.listView_.Size = new System.Drawing.Size(280, 451);
      this.listView_.TabIndex = 1;
      this.listView_.UseCompatibleStateImageBehavior = false;
      this.listView_.View = System.Windows.Forms.View.List;
      // 
      // SkeletonTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.listView_);
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "SkeletonTab";
      this.Size = new System.Drawing.Size(280, 451);
      this.ResumeLayout(false);

    }

        #endregion

        private ListView listView_;
    }
}
