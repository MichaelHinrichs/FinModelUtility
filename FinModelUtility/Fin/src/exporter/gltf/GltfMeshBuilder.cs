using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using fin.math;
using fin.model;
using fin.model.impl;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;

using PrimitiveType = fin.model.PrimitiveType;

namespace fin.exporter.gltf {
  using VERTEX =
      VertexBuilder<VertexPositionNormal, VertexColor1Texture2, VertexJoints4>;

  public class GltfMeshBuilder {
    public bool UvIndices { get; set; }

    public Mesh BuildAndBindMesh(
        ModelRoot gltfModel,
        IModel model,
        Dictionary<IMaterial, (IList<byte>, MaterialBuilder)>
            finToTexCoordAndGltfMaterial) {
      var skin = model.Skin;

      var boneTransformManager = new BoneTransformManager();
      var boneToIndex = boneTransformManager.CalculateMatrices(
          model.Skeleton.Root,
          null);

      var meshBuilder = VERTEX.CreateCompatibleMesh();

      var outPosition = new ModelImpl.PositionImpl();
      var outNormal = new ModelImpl.NormalImpl();

      var nullMaterialBuilder =
          new MaterialBuilder("null").WithDoubleSide(false)
                                     .WithSpecularGlossiness();

      foreach (var primitive in skin.Primitives) {
        MaterialBuilder materialBuilder;
        if (primitive.Material != null) {
          (_, materialBuilder) =
              finToTexCoordAndGltfMaterial[primitive.Material];
        } else {
          materialBuilder = nullMaterialBuilder;
        }

        var points = primitive.Vertices;
        var pointsCount = points.Count;
        var vertices = new VERTEX[pointsCount];

        for (var p = 0; p < pointsCount; ++p) {
          var point = points[p];

          boneTransformManager.ProjectVertex(point, outPosition, outNormal);

          var position =
              new Vector3(outPosition.X, outPosition.Y, outPosition.Z);
          // TODO: Don't regenerate the skinning for each vertex, cache this somehow!
          var vertexBuilder = VERTEX.Create(position);

          if (point.Weights != null) {
            vertexBuilder = vertexBuilder.WithSkinning(
                point.Weights.Select(
                         boneWeight
                             => (boneToIndex[boneWeight.Bone],
                                 boneWeight.Weight))
                     .ToArray());
          } else {
            vertexBuilder = vertexBuilder.WithSkinning((0, 1));
          }

          if (point.LocalNormal != null) {
            vertexBuilder = vertexBuilder.WithGeometry(
                position,
                new Vector3(outNormal.X, outNormal.Y, outNormal.Z));
          }

          // TODO: Include color
          var finColor = point.Color;
          var hasColor = finColor != null;
          var assColor = hasColor
                             ? new Vector4(finColor.Rf,
                                           finColor.Bf,
                                           finColor.Gf,
                                           finColor.Af)
                             : new Vector4(1, 1, 1, 1);

          var uvs = point.Uvs;
          var hasUvs = (uvs?.Count ?? 0) > 0;
          if (!this.UvIndices) {
            if (hasUvs) {
              var uv = uvs[0];
              vertexBuilder =
                  vertexBuilder.WithMaterial(assColor,
                                             new Vector2(uv.U, uv.V));
            } else if (hasColor) {
              vertexBuilder = vertexBuilder.WithMaterial(assColor);
            }
          } else {
            // Importing the color directly via Assimp doesn't work for some
            // reason.
            vertexBuilder =
                vertexBuilder.WithMaterial(new Vector4(1, 1, 1, 1),
                                           new Vector2(
                                               hasUvs ? point.Index : -1,
                                               hasColor ? point.Index : -1));
          }

          vertices[p] = vertexBuilder;
        }

        switch (primitive.Type) {
          case PrimitiveType.TRIANGLES: {
            var triangles =
                meshBuilder.UsePrimitive(materialBuilder, 3);
            for (var v = 0; v < pointsCount; v += 3) {
              var v1 = vertices[v + 0];
              var v2 = vertices[v + 1];
              var v3 = vertices[v + 2];

              // Intentionally flipped to fix bug where faces were backwards.
              triangles.AddTriangle(v1, v3, v2);
            }
            break;
          }
          case PrimitiveType.TRIANGLE_STRIP: {
            var triangleStrip =
                meshBuilder.UsePrimitive(materialBuilder, 3);
            for (var v = 0; v < pointsCount - 2; ++v) {
              VERTEX v1, v2, v3;
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
              triangleStrip.AddTriangle(v1, v3, v2);
            }
            break;
          }
          case PrimitiveType.TRIANGLE_FAN: {
            var triangleStrip =
                meshBuilder.UsePrimitive(materialBuilder, 3);

            // https://stackoverflow.com/a/8044252
            var firstVertex = vertices[0];
            for (var v = 2; v < pointsCount; ++v) {
              var v1 = firstVertex;
              var v2 = vertices[v - 1];
              var v3 = vertices[v];

              // Intentionally flipped to fix bug where faces were backwards.
              triangleStrip.AddTriangle(v1, v3, v2);
            }
            break;
          }
          case PrimitiveType.QUADS: {
            var quads =
                meshBuilder.UsePrimitive(
                    materialBuilder,
                    4);
            for (var v = 0; v < pointsCount; v += 4) {
              quads.AddQuadrangle(vertices[v + 0],
                                  vertices[v + 1],
                                  vertices[v + 2],
                                  vertices[v + 3]);
            }
            break;
          }
          default: throw new NotSupportedException();
        }
      }

      return gltfModel.CreateMesh(meshBuilder);
    }
  }
}