using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;

using System;
using System.Collections.Generic;
using System.Linq;


namespace fin.gl {
  public class GlBufferManager : IDisposable {
    private readonly IReadOnlyList<IVertex> vertices_;

    private readonly Position[] positionData_;
    private readonly Normal[] normalData_;
    private readonly Tangent[] tangentData_;
    private readonly float[][] uvData_;
    private readonly float[][] colorData_;

    private int vaoId_;
    private int[] vboIds_ = new int[1 + 1 + 1 + MaterialConstants.MAX_UVS + MaterialConstants.MAX_COLORS];

    private const int POSITION_SIZE_ = 3;
    private const int NORMAL_SIZE_ = 3;
    private const int TANGENT_SIZE_ = 4;
    private const int UV_SIZE_ = 2;
    private const int COLOR_SIZE_ = 4;

    private bool uvStale_ = true;
    private bool colorStale_ = true;

    public GlBufferManager(IModel model) {
      this.vertices_ = model.Skin.Vertices;

      this.positionData_ = new Position[this.vertices_.Count];
      this.normalData_ = new Normal[this.vertices_.Count];
      this.tangentData_ = new Tangent[this.vertices_.Count];
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

    public unsafe void UpdateTransforms(IBoneTransformManager? boneTransformManager) {
      for (var i = 0; i < this.vertices_.Count; ++i) {
        var vertex = this.vertices_[i];

        Position position;
        Normal normal;
        Tangent tangent;
        if (boneTransformManager != null) {
          boneTransformManager.ProjectVertexPositionNormalTangent(
              vertex,
              out position,
              out normal,
              out tangent);
        } else {
          position = vertex.LocalPosition;
          normal = vertex.LocalNormal.GetValueOrDefault();
          tangent = vertex.LocalTangent.GetValueOrDefault();
        }

        this.positionData_[i] = position;
        this.normalData_[i] = normal;
        this.tangentData_[i] = tangent;

        if (uvStale_) {
          var uvCount = Math.Min(this.uvData_.Length, vertex.Uvs?.Count ?? 0);
          for (var u = 0; u < uvCount; ++u) {
            var uv = vertex.GetUv(u);
            if (uv != null) {
              var uvOffset = UV_SIZE_ * i;
              var uvData = this.uvData_[u];
              uvData[uvOffset + 0] = uv?.U ?? 0;
              uvData[uvOffset + 1] = uv?.V ?? 0;
            }
          }
        }

        if (colorStale_) {
          var colorCount = Math.Min(this.colorData_.Length,
                                    vertex.Colors?.Count ?? 0);
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
      }

      this.uvStale_ = false;
      this.colorStale_ = false;

      GL.BindVertexArray(this.vaoId_);

      var currentVertexAttrib = 0;

      // Position
      var vertexAttribPosition = currentVertexAttrib++;
      GL.BindBuffer(BufferTarget.ArrayBuffer,
                    this.vboIds_[vertexAttribPosition]);
      GL.BufferData(BufferTarget.ArrayBuffer,
                    new IntPtr(sizeof(float) * GlBufferManager.POSITION_SIZE_ * this.positionData_.Length),
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
      var vertexAttribNormal = currentVertexAttrib++;
      GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[vertexAttribNormal]);
      GL.BufferData(BufferTarget.ArrayBuffer,
                    new IntPtr(sizeof(float) * GlBufferManager.NORMAL_SIZE_ * this.normalData_.Length),
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

      // Tangent
      var vertexAttribTangent = currentVertexAttrib++;
      GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[vertexAttribTangent]);
      GL.BufferData(BufferTarget.ArrayBuffer,
                    new IntPtr(sizeof(float) * GlBufferManager.TANGENT_SIZE_ * this.tangentData_.Length),
                    this.tangentData_,
                    BufferUsageHint.DynamicDraw);
      GL.VertexAttribPointer(
          vertexAttribTangent,
          TANGENT_SIZE_,
          VertexAttribPointerType.Float,
          false,
          0,
          0);
      GL.EnableVertexAttribArray(vertexAttribTangent);

      // Uv
      for (var i = 0; i < this.uvData_.Length; ++i) {
        var vertexAttribUv = currentVertexAttrib++;
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.vboIds_[vertexAttribUv]);
        GL.BufferData(BufferTarget.ArrayBuffer,
                      new IntPtr(sizeof(float) * this.uvData_[i].Length),
                      this.uvData_[i],
                      BufferUsageHint.DynamicDraw);
        GL.VertexAttribPointer(
            vertexAttribUv,
            UV_SIZE_,
            VertexAttribPointerType.Float,
            false,
            0,
            0);
        GL.EnableVertexAttribArray(vertexAttribUv);
      }

      // Color
      for (var i = 0; i < this.colorData_.Length; ++i) {
        var vertexAttribColor = currentVertexAttrib++;
        GL.BindBuffer(BufferTarget.ArrayBuffer,
                      this.vboIds_[vertexAttribColor]);
        GL.BufferData(BufferTarget.ArrayBuffer,
                      new IntPtr(sizeof(float) * this.colorData_[i].Length),
                      this.colorData_[i],
                      BufferUsageHint.DynamicDraw);
        GL.VertexAttribPointer(
            vertexAttribColor,
            COLOR_SIZE_,
            VertexAttribPointerType.Float,
            false,
            0,
            0);
        GL.EnableVertexAttribArray(vertexAttribColor);
      }

      // Make sure the buffers are not changed by outside code
      GL.BindVertexArray(0);
      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public GlBufferRenderer CreateRenderer(IReadOnlyList<IVertex> triangleVertices)
      => new(this.vaoId_, triangleVertices);

    public class GlBufferRenderer : IDisposable {
      private readonly int vaoId_;
      private int eboId_;

      private readonly int[] indices_;

      public GlBufferRenderer(
          int vaoId,
          IReadOnlyList<IVertex> triangleVertices) {
        this.vaoId_ = vaoId;

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