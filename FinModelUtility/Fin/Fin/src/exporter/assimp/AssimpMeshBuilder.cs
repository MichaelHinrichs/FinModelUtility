using System;
using System.Collections.Generic;

using Assimp;

using fin.math.matrix;
using fin.model;
using fin.src.data;


namespace fin.exporter.assimp {
  using FinPrimitiveType = fin.model.PrimitiveType;
  using AssPrimitiveType = Assimp.PrimitiveType;

  public class AssimpMeshBuilder {
    public void BuildAndBindMesh(
        Scene assScene,
        IModel finModel) {
      var meshNode = new Node("meshes");
      assScene.RootNode.Children.Add(meshNode);

      var primitivesByMaterial =
          new ReadOnlyListDictionary<IMaterial, IPrimitive>(
              finModel.Skin.Primitives,
              primitive => primitive.Material);

      var allFinVertices = finModel.Skin.Vertices;
      var allAssVertexLocationsByFinIndex = new Vector3D[allFinVertices.Count];
      var allAssVertexNormalsByFinIndex = new Vector3D[allFinVertices.Count];

      for (var i = 0; i < allFinVertices.Count; ++i) {
        var finVertex = allFinVertices[i];

        var location = finVertex.LocalPosition;
        allAssVertexLocationsByFinIndex[i] =
            new Vector3D(location.X, location.Y, location.Z);

        var normal = finVertex.LocalNormal;
        if (normal != null) {
          allAssVertexNormalsByFinIndex[i] =
              new Vector3D(normal.X, normal.Y, normal.Z);
        }
      }

      var finToAssVertexIndices = new int[allFinVertices.Count];
      var assMeshes = assScene.Meshes;
      foreach (var primitivesAndMaterial in primitivesByMaterial) {
        var meshIndex = meshNode.MeshIndices.Count;
        var meshName = $"mesh {meshIndex}";

        var assMesh = new Mesh(AssPrimitiveType.Triangle);
        meshNode.MeshIndices.Add(meshIndex);
        assMesh.Name = meshName;

        for (var i = 0; i < finToAssVertexIndices.Length; ++i) {
          finToAssVertexIndices[i] = -1;
        }

        // Adds all primitives
        var finVertexIndicesInAssMesh = new List<int>();
        var assVertexCounter = 0;
        foreach (var finPrimitive in primitivesAndMaterial.Value) {
          var finVertices = finPrimitive.Vertices;

          // TODO: Add more support for primitives.
          foreach (var finVertex in finVertices) {
            var finVertexIndex = finVertex.Index;
            if (finToAssVertexIndices[finVertexIndex] == -1) {
              finToAssVertexIndices[finVertexIndex] = assVertexCounter++;
              finVertexIndicesInAssMesh.Add(finVertexIndex);
            }
          }

          switch (finPrimitive.Type) {
            case FinPrimitiveType.TRIANGLES: {
              for (var v = 0; v < finVertices.Count; v += 3) {
                var assFaceVertexIndices = new int[3];

                for (var i = 0; i < 3; ++i) {
                  assFaceVertexIndices[i] =
                      finToAssVertexIndices[finVertices[v + i].Index];
                }

                //assMesh.Faces.Add(new Face(assFaceVertexIndices));
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
                //assMesh.Faces.Add(new Face(assVertexIndices));
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

                //assMesh.Faces.Add(new Face(new[] {a, b, d}));
                //assMesh.Faces.Add(new Face(new[] {b, c, d}));
              }
              break;
            }
            default: throw new NotSupportedException();
          }
        }

        // Adds all vertices to the mesh
        foreach (var finVertexIndex in finVertexIndicesInAssMesh) {
          //assMesh.Vertices.Add(
          //    allAssVertexLocationsByFinIndex[finVertexIndex]);

          // TODO: Are these allowed to be null?
          var normal = allAssVertexNormalsByFinIndex[finVertexIndex];
          //assMesh.Normals.Add(normal);
        }

        // NOTES:
        // - All bones are added for each mesh
        // - All bones must have at least one vertex weight in the list. If none,
        //   this should be vertex 0 with weight 0.
        var assBones = new Dictionary<string, Bone>();

        // Adds all bones to the mesh.
        var finBoneQueue = new Queue<IBone>();
        finBoneQueue.Enqueue(finModel.Skeleton.Root);
        while (finBoneQueue.Count > 0) {
          var finBone = finBoneQueue.Dequeue();
          var boneName = finBone.Name;

          if (boneName != null) {
            var assBone = new Bone {Name = boneName};
            //assMesh.Bones.Add(assBone);

            assBones[boneName] = assBone;
          }

          foreach (var childBone in finBone.Children) {
            finBoneQueue.Enqueue(childBone);
          }
        }

        // Adds weights for each bone.
        for (var i = 0; i < finVertexIndicesInAssMesh.Count; ++i) {
          var finVertexIndex = finVertexIndicesInAssMesh[i];
          var finVertex = allFinVertices[finVertexIndex];

          var finWeights = finVertex.Weights;
          if (finWeights != null) {
            foreach (var finWeight in finWeights) {
              var boneName = finWeight.Bone.Name;
              var assBone = assBones[boneName];

              if (assBone.VertexWeights.Count == 0) {
                var assOffsetMatrix = new Matrix4x4();
                MatrixConversionUtil.CopyFinIntoAssimp(
                    finWeight.SkinToBone,
                    ref assOffsetMatrix);
                assBone.OffsetMatrix = assOffsetMatrix;
              }

              assBone.VertexWeights.Add(
                  new VertexWeight(i, finWeight.Weight));
            }
          }
        }

        // Verifies that all bones have at least one weight.
        // (Is this needed?)
        foreach (var assBone in assBones.Values) {
          if (assBone.VertexWeights.Count == 0) {
            assBone.VertexWeights.Add(new VertexWeight(0, 0));
          }
        }
      }
    }
  }
}