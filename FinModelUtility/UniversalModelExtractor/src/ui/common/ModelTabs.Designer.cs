namespace uni.ui.common {
  partial class ModelTabs {
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
      System.Windows.Forms.TabControl tabControl;
      System.Windows.Forms.TabPage infoTabPage;
      System.Windows.Forms.TabPage texturesTabPage;
      this.animationsTabPage = new System.Windows.Forms.TabPage();
      this.shaderTabPage = new System.Windows.Forms.TabPage();
      tabControl = new System.Windows.Forms.TabControl();
      infoTabPage = new System.Windows.Forms.TabPage();
      texturesTabPage = new System.Windows.Forms.TabPage();
      tabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      tabControl.Controls.Add(infoTabPage);
      tabControl.Controls.Add(texturesTabPage);
      tabControl.Controls.Add(this.animationsTabPage);
      tabControl.Controls.Add(this.shaderTabPage);
      tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      tabControl.Location = new System.Drawing.Point(0, 0);
      tabControl.Multiline = true;
      tabControl.Name = "tabControl";
      tabControl.SelectedIndex = 0;
      tabControl.Size = new System.Drawing.Size(216, 435);
      tabControl.TabIndex = 0;
      // 
      // infoTabPage
      // 
      infoTabPage.Location = new System.Drawing.Point(4, 44);
      infoTabPage.Name = "infoTabPage";
      infoTabPage.Padding = new System.Windows.Forms.Padding(3);
      infoTabPage.Size = new System.Drawing.Size(208, 387);
      infoTabPage.TabIndex = 0;
      infoTabPage.Text = "Info";
      infoTabPage.UseVisualStyleBackColor = true;
      // 
      // texturesTabPage
      // 
      texturesTabPage.Location = new System.Drawing.Point(4, 44);
      texturesTabPage.Name = "texturesTabPage";
      texturesTabPage.Padding = new System.Windows.Forms.Padding(3);
      texturesTabPage.Size = new System.Drawing.Size(208, 387);
      texturesTabPage.TabIndex = 1;
      texturesTabPage.Text = "Textures";
      texturesTabPage.UseVisualStyleBackColor = true;
      // 
      // animationsTabPage
      // 
      this.animationsTabPage.Location = new System.Drawing.Point(4, 44);
      this.animationsTabPage.Name = "animationsTabPage";
      this.animationsTabPage.Size = new System.Drawing.Size(208, 387);
      this.animationsTabPage.TabIndex = 2;
      this.animationsTabPage.Text = "Animations";
      this.animationsTabPage.UseVisualStyleBackColor = true;
      // 
      // shaderTabPage
      // 
      this.shaderTabPage.Location = new System.Drawing.Point(4, 44);
      this.shaderTabPage.Name = "shaderTabPage";
      this.shaderTabPage.Size = new System.Drawing.Size(208, 387);
      this.shaderTabPage.TabIndex = 3;
      this.shaderTabPage.Text = "Shader";
      this.shaderTabPage.UseVisualStyleBackColor = true;
      // 
      // ModelTabs
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(tabControl);
      this.Name = "ModelTabs";
      this.Size = new System.Drawing.Size(216, 435);
      tabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private TabControl tabControl;
    private TabPage infoTabPage;
    private TabPage texturesTabPage;
    private TabPage animationsTabPage;
    private TabPage shaderTabPage;
  }
}
