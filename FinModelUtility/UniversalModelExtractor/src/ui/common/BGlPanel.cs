using System.Diagnostics;
using fin.util.time;

namespace uni.ui.common {
  public abstract partial class BGlPanel : UserControl {
    private readonly TimedCallback timedCallback;
    private readonly Stopwatch stopwatch_ = Stopwatch.StartNew();
    private const float DEFAULT_FRAMERATE_ = 30;

    public BGlPanel() {
      InitializeComponent();

      this.impl_.InitializeContexts();

      if (!DesignModeUtil.InDesignMode) {
        this.InitGl();
        this.impl_.CreateGraphics();
        this.timedCallback =
            TimedCallback.WithFrequency(this.Invalidate, DEFAULT_FRAMERATE_);
      }
    }

    public float Framerate {
      get => this.timedCallback.Frequency;
      set => this.timedCallback.Frequency = value;
    }

    protected abstract void InitGl();

    protected abstract void RenderGl();

    protected override void OnPaint(PaintEventArgs pe) {
      base.OnPaint(pe);
      if (!DesignModeUtil.InDesignMode) {
        this.RenderGl();
        this.impl_.Invalidate();
      }
    }
  }
}