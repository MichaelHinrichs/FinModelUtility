using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using fin.math;
using fin.math.matrix;

using bmd.GCN;

using fin.model;
using fin.model.impl;
using fin.util.asserts;

using bmd.G3D_Binary_File_Format;

using fin.io;
using fin.log;

using Tao.OpenGl;


namespace bmd.exporter {
  using MkdsNode = bmd._3D_Formats.MA.Node;

  public class BmdModelFileBundle : IModelFileBundle {
    public IFile BmdFile { get; set; }
    public IReadOnlyList<IFile>? BcxFiles { get; set; }
    public IReadOnlyList<IFile>? BtiFiles { get; set; }
    public float FrameRate { get; set; } = 30;
    public string FileName => this.BmdFile.NameWithoutExtension;
  }

  public class BmdModelLoader : IModelLoader<BmdModelFileBundle> {
    public IModel LoadModel(BmdModelFileBundle modelFileBundle) {
      var logger = Logging.Create<BmdModelLoader>();

      var bmd = new BMD(modelFileBundle.BmdFile.SkimAllBytes());

      List<(string, IBcx)>? pathsAndBcxs;
      try {
        pathsAndBcxs =
            modelFileBundle
                .BcxFiles?
                .Select(bcxFile => {
                  var extension = bcxFile.Extension.ToLower();
                  IBcx bcx = extension switch {
                      ".bca" =>
                          new BCA(bcxFile.SkimAllBytes()),
                      ".bck" =>
                          new BCK(bcxFile.SkimAllBytes()),
                      _ => throw new NotSupportedException(),
                  };
                  return (bcxFile.FullName, bcx);
                })
                .ToList();
      } catch {
        logger.LogError("Failed to load BCX!");
        throw;
      }

      List<(string, BTI)>? pathsAndBtis;
      try {
        pathsAndBtis =
            modelFileBundle
                .BtiFiles?
                .Select(btiFile
                            => (btiFile.FullName,
                                new BTI(btiFile.SkimAllBytes())))
                .ToList();
      } catch {
        logger.LogError("Failed to load BTI!");
        throw;
      }

      var model = new ModelImpl();

      var materialManager =
          new BmdMaterialManager(model, bmd, pathsAndBtis);

      var jointsAndBones = this.ConvertBones_(model, bmd);
      this.ConvertAnimations_(model,
                              bmd,
                              pathsAndBcxs,
                              modelFileBundle.FrameRate,
                              jointsAndBones);
      this.ConvertMesh_(model, bmd, jointsAndBones, materialManager);

      return model;
    }

    private (MkdsNode, IBone)[] ConvertBones_(IModel model, BMD bmd) {
      var joints = bmd.GetJoints();

      var jointsAndBones = new (MkdsNode, IBone)[joints.Length];
      var jointNameToBone = new Dictionary<string, IBone>();

      for (var j = 0; j < joints.Length; ++j) {
        var joint = joints[j];
        var jointName = joint.Name;

        var parentBone = joint.Parent == null
                             ? model.Skeleton.Root
                             : jointNameToBone[joint.Parent];

        var jnt = bmd.JNT1.Joints[j];

        var rotationFactor = 1f / 32768f * 3.14159f;
        var bone = parentBone.AddChild(jnt.Tx, jnt.Ty, jnt.Tz)
                             .SetLocalRotationRadians(
                                 jnt.Rx * rotationFactor,
                                 jnt.Ry * rotationFactor,
                                 jnt.Rz * rotationFactor)
                             .SetLocalScale(jnt.Sx, jnt.Sy, jnt.Sz);
        bone.Name = jointName;

        jointsAndBones[j] = (joint, bone);
        jointNameToBone[jointName] = bone;
      }

      return jointsAndBones;
    }

