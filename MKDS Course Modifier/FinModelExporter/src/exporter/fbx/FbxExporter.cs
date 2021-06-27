using System.Collections.Generic;

using Aspose.ThreeD;
using Aspose.ThreeD.Deformers;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;

using fin.math;
using fin.model;
using fin.util.asserts;

namespace fin.exporter.fbx {
  public class FbxExporter : IExporter {
    public void Export(
        string outputPath,
        IModel model) {
      Asserts.True(outputPath.EndsWith(".fbx"));

      var animations = model.AnimationManager.Animations;
      var firstAnimation = (animations?.Count ?? 0) > 0 ? animations[0] : null;

      if (firstAnimation != null) {
        this.ApplyFirstFrameToSkeleton_(model.Skeleton, firstAnimation);
      }

      var scene = new Scene();

      var mesh = new Mesh();

      {
        var skeleton = model.Skeleton;

        var skd = new SkinDeformer();
        var boneQueue = new Queue<(Node, IBone)>();

        scene.RootNode.Name = skeleton.Root.Name + "Node";
        boneQueue.Enqueue((scene.RootNode, skeleton.Root));

        while (boneQueue.Count > 0) {
          var (fbxBoneNode, finBone) = boneQueue.Dequeue();

          var transform = fbxBoneNode.Transform;

          var position = finBone.LocalPosition;
          transform.Translation =
              new Vector3(position.X, position.Y, position.Z);

          var rotation = QuaternionUtil.Create(finBone.LocalRotation);
          transform.Rotation =
              new Quaternion(rotation.W, rotation.X, rotation.Y, rotation.Z);

          var fbxBone = new Bone(finBone.Name + "Bone") {
              Node = fbxBoneNode
          };

          skd.Bones.Add(fbxBone);

          foreach (var child in finBone.Children) {
            var childNode = new Node(child.Name + "Node");
            fbxBoneNode.AddChildNode(childNode);
            boneQueue.Enqueue((childNode, child));
          }
        }

        scene.RootNode.AddEntity(mesh);

        mesh.Deformers.Add(skd);
      }

      /*{
        var vertices = model.Skin.Vertices;

        var positions = new Vector4[vertices.Count];
        foreach (var vertex in vertices) {
          var localPosition = vertex.LocalPosition;
          positions[vertex.Index] = vertex.LocalPosition.
        }
      }

      mesh.ControlPoints.AddRange();


      foreach (var primitive in skin.Primitives) {
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

          vertices[p] = vertexBuilder;
        }

        switch (primitive.Type) {
          case PrimitiveType.TRIANGLES: {
              var triangles =
                  meshBuilder.UsePrimitive(materialBuilder,
                                           3);
              for (var v = 0; v < pointsCount; v += 3) {
                triangles.AddTriangle(vertices[v + 0],
                                      vertices[v + 1],
                                      vertices[v + 2]);
              }
              break;
            }
          case PrimitiveType.TRIANGLE_STRIP: {
              var triangleStrip =
                  meshBuilder.UsePrimitive(materialBuilder,
                                           3);
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


      mesh.CreatePolygon();*/

      scene.Save(outputPath, FileFormat.FBX7700Binary);
    }

    // TODO: Pull this out somewhere else, or make this part of the model creation flow?
    private void ApplyFirstFrameToSkeleton_(
        ISkeleton skeleton,
        IAnimation animation) {
      var boneQueue = new Queue<IBone>();
      boneQueue.Enqueue(skeleton.Root);

      while (boneQueue.Count > 0) {
        var bone = boneQueue.Dequeue();

        animation.BoneTracks.TryGetValue(bone, out var boneTracks);

        var localPosition = boneTracks?.Positions.GetInterpolatedFrame(0);
        bone.SetLocalPosition(localPosition?.X ?? 0,
                              localPosition?.Y ?? 0,
                              localPosition?.Z ?? 0);

        var localRotation = boneTracks?.Rotations.GetKeyframe(0);
        bone.SetLocalRotationRadians(localRotation?.XRadians ?? 0,
                                     localRotation?.YRadians ?? 0,
                                     localRotation?.ZRadians ?? 0);

        // It seems like some animations shrink a bone to 0 to hide it, but
        // this prevents us from calculating a determinant to invert the
        // matrix. As a result, we can't safely include the scale here.
        IScale?
            localScale = null; //boneTracks?.Scales.GetInterpolatedAtFrame(0);
        bone.SetLocalScale(localScale?.X ?? 1,
                           localScale?.Y ?? 1,
                           localScale?.Z ?? 1);

        foreach (var child in bone.Children) {
          boneQueue.Enqueue(child);
        }
      }
    }
  }
}