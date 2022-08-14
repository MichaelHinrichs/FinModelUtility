using fin.data;
using fin.gl;
using fin.gl.material;
using fin.math;
using fin.model;
using fin.model.impl;

using OpenTK.Graphics.OpenGL;

using PrimitiveType = fin.model.PrimitiveType;
using GlPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;


namespace uni.ui.gl {
  /// <summary>
  ///   A renderer for a Fin model.
  /// </summary>
  public class ModelRenderer : IModelRenderer {
    private readonly BoneTransformManager boneTransformManager_;
    private readonly List<MaterialMeshRenderer> materialMeshRenderers_ = new();

    public ModelRenderer(IModel model,
                         BoneTransformManager boneTransformManager) {
      this.Model = model;
      this.boneTransformManager_ = boneTransformManager;

      var primitivesByMaterial = new ListDictionary<IMaterial?, IPrimitive>();
      foreach (var mesh in model.Skin.Meshes) {
        foreach (var primitive in mesh.Primitives) {
          primitivesByMaterial.Add(primitive.Material, primitive);
        }
      }

      foreach (var (material, primitives) in primitivesByMaterial) {
        materialMeshRenderers_.Add(
            new MaterialMeshRenderer(
                this.boneTransformManager_,
                material,
                primitives));
      }
    }

    ~ModelRenderer() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Dispose();
      }
      materialMeshRenderers_.Clear();
    }

    public IModel Model { get; }

    public void InvalidateDisplayLists() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.InvalidateDisplayLists();
      }
    }

    public void Render() {
      foreach (var materialMeshRenderer in this.materialMeshRenderers_) {
        materialMeshRenderer.Render();
      }
    }
  }

  /// <summary>
  ///   A renderer for all of the primitives of a Fin model with a common material.
  /// </summary>
  public class MaterialMeshRenderer : IMaterialMeshRenderer {
    // TODO: Set up shader for material
    // TODO: Use material's textures

    private readonly BoneTransformManager boneTransformManager_;

    private readonly IMaterial? material_;

    private readonly IList<IPrimitive> primitives_;

    private readonly GlDisplayList displayList_;
    private readonly IGlMaterialShader? materialShader_;


    public MaterialMeshRenderer(BoneTransformManager boneTransformManager,
                                IMaterial? material,
                                IList<IPrimitive> primitives) {
      this.boneTransformManager_ = boneTransformManager;

      this.material_ = material;

      if (DebugFlags.ENABLE_FIXED_FUNCTION_SHADER
          && !DebugFlags.ENABLE_WEIGHT_COLORS
          && material is IFixedFunctionMaterial fixedFunctionMaterial) {
        this.materialShader_ =
            new GlFixedFunctionMaterialShader(fixedFunctionMaterial);
      } else if (material is IStandardMaterial standardMaterial) {
        this.materialShader_ = new GlStandardMaterialShader(standardMaterial);
      } else if (material != null) {
        this.materialShader_ = new GlSimpleMaterialShader(material);
      }

      this.primitives_ = primitives;
      this.displayList_ = new(this.CompileDisplayList_);
    }

    ~MaterialMeshRenderer() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.materialShader_?.Dispose();
      this.displayList_.Dispose();
    }

    public void InvalidateDisplayLists() {
      this.displayList_.Invalidate();
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

      this.displayList_.CompileOrRender();

      for (var i = 0; i < 8; ++i) {
        GL.ActiveTexture(TextureUnit.Texture0 + i);
        GL.BindTexture(TextureTarget.Texture2D, -1);
      }

      if (fixedFunctionMaterial != null) {
        GlUtil.ResetBlending();
      }
    }

    private void CompileDisplayList_() {
      GL.Begin(GlPrimitiveType.Triangles);

      foreach (var primitive in this.primitives_) {
        var vertices = primitive.Vertices;
        var pointsCount = vertices.Count;

        switch (primitive.Type) {
          case PrimitiveType.TRIANGLES: {
            for (var v = 0; v < pointsCount; v += 3) {
              if (primitive.VertexOrder == VertexOrder.FLIP) {
                this.RenderVertex_(vertices[v + 0]);
                this.RenderVertex_(vertices[v + 2]);
                this.RenderVertex_(vertices[v + 1]);
              } else {
                this.RenderVertex_(vertices[v + 0]);
                this.RenderVertex_(vertices[v + 1]);
                this.RenderVertex_(vertices[v + 2]);
              }
            }
            break;
          }
          case PrimitiveType.TRIANGLE_STRIP: {
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
                this.RenderVertex_(v1);
                this.RenderVertex_(v3);
                this.RenderVertex_(v2);
              } else {
                this.RenderVertex_(v1);
                this.RenderVertex_(v2);
                this.RenderVertex_(v3);
              }
            }
            break;
          }
          case PrimitiveType.TRIANGLE_FAN: {
            // https://stackoverflow.com/a/8044252
            var firstVertex = vertices[0];
            for (var v = 2; v < pointsCount; ++v) {
              var v1 = firstVertex;
              var v2 = vertices[v - 1];
              var v3 = vertices[v];

              if (primitive.VertexOrder == VertexOrder.FLIP) {
                this.RenderVertex_(v1);
                this.RenderVertex_(v3);
                this.RenderVertex_(v2);
              } else {
                this.RenderVertex_(v1);
                this.RenderVertex_(v2);
                this.RenderVertex_(v3);
              }
            }
            break;
          }
          case PrimitiveType.QUADS: {
            for (var v = 0; v < pointsCount; v += 4) {
              this.RenderVertex_(vertices[v + 0]);
              this.RenderVertex_(vertices[v + 1]);
              this.RenderVertex_(vertices[v + 2]);
              this.RenderVertex_(vertices[v + 3]);
            }
            break;
          }
        }
      }

      GL.End();
    }

    private readonly IPosition position_ = new ModelImpl.PositionImpl();
    private readonly INormal normal_ = new ModelImpl.NormalImpl();

    private void RenderVertex_(IVertex vertex) {
      // TODO: Load in the matrix instead, so we can perform projection on the GPU.
      this.boneTransformManager_.ProjectVertex(
          vertex, position_, normal_, true);

      var color = vertex.GetColor();
      if (color != null) {
        GL.Color4(color.Rf, color.Gf, color.Bf, color.Af);
      }

      if (DebugFlags.ENABLE_WEIGHT_COLORS) {
        float r = 0, g = 0, b = 0;

        if (vertex.BoneWeights == null) { } else if (vertex.BoneWeights.Weights
                                                         .Count == 0) {
          r = 1;
          g = 1;
        } else if (vertex.BoneWeights.Weights.Count == 1) {
          g = 1;
        } else {
          r = 1;
        }

        if (vertex.BoneWeights?.PreprojectMode == PreprojectMode.NONE) {
          b = 1;
        }

        GL.Color4(r, g, b, 1);
      }

      for (var i = 0; i < 8; ++i) {
        var textureUnit = TextureUnit.Texture0 + i;

        var uvi = vertex.GetUv(i);
        GL.MultiTexCoord2(textureUnit, uvi?.U ?? 0, uvi?.V ?? 0);
      }

      GL.Normal3(normal_.X, normal_.Y, normal_.Z);
      GL.Vertex3(position_.X, position_.Y, position_.Z);
    }
  }
}