    private void ConvertAnimations_(
        IModel model,
        BMD bmd,
        IList<(string, IBcx)>? pathsAndBcxs,
        float frameRate,
        (MkdsNode, IBone)[] jointsAndBones) {
      var bcxCount = pathsAndBcxs?.Count ?? 0;
      for (var a = 0; a < bcxCount; ++a) {
        var (bcxPath, bcx) = pathsAndBcxs![a];
        var animationName = new FileInfo(bcxPath).Name.Split('.')[0];

        var animation = model.AnimationManager.AddAnimation();
        animation.Name = animationName;

        animation.FrameCount = bcx.Anx1.FrameCount;
        animation.FrameRate = frameRate;

        // Writes translation/rotation/scale for each joint.
        foreach (var (joint, bone) in jointsAndBones) {
          var jointIndex = bmd.JNT1.StringTable[joint.Name];

          var boneTracks = animation.AddBoneTracks(bone);

          // TODO: Handle mirrored animations
          // TODO: *Just* write keyframes.
          for (var f = 0; f < bcx.Anx1.FrameCount; ++f) {
            var position = JointUtil.GetTranslation(bcx, jointIndex, f);
            if (!float.IsFinite(position.X) ||
                !float.IsFinite(position.Y) ||
                !float.IsFinite(position.Z)) {
              throw new NotFiniteNumberException();
            }
            boneTracks.Positions.Set(f, 0, position.X);
            boneTracks.Positions.Set(f, 1, position.Y);
            boneTracks.Positions.Set(f, 2, position.Z);

            var (xRadians, yRadians, zRadians) =
                JointUtil.GetRotation(bcx, jointIndex, f);
            if (!float.IsFinite(xRadians) ||
                !float.IsFinite(yRadians) ||
                !float.IsFinite(zRadians)) {
              throw new NotFiniteNumberException();
            }
            var rotation = new ModelImpl.RotationImpl();
            rotation.SetRadians(xRadians, yRadians, zRadians);
            boneTracks.Rotations.Set(f, 0, rotation.XRadians);
            boneTracks.Rotations.Set(f, 1, rotation.YRadians);
            boneTracks.Rotations.Set(f, 2, rotation.ZRadians);

            var scale = JointUtil.GetScale(bcx, jointIndex, f);
            if (!float.IsFinite(scale.X) ||
                !float.IsFinite(scale.Y) ||
                !float.IsFinite(scale.Z)) {
              throw new NotFiniteNumberException();
            }
            boneTracks.Scales.Set(f, 0, scale.X);
            boneTracks.Scales.Set(f, 1, scale.Y);
            boneTracks.Scales.Set(f, 2, scale.Z);
          }
        }
      }
    }

