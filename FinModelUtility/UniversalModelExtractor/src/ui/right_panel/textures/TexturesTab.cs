using fin.model;


namespace uni.ui.right_panel.textures {
  public partial class TexturesTab : UserControl {
    private IList<ITexture> textures_;

    public TexturesTab() {
      InitializeComponent();

      this.listView_.ItemSelectionChanged += (_, e) => {
        var selectedTexture = this.textures_[e.ItemIndex];
        this.texturePanel_.Texture = selectedTexture;
      };
    }

    public IModel Model {
      set {
        this.listView_.Clear();
        this.texturePanel_.Texture = null;

        var imageList = this.listView_.LargeImageList = new ImageList();

        this.textures_ =
            value.MaterialManager.All.SelectMany(material => material.Textures)
                 .ToHashSet(new TextureEqualityComparer())
                 .OrderBy(texture => texture.Name)
                 .ToList();

        foreach (var texture in this.textures_) {
          this.listView_.Items.Add(texture.Name, imageList.Images.Count);
          imageList.Images.Add(texture.ImageData);
        }
      }
    }

    private class TextureEqualityComparer : IEqualityComparer<ITexture> {
      public bool Equals(ITexture x, ITexture y) {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.ImageData.Equals(y.ImageData);
      }

      public int GetHashCode(ITexture obj) {
        return obj.ImageData.GetHashCode();
      }
    }
  }
}