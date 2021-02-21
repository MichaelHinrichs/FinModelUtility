using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;

using MathNet.Numerics.LinearAlgebra.Double;

using MKDS_Course_Modifier.G3D_Binary_File_Format;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

using MKDS_Course_Modifier.GCN;

using Tao.OpenGl;

using TextureWrapMode = SharpGLTF.Schema2.TextureWrapMode;

namespace mkds.exporter {
  using VERTEX =
      VertexBuilder<VertexPositionNormal, VertexColor1Texture2, VertexJoints4>;
  using GltfNode = SharpGLTF.Schema2.Node;
  using MkdsNode = MKDS_Course_Modifier._3D_Formats.MA.Node;
  using AnimatedMkdsNode = MKDS_Course_Modifier._3D_Formats.MA.AnimatedNode;
  using CsQuaternion = System.Numerics.Quaternion;
  using MsQuaternion = Microsoft.Xna.Framework.Quaternion;
  using CsVector = System.Numerics.Vector3;
  using MsVector = Microsoft.Xna.Framework.Vector3;
  using GxWrapTag = BMD.TEX1Section.GX_WRAP_TAG;

  public static class GltfExporter {
    public static bool IncludeRootNode = false;

    public static void Export(
        string filePath,
        BMD bmd,
        IList<(string, IBcx)>? pathsAndBcxs = null) {
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

        node.WithLocalTranslation(
            GltfExporter.ConvertMsToCs_(joint.Trans, scale));

        var jointRotation = joint.Rot;
        if (jointRotation.Length() > 0) {
          node.WithLocalRotation(GltfExporter.ConvertMsToCs_(jointRotation));
        }

        var jointScale = joint.Scale;
        node.WithLocalScale(GltfExporter.ConvertMsToCs_(jointScale));

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

      // Gathers up texture materials.
      // TODO: Check MAT3 for more specific material values
      var basePath = new FileInfo(filePath).Directory.FullName;

      var textures = bmd.TEX1.TextureHeaders;
      var materials = new MaterialBuilder[1 + textures.Length];
      materials[0] = new MaterialBuilder("null")
                     .WithDoubleSide(true)
                     .WithUnlitShader();
      for (var i = 0; i < textures.Length; ++i) {
        var glTexture = textures[i];

        var stream = new MemoryStream();
        glTexture.ToBitmap().Save(stream, ImageFormat.Png);
        var glTfImage = new MemoryImage(stream.ToArray());

        // TODO: Need to handle wrapping in the shader?
        var wrapModeS = GltfExporter.GetWrapMode_(glTexture.WrapS);
        var wrapModeT = GltfExporter.GetWrapMode_(glTexture.WrapS);

        // TODO: Alpha isn't always needed.
        // TODO: Double-sided isn't always needed.
        var material = new MaterialBuilder($"material{i}")
                       .WithAlpha(SharpGLTF.Materials.AlphaMode.MASK)
                       .WithDoubleSide(true)
                       .WithSpecularGlossinessShader()
                       .WithSpecularGlossiness(new Vector3(0), 0);

        material.UseChannel(KnownChannel.Diffuse)
                .UseTexture()
                .WithPrimaryImage(glTfImage)
                .WithSampler(wrapModeS, wrapModeT);

        materials[1 + i] = material;
      }

      // Gathers up animations.
      var bcxCount = pathsAndBcxs?.Count ?? 0;
      for (var a = 0; a < bcxCount; ++a) {
        var (bcxPath, bcx) = pathsAndBcxs![a];
        var animatedJoints = bcx.Anx1.Joints;

        var animationName = new FileInfo(bcxPath).Name.Split('.')[0];

        var glTfAnimation = model.UseAnimation(animationName);

        // Writes translation/rotation/scale for each joint.
        var translationKeyframes = new Dictionary<float, Vector3>();
        var rotationKeyframes = new Dictionary<float, Quaternion>();
        var scaleKeyframes = new Dictionary<float, Vector3>();
        foreach (var (joint, node) in jointsAndNodes) {
          var jointIndex = bmd.JNT1.StringTable[joint.Name];
          var animatedJoint = animatedJoints[jointIndex];
          var values = animatedJoint.Values;

          // TODO: Handle mirrored animations
          for (var f = 0; f < bcx.Anx1.FrameCount; ++f) {
            var time = f / 30f;

            var x = animatedJoint.GetAnimValue(values.translationsX, f) *
                    scale;
            var y = animatedJoint.GetAnimValue(values.translationsY, f) *
                    scale;
            var z = animatedJoint.GetAnimValue(values.translationsZ, f) *
                    scale;
            translationKeyframes[time] = new CsVector(x, y, z);

            var xRadians = animatedJoint.GetAnimValue(values.rotationsX, f);
            var yRadians = animatedJoint.GetAnimValue(values.rotationsY, f);
            var zRadians = animatedJoint.GetAnimValue(values.rotationsZ, f);

            rotationKeyframes[time] =
                GltfExporter.CreateQuaternion_(xRadians, yRadians, zRadians);

            var scaleX = animatedJoint.GetAnimValue(values.scalesX, f);
            var scaleY = animatedJoint.GetAnimValue(values.scalesY, f);
            var scaleZ = animatedJoint.GetAnimValue(values.scalesZ, f);
            scaleKeyframes[time] = new CsVector(scaleX, scaleY, scaleZ);
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
      var mesh = GltfExporter.WriteMesh_(jointNodes, model, bmd);
      scene.CreateNode()
           .WithSkinnedMesh(mesh, rootNode.WorldMatrix, jointNodes.ToArray());

      var writeSettings = new WriteSettings {
          ImageWriting = ResourceWriteMode.SatelliteFile,
      };
      model.Save(filePath, writeSettings);
    }

    private static SoftwareModelViewMatrixTransformer transformer =
        new SoftwareModelViewMatrixTransformer();

    private static Mesh WriteMesh_(
        Node[] jointNodes,
        ModelRoot model,
        BMD bmd) {
      MaterialBuilder? currentMaterial = new MaterialBuilder("null")
                                         .WithDoubleSide(true)
                                         .WithUnlitShader();

      var entries = bmd.INF1.Entries;
      var joints = bmd.GetJoints();

      var vertexPositions = bmd.VTX1.Positions;
      var vertexNormals = bmd.VTX1.Normals;
      var vertexColors = bmd.VTX1.Colors;
      var vertexUvs = bmd.VTX1.Texcoords;
      var batches = bmd.SHP1.Batches;

      var meshBuilder = VERTEX.CreateCompatibleMesh();

      var matrixIndices = new Dictionary<int, int>();

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
            var m = GlMatrixUtil.CsToMn(gltfNode.WorldMatrix);
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
            var materialIndex = bmd.MAT3.MaterialEntryIndieces[entry.Index];
            var material = bmd.MAT3.MaterialEntries[materialIndex];

            // TODO: Use textures
            /*if (material.TexStages[0] != ushort.MaxValue) {
              bmd.MAT3.TextureIndieces[material]

              currentMaterial = null;
            } else {
              currentMaterial = null;
            }*/

            /*for (int index = 0; index < 8; ++index) {
              if (this.MAT3.MaterialEntries[
                          (int) this.MAT3.MaterialEntryIndieces[
                              (int) entry.Index]]
                      .TexStages[index] !=
                  ushort.MaxValue)
                Gl.glBindTexture(3553,
                                 (int) this.MAT3.TextureIndieces[
                                     (int) this
                                           .MAT3.MaterialEntries[
                                               (int) this
                                                     .MAT3
                                                     .MaterialEntryIndieces
                                                     [(int) entry
                                                          .Index]]
                                           .TexStages[index]] +
                                 1);
              else
                Gl.glBindTexture(3553, 0);
            }*/

            /*Gl.glMatrixMode(5888);
            this.MAT3.glAlphaCompareglBendMode(
                (int) this
                      .MAT3.MaterialEntries[
                          (int) this.MAT3.MaterialEntryIndieces[
                              (int) entry.Index]]
                      .Indices2[1],
                (int) this
                      .MAT3.MaterialEntries[
                          (int) this.MAT3.MaterialEntryIndieces[
                              (int) entry.Index]]
                      .Indices2[2],
                (int) this
                      .MAT3.MaterialEntries[
                          (int) this.MAT3.MaterialEntryIndieces[
                              (int) entry.Index]]
                      .Indices2[3]);
            this.Shaders[
                    (int) this.MAT3.MaterialEntryIndieces[(int) entry.Index]]
                .Enable();*/
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
                  var weightedMatrices = new List<MathNet.Numerics.LinearAlgebra.Matrix<double>>();
                  var skinning = new List<(int, float)>();

                  for (var w = 0; w < weightedIndices.Indices.Length; ++w) {
                    var jointIndex = weightedIndices.Indices[w];
                    var weight = weightedIndices.Weights[w];

                    if (jointIndex >= joints.Length) {
                      throw new InvalidDataException();
                    }

                    var skinToLocalLimbMatrix = ConvertMkdsToMn_(bmd.EVP1.Matrices[jointIndex]);
                    var localLimbToWorldMatrix = matrices[jointIndex];
                    var skinToWorldMatrix = localLimbToWorldMatrix.Multiply(skinToLocalLimbMatrix);
                    //var skinToWorldMatrix = skinToLocalLimbMatrix.Multiply(localLimbToWorldMatrix);

                    weightedMatrices.Add(skinToWorldMatrix);

                    if (IncludeRootNode) {
                      skinning.Add((1 + jointIndex, weight));
                    } else {
                      skinning.Add((jointIndex, weight));
                    }
                  }

                  var wm = new WeightedMatrix {
                      Matrices = weightedMatrices.ToArray(),
                      Skinning = skinning.ToArray(),
                  };
                  matrixTable[i] = wm;
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

                  var wm = new WeightedMatrix {
                      Matrices = new [] {matrices[jointIndex]},
                      Skinning = skinning.ToArray(),
                  };
                  matrixTable[i] = wm;
                }
              }

              // Adds primitives
              foreach (var primitive in packet.Primitives) {
                var points = primitive.Points;
                var pointsCount = points.Length;
                var vertices = new VERTEX[pointsCount];

                // TODO: Here or just above primitives?
                var weightedMatrix = matrixTable[0];

                for (var p = 0; p < pointsCount; ++p) {
                  var point = points[p];

                  var matrixIndex = point.MatrixIndex / 3;
                  weightedMatrix = matrixTable[matrixIndex];

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
                  }

                  // TODO: Support multiple texture coords?
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
                        meshBuilder.UsePrimitive(currentMaterial, 3);

                    for (var v = 0; v < pointsCount; v += 3) {
                      triangles.AddTriangle(vertices[v + 0],
                                            vertices[v + 1],
                                            vertices[v + 2]);
                    }

                    break;
                  }

                  case Gl.GL_TRIANGLE_STRIP: {
                    var triangleStrip =
                        meshBuilder.UsePrimitive(currentMaterial, 3);

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

                  case Gl.GL_QUADS: {
                    var quads =
                        meshBuilder.UsePrimitive(currentMaterial, 4);

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
      double outX = 0;
      double outY = 0;
      double outZ = 0;

      transformer.Push();
      for (var i = 0; i < weightedMatrix.Skinning.Length; ++i) {
        var (_, weight) = weightedMatrix.Skinning[i];
        var matrix = weightedMatrix.Matrices[i];

        var tempX = x;
        var tempY = y;
        var tempZ = z;

        // TODO: Need to factor in binding matrix to get coord in skin space.

        transformer.Set(matrix);
        transformer.ProjectVertex(ref tempX, ref tempY, ref tempZ);

        outX += weight * tempX;
        outY += weight * tempY;
        outZ += weight * tempZ;
      }
      transformer.Pop();

      x = outX;
      y = outY;
      z = outZ;
    }

    private static void ProjectNormalWeighted(
        WeightedMatrix weightedMatrix,
        ref double x,
        ref double y,
        ref double z) {
      double outX = 0;
      double outY = 0;
      double outZ = 0;

      transformer.Push();
      for (var i = 0; i < weightedMatrix.Skinning.Length; ++i) {
        var (_, weight) = weightedMatrix.Skinning[i];
        var matrix = weightedMatrix.Matrices[i];

        var tempX = x;
        var tempY = y;
        var tempZ = z;

        transformer.Set(matrix);
        transformer.ProjectNormal(ref tempX, ref tempY, ref tempZ);

        outX += weight * tempX;
        outY += weight * tempY;
        outZ += weight * tempZ;
      }
      transformer.Pop();

      var len = Math.Sqrt(outX * outX + outY * outY + outZ * outZ);
      x = outX / len;
      y = outY / len;
      z = outZ / len;
    }

    private static CsQuaternion CreateQuaternion_(
        float xRadians,
        float yRadians,
        float zRadians) {
      var qz = CsQuaternion.CreateFromYawPitchRoll(0, 0, zRadians);
      var qy = CsQuaternion.CreateFromYawPitchRoll(yRadians, 0, 0);
      var qx = CsQuaternion.CreateFromYawPitchRoll(0, xRadians, 0);

      return CsQuaternion.Normalize(qz * qy * qx);
    }

    private static CsVector ConvertMsToCs_(MsVector msVector, float scale = 1)
      => new CsVector {
          X = msVector.X * scale,
          Y = msVector.Y * scale,
          Z = msVector.Z * scale,
      };

    private static CsQuaternion ConvertMsToCs_(MsQuaternion msQuaternion) =>
        new CsQuaternion {
            X = msQuaternion.X,
            Y = msQuaternion.Y,
            Z = msQuaternion.Z,
            W = msQuaternion.W
        };

    private static Matrix ConvertMkdsToMn_(MTX44 mkds) {
      var mn = new DenseMatrix(4, 4);

      for (var y = 0; y < 4; ++y) {
        for (var x = 0; x < 4; ++x) {
          mn[y, x] = mkds[x, y];
        }
      }

      return mn;
    }
    private static TextureWrapMode GetWrapMode_(GxWrapTag wrapMode) {
      if ((wrapMode & GxWrapTag.GX_MIRROR) != 0) {
        return TextureWrapMode.MIRRORED_REPEAT;
      }

      if ((wrapMode & GxWrapTag.GX_REPEAT) != 0) {
        return TextureWrapMode.REPEAT;
      }

      return TextureWrapMode.CLAMP_TO_EDGE;
    }
  }

  public class WeightedMatrix {
    public (int, float)[] Skinning { get; set; }
    public MathNet.Numerics.LinearAlgebra.Matrix<double>[] Matrices { get; set; }
  }
}