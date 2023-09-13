using fin.model;
using fin.util.enumerables;

namespace uni.ui.right_panel.materials {
  public partial class TextureSection : UserControl {
    public TextureSection() {
      InitializeComponent();

      this.textureSelectorBox_.OnTextureSelected
          += texture => this.texturePanel_.Texture = texture;
    }

    public IMaterial? Material {
      set => this.textureSelectorBox_.Textures =
          ((value is IReadOnlyFixedFunctionMaterial fixedFunctionMaterial)
              ? fixedFunctionMaterial.TextureSources.Nonnull()
              : value?.Textures)?.ToArray() ?? Array.Empty<ITexture>();
    }
  }
}