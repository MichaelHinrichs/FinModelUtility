using fin.animation.playback;
using fin.io.bundles;
using fin.model;


namespace uni.ui.right_panel {
  public partial class ModelTabs : UserControl {
    public ModelTabs() {
      InitializeComponent();
    }

    public (IFileBundle, IModel)? Model {
      set {
        var modelFileBundle = value?.Item1;
        var model = value?.Item2;

        this.infoTab_.FileBundle = modelFileBundle;
        this.animationsTab_.Model = model;
        this.materialsTab_.Materials = model?.MaterialManager.All;
        this.skeletonTab_.Model = model;
        this.texturesTab_.Model = model;
      }
    }

    public IAnimationPlaybackManager? AnimationPlaybackManager {
      get => this.animationsTab_.AnimationPlaybackManager;
      set => this.animationsTab_.AnimationPlaybackManager = value;
    }

    public event AnimationsTab.AnimationSelected OnAnimationSelected {
      add => this.animationsTab_.OnAnimationSelected += value;
      remove => this.animationsTab_.OnAnimationSelected -= value;
    }

    public event skeleton.SkeletonTab.BoneSelected OnBoneSelected {
      add => this.skeletonTab_.OnBoneSelected += value;
      remove => this.skeletonTab_.OnBoneSelected -= value;
    }
  }
}
