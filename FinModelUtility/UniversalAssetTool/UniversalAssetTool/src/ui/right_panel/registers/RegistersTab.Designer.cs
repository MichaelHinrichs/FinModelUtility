namespace uni.ui.right_panel.registers {
  partial class RegistersTab {
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
      this.tableLayoutPanel_ = new TableLayoutPanel();
      this.SuspendLayout();
      // 
      // tableLayoutPanel_
      // 
      this.tableLayoutPanel_.AutoSize = true;
      this.tableLayoutPanel_.ColumnCount = 2;
      this.tableLayoutPanel_.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
      this.tableLayoutPanel_.ColumnStyles.Add(new ColumnStyle());
      this.tableLayoutPanel_.Dock = DockStyle.Top;
      this.tableLayoutPanel_.Location = new Point(0, 0);
      this.tableLayoutPanel_.Name = "tableLayoutPanel_";
      this.tableLayoutPanel_.RowCount = 1;
      this.tableLayoutPanel_.RowStyles.Add(new RowStyle());
      this.tableLayoutPanel_.Size = new Size(280, 0);
      this.tableLayoutPanel_.TabIndex = 0;
      // 
      // RegistersTab
      // 
      this.AutoScaleDimensions = new SizeF(8F, 20F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoScroll = true;
      this.Controls.Add(this.tableLayoutPanel_);
      this.Margin = new Padding(3, 4, 3, 4);
      this.Name = "RegistersTab";
      this.Size = new Size(280, 451);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel_;
  }
}
