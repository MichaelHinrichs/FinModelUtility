using fin.audio.impl.al;
using fin.audio;
using fin.data;
using fin.gl;
using OpenTK.Graphics.OpenGL;
using uni.ui.gl;


namespace uni.ui.common {
  public class AudioPlayerGlPanel : BGlPanel {
    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private IReadOnlyList<IAudioFileBundle>? audioFileBundles_;
    private ShuffledListView<IAudioFileBundle>? shuffledListView_;
    private readonly IAudioManager<short> audioManager_ = new AlAudioManager();
    private IActiveSound<short>? activeSound_;

    private readonly WaveformRenderer waveformRenderer_ = new();

    private GlShaderProgram texturelessShaderProgram_;

    /// <summary>
    ///   Sets the audio file bundles to play in the player.
    /// </summary>
    public IReadOnlyList<IAudioFileBundle>? AudioFileBundles {
      get => this.audioFileBundles_;
      set {
        this.audioFileBundles_ = value;

        this.shuffledListView_
            = value != null
                  ? new ShuffledListView<IAudioFileBundle>(value)
                  : null;
        this.PlayNext_();
      }
    }

    private void PlayNext_() {
      this.activeSound_?.Stop();

      if (this.shuffledListView_ == null) {
        this.activeSound_ = null;
        return;
      }

      var audioBuffer =
          new GlobalAudioLoader().LoadAudio(this.audioManager_,
                                            this.shuffledListView_.Next());
      var audioStream = this.audioManager_.CreateBufferAudioStream(audioBuffer);

      this.activeSound_ = this.audioManager_.CreateAudioSource()
                             .Play(audioStream);

      this.waveformRenderer_.ActiveSound = this.activeSound_;
    }

    protected override void InitGl() {
      this.texturelessShaderProgram_ =
          GlShaderProgram.FromShaders(@"
# version 120

varying vec4 vertexColor;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * gl_Vertex; 
    vertexColor = gl_Color;
}", @"
# version 130 

out vec4 fragColor;

in vec4 vertexColor;

void main() {
    fragColor = vertexColor;
}");

      ResetGl_();
    }

    private void ResetGl_() {
      GL.ShadeModel(ShadingModel.Smooth);
      GL.Enable(EnableCap.PointSmooth);
      GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);

      GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

      GL.ClearDepth(5.0F);

      GL.DepthFunc(DepthFunction.Lequal);
      GL.Enable(EnableCap.DepthTest);
      GL.DepthMask(true);

      GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

      GL.Enable(EnableCap.Normalize);

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(CullFaceMode.Back);

      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      GL.ClearColor(backgroundColor_.R / 255f, backgroundColor_.G / 255f,
                    backgroundColor_.B / 255f, 1);
    }

    protected override void RenderGl() {
      var width = this.Width;
      var height = this.Height;
      GL.Viewport(0, 0, width, height);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      this.texturelessShaderProgram_.Use();
      this.RenderOrtho_();
    }

    private void RenderOrtho_() {
      var width = this.Width;
      var height = this.Height;

      {
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GlUtil.Ortho2d(0, width, height, 0);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
      }

      GL.LineWidth(1.5f);

      var amplitude = height * .45f;
      this.waveformRenderer_.Width = width;
      this.waveformRenderer_.Amplitude = amplitude;
      this.waveformRenderer_.MiddleY = height / 2f;
      this.waveformRenderer_.Render();
    }
  }
}