using System.Numerics;

using fin.io;
using fin.math;
using fin.model;
using fin.model.impl;

using modl.protos.bw1;


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

      {
        var nodeQueue = new Queue<(IBone, ushort)>();
        nodeQueue.Enqueue((model.Skeleton.Root, 0));

        while (nodeQueue.Count > 0) {
          var (parentFinBone, modlNodeId) = nodeQueue.Dequeue();

          var modlNode = bw1Model.Nodes[modlNodeId];

          var transform = modlNode.Transform;
          var position = transform.Position;

          var modlRotation = transform.Rotation;
          var rotation = new Quaternion(
              modlRotation.X,
              modlRotation.Y,
              modlRotation.Z,
              modlRotation.W);
          var eulerRadians = QuaternionUtil.ToEulerRadians(rotation);

          var finBone =
              parentFinBone.AddChild(position.X, position.Y, position.Z)
                           .SetLocalRotationRadians(
                               eulerRadians.X, eulerRadians.Y, eulerRadians.Z);
          finBone.Name = $"Node {modlNodeId}";

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