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
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.shaderSection_ = new uni.ui.right_panel.materials.ShaderSection();
      this.textureSection_ = new uni.ui.right_panel.materials.TextureSection();
      splitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).BeginInit();
      splitContainer.Panel1.SuspendLayout();
      splitContainer.Panel2.SuspendLayout();
      splitContainer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
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
      splitContainer.Panel1.Controls.Add(this.materialSelector_);
      splitContainer.Panel1MinSize = 23;
      // 
      // splitContainer.Panel2
      // 
      splitContainer.Panel2.Controls.Add(this.splitContainer1);
      splitContainer.Size = new System.Drawing.Size(296, 456);
      splitContainer.SplitterDistance = 25;
      splitContainer.TabIndex = 0;
      // 
      // materialSelector_
      // 
      this.materialSelector_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.materialSelector_.Location = new System.Drawing.Point(0, 0);
      this.materialSelector_.Materials = new fin.model.IMaterial[0];
      this.materialSelector_.Name = "materialSelector_";
      this.materialSelector_.SelectedMaterial = null;
      this.materialSelector_.Size = new System.Drawing.Size(296, 25);
      this.materialSelector_.TabIndex = 0;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
      this.splitContainer1.Size = new System.Drawing.Size(296, 427);
      this.splitContainer1.SplitterDistance = 106;
      this.splitContainer1.TabIndex = 0;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.textureSection_);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.shaderSection_);
      this.splitContainer2.Size = new System.Drawing.Size(296, 317);
      this.splitContainer2.SplitterDistance = 155;
      this.splitContainer2.TabIndex = 0;
      // 
      // shaderSection_
      // 
      this.shaderSection_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.shaderSection_.Location = new System.Drawing.Point(0, 0);
      this.shaderSection_.Name = "shaderSection_";
      this.shaderSection_.Size = new System.Drawing.Size(296, 158);
      this.shaderSection_.TabIndex = 0;
      // 
      // textureSection_
      // 
      this.textureSection_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textureSection_.Location = new System.Drawing.Point(0, 0);
      this.textureSection_.Name = "textureSection_";
      this.textureSection_.Size = new System.Drawing.Size(296, 155);
      this.textureSection_.TabIndex = 0;
      // 
      // MaterialsTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(splitContainer);
      this.Name = "MaterialsTab";
      this.Size = new System.Drawing.Size(296, 456);
      splitContainer.Panel1.ResumeLayout(false);
      splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(splitContainer)).EndInit();
      splitContainer.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private MaterialSelector materialSelector_;
    private SplitContainer splitContainer1;
    private SplitContainer splitContainer2;
    private ShaderSection shaderSection_;
    private TextureSection textureSection_;
  }
}
