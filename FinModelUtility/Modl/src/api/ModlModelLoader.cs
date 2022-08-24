using System.Numerics;

using fin.data;
using fin.data.queue;
using fin.image;
using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;

using modl.schema.anim;
using modl.schema.modl;
using modl.schema.modl.bw1;
using modl.schema.modl.bw2;


namespace modl.api {
  public enum ModlType {
    BW1,
    BW2,
  }

  public class ModlModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.ModlFile;

    public ModlType ModlType { get; set; }
    public IFileHierarchyFile ModlFile { get; set; }

    public IList<IFileHierarchyFile>? AnimFiles { get; set; }
  }

  public class ModlModelLoader : IModelLoader<ModlModelFileBundle> {
    public IModel LoadModel(ModlModelFileBundle modelFileBundle) {
      var flipSign = ModlFlags.FLIP_HORIZONTALLY ? -1 : 1;

      var modlFile = modelFileBundle.ModlFile;

      using var er = new EndianBinaryReader(modlFile.Impl.OpenRead(),
                                            Endianness.LittleEndian);
      var bwModel = modelFileBundle.ModlType switch {
          ModlType.BW1 => (IBwModel) er.ReadNew<Bw1Model>(),
          ModlType.BW2 => er.ReadNew<Bw2Model>(),
      };

      var model = new ModelImpl();
      var finMesh = model.Skin.AddMesh();

      var finBones = new IBone[bwModel.Nodes.Count];
      var finBonesByModlNode = new Dictionary<IBwNode, IBone>();
      var finBonesByWeirdId = new Dictionary<uint, IBone>();

      {
        var nodeQueue =
            new FinTuple2Queue<IBone, ushort>((model.Skeleton.Root, 0));

        while (nodeQueue.TryDequeue(out var parentFinBone,
                                    out var modlNodeId)) {
          var modlNode = bwModel.Nodes[modlNodeId];

          var transform = modlNode.Transform;
          var bonePosition = transform.Position;

          var modlRotation = transform.Rotation;
          var rotation = new Quaternion(
              flipSign * modlRotation.X,
              modlRotation.Y,
              modlRotation.Z,
              flipSign * modlRotation.W);
          var eulerRadians = QuaternionUtil.ToEulerRadians(rotation);

          var finBone =
              parentFinBone
                  .AddChild(flipSign * bonePosition.X, bonePosition.Y,
                            bonePosition.Z)
                  .SetLocalRotationRadians(
                      eulerRadians.X, eulerRadians.Y, eulerRadians.Z);
          finBone.Name = $"Node {modlNodeId}";
          finBones[modlNodeId] = finBone;
          finBonesByModlNode[modlNode] = finBone;
          finBonesByWeirdId[modlNode.WeirdId] = finBone;

          if (bwModel.CnctParentToChildren.TryGetList(
                  modlNodeId, out var modlChildIds)) {
            nodeQueue.Enqueue(
                modlChildIds!.Select(modlChildId => (finBone, modlChildId)));
          }
        }

        foreach (var animFile in modelFileBundle.AnimFiles ??
                                 Array.Empty<IFileHierarchyFile>()) {
          var anim = animFile.Impl.ReadNew<Anim>(Endianness.BigEndian);

          var maxFrameCount = -1;
          foreach (var animBone in anim.AnimBones) {
            maxFrameCount = (int) Math.Max(maxFrameCount,
                                           Math.Max(
                                               animBone
                                                   .PositionKeyframeCount,
                                               animBone
                                                   .RotationKeyframeCount));
          }

          var finAnimation = model.AnimationManager.AddAnimation();
          finAnimation.Name = animFile.NameWithoutExtension;
          finAnimation.FrameRate = 30;
          finAnimation.FrameCount = maxFrameCount;

          for (var b = 0; b < anim.AnimBones.Count; ++b) {
            var animBone = anim.AnimBones[b];
            var animBoneFrames = anim.AnimBoneFrames[b];

            var animWeirdId = animBone.WeirdId;
            if (!finBonesByWeirdId.TryGetValue(animWeirdId, out var finBone)) {
              // TODO: Gross hack for the vet models, what's the real fix???
              if (animWeirdId == 33) {
                finBone = finBonesByWeirdId[34];
              }
            }

            var finBoneTracks = finAnimation.AddBoneTracks(finBone!);

            var fbtPositions = finBoneTracks.Positions;
            for (var f = 0; f < animBone.PositionKeyframeCount; ++f) {
              var (fPX, fPY, fPZ) = animBoneFrames.PositionFrames[f];

              fbtPositions.Set(f, 0, flipSign * fPX);
              fbtPositions.Set(f, 1, fPY);
              fbtPositions.Set(f, 2, fPZ);
            }

            var fbtRotations = finBoneTracks.Rotations;
            for (var f = 0; f < animBone.RotationKeyframeCount; ++f) {
              var (fRX, fRY, fRZ, frW) = animBoneFrames.RotationFrames[f];

              var animationQuaternion =
                  new Quaternion(flipSign * fRX, fRY, fRZ, flipSign * frW);
              var eulerRadians =
                  QuaternionUtil.ToEulerRadians(animationQuaternion);

              fbtRotations.Set(f, 0, eulerRadians.X);
              fbtRotations.Set(f, 1, eulerRadians.Y);
              fbtRotations.Set(f, 2, eulerRadians.Z);
            }
          }
        }

        var textureDictionary = new LazyDictionary<string, ITexture>(
            textureName => {
              var textureFile =
                  modlFile.Parent.Files.Single(
                      file => file.Name.ToLower() == $"{textureName}.png");

              var image = FinImage.FromFile(textureFile.Impl);

              var finTexture =
                  model.MaterialManager.CreateTexture(image);
              finTexture.Name = textureName;

              // TODO: Need to handle wrapping
              finTexture.WrapModeU = WrapMode.REPEAT;
              finTexture.WrapModeV = WrapMode.REPEAT;

              return finTexture;
            });

        foreach (var modlNode in bwModel.Nodes) {
          var finMaterials =
              modlNode.Materials.Select(modlMaterial => {
                        var textureName = modlMaterial.Texture1.ToLower();
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
                                 flipSign * position.X * modlNode.Scale,
                                 position.Y * modlNode.Scale,
                                 position.Z * modlNode.Scale);

                if (vertexAttributeIndices.NormalIndex != null) {
                  var normal =
                      modlNode.Normals[
                          vertexAttributeIndices.NormalIndex.Value];
                  vertex.SetLocalNormal(flipSign * normal.X, normal.Y,
                                        normal.Z);
                }

                if (vertexAttributeIndices.NodeIndex != null) {
                  var finBone =
                      finBones[vertexAttributeIndices.NodeIndex.Value];
                  vertex.SetBoneWeights(
                      model.Skin
                           .GetOrCreateBoneWeights(
                               PreprojectMode.NONE,
                               new BoneWeight(finBone, null, 1)));
                } else {
                  var finBone = finBonesByModlNode[modlNode];
                  vertex.SetBoneWeights(
                      model.Skin.GetOrCreateBoneWeights(
                          PreprojectMode.BONE, finBone));
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

              var triangleStripPrimitive = finMesh.AddTriangleStrip(vertices);
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