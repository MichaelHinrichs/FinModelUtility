using System;
using System.Collections.Generic;
using System.Linq;

using fin.animation;
using fin.math;
using fin.model;

namespace fin.scene {
  public partial class SceneImpl {
    private class SceneModelImpl : ISceneModel {
      private readonly List<ISceneModel> children_ = new();
      private IModelAnimation? animation_;

      public SceneModelImpl(IModel model) {
        this.Model = model;
        this.BoneTransformManager = new BoneTransformManager();

        this.Init_();
      }

      private SceneModelImpl(IModel model, SceneModelImpl parent, IBone bone) {
        this.Model = model;
        this.BoneTransformManager =
            new BoneTransformManager((parent.BoneTransformManager, bone));

        this.Init_();
      }

      private void Init_() {
        this.BoneTransformManager.CalculateMatrices(
            this.Model.Skeleton.Root,
            this.Model.Skin.BoneWeights,
            null);
        this.BoneTransformManager.InitModelVertices(this.Model, true);

        this.AnimationPlaybackManager = new FrameAdvancer { Config = new AnimationInterpolationConfig { UseLoopingInterpolation = true } };
        this.Animation =
            this.Model.AnimationManager.Animations.FirstOrDefault();
        this.AnimationPlaybackManager.IsPlaying = true;
      }

      ~SceneModelImpl() => ReleaseUnmanagedResources_();

      public void Dispose() {
        ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        foreach (var child in this.children_) {
          child.Dispose();
        }
      }

      public IReadOnlyList<ISceneModel> Children => this.children_;

      public ISceneModel AddModelOntoBone(IModel model, IBone bone) {
        var child = new SceneModelImpl(model, this, bone);
        this.children_.Add(child);
        return child;
      }

      public IModel Model { get; }

      public IBoneTransformManager BoneTransformManager { get; }

      public IModelAnimation? Animation {
        get => this.animation_;
        set {
          if (this.animation_ == value) {
            return;
          }

          this.animation_ = value;

          this.AnimationPlaybackManager.Frame = 0;
          this.AnimationPlaybackManager.FrameRate =
              (int) (value?.FrameRate ?? 20);
          this.AnimationPlaybackManager.TotalFrames =
              value?.FrameCount ?? 0;
        }
      }

      public IAnimationPlaybackManager AnimationPlaybackManager {
        get;
        private set;
      }


      public float ViewerScale { get; set; } = 1;
    }
  }
}