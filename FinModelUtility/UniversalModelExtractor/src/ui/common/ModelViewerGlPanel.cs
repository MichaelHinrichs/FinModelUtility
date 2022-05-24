using System.Diagnostics;

using fin.gl;

using Tao.OpenGl;
using Tao.Platform.Windows;
using System.Drawing;


namespace uni.ui.common {
  public partial class ModelViewerGlPanel : BGlPanel {
    private readonly Stopwatch stopwatch_ = Stopwatch.StartNew();
    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private GlShaderProgram shaderProgram_;

    protected override void InitGl() {
      GlUtil.Init();

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

      this.shaderProgram_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);
      this.shaderProgram_.Use();

      ResetGl_();
      Wgl.wglSwapIntervalEXT(1);
    }

    private void ResetGl_() {
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

      Gl.glClearColor(backgroundColor_.R / 255f, backgroundColor_.G / 255f, backgroundColor_.B / 255f, 1);
    }

    protected override void RenderGl() {
      var width = this.Width;
      var height = this.Height;

      {
        Gl.glViewport(0, 0, width, height);

        Gl.glMatrixMode(Gl.GL_PROJECTION);
        Gl.glLoadIdentity();
        Glu.gluOrtho2D(0, width, height, 0);

        Gl.glMatrixMode(Gl.GL_MODELVIEW);
        Gl.glLoadIdentity();

        var t = this.stopwatch_.Elapsed.TotalSeconds;
        var angle = t * 45;
        Gl.glTranslated(width / 2, height / 2, 0);
        Gl.glRotated(angle, 0, 0, 1);
      }

      var size = MathF.Max(width, height) * MathF.Sqrt(2);

      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

      Gl.glBegin(Gl.GL_QUADS);

      Gl.glColor3f(1, 0, 0);
      Gl.glVertex2f(-size / 2, -size / 2);

      Gl.glColor3f(0, 1, 0);
      Gl.glVertex2f(-size / 2, size / 2);

      Gl.glColor3f(1, 1, 1);
      Gl.glVertex2f(size / 2, size / 2);

      Gl.glColor3f(0, 0, 1);
      Gl.glVertex2f(size / 2, -size / 2);

      Gl.glEnd();

      Gl.glFlush();
    }
  }
}