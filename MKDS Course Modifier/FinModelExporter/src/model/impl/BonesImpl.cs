using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace fin.model.impl {
  public partial class ModelImpl {
    public IBones Bones { get; } = new BonesImpl();

    private class BonesImpl : IBones {
      private IList<IBone> impl_ = new List<IBone>();

      public BonesImpl() {
        this.All = new ReadOnlyCollection<IBone>(this.impl_);
      }

      public IReadOnlyList<IBone> All { get; }

      public IBone AddBone(float x, float y, float z) {
        var bone = new BoneImpl(x, y, z);
        this.impl_.Add(bone);
        return bone;
      }

      private class BoneImpl : IBone {
        public BoneImpl(float x, float y, float z)
          => this.SetLocalPosition(x, y, z);

        public string Name { get; set; }

        public IPosition LocalPosition { get; } =
          new PositionImpl();

        public IQuaternion LocalRotation { get; }

        public IScale LocalScale { get; } = new ScaleImpl();

        public IBone SetLocalPosition(float x, float y, float z) {
          this.LocalPosition.X = x;
          this.LocalPosition.Y = y;
          this.LocalPosition.Z = z;
          return this;
        }

        public IBone SetLocalRotationDegrees(float x, float y, float z) {
          this.LocalRotation.SetDegrees(x, y, z);
          return this;
        }

        public IBone SetLocalRotationRadians(float x, float y, float z) { 
          this.LocalRotation.SetRadians(x, y, z);
          return this;
        }

        public IBone SetLocalScale(float x, float y, float z) {
          this.LocalScale.X = x;
          this.LocalScale.Y = y;
          this.LocalScale.Z = z;
          return this;
        }
      }
    }
  }
}