using fin.model;

using OpenTK.Graphics.OpenGL;

using fin.data;

using GL = OpenTK.Graphics.OpenGL.GL;
using PrimitiveType = fin.model.PrimitiveType;


namespace fin.ui.rendering.gl {
  public class GlBufferManager : IDisposable {
    private class VertexArrayObject : IDisposable {
      private const int POSITION_SIZE_ = 3;
      private const int NORMAL_SIZE_ = 3;
      private const int TANGENT_SIZE_ = 4;
      private const int MATRIX_ID_SIZE = 1;
      private const int UV_SIZE_ = 2;
      private const int COLOR_SIZE_ = 4;

      private readonly IReadOnlyList<IReadOnlyVertex> vertices_;
      private readonly IVertexAccessor vertexAccessor_;

      private int vaoId_;

      private int[] vboIds_ = new int[1 + 1 + 1 + 1 + MaterialConstants.MAX_UVS +
                                      MaterialConstants.MAX_COLORS];

      private const int POSITION_VERTEX_ATTRIB_INDEX = 0;

      private const int NORMAL_VERTEX_ATTRIB_INDEX =
          POSITION_VERTEX_ATTRIB_INDEX + 1;

      private const int TANGENT_VERTEX_ATTRIB_INDEX =
          NORMAL_VERTEX_ATTRIB_INDEX + 1;

      private const int MATRIX_ID_VERTEX_ATTRIB_INDEX =
          TANGENT_VERTEX_ATTRIB_INDEX + 1;

      private const int UV_VERTEX_ATTRIB_INDEX =
          MATRIX_ID_VERTEX_ATTRIB_INDEX + 1;

      private const int COLOR_VERTEX_ATTRIB_INDEX =
          UV_VERTEX_ATTRIB_INDEX + MaterialConstants.MAX_UVS;

      private readonly Position[] positionData_;
      private readonly Normal[] normalData_;
      private readonly Tangent[] tangentData_;
      private readonly int[] matrixIdData_;

      private readonly float[][] uvData_;
      private readonly float[][] colorData_;

      public VertexArrayObject(IModel model) {
        this.vertices_ = model.Skin.Vertices;
        this.vertexAccessor_ =
            ConsistentVertexAccessor.GetAccessorForModel(model);

        this.positionData_ = new Position[this.vertices_.Count];
        this.normalData_ = new Normal[this.vertices_.Count];
        this.tangentData_ = new Tangent[this.vertices_.Count];
        this.matrixIdData_ = new int[this.vertices_.Count];

        this.uvData_ = new float[MaterialConstants.MAX_UVS][];
        for (var i = 0; i < this.uvData_.Length; ++i) {
          this.uvData_[i] = new float[UV_SIZE_ * this.vertices_.Count];
        }

        this.colorData_ = new float[MaterialConstants.MAX_COLORS][];
        for (var i = 0; i < this.colorData_.Length; ++i) {
          var colorData = this.colorData_[i] =
              new float[COLOR_SIZE_ * this.vertices_.Count];
          for (var c = 0; c < colorData.Length; ++c) {
            colorData[c] = 1;
          }
        }

        GL.GenVertexArrays(1, out this.vaoId_);
        GL.GenBuffers(this.vboIds_.Length, this.vboIds_);

        this.InitializeStatic_();
      }

      ~VertexArrayObject() => ReleaseUnmanagedResources_();

      public void Dispose() {
        ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        GL.DeleteVertexArrays(1, ref this.vaoId_);
        GL.DeleteBuffers(this.vboIds_.Length, this.vboIds_);
      }

      public int VaoId => this.vaoId_;

