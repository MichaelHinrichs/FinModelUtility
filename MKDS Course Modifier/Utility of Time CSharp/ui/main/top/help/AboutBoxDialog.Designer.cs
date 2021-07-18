using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace UoT.ui.main.top.help {
  public partial class AboutBoxDialog {

    // Form overrides dispose to clean up the component list.
    protected override void Dispose(bool disposing) {
      if (disposing && components is object) {
        components.Dispose();
      }

      base.Dispose(disposing);
    }


    // Required by the Windows Form Designer
    private System.ComponentModel.IContainer components;

    // NOTE: The following procedure is required by the Windows Form Designer
    // It can be modified using the Windows Form Designer.  
    // Do not modify it using the code editor.
    [DebuggerStepThrough()]
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBoxDialog));
      this.Button1 = new System.Windows.Forms.Button();
      this.Label3 = new System.Windows.Forms.Label();
      this._LinkLabel1 = new System.Windows.Forms.LinkLabel();
      this.GroupBox1 = new System.Windows.Forms.GroupBox();
      this._LinkLabel3 = new System.Windows.Forms.LinkLabel();
      this._LinkLabel2 = new System.Windows.Forms.LinkLabel();
      this.Label1 = new System.Windows.Forms.Label();
      this.GroupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // Button1
      // 
      this.Button1.Location = new System.Drawing.Point(14, 74);
      this.Button1.Name = "Button1";
      this.Button1.Size = new System.Drawing.Size(75, 23);
      this.Button1.TabIndex = 6;
      this.Button1.Text = "&OK";
      this.Button1.UseVisualStyleBackColor = true;
      // 
      // Label3
      // 
      this.Label3.AutoSize = true;
      this.Label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Label3.Location = new System.Drawing.Point(11, 15);
      this.Label3.Name = "Label3";
      this.Label3.Size = new System.Drawing.Size(107, 15);
      this.Label3.TabIndex = 8;
      this.Label3.Text = "Version 0.95 BETA";
      // 
      // _LinkLabel1
      // 
      this._LinkLabel1.AutoSize = true;
      this._LinkLabel1.Location = new System.Drawing.Point(12, 45);
      this._LinkLabel1.Name = "_LinkLabel1";
      this._LinkLabel1.Size = new System.Drawing.Size(79, 13);
      this._LinkLabel1.TabIndex = 10;
      this._LinkLabel1.TabStop = true;
      this._LinkLabel1.Text = "cooliscool.NET";
      this._LinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel1_LinkClicked_1);
      // 
      // GroupBox1
      // 
      this.GroupBox1.Controls.Add(this._LinkLabel3);
      this.GroupBox1.Controls.Add(this._LinkLabel2);
      this.GroupBox1.Location = new System.Drawing.Point(141, 9);
      this.GroupBox1.Name = "GroupBox1";
      this.GroupBox1.Size = new System.Drawing.Size(106, 88);
      this.GroupBox1.TabIndex = 11;
      this.GroupBox1.TabStop = false;
      this.GroupBox1.Text = "Useful links";
      // 
      // _LinkLabel3
      // 
      this._LinkLabel3.AutoSize = true;
      this._LinkLabel3.Location = new System.Drawing.Point(12, 59);
      this._LinkLabel3.Name = "_LinkLabel3";
      this._LinkLabel3.Size = new System.Drawing.Size(20, 13);
      this._LinkLabel3.TabIndex = 1;
      this._LinkLabel3.TabStop = true;
      this._LinkLabel3.Text = "Jul";
      this._LinkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel3_LinkClicked);
      // 
      // _LinkLabel2
      // 
      this._LinkLabel2.AutoSize = true;
      this._LinkLabel2.Location = new System.Drawing.Point(12, 25);
      this._LinkLabel2.Name = "_LinkLabel2";
      this._LinkLabel2.Size = new System.Drawing.Size(79, 13);
      this._LinkLabel2.TabIndex = 0;
      this._LinkLabel2.TabStop = true;
      this._LinkLabel2.Text = "Spinout\'s forum";
      this._LinkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel2_LinkClicked);
      // 
      // Label1
      // 
      this.Label1.AutoSize = true;
      this.Label1.Location = new System.Drawing.Point(95, 390);
      this.Label1.Name = "Label1";
      this.Label1.Size = new System.Drawing.Size(0, 13);
      this.Label1.TabIndex = 12;
      // 
      // AboutBoxDialog
      // 
      this.AcceptButton = this.Button1;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(262, 105);
      this.Controls.Add(this.Label1);
      this.Controls.Add(this.GroupBox1);
      this.Controls.Add(this._LinkLabel1);
      this.Controls.Add(this.Label3);
      this.Controls.Add(this.Button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBoxDialog";
      this.Padding = new System.Windows.Forms.Padding(9);
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Utility of Time";
      this.GroupBox1.ResumeLayout(false);
      this.GroupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    internal Button Button1;
    internal Label Label3;
    private LinkLabel _LinkLabel1;

    internal LinkLabel LinkLabel1 {
      [MethodImpl(MethodImplOptions.Synchronized)]
      get {
        return _LinkLabel1;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      set {
        if (_LinkLabel1 != null) {
          _LinkLabel1.LinkClicked -= LinkLabel1_LinkClicked_1;
        }

        _LinkLabel1 = value;
        if (_LinkLabel1 != null) {
          _LinkLabel1.LinkClicked += LinkLabel1_LinkClicked_1;
        }
      }
    }

    internal GroupBox GroupBox1;
    private LinkLabel _LinkLabel2;

    internal LinkLabel LinkLabel2 {
      [MethodImpl(MethodImplOptions.Synchronized)]
      get {
        return _LinkLabel2;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      set {
        if (_LinkLabel2 != null) {
          _LinkLabel2.LinkClicked -= LinkLabel2_LinkClicked;
        }

        _LinkLabel2 = value;
        if (_LinkLabel2 != null) {
          _LinkLabel2.LinkClicked += LinkLabel2_LinkClicked;
        }
      }
    }

    private LinkLabel _LinkLabel3;

    internal LinkLabel LinkLabel3 {
      [MethodImpl(MethodImplOptions.Synchronized)]
      get {
        return _LinkLabel3;
      }

      [MethodImpl(MethodImplOptions.Synchronized)]
      set {
        if (_LinkLabel3 != null) {
          _LinkLabel3.LinkClicked -= LinkLabel3_LinkClicked;
        }

        _LinkLabel3 = value;
        if (_LinkLabel3 != null) {
          _LinkLabel3.LinkClicked += LinkLabel3_LinkClicked;
        }
      }
    }

    internal Label Label1;
  }
}