using fin.model;


namespace uni.ui.right_panel.textures {
  public partial class TexturePanel : UserControl {
    public TexturePanel() {
      InitializeComponent();

      this.Texture = null;
    }

    public ITexture? Texture {
      set {
        this.groupBox_.Text = value?.Name ?? "(Select a texture)";
        this.pictureBox_.Image = value?.ImageData;
      }
    }
  }
}
