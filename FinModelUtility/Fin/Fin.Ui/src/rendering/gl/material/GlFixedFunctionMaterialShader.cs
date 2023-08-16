using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.ui.rendering.gl.material {
  public class GlFixedFunctionMaterialShader
      : BGlMaterialShader<IReadOnlyFixedFunctionMaterial> {
    private int[] textureLocations_ =
        new int[MaterialConstants.MAX_TEXTURES];

    private IList<GlTexture> textures_;

    public GlFixedFunctionMaterialShader(
        IModel model,
        IReadOnlyFixedFunctionMaterial fixedFunctionMaterial,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting)
        : base(model, fixedFunctionMaterial, boneTransformManager, lighting) { }

    protected override void DisposeInternal() {
      if (this.DisposeTextures) {
        foreach (var texture in this.textures_) {
          GlMaterialConstants.DisposeIfNotCommon(texture);
        }
      }
    }

    protected override void Setup(
        IReadOnlyFixedFunctionMaterial material,
        GlShaderProgram impl) {
      for (var i = 0; i < MaterialConstants.MAX_TEXTURES; ++i) {
        this.textureLocations_[i] =
            impl.GetUniformLocation($"texture{i}");
      }

      var finTextures = material.TextureSources;

      this.textures_ = new List<GlTexture>();
      for (var i = 0; i < MaterialConstants.MAX_TEXTURES; ++i) {
        var finTexture = i < (finTextures?.Count ?? 0)
            ? finTextures[i]
            : null;

        this.textures_.Add(finTexture != null
                               ? GlTexture.FromTexture(finTexture)
                               : GlMaterialConstants.NULL_WHITE_TEXTURE);
      }
    }

    protected override void PassUniformsAndBindTextures(
        GlShaderProgram shaderProgram) {
      for (var t = 0; t < MaterialConstants.MAX_TEXTURES; ++t) {
        GL.Uniform1(textureLocations_[t], t);
      }

      for (var i = 0; i < this.textures_.Count; ++i) {
        this.textures_[i].Bind(i);
      }
    }
  }
}