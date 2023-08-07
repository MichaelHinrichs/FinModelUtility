using fin.graphics.gl.material;
using fin.model;


namespace uni.ui.right_panel.materials {
  public partial class ShaderSection : UserControl {
    public ShaderSection() {
      InitializeComponent();
    }

    public (IModel, IMaterial)? ModelAndMaterial {
      set {
        if (value == null) {
          this.richTextBox_.Text = "(n/a)";
        } else {
          var (model, material) = value.Value;
          this.richTextBox_.Text =
              material.ToShaderSource(model, false).FragmentShaderSource;
        }
      }
    }
  }
}
