using uni.ui.right_panel.textures;

namespace uni.ui.right_panel.materials {
  partial class MaterialsTab {
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
      this.materialSelector_ = new uni.ui.right_panel.materials.MaterialSelector();
      splitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).BeginInit();
      splitContainer.Panel1.SuspendLayout();
      splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      splitContainer.Location = new System.Drawing.Point(0, 0);
      splitContainer.Name = "splitContainer";
      splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      splitContainer.Panel1.Controls.Add(this.materialSelector_);
      splitContainer.Panel1MinSize = 23;
      splitContainer.Size = new System.Drawing.Size(218, 366);
      splitContainer.SplitterDistance = 23;
      splitContainer.TabIndex = 0;
      // 
      // materialSelector_
      // 
      this.materialSelector_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.materialSelector_.Location = new System.Drawing.Point(0, 0);
      this.materialSelector_.Materials = new fin.model.IMaterial[0];
      this.materialSelector_.Name = "materialSelector_";
      this.materialSelector_.Size = new System.Drawing.Size(218, 23);
      this.materialSelector_.TabIndex = 0;
      // 
      // MaterialsTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(splitContainer);
      this.Name = "MaterialsTab";
      this.Size = new System.Drawing.Size(218, 366);
      splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).EndInit();
      splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private MaterialSelector materialSelector_;
  }
}
