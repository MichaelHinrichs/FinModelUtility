using System;

using fin.model;


namespace fin.gl.material {
  public interface IGlMaterialShaderSource {
    string VertexShaderSource { get; }
    string FragmentShaderSource { get; }
  }

  public interface IGlMaterialShader : IDisposable {
    IReadOnlyMaterial Material { get; }

    bool UseLighting { get; set; }

    void Use();
  }
}