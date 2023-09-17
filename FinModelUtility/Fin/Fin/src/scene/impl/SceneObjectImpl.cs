using System;
using System.Collections.Generic;

using fin.model;
using fin.model.impl;

namespace fin.scene {
  public partial class SceneImpl {
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
  }
}