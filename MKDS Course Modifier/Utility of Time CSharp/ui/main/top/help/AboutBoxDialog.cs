using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace UoT.ui.main.top.help {
  public partial class AboutBoxDialog : Form {
    public AboutBoxDialog() {
      this.InitializeComponent();
      this._LinkLabel1.Name = "LinkLabel1";
      this._LinkLabel3.Name = "LinkLabel3";
      this._LinkLabel2.Name = "LinkLabel2";
    }

    private void OKButton_Click(object sender, EventArgs e) {
      this.Dispose();
    }

    private void LinkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start("mailto:cooliscool@msn.com");
    }

    private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start("http://z64.spinout182.com/index.php");
    }

    private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      Process.Start("http://www.jul.rustedlogic.net");
    }
  }
}
