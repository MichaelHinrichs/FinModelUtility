using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using fin.gl;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;


namespace uni.ui.common {
  public partial class GlPanel : UserControl {
    public GlPanel() {
      InitializeComponent();

      this.impl_.InitializeContexts();

      if (!IsInDesignMode) {
        this.InitGl();
      }
    }

    public static bool IsInDesignMode {
      get {
        var isInDesignMode =
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        if (!isInDesignMode) {
          using (var process = Process.GetCurrentProcess()) {
            return process.ProcessName.ToLowerInvariant().Contains("devenv");
          }
        }

        return isInDesignMode;
      }
    }

    private void InitGl() {
      GlUtil.Init();

      this.impl_.CreateGraphics();

      var vertexShaderSrc = @"
# version 120 
  
varying vec4 vertexColor;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
}";

      var fragmentShaderSrc = @"
# version 120 

in vec4 vertexColor;

void main() {
    gl_FragColor = vertexColor;
}";

      var vertexShader = Gl.glCreateShader(Gl.GL_VERTEX_SHADER);
      Gl.glShaderSource(vertexShader, 1, new[] {vertexShaderSrc}, null);
      Gl.glCompileShader(vertexShader);

      var fragmentShader = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER);
      Gl.glShaderSource(fragmentShader, 1, new[] {fragmentShaderSrc}, null);
      Gl.glCompileShader(fragmentShader);

      var bufferSize = 10000;

      var vertexShaderErrorBuilder = new StringBuilder(bufferSize);
      Gl.glGetShaderInfoLog(vertexShader, bufferSize, out var
                                vertexShaderErrorLength,
                            vertexShaderErrorBuilder);
      var vertexShaderError = vertexShaderErrorBuilder.ToString();

      var fragmentShaderErrorBuilder = new StringBuilder(bufferSize);
      Gl.glGetShaderInfoLog(fragmentShader, bufferSize, out  var fragmentShaderErrorLength,
                            fragmentShaderErrorBuilder);
      var fragmentShaderError = fragmentShaderErrorBuilder.ToString();

      var shaderProgram = Gl.glCreateProgram();

      Gl.glAttachShader(shaderProgram, vertexShader);
      Gl.glAttachShader(shaderProgram, fragmentShader);
      Gl.glLinkProgram(shaderProgram);

      Gl.glUseProgram(shaderProgram);

      Glut.glutDisplayFunc(MainLoop);

      ResetGl();
      Wgl.wglSwapIntervalEXT(1);
    }

    public static void ResetGl() {
      Gl.glShadeModel(Gl.GL_SMOOTH);
      Gl.glEnable(Gl.GL_POINT_SMOOTH);
      Gl.glHint(Gl.GL_POINT_SMOOTH_HINT, Gl.GL_NICEST);

      Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);

      Gl.glClearDepth(5.0F);

      Gl.glDepthFunc(Gl.GL_LEQUAL);
      Gl.glEnable(Gl.GL_DEPTH_TEST);
      Gl.glDepthMask(Gl.GL_TRUE);

      Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);

      Gl.glEnable(Gl.GL_LIGHT0);

      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_NORMALIZE);

      Gl.glEnable(Gl.GL_CULL_FACE);
      Gl.glCullFace(Gl.GL_BACK);

      Gl.glEnable(Gl.GL_BLEND);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

      Gl.glClearColor(0.2f, 0.5f, 0.7f, 1);
    }

    public void MainLoop() {
      var width = this.Width;
      var height = this.Height;

      {
        Gl.glViewport(0, 0, width, height);

        Gl.glMatrixMode(Gl.GL_PROJECTION);
        Gl.glLoadIdentity();
        Glu.gluOrtho2D(0, width, height, 0);

        Gl.glMatrixMode(Gl.GL_MODELVIEW);
        Gl.glLoadIdentity();
      }

      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glColor3f(1, 0, 0);
      Gl.glVertex2f(0, 0);

      Gl.glColor3f(0, 1, 0);
      Gl.glVertex2f(0, height);

      Gl.glColor3f(1, 1, 1);
      Gl.glVertex2f(width, height);

      Gl.glColor3f(0, 0, 1);
      Gl.glVertex2f(width, 0);

      Gl.glEnd();

      Gl.glFlush();

      this.impl_.Invalidate();
    }

    protected override void OnPaint(PaintEventArgs pe) {
      base.OnPaint(pe);

      if (!IsInDesignMode) {
        this.MainLoop();
      }
    }
  }
}