      private void InitializeStatic_() {
        for (var i = 0; i < this.vertices_.Count; ++i) {
          this.vertexAccessor_.Target(this.vertices_[i]);
          var vertex = this.vertexAccessor_;

          this.positionData_[i] = vertex.LocalPosition;
          this.normalData_[i] = vertex.LocalNormal.GetValueOrDefault();
          this.tangentData_[i] = vertex.LocalTangent.GetValueOrDefault();
          this.matrixIdData_[i] = (vertex.BoneWeights?.Index ?? -1) + 1;

          var uvCount = Math.Min(this.uvData_.Length, vertex.UvCount);
          for (var u = 0; u < uvCount; ++u) {
            var uv = vertex.GetUv(u);
            if (uv != null) {
              var uvOffset = UV_SIZE_ * i;
              var uvData = this.uvData_[u];
              uvData[uvOffset + 0] = uv?.U ?? 0;
              uvData[uvOffset + 1] = uv?.V ?? 0;
            }
          }

          var colorCount = Math.Min(this.colorData_.Length, vertex.ColorCount);
          for (var c = 0; c < colorCount; ++c) {
            var color = vertex.GetColor(c);
            if (color != null) {
              var colorOffset = COLOR_SIZE_ * i;
              var colorData = this.colorData_[c];
              colorData[colorOffset + 0] = color?.Rf ?? 1;
              colorData[colorOffset + 1] = color?.Gf ?? 1;
              colorData[colorOffset + 2] = color?.Bf ?? 1;
              colorData[colorOffset + 3] = color?.Af ?? 1;
            }
          }
        }

        GL.BindVertexArray(this.vaoId_);

        // Position
        var vertexAttribPosition = POSITION_VERTEX_ATTRIB_INDEX;
        GL.BindBuffer(BufferTarget.ArrayBuffer,
                      this.vboIds_[vertexAttribPosition]);
        GL.BufferData(BufferTarget.ArrayBuffer,
                      new IntPtr(sizeof(float) * POSITION_SIZE_ *
                                 this.positionData_.Length),
                      this.positionData_,
                      BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(vertexAttribPosition);
        GL.VertexAttribPointer(
            vertexAttribPosition,
            POSITION_SIZE_,
            VertexAttribPointerType.Float,
            false,
            0,
            0);

        // Normal
        var vertexAttribNormal = NORMAL_VERTEX_ATTRIB_INDEX;
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[vertexAttribNormal]);
        GL.BufferData(BufferTarget.ArrayBuffer,
                      new IntPtr(sizeof(float) * NORMAL_SIZE_ *
                                 this.normalData_.Length),
                      this.normalData_,
                      BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(vertexAttribNormal);
        GL.VertexAttribPointer(
            vertexAttribNormal,
            NORMAL_SIZE_,
            VertexAttribPointerType.Float,
            false,
            0,
            0);

        // Tangent
        var vertexAttribTangent = TANGENT_VERTEX_ATTRIB_INDEX;
        GL.BindBuffer(BufferTarget.ArrayBuffer,
                      this.vboIds_[vertexAttribTangent]);
        GL.BufferData(BufferTarget.ArrayBuffer,
                      new IntPtr(sizeof(float) * TANGENT_SIZE_ *
                                 this.tangentData_.Length),
                      this.tangentData_,
                      BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(vertexAttribTangent);
        GL.VertexAttribPointer(
            vertexAttribTangent,
            TANGENT_SIZE_,
            VertexAttribPointerType.Float,
            false,
            0,
            0);

        // Matrix id
        var vertexAttribMatrixId = MATRIX_ID_VERTEX_ATTRIB_INDEX;
        GL.BindBuffer(BufferTarget.ArrayBuffer,
                      this.vboIds_[vertexAttribMatrixId]);
        GL.BufferData(BufferTarget.ArrayBuffer,
                      new IntPtr(sizeof(int) * MATRIX_ID_SIZE *
                                 this.matrixIdData_.Length),
                      this.matrixIdData_,
                      BufferUsageHint.StaticDraw);
        GL.EnableVertexAttribArray(vertexAttribMatrixId);
        GL.VertexAttribIPointer(
            vertexAttribMatrixId,
            MATRIX_ID_SIZE,
            VertexAttribIPointerType.Int,
            0,
            0);

        // Uv
        for (var i = 0; i < this.uvData_.Length; ++i) {
          var vertexAttribUv = UV_VERTEX_ATTRIB_INDEX + i;
          GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[vertexAttribUv]);
          GL.BufferData(BufferTarget.ArrayBuffer,
                        new IntPtr(sizeof(float) * this.uvData_[i].Length),
                        this.uvData_[i],
                        BufferUsageHint.StaticDraw);
          GL.EnableVertexAttribArray(vertexAttribUv);
          GL.VertexAttribPointer(
              vertexAttribUv,
              UV_SIZE_,
              VertexAttribPointerType.Float,
              false,
              0,
              0);
        }

        // Color
        for (var i = 0; i < this.colorData_.Length; ++i) {
          var vertexAttribColor = COLOR_VERTEX_ATTRIB_INDEX + i;
          GL.BindBuffer(BufferTarget.ArrayBuffer,
                        this.vboIds_[vertexAttribColor]);
          GL.BufferData(BufferTarget.ArrayBuffer,
                        new IntPtr(sizeof(float) * this.colorData_[i].Length),
                        this.colorData_[i],
                        BufferUsageHint.StaticDraw);
          GL.EnableVertexAttribArray(vertexAttribColor);
          GL.VertexAttribPointer(
              vertexAttribColor,
              COLOR_SIZE_,
              VertexAttribPointerType.Float,
              false,
              0,
              0);
        }

        // Make sure the buffers are not changed by outside code
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      }
    }

