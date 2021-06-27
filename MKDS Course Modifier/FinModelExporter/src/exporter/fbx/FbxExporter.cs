using System;
using System.Collections.Generic;

using Aspose.ThreeD;
using Aspose.ThreeD.Deformers;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;

using fin.math;
using fin.model;
using fin.model.impl;
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

      var finToFbxBones = new Dictionary<IBone, Bone>();
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

          finToFbxBones[finBone] = fbxBone;
          skd.Bones.Add(fbxBone);

          foreach (var child in finBone.Children) {
            var childNode = fbxBoneNode.CreateChildNode(child.Name + "Node");
            boneQueue.Enqueue((childNode, child));
          }
        }

        mesh.Deformers.Add(skd);
      }

      {
        var outPosition = new ModelImpl.PositionImpl();
        var outNormal = new ModelImpl.NormalImpl();
        var boneTransformManager = new BoneTransformManager();

        boneTransformManager.CalculateMatrices(model.Skeleton.Root,
                                               firstAnimation != null
                                                   ? (firstAnimation, 0)
                                                   : null);

        var vertices = model.Skin.Vertices;

        var positions = new Vector4[vertices.Count];
        var normals = new Vector4[vertices.Count];
        foreach (var vertex in vertices) {
          boneTransformManager.ProjectVertex(vertex,
                                             outPosition,
                                             outNormal);

          var vertexIndex = vertex.Index;

          positions[vertexIndex] =
              new Vector4(outPosition.X,
                          outPosition.Y,
                          outPosition.Z,
                          outPosition.W);
          normals[vertexIndex] =
              new Vector4(outNormal.X, outNormal.Y, outNormal.Z, outNormal.W);

          if (vertex.Weights != null) {
            foreach (var weight in vertex.Weights) {
              var fbxBone = finToFbxBones[weight.Bone];
              fbxBone.SetWeight(vertexIndex, weight.Weight);
            }
          }
        }

        mesh.ControlPoints.AddRange(positions);

        var normalElement = new VertexElementNormal();
        normalElement.Data.AddRange(normals);

        mesh.AddElement(normalElement);

        // TODO: Support normals.
      }

      {
        var polygonBuilder = new PolygonBuilder(mesh);

        foreach (var primitive in model.Skin.Primitives) {
          var points = primitive.Vertices;
          var pointsCount = points.Count;

          switch (primitive.Type) {
            case PrimitiveType.TRIANGLES: {
              for (var v = 0; v < pointsCount; v += 3) {
                polygonBuilder.Begin();
                polygonBuilder.AddVertex(points[v + 0].Index);
                polygonBuilder.AddVertex(points[v + 1].Index);
                polygonBuilder.AddVertex(points[v + 2].Index);
                polygonBuilder.End();
              }
              break;
            }
            case PrimitiveType.TRIANGLE_STRIP: {
              for (var v = 0; v < pointsCount - 2; ++v) {
                polygonBuilder.Begin();
                if (v % 2 == 0) {
                  polygonBuilder.AddVertex(points[v + 0].Index);
                  polygonBuilder.AddVertex(points[v + 1].Index);
                  polygonBuilder.AddVertex(points[v + 2].Index);
                } else {
                  // Switches drawing order to maintain proper winding:
                  // https://www.khronos.org/opengl/wiki/Primitive
                  polygonBuilder.AddVertex(points[v + 1].Index);
                  polygonBuilder.AddVertex(points[v + 0].Index);
                  polygonBuilder.AddVertex(points[v + 2].Index);
                }
                polygonBuilder.End();
              }
              break;
            }
            case PrimitiveType.QUADS: {
              for (var v = 0; v < pointsCount; v += 4) {
                polygonBuilder.Begin();
                polygonBuilder.AddVertex(points[v + 0].Index);
                polygonBuilder.AddVertex(points[v + 1].Index);
                polygonBuilder.AddVertex(points[v + 2].Index);
                polygonBuilder.AddVertex(points[v + 3].Index);
                polygonBuilder.End();
              }
              break;
            }
            default: throw new NotSupportedException();
          }
        }
      }

      {
        //scene.RootNode.AddEntity(PolygonModifier.Triangulate(mesh));
        scene.RootNode.CreateChildNode("mesh", mesh);
      }

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