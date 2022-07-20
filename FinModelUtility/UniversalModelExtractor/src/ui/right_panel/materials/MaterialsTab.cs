using fin.model;


namespace uni.ui.right_panel.materials {
  public partial class MaterialsTab : UserControl {
    public MaterialsTab() {
      InitializeComponent();
    }

    public IReadOnlyList<IMaterial> Materials {
      set => this.materialSelector_.Materials = value;
    }
  }
}