using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.asserts;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;

using PrimitiveType = fin.model.PrimitiveType;

namespace fin.exporter.gltf {
  using VERTEX =
      VertexBuilder<VertexPositionNormal, VertexColor1Texture2, VertexJoints4>;

  public class GltfMeshBuilder {
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

      foreach (var primitive in skin.Primitives) {
        var (texCoordIndices, materialBuilder) =
            finToTexCoordAndGltfMaterial[primitive.Material];
        Asserts.Nonnull(materialBuilder);

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
          }

          if (point.LocalNormal != null) {
            vertexBuilder = vertexBuilder.WithGeometry(
                position,
                new Vector3(outNormal.X, outNormal.Y, outNormal.Z));
          }

          // TODO: Include color
          var uvs =
              texCoordIndices
                  .Select(texCoordIndex => point.GetUv(texCoordIndex))
                  .Where(uv => uv != null)
                  .Select(uv => new Vector2(uv.U, uv.V));
          vertexBuilder =
              vertexBuilder.WithMaterial(new Vector4(1, 1, 1, 1),
                                         uvs.ToArray());

          vertices[p] = vertexBuilder;
        }

        switch (primitive.Type) {
          case PrimitiveType.TRIANGLES: {
            var triangles =
                meshBuilder.UsePrimitive(materialBuilder, 3);
            for (var v = 0; v < pointsCount; v += 3) {
              triangles.AddTriangle(vertices[v + 0],
                                    vertices[v + 1],
                                    vertices[v + 2]);
            }
            break;
          }
          case PrimitiveType.TRIANGLE_STRIP: {
            var triangleStrip =
                meshBuilder.UsePrimitive(materialBuilder, 3);
            for (var v = 0; v < pointsCount - 2; ++v) {
              if (v % 2 == 0) {
                triangleStrip.AddTriangle(vertices[v + 0],
                                          vertices[v + 1],
                                          vertices[v + 2]);
              } else {
                // Switches drawing order to maintain proper winding:
                // https://www.khronos.org/opengl/wiki/Primitive
                triangleStrip.AddTriangle(vertices[v + 1],
                                          vertices[v + 0],
                                          vertices[v + 2]);
              }
            }
            break;
          }
          case PrimitiveType.TRIANGLE_FAN: {
            var triangleStrip =
                meshBuilder.UsePrimitive(materialBuilder, 3);

            // https://stackoverflow.com/a/8044252
            var firstVertex = vertices[0];
            var previousVertex = vertices[1];
            for (var v = 2; v < pointsCount; ++v) {
              triangleStrip.AddTriangle(firstVertex,
                                        previousVertex,
                                        vertices[v]);
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