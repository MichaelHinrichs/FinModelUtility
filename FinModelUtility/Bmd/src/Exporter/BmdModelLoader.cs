using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fin.math;
using fin.math.matrix;
using bmd.GCN;
using bmd.schema.bcx;
using bmd.schema.bmd.jnt1;
using bmd.schema.bmd.mat3;
using bmd.schema.bti;
using fin.data;
using fin.model;
using fin.model.impl;
using fin.util.asserts;
using fin.io;
using fin.log;
using fin.schema.matrix;
using gx;
using System.Numerics;


namespace bmd.exporter {
  using MkdsNode = bmd._3D_Formats.MA.Node;
  using GxPrimitiveType = BMD.SHP1Section.Batch.Packet.Primitive.GXPrimitive;

  public class BmdModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.BmdFile;

    public IFileHierarchyFile BmdFile { get; set; }
    public IReadOnlyList<IFileHierarchyFile>? BcxFiles { get; set; }
    public IReadOnlyList<IFileHierarchyFile>? BtiFiles { get; set; }
    public float FrameRate { get; set; } = 30;
  }

  public class BmdModelLoader : IModelLoader<BmdModelFileBundle> {
    public IModel LoadModel(BmdModelFileBundle modelFileBundle) {
      var logger = Logging.Create<BmdModelLoader>();

      var bmd = new BMD(modelFileBundle.BmdFile.Impl.ReadAllBytes());

      List<(string, IBcx)>? pathsAndBcxs;
      try {
        pathsAndBcxs =
            modelFileBundle
                .BcxFiles?
                .Select(bcxFile => {
                  var extension = bcxFile.Extension.ToLower();
                  IBcx bcx = extension switch {
                      ".bca" =>
                          new Bca(bcxFile.Impl.ReadAllBytes()),
                      ".bck" =>
                          new Bck(bcxFile.Impl.ReadAllBytes()),
                      _ => throw new NotSupportedException(),
                  };
                  return (bcxFile.FullName, bcx);
                })
                .ToList();
      } catch {
        logger.LogError("Failed to load BCX!");
        throw;
      }

      List<(string, Bti)>? pathsAndBtis;
      try {
        pathsAndBtis =
            modelFileBundle
                .BtiFiles?
                .Select(btiFile
                            => (btiFile.FullName,
                                btiFile.Impl.ReadNew<Bti>(
                                    Endianness.BigEndian)))
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
      var jointIdToBone = new Dictionary<int, IBone>();

      for (var j = 0; j < joints.Length; ++j) {
        var node = joints[j];

        var parentBone = node.ParentJointIndex == -1
                             ? model.Skeleton.Root
                             : jointIdToBone[node.ParentJointIndex];

        var joint = node.Entry;
        var jointName = node.Name;

        var rotationFactor = 1f / 32768f * 3.14159f;
        var bone =
            parentBone
                .AddChild(
                    joint.Translation.X,
                    joint.Translation.Y,
                    joint.Translation.Z)
                .SetLocalRotationRadians(
                    joint.Rotation.X * rotationFactor,
                    joint.Rotation.Y * rotationFactor,
                    joint.Rotation.Z * rotationFactor)
                .SetLocalScale(
                    joint.Scale.X,
                    joint.Scale.Y,
                    joint.Scale.Z);
        bone.Name = jointName;

        bone.IgnoreParentScale = node.Entry.IgnoreParentScale;

        // TODO: How to do this without hardcoding???
        if (node.Entry.JointType == JointType.MANUAL) {
          if (node.Name.StartsWith("balloon")) {
            var rotateYaw =
                Quaternion.CreateFromYawPitchRoll(MathF.PI / 2, 0, 0);
            var rotatePitch =
                Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 2, 0);
            bone.AlwaysFaceTowardsCamera(rotateYaw * rotatePitch);
          }

          // Japanese word for light
          if (node.Name.StartsWith("hikair") ||
              node.Name.StartsWith("hikari")) {
            var rotateYaw =
                Quaternion.CreateFromYawPitchRoll(-MathF.PI / 2, 0, 0);
            bone.AlwaysFaceTowardsCamera(rotateYaw);
          }
        }

        jointsAndBones[j] = (node, bone);
        jointIdToBone[j] = bone;
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

      var rootNode = new Node<bool>();
      var currentNode = rootNode;

      var scheduledDrawOnWayDownPrimitives = new List<IPrimitive>();
      var scheduledDrawOnWayUpPrimitives = new List<IPrimitive>();

      GxFixedFunctionMaterial? currentMaterial = null;
      MaterialEntry? currentMaterialEntry = null;

      uint currentRenderIndex = 1;

      var weightsTable = new IBoneWeights?[10];
      foreach (var entry in entries) {
        switch (entry.Type) {
          // Terminator
          case 0x00:
            goto DoneRendering;

          case 0x01: {
            var child = new Node<bool>();
            currentNode.AddChild(child);
            currentNode = child;

            foreach (var primitive in scheduledDrawOnWayDownPrimitives) {
              primitive.SetInversePriority(currentRenderIndex++);
            }
            scheduledDrawOnWayDownPrimitives.Clear();
            break;
          }

          case 0x02: {
            currentNode = currentNode.Parent;

            foreach (var primitive in scheduledDrawOnWayUpPrimitives) {
              primitive.SetInversePriority(currentRenderIndex++);
            }
            scheduledDrawOnWayUpPrimitives.Clear();
            break;
          }

          // Material
          case 0x11:
            currentMaterial = materialManager.Get(entry.Index);
            currentMaterialEntry =
                bmd.MAT3.MaterialEntries[
                    bmd.MAT3.MaterialEntryIndieces[entry.Index]];
            break;

          // Batch
          case 0x12:
            var batch = batches[(int)entry.Index];

            // TODO: Pass matrix type into joint (how?)
            var matrixType = batch.MatrixType;

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
                        BmdModelLoader.ConvertSchemaToFin_(
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
                  weights = new[] {
                      new BoneWeight(bone, MatrixTransformUtil.IDENTITY, 1)
                  };
                }
                weightsTable[i] =
                    finSkin.GetOrCreateBoneWeights(
                        PreprojectMode.BONE, weights);
              }

              foreach (var primitive in packet.Primitives) {
                var points = primitive.Points;
                var pointsCount = points.Length;
                var vertices = new IVertex[pointsCount];

                var weightsUsedByPrimitive = new HashSet<IBoneWeights>();
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
                    weightsUsedByPrimitive.Add(weights);
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

                var gxPrimitiveType = primitive.Type;

                Asserts.Nonnull(currentMaterial);

                IPrimitive finPrimitive;
                switch (gxPrimitiveType) {
                  case GxPrimitiveType.GX_TRIANGLES: {
                    finPrimitive = finMesh.AddTriangles(vertices)
                                          .SetMaterial(
                                              currentMaterial.Material);
                    break;
                  }

                  case GxPrimitiveType.GX_TRIANGLESTRIP: {
                    finPrimitive =
                        finMesh.AddTriangleStrip(vertices)
                               .SetMaterial(currentMaterial.Material);
                    break;
                  }

                  case GxPrimitiveType.GX_TRIANGLEFAN: {
                    finPrimitive = finMesh.AddTriangleFan(vertices)
                                          .SetMaterial(
                                              currentMaterial.Material);
                    break;
                  }

                  case GxPrimitiveType.GX_QUADS: {
                    finPrimitive =
                        finMesh.AddQuads(vertices)
                               .SetMaterial(currentMaterial.Material);
                    break;
                  }

                  default:
                    throw new NotSupportedException(
                        $"Unsupported primitive type: {gxPrimitiveType}");
                }

                var renderOrder = currentMaterialEntry?.RenderOrder ??
                                  RenderOrder.DRAW_ON_WAY_DOWN;
                switch (renderOrder) {
                  case RenderOrder.DRAW_ON_WAY_DOWN: {
                    scheduledDrawOnWayDownPrimitives.Add(finPrimitive);
                    break;
                  }
                  case RenderOrder.DRAW_ON_WAY_UP: {
                    scheduledDrawOnWayUpPrimitives.Add(finPrimitive);
                    break;
                  }
                  default: throw new ArgumentOutOfRangeException();
                }
              }
            }
            break;
        }
      }

      DoneRendering: ;
    }

    private static IFinMatrix4x4 ConvertSchemaToFin_(Matrix3x4f schemaMatrix) {
      var finMatrix = new FinMatrix4x4().SetIdentity();

      for (var r = 0; r < 3; ++r) {
        for (var c = 0; c < 4; ++c) {
          finMatrix[r, c] = schemaMatrix[r, c];
        }
      }

      return finMatrix;
    }
  }
}