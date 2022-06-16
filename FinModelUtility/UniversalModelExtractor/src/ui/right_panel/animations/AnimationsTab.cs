using fin.animation.playback;
using fin.model;


namespace uni.ui.right_panel {
  public partial class AnimationsTab : UserControl {
    private IAnimation[] animations_;

    public AnimationsTab() {
      InitializeComponent();

      this.listView_.SelectedIndexChanged += (_, e) => {
        var selectedIndices = this.listView_.SelectedIndices;

        var selectedAnimation = selectedIndices.Count > 0
                                    ? this.animations_[selectedIndices[0]]
                                    : null;

        this.OnAnimationSelected?.Invoke(selectedAnimation);
      };
    }

    public IModel Model {
      set {
        this.listView_.SelectedIndices.Clear();
        this.listView_.Items.Clear();

        this.animations_ =
            value.AnimationManager.Animations.OrderBy(
                     animation => animation.Name)
                 .ToArray();

        foreach (var animation in this.animations_) {
          this.listView_.Items.Add(animation.Name);
        }

        if (this.listView_.Items.Count > 0) {
          this.listView_.Items[0].Selected = true;
        }
      }
    }

    public IAnimationPlaybackManager AnimationPlaybackManager =>
        this.animationPlaybackPanel_;

    public delegate void AnimationSelected(IAnimation? animation);

    public event AnimationSelected OnAnimationSelected;
  }
}