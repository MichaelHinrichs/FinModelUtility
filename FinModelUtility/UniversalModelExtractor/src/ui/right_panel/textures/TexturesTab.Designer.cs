namespace uni.ui.right_panel.textures {
  partial class TexturesTab {
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
      this.listView_ = new System.Windows.Forms.ListView();
      this.texturePanel_ = new uni.ui.right_panel.textures.TexturePanel();
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
      splitContainer.Location = new System.Drawing.Point(0, 0);
      splitContainer.Name = "splitContainer";
      splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      splitContainer.Panel1.Controls.Add(this.listView_);
      // 
      // splitContainer.Panel2
      // 
      splitContainer.Panel2.Controls.Add(this.texturePanel_);
      splitContainer.Size = new System.Drawing.Size(218, 366);
      splitContainer.SplitterDistance = 203;
      splitContainer.TabIndex = 0;
      // 
      // listView_
      // 
      this.listView_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listView_.Location = new System.Drawing.Point(0, 0);
      this.listView_.MultiSelect = false;
      this.listView_.Name = "listView_";
      this.listView_.Size = new System.Drawing.Size(218, 203);
      this.listView_.TabIndex = 1;
      this.listView_.UseCompatibleStateImageBehavior = false;
      this.listView_.View = System.Windows.Forms.View.List;
      // 
      // texturePanel_
      // 
      this.texturePanel_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.texturePanel_.Location = new System.Drawing.Point(0, 0);
      this.texturePanel_.Name = "texturePanel_";
      this.texturePanel_.Size = new System.Drawing.Size(218, 159);
      this.texturePanel_.TabIndex = 0;
      // 
      // TexturesTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(splitContainer);
      this.Name = "TexturesTab";
      this.Size = new System.Drawing.Size(218, 366);
      splitContainer.Panel1.ResumeLayout(false);
      splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).EndInit();
      splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private ListView listView_;
    private TexturePanel texturePanel_;
  }
}
