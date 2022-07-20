﻿namespace uni.ui.right_panel {
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
      System.Windows.Forms.TabPage animationsTabPage;
      System.Windows.Forms.TabPage materialsTabPage;
      this.texturesTab_ = new uni.ui.right_panel.textures.TexturesTab();
      this.animationsTab_ = new uni.ui.right_panel.AnimationsTab();
      this.materialsTab_ = new uni.ui.right_panel.materials.MaterialsTab();
      tabControl = new System.Windows.Forms.TabControl();
      infoTabPage = new System.Windows.Forms.TabPage();
      texturesTabPage = new System.Windows.Forms.TabPage();
      animationsTabPage = new System.Windows.Forms.TabPage();
      materialsTabPage = new System.Windows.Forms.TabPage();
      tabControl.SuspendLayout();
      texturesTabPage.SuspendLayout();
      animationsTabPage.SuspendLayout();
      materialsTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      tabControl.Controls.Add(infoTabPage);
      tabControl.Controls.Add(texturesTabPage);
      tabControl.Controls.Add(animationsTabPage);
      tabControl.Controls.Add(materialsTabPage);
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
      texturesTabPage.Controls.Add(this.texturesTab_);
      texturesTabPage.Location = new System.Drawing.Point(4, 44);
      texturesTabPage.Name = "texturesTabPage";
      texturesTabPage.Padding = new System.Windows.Forms.Padding(3);
      texturesTabPage.Size = new System.Drawing.Size(208, 387);
      texturesTabPage.TabIndex = 1;
      texturesTabPage.Text = "Textures";
      texturesTabPage.UseVisualStyleBackColor = true;
      // 
      // texturesTab_
      // 
      this.texturesTab_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.texturesTab_.Location = new System.Drawing.Point(3, 3);
      this.texturesTab_.Name = "texturesTab_";
      this.texturesTab_.Size = new System.Drawing.Size(202, 381);
      this.texturesTab_.TabIndex = 0;
      // 
      // animationsTabPage
      // 
      animationsTabPage.Controls.Add(this.animationsTab_);
      animationsTabPage.Location = new System.Drawing.Point(4, 44);
      animationsTabPage.Name = "animationsTabPage";
      animationsTabPage.Size = new System.Drawing.Size(208, 387);
      animationsTabPage.TabIndex = 2;
      animationsTabPage.Text = "Animations";
      animationsTabPage.UseVisualStyleBackColor = true;
      // 
      // animationsTab_
      // 
      this.animationsTab_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.animationsTab_.Location = new System.Drawing.Point(0, 0);
      this.animationsTab_.Name = "animationsTab_";
      this.animationsTab_.Size = new System.Drawing.Size(208, 387);
      this.animationsTab_.TabIndex = 0;
      // 
      // materialsTabPage
      // 
      materialsTabPage.Controls.Add(this.materialsTab_);
      materialsTabPage.Location = new System.Drawing.Point(4, 44);
      materialsTabPage.Name = "materialsTabPage";
      materialsTabPage.Size = new System.Drawing.Size(208, 387);
      materialsTabPage.TabIndex = 3;
      materialsTabPage.Text = "Materials";
      materialsTabPage.UseVisualStyleBackColor = true;
      // 
      // materialsTab_
      // 
      this.materialsTab_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.materialsTab_.Location = new System.Drawing.Point(0, 0);
      this.materialsTab_.Name = "materialsTab_";
      this.materialsTab_.Size = new System.Drawing.Size(208, 387);
      this.materialsTab_.TabIndex = 0;
      // 
      // ModelTabs
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(tabControl);
      this.Name = "ModelTabs";
      this.Size = new System.Drawing.Size(216, 435);
      tabControl.ResumeLayout(false);
      texturesTabPage.ResumeLayout(false);
      animationsTabPage.ResumeLayout(false);
      materialsTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private TabControl tabControl;
    private TabPage infoTabPage;
    private TabPage texturesTabPage;
    private TabPage animationsTabPage;
    private TabPage materialsTabPage;
    private AnimationsTab animationsTab_;
    private textures.TexturesTab texturesTab_;
    private materials.MaterialsTab materialsTab_;
  }
}
