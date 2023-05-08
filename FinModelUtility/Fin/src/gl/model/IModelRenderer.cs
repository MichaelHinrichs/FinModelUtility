using fin.model;
using System;
using System.Collections.Generic;


namespace fin.gl.model {
  public interface IModelRenderer : IRenderable, IDisposable {
    IModel Model { get; }
    ISet<IMesh> HiddenMeshes { get; }

    bool UseLighting { get; set; }
  }
}