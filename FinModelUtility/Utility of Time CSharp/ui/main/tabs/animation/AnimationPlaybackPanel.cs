using System;
using System.Windows.Forms;

using UoT.animation.playback;

namespace UoT.ui.main.tabs.animation {
  public partial class AnimationPlaybackPanel : UserControl, IAnimationPlaybackManager {
    // TODO: Add tests.
    // TODO: How to right-align frame label?
    // TODO: Fix bug where scrolling trackbar changes time dramatically?

    private readonly IAnimationPlaybackManager impl_ = new FrameAdvancer();

    public AnimationPlaybackPanel() {
      this.InitializeComponent();

      this.playButton_.Click += (sender, args) => {
        this.IsPlaying = true;
      };
      this.pauseButton_.Click += (sender, args) => {
        this.IsPlaying = false;
      };
      this.loopCheckBox_.Click += (sender, args) => {
        this.ShouldLoop = this.loopCheckBox_.Checked;
      };
      this.frameTrackBar_.Scroll += (sender, args) => {
        this.Frame = this.frameTrackBar_.Value;
      };
      this.frameRateSelector_.ValueChanged += (sender, args) => {
        var newFrameRate = (int)Math.Floor(this.frameRateSelector_.Value);
        if (this.FrameRate != newFrameRate) {
          this.FrameRate = newFrameRate;
        }
      };

      this.FrameRate = (int) Math.Floor(this.frameRateSelector_.Value);
    }

    // TODO: Should carry over changes to form.
    public double Frame {
      get => this.impl_.Frame;
      set {
        this.impl_.Frame = value;

        if (this.TotalFrames > 0) {
          this.frameTrackBar_.Value = (int)value;
        }
      } 
    }
    public int TotalFrames {
      get => this.impl_.TotalFrames;
      set {
        this.impl_.TotalFrames = value;
        this.frameTrackBar_.Maximum = value;
      }
    }

    public int FrameRate {
      get => this.impl_.FrameRate;
      set {
        this.impl_.FrameRate = value;
        this.frameRateSelector_.Value = value;
      }
    }


    public bool IsPlaying {
      get => this.impl_.IsPlaying;
      set => this.impl_.IsPlaying = value;
    }
    public bool ShouldLoop {
      get => this.impl_.ShouldLoop;
      set {
        this.impl_.ShouldLoop = value;
        this.loopCheckBox_.Checked = value;
      }
    }


    public void Reset() => this.impl_.Reset();

    public void Tick() {
      this.impl_.Tick();
      this.Update_();
    }

    private void Update_() {
      var elapsedSecondsText = "0.0s";
      var elapsedFramesText = "0/0";

      // TODO: What's with all these extra checks?
      if (this.TotalFrames > 0) {
        var frame = this.Frame;
        var frameIndex = (int)Math.Floor(frame);

        this.frameTrackBar_.Value = frameIndex;

        var seconds = frame / this.FrameRate;
        var secondsInt = (int)Math.Max(0, seconds);
        var secondsDelta = (int)(seconds * 1000 % 1000);

        elapsedSecondsText = secondsInt +
                              "." +
                              secondsDelta +
                              "s";
        elapsedFramesText = frameIndex + "/" + this.TotalFrames;
      }

      this.elapsedSeconds_.Text = elapsedSecondsText;
      this.elapsedFrames_.Text = elapsedFramesText;
    }
  }
}
