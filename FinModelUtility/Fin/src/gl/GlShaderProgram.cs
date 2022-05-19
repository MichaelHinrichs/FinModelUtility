using System;
using System.Text;

using Tao.OpenGl;


namespace fin.gl {
  public class GlShaderProgram : IDisposable {
    private const int UNDEFINED_ID = -1;

    private readonly int programId_ = UNDEFINED_ID;
    private readonly int vertexShaderId_ = UNDEFINED_ID;
    private readonly int fragmentShaderId_ = UNDEFINED_ID;

    public static GlShaderProgram
        FromShaders(string vertexShaderSrc, string fragmentShaderSrc)
      => new(vertexShaderSrc, fragmentShaderSrc);

    private GlShaderProgram(string vertexShaderSrc,
                            string fragmentShaderSrc) {
      this.vertexShaderId_ =
          CreateAndCompileShader(vertexShaderSrc, Gl.GL_VERTEX_SHADER);
      this.fragmentShaderId_ =
          CreateAndCompileShader(fragmentShaderSrc, Gl.GL_FRAGMENT_SHADER);

      this.programId_ = Gl.glCreateProgram();

      Gl.glAttachShader(this.programId_, this.vertexShaderId_);
      Gl.glAttachShader(this.programId_, this.fragmentShaderId_);
      Gl.glLinkProgram(this.programId_);
    }

    ~GlShaderProgram() => this.Destroy();

    public void Dispose() {
      this.Destroy();
      GC.SuppressFinalize(this);
    }

    private static int CreateAndCompileShader(string src, int shaderType) {
      var shaderId = Gl.glCreateShader(shaderType);
      Gl.glShaderSource(shaderId, 1, new[] {src}, null);
      Gl.glCompileShader(shaderId);

      // TODO: Throw/return this error
      var bufferSize = 10000;
      var shaderErrorBuilder = new StringBuilder(bufferSize);
      Gl.glGetShaderInfoLog(shaderId, bufferSize, out var shaderErrorLength,
                            shaderErrorBuilder);
      var vertexShaderError = shaderErrorBuilder.ToString();

      return shaderId;
    }

    private void Destroy() {
      Gl.glDeleteProgram(this.programId_);
      if (this.vertexShaderId_ != UNDEFINED_ID) {
        Gl.glDeleteShader(this.vertexShaderId_);
      }
      if (this.fragmentShaderId_ != UNDEFINED_ID) {
        Gl.glDeleteShader(this.fragmentShaderId_);
      }
    }

    public void Use() => Gl.glUseProgram(this.programId_);
  }
}