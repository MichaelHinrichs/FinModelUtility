namespace UoT.ui.main.files {
  partial class ZFileTreeView {
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
      System.Windows.Forms.SplitContainer splitContainer;
      this.filterTextBox_ = new UoT.WaterMarkTextBox();
      this.fileTreeView_ = new System.Windows.Forms.TreeView();
      splitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).BeginInit();
      splitContainer.Panel1.SuspendLayout();
      splitContainer.Panel2.SuspendLayout();
      splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      splitContainer.IsSplitterFixed = true;
      splitContainer.Location = new System.Drawing.Point(0, 0);
      splitContainer.Name = "splitContainer";
      splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      splitContainer.Panel1.Controls.Add(this.filterTextBox_);
      splitContainer.Panel1MinSize = 20;
      // 
      // splitContainer.Panel2
      // 
      splitContainer.Panel2.Controls.Add(this.fileTreeView_);
      splitContainer.Size = new System.Drawing.Size(225, 627);
      splitContainer.SplitterDistance = 25;
      splitContainer.TabIndex = 0;
      // 
      // filterTextBox_
      // 
      this.filterTextBox_.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.filterTextBox_.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
      this.filterTextBox_.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
      this.filterTextBox_.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.filterTextBox_.Location = new System.Drawing.Point(0, 4);
      this.filterTextBox_.Name = "filterTextBox_";
      this.filterTextBox_.Size = new System.Drawing.Size(225, 20);
      this.filterTextBox_.TabIndex = 1;
      this.filterTextBox_.WaterMarkColor = System.Drawing.Color.Gray;
      this.filterTextBox_.WaterMarkText = "Filter files...";
      // 
      // fileTreeView_
      // 
      this.fileTreeView_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.fileTreeView_.HideSelection = false;
      this.fileTreeView_.HotTracking = true;
      this.fileTreeView_.Location = new System.Drawing.Point(0, 0);
      this.fileTreeView_.Name = "fileTreeView_";
      this.fileTreeView_.Size = new System.Drawing.Size(225, 598);
      this.fileTreeView_.TabIndex = 15;
      // 
      // ZFileTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(splitContainer);
      this.Name = "ZFileTreeView";
      this.Size = new System.Drawing.Size(225, 627);
      splitContainer.Panel1.ResumeLayout(false);
      splitContainer.Panel1.PerformLayout();
      splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).EndInit();
      splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TreeView fileTreeView_;
    private WaterMarkTextBox filterTextBox_;
  }
}
