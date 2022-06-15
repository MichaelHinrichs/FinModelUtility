using fin.model;


namespace uni.ui.right_panel.textures {
  public partial class TexturesTab : UserControl {
    public TexturesTab() {
      InitializeComponent();
    }

    public IModel Model {
      set {
        this.listView_.Clear();

        var imageList = this.listView_.LargeImageList = new ImageList();

        var textures =
            value.MaterialManager.All.SelectMany(material => material.Textures)
                 .OrderBy(texture => texture.Name);

        foreach (var texture in textures) {
          this.listView_.Items.Add(texture.Name, imageList.Images.Count);
          imageList.Images.Add(texture.ImageData);
        }
      }
    }
  }
}