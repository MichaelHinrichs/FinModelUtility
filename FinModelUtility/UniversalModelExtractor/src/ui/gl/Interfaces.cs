using fin.model;


namespace uni.ui.gl {
  public interface IModelRenderer : IDisposable {
    IModel Model { get; }
    ISet<IMesh> HiddenMeshes { get; }

    bool UseLighting { get; set; }

    void InvalidateDisplayLists();
    void Render();
  }

  public interface IMaterialMeshRenderer : IDisposable {
    void InvalidateDisplayLists();
    void Render();
  }
}