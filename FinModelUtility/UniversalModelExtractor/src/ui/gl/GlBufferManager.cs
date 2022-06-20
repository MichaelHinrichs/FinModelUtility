using fin.gl;
using fin.math;
using fin.model;
using fin.model.impl;

using Tao.OpenGl;


namespace uni.ui.gl {
  public class GlBufferManager : IDisposable {
    private readonly IReadOnlyList<IVertex> vertices_;
    private readonly float[] vertexData_;

    private int vaoId_;
    private int vboId_;

    public GlBufferManager(IModel model) {
      this.vertices_ = model.Skin.Vertices;
      this.vertexData_ = new float[this.vertices_.Count];

      Gl.glGenVertexArraysAPPLE(1, out this.vaoId_);
      Gl.glGenBuffers(1, out this.vboId_);
    }

    ~GlBufferManager() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      Gl.glDeleteVertexArraysAPPLE(1, ref this.vaoId_);
      Gl.glDeleteBuffers(1, ref this.vboId_);
    }

    private readonly IPosition position_ = new ModelImpl.PositionImpl();

    public void UpdateTransforms(BoneTransformManager boneTransformManager) {
      for (var i = 0; i < this.vertices_.Count; ++i) {
        var vertex = this.vertices_[i];

        boneTransformManager.ProjectVertex(vertex, this.position_);

        var offset = 3 * i;
        this.vertexData_[offset + 0] = this.position_.X;
        this.vertexData_[offset + 1] = this.position_.Y;
        this.vertexData_[offset + 2] = this.position_.Z;
      }

      Gl.glBindVertexArrayAPPLE(this.vaoId_);

      Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, this.vboId_);
      Gl.glBufferData(Gl.GL_ARRAY_BUFFER,
                      new IntPtr(sizeof(float) * this.vertexData_.Length),
                      this.vertexData_,
                      Gl.GL_DYNAMIC_DRAW);

      Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE,
                               3 * sizeof(float), new IntPtr(0));
      Gl.glEnableVertexAttribArray(0);
    }

    public GlBufferRenderer CreateRenderer(
        IEnumerable<(IVertex, IVertex, IVertex)> triangles)
      => new GlBufferRenderer(this.vaoId_, triangles);

    public class GlBufferRenderer : IDisposable {
      private readonly int vaoId_;
      private int eboId_;

      private readonly int[] indices_;

      private readonly GlShaderProgram shaderProgram_;

      public GlBufferRenderer(
          int vaoId,
          IEnumerable<(IVertex, IVertex, IVertex)>
              triangles) {
        this.vaoId_ = vaoId;
        Gl.glGenBuffers(1, out this.eboId_);

        this.indices_ = triangles.SelectMany(triangle => new int[] {
                                     triangle.Item1.Index,
                                     triangle.Item2.Index,
                                     triangle.Item3.Index
                                 })
                                 .ToArray();

        Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, this.eboId_);
        Gl.glBufferData(Gl.GL_ELEMENT_ARRAY_BUFFER,
                        new IntPtr(sizeof(int) * this.indices_.Length),
                        this.indices_,
                        Gl.GL_STATIC_DRAW);

        var vertexShaderSrc = @"
# version 120

layout (location = 0) in vec3 position;

in vec2 in_uv0;

varying vec4 vertexColor;
varying vec2 uv0;

void main() {
    gl_Position = gl_ProjectionMatrix * gl_ModelViewMatrix * position; 
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

    if (fragColor.a < .95) {
      discard;
    }
}";

        this.shaderProgram_ =
            GlShaderProgram.FromShaders(vertexShaderSrc, fragmentShaderSrc);

        var texture0Location_ =
            Gl.glGetUniformLocation(this.shaderProgram_.ProgramId,
                                    "texture0");
      }

      ~GlBufferRenderer() => ReleaseUnmanagedResources_();

      public void Dispose() {
        ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        Gl.glDeleteBuffers(1, ref this.eboId_);
      }

      public void Render() {
        this.shaderProgram_.Use();
        Gl.glBindBuffer(Gl.GL_ELEMENT_ARRAY_BUFFER, this.eboId_);
        Gl.glBindVertexArrayAPPLE(this.vaoId_);
        Gl.glDrawElements(Gl.GL_TRIANGLES,
                          this.indices_.Length,
                          Gl.GL_INT,
                          0);
        Gl.glBindVertexArrayAPPLE(0);
      }
    }
  }
}