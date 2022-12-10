using System.Collections.Generic;
using fin.data;
using System.Numerics;


namespace fin.model {
  public interface ISkeleton : IEnumerable<IBone> {
    IBone Root { get; }
  }

  public interface ILeafBone : IIndexable {
    string Name { get; set; }

    IBone Root { get; }
    IBone? Parent { get; }

    IPosition LocalPosition { get; }
    IRotation? LocalRotation { get; }
    IScale? LocalScale { get; }

    IBone SetLocalPosition(float x, float y, float z);
    IBone SetLocalRotationDegrees(float x, float y, float z);
    IBone SetLocalRotationRadians(float x, float y, float z);
    IBone SetLocalScale(float x, float y, float z);

    bool IgnoreParentScale { get; set; }

    IBone AlwaysFaceTowardsCamera(Quaternion adjustment);
    bool FaceTowardsCamera { get; }
    Quaternion FaceTowardsCameraAdjustment { get; }
  }

  public interface IBone : ILeafBone {
    IReadOnlyList<IBone> Children { get; }
    IBone AddRoot(float x, float y, float z);
    IBone AddChild(float x, float y, float z);
  }
}