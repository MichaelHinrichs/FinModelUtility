using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using fin.io;
using fin.log;
using fin.math;

using MathNet.Numerics.LinearAlgebra.Double;

using bmd.G3D_Binary_File_Format;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Schema2;

using bmd.GCN;

using Tao.OpenGl;

namespace bmd.exporter {
  using VERTEX =
      VertexBuilder<VertexPositionNormal, VertexColor1Texture2, VertexJoints4>;
  using GltfNode = SharpGLTF.Schema2.Node;
  using MkdsNode = bmd._3D_Formats.MA.Node;
  using CsVector = System.Numerics.Vector3;

  public class GltfExporterOld {
    private readonly ILogger logger_ = Logging.Create<GltfExporterOld>();

    public const bool IncludeRootNode = false;

    public void Export(
        string outputDirectory,
        (string, BMD) pathAndBmd,
        IList<(string, IBcx)>? pathsAndBcxs = null,
        IList<(string, BTI)>? pathsAndBtis = null) {
      using var _ = this.logger_.BeginScope("Export");

      var (bmdPath, bmd) = pathAndBmd;

      var joints = bmd.GetJoints();

      var scale = 1;

      // Options
      var model = ModelRoot.CreateModel();

      var scene = model.UseScene("default");

      var skin = model.CreateSkin();

      var rootNode = scene.CreateNode();

      GltfNode[] skinNodes;
      if (IncludeRootNode) {
        skinNodes = new GltfNode[1 + joints.Length];
        skinNodes[0] = rootNode;
      } else {
        skinNodes = new GltfNode[joints.Length];
      }

      // TODO: Use buffers for shader stuff?
      // TODO: Eliminate redundant definitions.
      // TODO: Include face animations, somehow?
      // TODO: Fix large filesize for Link, seems to be animations?
      // TODO: Tweak shininess.
      // TODO: Fix limb matrices for some characters, like Bazaar Shopkeeper?
      // TODO: Environment color isn't used yet, giving weird colors for link.

      // Gathers up joints and their gltf nodes.
      // TODO: Jeez, clean these up.
      var bcxCount = pathsAndBcxs?.Count ?? 0;
      var firstAnimation = bcxCount > 0 ? pathsAndBcxs[0].Item2 : null;

      var jointNameToNode = new Dictionary<string, GltfNode>();
      var jointsAndNodes = new (MkdsNode, GltfNode)[joints.Length];
      var jointNodes = new GltfNode[joints.Length];
      for (var j = 0; j < joints.Length; ++j) {
        var joint = joints[j];
        var jointName = joint.Name;

        var parentNode = joint.Parent == null
                             ? rootNode
                             : jointNameToNode[joint.Parent];

        var node = parentNode.CreateNode(jointName);

        // We should be able to use the raw from joint, but this screws up the
        // bones with multiple weights for some reason, perhaps because the
        // model is contorted in an unnatural way? Anyway, we NEED to use the
        // first animation instead.
        if (firstAnimation != null) {
          var jointTranslation =
              JointUtil.GetTranslation(firstAnimation, j, 0) * scale;
          node.WithLocalTranslation(jointTranslation);

          var jointRotation =
              JointUtil.GetQuaternion(firstAnimation, j, 0);
          if (jointRotation.Length() > 0) {
            node.WithLocalRotation(jointRotation);
          }

          // It seems like some animations shrink a bone to 0 to hide it, but
          // this prevents us from calculating a determinant to invert the
          // matrix. As a result, we can't safely include this here.
          /*var jointScale = JointUtil.GetScale(firstAnimation, j, 0);
          node.WithLocalScale(jointScale);*/
        } else {
          var jnt = bmd.JNT1.Joints[j];
          node.WithLocalTranslation(new Vector3(jnt.Tx, jnt.Ty, jnt.Tz) *
                                    scale);

          var jointRotation = QuaternionUtil.Create(jnt.Rx, jnt.Ry, jnt.Rz);
          if (jointRotation.Length() > 0) {
            node.WithLocalRotation(jointRotation);
          }

          node.WithLocalScale(new Vector3(jnt.Sx, jnt.Sy, jnt.Sz));
        }

        jointNodes[j] = node;
        jointNameToNode[jointName] = node;
        if (IncludeRootNode) {
          skinNodes[1 + j] = node;
        } else {
          skinNodes[j] = node;
        }
        jointsAndNodes[j] = (joint, node);
      }
      skin.BindJoints(skinNodes.ToArray());

      // Gathers up animations.
      for (var a = 0; a < bcxCount; ++a) {
        var (bcxPath, bcx) = pathsAndBcxs![a];
        var animationName = new FileInfo(bcxPath).Name.Split('.')[0];

        var glTfAnimation = model.UseAnimation(animationName);

        // Writes translation/rotation/scale for each joint.
        var translationKeyframes = new Dictionary<float, Vector3>();
        var rotationKeyframes = new Dictionary<float, Quaternion>();
        var scaleKeyframes = new Dictionary<float, Vector3>();
        foreach (var (joint, node) in jointsAndNodes) {
          var jointIndex = bmd.JNT1.StringTable[joint.Name];

          // TODO: Handle mirrored animations
          for (var f = 0; f < bcx.Anx1.FrameCount; ++f) {
            var time = f / 30f;

            translationKeyframes[time] =
                JointUtil.GetTranslation(bcx, jointIndex, f) * scale;
            rotationKeyframes[time] =
                JointUtil.GetQuaternion(bcx, jointIndex, f);
            scaleKeyframes[time] = JointUtil.GetScale(bcx, jointIndex, f);
          }

          glTfAnimation.CreateTranslationChannel(
              node,
              translationKeyframes);
          glTfAnimation.CreateRotationChannel(
              node,
              rotationKeyframes);
          glTfAnimation.CreateScaleChannel(
              node,
              scaleKeyframes);
        }
      }

      // Gathers up vertex builders.
      var mesh =
          GltfExporterOld.WriteMesh_(jointNodes, model, bmd, pathsAndBtis);
      scene.CreateNode()
           .WithSkinnedMesh(mesh, rootNode.WorldMatrix, jointNodes.ToArray());

      var writeSettings = new WriteSettings {
          ImageWriting = ResourceWriteMode.SatelliteFile,
      };

      var bmdFile = new FileInfo(bmdPath);
      var outputFile = new FinFile(Path.Join(outputDirectory, bmdFile.Name));
      var outputPath = outputFile.CloneWithExtension(".glb").FullName;

      this.logger_.LogInformation($"Writing to {outputPath}...");
      model.Save(outputPath, writeSettings);
    }

