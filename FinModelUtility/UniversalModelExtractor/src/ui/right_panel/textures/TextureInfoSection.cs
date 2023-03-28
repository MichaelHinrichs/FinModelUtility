using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using fin.model;


namespace uni.ui.right_panel.textures {
  public partial class TextureInfoSection : UserControl {
    public TextureInfoSection() {
      this.InitializeComponent();

      this.SelectedTexture = null;
    }

    public ITexture? SelectedTexture {
      set => this.propertyGrid_.SelectedObject =
          new PropertyGridTexture(value);
    }

    private class PropertyGridTexture {
      private ITexture? impl_;

      public PropertyGridTexture(ITexture? impl) {
        this.impl_ = impl;
      }

      [Display(Order = 0)]
      [Category("Metadata")]
      public string? Name => this.impl_?.Name;

      [Display(Order = 1)]
      [Category("Metadata")]
      public int? Width => this.impl_?.Image.Width;
 
      [Display(Order = 2)]
      [Category("Metadata")]
      public int? Height => this.impl_?.Image.Height;
    }
  }
}