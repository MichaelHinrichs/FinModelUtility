using System.Collections.Generic;

namespace fin.model {
  public interface ISkeleton {
    IBone Root { get; }
  }

  public interface IBone {
    // TODO: Allow caching matrices directly on this type.

    string Name { get; set; }

    IBone Root { get; }
    IBone? Parent { get; }
    IReadOnlyList<IBone> Children { get; }
    IBone AddRoot(float x, float y, float z);
    IBone AddChild(float x, float y, float z);

    IPosition LocalPosition { get; }
    IRotation? LocalRotation { get; }
    IScale? LocalScale { get; }

    IBone SetLocalPosition(float x, float y, float z);
    IBone SetLocalRotationDegrees(float x, float y, float z);
    IBone SetLocalRotationRadians(float x, float y, float z);
    IBone SetLocalScale(float x, float y, float z);
  }
}
