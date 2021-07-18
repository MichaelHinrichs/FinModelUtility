using System;
using System.Collections.Generic;

using UoT.limbs;

namespace UoT.animation.banks {
  public interface IAnimationBanks {
    void Populate(IReadOnlyList<IAnimationBank> banks);
    void Reset(string filename, IList<Limb>? limbs);

    IList<IAnimation>? Animations { get; }

    IAnimation? SelectedAnimation { get; }
    event Action<IAnimation?> AnimationSelected;

    bool ShowBones { get; }
  }
}
