using fin.model;


namespace uni.ui.right_panel.skeleton {
  public partial class SkeletonTab : UserControl {
    private IBone[]? bones_;

    public SkeletonTab() {
      InitializeComponent();

      this.skeletonTreeView_.BoneSelected += boneNode =>
          this.OnBoneSelected?.Invoke(boneNode.Bone);
    }

    public IModel? Model {
      set => this.skeletonTreeView_.Populate(value?.Skeleton);
    }

    public delegate void BoneSelected(IBone? bone);

    public event BoneSelected OnBoneSelected;
  }
}