    private static SoftwareModelViewMatrixTransformer2 transformer =
        new SoftwareModelViewMatrixTransformer2();

    private static Mesh WriteMesh_(
        GltfNode[] jointNodes,
        ModelRoot model,
        BMD bmd,
        IList<(string, BTI)>? pathsAndBtis = null) {
      GltfMaterial? currentMaterial = null;

      var entries = bmd.INF1.Entries;
      var joints = bmd.GetJoints();

      var vertexPositions = bmd.VTX1.Positions;
      var vertexNormals = bmd.VTX1.Normals;
      var vertexColors = bmd.VTX1.Colors;
      var vertexUvs = bmd.VTX1.Texcoords;
      var batches = bmd.SHP1.Batches;

      var meshBuilder = VERTEX.CreateCompatibleMesh();

      var matrixIndices = new Dictionary<int, int>();

      var materialManager = new GltfMaterialManager(bmd, pathsAndBtis);

      // TODO: Need to pre-compute matrices, vertices come w/ matrix index.
      var matrices = new Matrix[joints.Length];
      for (var e = 0; e < entries.Length; ++e) {
        var entry = entries[e];
        switch (entry.Type) {
          case 0x01: {
            transformer.Push();
            break;
          }

          case 0x02: {
            transformer.Pop();
            break;
          }

          // Joint
          case 0x10:
            var jointIndex = entry.Index;
            var gltfNode = jointNodes[jointIndex];

            // TODO: I think this is wrong, should be ignoring scale?
            var m = GlMatrixUtil2.CsToMn(gltfNode.WorldMatrix);
            transformer.Set(m);

            var cur = new DenseMatrix(4, 4);
            transformer.Get(cur);
            matrices[jointIndex] = cur;
            break;
        }
      }

      var matrixTable = new WeightedMatrix[10];
      for (var e = 0; e < entries.Length; ++e) {
        var entry = entries[e];
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

                if (isWeighted) {
                  var weightedIndices = bmd.EVP1.WeightedIndices[drw1Index];
                  var weightedMatrices =
                      new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                  var skinning = new List<(int, float)>();

                  var mergedMatrix = new DenseMatrix(4, 4);

                  for (var w = 0; w < weightedIndices.Indices.Length; ++w) {
                    var jointIndex = weightedIndices.Indices[w];
                    var weight = weightedIndices.Weights[w];

                    if (jointIndex >= joints.Length) {
                      throw new InvalidDataException();
                    }

                    var skinToLocalLimbMatrix =
                        ConvertMkdsToMn_(
                            bmd.EVP1.InverseBindMatrices[jointIndex]);
                    var localLimbToWorldMatrix = matrices[jointIndex];
                    var skinToWorldMatrix =
                        localLimbToWorldMatrix * skinToLocalLimbMatrix * weight;

                    for (var j = 0; j < 4; ++j) {
                      for (var k = 0; k < 4; ++k) {
                        mergedMatrix[j, k] += skinToWorldMatrix[j, k];
                      }
                    }

                    weightedMatrices.Add(skinToWorldMatrix);

                    if (IncludeRootNode) {
                      skinning.Add((1 + jointIndex, weight));
                    } else {
                      skinning.Add((jointIndex, weight));
                    }
                  }

                  matrixTable[i] = new WeightedMatrix {
                      Matrix = mergedMatrix,
                      Skinning = skinning.ToArray(),
                  };
                }
                // Unweighted bones are simple, just gets our precomputed limb
                // matrix
                else {
                  var jointIndex = drw1Index;
                  if (jointIndex >= joints.Length) {
                    throw new InvalidDataException();
                  }

                  var skinning = new List<(int, float)>();
                  if (IncludeRootNode) {
                    skinning.Add((1 + jointIndex, 1));
                  } else {
                    skinning.Add((jointIndex, 1));
                  }

                  matrixTable[i] = new WeightedMatrix {
                      Matrix = matrices[jointIndex],
                      Skinning = skinning.ToArray(),
                  };
                }
              }

              // Adds primitives
              foreach (var primitive in packet.Primitives) {
                var points = primitive.Points;
                var pointsCount = points.Length;
                var vertices = new VERTEX[pointsCount];

                for (var p = 0; p < pointsCount; ++p) {
                  var point = points[p];

                  var matrixIndex = point.MatrixIndex / 3;
                  var weightedMatrix = matrixTable[matrixIndex];

                  foreach (var skins in weightedMatrix.Skinning) {
                    var ji = skins.Item1;
                    var c = matrixIndices.ContainsKey(ji)
                                ? matrixIndices[ji]
                                : 0;
                    matrixIndices[ji] = c + 1;
                  }

                  CsVector? gltfPosition = null;
                  if (!batch.HasPositions) {
                    throw new Exception(
                        "How can a point not have a position??");
                  } else {
                    var position = vertexPositions[point.PosIndex];

                    double x = position.X;
                    double y = position.Y;
                    double z = position.Z;

                    ProjectVertexWeighted(weightedMatrix, ref x, ref y, ref z);

                    gltfPosition =
                        new CsVector((float) x, (float) y, (float) z);
                  }

                  var vertexBuilder = VERTEX
                                      .Create(gltfPosition.Value)
                                      .WithSkinning(weightedMatrix.Skinning);

                  if (batch.HasNormals) {
                    var normal = vertexNormals[point.NormalIndex];

                    double normalX = normal.X;
                    double normalY = normal.Y;
                    double normalZ = normal.Z;

                    ProjectNormalWeighted(weightedMatrix,
                                          ref normalX,
                                          ref normalY,
                                          ref normalZ);

                    vertexBuilder = vertexBuilder.WithGeometry(
                        vertexBuilder.Position,
                        new CsVector((float) normalX,
                                     (float) normalY,
                                     (float) normalZ));
                  }

                  Vector4? gltfColor = null;
                  if (batch.HasColors[0]) {
                    var colorIndex = point.ColorIndex[0];
                    var color = vertexColors[0][colorIndex];

                    var r = color.R / (float) byte.MaxValue;
                    var b = color.G / (float) byte.MaxValue;
                    var g = color.B / (float) byte.MaxValue;
                    var a = color.A / (float) byte.MaxValue;

                    gltfColor = new Vector4(r, g, b, a);
                  } else {
                    // TODO: Is this needed?
                    // Keeps the model from being pitch black.
                    gltfColor = new Vector4(1, 1, 1, 1);
                  }

                  var texStageIndices = currentMaterial.CurrentTexStageIndices;

                  // TODO: Support multiple texture coords?
                  // TODO: There can actually be up to 8, but how to use them all in glTF???
                  Vector2? gltfUv = null;
                  if (batch.HasTexCoords[0]) {
                    var uv = vertexUvs[0][point.TexCoordIndex[0]];
                    gltfUv = new Vector2(uv.S, uv.T);
                  }

                  var hasColor = gltfColor != null;
                  var hasUv = gltfUv != null;
                  if (hasColor && hasUv) {
                    vertexBuilder =
                        vertexBuilder.WithMaterial(
                            gltfColor.Value,
                            gltfUv.Value);
                  } else if (hasColor) {
                    vertexBuilder = vertexBuilder.WithMaterial(gltfColor.Value);
                  } else if (hasUv) {
                    vertexBuilder = vertexBuilder.WithMaterial(gltfUv.Value);
                  }

                  vertices[p] = vertexBuilder;
                }

                var glPrimitiveType = primitive.GetGlPrimitive();

                switch (glPrimitiveType) {
                  case Gl.GL_TRIANGLES: {
                    var triangles =
                        meshBuilder.UsePrimitive(
                            currentMaterial.MaterialBuilder,
                            3);

                    for (var v = 0; v < pointsCount; v += 3) {
                      triangles.AddTriangle(vertices[v + 0],
                                            vertices[v + 1],
                                            vertices[v + 2]);
                    }

                    break;
                  }

                  case Gl.GL_TRIANGLE_STRIP: {
                    var triangleStrip =
                        meshBuilder.UsePrimitive(
                            currentMaterial.MaterialBuilder,
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

                  case Gl.GL_TRIANGLE_FAN: {
                    var triangleStrip =
                        meshBuilder.UsePrimitive(
                            currentMaterial.MaterialBuilder,
                            3);

                    // https://stackoverflow.com/a/8044252
                    var firstVertex = vertices[0];
                    var previousVertex = vertices[1];
                    for (var v = 2; v < pointsCount; ++v) {
                      triangleStrip.AddTriangle(firstVertex,
                                                previousVertex,
                                                vertices[v]);
                    }
                    break;
                  }

                  case Gl.GL_QUADS: {
                    var quads =
                        meshBuilder.UsePrimitive(
                            currentMaterial.MaterialBuilder,
                            4);

                    for (var v = 0; v < pointsCount; v += 4) {
                      quads.AddQuadrangle(vertices[v + 0],
                                          vertices[v + 1],
                                          vertices[v + 2],
                                          vertices[v + 3]);
                    }

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

      DoneRendering:
      return model.CreateMesh(meshBuilder);
    }

    private static void ProjectVertexWeighted(
        WeightedMatrix weightedMatrix,
        ref double x,
        ref double y,
        ref double z) {
      transformer.Push();
      transformer.Set(weightedMatrix.Matrix);

      transformer.ProjectVertex(ref x, ref y, ref z, false);

      transformer.Pop();
    }

    private static void ProjectNormalWeighted(
        WeightedMatrix weightedMatrix,
        ref double x,
        ref double y,
        ref double z) {
      transformer.Push();
      transformer.Set(weightedMatrix.Matrix);

      transformer.ProjectNormal(ref x, ref y, ref z);

      transformer.Pop();

      // All of the normals are inside-out for some reason, we have to flip
      // them manually.
      x = -x;
      y = -y;
      z = -z;
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

  public class WeightedMatrix {
    public (int, float)[] Skinning { get; set; }

    public MathNet.Numerics.LinearAlgebra.Matrix<double> Matrix;
  }
}