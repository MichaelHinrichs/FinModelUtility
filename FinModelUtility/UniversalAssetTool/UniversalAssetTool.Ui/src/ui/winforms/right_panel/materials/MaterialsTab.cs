using System.Collections.Generic;
using System.Windows.Forms;

using fin.model;

namespace uni.ui.winforms.right_panel.materials {
  public partial class MaterialsTab : UserControl {
    private IModel? model_;

    public MaterialsTab() {
      InitializeComponent();

      this.materialSelector_.OnMaterialSelected += material => {
        this.materialViewerPanel1.Material = material;
        this.shaderSection_.ModelAndMaterial =
            material != null ? (this.model_!, material) : null;
        this.textureSection_.Material = material;
      };
    }

    public (IModel, IReadOnlyList<IMaterial>)? ModelAndMaterials {
      set {
        this.model_ = value?.Item1;
        this.materialSelector_.Materials = value?.Item2;
      }
    }
  }
}