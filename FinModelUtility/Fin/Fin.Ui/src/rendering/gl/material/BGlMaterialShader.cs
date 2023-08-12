using fin.math;
using fin.model;
using fin.shaders.glsl;

using OpenTK.Graphics.OpenGL;

using Matrix4x4 = System.Numerics.Matrix4x4;


namespace fin.ui.rendering.gl.material {
  public abstract class BGlMaterialShader<TMaterial> : IGlMaterialShader
      where TMaterial : IReadOnlyMaterial {
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
  }
}