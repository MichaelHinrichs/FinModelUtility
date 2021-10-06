using System;
using System.Collections.Generic;
using System.Windows.Forms;

using UoT.animation.banks;
using UoT.animation.playback;
using UoT.util;

namespace UoT.ui.main.tabs.animation {
  public partial class AnimationTab : UserControl {
    public AnimationTab() => this.InitializeComponent();

    public IAnimationBanks AnimationBanks => this.animationSelectorPanel_;
    public event Action<IAnimation?> AnimationSelected {
      add => this.AnimationBanks.AnimationSelected += value;
      remove => this.AnimationBanks.AnimationSelected -= value;
    }

    public IAnimationPlaybackManager AnimationPlaybackManager
      => this.animationPlaybackPanel_;
  }
}