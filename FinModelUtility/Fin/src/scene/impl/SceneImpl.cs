using fin.animation.playback;
using fin.gl.material;
using fin.gl.model;
using fin.math;
using fin.model;
using fin.model.impl;
using fin.util.optional;
using System.Collections.Generic;
using System.Linq;


namespace fin.scene {
  public class SceneImpl : IScene {
    private readonly List<ISceneArea> areas_ = new();
    public IReadOnlyList<ISceneArea> Areas => this.areas_;

    public ISceneArea AddArea() {
      var area = new SceneAreaImpl();
      this.areas_.Add(area);
      return area;
    }

    public void Tick() {
      foreach (var area in this.areas_) {
        area.Tick();
      }
    }

    public void Render() {
      foreach (var area in this.areas_) {
        area.Render();
      }
    }

    private class SceneAreaImpl : ISceneArea {
      private readonly List<ISceneObject> objects_ = new();
      public IReadOnlyList<ISceneObject> Objects => this.objects_;

      public ISceneObject AddObject() {
        var obj = new SceneObjectImpl();
        this.objects_.Add(obj);
        return obj;
      }

      public void Tick() {
        foreach (var obj in this.objects_) {
          obj.Tick();
        }
      }

      public void Render() {
        foreach (var obj in this.objects_) {
          obj.Render();
        }
      }
    }

    private class SceneObjectImpl : ISceneObject {
      private readonly List<ISceneModel> models_ = new();
      private ISceneObject.OnTick? tickHandler_;

      public IPosition Position { get; } = new ModelImpl.PositionImpl();
      public IRotation Rotation { get; } = new ModelImpl.RotationImpl();

      public ISceneObject SetPosition(IPosition position) {
        this.Position.X = position.X;
        this.Position.Y = position.Y;
        this.Position.Z = position.Z;
        return this;
      }

      public ISceneObject SetRotation(IRotation rotation) {
        this.Rotation.SetRadians(
            rotation.XRadians,
            rotation.YRadians,
            rotation.ZRadians
        );
        return this;
      }

      public IReadOnlyList<ISceneModel> Models => this.models_;

      public ISceneModel AddSceneModel()
        => this.AddSceneModel(new ModelImpl());

      public ISceneModel AddSceneModel(IModel model) {
        var sceneModel = new SceneModelImpl(model);
        this.models_.Add(sceneModel);
        return sceneModel;
      }

      public ISceneObject SetOnTickHandler(ISceneObject.OnTick handler) {
        this.tickHandler_ = handler;
        return this;
      }

      public void Tick() => this.tickHandler_?.Invoke(this);

      public void Render() {
        foreach (var model in this.Models) {
          model.Render();
        }
      }
    }

    private class SceneModelImpl : ISceneModel {
      private readonly IBoneTransformManager boneTransformManager_;

      public SceneModelImpl(IModel model) {
        this.Model = model;
        this.boneTransformManager_ = new BoneTransformManager();

        this.Init_();
      }

      private SceneModelImpl(IModel model, SceneModelImpl parent, IBone bone) {
        this.Model = model;
        this.boneTransformManager_ =
            new BoneTransformManager((parent.boneTransformManager_, bone));

        this.Init_();
      }

      private void Init_() {
        this.ModelRenderer =
            new ModelRendererV2(this.Model, this.boneTransformManager_);

        var hasNormals = false;
        foreach (var vertex in this.Model.Skin.Vertices) {
          if (vertex.LocalNormal != null) {
            hasNormals = true;
            break;
          }
        }
        this.ModelRenderer.UseLighting = hasNormals;

        this.Animation =
            this.Model.AnimationManager.Animations.FirstOrDefault();
        this.AnimationPlaybackManager = new FrameAdvancer();
      }

      public void Render() {
        if (this.Animation != null) {
          this.AnimationPlaybackManager.Tick();

          var frame = (float)this.AnimationPlaybackManager.Frame;
          this.boneTransformManager_.CalculateMatrices(
              this.Model.Skeleton.Root,
              this.Model.Skin.BoneWeights,
              (this.Animation, frame),
              this.AnimationPlaybackManager.ShouldLoop);

          this.ModelRenderer.InvalidateDisplayLists();

          var hiddenMeshes = this.ModelRenderer.HiddenMeshes;

          hiddenMeshes.Clear();
          var defaultDisplayState = Optional.Of(MeshDisplayState.VISIBLE);
          foreach (var (mesh, meshTracks) in this.Animation.MeshTracks) {
            var displayState =
                meshTracks.DisplayStates.GetInterpolatedFrame(
                    frame, defaultDisplayState);
            if (displayState.Assert() == MeshDisplayState.HIDDEN) {
              hiddenMeshes.Add(mesh);
            }
          }
        }

        this.ModelRenderer.Render();

        if (this.ShowSkeleton) {
          CommonShaderPrograms.TEXTURELESS_SHADER_PROGRAM.Use();
          this.SkeletonRenderer?.Render();
        }
      }

      public IReadOnlyList<ISceneModel> Children { get; }

      public ISceneModel AddModelOntoBone(IModel model, IBone bone) {
        throw new System.NotImplementedException();
      }

      public IModel Model { get; }
      public IModelRenderer ModelRenderer { get; private set; }

      public IAnimation? Animation { get; set; }

      public IAnimationPlaybackManager AnimationPlaybackManager {
        get;
        private set;
      }

      public bool ShowSkeleton { get; set; }
      public ISkeletonRenderer SkeletonRenderer { get; private set; }
    }
  }
}