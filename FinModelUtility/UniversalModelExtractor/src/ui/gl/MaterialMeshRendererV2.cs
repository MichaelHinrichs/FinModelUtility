using fin.gl;
using fin.gl.material;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace uni.ui.gl {
  public class MaterialMeshRendererV2 : IDisposable {
    // TODO: Set up shader for material
    // TODO: Use material's textures

    private readonly GlBufferManager.GlBufferRenderer bufferRenderer_;

    private readonly IMaterial? material_;

    private readonly IGlMaterialShader? materialShader_;

    public MaterialMeshRendererV2(
        GlBufferManager bufferManager,
        IMaterial? material,
        IList<IPrimitive> primitives) {
      this.material_ = material;

      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        this.materialShader_ =
            new GlFixedFunctionMaterialShaderV2(fixedFunctionMaterial);
      } else if (material is IStandardMaterial standardMaterial) {
        this.materialShader_ = new GlStandardMaterialShaderV2(standardMaterial);
      } else if (material != null) {
        this.materialShader_ = new GlSimpleMaterialShaderV2(material);
      } else {
        this.materialShader_ = new GlNullMaterialShaderV2();
      }

      var triangles = primitives.SelectMany(primitive => {
        var triangles = new List<(IVertex, IVertex, IVertex)>();

        var vertices = primitive.Vertices;
        var pointsCount = vertices.Count;
        switch (primitive.Type) {
          case fin.model.PrimitiveType.TRIANGLES: {
              for (var v = 0; v < pointsCount; v += 3) {
                if (primitive.VertexOrder == VertexOrder.FLIP) {
                  triangles.Add((vertices[v + 0],
                                 vertices[v + 2],
                                 vertices[v + 1]));
                } else {
                  triangles.Add((vertices[v + 0],
                                 vertices[v + 1],
                                 vertices[v + 2]));
                }
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

                if (primitive.VertexOrder == VertexOrder.FLIP) {
                  triangles.Add((v1, v3, v2));
                } else {
                  triangles.Add((v1, v2, v3));
                }
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

                if (primitive.VertexOrder == VertexOrder.FLIP) {
                  triangles.Add((v1, v3, v2));
                } else {
                  triangles.Add((v1, v2, v3));
                }
              }
              break;
            }
          default: throw new NotImplementedException();
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
      this.materialShader_?.Dispose();
      this.bufferRenderer_.Dispose();
    }

    public bool UseLighting {
      get => this.materialShader_?.UseLighting ?? false;
      set {
        if (this.materialShader_ != null) {
          this.materialShader_.UseLighting = value;
        }
      }
    }

    public void Render() {
      this.materialShader_?.Use();

      var fixedFunctionMaterial = this.material_ as IFixedFunctionMaterial;
      if (fixedFunctionMaterial != null) {
        GlUtil.SetBlending(fixedFunctionMaterial.BlendMode,
                           fixedFunctionMaterial.SrcFactor,
                           fixedFunctionMaterial.DstFactor,
                           fixedFunctionMaterial.LogicOp);
      }

      GlUtil.SetCulling(this.material_?.CullingMode ?? CullingMode.SHOW_BOTH);
      GlUtil.SetDepth(this.material_?.DepthMode ?? DepthMode.USE_DEPTH_BUFFER,
          this.material_?.DepthCompareType ?? DepthCompareType.LEqual);

      this.bufferRenderer_.Render();

      for (var i = 0; i < 8; ++i) {
        GL.ActiveTexture(TextureUnit.Texture0 + i);
        GL.BindTexture(TextureTarget.Texture2D, -1);
      }
      if (fixedFunctionMaterial != null) {
        GlUtil.ResetBlending();
      }

      GlUtil.ResetDepth();
    }
  }
}