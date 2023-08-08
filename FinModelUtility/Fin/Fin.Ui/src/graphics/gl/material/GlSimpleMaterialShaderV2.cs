using fin.math;
using fin.model;
using fin.model.util;


namespace fin.ui.graphics.gl.material {
  public class GlSimpleMaterialShaderV2 : BGlMaterialShader<IReadOnlyMaterial> {
    private readonly GlTexture primaryGlTexture_;

    public GlSimpleMaterialShaderV2(
        IModel model,
        IReadOnlyMaterial material,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting) :
        base(model, material, boneTransformManager, lighting) {
      var primaryFinTexture = PrimaryTextureFinder.GetFor(material);
      this.primaryGlTexture_ = primaryFinTexture != null
          ? GlTexture.FromTexture(primaryFinTexture)
          : GlMaterialConstants.NULL_WHITE_TEXTURE;
    }

    protected override void DisposeInternal() {
      if (this.DisposeTextures) {
        GlMaterialConstants.DisposeIfNotCommon(this.primaryGlTexture_);
      }
    }

    protected override void Setup(
        IReadOnlyMaterial material,
        GlShaderProgram shaderProgram) { }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) {
      // 0 is inferred, and therefore unnecessary to pass in.
      // GL.Uniform1(this.diffuseTextureLocation_, 0);
      this.primaryGlTexture_.Bind();
    }
  }
}