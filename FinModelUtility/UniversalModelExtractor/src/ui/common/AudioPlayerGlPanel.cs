using ast.schema;
using fin.audio.impl.al;
using fin.audio;
using fin.gl;
using fin.io;
using OpenTK.Graphics.OpenGL;
using uni.ui.gl;


namespace uni.ui.common {
  public class AudioPlayerGlPanel : BGlPanel {
    private readonly Color backgroundColor_ = Color.FromArgb(51, 128, 179);

    private readonly IAudioManager<short> audioManager_ = new AlAudioManager();
    private readonly IAudioStream<short> musicStream_;
    private readonly IActiveSound<short> musicSound_;

    private readonly WaveformRenderer waveformRenderer_ = new();

    private GlShaderProgram texturelessShaderProgram_;

    public AudioPlayerGlPanel() {
      /*var musicDir = DirectoryConstants.CLI_DIRECTORY.GetSubdir("ui/music");
      var musicFiles = musicDir.GetExistingFiles()
                               .Where(
                                   file => file.Extension.ToLower() == ".ogg")
                               .ToArray();

      this.musicStream_ =
          this.audioManager_.CreateBufferAudioStream(
              this.audioManager_.LoadIntoBuffer(musicFiles[0]));*/

      if (!DesignModeUtil.InDesignMode) {
        /*var astDir = new FinDirectory(
             @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\mario_kart_double_dash\AudioRes\Stream");*/
        var astDir = new FinDirectory(
            @"R:\Documents\CSharpWorkspace\Pikmin2Utility\cli\roms\pikmin_2\AudioRes\Stream");
        var astFiles = astDir.GetExistingFiles()
                             .Where(
                                 file => file.Extension.ToLower() == ".ast")
                             .ToArray();

        var firstAstFile = astFiles[Random.Shared.Next(astFiles.Length)];
        var name = firstAstFile.Name;

        var ast = firstAstFile.ReadNew<Ast>(Endianness.BigEndian);

        var mutableBuffer = this.audioManager_.CreateMutableBuffer();

        mutableBuffer.Frequency = (int)ast.StrmHeader.SampleRate;

        var channelData =
            ast.ChannelData.Select(data => data.ToArray()).ToArray();
        mutableBuffer.SetPcm(channelData);

        this.musicStream_ =
            this.audioManager_.CreateBufferAudioStream(mutableBuffer);

        this.musicSound_ = this.audioManager_.CreateAudioSource()
                               .Play(this.musicStream_);
        this.musicSound_.Looping = true;

        this.waveformRenderer_.ActiveSound = this.musicSound_;
      }
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