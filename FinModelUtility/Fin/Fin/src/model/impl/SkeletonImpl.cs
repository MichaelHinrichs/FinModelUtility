﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

using fin.data;

namespace fin.model.impl {
  public partial class ModelImpl<TVertex> {
    public ISkeleton Skeleton { get; } = new SkeletonImpl();

    private class SkeletonImpl : ISkeleton {
      public readonly List<IBone> bones = new();

      public IBone Root { get; }
      public IReadOnlyList<IBone> Bones => this.bones;

      public SkeletonImpl() {
        this.Root = new BoneImpl(this, null, 0, 0, 0);
      }

      private class BoneImpl : IBone {
        private readonly SkeletonImpl skeleton_;
        private readonly IList<IBone> children_ = new List<IBone>();
        private readonly Counter counter_;

        public BoneImpl(SkeletonImpl skeletonImpl,
                        IBone? parent,
                        float x,
                        float y,
                        float z) {
          this.skeleton_ = skeletonImpl;
          this.Root = this;
          this.Parent = parent;
          this.SetLocalPosition(x, y, z);

          this.skeleton_.bones.Add(this);

          this.Children = new ReadOnlyCollection<IBone>(this.children_);

          this.counter_ = (parent as BoneImpl)?.counter_ ?? new Counter();
          this.Index = this.counter_.GetAndIncrement();
        }

        public BoneImpl(SkeletonImpl skeletonImpl,
                        IBone root,
                        IBone? parent,
                        float x,
                        float y,
                        float z) {
          this.skeleton_ = skeletonImpl;
          this.Root = root;
          this.Parent = parent;
          this.SetLocalPosition(x, y, z);

          this.skeleton_.bones.Add(this);

          this.Children = new ReadOnlyCollection<IBone>(this.children_);

          this.counter_ = (parent as BoneImpl ?? root as BoneImpl)!.counter_;
          this.Index = this.counter_.GetAndIncrement();
        }

        public string Name { get; set; }
        public int Index { get; set; }

        public override string ToString() => this.Name;

        public IBone Root { get; }
        public IBone? Parent { get; }
        public IReadOnlyList<IBone> Children { get; }


        public IBone AddRoot(float x, float y, float z) {
          var child = new BoneImpl(this.skeleton_, this, x, y, z);
          this.children_.Add(child);
          return child;
        }

        public IBone AddChild(float x, float y, float z) {
          var child = new BoneImpl(this.skeleton_, this.Root, this, x, y, z);
          this.children_.Add(child);
          return child;
        }


        public Position LocalPosition { get; private set; }

        public IRotation? LocalRotation { get; private set; }

        public Scale? LocalScale { get; private set; }

        public IBone SetLocalPosition(float x, float y, float z) {
          this.LocalPosition = new Position(x, y, z);
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
          this.LocalScale ??= new Scale(x, y, z);
          return this;
        }


        public bool IgnoreParentScale { get; set; }


        public IBone AlwaysFaceTowardsCamera(Quaternion adjustment) {
          this.FaceTowardsCamera = true;
          this.FaceTowardsCameraAdjustment = adjustment;
          return this;
        }

        public bool FaceTowardsCamera { get; private set; }
        public Quaternion FaceTowardsCameraAdjustment { get; private set; }
      }

      IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

      public IEnumerator<IBone> GetEnumerator() {
        var queue = new Queue<IBone>();
        queue.Enqueue(this.Root);
        while (queue.Count > 0) {
          var bone = queue.Dequeue();
          yield return bone;

          foreach (var child in bone.Children) {
            queue.Enqueue(child);
          }
        }
      }
    }
  }
}