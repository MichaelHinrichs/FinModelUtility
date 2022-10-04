using fin.model;


namespace uni.ui.right_panel.textures {
  public partial class TexturesTab : UserControl {
    public TexturesTab() {
      InitializeComponent();

      this.textureSelectorBox_.OnTextureSelected
          += texture => this.texturePanel_.Texture = texture;
    }

    public IModel Model {
      set => this.textureSelectorBox_.Textures = value.MaterialManager.Textures;
    }
  }
}