using System.Collections.Generic;

using fin.language.equations.fixedFunction;
using fin.model;

using OpenTK.Graphics.OpenGL;


namespace fin.gl.material {
  public class GlFixedFunctionMaterialShaderSource : IGlMaterialShaderSource {
    public GlFixedFunctionMaterialShaderSource(
        IFixedFunctionMaterial material)
      => this.FragmentShaderSource =
          new FixedFunctionEquationsGlslPrinter(material.TextureSources)
              .Print(material);

    public string VertexShaderSource => CommonShaderPrograms.VERTEX_SRC;
    public string FragmentShaderSource { get; }
  }

  public class GlFixedFunctionMaterialShaderV2
      : BGlMaterialShader<IReadOnlyFixedFunctionMaterial> {
    private int[] textureLocations_ =
        new int[MaterialConstants.MAX_TEXTURES];

    private IList<GlTexture> textures_;

    public GlFixedFunctionMaterialShaderV2(
        IReadOnlyFixedFunctionMaterial fixedFunctionMaterial,
        ILighting? lighting)
        : base(fixedFunctionMaterial, lighting) { }

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