using System.Numerics;

using fin.math;
using fin.math.matrix.four;
using fin.math.matrix.three;
using fin.math.rotations;
using fin.model;
using fin.shaders.glsl;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public abstract class BGlMaterialShader<TMaterial> : IGlMaterialShader
      where TMaterial : IReadOnlyMaterial {
    private LinkedList<CachedTextureUniformData> cachedTextureUniformDatas_ =
        new();

    private LinkedList<CachedLightUniformData> cachedLightUniformDatas_ = new();

    private readonly IModel model_;
    private readonly ILighting? lighting_;
    private readonly IBoneTransformManager? boneTransformManager_;
    private readonly GlShaderProgram impl_;

    private readonly int modelViewMatrixLocation_;
    private readonly int projectionMatrixLocation_;

    private readonly int matricesLocation_;
    private readonly Matrix4x4[] matrices_;

    private readonly int shininessLocation_;

    private readonly int useLightingLocation_;
    private readonly int ambientLightColorLocation_;

    protected BGlMaterialShader(
        IModel model,
        TMaterial material,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) {
      this.model_ = model;
      this.Material = material;
      this.boneTransformManager_ = boneTransformManager;
      this.lighting_ = lighting;

      var shaderSource = this.GenerateShaderSource(model, material);
      this.impl_ = GlShaderProgram.FromShaders(
          shaderSource.VertexShaderSource,
          shaderSource.FragmentShaderSource);

      this.modelViewMatrixLocation_ =
          this.impl_.GetUniformLocation(
              GlslConstants.UNIFORM_MODEL_VIEW_MATRIX_NAME);
      this.projectionMatrixLocation_ =
          this.impl_.GetUniformLocation(
              GlslConstants.UNIFORM_PROJECTION_MATRIX_NAME);
      this.matricesLocation_ = this.impl_.GetUniformLocation(
          GlslConstants.UNIFORM_BONE_MATRICES_NAME);
      this.matrices_ = new Matrix4x4[1 + model.Skin.BoneWeights.Count];

      this.shininessLocation_ =
          this.impl_.GetUniformLocation(GlslConstants.UNIFORM_SHININESS_NAME);

      this.useLightingLocation_ = this.impl_.GetUniformLocation(
          GlslConstants.UNIFORM_USE_LIGHTING_NAME);

      this.ambientLightColorLocation_ =
          this.impl_.GetUniformLocation("ambientLightColor");

      if (lighting != null) {
        var lights = lighting.Lights;
        for (var i = 0; i < lights.Count; ++i) {
          var light = lights[i];
          this.cachedLightUniformDatas_.AddLast(
              new CachedLightUniformData(i, light, this.impl_));
        }
      }

      this.Setup(material, this.impl_);
    }

    ~BGlMaterialShader() => this.Dispose();

    public void Dispose() {
      this.ReleaseUnmanagedResources_();
      GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources_() {
      this.impl_.Dispose();

      if (this.DisposeTextures) {
        foreach (var cachedTextureUniformData in
                 this.cachedTextureUniformDatas_) {
          GlMaterialConstants.DisposeIfNotCommon(
              cachedTextureUniformData.GlTexture);
        }
      }

      this.DisposeInternal();
    }

    protected abstract void DisposeInternal();

    protected virtual IShaderSourceGlsl GenerateShaderSource(
        IModel model,
        TMaterial material) => material.ToShaderSource(model, true);

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

      this.matrices_[0] = Matrix4x4.Identity;
      foreach (var boneWeights in this.model_.Skin.BoneWeights) {
        this.matrices_[1 + boneWeights.Index] = this
                                                .boneTransformManager_
                                                ?.GetTransformMatrix(
                                                    boneWeights)
                                                .Impl ?? Matrix4x4.Identity;
      }

      GlTransform.UniformMatrix4s(this.matricesLocation_, this.matrices_);

      GL.Uniform1(this.shininessLocation_, this.Material.Shininess);

      this.PassInLightUniforms_(this.lighting_);

      foreach (var cachedTextureUniformData in
               this.cachedTextureUniformDatas_) {
        cachedTextureUniformData.BindTextureAndPassInUniforms();
      }

      this.PassUniformsAndBindTextures(this.impl_);
    }

    private void PassInLightUniforms_(ILighting? lighting) {
      var useLighting = this.UseLighting && this.lighting_ != null;
      GL.Uniform1(this.useLightingLocation_, useLighting ? 1f : 0f);

      if (!useLighting) {
        return;
      }

      var ambientLightStrength = lighting.AmbientLightStrength;
      var ambientLightColor = lighting.AmbientLightColor;
      GL.Uniform4(this.ambientLightColorLocation_,
                  ambientLightColor.Rf * ambientLightStrength,
                  ambientLightColor.Gf * ambientLightStrength,
                  ambientLightColor.Bf * ambientLightStrength,
                  ambientLightColor.Af * ambientLightStrength);

      foreach (var cachedLightUniformData in this.cachedLightUniformDatas_) {
        cachedLightUniformData.PassInUniforms();
      }
    }

    protected void SetUpTexture(
        string textureName,
        int textureIndex,
        ITexture? finTexture,
        GlTexture glTexture)
      => this.cachedTextureUniformDatas_.AddLast(
          new CachedTextureUniformData(textureName,
                                       textureIndex,
                                       finTexture,
                                       glTexture,
                                       this.impl_));
  }
}