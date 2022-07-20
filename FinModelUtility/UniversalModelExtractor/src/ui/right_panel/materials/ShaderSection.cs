using fin.language.equations.fixedFunction;
using fin.model;


namespace uni.ui.right_panel.materials {
  public partial class ShaderSection : UserControl {
    public ShaderSection() {
      InitializeComponent();
    }

    public IMaterial? Material {
      set {
        var shaderText = "(n/a)";
        if (value is IFixedFunctionMaterial fixedFunctionMaterial) {
          shaderText = new FixedFunctionEquationsGlslPrinter(
                  fixedFunctionMaterial.TextureSources)
              .Print(fixedFunctionMaterial);
        }

        this.richTextBox_.Text = shaderText;
      }
    }
  }
}
