using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;
using glo.schema;
using System.Drawing;
using System.Numerics;

namespace glo.api {
  public class ModelConverter {
    public IModel Convert(
        Glo glo,
        IDirectory outputDirectory,
        IList<IDirectory> textureDirectories,
        float fps) {
      var finModel = new ModelImpl();

      var finRootBone = finModel.Skeleton.Root;

      foreach (var gloObject in glo.Objects) {
        var finObjectRootBone = finRootBone.AddRoot(0, 0, 0);
        var meshQueue = new Queue<(GloMesh, IBone)>();
        meshQueue.Enqueue((gloObject.Meshes[0], finObjectRootBone));

        var finMaterials = new Dictionary<string, IMaterial>();

        var finAnimation = finModel.AnimationManager.AddAnimation();
        finAnimation.FrameRate = fps;

        while (meshQueue.Count > 0) {
          var (gloMesh, parentFinBone) = meshQueue.Dequeue();

          var position = gloMesh.MoveKeys[0];

          var rotation = gloMesh.RotateKeys[0];
          var quaternion = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
          var xyzRadians = QuaternionUtil.ToEulerRadians(quaternion);

          var scale = gloMesh.ScaleKeys[0];

          // TODO: This does not seem to actually be the bone's position
          var finBone = parentFinBone.AddChild(position.X, position.Y, position.Z).SetLocalRotationRadians(xyzRadians.X, xyzRadians.Y, xyzRadians.Z).SetLocalScale(scale.X, scale.Y, scale.Z);

          var finBoneTracks = finAnimation.AddBoneTracks(finBone);
          foreach (var moveKey in gloMesh.MoveKeys) {
            finAnimation.FrameCount = Math.Max(finAnimation.FrameCount, (int)moveKey.Time + 1);

            finBoneTracks.Positions.Set((int)moveKey.Time, 0, moveKey.X);
            finBoneTracks.Positions.Set((int)moveKey.Time, 1, moveKey.Y);
            finBoneTracks.Positions.Set((int)moveKey.Time, 2, moveKey.Z);
          }
          foreach (var rotateKey in gloMesh.RotateKeys) {
            finAnimation.FrameCount = Math.Max(finAnimation.FrameCount, (int)rotateKey.Time + 1);

            var quaternionKey = new Quaternion(rotateKey.X, rotateKey.Y, rotateKey.Z, rotateKey.W);
            var xyzRadiansKey = QuaternionUtil.ToEulerRadians(quaternionKey);

            finBoneTracks.Rotations.Set((int)rotateKey.Time, 0, xyzRadiansKey.X);
            finBoneTracks.Rotations.Set((int)rotateKey.Time, 1, xyzRadiansKey.Y);
            finBoneTracks.Rotations.Set((int)rotateKey.Time, 2, xyzRadiansKey.Z);
          }
          foreach (var scaleKey in gloMesh.ScaleKeys) {
            finAnimation.FrameCount = Math.Max(finAnimation.FrameCount, (int)scaleKey.Time + 1);

            finBoneTracks.Scales.Set((int)scaleKey.Time, 0, scaleKey.X);
            finBoneTracks.Scales.Set((int)scaleKey.Time, 1, scaleKey.Y);
            finBoneTracks.Scales.Set((int)scaleKey.Time, 2, scaleKey.Z);
          }

          var finSkin = finModel.Skin;
          var gloVertices = gloMesh.Vertices;

          var finMesh = finSkin.AddMesh();

          var gloFaces = gloMesh.Faces;
          for (var i = 0; i < gloFaces.Length; ++i) {
            var gloFace = gloFaces[i];
            var textureFilename = new string(gloFace.TextureFilename).Replace("\0", "");

            var gloFaceColor = gloFace.Color;
            var finFaceColor = ColorImpl.FromRgbaBytes(gloFaceColor.R, gloFaceColor.G, gloFaceColor.B, gloFaceColor.A);

            if (!finMaterials.TryGetValue(textureFilename, out var finMaterial)) {
              foreach (var textureDirectory in textureDirectories) {
                try {
                  var textureFile = textureDirectory.GetExistingFile(textureFilename);
                  if (textureFile != null) {
                    var textureImage = Bitmap.FromFile(textureFile.FullName) as Bitmap;
                    finMaterial = finModel.MaterialManager.AddTextureMaterial(finModel.MaterialManager.CreateTexture(textureImage!));
                    finMaterials[textureFilename] = finMaterial;
                  }
                } catch { }
              }

              if (finMaterial == null) {
                ;
              }
            }

            var finFaceVertices = new IVertex[3];
            for (var v = 0; v < 3; ++v) {
              var gloVertexRef = gloFace.VertexRefs[v];
              var gloVertex = gloVertices[gloVertexRef.Index];

              var finVertex = finSkin.AddVertex(gloVertex.X, gloVertex.Y, gloVertex.Z).SetUv(gloVertexRef.U, gloVertexRef.V);
              finVertex.SetBone(finBone);
              finFaceVertices[v] = finVertex;
            }

            // TODO: Merge triangles together
            var finTriangles = new (IVertex, IVertex, IVertex)[1];
            finTriangles[0] = (finFaceVertices[0], finFaceVertices[2], finFaceVertices[1]);
            finMesh.AddTriangles(finTriangles).SetMaterial(finMaterial!);
          }

          var child = gloMesh.Pointers.Child;
          if (child != null) {
            meshQueue.Enqueue((child, finBone));
          }

          var next = gloMesh.Pointers.Next;
          if (next != null) {
            meshQueue.Enqueue((next, parentFinBone));
          }
        }

        // TODO: Split out animations
      }

      return finModel;
    }
  }
}