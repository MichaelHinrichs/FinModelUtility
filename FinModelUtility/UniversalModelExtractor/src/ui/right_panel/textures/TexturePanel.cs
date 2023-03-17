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

    private void pictureBox__Click(object sender, EventArgs e) {
      if (this.pictureBox_.Image != null && 
          e is MouseEventArgs { Button: MouseButtons.Right } mouseEventArgs) {
        var contextMenu = new ContextMenuStrip();

        var copyImageButton = contextMenu.Items.Add("Copy image");
        copyImageButton.Click +=
            (s, e) => Clipboard.SetImage(this.pictureBox_.Image);
        
        contextMenu.Show(this.pictureBox_, mouseEventArgs.Location);
      }
    }
  }
}