using System;
using System.Collections.Generic;
using System.Windows.Forms;

using UoT.animation.banks;
using UoT.hacks;
using UoT.limbs;
using UoT.util;

namespace UoT.ui.main.tabs.animation {
  public partial class AnimationSelectorPanel : UserControl, IAnimationBanks {
    // TODO: Add tests to make sure expected # of events are fired in expected places.

    private readonly AnimationReader animationReader_ = new AnimationReader();
    private IReadOnlyList<IAnimationBank>? animationBanks_;
    public IList<IAnimation>? Animations { get; private set; }
    private IList<Limb>? limbs_;

    public AnimationSelectorPanel() => this.InitializeComponent();

    public void Populate(IReadOnlyList<IAnimationBank> banks) {
      this.animationBanks_ = banks;

      this.bankComboBox_.Items.Clear();
      foreach (var bank in this.animationBanks_) {
        this.bankComboBox_.Items.Add(bank.Name);
      }
    }

    public void Reset(string filename, IList<Limb>? limbs) {
      this.limbs_ = limbs;

      if (limbs != null) {
        var initialIndex = this.bankComboBox_.SelectedIndex;

        var targetBankName =
            DefaultAnimationBankHack.GetDefaultAnimationBankForObject(filename);
        var targetIndex = 0;
        for (var i = 0; i < this.animationBanks_!.Count; ++i) {
          var bank = this.animationBanks_[i];
          if (bank.Name == targetBankName) {
            targetIndex = i;
            break;
          }
        }

        this.bankComboBox_.SelectedIndex = targetIndex;

        // If initial index was 0, then change wasn't detected. We need to 
        // manually reload.
        if (initialIndex == targetIndex) {
          this.LoadBankAndSelectFirstAnimation_();
        }
      } else {
        this.bankComboBox_.SelectedIndex = -1;
        this.TriggerAnimationSelectedEvent_();
      }
    }

    public event Action<IAnimation?> AnimationSelected = delegate {};

    public bool ShowBones => this.showBonesCheckBox_.Checked;


    public IAnimationBank? SelectedAnimationBank {
      get {
        var selectedIndex = this.bankComboBox_.SelectedIndex;
        return selectedIndex == -1
                   ? null
                   : this.animationBanks_?[selectedIndex];
      }
    }

    private void bankComboBox__SelectedIndexChanged_(
        object sender,
        System.EventArgs e) => this.LoadBankAndSelectFirstAnimation_();

    private void LoadBankAndSelectFirstAnimation_() {
      this.Animations = null;

      var limbCount = this.limbs_?.Count ?? 0;
      var selectedAnimationBank = this.SelectedAnimationBank;

      if (limbCount > 0 && selectedAnimationBank != null) {
        // Inline
        if (selectedAnimationBank.Bank == null) {
          this.Animations =
              this.animationReader_.GetCommonAnimations(
                  RamBanks.ZFileBuffer,
                  limbCount,
                  this.animationsListBox_);
        } else {
          if (selectedAnimationBank.Name == "link_animetion") {
            this.Animations = this.animationReader_.GetLinkAnimations(
                RamBanks.GameplayKeep,
                limbCount,
                selectedAnimationBank.Bank,
                this.animationsListBox_);
          } else {
            this.Animations = this.animationReader_.GetCommonAnimations(
                selectedAnimationBank.Bank,
                limbCount,
                this.animationsListBox_);
          }
        }
      }

      if (this.Animations != null) {
        Asserts.Assert(this.Animations.Count > 0);
        this.animationsListBox_.SelectedIndex = 0;
      } else {
        this.animationsListBox_.SelectedIndex = -1;
        this.TriggerAnimationSelectedEvent_();
      }
    }


    public IAnimation? SelectedAnimation {
      get {
        var selectedIndex = this.animationsListBox_.SelectedIndex;
        return selectedIndex == -1 ? null : this.Animations?[selectedIndex];
      }
    }

    private void animationsListBox__SelectedIndexChanged(
        object sender,
        EventArgs e) => this.TriggerAnimationSelectedEvent_();

    private void TriggerAnimationSelectedEvent_()
      => this.AnimationSelected.Invoke(this.SelectedAnimation);
  }
}