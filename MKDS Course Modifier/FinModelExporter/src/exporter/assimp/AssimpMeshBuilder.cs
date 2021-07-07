using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;

using fin.model;
using fin.src.data;


namespace fin.exporter.assimp {
  using FinPrimitiveType = fin.model.PrimitiveType;
  using AssPrimitiveType = Assimp.PrimitiveType;

  public class AssimpMeshBuilder {
    public void BuildAndBindMesh(
        Scene assScene,
        IModel finModel) {
      var primitivesByMaterial =
          new ReadOnlyListDictionary<IMaterial, IPrimitive>(
              finModel.Skin.Primitives,
              primitive => primitive.Material);

      var allFinVertices = finModel.Skin.Vertices;
      var allAssVertexLocationsByFinIndex = new Vector3D[allFinVertices.Count];

      for (var i = 0; i < allFinVertices.Count; ++i) {
        var finVertex = allFinVertices[i];

        var location = finVertex.LocalPosition;
        allAssVertexLocationsByFinIndex[i] =
            new Vector3D(location.X, location.Y, location.Z);
      }

      var finToAssVertexIndices = new int[allFinVertices.Count];
      var assMeshes = assScene.Meshes;
      foreach (var primitivesAndMaterial in primitivesByMaterial) {
        var assMesh = new Mesh(AssPrimitiveType.Triangle);

        for (var i = 0; i < finToAssVertexIndices.Length; ++i) {
          finToAssVertexIndices[i] = -1;
        }

        var assVertexCounter = 0;
        foreach (var finPrimitive in primitivesAndMaterial.Value) {
          var finVertices = finPrimitive.Vertices;

          // TODO: Add more support for primitives.
          foreach (var finVertex in finVertices) {
            var finVertexIndex = finVertex.Index;
            if (finToAssVertexIndices[finVertexIndex] == -1) {
              finToAssVertexIndices[finVertexIndex] = assVertexCounter++;
              assMesh.Vertices.Add(
                  allAssVertexLocationsByFinIndex[finVertexIndex]);
            }
          }

          switch (finPrimitive.Type) {
            case FinPrimitiveType.TRIANGLES: {
              for (var v = 0; v < finVertices.Count; v += 3) {
                var assVertexIndices = new int[3];

                for (var i = 0; i < 3; ++i) {
                  assVertexIndices[i] =
                      finToAssVertexIndices[finVertices[v + i].Index];
                }

                assMesh.Faces.Add(new Face(assVertexIndices));
              }
              break;
            }
            case FinPrimitiveType.TRIANGLE_STRIP: {
              for (var v = 0; v < finVertices.Count - 2; ++v) {
                var assVertexIndices = new int[3];
                if (v % 2 == 0) {
                  for (var i = 0; i < 3; ++i) {
                    assVertexIndices[i] =
                        finToAssVertexIndices[finVertices[v + i].Index];
                  }
                } else {
                  // Switches drawing order to maintain proper winding:
                  // https://www.khronos.org/opengl/wiki/Primitive
                  for (var i = 0; i < 3; ++i) {
                    var swizzle = i < 2 ? 1 - i : i;
                    assVertexIndices[i] =
                        finToAssVertexIndices[finVertices[v + swizzle].Index];
                  }
                }
                assMesh.Faces.Add(new Face(assVertexIndices));
              }
              break;
            }
            case FinPrimitiveType.QUADS: {
              for (var v = 0; v < finVertices.Count; v += 4) {
                // TODO: Factor in distance like this approach:
                // http://james-ramsden.com/triangulate-a-quad-mesh-in-c/
                var a = finToAssVertexIndices[finVertices[v + 0].Index];
                var b = finToAssVertexIndices[finVertices[v + 1].Index];
                var c = finToAssVertexIndices[finVertices[v + 2].Index];
                var d = finToAssVertexIndices[finVertices[v + 3].Index];

                assMesh.Faces.Add(new Face(new[] {a, b, d}));
                assMesh.Faces.Add(new Face(new[] {b, c, d}));
              }
              break;
            }
            default: throw new NotSupportedException();
          }

          assMeshes.Add(assMesh);
        }
      }
    }
  }
}