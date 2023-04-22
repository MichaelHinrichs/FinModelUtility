using System;

using UoT.util.array;

namespace UoT.displaylist {
  public class DlShaderParams {
    public DlShaderParams() { }

    private DlShaderParams(UnpackedCombiner combArg) => this.CombArg = combArg;

    public DlShaderParams Clone() {
      var newParams = new DlShaderParams(this.CombArg.Clone()) {
          PrimColorLOD = this.PrimColorLOD,
          PrimColorM = this.PrimColorM,
          PrecompiledCombiner = this.PrecompiledCombiner,
          MultiTexture = this.MultiTexture,
          MultiTexCoord = this.MultiTexCoord,
          EnableCombiner = this.EnableCombiner,
          EnableLighting = this.EnableLighting,
          EnableSphericalUv = this.EnableSphericalUv,
          EnableLinearUv = this.EnableLinearUv,
      };

      ArrayUtil.CopyTo(this.PrimColor, newParams.PrimColor);
      ArrayUtil.CopyTo(this.EnvironmentColor, newParams.EnvironmentColor);
      ArrayUtil.CopyTo(this.BlendColor, newParams.BlendColor);
      ArrayUtil.CopyTo(this.FogColor, newParams.FogColor);

      return newParams;
    }

    public float[] PrimColor { get; } = new float[4];
    public float[] EnvironmentColor { get; } = new float[4];
    public float[] BlendColor { get; } = new float[4];
    public float[] FogColor { get; } = new float[4];

    public float PrimColorLOD { get; set; }
    public float PrimColorM { get; set; }

    public UnpackedCombiner CombArg { get; } = new UnpackedCombiner();
    public bool PrecompiledCombiner { get; set; }

    public bool MultiTexture { get; set; }
    public bool MultiTexCoord { get; set; }
    public bool EnableCombiner { get; set; }
    public bool EnableLighting { get; set; }
    public bool EnableSphericalUv { get; set; }
    public bool EnableLinearUv { get; set; }
  }
}