    private void ConvertMesh_(
        IModel model,
        BMD bmd,
        (MkdsNode, IBone)[] jointsAndBones,
        BmdMaterialManager materialManager) {
      var finSkin = model.Skin;
      // TODO: Actually split this up
      var finMesh = finSkin.AddMesh();

      var joints = bmd.GetJoints();

      var vertexPositions = bmd.VTX1.Positions;
      var vertexNormals = bmd.VTX1.Normals;
      var vertexColors = bmd.VTX1.Colors;
      var vertexUvs = bmd.VTX1.Texcoords;
      var entries = bmd.INF1.Entries;
      var batches = bmd.SHP1.Batches;

      BMD.MAT3Section.MaterialEntry currentMaterialEntry = null;
      BmdFixedFunctionMaterial? currentBmdMaterial = null;

      var weightsTable = new IBoneWeights?[10];
      foreach (var entry in entries) {
        switch (entry.Type) {
          // Terminator
          case 0x00:
            goto DoneRendering;

          // Material
          case 0x11:
            var mappedMaterialIndex =
                bmd.MAT3.MaterialEntryIndieces[entry.Index];
            currentMaterialEntry =
                bmd.MAT3.MaterialEntries[mappedMaterialIndex];
            currentBmdMaterial = materialManager.Get(entry.Index);
            break;

          // Batch
          case 0x12:
            var batch = batches[(int) entry.Index];
            foreach (var packet in batch.Packets) {
              // Updates contents of matrix table
              for (var i = 0; i < packet.MatrixTable.Length; ++i) {
                var matrixTableIndex = packet.MatrixTable[i];

                // Max value means keep old value.
                if (matrixTableIndex == ushort.MaxValue) {
                  continue;
                }

                var isWeighted = bmd.DRW1.IsWeighted[matrixTableIndex];
                var drw1Index = bmd.DRW1.Data[matrixTableIndex];

                BoneWeight[] weights;
                if (isWeighted) {
                  var weightedIndices = bmd.EVP1.WeightedIndices[drw1Index];
                  weights = new BoneWeight[weightedIndices.Indices.Length];
                  for (var w = 0; w < weightedIndices.Indices.Length; ++w) {
                    var jointIndex = weightedIndices.Indices[w];
                    var weight = weightedIndices.Weights[w];

                    if (jointIndex >= joints.Length) {
                      throw new InvalidDataException();
                    }

                    var skinToBoneMatrix =
                        BmdModelLoader.ConvertMkdsToFin_(
                            bmd.EVP1.InverseBindMatrices[jointIndex]);

                    var bone = jointsAndBones[jointIndex].Item2;
                    weights[w] = new BoneWeight(bone, skinToBoneMatrix, weight);
                  }
                }
                // Unweighted bones are simple, just gets our precomputed limb
                // matrix
                else {
                  var jointIndex = drw1Index;
                  if (jointIndex >= joints.Length) {
                    throw new InvalidDataException();
                  }

                  var bone = jointsAndBones[jointIndex].Item2;
                  weights = new[]
                      {new BoneWeight(bone, MatrixTransformUtil.IDENTITY, 1)};
                }
                weightsTable[i] = finSkin.GetOrCreateBoneWeights(PreprojectMode.BONE, weights);
              }

              // TODO: Encapsulate this projection logic?
              // Adds primitives
              foreach (var primitive in packet.Primitives) {
                var points = primitive.Points;
                var pointsCount = points.Length;
                var vertices = new IVertex[pointsCount];

                for (var p = 0; p < pointsCount; ++p) {
                  var point = points[p];

                  if (!batch.HasPositions) {
                    throw new Exception(
                        "How can a point not have a position??");
                  }
                  var position = vertexPositions[point.PosIndex];
                  var vertex =
                      finSkin.AddVertex(position.X, position.Y, position.Z);
                  vertices[p] = vertex;

                  if (batch.HasNormals) {
                    var normal = vertexNormals[point.NormalIndex];
                    vertex.SetLocalNormal(normal.X, normal.Y, normal.Z);
                  }

                  var matrixIndex = point.MatrixIndex / 3;
                  var weights = weightsTable[matrixIndex];
                  if (weights != null) {
                    vertex.SetBoneWeights(weights);
                  }

                  for (var c = 0; c < 2; ++c) {
                    if (batch.HasColors[c]) {
                      var colorIndex = point.ColorIndex[c];
                      var color = vertexColors[c][colorIndex];
                      vertex.SetColorBytes(c,
                                           color.R,
                                           color.G,
                                           color.B,
                                           color.A);
                    }
                  }

                  for (var i = 0; i < 8; ++i) {
                    if (batch.HasTexCoords[i]) {
                      var texCoord = vertexUvs[i][point.TexCoordIndex[i]];
                      vertex.SetUv(i, texCoord.S, texCoord.T);
                    }
                  }
                }

                var glPrimitiveType = primitive.GetGlPrimitive();

                Asserts.Nonnull(currentBmdMaterial);
                switch (glPrimitiveType) {
                  case Gl.GL_TRIANGLES: {
                    finMesh.AddTriangles(vertices)
                           .SetMaterial(currentBmdMaterial.Material);
                    break;
                  }

                  case Gl.GL_TRIANGLE_STRIP: {
                    finMesh.AddTriangleStrip(vertices)
                           .SetMaterial(currentBmdMaterial.Material);
                    break;
                  }

                  case Gl.GL_TRIANGLE_FAN: {
                    finMesh.AddTriangleFan(vertices)
                           .SetMaterial(currentBmdMaterial.Material);
                    break;
                  }

                  case Gl.GL_QUADS: {
                    finMesh.AddQuads(vertices)
                           .SetMaterial(currentBmdMaterial.Material);
                    break;
                  }

                  default:
                    throw new NotSupportedException(
                        $"Unsupported primitive type: {glPrimitiveType}");
                }
              }
            }
            break;
        }
      }

      DoneRendering: ;
    }

    private static IFinMatrix4x4 ConvertMkdsToFin_(MTX44 mkdsMatrix) {
      var finMatrix = new FinMatrix4x4();

      for (var y = 0; y < 4; ++y) {
        for (var x = 0; x < 4; ++x) {
          finMatrix[y, x] = mkdsMatrix[x, y];
        }
      }

      return finMatrix;
    }
  }
}