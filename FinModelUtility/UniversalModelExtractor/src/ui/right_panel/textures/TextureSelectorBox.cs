using fin.model;
using fin.util.lists;


namespace uni.ui.right_panel.textures {
  public partial class TextureSelectorBox : UserControl {
    private IReadOnlyList<ITexture> textures_;
    private ITexture? selectedTexture_;

    public TextureSelectorBox() {
      InitializeComponent();

      this.listView_.ItemSelectionChanged += (_, e) => {
        this.SelectedTexture = this.textures_[e.ItemIndex];
      };
    }

    public IReadOnlyList<ITexture> Textures {
      set {
        this.listView_.Clear();

        var imageList = this.listView_.SmallImageList = new ImageList();

        this.textures_ =
            value.ToHashSet(new TextureEqualityComparer())
                 .OrderBy(texture => texture.Name)
                 .ToList();

        foreach (var texture in this.textures_) {
          this.listView_.Items.Add(texture.Name, imageList.Images.Count);
          imageList.Images.Add(texture.ImageData);
        }

        this.SelectedTexture =
            this.textures_.Count > 0 ? this.textures_[0] : null;
      }
    }

    public ITexture? SelectedTexture {
      get => this.selectedTexture_;
      set {
        if (this.selectedTexture_ == value) {
          return;
        }

        var selectedIndices = this.listView_.SelectedIndices;
        selectedIndices.Clear();
        if (value != null) {
          selectedIndices.Add(ListUtil.AssertFindFirst(this.textures_, value));
        }

        this.OnTextureSelected(this.selectedTexture_ = value);
      }
    }

    public delegate void OnTextureSelectedHandler(ITexture? texture);

    public event OnTextureSelectedHandler OnTextureSelected = delegate { };

    private class TextureEqualityComparer : IEqualityComparer<ITexture> {
      public bool Equals(ITexture x, ITexture y) {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Image.Equals(y.Image);
      }

      public int GetHashCode(ITexture obj) {
        return obj.Image.GetHashCode();
      }
    }
  }
}