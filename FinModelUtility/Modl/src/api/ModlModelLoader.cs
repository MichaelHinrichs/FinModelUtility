using System.Drawing;
using System.Numerics;

using fin.data;
using fin.data.queue;
using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;

using modl.schema.anim;
using modl.schema.modl.bw1;


namespace modl.api {
  public class ModlModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.ModlFile;

    public IFileHierarchyFile ModlFile { get; set; }
    public IList<IFileHierarchyFile>? AnimFiles { get; set; }
  }

  public class ModlModelLoader : IModelLoader<ModlModelFileBundle> {
    public IModel LoadModel(ModlModelFileBundle modelFileBundle) {
      var modlFile = modelFileBundle.ModlFile;

      using var er = new EndianBinaryReader(modlFile.Impl.OpenRead(),
                                            Endianness.LittleEndian);
      var bw1Model = er.ReadNew<Bw1Model>();

      var model = new ModelImpl();
      var finMesh = model.Skin.AddMesh();

      var finBones = new IBone[bw1Model.Nodes.Count];
      var finBonesByWeirdId = new Dictionary<uint, IBone>();

      {
        var nodeQueue =
            new FinTuple2Queue<IBone, ushort>((model.Skeleton.Root, 0));
        while (nodeQueue.TryDequeue(out var parentFinBone,
                                    out var modlNodeId)) {
          var modlNode = bw1Model.Nodes[modlNodeId];

          var transform = modlNode.Transform;
          var bonePosition = transform.Position;

          var modlRotation = transform.Rotation;
          var rotation = new Quaternion(
              modlRotation.X,
              modlRotation.Y,
              modlRotation.Z,
              modlRotation.W);
          var eulerRadians = QuaternionUtil.ToEulerRadians(rotation);

          var finBone =
              parentFinBone
                  .AddChild(bonePosition.X, bonePosition.Y, bonePosition.Z)
                  .SetLocalRotationRadians(
                      eulerRadians.X, eulerRadians.Y, eulerRadians.Z);
          finBone.Name = $"Node {modlNodeId}";
          finBones[modlNodeId] = finBone;
          finBonesByWeirdId[modlNode.WeirdId] = finBone;

          if (bw1Model.CnctParentToChildren.TryGetList(
                  modlNodeId, out var modlChildIds)) {
            nodeQueue.Enqueue(
                modlChildIds!.Select(modlChildId => (finBone, modlChildId)));
          }
        }

        foreach (var animFile in modelFileBundle.AnimFiles ??
                                 Array.Empty<IFileHierarchyFile>()) {
          var anim = animFile.Impl.ReadNew<Anim>(Endianness.BigEndian);


          var animWeirdIds = anim.AnimBones
                                 .Select(animBone => animBone.Data.WeirdId)
                                 .ToList();
          animWeirdIds.Sort();

          var modlWeirdIds = bw1Model.NodeByWeirdId.Keys.ToList();
          modlWeirdIds.Sort();

          ;

          var maxFrameCount = -1;
          foreach (var animBone in anim.AnimBones) {
            maxFrameCount = (int) Math.Max(maxFrameCount,
                                           Math.Max(
                                               animBone.Data
                                                   .PositionKeyframeCount,
                                               animBone.Data
                                                   .RotationKeyframeCount));
          }

          var finAnimation = model.AnimationManager.AddAnimation();
          finAnimation.Name = animFile.NameWithoutExtension;
          finAnimation.FrameRate = 30;
          finAnimation.FrameCount = maxFrameCount;

          for (var b = 0; b < anim.AnimBones.Count; ++b) {
            var animBone = anim.AnimBones[b];
            var animBoneFrames = anim.AnimBoneFrames[b];

            var animWeirdId = animBone.Data.WeirdId;
            var finBone = finBonesByWeirdId[animWeirdId];

            //var finBone = finBones[animBone.BoneIndex];
            var localRotation = finBone.LocalRotation;
            var baseQuaternion = QuaternionUtil.Create(localRotation ?? new ModelImpl.RotationImpl());

            var finBoneTracks = finAnimation.AddBoneTracks(finBone);

            var fbtPositions = finBoneTracks.Positions;
            for (var f = 0; f < animBone.Data.PositionKeyframeCount; ++f) {
              var (fPX, fPY, fPZ) = animBoneFrames.PositionFrames[f];

              fbtPositions.Set(f, 0, fPX);
              fbtPositions.Set(f, 1, fPY);
              fbtPositions.Set(f, 2, fPZ);
            }

            /*var fbtRotations = finBoneTracks.Rotations;
            for (var f = 0; f < animBone.Data.RotationKeyframeCount; ++f) {
              var (fRX, fRY, fRZ, frW) = animBoneFrames.RotationFrames[f];

              var animationQuaternion = new Quaternion(fRX, fRY, fRZ, frW);
              /*var eulerRadians = QuaternionUtil.ToEulerRadians(quaternion);

              var eRX = eulerRadians.X;
              var eRY = eulerRadians.Y;
              var eRZ = eulerRadians.Z;

              var influence = 1;
              var rXR = localRotation.XRadians * influence + eRX;
              var rYR = localRotation.YRadians * influence + eRY;
              var rZR = localRotation.ZRadians * influence + eRZ;*/

              /*var sumQuaternion = baseQuaternion * animationQuaternion;

              var eulerRadians = QuaternionUtil.ToEulerRadians(sumQuaternion);

              var rXR = eulerRadians.X;
              var rYR = eulerRadians.Y;
              var rZR = eulerRadians.Z;

              fbtRotations.Set(f, 0, rXR);
              fbtRotations.Set(f, 1, rYR);
              fbtRotations.Set(f, 2, rZR);
            }*/
          }
        }

        var textureDictionary = new LazyDictionary<string, ITexture>(
            textureName => {
              var textureFile =
                  modlFile.Parent.Files.Single(
                      file => file.Name.ToLower() == $"{textureName}.png");
              var image =
                  (Bitmap) Image.FromFile(textureFile.FullName);

              var finTexture =
                  model.MaterialManager.CreateTexture(image);
              finTexture.Name = textureName;

              // TODO: Need to handle wrapping
              finTexture.WrapModeU = WrapMode.REPEAT;
              finTexture.WrapModeV = WrapMode.REPEAT;

              return finTexture;
            });

        foreach (var modlNode in bw1Model.Nodes) {
          var finMaterials =
              modlNode.Materials.Select(modlMaterial => {
                        var textureName =
                            modlMaterial.Texture1.Replace("\0", "").ToLower();
                        if (textureName == "") {
                          return null;
                        }

                        var finTexture = textureDictionary[textureName];

                        var finMaterial =
                            model.MaterialManager
                                 .AddTextureMaterial(finTexture);

                        return finMaterial;
                      })
                      .ToArray();

          foreach (var modlMesh in modlNode.Meshes) {
            var finMaterial = finMaterials[modlMesh.MaterialIndex];

            foreach (var triangleStrip in modlMesh.TriangleStrips) {
              var vertices =
                  new IVertex[triangleStrip.VertexAttributeIndicesList.Count];
              for (var i = 0; i < vertices.Length; i++) {
                var vertexAttributeIndices =
                    triangleStrip.VertexAttributeIndicesList[i];

                var position =
                    modlNode.Positions[vertexAttributeIndices.PositionIndex];
                var vertex = vertices[i] = model.Skin.AddVertex(
                                 position.X * modlNode.Scale,
                                 position.Y * modlNode.Scale,
                                 position.Z * modlNode.Scale);

                if (vertexAttributeIndices.NormalIndex != null) {
                  var normal =
                      modlNode.Normals[
                          vertexAttributeIndices.NormalIndex.Value];
                  vertex.SetLocalNormal(normal.X, normal.Y, normal.Z);
                }

                if (vertexAttributeIndices.NodeIndex != null) {
                  var finBone =
                      finBones[vertexAttributeIndices.NodeIndex.Value];
                  vertex.SetBoneWeights(
                      model.Skin
                           .GetOrCreateBoneWeights(
                               PreprojectMode.NONE,
                               new BoneWeight(finBone, null, 1)));
                }

                var texCoordIndex0 = vertexAttributeIndices.TexCoordIndices[0];
                var texCoordIndex1 = vertexAttributeIndices.TexCoordIndices[1];
                if (texCoordIndex1 != null) {
                  int texCoordIndex;
                  if (texCoordIndex0 != null) {
                    texCoordIndex =
                        (texCoordIndex0.Value << 8) | texCoordIndex1.Value;
                  } else {
                    texCoordIndex = texCoordIndex1.Value;
                  }

                  var uv = modlNode.UvMaps[0][texCoordIndex];
                  vertex.SetUv(uv.U, uv.V);
                }
              }

              var triangleStripPrimitive = finMesh.AddTriangleStrip(vertices)
                                                  .SetVertexOrder(
                                                      VertexOrder.NORMAL);
              if (finMaterial != null) {
                triangleStripPrimitive.SetMaterial(finMaterial);
              }
            }
          }
        }
      }

      return model;
    }
  }
}