    private static ReferenceCountCacheDictionary<IModel, VertexArrayObject>
        vaoCache_ =
            new(model => new VertexArrayObject(model),
                (_, vao) => vao.Dispose());


    private readonly IModel model_;
    private readonly VertexArrayObject vao_;

    public GlBufferManager(IModel model) {
      this.model_ = model;
      this.vao_ = GlBufferManager.vaoCache_.GetAndIncrement(model);
    }

    ~GlBufferManager() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      GlBufferManager.vaoCache_.DecrementAndMaybeDispose(this.model_);
    }

    public GlBufferRenderer CreateRenderer(
        PrimitiveType primitiveType,
        IReadOnlyList<IReadOnlyVertex> triangleVertices,
        bool isFlipped = false)
      => new(this.vao_.VaoId, primitiveType, isFlipped, triangleVertices);


    public class GlBufferRenderer : IDisposable {
      private readonly int vaoId_;
      private int eboId_;
      private BeginMode beginMode_;
      private readonly bool isFlipped_;

      private readonly int[] indices_;

      public GlBufferRenderer(
          int vaoId,
          PrimitiveType primitiveType,
          bool isFlipped,
          IReadOnlyList<IReadOnlyVertex> triangleVertices) {
        this.vaoId_ = vaoId;
        this.beginMode_ = primitiveType switch {
            PrimitiveType.POINTS         => BeginMode.Points,
            PrimitiveType.LINES          => BeginMode.Lines,
            PrimitiveType.TRIANGLES      => BeginMode.Triangles,
            PrimitiveType.TRIANGLE_FAN   => BeginMode.TriangleFan,
            PrimitiveType.TRIANGLE_STRIP => BeginMode.TriangleStrip,
            PrimitiveType.QUADS          => BeginMode.Quads,
        };
        this.isFlipped_ = isFlipped;

        GL.BindVertexArray(this.vaoId_);
        GL.GenBuffers(1, out this.eboId_);

        this.indices_ =
            triangleVertices.Select(vertex => vertex.Index).ToArray();

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
        GlUtil.SetFlipFaces(this.isFlipped_);
        GlUtil.BindVao(this.vaoId_);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.eboId_);

        GL.DrawElements(
            this.beginMode_,
            this.indices_.Length,
            DrawElementsType.UnsignedInt,
            0);
      }
    }
  }
}