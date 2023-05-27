using System;

using fin.data;

using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public class GlShaderProgram : IDisposable {
    private static ReferenceCountCacheDictionary<string, int>
        vertexShaderCache_ =
            new(src =>
                    GlShaderProgram.CreateAndCompileShader_(
                        src,
                        ShaderType.VertexShader),
                (_, id) => {
                  if (id != UNDEFINED_ID) {
                    GL.DeleteShader(id);
                  }
                });

    private static ReferenceCountCacheDictionary<string, int>
        fragmentShaderCache_ =
            new(src =>
                    GlShaderProgram.CreateAndCompileShader_(
                        src,
                        ShaderType.FragmentShader),
                (_, id) => {
                  if (id != UNDEFINED_ID) {
                    GL.DeleteShader(id);
                  }
                });

    private static
        ReferenceCountCacheDictionary<(string vertexSrc, string fragmentSrc),
            int> programCache_ =
            new(vertexAndFragmentSrc => {
                  var (vertexSrc, fragmentSrc) = vertexAndFragmentSrc;
                  var vertexShaderId =
                      GlShaderProgram.vertexShaderCache_.GetAndIncrement(
                          vertexSrc);
                  var fragmentShaderId =
                      GlShaderProgram.fragmentShaderCache_.GetAndIncrement(
                          fragmentSrc);

                  var programId = GL.CreateProgram();

                  GL.AttachShader(programId, vertexShaderId);
                  GL.AttachShader(programId, fragmentShaderId);
                  GL.LinkProgram(programId);

                  return programId;
                },
                (vertexAndFragmentSrc, programId) => {
                  GL.DeleteProgram(programId);

                  var (vertexSrc, fragmentSrc) = vertexAndFragmentSrc;
                  GlShaderProgram.vertexShaderCache_.DecrementAndMaybeDispose(
                      vertexSrc);
                  GlShaderProgram.fragmentShaderCache_.DecrementAndMaybeDispose(
                      fragmentSrc);
                });

    private const int UNDEFINED_ID = -1;

    public static GlShaderProgram
        FromShaders(string vertexShaderSrc, string fragmentShaderSrc)
      => new(vertexShaderSrc, fragmentShaderSrc);

    private GlShaderProgram(string vertexShaderSrc,
                            string fragmentShaderSrc) {
      this.VertexShaderSource = vertexShaderSrc;
      this.FragmentShaderSource = fragmentShaderSrc;
      this.ProgramId =
          GlShaderProgram.programCache_.GetAndIncrement(
              (vertexShaderSrc, fragmentShaderSrc));
    }

    ~GlShaderProgram() => this.ReleaseUnmanagedResources_();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      GlShaderProgram.programCache_.DecrementAndMaybeDispose(
          (this.VertexShaderSource, this.FragmentShaderSource));
      this.ProgramId = UNDEFINED_ID;
    }

    private static int CreateAndCompileShader_(string src,
                                               ShaderType shaderType) {
      var shaderId = GL.CreateShader(shaderType);
      GL.ShaderSource(shaderId, 1, new[] { src }, (int[]) null);
      GL.CompileShader(shaderId);

      // TODO: Throw/return this error
      var bufferSize = 10000;
      GL.GetShaderInfoLog(
          shaderId,
          bufferSize,
          out var shaderErrorLength,
          out var shaderError);

      if (shaderError?.Length > 0) {
        ;
      }

      return shaderId;
    }

    public string VertexShaderSource { get; }
    public string FragmentShaderSource { get; }

    public int ProgramId { get; private set; } = UNDEFINED_ID;

    public void Use() => GL.UseProgram(this.ProgramId);

    public int GetUniformLocation(string name) =>
        GL.GetUniformLocation(this.ProgramId, name);

    public int GetAttribLocation(string name) =>
        GL.GetAttribLocation(this.ProgramId, name);
  }
}