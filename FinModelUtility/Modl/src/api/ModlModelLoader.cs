using System.Numerics;

using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.json;

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

          foreach (var modlMesh in modlNode.Meshes) {
            foreach (var triangleStrip in modlMesh.TriangleStrips) {
              var vertices = new IVertex[triangleStrip.Positions.Count];

              for (var i = 0; i < vertices.Length; i++) {
                var position = modlNode.Positions[triangleStrip.Positions[i]];

                var vertex = vertices[i] = model.Skin.AddVertex(
                                                    position.X * modlNode.Scale,
                                                    position.Y * modlNode.Scale,
                                                    position.Z * modlNode.Scale)
                                                .SetBoneWeights(
                                                    model.Skin
                                                        .GetOrCreateBoneWeights(
                                                            PreprojectMode.BONE,
                                                            finBone));

                if (modlNode.Normals.Count > 0) {
                  var normal = modlNode.Normals[triangleStrip.Normals[i]];
                  vertex.SetLocalNormal(normal.X, normal.Y, normal.Z);
                }
              }

              finMesh.AddTriangleStrip(vertices);
            }
          }

          if (bw1Model.CnctParentToChildren.TryGetList(
                  modlNodeId, out var modlChildIds)) {
            foreach (var modlChildId in modlChildIds) {
              nodeQueue.Enqueue((finBone, modlChildId));
            }
          }
        }
      }

      return model;
    }
  }
}