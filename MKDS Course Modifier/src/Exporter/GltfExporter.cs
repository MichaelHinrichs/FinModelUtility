using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Schema2;

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.GCN;

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
        IList<BCA>? bcas = null) {
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
        var node = parentNode.CreateNode(jointName)
                             .WithLocalTranslation(
                                 GltfExporter.ConvertMsToCs_(
                                     joint.Trans,
                                     scale))
                             .WithLocalRotation(
                                 GltfExporter.ConvertMsToCs_(joint.Rot))
                             .WithLocalScale(
                                 GltfExporter.ConvertMsToCs_(joint.Scale));

        jointNameToNode[jointName] = node;
        jointNodes[1 + j] = node;
        jointsAndNodes[j] = (joint, node);
      }
      skin.BindJoints(jointNodes.ToArray());

      // Gathers up texture materials.
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
      var bcaCount = bcas?.Count ?? 0;
      for (var a = 0; a < bcaCount; ++a) {
        var bca = bcas![a];
        var animatedJoints = bca.ANF1.Joints;

        var animationName = $"animation{a}";

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