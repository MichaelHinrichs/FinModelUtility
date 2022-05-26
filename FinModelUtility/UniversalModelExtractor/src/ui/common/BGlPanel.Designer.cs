namespace uni.ui.common {
  abstract partial class BGlPanel {
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
      this.impl_ = new UoT.Tao.Platform.Windows.SimpleOpenGlControl();
      this.SuspendLayout();
      // 
      // impl_
      // 
      this.impl_.AccumBits = ((byte)(0));
      this.impl_.AutoCheckErrors = false;
      this.impl_.AutoFinish = false;
      this.impl_.AutoMakeCurrent = true;
      this.impl_.AutoSwapBuffers = true;
      this.impl_.BackColor = System.Drawing.Color.Black;
      this.impl_.ColorBits = ((byte)(32));
      this.impl_.DepthBits = ((byte)(16));
      this.impl_.Dock = System.Windows.Forms.DockStyle.Fill;
      this.impl_.Location = new System.Drawing.Point(0, 0);
      this.impl_.Name = "impl_";
      this.impl_.Size = new System.Drawing.Size(511, 331);
      this.impl_.StencilBits = ((byte)(0));
      this.impl_.TabIndex = 0;
      // 
      // BGlPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Fuchsia;
      this.Controls.Add(this.impl_);
      this.Name = "BGlPanel";
      this.Size = new System.Drawing.Size(511, 331);
      this.ResumeLayout(false);

    }

    #endregion

    protected UoT.Tao.Platform.Windows.SimpleOpenGlControl impl_;
  }
}
