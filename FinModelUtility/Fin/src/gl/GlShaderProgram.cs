using System;
using System.Text;

using Tao.OpenGl;


namespace fin.gl {
  public class GlShaderProgram : IDisposable {
    private const int UNDEFINED_ID = -1;

    private int vertexShaderId_ = UNDEFINED_ID;
    private int fragmentShaderId_ = UNDEFINED_ID;

    public static GlShaderProgram
        FromShaders(string vertexShaderSrc, string fragmentShaderSrc)
      => new(vertexShaderSrc, fragmentShaderSrc);

    private GlShaderProgram(string vertexShaderSrc,
                            string fragmentShaderSrc) {
      this.vertexShaderId_ =
          CreateAndCompileShader_(vertexShaderSrc, Gl.GL_VERTEX_SHADER);
      this.fragmentShaderId_ =
          CreateAndCompileShader_(fragmentShaderSrc, Gl.GL_FRAGMENT_SHADER);

      this.ProgramId = Gl.glCreateProgram();

      Gl.glAttachShader(this.ProgramId, this.vertexShaderId_);
      Gl.glAttachShader(this.ProgramId, this.fragmentShaderId_);
      Gl.glLinkProgram(this.ProgramId);
    }

    ~GlShaderProgram() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }
    
    private void ReleaseUnmanagedResources_() {
      Gl.glDeleteProgram(this.ProgramId);
      if (this.vertexShaderId_ != UNDEFINED_ID) {
        Gl.glDeleteShader(this.vertexShaderId_);
      }
      if (this.fragmentShaderId_ != UNDEFINED_ID) {
        Gl.glDeleteShader(this.fragmentShaderId_);
      }

      this.ProgramId =
          this.vertexShaderId_ = this.fragmentShaderId_ = UNDEFINED_ID;
    }

    private static int CreateAndCompileShader_(string src, int shaderType) {
      var shaderId = Gl.glCreateShader(shaderType);
      Gl.glShaderSource(shaderId, 1, new[] {src}, null);
      Gl.glCompileShader(shaderId);

      // TODO: Throw/return this error
      var bufferSize = 10000;
      var shaderErrorBuilder = new StringBuilder(bufferSize);
      Gl.glGetShaderInfoLog(shaderId, bufferSize, out var shaderErrorLength,
                            shaderErrorBuilder);
      var shaderError = shaderErrorBuilder.ToString();

      if (shaderError.Length > 0) {
        ;
      }

      return shaderId;
    }

    public int ProgramId { get; private set; } = UNDEFINED_ID;

    public void Use() => Gl.glUseProgram(this.ProgramId);
  }
}