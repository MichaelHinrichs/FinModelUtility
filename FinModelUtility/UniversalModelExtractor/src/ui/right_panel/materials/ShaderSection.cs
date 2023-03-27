using fin.gl.material;
using fin.model;


namespace uni.ui.right_panel.materials {
  public partial class ShaderSection : UserControl {
    public ShaderSection() {
      InitializeComponent();
    }

    public IMaterial? Material {
      set {
        this.richTextBox_.Text =
            value?.ToShaderSource().FragmentShaderSource ?? "(n/a)";
      }
    }
  }
}
