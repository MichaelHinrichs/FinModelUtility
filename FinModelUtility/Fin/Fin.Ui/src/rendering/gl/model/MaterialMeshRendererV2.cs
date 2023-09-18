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
        ILighting? lighting,
        IList<IPrimitive> primitives) {
      this.material_ = material;

      this.materialShader_ = GlMaterialShader.FromMaterial(model,
        material,
        boneTransformManager,
        lighting);

      PrimitiveType primitiveType;
      bool isFlipped;
      IReadOnlyList<IReadOnlyVertex> primitiveVertices;

      var primitiveTypes =
          primitives.Select(primitive => primitive.Type).ToHashSet();

      if (primitives.Count == 1) {
        var firstPrimitive = primitives[0];
        primitiveType = firstPrimitive.Type;
        isFlipped = firstPrimitive.VertexOrder == VertexOrder.FLIP;
        primitiveVertices = firstPrimitive.Vertices;
      } else if (primitiveTypes.Count == 1 &&
                 primitiveTypes.First() is PrimitiveType.LINES
                                           or PrimitiveType.POINTS) {
        primitiveType = primitiveTypes.First();
        isFlipped = false;
        primitiveVertices = primitives
                            .SelectMany(primitive => primitive.Vertices)
                            .ToArray();
      } else {
        primitiveType = PrimitiveType.TRIANGLES;
        isFlipped = false;
        primitiveVertices = primitives
                            .SelectMany(primitive
                                            => primitive
                                                .GetOrderedTriangleVertices())
                            .ToArray();
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

      if (this.material_ is IReadOnlyFixedFunctionMaterial
          fixedFunctionMaterial) {
        GlUtil.SetBlendingSeparate(
            fixedFunctionMaterial.ColorBlendEquation,
            fixedFunctionMaterial.ColorSrcFactor,
            fixedFunctionMaterial.ColorDstFactor,
            fixedFunctionMaterial.AlphaBlendEquation,
            fixedFunctionMaterial.AlphaSrcFactor,
            fixedFunctionMaterial.AlphaDstFactor,
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