using fin.model;


namespace uni.ui.right_panel.materials {
  public partial class TextureSection : UserControl {
    public TextureSection() {
      InitializeComponent();

      this.textureSelectorBox_.OnTextureSelected
          += texture => this.texturePanel_.Texture = texture;
    }

    public IMaterial? Material {
      set => this.textureSelectorBox_.Textures =
                 value?.Textures ?? Array.Empty<ITexture>();
    }
  }
}