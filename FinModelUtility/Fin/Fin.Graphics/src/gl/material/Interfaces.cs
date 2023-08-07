using fin.model;


namespace fin.graphics.gl.material {
  public interface IGlMaterialShader : IDisposable {
    IReadOnlyMaterial Material { get; }

    bool UseLighting { get; set; }
    bool DisposeTextures { get; set; }

    void Use();
  }
}