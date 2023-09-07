using fin.io.bundles;

namespace uni.ui.right_panel.info {
  public partial class InfoTab : UserControl {
    public InfoTab() {
      InitializeComponent();
    }

    public IAnnotatedFileBundle? FileBundle {
      set {
        var items = this.filesListBox_.Items;
        items.Clear();

        if (value == null) {
          return;
        }

        var files = value.FileBundle.Files.Select(file => file.DisplayFullPath)
                         .Distinct()
                         .Order();

        foreach (var file in files) {
          items.Add(file);
        }
      }
    }
  }
}