namespace uni.ui.gl {
  public interface IModelRenderer : IDisposable {
    void InvalidateDisplayLists();
    void Render();
  }

  public interface IMaterialMeshRenderer : IDisposable {
    void InvalidateDisplayLists();
    void Render();
  }
}