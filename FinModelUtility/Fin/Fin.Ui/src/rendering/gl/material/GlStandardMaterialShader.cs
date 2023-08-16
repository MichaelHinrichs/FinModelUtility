using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.ui.rendering.gl.material {
  public class GlStandardMaterialShader
      : BGlMaterialShader<IStandardMaterial> {
    private GlTexture diffuseTexture_;
    private GlTexture normalTexture_;
    private GlTexture ambientOcclusionTexture_;
    private GlTexture emissiveTexture_;

    public GlStandardMaterialShader(
        IModel model,
        IStandardMaterial standardMaterial,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, standardMaterial, boneTransformManager, lighting) { }

    protected override void DisposeInternal() {
      if (this.DisposeTextures) {
        GlMaterialConstants.DisposeIfNotCommon(this.diffuseTexture_);
        GlMaterialConstants.DisposeIfNotCommon(this.normalTexture_);
        GlMaterialConstants.DisposeIfNotCommon(this.ambientOcclusionTexture_);
        GlMaterialConstants.DisposeIfNotCommon(this.emissiveTexture_);
      }
    }

    protected override void Setup(IStandardMaterial material,
                                  GlShaderProgram shaderProgram) {
      var diffuseTexture = material.DiffuseTexture;
      this.diffuseTexture_ = diffuseTexture != null
          ? GlTexture.FromTexture(diffuseTexture)
          : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var normalTexture = material.NormalTexture;
      this.normalTexture_ = normalTexture != null
          ? GlTexture.FromTexture(normalTexture)
          : GlMaterialConstants.NULL_GRAY_TEXTURE;

      var ambientOcclusionTexture = material.AmbientOcclusionTexture;
      this.ambientOcclusionTexture_ =
          ambientOcclusionTexture != null
              ? GlTexture.FromTexture(ambientOcclusionTexture)
              : GlMaterialConstants.NULL_WHITE_TEXTURE;

      var emissiveTexture = material.EmissiveTexture;
      this.emissiveTexture_ = emissiveTexture != null
          ? GlTexture.FromTexture(emissiveTexture)
          : GlMaterialConstants.NULL_BLACK_TEXTURE;
    }

    protected override void PassUniformsAndBindTextures(GlShaderProgram impl) {
      var diffuseTextureLocation =
          impl.GetUniformLocation("diffuseTexture");
      GL.Uniform1(diffuseTextureLocation, 0);
      this.diffuseTexture_.Bind(0);

      var normalTextureLocation =
          impl.GetUniformLocation("normalTexture");
      GL.Uniform1(normalTextureLocation, 1);
      this.normalTexture_.Bind(1);

      var ambientOcclusionTextureLocation =
          impl.GetUniformLocation("ambientOcclusionTexture");
      GL.Uniform1(ambientOcclusionTextureLocation, 2);
      this.ambientOcclusionTexture_.Bind(2);

      var emissiveTextureLocation =
          impl.GetUniformLocation("emissiveTexture");
      GL.Uniform1(emissiveTextureLocation, 3);
      this.emissiveTexture_.Bind(3);
    }
  }
}