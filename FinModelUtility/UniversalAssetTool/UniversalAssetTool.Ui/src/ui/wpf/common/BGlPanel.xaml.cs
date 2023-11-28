using System.ComponentModel;

using fin.ui.rendering.gl;

using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;

using UserControl = System.Windows.Controls.UserControl;

namespace uni.ui.wpf.common {
  /// <summary>
  ///   Interaction logic for BGlPanel.xaml
  /// </summary>
  [TypeDescriptionProvider(typeof(
      AbstractControlDescriptionProvider<BGlPanel, UserControl>))]
  public abstract partial class BGlPanel : UserControl {
    public BGlPanel() {
      InitializeComponent();

      var settings = new GLWpfControlSettings {
          MajorVersion = 4,
          MinorVersion = 8,
          TransparentBackground = true,
      };
      impl_.Start(settings);

      if (!DesignModeUtil.InDesignMode) {
        GlUtil.Init();
        this.InitGl();
      }
    }

    protected abstract void InitGl();
    protected abstract void RenderGl();

    private void Render_(TimeSpan delta) {
      if (!DesignModeUtil.InDesignMode) {
        this.RenderGl();
        GL.Flush();
      }
    }
  }
}