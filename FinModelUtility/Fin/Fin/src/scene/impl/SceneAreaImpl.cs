using System;
using System.Collections.Generic;
using System.Drawing;

namespace fin.scene {
  public partial class SceneImpl {
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

      private readonly List<ISceneObject> objects_ = [];

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
  }
}