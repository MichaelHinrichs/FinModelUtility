using System;
using System.Runtime.CompilerServices;

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

    private static CullingMode currentCullingMode_ = CullingMode.SHOW_FRONT_ONLY;

    public static bool SetCulling(CullingMode cullingMode) {
      if (GlUtil.currentCullingMode_ == cullingMode) {
        return false;
      }

      GlUtil.currentCullingMode_ = cullingMode;

      if (cullingMode == CullingMode.SHOW_BOTH) {
        GL.Disable(EnableCap.CullFace);
      } else {
        GL.Enable(EnableCap.CullFace);
        GL.CullFace(cullingMode switch {
            CullingMode.SHOW_FRONT_ONLY => CullFaceMode.Back,
            CullingMode.SHOW_BACK_ONLY  => CullFaceMode.Front,
            CullingMode.SHOW_NEITHER    => CullFaceMode.FrontAndBack,
            _ => throw new ArgumentOutOfRangeException(
                nameof(cullingMode), cullingMode, null)
        });
      }

      return true;
    }

    public static void ResetDepth()
      => SetDepth(DepthMode.USE_DEPTH_BUFFER, DepthCompareType.LEqual);

    private static (DepthMode, DepthCompareType) depthModeAndCompareType =
        (DepthMode.USE_DEPTH_BUFFER, DepthCompareType.LEqual);

    public static bool SetDepth(
        DepthMode depthMode,
        DepthCompareType depthCompareType) {
      if (GlUtil.depthModeAndCompareType == (depthMode, depthCompareType)) {
        return false;
      }

      GlUtil.depthModeAndCompareType = (depthMode, depthCompareType);

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

      return true;
    }

    public static void ResetBlending() {
      GL.Disable(EnableCap.Blend);
      GL.BlendEquation(BlendEquationMode.FuncAdd);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
      GL.Disable(EnableCap.ColorLogicOp);
    }

    private static (BlendMode, BlendFactor, BlendFactor, FinLogicOp)
        currentBlending_ = (BlendMode.ADD, BlendFactor.SRC_ALPHA,
                            BlendFactor.ONE_MINUS_SRC_ALPHA,
                            FinLogicOp.UNDEFINED);

    public static bool SetBlending(
        BlendMode blendMode,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        FinLogicOp logicOp) {
      if (GlUtil.currentBlending_ ==
          (blendMode, srcFactor, dstFactor, logicOp)) {
        return false;
      }

      GlUtil.currentBlending_ = (blendMode, srcFactor, dstFactor, logicOp);

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


    private static int activeTexture_ = -1;

    private static int[] currentTextureBindings_ =
        new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindTexture(int textureIndex, int value) {
      if (GlUtil.currentTextureBindings_[textureIndex] == value) {
        return;
      }

      if (GlUtil.activeTexture_ != textureIndex) {
        GlUtil.activeTexture_ = textureIndex;
        GL.ActiveTexture(TextureUnit.Texture0 + textureIndex);
      }

      GlUtil.currentTextureBindings_[textureIndex] = value;
      GL.BindTexture(TextureTarget.Texture2D, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UnbindTexture(int textureIndex)
      => BindTexture(textureIndex, -1);
  }
}