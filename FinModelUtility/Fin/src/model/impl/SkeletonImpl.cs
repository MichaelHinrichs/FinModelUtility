using System.Collections.Generic;
using System.Collections.ObjectModel;

using fin.data;


namespace fin.model.impl {
  public partial class ModelImpl {
    public ISkeleton Skeleton { get; } = new SkeletonImpl();

    private class SkeletonImpl : ISkeleton {
      public IBone Root { get; } = new BoneImpl(null, 0, 0, 0);

      private class BoneImpl : IBone {
        private readonly IList<IBone> children_ = new List<IBone>();
        private readonly Counter counter_;

        public BoneImpl(IBone? parent, float x, float y, float z) {
          this.Root = this;
          this.Parent = parent;
          this.SetLocalPosition(x, y, z);

          this.Children = new ReadOnlyCollection<IBone>(this.children_);

          this.counter_ = (parent as BoneImpl)?.counter_ ?? new Counter();
          this.Id = this.counter_.GetAndIncrement();
        }

        public BoneImpl(IBone root, IBone? parent, float x, float y, float z) {
          this.Root = root;
          this.Parent = parent;
          this.SetLocalPosition(x, y, z);

          this.Children = new ReadOnlyCollection<IBone>(this.children_);

          this.counter_ = (parent as BoneImpl ?? root as BoneImpl)!.counter_;
          this.Id = this.counter_.GetAndIncrement();
        }

        public string Name { get; set; }
        public int Id { get; set; }

        public IBone Root { get; }
        public IBone? Parent { get; }
        public IReadOnlyList<IBone> Children { get; }


        public IBone AddRoot(float x, float y, float z) {
          var child = new BoneImpl(this, x, y, z);
          this.children_.Add(child);
          return child;
        }

        public IBone AddChild(float x, float y, float z) {
          var child = new BoneImpl(this.Root, this, x, y, z);
          this.children_.Add(child);
          return child;
        }


        public IPosition LocalPosition { get; } =
          new PositionImpl();

        public IRotation? LocalRotation { get; private set; }

        public IScale? LocalScale { get; private set; }

        public IBone SetLocalPosition(float x, float y, float z) {
          this.LocalPosition.X = x;
          this.LocalPosition.Y = y;
          this.LocalPosition.Z = z;
          return this;
        }

        public IBone SetLocalRotationDegrees(float x, float y, float z) {
          this.LocalRotation ??= new RotationImpl();
          this.LocalRotation.SetDegrees(x, y, z);
          return this;
        }

        public IBone SetLocalRotationRadians(float x, float y, float z) {
          this.LocalRotation ??= new RotationImpl();
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