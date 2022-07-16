using System.Collections;

using dat.schema;

using fin.io;
using fin.model;
using fin.model.impl;


namespace dat.api {
  public class DatModelFileBundle : IModelFileBundle {
    public DatModelFileBundle(IFileHierarchyFile datFile) {
      this.DatFile = datFile;
    }

    public IFileHierarchyFile MainFile => this.DatFile;
    public IFileHierarchyFile DatFile { get; }
  }

  public class DatModelLoader : IModelLoader<DatModelFileBundle> {
    public IModel LoadModel(DatModelFileBundle modelFileBundle) {
      using var er = new EndianBinaryReader(
          modelFileBundle.DatFile.Impl.OpenRead(), Endianness.BigEndian);
      var dat = er.ReadNew<Dat>();

      var finModel = new ModelImpl();

      var boneQueue = new Queue<(IBone finParentBone, BoneStruct datBone)>();
      foreach (var datRootBone in dat.RootBoneStructs) {
        boneQueue.Enqueue((finModel.Skeleton.Root, datRootBone));
      }

      while (boneQueue.Count > 0) {
        var (finParentBone, datBone) = boneQueue.Dequeue();

        var datBoneData = datBone.Data;

        var finBone =
            finParentBone.AddChild(datBoneData.X, datBoneData.Y, datBoneData.Z)
                         .SetLocalRotationRadians(
                             datBoneData.RotationRadiansX,
                             datBoneData.RotationRadiansY,
                             datBoneData.RotationRadiansZ)
                         .SetLocalScale(
                             datBoneData.ScaleX,
                             datBoneData.ScaleY,
                             datBoneData.ScaleZ);

        foreach (var datChildBone in datBone.Children) {
          boneQueue.Enqueue((finBone, datChildBone));
        }
      }

      return finModel;
    }
  }
}