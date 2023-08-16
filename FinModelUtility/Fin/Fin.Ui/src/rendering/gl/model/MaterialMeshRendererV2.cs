using fin.math;
using fin.model;
using fin.ui.rendering.gl.material;

namespace fin.ui.rendering.gl.model {
  public class MaterialMeshRendererV2 : IDisposable {
    // TODO: Set up shader for material
    // TODO: Use material's textures

    private readonly GlBufferManager.GlBufferRenderer bufferRenderer_;

    private readonly IMaterial? material_;

    private readonly IGlMaterialShader? materialShader_;

    public MaterialMeshRendererV2(
        IBoneTransformManager? boneTransformManager,
        GlBufferManager bufferManager,
        IModel model,
        IMaterial? material,
        IList<IPrimitive> primitives) {
      this.material_ = material;

      this.materialShader_ =
          GlMaterialShader.FromMaterial(model,
                                        material,
                                        boneTransformManager,
                                        model.Lighting);

      PrimitiveType primitiveType;
      bool isFlipped;
      IReadOnlyList<IReadOnlyVertex> primitiveVertices;

      if (primitives.Count == 1) {
        var firstPrimitive = primitives[0];
        primitiveType = firstPrimitive.Type;
        isFlipped = firstPrimitive.VertexOrder == VertexOrder.FLIP;
        primitiveVertices = firstPrimitive.Vertices;
      } else {
        primitiveType = PrimitiveType.TRIANGLES;
        isFlipped = false;

        var totalVertexCount = primitives.Sum(primitive => {
          switch (primitive.Type) {
            case PrimitiveType.TRIANGLES:
              return primitive.Vertices.Count;
            case PrimitiveType.TRIANGLE_STRIP:
            case PrimitiveType.TRIANGLE_FAN:
              return (primitive.Vertices.Count - 2) * 3;
            default: throw new NotImplementedException();
          }
        });

        var allVertices = new List<IReadOnlyVertex>(totalVertexCount);
        primitiveVertices = allVertices;

        foreach (var primitive in primitives) {
          allVertices.AddRange(primitive.GetOrderedTriangleVertices());
        }
      }

      this.bufferRenderer_ = bufferManager.CreateRenderer(primitiveType,
        primitiveVertices,
        isFlipped);
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

    public IMesh Mesh { get; }

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
      } else {
        GlUtil.ResetBlending();
      }

      GlUtil.SetCulling(this.material_?.CullingMode ?? CullingMode.SHOW_BOTH);
      GlUtil.SetDepth(
          this.material_?.DepthMode ?? DepthMode.USE_DEPTH_BUFFER,
          this.material_?.DepthCompareType ??
          DepthCompareType.LEqual);

      this.bufferRenderer_.Render();
    }
  }
}