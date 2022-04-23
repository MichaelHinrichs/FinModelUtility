using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;

using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;

using glo.schema;

using System.Numerics;

using fin.data;
using fin.util.image;


namespace glo.api {
  public class ModelConverter {
    public unsafe IModel Convert(
        Glo glo,
        IDirectory outputDirectory,
        IList<IDirectory> textureDirectories,
        float fps) {
      var finModel = new ModelImpl();
      var finSkin = finModel.Skin;

      var finRootBone = finModel.Skeleton.Root;

      var finTextureMap = new LazyDictionary<string, ITexture?>(
          textureFilename => {
            foreach (var textureDirectory in textureDirectories) {
              try {
                var textureFile =
                    textureDirectory.GetExistingFile(textureFilename);
                if (textureFile != null) {
                  var rawTextureImage =
                      (Bitmap.FromFile(textureFile.FullName) as Bitmap)!;

                  var textureWidth = rawTextureImage.Width;
                  var textureHeight = rawTextureImage.Height;

                  var textureImageWithAlpha = new Bitmap(
                      textureWidth, textureHeight, PixelFormat.Format32bppArgb);
                  BitmapUtil.InvokeAsLocked(
                      textureImageWithAlpha,
                      bmpData => {
                        var ptr = (byte*) bmpData.Scan0;
                        for (var y = 0; y < textureHeight; ++y) {
                          for (var x = 0; x < textureWidth; ++x) {
                            var i = 4 * (y * textureWidth + x);

                            // TODO: Slow as heck
                            var pixel = rawTextureImage.GetPixel(x, y);
                            
                            var r = pixel.R;
                            var g = pixel.G;
                            var b = pixel.B;
                            var a = pixel.A;

                            if (r == 255 && g == 0 && b == 255) {
                              ptr[i] = 0;
                            } else {
                              ptr[i] = b;
                              ptr[i + 1] = g;
                              ptr[i + 2] = r;
                              ptr[i + 3] = a;
                            }
                          }
                        }
                      });

                  var finTexture = finModel.MaterialManager.CreateTexture(
                      textureImageWithAlpha);
                  finTexture.WrapModeU = WrapMode.MIRROR_REPEAT;
                  finTexture.WrapModeV = WrapMode.MIRROR_REPEAT;

                  return finTexture;
                }
              } catch { }
            }
            return null;
          });
      var withCullingMap =
          new LazyDictionary<string, IMaterial>(textureFilename => {
            var finTexture = finTextureMap[textureFilename];
            if (finTexture == null) {
              return finModel.MaterialManager.AddStandardMaterial();
            }
            return finModel.MaterialManager.AddTextureMaterial(finTexture);
          });
      var withoutCullingMap = new LazyDictionary<string, IMaterial>(
          textureFilename => {
            var finTexture = finTextureMap[textureFilename];
            IMaterial finMaterial = finTexture == null
                                        ? finModel.MaterialManager
                                                  .AddStandardMaterial()
                                        : finModel.MaterialManager
                                                  .AddTextureMaterial(
                                                      finTexture);
            finMaterial.CullingMode = CullingMode.SHOW_BOTH;
            return finMaterial;
          });

      // TODO: Consider separating these out as separate models
      foreach (var gloObject in glo.Objects) {
        var finObjectRootBone = finRootBone.AddRoot(0, 0, 0);
        var meshQueue = new Queue<(GloMesh, IBone)>();
        meshQueue.Enqueue((gloObject.Meshes[0], finObjectRootBone));

        var finAnimation = finModel.AnimationManager.AddAnimation();
        finAnimation.FrameRate = fps;

        while (meshQueue.Count > 0) {
          var (gloMesh, parentFinBone) = meshQueue.Dequeue();

          var finMesh = finSkin.AddMesh();
          finMesh.Name = new string(gloMesh.Name).Replace("\0", "");

          var position = gloMesh.MoveKeys[0];

          var rotation = gloMesh.RotateKeys[0];
          var quaternion =
              new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
          var xyzRadians = QuaternionUtil.ToEulerRadians(quaternion);

          var scale = gloMesh.ScaleKeys[0];

          var finBone = parentFinBone
                        .AddChild(position.X, position.Y, position.Z)
                        .SetLocalRotationRadians(
                            xyzRadians.X, xyzRadians.Y, xyzRadians.Z)
                        .SetLocalScale(scale.X, scale.Y, scale.Z);

          var finBoneTracks = finAnimation.AddBoneTracks(finBone);
          foreach (var moveKey in gloMesh.MoveKeys) {
            finAnimation.FrameCount =
                Math.Max(finAnimation.FrameCount, (int) moveKey.Time + 1);

            finBoneTracks.Positions.Set((int) moveKey.Time, 0, moveKey.X);
            finBoneTracks.Positions.Set((int) moveKey.Time, 1, moveKey.Y);
            finBoneTracks.Positions.Set((int) moveKey.Time, 2, moveKey.Z);
          }
          foreach (var rotateKey in gloMesh.RotateKeys) {
            finAnimation.FrameCount =
                Math.Max(finAnimation.FrameCount, (int) rotateKey.Time + 1);

            var quaternionKey =
                new Quaternion(rotateKey.X, rotateKey.Y, rotateKey.Z,
                               rotateKey.W);
            var xyzRadiansKey = QuaternionUtil.ToEulerRadians(quaternionKey);

            finBoneTracks.Rotations.Set((int) rotateKey.Time, 0,
                                        xyzRadiansKey.X);
            finBoneTracks.Rotations.Set((int) rotateKey.Time, 1,
                                        xyzRadiansKey.Y);
            finBoneTracks.Rotations.Set((int) rotateKey.Time, 2,
                                        xyzRadiansKey.Z);
          }
          foreach (var scaleKey in gloMesh.ScaleKeys) {
            finAnimation.FrameCount =
                Math.Max(finAnimation.FrameCount, (int) scaleKey.Time + 1);

            finBoneTracks.Scales.Set((int) scaleKey.Time, 0, scaleKey.X);
            finBoneTracks.Scales.Set((int) scaleKey.Time, 1, scaleKey.Y);
            finBoneTracks.Scales.Set((int) scaleKey.Time, 2, scaleKey.Z);
          }

          var gloVertices = gloMesh.Vertices;

          var gloFaces = gloMesh.Faces;
          for (var i = 0; i < gloFaces.Length; ++i) {
            var gloFace = gloFaces[i];
            var textureFilename =
                new string(gloFace.TextureFilename).Replace("\0", "");

            var gloFaceColor = gloFace.Color;
            var finFaceColor = ColorImpl.FromRgbaBytes(
                gloFaceColor.R, gloFaceColor.G, gloFaceColor.B, gloFaceColor.A);

            var enableBackfaceCulling = (gloFace.Flags & 1 << 2) != 0;

            var finMaterial = enableBackfaceCulling
                                  ? withCullingMap[textureFilename]
                                  : withoutCullingMap[textureFilename];

            // Face flag:
            // 0: potentially some kind of repeat mode??

            var finFaceVertices = new IVertex[3];
            for (var v = 0; v < 3; ++v) {
              var gloVertexRef = gloFace.VertexRefs[v];
              var gloVertex = gloVertices[gloVertexRef.Index];

              var finVertex = finSkin
                              .AddVertex(gloVertex.X, gloVertex.Y, gloVertex.Z)
                              .SetUv(gloVertexRef.U, gloVertexRef.V);
              finVertex.SetBone(finBone);
              finFaceVertices[v] = finVertex;
            }

            // TODO: Merge triangles together
            var finTriangles = new (IVertex, IVertex, IVertex)[1];
            finTriangles[0] = (finFaceVertices[0], finFaceVertices[2],
                               finFaceVertices[1]);
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