using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;

using fin.exporter.gltf;
using fin.io;
using fin.log;
using fin.math;
using fin.model;
using fin.model.impl;
using fin.src.exporter.assimp.indirect;
using fin.util.asserts;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WrapMode = fin.model.WrapMode;

namespace fin.exporter.assimp {
  using FinBlendMode = fin.model.BlendMode;

  public class AssimpIndirectExporter : IExporter {
    // You can bet your ass I'm gonna prefix everything with ass.

    public void Export(IFile outputFile, IModel model) {
      var outputPath = outputFile.FullName;
      var outputExtension = outputFile.Extension;

      using var ctx = new AssimpContext();

      string exportFormatId;
      {
        var supportedExportFormats = ctx.GetSupportedExportFormats();
        var exportFormatIds =
            supportedExportFormats
                .Where(exportFormat
                           => outputExtension ==
                              $".{exportFormat.FileExtension}")
                .Select(exportFormat => exportFormat.FormatId);
        Asserts.True(exportFormatIds.Any(),
                     $"'{outputExtension}' is not a supported export format!");

        exportFormatId = exportFormatIds.First();
      }

      var assScene = new Scene();
      assScene.RootNode = new Node("ROOT");

      var inputFile = outputFile.CloneWithExtension(".glb");
      var inputPath = inputFile.FullName;

      var gltfExporter = new GltfExporter();
      gltfExporter.UvIndices = true;
      gltfExporter.Export(inputFile, model);
      var sc = ctx.ImportFile(inputPath);

      // [ ]: Get skeleton working
      // [ ]: Get mesh working
      // [ ]: Get UVs working
      // [ ]: Get materials working
      // [ ]: Get multi-bone-binding working

      // Importing the pre-generated GLTF file does most of the hard work off
      // the bat: generating the mesh with properly weighted bones.

      // Bone orientation is already correct, you just need to enable
      // "Automatic Bone Orientation" if importing in Blender.

      new AssimpIndirectAnimationFixer().Fix(model, sc);
      new AssimpIndirectUvFixer().Fix(model, sc);
      new AssimpIndirectTextureFixer().Fix(model, sc);

      var success = ctx.ExportFile(sc, outputPath, exportFormatId);
      Asserts.True(success, "Failed to export model.");

      /*var scene = new Scene();
    
      var animations = model.AnimationManager.Animations;
      var firstAnimation = (animations?.Count ?? 0) > 0 ? animations[0] : null;
    
      if (firstAnimation != null) {
        this.ApplyFirstFrameToSkeleton_(model.Skeleton, firstAnimation);
      }
    
      var boneTransformManager = new BoneTransformManager();
      boneTransformManager.CalculateMatrices(model.Skeleton.Root,
                                             firstAnimation != null
                                                 ? (firstAnimation, 0)
                                                 : null);
    
    
    
      var mesh = new Mesh();
      var meshNode = scene.RootNode.CreateChildNode("mesh", mesh);
    
      var bones = new List<Bone>();
    
      var finToFbxBones = new Dictionary<IBone, Bone>();
      var skd = new SkinDeformer("skeleton");
      {
        var skeleton = model.Skeleton;
    
        var boneQueue = new Queue<(Node, IBone)>();
    
        var fbxSkeletonRoot =
            scene.RootNode.CreateChildNode("skeletonNode", new Skeleton("sk"));
        boneQueue.Enqueue((fbxSkeletonRoot, skeleton.Root));
    
        while (boneQueue.Count > 0) {
          var (fbxBoneNode, finBone) = boneQueue.Dequeue();
    
          var transform = fbxBoneNode.Transform;
    
          var position = finBone.LocalPosition;
          transform.Translation =
              new Vector3(position.X, position.Y, position.Z);
    
          var rotation = finBone.LocalRotation;
          if (rotation != null) {
            transform.Rotation = Quaternion.FromEulerAngle(
                new Vector3(rotation.XRadians,
                            rotation.YRadians,
                            rotation.ZRadians));
          }
    
          var fbxBone = new Bone(finBone.Name + "Bone") {
              Node = fbxBoneNode
          };
    
          fbxBone.BoneTransform = fbxBone.Transform.Inverse();
    
          bones.Add(fbxBone);
          finToFbxBones[finBone] = fbxBone;
    
          foreach (var child in finBone.Children) {
            var sk = new Skeleton(child.Name + "Sk") {Type = SkeletonType.Bone};
    
            var childNode =
                fbxBoneNode.CreateChildNode(child.Name + "Node", sk);
            boneQueue.Enqueue((childNode, child));
          }
        }
      }
    
      {
        IPosition outPosition = new ModelImpl.PositionImpl();
        INormal outNormal = new ModelImpl.NormalImpl();
    
        var vertices = model.Skin.Vertices;
    
        var positions = new Vector4[vertices.Count];
        var normals = new Vector4[vertices.Count];
        foreach (var vertex in vertices) {
          boneTransformManager.ProjectVertex(vertex,
                                             outPosition,
                                             outNormal);
    
          var vertexIndex = vertex.Index;
    
          var hasMultipleWeights = (vertex.Weights?.Count ?? 0) > 1;
    
          var position = outPosition;
          //outNormal = vertex.LocalNormal;
    
          positions[vertexIndex] =
              new Vector4(position.X,
                          position.Y,
                          position.Z,
                          position.W);
          normals[vertexIndex] =
              new Vector4(outNormal.X, outNormal.Y, outNormal.Z, outNormal.W);
    
          if (vertex.Weights != null) {
            foreach (var weight in vertex.Weights) {
              var fbxBone = finToFbxBones[weight.Bone];
              fbxBone.SetWeight(vertexIndex, weight.Weight);
    
              if (hasMultipleWeights && false) {
                var boneMatrix = weight.SkinToBone;
                var boneTransform = fbxBone.BoneTransform;
    
                /*boneTransform.m00 = boneMatrix[0, 0];
                boneTransform.m01 = boneMatrix[0, 1];
                boneTransform.m02 = boneMatrix[0, 2];
                boneTransform.m03 = boneMatrix[0, 3];
    
                boneTransform.m10 = boneMatrix[1, 0];
                boneTransform.m11 = boneMatrix[1, 1];
                boneTransform.m12 = boneMatrix[1, 2];
                boneTransform.m13 = boneMatrix[1, 3];
    
                boneTransform.m20 = boneMatrix[2, 0];
                boneTransform.m21 = boneMatrix[2, 1];
                boneTransform.m22 = boneMatrix[2, 2];
                boneTransform.m23 = boneMatrix[2, 3];
    
                boneTransform.m30 = boneMatrix[3, 0];
                boneTransform.m31 = boneMatrix[3, 1];
                boneTransform.m32 = boneMatrix[3, 2];
                boneTransform.m33 = boneMatrix[3, 3];*/

      /*boneTransform.m00 = boneMatrix[0, 0];
      boneTransform.m10 = boneMatrix[0, 1];
      boneTransform.m20 = boneMatrix[0, 2];
      boneTransform.m30 = boneMatrix[0, 3];

      boneTransform.m01 = boneMatrix[1, 0];
      boneTransform.m11 = boneMatrix[1, 1];
      boneTransform.m21 = boneMatrix[1, 2];
      boneTransform.m31 = boneMatrix[1, 3];

      boneTransform.m02 = boneMatrix[2, 0];
      boneTransform.m12 = boneMatrix[2, 1];
      boneTransform.m22 = boneMatrix[2, 2];
      boneTransform.m32 = boneMatrix[2, 3];

      boneTransform.m03 = boneMatrix[3, 0];
      boneTransform.m13 = boneMatrix[3, 1];
      boneTransform.m23 = boneMatrix[3, 2];
      boneTransform.m33 = boneMatrix[3, 3];

      fbxBone.BoneTransform = boneTransform;
    }
  }
  }
  }

  mesh.ControlPoints.AddRange(positions);

  var normalElement = new VertexElementNormal();
  normalElement.Data.AddRange(normals);
  mesh.AddElement(normalElement);
  }

  foreach (var bone in bones) {
  skd.Bones.Add(bone);
  }

  mesh.Deformers.Add(skd);

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

  var success = ctx.ExportFile(scene, outputPath, exportFormatId);
  Asserts.True(success, "Failed to export model.");
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
  }*/
    }
  }
}