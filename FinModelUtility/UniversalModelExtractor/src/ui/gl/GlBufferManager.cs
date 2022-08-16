using fin.gl;
using fin.math;
using fin.model;
using fin.model.impl;

using OpenTK.Graphics.OpenGL;


namespace uni.ui.gl {
  public class GlBufferManager : IDisposable {
    private readonly IReadOnlyList<IVertex> vertices_;

    private readonly float[] positionData_;
    private readonly float[] normalData_;
    private readonly float[] uv0Data_;

    private int vaoId_;
    private int[] vboIds_ = new int[3];

    private const int POSITION_SIZE_ = 3;
    private const int NORMAL_SIZE_ = 3;
    private const int UV0_SIZE_ = 2;

    public GlBufferManager(IModel model) {
      this.vertices_ = model.Skin.Vertices;

      this.positionData_ = new float[POSITION_SIZE_ * this.vertices_.Count];
      this.normalData_ = new float[NORMAL_SIZE_ * this.vertices_.Count];
      this.uv0Data_ = new float[UV0_SIZE_ * this.vertices_.Count];

      GL.GenVertexArrays(1, out this.vaoId_);
      GL.GenBuffers(this.vboIds_.Length, this.vboIds_);
    }

    ~GlBufferManager() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      GL.DeleteVertexArrays(1, ref this.vaoId_);
      GL.DeleteBuffers(this.vboIds_.Length, this.vboIds_);
    }

    private readonly IPosition position_ = new ModelImpl.PositionImpl();
    private readonly INormal normal_ = new ModelImpl.NormalImpl();

    public void UpdateTransforms(BoneTransformManager boneTransformManager) {
      for (var i = 0; i < this.vertices_.Count; ++i) {
        var vertex = this.vertices_[i];

        boneTransformManager.ProjectVertex(
            vertex,
            this.position_,
            this.normal_, 
            true);

        var positionOffset = POSITION_SIZE_ * i;
        this.positionData_[positionOffset + 0] = this.position_.X;
        this.positionData_[positionOffset + 1] = this.position_.Y;
        this.positionData_[positionOffset + 2] = this.position_.Z;

        var normalOffset = NORMAL_SIZE_ * i;
        this.normalData_[normalOffset + 0] = this.normal_.X;
        this.normalData_[normalOffset + 1] = this.normal_.Y;
        this.normalData_[normalOffset + 2] = this.normal_.Z;

        var uv0 = vertex.GetUv(0);
        var uv0Offset = UV0_SIZE_ * i;
        this.uv0Data_[uv0Offset + 0] = uv0?.U ?? 0;
        this.uv0Data_[uv0Offset + 1] = uv0?.V ?? 0;
      }

      GL.BindVertexArray(this.vaoId_);

      // Position
      var vertexAttribPosition = 0;
      GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[0]);
      GL.BufferData(BufferTarget.ArrayBuffer,
                    new IntPtr(sizeof(float) * this.positionData_.Length),
                    this.positionData_,
                    BufferUsageHint.DynamicDraw);
      GL.VertexAttribPointer(
          vertexAttribPosition,
          POSITION_SIZE_,
          VertexAttribPointerType.Float,
          false,
          0,
          0);
      GL.EnableVertexAttribArray(vertexAttribPosition);

      // Normal
      var vertexAttribNormal = 1;
      GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[1]);
      GL.BufferData(BufferTarget.ArrayBuffer,
                    new IntPtr(sizeof(float) * this.normalData_.Length),
                    this.normalData_,
                    BufferUsageHint.DynamicDraw);
      GL.VertexAttribPointer(
          vertexAttribNormal,
          NORMAL_SIZE_,
          VertexAttribPointerType.Float,
          false,
          0,
          0);
      GL.EnableVertexAttribArray(vertexAttribNormal);

      // Uv0
      var vertexAttribUv0 = 2;
      GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[2]);
      GL.BufferData(BufferTarget.ArrayBuffer,
                    new IntPtr(sizeof(float) * this.uv0Data_.Length),
                    this.uv0Data_,
                    BufferUsageHint.DynamicDraw);
      GL.VertexAttribPointer(
          vertexAttribUv0,
          UV0_SIZE_,
          VertexAttribPointerType.Float,
          false,
          0,
          0);
      GL.EnableVertexAttribArray(vertexAttribUv0);

      // Make sure the buffers are not changed by outside code
      GL.BindVertexArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public GlBufferRenderer CreateRenderer(
        IEnumerable<(IVertex, IVertex, IVertex)> triangles)
      => new(this.vaoId_, triangles);

    public class GlBufferRenderer : IDisposable {
      private readonly int vaoId_;
      private int eboId_;

      private readonly int[] indices_;

      public GlBufferRenderer(
          int vaoId,
          IEnumerable<(IVertex, IVertex, IVertex)>
              triangles) {
        this.vaoId_ = vaoId;

        GL.BindVertexArray(this.vaoId_);
        GL.GenBuffers(1, out this.eboId_);

        this.indices_ = triangles.SelectMany(triangle => new[] {
                                     triangle.Item1.Index,
                                     triangle.Item2.Index,
                                     triangle.Item3.Index
                                 })
                                 .ToArray();

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboId_);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
                      new IntPtr(sizeof(int) * this.indices_.Length),
                      this.indices_,
                      BufferUsageHint.StaticDraw);

        GL.BindVertexArray(0);
      }

      ~GlBufferRenderer() => ReleaseUnmanagedResources_();

      public void Dispose() {
        ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        GL.DeleteBuffers(1, ref this.eboId_);
      }

      public void Render() {
        GL.BindVertexArray(this.vaoId_);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboId_);

        GL.DrawElements(BeginMode.Triangles,
                        this.indices_.Length,
                        DrawElementsType.UnsignedInt,
                        0);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
      }
    }
  }
}