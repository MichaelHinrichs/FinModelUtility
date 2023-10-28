using System.Numerics;

using fin.math;
using fin.math.matrix.four;
using fin.math.matrix.three;
using fin.math.rotations;
using fin.model;
using fin.shaders.glsl;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class CachedTextureUniformData {
    public required int TextureIndex { get; init; }
    public required ITexture? FinTexture { get; init; }
    public required GlTexture GlTexture { get; init; }

    public required IReadOnlyFinMatrix3x2? Transform2d { get; init; }
    public required IReadOnlyFinMatrix4x4? Transform3d { get; init; }

    public required bool HasFancyData { get; init; }
    public required int SamplerLocation { get; init; }
    public required int ClampMinLocation { get; init; }
    public required int ClampMaxLocation { get; init; }
    public required int Transform2dLocation { get; init; }
    public required int Transform3dLocation { get; init; }
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

    private readonly int shininessLocation_;

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

      this.shininessLocation_ =
          this.impl_.GetUniformLocation(GlslConstants.UNIFORM_SHININESS_NAME);

      this.useLightingLocation_ = this.impl_.GetUniformLocation(
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

      GL.Uniform1(this.shininessLocation_, this.Material.Shininess);

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
      var ambientLightStrength = lighting.AmbientLightStrength;
      var ambientLightColor = lighting.AmbientLightColor;
      GL.Uniform4(this.ambientLightColorLocation_,
                  ambientLightColor.Rf * ambientLightStrength,
                  ambientLightColor.Gf * ambientLightStrength,
                  ambientLightColor.Bf * ambientLightStrength,
                  ambientLightColor.Af * ambientLightStrength);

      var lights = lighting.Lights;
      for (var i = 0; i < max; ++i) {
        var isEnabled = i < lights.Count && lights[i].Enabled;

        if (!isEnabled) {
          continue;
        }

        var light = lights[i];
        GL.Uniform1(this.lightEnabledLocations_[i], 1);

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

        var strength = light.Strength;
        var color = light.Color;
        GL.Uniform4(this.lightColorLocations_[i],
                    color.Rf * strength,
                    color.Gf * strength,
                    color.Bf * strength,
                    color.Af * strength);
      }
    }

    protected void SetUpTexture(
        string textureName,
        int textureIndex,
        ITexture? finTexture,
        GlTexture glTexture) {
      int samplerLocation;
      int clampMinLocation = -1;
      int clampMaxLocation = -1;
      int transform2dLocation = -1;
      int transform3dLocation = -1;

      var hasFancyData = GlslUtil.RequiresFancyTextureData(finTexture);
      if (!hasFancyData) {
        samplerLocation = this.impl_.GetUniformLocation($"{textureName}");
      } else {
        samplerLocation =
            this.impl_.GetUniformLocation($"{textureName}.sampler");
        clampMinLocation =
            this.impl_.GetUniformLocation($"{textureName}.clampMin");
        clampMaxLocation =
            this.impl_.GetUniformLocation($"{textureName}.clampMax");
        transform2dLocation =
            this.impl_.GetUniformLocation($"{textureName}.transform2d");
        transform3dLocation =
            this.impl_.GetUniformLocation($"{textureName}.transform3d");
      }

      var isTransform3d = finTexture?.IsTransform3d ?? false;

      var cachedTextureUniformData = new CachedTextureUniformData {
          TextureIndex = textureIndex,
          FinTexture = finTexture,
          GlTexture = glTexture,
          Transform2d = isTransform3d
              ? null
              : CalculateTextureTransform2d_(finTexture),
          Transform3d = isTransform3d
              ? CalculateTextureTransform3d_(finTexture)
              : null,
          HasFancyData = hasFancyData,
          SamplerLocation = samplerLocation,
          ClampMinLocation = clampMinLocation,
          ClampMaxLocation = clampMaxLocation,
          Transform2dLocation = transform2dLocation,
          Transform3dLocation = transform3dLocation,
      };

      this.cachedTextureUniformDatas_.AddLast(cachedTextureUniformData);
    }

    private static IReadOnlyFinMatrix3x2 CalculateTextureTransform2d_(
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
        scale = new Vector2(textureScale.X, textureScale.Y);
      }

      return FinMatrix3x2Util.FromTrss(offset,
                                       textureRotationRadians?.Z,
                                       scale,
                                       null);
    }

    private static IReadOnlyFinMatrix4x4 CalculateTextureTransform3d_(
        ITexture? texture) {
      if (texture == null) {
        return FinMatrix4x4.IDENTITY;
      }

      var textureOffset = texture.Offset;
      var textureScale = texture.Scale;
      var textureRotationRadians = texture.RotationRadians;

      if (textureOffset == null &&
          textureScale == null &&
          textureRotationRadians == null) {
        return FinMatrix4x4.IDENTITY;
      }

      Position? offset = null;
      if (textureOffset != null) {
        offset =
            new Position(textureOffset.X, textureOffset.Y, textureOffset.Z);
      }

      Quaternion? rotation = null;
      if (textureRotationRadians != null) {
        rotation = QuaternionUtil.CreateZyx(textureRotationRadians.X,
                                            textureRotationRadians.Y,
                                            textureRotationRadians.Z);
      }

      Scale? scale = null;
      if (textureScale != null) {
        scale = new(textureScale.X, textureScale.Y, textureScale.Z);
      }

      return FinMatrix4x4Util.FromTrs(offset, rotation, scale);
    }

    private unsafe void BindTextureAndSetUpUniforms_(
        CachedTextureUniformData uniformData) {
      uniformData.GlTexture.Bind(uniformData.TextureIndex);
      GL.Uniform1(uniformData.SamplerLocation, uniformData.TextureIndex);

      if (uniformData.HasFancyData) {
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

        if (!(uniformData.FinTexture?.IsTransform3d ?? false)) {
          var mat2d = uniformData.Transform2d!.Impl;
          var ptr = (float*) &mat2d;
          GL.UniformMatrix2x3(uniformData.Transform2dLocation, 1, true, ptr);
        } else {
          var mat3d = uniformData.Transform3d!.Impl;
          GlTransform.UniformMatrix4(uniformData.Transform3dLocation, mat3d);
        }
      }
    }
  }
}