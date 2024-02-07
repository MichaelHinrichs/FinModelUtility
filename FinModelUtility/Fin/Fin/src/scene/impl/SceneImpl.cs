using System;
using System.Collections.Generic;

using fin.model;
using fin.model.impl;

namespace fin.scene {
  public partial class SceneImpl : IScene {
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

    private readonly List<ISceneArea> areas_ = [];
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


    public ILighting? Lighting { get; private set; }
    public ILighting CreateLighting() => this.Lighting = new LightingImpl();


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
  }
}