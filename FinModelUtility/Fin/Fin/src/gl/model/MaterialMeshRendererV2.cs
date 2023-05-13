using fin.gl.material;
using fin.model;

using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

using fin.math;

using PrimitiveType = fin.model.PrimitiveType;


namespace fin.gl.model {
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

      IReadOnlyList<IVertex> triangleVertices;
      if (primitives is [{ Type: PrimitiveType.TRIANGLES, VertexOrder:
              VertexOrder.NORMAL }]) {
        triangleVertices = primitives[0].Vertices;
      } else {
        triangleVertices = primitives.SelectMany(primitive => {
          var triangleVertices = new List<IVertex>();

          var vertices = primitive.Vertices;
          var pointsCount = vertices.Count;
          switch (primitive.Type) {
            case fin.model.PrimitiveType.TRIANGLES: {
                for (var v = 0; v < pointsCount; v += 3) {
                  if (primitive.VertexOrder == VertexOrder.FLIP) {
                    triangleVertices.Add(vertices[v + 0]);
                    triangleVertices.Add(vertices[v + 2]);
                    triangleVertices.Add(vertices[v + 1]);
                  } else {
                    triangleVertices.Add(vertices[v + 0]);
                    triangleVertices.Add(vertices[v + 1]);
                    triangleVertices.Add(vertices[v + 2]);
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
                    triangleVertices.Add(v1);
                    triangleVertices.Add(v3);
                    triangleVertices.Add(v2);
                  } else {
                    triangleVertices.Add(v1);
                    triangleVertices.Add(v2);
                    triangleVertices.Add(v3);
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
                    triangleVertices.Add(v1);
                    triangleVertices.Add(v3);
                    triangleVertices.Add(v2);
                  } else {
                    triangleVertices.Add(v1);
                    triangleVertices.Add(v2);
                    triangleVertices.Add(v3);
                  }
                }
                break;
              }
            default: throw new NotImplementedException();
          }

          return triangleVertices;
        }).ToArray();

      }

      this.bufferRenderer_ = bufferManager.CreateRenderer(triangleVertices);
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