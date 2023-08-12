using fin.model;


namespace fin.ui.rendering.gl.model {
  public interface IModelRenderer : IRenderable, IDisposable {
    IModel Model { get; }
    ISet<IMesh> HiddenMeshes { get; }

    bool UseLighting { get; set; }
  }
}