using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fin.model.impl {
  public partial class ModelImpl {
    public ISkeleton Skeleton { get; } = new SkeletonImpl();

    private class SkeletonImpl : ISkeleton {
      public IBone Root { get; } = new BoneImpl(null, 0, 0, 0);

      private class BoneImpl : IBone {
        private readonly IList<IBone> children_ = new List<IBone>();

        public BoneImpl(IBone? parent, float x, float y, float z) {
          this.Parent = parent;
          this.SetLocalPosition(x, y, z);

          this.Children = new ReadOnlyCollection<IBone>(this.children_);
        }

        public string Name { get; set; }


        public IBone? Parent { get; }
        public IReadOnlyList<IBone> Children { get; }

        public IBone AddChild(float x, float y, float z) {
          var child = new BoneImpl(this, x, y, z);
          this.children_.Add(child);
          return child;
        }


        public IPosition LocalPosition { get; } =
          new PositionImpl();

        public IQuaternion? LocalRotation { get; private set; }

        public IScale? LocalScale { get; private set; }

        public IBone SetLocalPosition(float x, float y, float z) {
          this.LocalPosition.X = x;
          this.LocalPosition.Y = y;
          this.LocalPosition.Z = z;
          return this;
        }

        public IBone SetLocalRotationDegrees(float x, float y, float z) {
          this.LocalRotation ??= new QuaternionImpl();
          this.LocalRotation.SetDegrees(x, y, z);
          return this;
        }

        public IBone SetLocalRotationRadians(float x, float y, float z) {
          this.LocalRotation ??= new QuaternionImpl();
          this.LocalRotation.SetRadians(x, y, z);
          return this;
        }

        public IBone SetLocalScale(float x, float y, float z) {
          this.LocalScale ??= new ScaleImpl {X = x, Y = y, Z = z};
          return this;
        }
      }
    }
  }
}