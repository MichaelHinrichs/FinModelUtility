using System;
using fin.model;
using OpenTK.Graphics.OpenGL;

using FinLogicOp = fin.model.LogicOp;
using GlLogicOp = OpenTK.Graphics.OpenGL.LogicOp;


namespace fin.gl {
  public static class GlUtil {
    public static bool IsInitialized { get; private set; }

    public static void Init() {
      if (IsInitialized) {
        return;
      }

      // (Set up DLL here, if ever needed again.)

      IsInitialized = true;
    }

    private static readonly object GL_LOCK_ = new();

    public static void RunLockedGl(Action handler) {
      lock (GL_LOCK_) {
        handler();
      }
    }

    public static void SetCulling(CullingMode cullingMode) {
      if (cullingMode == CullingMode.SHOW_BOTH) {
        GL.Disable(EnableCap.CullFace);
        return;
      }

      GL.Enable(EnableCap.CullFace);
      GL.CullFace(cullingMode switch {
          CullingMode.SHOW_FRONT_ONLY => CullFaceMode.Back,
          CullingMode.SHOW_BACK_ONLY  => CullFaceMode.Front,
          CullingMode.SHOW_NEITHER    => CullFaceMode.FrontAndBack,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(cullingMode), cullingMode, null)
      });
    }

    public static void ResetDepth()
      => SetDepth(DepthMode.USE_DEPTH_BUFFER, DepthCompareType.LEqual);

    public static void SetDepth(
        DepthMode depthMode,
        DepthCompareType depthCompareType) {
      switch (depthMode) {
        case DepthMode.USE_DEPTH_BUFFER: {
          GL.DepthFunc(ConvertFinDepthCompareTypeToGl_(depthCompareType));
          GL.Enable(EnableCap.DepthTest);
          GL.DepthMask(true);
          break;
        }
        case DepthMode.IGNORE_DEPTH_BUFFER: {
          GL.Disable(EnableCap.DepthTest);
          GL.DepthMask(false);
          break;
        }
        case DepthMode.SKIP_WRITE_TO_DEPTH_BUFFER: {
          GL.DepthFunc(ConvertFinDepthCompareTypeToGl_(depthCompareType));
          GL.Enable(EnableCap.DepthTest);
          GL.DepthMask(false);
          break;
        }
        default:
          throw new ArgumentOutOfRangeException(nameof(depthMode), depthMode,
                                                null);
      }
    }

    public static void ResetBlending() {
      GL.Disable(EnableCap.Blend);
      GL.BlendEquation(BlendEquationMode.FuncAdd);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      GL.Disable(EnableCap.ColorLogicOp);
    }

    public static void SetBlending(
        BlendMode blendMode,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        FinLogicOp logicOp) {
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
    }

    private static DepthFunction ConvertFinDepthCompareTypeToGl_(
        DepthCompareType finDepthCompareType)
      => finDepthCompareType switch {
          DepthCompareType.LEqual  => DepthFunction.Lequal,
          DepthCompareType.Less    => DepthFunction.Less,
          DepthCompareType.Equal   => DepthFunction.Equal,
          DepthCompareType.Greater => DepthFunction.Greater,
          DepthCompareType.NEqual  => DepthFunction.Notequal,
          DepthCompareType.GEqual  => DepthFunction.Gequal,
          DepthCompareType.Always  => DepthFunction.Always,
          DepthCompareType.Never   => DepthFunction.Never,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finDepthCompareType), finDepthCompareType, null)
      };

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