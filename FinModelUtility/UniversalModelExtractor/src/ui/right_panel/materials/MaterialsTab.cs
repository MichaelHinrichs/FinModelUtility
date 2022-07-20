using fin.model;


namespace uni.ui.right_panel.materials {
  public partial class MaterialsTab : UserControl {
    public MaterialsTab() {
      InitializeComponent();

      this.materialSelector_.OnMaterialSelected += material => {
        this.shaderSection_.Material = material;
      };
    }

    public IReadOnlyList<IMaterial> Materials {
      set => this.materialSelector_.Materials = value;
    }
  }
}