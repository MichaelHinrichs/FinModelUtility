using fin.language.equations.fixedFunction;
using fin.math;
using fin.model;

using OpenTK.Graphics.OpenGL;

namespace fin.ui.rendering.gl.material {
  public class GlFixedFunctionMaterialShader
      : BGlMaterialShader<IReadOnlyFixedFunctionMaterial> {
    private readonly IFixedFunctionRegisters registers_;

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

    protected override void DisposeInternal() { }

    protected override void Setup(
        IReadOnlyFixedFunctionMaterial material,
        GlShaderProgram impl) {
      var finTextures = material.TextureSources;

      var equations = material.Equations;
      for (var i = 0; i < MaterialConstants.MAX_TEXTURES; ++i) {
        if (!equations.DoOutputsDependOn(
                new[] {
                    FixedFunctionSource.TEXTURE_COLOR_0 + i,
                    FixedFunctionSource.TEXTURE_ALPHA_0 + i
                })) {
          continue;
        }

        var finTexture = i < (finTextures?.Count ?? 0)
            ? finTextures[i]
            : null;
        var glTexture = finTexture != null
            ? GlTexture.FromTexture(finTexture)
            : GlMaterialConstants.NULL_WHITE_TEXTURE;

        this.SetUpTexture($"texture{i}", i, finTexture, glTexture);
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
      foreach (var colorRegister in this.registers_.ColorRegisters) {
        var location = this.colorRegisterLocations_[colorRegister];
        if (location == -1) {
          continue;
        }

        var value = colorRegister.Value;
        GL.Uniform3(location, value.Rf, value.Gf, value.Bf);
      }

      foreach (var scalarRegister in this.registers_.ScalarRegisters) {
        var location = this.scalarRegisterLocations_[scalarRegister];
        if (location == -1) {
          continue;
        }

        GL.Uniform1(location, scalarRegister.Value);
      }
    }
  }
}