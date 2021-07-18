using System.Windows.Forms;
using System.Drawing;

namespace UoT.ui.main.top.help {
  public class HelpDropdown : ToolStripMenuItem {
    public HelpDropdown() {
      // TODO: width of 127?
      // TODO: Add the rest of the items.

      // Controls
      /*var controlsMenuItem = new ToolStripMenuItem();
      controlsMenuItem.ShortcutKeyDisplayString = "";
      //ControlsInfoToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.H), System.Windows.Forms.Keys);
      controlsMenuItem.Size = new Size(227, 22);
      controlsMenuItem.Text = "&Controls";

      // Separator
      var separator = new ToolStripSeparator();
      separator.Size = new System.Drawing.Size(224, 6);*/

      // About UoT
      var aboutMenuItem = new ToolStripMenuItem();
      aboutMenuItem.ShortcutKeyDisplayString = "";
      //AboutToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Q), System.Windows.Forms.Keys)
      aboutMenuItem.Size = new System.Drawing.Size(227, 22);
      aboutMenuItem.Text = "&About Utility of Time";
      aboutMenuItem.Click += (sender, args) => {
        var aboutBoxDialog = new AboutBoxDialog();
        aboutBoxDialog.Show();
        aboutBoxDialog.Focus();
      };

      // Search For Updates
      /*var searchForUpdatesMenuItem = new ToolStripMenuItem();
      //Me.SearchForUpdatesToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.U), System.Windows.Forms.Keys)
      searchForUpdatesMenuItem.Size = new Size(227, 22);
      searchForUpdatesMenuItem.Text = "Search for updates...";*/

      // Help
      //this.DropDownItems.AddRange(new ToolStripItem[] { controlsMenuItem, separator, aboutMenuItem, searchForUpdatesMenuItem});
      this.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
      this.Size = new Size(41, 17);
      this.Text = "&Help";
    }
  }
}
