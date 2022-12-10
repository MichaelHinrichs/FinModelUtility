using fin.model;


namespace uni.ui.right_panel {
  public partial class SkeletonTab : UserControl {
    private IBone[]? bones_;

    public SkeletonTab() {
      InitializeComponent();

      // TODO: Use a tree view instead
      this.listView_.SelectedIndexChanged += (_, e) => {
        var selectedIndices = this.listView_.SelectedIndices;

        var selectedBone = selectedIndices.Count > 0
                               ? this.bones_[selectedIndices[0]]
                               : null;

        this.OnBoneSelected?.Invoke(selectedBone);
      };
    }

    public IModel? Model {
      set {
        this.listView_.SelectedIndices.Clear();
        this.listView_.Items.Clear();

        var skeleton = value?.Skeleton;
        this.bones_ = skeleton?.SkipWhile(bone => bone == skeleton.Root)
                              .OrderBy(bone => bone.Name)
                              .ToArray();

        if (this.bones_ == null) {
          return;
        }

        foreach (var bone in this.bones_) {
          this.listView_.Items.Add(bone.Name);
        }
      }
    }

    public delegate void BoneSelected(IBone? bone);

    public event BoneSelected OnBoneSelected;
  }
}