using System.Diagnostics;

using fin.animation.playback;
using fin.gl;

using Tao.OpenGl;
using Tao.Platform.Windows;

using fin.model;

using uni.ui.gl;


namespace uni.ui.common {
  public partial class ModelViewerGlPanel : BGlPanel {
    private readonly Camera camera_ = new();
    private float fovY_ = 30;

    private readonly Stopwatch stopwatch_ = Stopwatch.StartNew();
    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private GlShaderProgram shaderProgram_;
    private int texture0Location_;

    private ModelRenderer? modelRenderer_;

    private readonly FrameAdvancer frameAdvancer_ = new() {
        IsPlaying = true,
        ShouldLoop = true,
    };

    public IModel? Model {
      get => this.modelRenderer_?.Model;
      set {
        this.modelRenderer_?.Dispose();
        this.modelRenderer_ =
            value != null ? new ModelRenderer(value) : null;
        this.Animation = value?.AnimationManager.Animations.FirstOrDefault();

        this.frameAdvancer_.FrameRate = (int) (this.Animation?.FrameRate ?? 20);
        this.frameAdvancer_.TotalFrames = this.Animation?.FrameCount ?? 0;
      }
    }

    public IAnimation? Animation { get; set; }

    private bool isMouseDown_ = false;
    private (int, int)? prevMousePosition_ = null;

    private bool isForwardDown_ = false;
    private bool isBackwardDown_ = false;
    private bool isLeftwardDown_ = false;
    private bool isRightwardDown_ = false;

    public ModelViewerGlPanel() {
      this.impl_.MouseDown += (sender, args) => {
        if (args.Button == MouseButtons.Left) {
          isMouseDown_ = true;
          this.prevMousePosition_ = null;
        }
      };
      this.impl_.MouseUp += (sender, args) => {
        if (args.Button == MouseButtons.Left) {
          isMouseDown_ = false;
        }
      };
      this.impl_.MouseMove += (sender, args) => {
        if (this.isMouseDown_) {
          var mouseLocation = (args.X, args.Y);

          if (this.prevMousePosition_ != null) {
            var (prevMouseX, prevMouseY) = this.prevMousePosition_.Value;
            var (mouseX, mouseY) = mouseLocation;

            var deltaMouseX = mouseX - prevMouseX;
            var deltaMouseY = mouseY - prevMouseY;

            var fovY = this.fovY_;
            var fovX = fovY / this.Height * this.Width;

            var deltaXFrac = 1f * deltaMouseX / this.Width;
            var deltaYFrac = 1f * deltaMouseY / this.Height;

            var mouseSpeed = 3;

            this.camera_.Pitch -= deltaYFrac * fovY * mouseSpeed;
            this.camera_.Yaw -= deltaXFrac * fovX * mouseSpeed;
          }

          this.prevMousePosition_ = mouseLocation;
        }
      };

      this.impl_.KeyDown += (sender, args) => {
        switch (args.KeyCode) {
          case Keys.W: {
            this.isForwardDown_ = true;
            break;
          }
          case Keys.S: {
            this.isBackwardDown_ = true;
            break;
          }
          case Keys.A: {
            this.isLeftwardDown_ = true;
            break;
          }
          case Keys.D: {
            this.isRightwardDown_ = true;
            break;
          }
        }
      };

      this.impl_.KeyUp += (sender, args) => {
        switch (args.KeyCode) {
          case Keys.W: {
            this.isForwardDown_ = false;
            break;
          }
          case Keys.S: {
            this.isBackwardDown_ = false;
            break;
          }
          case Keys.A: {
            this.isLeftwardDown_ = false;
            break;
          }
          case Keys.D: {
            this.isRightwardDown_ = false;
            break;
          }
        }
      };
    }

    protected override void InitGl() {
      GlUtil.Init();

      var vertexShaderSrc = @"
# version 120

in vec2 in_uv0;

varying vec4 vertexColor;
varying vec2 uv0;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
    uv0 = gl_MultiTexCoord0.st;
}";

      var fragmentShaderSrc = @"
# version 130 

uniform sampler2D texture0;

out vec4 fragColor;

in vec4 vertexColor;
in vec2 uv0;

void main() {
    vec4 texColor = texture(texture0, uv0);

    fragColor = texColor * vertexColor;
}";

      this.shaderProgram_ =
          GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);
      this.shaderProgram_.Use();

      this.texture0Location_ =
          Gl.glGetUniformLocation(this.shaderProgram_.ProgramId, "texture0");

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
      Gl.glEnable(Gl.GL_TEXTURE_2D);

      Gl.glEnable(Gl.GL_LIGHTING);
      Gl.glEnable(Gl.GL_NORMALIZE);

      Gl.glEnable(Gl.GL_CULL_FACE);
      Gl.glCullFace(Gl.GL_BACK);

      Gl.glEnable(Gl.GL_BLEND);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

      Gl.glClearColor(backgroundColor_.R / 255f, backgroundColor_.G / 255f,
                      backgroundColor_.B / 255f, 1);
    }

    protected override void RenderGl() {
      var forwardVector =
          (this.isForwardDown_ ? 1 : 0) - (this.isBackwardDown_ ? 1 : 0);
      var rightwardVector =
          (this.isRightwardDown_ ? 1 : 0) - (this.isLeftwardDown_ ? 1 : 0);
      this.camera_.Move(forwardVector, rightwardVector, 15);

      var width = this.Width;
      var height = this.Height;
      Gl.glViewport(0, 0, width, height);

      Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

      Gl.glUniform1i(this.texture0Location_, 0);

      this.RenderPerspective_();
      //this.RenderOrtho_();

      Gl.glFlush();
    }

    private void RenderPerspective_() {
      var width = this.Width;
      var height = this.Height;

      {
        Gl.glMatrixMode(Gl.GL_PROJECTION);
        Gl.glLoadIdentity();
        Glu.gluPerspective(this.fovY_, 1f * width / height, .1, 10000);
        Glu.gluLookAt(this.camera_.X, this.camera_.Y, this.camera_.Z,
                      this.camera_.X + this.camera_.XNormal,
                      this.camera_.Y + this.camera_.YNormal,
                      this.camera_.Z + this.camera_.ZNormal, 0, 0, 1);

        Gl.glMatrixMode(Gl.GL_MODELVIEW);
        Gl.glLoadIdentity();
        Gl.glRotated(90, 1, 0, 0);
      }

      if (this.Animation != null) {
        this.frameAdvancer_.Tick();
        this.modelRenderer_?.CalculateAnimationMatrices(
            this.Animation, (float) this.frameAdvancer_.Frame);
      }
      // TODO: Normalize the model scale somehow
      this.modelRenderer_?.Render();
    }

    private void RenderOrtho_() {
      var width = this.Width;
      var height = this.Height;

      {
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
    }
  }
}