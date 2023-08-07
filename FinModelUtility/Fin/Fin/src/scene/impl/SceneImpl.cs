using fin.animation.playback;
using fin.math;
using fin.model;
using fin.model.impl;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace fin.scene {
  public class SceneImpl : IScene {
    ~SceneImpl() => ReleaseUnmanagedResources_();

    public void Dispose() {
      ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      foreach (var area in this.areas_) {
        area.Dispose();
      }
    }

    private readonly List<ISceneArea> areas_ = new();
    public IReadOnlyList<ISceneArea> Areas => this.areas_;

    public ISceneArea AddArea() {
      var area = new SceneAreaImpl { ViewerScale = this.viewerScale_ };
      this.areas_.Add(area);
      return area;
    }

    public void Tick() {
      foreach (var area in this.areas_) {
        area.Tick();
      }
    }

    private float viewerScale_ = 1;

    public float ViewerScale {
      get => this.viewerScale_;
      set {
        this.viewerScale_ = value;
        foreach (var area in this.areas_) {
          area.ViewerScale = this.viewerScale_;
        }
      }
    }

    private class SceneAreaImpl : ISceneArea {
      ~SceneAreaImpl() => ReleaseUnmanagedResources_();

      public void Dispose() {
        ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        this.CustomSkyboxObject?.Dispose();
        foreach (var obj in this.objects_) {
          obj.Dispose();
        }
      }

      private readonly List<ISceneObject> objects_ = new();

      public IReadOnlyList<ISceneObject> Objects => this.objects_;

      public ISceneObject AddObject() {
        var obj = new SceneObjectImpl { ViewerScale = this.ViewerScale };
        this.objects_.Add(obj);
        return obj;
      }

      public void Tick() {
        foreach (var obj in this.objects_) {
          obj.Tick();
        }
      }

      private float viewerScale_ = 1;

      public float ViewerScale {
        get => this.viewerScale_;
        set {
          this.viewerScale_ = value;
          foreach (var obj in this.objects_) {
            obj.ViewerScale = this.viewerScale_;
          }
        }
      }

      public Color? BackgroundColor { get; set; }
      public ISceneObject? CustomSkyboxObject { get; set; }

      public ISceneObject CreateCustomSkyboxObject()
        => this.CustomSkyboxObject =
            new SceneObjectImpl { ViewerScale = 1 };
    }

    private class SceneObjectImpl : ISceneObject {
      private readonly List<ISceneModel> models_ = new();
      private ISceneObject.OnTick? tickHandler_;

      ~SceneObjectImpl() => ReleaseUnmanagedResources_();

      public void Dispose() {
        ReleaseUnmanagedResources_();
        GC.SuppressFinalize(this);
      }

      private void ReleaseUnmanagedResources_() {
        foreach (var model in this.models_) {
          model.Dispose();
        }
      }

      public Position Position { get; private set; }
      public IRotation Rotation { get; } = new RotationImpl();

      public Scale Scale { get; private set; } = new Scale(1, 1, 1);

      public ISceneObject SetPosition(float x, float y, float z) {
        this.Position = new Position(x, y, z);
        return this;
      }

      public ISceneObject SetRotationRadians(float xRadians,
                                             float yRadians,
                                             float zRadians) {
        this.Rotation.SetRadians(
            xRadians,
            yRadians,
            zRadians
        );
        return this;
      }

      public ISceneObject SetRotationDegrees(float xDegrees,
                                             float yDegrees,
                                             float zDegrees) {
        this.Rotation.SetDegrees(
            xDegrees,
            yDegrees,
            zDegrees
        );
        return this;
      }

      public ISceneObject SetScale(float x, float y, float z) {
        this.Scale = new Scale(x, y, z);
        return this;
      }

      public IReadOnlyList<ISceneModel> Models => this.models_;

      public ISceneModel AddSceneModel()
        => this.AddSceneModel(new ModelImpl());

      public ISceneModel AddSceneModel(IModel model) {
        var sceneModel =
            new SceneModelImpl(model) { ViewerScale = this.ViewerScale };
        this.models_.Add(sceneModel);
        return sceneModel;
      }

      public ISceneObject SetOnTickHandler(ISceneObject.OnTick handler) {
        this.tickHandler_ = handler;
        return this;
      }

      public void Tick() => this.tickHandler_?.Invoke(this);


      private float viewerScale_ = 1;

      public float ViewerScale {
        get => this.viewerScale_;
        set {
          this.viewerScale_ = value;
          foreach (var model in this.models_) {
            model.ViewerScale = this.viewerScale_;
          }
        }
      }
    }

    private class SceneModelImpl : ISceneModel {
      private readonly List<ISceneModel> children_ = new();
      private IAnimation? animation_;

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

        this.AnimationPlaybackManager = new FrameAdvancer { ShouldLoop = true };
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

      public IAnimation? Animation {
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