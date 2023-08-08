using fin.model;
using OpenTK.Graphics.OpenGL;

using FinLogicOp = fin.model.LogicOp;
using GlLogicOp = OpenTK.Graphics.OpenGL.LogicOp;


namespace fin.ui.graphics.gl {
  public partial class GlState {
    public (BlendMode, BlendFactor, BlendFactor, FinLogicOp)
        CurrentBlending { get; set; } = (BlendMode.ADD, BlendFactor.SRC_ALPHA,
                                         BlendFactor.ONE_MINUS_SRC_ALPHA,
                                         FinLogicOp.UNDEFINED);
  }

  public static partial class GlUtil {
    public static void ResetBlending() => SetBlending(BlendMode.ADD,
      BlendFactor.SRC_ALPHA,
      BlendFactor.ONE_MINUS_SRC_ALPHA,
      FinLogicOp.UNDEFINED);


    public static bool SetBlending(
        BlendMode blendMode,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        FinLogicOp logicOp) {
      if (GlUtil.currentState_.CurrentBlending ==
          (blendMode, srcFactor, dstFactor, logicOp)) {
        return false;
      }

      GlUtil.currentState_.CurrentBlending =
          (blendMode, srcFactor, dstFactor, logicOp);

      if (blendMode is BlendMode.NONE) {
        GL.Disable(EnableCap.Blend);
        GL.BlendEquation(BlendEquationMode.FuncAdd);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      } else {
        GL.Enable(EnableCap.Blend);
        GL.BlendEquation(GlUtil.ConvertFinBlendModeToGl_(blendMode));
        GL.BlendFunc(GlUtil.ConvertFinBlendFactorToGl_(srcFactor),
                     GlUtil.ConvertFinBlendFactorToGl_(dstFactor));
      }

      if (logicOp == FinLogicOp.UNDEFINED) {
        GL.Disable(EnableCap.ColorLogicOp);
      } else {
        GL.Enable(EnableCap.ColorLogicOp);
        GL.LogicOp(GlUtil.ConvertFinLogicOpToGl_(logicOp));
      }

      return true;
    }

    private static BlendEquationMode ConvertFinBlendModeToGl_(
        BlendMode finBlendMode)
      => finBlendMode switch {
          BlendMode.ADD              => BlendEquationMode.FuncAdd,
          BlendMode.SUBTRACT         => BlendEquationMode.FuncSubtract,
          BlendMode.REVERSE_SUBTRACT => BlendEquationMode.FuncReverseSubtract,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finBlendMode), finBlendMode, null)
      };

    private static BlendingFactor ConvertFinBlendFactorToGl_(
        BlendFactor finBlendFactor)
      => finBlendFactor switch {
          BlendFactor.ZERO                => BlendingFactor.Zero,
          BlendFactor.ONE                 => BlendingFactor.One,
          BlendFactor.SRC_COLOR           => BlendingFactor.SrcColor,
          BlendFactor.ONE_MINUS_SRC_COLOR => BlendingFactor.OneMinusSrcColor,
          BlendFactor.SRC_ALPHA           => BlendingFactor.SrcAlpha,
          BlendFactor.ONE_MINUS_SRC_ALPHA => BlendingFactor.OneMinusSrcAlpha,
          BlendFactor.DST_ALPHA           => BlendingFactor.DstAlpha,
          BlendFactor.ONE_MINUS_DST_ALPHA => BlendingFactor.OneMinusDstAlpha,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finBlendFactor), finBlendFactor, null)
      };

    private static GlLogicOp ConvertFinLogicOpToGl_(FinLogicOp finLogicOp)
      => finLogicOp switch {
          FinLogicOp.CLEAR         => GlLogicOp.Clear,
          FinLogicOp.AND           => GlLogicOp.And,
          FinLogicOp.AND_REVERSE   => GlLogicOp.AndReverse,
          FinLogicOp.COPY          => GlLogicOp.Copy,
          FinLogicOp.AND_INVERTED  => GlLogicOp.AndInverted,
          FinLogicOp.NOOP          => GlLogicOp.Noop,
          FinLogicOp.XOR           => GlLogicOp.Xor,
          FinLogicOp.OR            => GlLogicOp.Or,
          FinLogicOp.NOR           => GlLogicOp.Nor,
          FinLogicOp.EQUIV         => GlLogicOp.Equiv,
          FinLogicOp.INVERT        => GlLogicOp.Invert,
          FinLogicOp.OR_REVERSE    => GlLogicOp.OrReverse,
          FinLogicOp.COPY_INVERTED => GlLogicOp.CopyInverted,
          FinLogicOp.OR_INVERTED   => GlLogicOp.OrInverted,
          FinLogicOp.NAND          => GlLogicOp.Nand,
          FinLogicOp.SET           => GlLogicOp.Set,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finLogicOp), finLogicOp, null)
      };
  }
}