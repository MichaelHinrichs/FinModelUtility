using System;

using fin.model;


namespace fin.gl.material {
  public interface IGlMaterialShader : IDisposable {
    IMaterial Material { get; }
    void Use();
  }
}