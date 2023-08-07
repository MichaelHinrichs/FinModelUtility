using fin.audio.al;
using fin.audio;
using fin.data;
using fin.graphics.gl;
using fin.util.time;
using OpenTK.Graphics.OpenGL;
using uni.ui.gl;


namespace uni.ui.common.audio {
  public class AudioPlayerGlPanel : BGlPanel, IAudioPlayerPanel {
    private IReadOnlyList<IAudioFileBundle>? audioFileBundles_;
    private ShuffledListView<IAudioFileBundle>? shuffledListView_;
    private readonly IAudioManager<short> audioManager_ = new AlAudioManager();
    private readonly IAudioSource<short> audioSource_;

    private readonly WaveformRenderer waveformRenderer_ = new();

    private readonly TimedCallback playNextCallback_;

    private GlShaderProgram texturelessShaderProgram_;

    public AudioPlayerGlPanel() {
      this.Disposed += (_, _) => this.audioManager_.Dispose();

      this.audioSource_ = this.audioManager_.CreateAudioSource();

      var playNextLock = new object();
      this.playNextCallback_ = new TimedCallback(() => {
        lock (playNextLock) {
          if (this.shuffledListView_ == null) {
            return;
          }

          var activeSound = this.waveformRenderer_.ActiveSound;
          if (activeSound?.State == SoundState.PLAYING) {
            return;
          }

          this.waveformRenderer_.ActiveSound = null;
          activeSound?.Stop();
          activeSound?.Dispose();

          if (this.shuffledListView_.TryGetNext(out var audioFileBundle)) {
            var audioBuffer =
                new GlobalAudioLoader().LoadAudio(this.audioManager_,
                                                  audioFileBundle);
            var audioStream =
                this.audioManager_.CreateBufferAudioStream(audioBuffer);

            activeSound = this.waveformRenderer_.ActiveSound =
                              this.audioSource_.Create(audioStream);
            activeSound.Volume = .1f;
            activeSound.Play();

            this.OnChange(audioFileBundle);
          }
        }
      }, .1f);
    }

    /// <summary>
    ///   Sets the audio file bundles to play in the player.
    /// </summary>
    public IReadOnlyList<IAudioFileBundle>? AudioFileBundles {
      get => this.audioFileBundles_;
      set {
        var originalValue = this.audioFileBundles_;
        this.audioFileBundles_ = value;

        this.waveformRenderer_.ActiveSound?.Stop();
        this.waveformRenderer_.ActiveSound = null;

        this.shuffledListView_
            = value != null
                  ? new ShuffledListView<IAudioFileBundle>(value)
                  : null;

        if (value == null && originalValue != null) {
          this.OnChange(null);
        }
      }
    }

    public event Action<IAudioFileBundle?> OnChange = delegate { };

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

      this.ResetGl_();
    }

    private void ResetGl_() => GlUtil.ResetGl();

    protected override void RenderGl() {
      var width = this.Width;
      var height = this.Height;
      GL.Viewport(0, 0, width, height);

      GlUtil.ClearColorAndDepth();

      this.texturelessShaderProgram_.Use();
      this.RenderOrtho_();
    }

    private void RenderOrtho_() {
      var width = this.Width;
      var height = this.Height;

      {
        GlTransform.MatrixMode(MatrixMode.Projection);
        GlTransform.LoadIdentity();
        GlTransform.Ortho2d(0, width, height, 0);

        GlTransform.MatrixMode(MatrixMode.Modelview);
        GlTransform.LoadIdentity();
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