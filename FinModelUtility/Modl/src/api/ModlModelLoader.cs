using System.Drawing;
using System.Numerics;

using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;

using modl.schema.modl.bw1;


namespace modl.api {
  public class ModlModelFileBundle : IModelFileBundle {
    public IFileHierarchyFile MainFile => this.ModlFile;

    public IFileHierarchyFile ModlFile { get; set; }
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

      {
        var nodeQueue = new Queue<(IBone, ushort)>();
        nodeQueue.Enqueue((model.Skeleton.Root, 0));
        while (nodeQueue.Count > 0) {
          var (parentFinBone, modlNodeId) = nodeQueue.Dequeue();

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

          if (bw1Model.CnctParentToChildren.TryGetList(
                  modlNodeId, out var modlChildIds)) {
            foreach (var modlChildId in modlChildIds) {
              nodeQueue.Enqueue((finBone, modlChildId));
            }
          }
        }

        foreach (var modlNode in bw1Model.Nodes) {
          var finMaterials =
              modlNode.Materials.Select(modlMaterial => {
                        var textureName =
                            modlMaterial.Texture1.Replace("\0", "");
                        if (textureName == "") {
                          return null;
                        }

                        var textureFile =
                            modlFile.Parent.Files.Single(
                                file => file.Name == $"{textureName}.png");
                        var image =
                            (Bitmap) Image.FromFile(textureFile.FullName);

                        var finTexture =
                            model.MaterialManager.CreateTexture(image);
                        finTexture.Name = textureName;
                        // TODO: Need to handle wrapping

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
                               PreprojectMode.ROOT,
                               finBone));
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

              finMesh.AddTriangleStrip(vertices)
                     .SetMaterial(finMaterial)
                     .SetVertexOrder(VertexOrder.NORMAL);
            }
          }
        }
      }

      return model;
    }
  }
}