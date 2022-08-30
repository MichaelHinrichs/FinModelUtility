using System.Drawing;
using System.Drawing.Imaging;

using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;

using glo.schema;

using System.Numerics;

using fin.color;
using fin.data;
using fin.image;
using fin.util.asserts;
using fin.util.image;


namespace glo.api {
  public class GloModelFileBundle : IModelFileBundle {
    public GloModelFileBundle(IFileHierarchyFile gloFile,
                              IReadOnlyList<IFileHierarchyDirectory>
                                  textureDirectories) {
      this.GloFile = gloFile;
      this.TextureDirectories = textureDirectories;
    }

    public IFileHierarchyFile MainFile => this.GloFile;

    public IFileHierarchyFile GloFile { get; }
    public IReadOnlyList<IFileHierarchyDirectory> TextureDirectories { get; }
  }

  public class GloModelLoader : IModelLoader<GloModelFileBundle> {
    private readonly string[] hiddenNames_ = new[] {
        "Box01", "puzzle"
    };

    private readonly string[] mirrorTextures_ = new[] {
        "Badg2.bmp"
    };

    public unsafe IModel LoadModel(GloModelFileBundle gloModelFileBundle) {
      var gloFile = gloModelFileBundle.GloFile;
      var textureDirectories = gloModelFileBundle.TextureDirectories;
      var fps = 20;

      var glo = gloFile.Impl.ReadNew<Glo>(Endianness.LittleEndian);

      var textureFilesByName = new Dictionary<string, IFileHierarchyFile>();
      foreach (var textureDirectory in textureDirectories) {
        foreach (var textureFile in textureDirectory.Files) {
          textureFilesByName[textureFile.Name.ToLower()] = textureFile;
        }
      }

      /*new MeshCsvWriter().WriteToFile(
          glo, new FinFile(Path.Join(outputDirectory.FullName, "mesh.csv")));
      new FaceCsvWriter().WriteToFile(
          glo, new FinFile(Path.Join(outputDirectory.FullName, "face.csv")));
      new VertexCsvWriter().WriteToFile(
          glo, new FinFile(Path.Join(outputDirectory.FullName, "vertex.csv")));*/

      var finModel = new ModelImpl();
      var finSkin = finModel.Skin;

      var finRootBone = finModel.Skeleton.Root;

      var finTextureMap = new LazyDictionary<string, ITexture?>(
          textureFilename => {
            if (!textureFilesByName.TryGetValue(textureFilename.ToLower(),
                                                out var textureFile)) {
              return null;
            }

            using var rawTextureImage = FinImage.FromFile(textureFile.Impl);
            var textureImageWithAlpha =
                GloModelLoader.AddTransparencyToGloImage_(rawTextureImage);

            var finTexture = finModel.MaterialManager.CreateTexture(
                textureImageWithAlpha);
            finTexture.Name = textureFilename;

            if (this.mirrorTextures_.Contains(textureFilename)) {
              finTexture.WrapModeU = WrapMode.MIRROR_REPEAT;
              finTexture.WrapModeV = WrapMode.MIRROR_REPEAT;
            } else {
              finTexture.WrapModeU = WrapMode.REPEAT;
              finTexture.WrapModeV = WrapMode.REPEAT;
            }

            return finTexture;
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

      var firstMeshMap = new Dictionary<string, GloMesh>();

      // TODO: Consider separating these out as separate models
      foreach (var gloObject in glo.Objects) {
        var finObjectRootBone = finRootBone.AddRoot(0, 0, 0);
        var meshQueue = new Queue<(GloMesh, IBone)>();
        foreach (var topLevelGloMesh in gloObject.Meshes) {
          meshQueue.Enqueue((topLevelGloMesh, finObjectRootBone));
        }

        List<(IAnimation, int, int)> finAndGloAnimations = new();
        foreach (var animSeg in gloObject.AnimSegs) {
          var startFrame = (int) animSeg.StartFrame;
          var endFrame = (int) animSeg.EndFrame;

          var finAnimation = finModel.AnimationManager.AddAnimation();
          finAnimation.Name = animSeg.Name;
          finAnimation.FrameCount =
              (int) (animSeg.EndFrame - animSeg.StartFrame + 1);

          finAnimation.FrameRate = fps * animSeg.Speed;

          finAndGloAnimations.Add((finAnimation, startFrame, endFrame));
        }

        while (meshQueue.Count > 0) {
          var (gloMesh, parentFinBone) = meshQueue.Dequeue();

          var name = gloMesh.Name;

          GloMesh idealMesh;
          if (!firstMeshMap.TryGetValue(name, out idealMesh)) {
            firstMeshMap[name] = idealMesh = gloMesh;
          }

          var position = gloMesh.MoveKeys[0].Xyz;

          var rotation = gloMesh.RotateKeys[0];
          var quaternion =
              new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
          var xyzRadians = QuaternionUtil.ToEulerRadians(quaternion);

          var scale = gloMesh.ScaleKeys[0].Scale;

          var finBone = parentFinBone
                        .AddChild(position.X, position.Y, position.Z)
                        .SetLocalRotationRadians(
                            xyzRadians.X, xyzRadians.Y, xyzRadians.Z)
                        .SetLocalScale(scale.X, scale.Y, scale.Z);
          finBone.Name = name + "_bone";

          var child = gloMesh.Pointers.Child;
          if (child != null) {
            meshQueue.Enqueue((child, finBone));
          }

          var next = gloMesh.Pointers.Next;
          if (next != null) {
            meshQueue.Enqueue((next, parentFinBone));
          }

          foreach (var (finAnimation, startFrame, endFrame) in
                   finAndGloAnimations) {
            var finBoneTracks = finAnimation.AddBoneTracks(finBone);

            long prevTime = -1;
            foreach (var moveKey in gloMesh.MoveKeys) {
              Asserts.True(moveKey.Time > prevTime);
              prevTime = moveKey.Time;

              if (!(moveKey.Time >= startFrame && moveKey.Time <= endFrame)) {
                continue;
              }

              var time = (int) (moveKey.Time - startFrame);
              Asserts.True(time >= 0 && time < finAnimation.FrameCount);

              var moveValue = moveKey.Xyz;
              finBoneTracks.Positions.Set(time, 0, moveValue.X);
              finBoneTracks.Positions.Set(time, 1, moveValue.Y);
              finBoneTracks.Positions.Set(time, 2, moveValue.Z);
            }

            prevTime = -1;
            foreach (var rotateKey in gloMesh.RotateKeys) {
              Asserts.True(rotateKey.Time > prevTime);
              prevTime = rotateKey.Time;

              if (!(rotateKey.Time >= startFrame &&
                    rotateKey.Time <= endFrame)) {
                continue;
              }

              var time = (int) (rotateKey.Time - startFrame);
              Asserts.True(time >= 0 && time < finAnimation.FrameCount);

              var quaternionKey =
                  new Quaternion(rotateKey.X, rotateKey.Y, rotateKey.Z,
                                 rotateKey.W);
              var xyzRadiansKey = QuaternionUtil.ToEulerRadians(quaternionKey);

              finBoneTracks.Rotations.Set(time, 0,
                                          xyzRadiansKey.X);
              finBoneTracks.Rotations.Set(time, 1,
                                          xyzRadiansKey.Y);
              finBoneTracks.Rotations.Set(time, 2,
                                          xyzRadiansKey.Z);
            }

            prevTime = -1;
            foreach (var scaleKey in gloMesh.ScaleKeys) {
              Asserts.True(scaleKey.Time > prevTime);
              prevTime = scaleKey.Time;

              if (!(scaleKey.Time >= startFrame && scaleKey.Time <= endFrame)) {
                continue;
              }

              var time = (int) (scaleKey.Time - startFrame);
              Asserts.True(time >= 0 && time < finAnimation.FrameCount);

              var scaleValue = scaleKey.Scale;
              finBoneTracks.Scales.Set(time, 0, scaleValue.X);
              finBoneTracks.Scales.Set(time, 1, scaleValue.Y);
              finBoneTracks.Scales.Set(time, 2, scaleValue.Z);
            }
          }

          // Anything with these names are debug objects and can be ignored.
          if (this.hiddenNames_.Contains(name)) {
            continue;
          }

          var finMesh = finSkin.AddMesh();
          finMesh.Name = name;

          var gloVertices = idealMesh.Vertices;

          string previousTextureName = null;
          IMaterial? previousMaterial = null;

          foreach (var gloFace in idealMesh.Faces) {
            // TODO: What can we do if texture filename is empty?
            var textureFilename = gloFace.TextureFilename;

            var gloFaceColor = gloFace.Color;
            var finFaceColor = FinColor.FromRgbaBytes(
                gloFaceColor.Rb, gloFaceColor.Gb, gloFaceColor.Bb, gloFaceColor.Ab);

            var enableBackfaceCulling = (gloFace.Flags & 1 << 2) == 0;

            IMaterial? finMaterial;
            if (textureFilename == previousTextureName) {
              finMaterial = previousMaterial;
            } else {
              previousTextureName = textureFilename;
              finMaterial = enableBackfaceCulling
                                ? withCullingMap[textureFilename]
                                : withoutCullingMap[textureFilename];
              previousMaterial = finMaterial;
            }

            // Face flag:
            // 0: potentially some kind of repeat mode??

            var color = (gloFace.Flags & 1 << 6) != 0
                            ? FinColor.FromRgbaBytes(255, 0, 0, 255)
                            : FinColor.FromRgbaBytes(0, 255, 0, 255);

            var finFaceVertices = new IVertex[3];
            for (var v = 0; v < 3; ++v) {
              var gloVertexRef = gloFace.VertexRefs[v];
              var gloVertex = gloVertices[gloVertexRef.Index];

              var finVertex = finSkin
                              .AddVertex(gloVertex.X, gloVertex.Y, gloVertex.Z)
                              .SetUv(gloVertexRef.U, gloVertexRef.V);
              //.SetColor(color);
              finVertex.SetBoneWeights(finSkin.GetOrCreateBoneWeights(
                                           PreprojectMode.BONE, finBone));
              finFaceVertices[v] = finVertex;
            }

            // TODO: Merge triangles together
            var finTriangles = new (IVertex, IVertex, IVertex)[1];
            finTriangles[0] = (finFaceVertices[0], finFaceVertices[2],
                               finFaceVertices[1]);
            finMesh.AddTriangles(finTriangles).SetMaterial(finMaterial!);
          }
        }

        // TODO: Split out animations
      }

      return finModel;
    }

    private static IImage AddTransparencyToGloImage_(IImage rawImage) {
      var width = rawImage.Width;
      var height = rawImage.Height;

      var textureImageWithAlpha = new Rgba32Image(width, height);
      textureImageWithAlpha.Mutate((_, setHandler) => {
        rawImage.Access(getHandler => {
          for (var y = 0; y < height; ++y) {
            for (var x = 0; x < width; ++x) {
              getHandler(x, y, out var r, out var g, out var b, out var a);

              if (r == 255 && g == 0 && b == 255) {
                a = 0;
              }

              setHandler(x, y, r, g, b, a);
            }
          }
        });
      });

      return textureImageWithAlpha;
    }
  }
}