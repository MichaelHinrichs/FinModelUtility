using System;

using fin.model;

using OpenTK.Graphics.OpenGL;

namespace fin.gl.material {
  public abstract class BGlMaterialShader<TMaterial> : IGlMaterialShader
      where TMaterial : IReadOnlyMaterial {
    private readonly GlShaderProgram impl_;

    private int modelViewMatrixLocation_;
    private int projectionMatrixLocation_;
    private int useLightingLocation_;

    protected BGlMaterialShader(TMaterial material) {
      this.Material = material;
      this.impl_ = this.GenerateShaderProgram(material);

      this.modelViewMatrixLocation_ = this.impl_.GetUniformLocation("modelViewMatrix");
      this.projectionMatrixLocation_ = this.impl_.GetUniformLocation("projectionMatrix");
      this.useLightingLocation_ = this.impl_.GetUniformLocation("useLighting");
    }

    ~BGlMaterialShader() => this.Dispose();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();
      this.DisposeInternal();
    }

    protected abstract void DisposeInternal();

    protected abstract GlShaderProgram GenerateShaderProgram(TMaterial material);
    protected abstract void PassUniformsAndBindTextures(GlShaderProgram shaderProgram);

    public IReadOnlyMaterial Material { get; }

    public bool UseLighting { get; set; }

    public void Use() {
      this.impl_.Use();

      var modelViewMatrix = GlTransform.ModelViewMatrix;
      GlTransform.UniformMatrix4(modelViewMatrixLocation_, modelViewMatrix);

      var projectionMatrix = GlTransform.ProjectionMatrix;
      GlTransform.UniformMatrix4(projectionMatrixLocation_, projectionMatrix);

      GL.Uniform1(useLightingLocation_, this.UseLighting ? 1f : 0f);

      this.PassUniformsAndBindTextures(this.impl_);
    }
  }
}