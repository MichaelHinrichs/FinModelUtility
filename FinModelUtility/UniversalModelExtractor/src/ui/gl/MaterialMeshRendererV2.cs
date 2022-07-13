using fin.gl;
using fin.model;
using fin.util.image;


namespace uni.ui.gl {
  /// </summary>
  public class MaterialMeshRendererV2 : IDisposable {
    // TODO: Set up shader for material
    // TODO: Use material's textures

    private readonly GlBufferManager.GlBufferRenderer bufferRenderer_;

    private readonly GlShaderProgram shaderProgram_;
    private static GlTexture? NULL_TEXTURE_;

    private readonly IMaterial material_;
    private readonly GlTexture? texture_;

    public MaterialMeshRendererV2(
        GlBufferManager bufferManager,
        IMaterial material,
        IList<IPrimitive> primitives) {
      this.material_ = material;

      if (MaterialMeshRendererV2.NULL_TEXTURE_ == null) {
        MaterialMeshRendererV2.NULL_TEXTURE_ =
            new GlTexture(BitmapUtil.Create1x1WithColor(Color.White));
      }

      ITexture? finTexture = material.Textures.FirstOrDefault();

      if (DebugFlags.ENABLE_WEIGHT_COLORS) {
        finTexture = null;
      }

      this.texture_ = finTexture != null
                          ? new GlTexture(finTexture)
                          : MaterialMeshRendererV2.NULL_TEXTURE_;

      var triangles = primitives.SelectMany(primitive => {
        var triangles = new List<(IVertex, IVertex, IVertex)>();

        var vertices = primitive.Vertices;
        var pointsCount = vertices.Count;
        switch (primitive.Type) {
          case fin.model.PrimitiveType.TRIANGLES: {
            for (var v = 0; v < pointsCount; v += 3) {
              triangles.Add((vertices[v + 0], vertices[v + 1],
                             vertices[v + 2]));
            }
            break;
          }
          case fin.model.PrimitiveType.TRIANGLE_STRIP: {
            for (var v = 0; v < pointsCount - 2; ++v) {
              IVertex v1, v2, v3;
              if (v % 2 == 0) {
                v1 = vertices[v + 0];
                v2 = vertices[v + 1];
                v3 = vertices[v + 2];
              } else {
                // Switches drawing order to maintain proper winding:
                // https://www.khronos.org/opengl/wiki/Primitive
                v1 = vertices[v + 1];
                v2 = vertices[v + 0];
                v3 = vertices[v + 2];
              }

              // Intentionally flipped to fix bug where faces were backwards.
              triangles.Add((v1, v3, v2));
            }
            break;
          }
          case fin.model.PrimitiveType.TRIANGLE_FAN: {
            // https://stackoverflow.com/a/8044252
            var firstVertex = vertices[0];
            for (var v = 2; v < pointsCount; ++v) {
              var v1 = firstVertex;
              var v2 = vertices[v - 1];
              var v3 = vertices[v];

              // Intentionally flipped to fix bug where faces were backwards.
              triangles.Add((v1, v3, v2));
            }
            break;
          }
          default: throw new ArgumentOutOfRangeException();
        }

        return triangles;
      });

      this.bufferRenderer_ = bufferManager.CreateRenderer(triangles);
    }

    ~MaterialMeshRendererV2() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      if (this.texture_ != MaterialMeshRendererV2.NULL_TEXTURE_) {
        this.texture_?.Dispose();
      }
      this.shaderProgram_.Dispose();
      this.bufferRenderer_.Dispose();
    }

    public void Render() {
      GlUtil.SetCulling(this.material_.CullingMode);
      this.texture_?.Bind();
      this.bufferRenderer_.Render();
      this.texture_?.Unbind();
    }
  }
}