using System.Numerics;

using fin.math;
using fin.math.matrix.three;
using fin.model;
using fin.shaders.glsl;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class CachedTextureUniformData {
    public required int TextureIndex { get; init; }
    public required ITexture? FinTexture { get; init; }
    public required GlTexture GlTexture { get; init; }
    public required IReadOnlyFinMatrix3x2 Transform { get; init; }

    public required int SamplerLocation { get; init; }
    public required int ClampMinLocation { get; init; }
    public required int ClampMaxLocation { get; init; }
    public required int TransformLocation { get; init; }
  }

  public abstract class BGlMaterialShader<TMaterial> : IGlMaterialShader
      where TMaterial : IReadOnlyMaterial {
    private LinkedList<CachedTextureUniformData> cachedTextureUniformDatas_ =
        new();

    private readonly IModel model_;
    private readonly ILighting? lighting_;
    private readonly IBoneTransformManager? boneTransformManager_;
    private readonly GlShaderProgram impl_;

    private readonly int modelViewMatrixLocation_;
    private readonly int projectionMatrixLocation_;

    private readonly int matricesLocation_;
    private readonly Matrix4x4[] matrices_;

    private readonly int useLightingLocation_;

    private readonly int ambientLightColorLocation_;
    private readonly int[] lightEnabledLocations_;
    private readonly int[] lightPositionLocations_;
    private readonly int[] lightNormalLocations_;
    private readonly int[] lightColorLocations_;

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
      this.useLightingLocation_ =
          this.impl_.GetUniformLocation(
              GlslConstants.UNIFORM_USE_LIGHTING_NAME);

      this.ambientLightColorLocation_ =
          this.impl_.GetUniformLocation("ambientLightColor");
      this.lightEnabledLocations_ = new int[MaterialConstants.MAX_LIGHTS];
      this.lightPositionLocations_ = new int[MaterialConstants.MAX_LIGHTS];
      this.lightNormalLocations_ = new int[MaterialConstants.MAX_LIGHTS];
      this.lightColorLocations_ = new int[MaterialConstants.MAX_LIGHTS];
      for (var i = 0; i < MaterialConstants.MAX_LIGHTS; ++i) {
        this.lightEnabledLocations_[i] =
            this.impl_.GetUniformLocation(
                $"{MaterialConstants.LIGHTS_NAME}[{i}].enabled");
        this.lightPositionLocations_[i] =
            this.impl_.GetUniformLocation(
                $"{MaterialConstants.LIGHTS_NAME}[{i}].position");
        this.lightNormalLocations_[i] =
            this.impl_.GetUniformLocation(
                $"{MaterialConstants.LIGHTS_NAME}[{i}].normal");
        this.lightColorLocations_[i] =
            this.impl_.GetUniformLocation(
                $"{MaterialConstants.LIGHTS_NAME}[{i}].color");
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
        foreach (var cachedTextureUniformData in cachedTextureUniformDatas_) {
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

      GL.Uniform1(this.useLightingLocation_,
                  this.UseLighting && this.lighting_ != null ? 1f : 0f);

      if (this.lighting_ != null) {
        this.SetUpLightUniforms_(this.lighting_, MaterialConstants.MAX_LIGHTS);
      }

      foreach (var cachedTextureUniformData in
               this.cachedTextureUniformDatas_) {
        this.BindTextureAndSetUpUniforms_(cachedTextureUniformData);
      }

      this.PassUniformsAndBindTextures(this.impl_);
    }

    private void SetUpLightUniforms_(ILighting lighting, int max) {
      var ambientLightColor = lighting.AmbientLightColor;
      GL.Uniform3(this.ambientLightColorLocation_,
                  ambientLightColor.Rf,
                  ambientLightColor.Gf,
                  ambientLightColor.Bf);

      var lights = lighting.Lights;
      for (var i = 0; i < max; ++i) {
        var isEnabled = i < lights.Count && lights[i].Enabled;
        if (!isEnabled) {
          continue;
        }

        GL.Uniform1(this.lightEnabledLocations_[i], 1);
        var light = lights[i];

        var position = light.Position;
        GL.Uniform3(this.lightPositionLocations_[i],
                    position.X,
                    position.Y,
                    position.Z);

        var normal = light.Normal;
        GL.Uniform3(this.lightNormalLocations_[i],
                    normal.X,
                    normal.Y,
                    normal.Z);

        var color = light.Color;
        GL.Uniform4(this.lightColorLocations_[i],
                    color.Rf,
                    color.Gf,
                    color.Bf,
                    color.Af);
      }
    }

    protected void SetUpTexture(
        string textureName,
        int textureIndex,
        ITexture? finTexture,
        GlTexture glTexture) {
      var cachedTextureUniformData = new CachedTextureUniformData {
          TextureIndex = textureIndex,
          FinTexture = finTexture,
          GlTexture = glTexture,
          Transform = CalculateTextureTransform_(finTexture),
          SamplerLocation =
              this.impl_.GetUniformLocation($"{textureName}.sampler"),
          ClampMinLocation =
              this.impl_.GetUniformLocation($"{textureName}.clampMin"),
          ClampMaxLocation =
              this.impl_.GetUniformLocation($"{textureName}.clampMax"),
          TransformLocation =
              this.impl_.GetUniformLocation($"{textureName}.transform"),
      };

      this.cachedTextureUniformDatas_.AddLast(cachedTextureUniformData);
    }

    private static IReadOnlyFinMatrix3x2 CalculateTextureTransform_(
        ITexture? texture) {
      if (texture == null) {
        return FinMatrix3x2.IDENTITY;
      }

      var textureOffset = texture.Offset;
      var textureScale = texture.Scale;
      var textureRotationRadians = texture.RotationRadians;

      if (textureOffset == null &&
          textureScale == null &&
          textureRotationRadians == null) {
        return FinMatrix3x2.IDENTITY;
      }

      Vector2? offset = null;
      if (textureOffset != null) {
        offset = new Vector2(textureOffset.X, textureOffset.Y);
      }

      Vector2? scale = null;
      if (textureScale != null) {
        offset = new Vector2(textureScale.X, textureScale.Y);
      }

      return FinMatrix3x2Util.FromTrss(offset,
                                       textureRotationRadians,
                                       scale,
                                       null);
    }

    private unsafe void BindTextureAndSetUpUniforms_(
        CachedTextureUniformData uniformData) {
      uniformData.GlTexture.Bind(uniformData.TextureIndex);
      GL.Uniform1(uniformData.SamplerLocation, uniformData.TextureIndex);

      OpenTK.Vector2 clampMin = new(-10000);
      OpenTK.Vector2 clampMax = new(10000);

      if (uniformData.FinTexture?.WrapModeU == WrapMode.MIRROR_CLAMP) {
        clampMin.X = -1;
        clampMax.X = 2;
      }

      if (uniformData.FinTexture?.WrapModeV == WrapMode.MIRROR_CLAMP) {
        clampMin.Y = -1;
        clampMax.Y = 2;
      }

      var clampS = uniformData.FinTexture?.ClampS;
      var clampT = uniformData.FinTexture?.ClampT;

      if (clampS != null) {
        clampMin.X = clampS.X;
        clampMax.X = clampS.Y;
      }

      if (clampT != null) {
        clampMin.Y = clampT.X;
        clampMax.Y = clampT.Y;
      }

      GL.Uniform2(uniformData.ClampMinLocation, clampMin);
      GL.Uniform2(uniformData.ClampMaxLocation, clampMax);

      var mat = uniformData.Transform.Impl;
      var ptr = (float*) &mat;
      GL.UniformMatrix2x3(uniformData.TransformLocation, 1, true, ptr);
    }
  }
}