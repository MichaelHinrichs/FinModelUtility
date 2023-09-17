using cmb.schema.cmb.mats;

using fin.language.equations.fixedFunction;
using fin.language.equations.fixedFunction.impl;
using fin.model;

namespace cmb.material {
  /// <summary>
  ///   Shamelessly copied from https://github.com/naclomi/noclip.website/blob/8b0de601d6d8f596683f0bdee61a9681a42512f9/src/oot3d/render.ts
  /// </summary>
  public class CmbCombinerGenerator {
    private readonly IFixedFunctionEquations<FixedFunctionSource> equations_;
    private readonly ColorFixedFunctionOps cOps_;
    private readonly ScalarFixedFunctionOps sOps_;

    public CmbCombinerGenerator(IFixedFunctionMaterial material) {
      this.equations_ = material.Equations;
      this.cOps_ = new ColorFixedFunctionOps(this.equations_);
      this.sOps_ = new ScalarFixedFunctionOps(this.equations_);
    }

    /*private LazyDictionary<(TexCombinerSource src, TexCombinerColorOp op),
            IColorValue>
        colorInGenerators_ = new(srcAndOp => {
          var (src, op) = srcAndOp;

          var color = src switch {
              TexCombinerSource.PrimaryColor           => expr,
              TexCombinerSource.FragmentPrimaryColor   => expr,
              TexCombinerSource.FragmentSecondaryColor => expr,
              TexCombinerSource.Texture0               => expr,
              TexCombinerSource.Texture1               => expr,
              TexCombinerSource.Texture2               => expr,
              TexCombinerSource.Texture3               => expr,
              TexCombinerSource.PreviousBuffer         => expr,
              TexCombinerSource.Constant               => expr,
              TexCombinerSource.Previous               => expr,
              _                                        => throw new ArgumentOutOfRangeException()
          };

          return op switch {
              TexCombinerColorOp.Color         => color,
              TexCombinerColorOp.OneMinusColor => expr,
              TexCombinerColorOp.Alpha         => expr,
              TexCombinerColorOp.OneMinusAlpha => expr,
              TexCombinerColorOp.Red           => expr,
              TexCombinerColorOp.OneMinusRed   => expr,
              TexCombinerColorOp.Green         => expr,
              TexCombinerColorOp.OneMinusGreen => expr,
              TexCombinerColorOp.Blue          => expr,
              TexCombinerColorOp.OneMinusBlue  => expr,
              _                                => throw new ArgumentOutOfRangeException()
          }
        });*/

    public void AddCombiner(
        Material cmbMaterial,
        Combiner cmbCombiner) {
    }
  }
}