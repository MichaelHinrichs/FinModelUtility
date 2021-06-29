using System;
using System.Collections.Generic;
using System.IO;

using fin.math;

using MKDS_Course_Modifier.GCN;

using fin.model;
using fin.model.impl;
using fin.util.asserts;

using MathNet.Numerics.LinearAlgebra.Double;

using MKDS_Course_Modifier.G3D_Binary_File_Format;

using Tao.OpenGl;

namespace mkds.exporter {
  using MkdsNode = MKDS_Course_Modifier._3D_Formats.MA.Node;

  public class ModelConverter {
    public IModel Convert(
        BMD bmd,
        IList<(string, IBcx)>? pathsAndBcxs = null,
        IList<(string, BTI)>? pathsAndBtis = null) {
      var model = new ModelImpl();

      var materialManager =
          new BmdMaterialManager(model, bmd, pathsAndBtis);

      var jointsAndBones = this.ConvertBones_(model, bmd);
      this.ConvertAnimations_(model, bmd, pathsAndBcxs, jointsAndBones);
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

        var bone = parentBone.AddChild(jnt.Tx, jnt.Ty, jnt.Tz)
                             .SetLocalRotationRadians(jnt.Rx, jnt.Ry, jnt.Rz)
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
        (MkdsNode, IBone)[] jointsAndBones) {
      var bcxCount = pathsAndBcxs?.Count ?? 0;
      for (var a = 0; a < bcxCount; ++a) {
        var (bcxPath, bcx) = pathsAndBcxs![a];
        var animationName = new FileInfo(bcxPath).Name.Split('.')[0];

        var animation = model.AnimationManager.AddAnimation();
        animation.Name = animationName;

        animation.FrameCount = bcx.Anx1.FrameCount;
        animation.Fps = 30;

        // Writes translation/rotation/scale for each joint.
        foreach (var (joint, bone) in jointsAndBones) {
          var jointIndex = bmd.JNT1.StringTable[joint.Name];

          var boneTracks = animation.AddBoneTracks(bone);

          // TODO: Handle mirrored animations
          // TODO: *Just* write keyframes.
          for (var f = 0; f < bcx.Anx1.FrameCount; ++f) {
            var position = JointUtil.GetTranslation(bcx, jointIndex, f);
            boneTracks.Positions.Set(f,
                                     new ModelImpl.PositionImpl {
                                         X = position.X,
                                         Y = position.Y,
                                         Z = position.Z
                                     });

            var (xRadians, yRadians, zRadians) =
                JointUtil.GetRotation(bcx, jointIndex, f);
            var rotation = new ModelImpl.RotationImpl();
            rotation.SetRadians(xRadians, yRadians, zRadians);
            boneTracks.Rotations.Set(f, rotation);

            var scale = JointUtil.GetScale(bcx, jointIndex, f);
            boneTracks.Scales.Set(f,
                                  new ModelImpl.ScaleImpl {
                                      X = scale.X,
                                      Y = scale.Y,
                                      Z = scale.Z,
                                  });
          }
        }
      }
    }

    private void ConvertMesh_(
        IModel model,
        BMD bmd,
        (MkdsNode, IBone)[] jointsAndBones,
        BmdMaterialManager materialManager) {
      var skin = model.Skin;

      var joints = bmd.GetJoints();

      var vertexPositions = bmd.VTX1.Positions;
      var vertexNormals = bmd.VTX1.Normals;
      var vertexColors = bmd.VTX1.Colors;
      var vertexUvs = bmd.VTX1.Texcoords;
      var entries = bmd.INF1.Entries;
      var batches = bmd.SHP1.Batches;

      BmdMaterial? currentMaterial = null;

      var weightsTable = new BoneWeight[]?[10];
      foreach (var entry in entries) {
        switch (entry.Type) {
          // Terminator
          case 0x00:
            goto DoneRendering;

          // Material
          case 0x11:
            currentMaterial = materialManager.Get(entry.Index);
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
                        ModelConverter.ConvertMkdsToMn_(
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
                      {new BoneWeight(bone, MatrixUtil.Identity, 1)};
                }
                weightsTable[i] = weights;
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
                      skin.AddVertex(position.X, position.Y, position.Z);
                  vertices[p] = vertex;

                  if (batch.HasNormals) {
                    var normal = vertexNormals[point.NormalIndex];
                    vertex.SetLocalNormal(normal.X, normal.Y, normal.Z);
                  }

                  var matrixIndex = point.MatrixIndex / 3;
                  var weights = weightsTable[matrixIndex];
                  if (weights != null) {
                    vertex.SetBones(weights);
                  }

                  // TODO: Include color
                  // TODO: I don't think this is technically correct.
                  if (batch.HasTexCoords[0]) {
                    var texCoord = vertexUvs[0][point.TexCoordIndex[0]];
                    vertex.SetUv(texCoord.S, texCoord.T);
                  }
                }

                var glPrimitiveType = primitive.GetGlPrimitive();

                Asserts.Nonnull(currentMaterial);
                switch (glPrimitiveType) {
                  case Gl.GL_TRIANGLES: {
                    skin.AddTriangles(vertices)
                        .SetMaterial(currentMaterial.Material);
                    break;
                  }

                  case Gl.GL_TRIANGLE_STRIP: {
                    skin.AddTriangleStrip(vertices)
                        .SetMaterial(currentMaterial.Material);
                      break;
                  }

                  case Gl.GL_QUADS: {
                    skin.AddQuads(vertices)
                        .SetMaterial(currentMaterial.Material);
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

    private static Matrix ConvertMkdsToMn_(MTX44 mkds) {
      var mn = new DenseMatrix(4, 4);

      for (var y = 0; y < 4; ++y) {
        for (var x = 0; x < 4; ++x) {
          mn[y, x] = mkds[x, y];
        }
      }

      return mn;
    }
  }
}