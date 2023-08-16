using fin.model;

using uni.ui.common;

namespace uni.ui.right_panel.textures {
  public partial class TexturePanel : UserControl {
    public TexturePanel() {
      InitializeComponent();

      this.Texture = null;

      this.pictureBox_.Click += ContextMenuUtils.CreateRightClickEventHandler(
          this.GenerateContextMenuItems_);
    }

    public ITexture? Texture {
      set {
        this.groupBox_.Text = value?.Name ?? "(Select a texture)";
        this.pictureBox_.Image = value?.ImageData;
      }
    }

    private IEnumerable<(string, Action)> GenerateContextMenuItems_() {
      if (this.pictureBox_.Image != null) {
        yield return ("Copy image", this.CopyImageToClipboard_);
      }
    }

    private void CopyImageToClipboard_()
      => Clipboard.SetImage(this.pictureBox_.Image);
  }
}