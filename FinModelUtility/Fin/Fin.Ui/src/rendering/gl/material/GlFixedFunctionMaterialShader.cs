using fin.language.equations.fixedFunction;
using fin.math;
using fin.model;

using Microsoft.Win32;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class GlFixedFunctionMaterialShader
      : BGlMaterialShader<IReadOnlyFixedFunctionMaterial> {
    private readonly IFixedFunctionRegisters registers_;

    private readonly int[] textureLocations_ =
        new int[MaterialConstants.MAX_TEXTURES];

    private IList<GlTexture> textures_;

    private readonly Dictionary<IColorRegister, int> colorRegisterLocations_ =
        new();

    private readonly Dictionary<IScalarRegister, int> scalarRegisterLocations_ =
        new();

    public GlFixedFunctionMaterialShader(
        IModel model,
        IReadOnlyFixedFunctionMaterial fixedFunctionMaterial,
        IBoneTransformManager? boneTransformManager,
        ILighting? lighting)
        : base(model, fixedFunctionMaterial, boneTransformManager, lighting) {
      this.registers_ = fixedFunctionMaterial.Registers;
    }

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

      var registers = material.Registers;
      foreach (var colorRegister in registers.ColorRegisters) {
        this.colorRegisterLocations_[colorRegister] =
            impl.GetUniformLocation($"color_{colorRegister.Name}");
      }

      foreach (var scalarRegister in registers.ScalarRegisters) {
        this.scalarRegisterLocations_[scalarRegister] =
            impl.GetUniformLocation($"scalar_{scalarRegister.Name}");
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

      foreach (var colorRegister in this.registers_.ColorRegisters) {
        var value = colorRegister.Value;
        GL.Uniform3(this.colorRegisterLocations_[colorRegister],
                    value.Rf,
                    value.Gf,
                    value.Bf);
      }

      foreach (var scalarRegister in this.registers_.ScalarRegisters) {
        GL.Uniform1(this.scalarRegisterLocations_[scalarRegister],
                    scalarRegister.Value);
      }
    }
  }
}