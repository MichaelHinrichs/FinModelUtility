using fin.gl;
using fin.util.time;


namespace uni.ui.common {
  public abstract partial class BGlPanel : UserControl {
    private readonly TimedCallback timedCallback;
    private const float DEFAULT_FRAMERATE_ = 30;

    protected BGlPanel() {
      InitializeComponent();

      if (!DesignModeUtil.InDesignMode) {
        GlUtil.Init();
        this.impl_.CreateGraphics();
        this.impl_.MakeCurrent();

        this.InitGl();

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
        this.impl_.MakeCurrent();

        this.RenderGl();
        this.impl_.SwapBuffers();
        //this.impl_.Invalidate();
      }
    }
  }
}