using System;
using System.Collections.Generic;

using fin.model;

using OpenTK.Graphics.OpenGL;

namespace fin.gl.material {
  public abstract class BGlMaterialShader<TMaterial> : IGlMaterialShader
      where TMaterial : IReadOnlyMaterial {
    private readonly ILighting? lighting_;
    private readonly GlShaderProgram impl_;

    private readonly int modelViewMatrixLocation_;
    private readonly int projectionMatrixLocation_;
    private readonly int useLightingLocation_;

    protected BGlMaterialShader(TMaterial material,
                                ILighting? lighting) {
      this.Material = material;
      this.lighting_ = lighting;

      var shaderSource = this.GenerateShaderSource(material);
      this.impl_ = GlShaderProgram.FromShaders(
          shaderSource.VertexShaderSource,
          shaderSource.FragmentShaderSource);

      this.modelViewMatrixLocation_ =
          this.impl_.GetUniformLocation("modelViewMatrix");
      this.projectionMatrixLocation_ =
          this.impl_.GetUniformLocation("projectionMatrix");
      this.useLightingLocation_ = this.impl_.GetUniformLocation("useLighting");

      this.Setup(material, this.impl_);
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

    protected virtual IGlMaterialShaderSource GenerateShaderSource(
        TMaterial material) => material.ToShaderSource();

    protected abstract void Setup(TMaterial material,
                                  GlShaderProgram shaderProgram);

    protected abstract void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram);

    public string VertexShaderSource => this.impl_.VertexShaderSource;
    public string FragmentShaderSource => this.impl_.FragmentShaderSource;

    public IReadOnlyMaterial Material { get; }

    public bool UseLighting { get; set; }
    public bool DisposeTextures { get; set; } = true;

    public void Use() {
      this.impl_.Use();

      var modelViewMatrix = GlTransform.ModelViewMatrix;
      GlTransform.UniformMatrix4(this.modelViewMatrixLocation_,
                                 modelViewMatrix);

      var projectionMatrix = GlTransform.ProjectionMatrix;
      GlTransform.UniformMatrix4(this.projectionMatrixLocation_,
                                 projectionMatrix);

      GL.Uniform1(this.useLightingLocation_,
                  this.UseLighting && this.lighting_ != null ? 1f : 0f);


      if (this.lighting_ != null) {
        this.SetUpLightUniforms_(this.impl_,
                                 this.lighting_.Lights,
                                 "lights",
                                 MaterialConstants.MAX_LIGHTS);
      }

      this.PassUniformsAndBindTextures(this.impl_);
    }

    private void SetUpLightUniforms_(GlShaderProgram impl,
                                     IReadOnlyList<ILight> lights,
                                     string name,
                                     int max) {
      for (var i = 0; i < max; ++i) {
        var isEnabled = i < lights.Count && lights[i].Enabled;
        var enabledLocation = impl.GetUniformLocation($"{name}[{i}].enabled");
        GL.Uniform1(enabledLocation, isEnabled ? 1 : 0);

        if (!isEnabled) {
          continue;
        }

        var light = lights[i];

        var position = light.Position;
        var positionLocation = impl.GetUniformLocation($"{name}[{i}].position");
        GL.Uniform3(positionLocation, position.X, position.Y, position.Z);

        var normal = light.Normal;
        var normalLocation = impl.GetUniformLocation($"{name}[{i}].normal");
        GL.Uniform3(normalLocation, normal.X, normal.Y, normal.Z);

        var color = light.Color;
        var colorLocation = impl.GetUniformLocation($"{name}[{i}].color");
        GL.Uniform4(colorLocation, color.Rf, color.Gf, color.Bf, color.Af);
      }
    }
  }
}