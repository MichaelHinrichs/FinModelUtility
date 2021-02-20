using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.G3D_Binary_File_Format;
using MKDS_Course_Modifier.GCN;

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
    public static void Export(
        string filePath,
        BMD bmd,
        IList<(string, BCA)>? pathsAndBcas = null) {
      var joints = bmd.GetJoints();

      var scale = 1;

      // Options
      var model = ModelRoot.CreateModel();

      var scene = model.UseScene("default");

      var skin = model.CreateSkin();

      var jointNodes = new GltfNode[1 + joints.Length];

      var rootNode = scene.CreateNode();
      jointNodes[0] = rootNode;

      // TODO: Use buffers for shader stuff?
      // TODO: Eliminate redundant definitions.
      // TODO: Include face animations, somehow?
      // TODO: Fix large filesize for Link, seems to be animations?
      // TODO: Tweak shininess.
      // TODO: Fix limb matrices for some characters, like Bazaar Shopkeeper?
      // TODO: Environment color isn't used yet, giving weird colors for link.

      // Gathers up joints and their gltf nodes.
      var jointNameToNode = new Dictionary<string, GltfNode>();
      var jointsAndNodes = new (MkdsNode, GltfNode)[joints.Length];
      for (var j = 0; j < joints.Length; ++j) {
        var joint = joints[j];
        var jointName = joint.Name;

        var parentNode = joint.Parent == null
                             ? rootNode
                             : jointNameToNode[joint.Parent];

        var scaleVec = GltfExporter.ConvertMsToCs_(joint.Scale);

        var node = parentNode.CreateNode(jointName);

        node.WithLocalTranslation(
            GltfExporter.ConvertMsToCs_(joint.Trans, scale));

        var jointRotation = joint.Rot;
        if (jointRotation.Length() > 0) {
          node.WithLocalRotation(GltfExporter.ConvertMsToCs_(jointRotation));
        }

        var jointScale = joint.Scale;
        node.WithLocalScale(GltfExporter.ConvertMsToCs_(jointScale));

        jointNameToNode[jointName] = node;
        jointNodes[1 + j] = node;
        jointsAndNodes[j] = (joint, node);
      }
      skin.BindJoints(jointNodes.ToArray());

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

        var texturePath = $"{basePath}/{i}.png";
        glTexture.ToBitmap().Save(texturePath);

        var glTfImage = new MemoryImage(texturePath);
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
      var bcaCount = pathsAndBcas?.Count ?? 0;
      for (var a = 0; a < bcaCount; ++a) {
        var (bcaPath, bca) = pathsAndBcas![a];
        var animatedJoints = bca.ANF1.Joints;

        var animationName = new FileInfo(bcaPath).Name.Split('.')[0];

        var glTfAnimation = model.UseAnimation(animationName);

        // Writes translation/rotation/scale for each joint.
        var translationKeyframes = new Dictionary<float, Vector3>();
        var rotationKeyframes = new Dictionary<float, Quaternion>();
        var scaleKeyframes = new Dictionary<float, Vector3>();
        foreach (var (joint, node) in jointsAndNodes) {
          var jointIndex = bmd.JNT1.StringTable[joint.Name];
          var animatedJoint = animatedJoints[jointIndex];
          var values = animatedJoint.Values;

          for (var f = 0; f < bca.ANF1.AnimLength; ++f) {
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
      var vertexData = bmd.VTX1;

      /*ModelViewMatrixTransformer.Push();
      ModelViewMatrixTransformer.Identity();

      var vertexBuilders = new VERTEX[this.allVertices_.Count];

      if (!hasLimbs) {
        for (var v = 0; v < this.allVertices_.Count; ++v) {
          var vertex = this.allVertices_[v];
          var position = new Vector3(
              (float)(vertex.X * scale),
              (float)(vertex.Y * scale),
              (float)(vertex.Z * scale));

          vertexBuilders[v] = VERTEX.Create(position).WithSkinning((0, 1));
        }
      } else {
        this.LimbMatrices.UpdateLimbMatrices(this.Limbs,
                                             firstAnimation,
                                             0);

        for (var l = 0; l < limbsAndNodes.Length; ++l) {
          var jointIndex = 1 + l;
          var (limb, _) = limbsAndNodes[l];

          ModelViewMatrixTransformer.Set(
              this.LimbMatrices.GetMatrixForLimb((uint)l));

          foreach (var vertexId in limb.OwnedVertices) {
            var vertex = this.allVertices_[vertexId];

            var x = vertex.X;
            var y = vertex.Y;
            var z = vertex.Z;

            // Model MUST be pre-projected to match the orientation of the rig!
            ModelViewMatrixTransformer.ProjectVertex(ref x, ref y, ref z);

            var position = new Vector3(
                (float)(x * scale),
                (float)(y * scale),
                (float)(z * scale));

            vertexBuilders[vertexId] = VERTEX
                                       .Create(position)
                                       .WithSkinning((jointIndex, 1));
          }
        }
      }*/

      // Builds mesh.
      var meshBuilder = VERTEX.CreateCompatibleMesh();

      /*void AddTrianglesToMesh(IList<TriangleParams> triangles) {
        foreach (var triangle in triangles) {
          var shaderParams = triangle.ShaderParams;
          var enableLighting = shaderParams.EnableLighting;
          var withNormal = enableLighting;
          var withPrimColor = withNormal && shaderParams.EnableCombiner;

          // TODO: Should be possible to merge these by texture/shader.

          var texture0MaterialPair = materials[1 + triangle.TextureIds[0]];

          var texture0Material = shaderParams.EnableSphericalUv
                                     ? texture0MaterialPair.Glossy
                                     : enableLighting
                                         ? texture0MaterialPair.Lit
                                         : texture0MaterialPair.Unlit;

          var trianglePrimitive = meshBuilder.UsePrimitive(texture0Material);
          var triangleVertexBuilders = new List<IVertexBuilder>();

          foreach (var vertexId in triangle.Vertices) {
            var vertex = this.allVertices_[vertexId];

            // TODO: How does environment color fit in?
            var color = withNormal
                            ? withPrimColor
                                  ? new Vector4(shaderParams.PrimColor[0],
                                                shaderParams.PrimColor[1],
                                                shaderParams.PrimColor[2],
                                                shaderParams.PrimColor[3])
                                  : new Vector4(1)
                            : new Vector4(vertex.R / 255f,
                                          vertex.G / 255f,
                                          vertex.B / 255f,
                                          vertex.A / 255f);

            var vertexBuilder = vertexBuilders[vertexId];

            var texture0Id = triangle.TextureIds[0];
            var texture0 = texture0Id >= 0
                               ? this.allTextures_[triangle.TextureIds[0]]
                               : null;
            var tileDescriptor0 = texture0?.TileDescriptor;

            var u = (float)(vertex.U * tileDescriptor0?.TextureWRatio ?? 0);
            var v = (float)(vertex.V * tileDescriptor0?.TextureHRatio ?? 0);
            vertexBuilder =
                vertexBuilder.WithMaterial(color, new Vector2(u, v));

            if (withNormal) {
              // TODO: Normals seem broken?
              // TODO: Might need to pre-project?

              double x = vertex.NormalX;
              double y = vertex.NormalY;
              double z = vertex.NormalZ;
              ModelViewMatrixTransformer.ProjectNormal(ref x, ref y, ref z);

              var normal =
                  Vector3.Normalize(
                      new Vector3((float)x, (float)y, (float)z));
              vertexBuilder =
                  vertexBuilder.WithGeometry(vertexBuilder.Position, normal);
            }

            triangleVertexBuilders.Add(vertexBuilder);
          }

          trianglePrimitive.AddTriangle(triangleVertexBuilders[0],
                                        triangleVertexBuilders[1],
                                        triangleVertexBuilders[2]);
        }
      }

      if (!hasLimbs) {
        AddTrianglesToMesh(this.root_!.Triangles);
      } else {
        for (var l = 0; l < limbsAndNodes.Length; ++l) {
          var (limb, _) = limbsAndNodes[l];

          ModelViewMatrixTransformer.Set(
              this.LimbMatrices.GetMatrixForLimb((uint)l));

          AddTrianglesToMesh(limb.Triangles);
        }
      }
      ModelViewMatrixTransformer.Pop();*/

      var mesh = model.CreateMesh(meshBuilder);

      /*scene.CreateNode()
           .WithSkinnedMesh(mesh, rootNode.WorldMatrix, jointNodes.ToArray());*/

      model.Save(filePath);
    }

    /*private static WriteMesh_(BMD bmd) {
      MaterialBuilder? currentMaterial;

      var vertexPositions = bmd.VTX1.Positions;
      var vertexNormals = bmd.VTX1.Normals;
      var vertexColors = bmd.VTX1.Colors;
      var vertexUvs = bmd.VTX1.Texcoords;
      var batches = bmd.SHP1.Batches;

      var meshBuilder = VERTEX.CreateCompatibleMesh();

      foreach (var entry in this.INF1.Entries) {
        switch (entry.Type) {
          case 0:
            goto label_35;

          case 16:
            Gl.glTranslatef(this.JNT1.Joints[(int) entry.Index].Tx,
                            this.JNT1.Joints[(int) entry.Index].Ty,
                            this.JNT1.Joints[(int) entry.Index].Tz);
            Gl.glRotatef(
                (float) ((double) this.JNT1.Joints[(int) entry.Index].Rx /
                         32768.0 *
                         180.0),
                1f,
                0.0f,
                0.0f);
            Gl.glRotatef(
                (float) ((double) this.JNT1.Joints[(int) entry.Index].Ry /
                         32768.0 *
                         180.0),
                0.0f,
                1f,
                0.0f);
            Gl.glRotatef(
                (float) ((double) this.JNT1.Joints[(int) entry.Index].Rz /
                         32768.0 *
                         180.0),
                0.0f,
                0.0f,
                1f);
            Gl.glScalef(this.JNT1.Joints[(int) entry.Index].Sx,
                        this.JNT1.Joints[(int) entry.Index].Sy,
                        this.JNT1.Joints[(int) entry.Index].Sz);
            break;

          case 17:
            Gl.glMatrixMode(5890);
            Gl.glLoadIdentity();
            for (int index = 0; index < 8; ++index) {
              Gl.glActiveTexture(33984 + index);
              Gl.glLoadIdentity();
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
            }
            Gl.glMatrixMode(5888);
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
                .Enable();
            break;

          case 18:
            var batch = batches[(int) entry.Index];
            foreach (var packet in batch.Packets) {
              foreach (var primitive in packet.Primitives) {
                var points = primitive.Points;
                var pointsCount = points.Length;
                var vertices = new VERTEX[pointsCount];

                for (var p = 0; p < pointsCount; ++p) {
                  var point = points[p];

                  CsVector? gltfPosition = null;
                  if (!batch.HasPositions) {
                    throw new Exception(
                        "How can a point not have a position??");
                  } else {
                    var position = vertexPositions[point.PosIndex];

                    double x = position.X;
                    double y = position.Y;
                    double z = position.Z;

                    PROJECT

                        gltfPosition =
                            new CsVector((float) x, (float) y, (float) z);
                  }
                  var vertexBuilder = VERTEX.Create(gltfPosition.Value);

                  if (batch.HasNormals) {
                    var normal = vertexNormals[point.NormalIndex];

                    double normalX = normal.X;
                    double normalY = normal.Y;
                    double normalZ = normal.Z;

                    PROJECT

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
                meshBuilder.UsePrimitive(currentMaterial);
                
                APPEND TO MODEL
              }
            }
            break;
        }
      }
    }*/

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
}