using System;

using fin.model;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform.Windows;


namespace fin.gl {
  public static class GlUtil {
    public static bool IsInitialized { get; private set; }

    public static void Init() {
      if (IsInitialized) {
        return;
      }

      IsInitialized = true;

      Glut.glutInit();
      Glut.glutInitDisplayMode(Glut.GLUT_SINGLE | Glut.GLUT_RGB);

      Wgl.ReloadFunctions();
      Gl.ReloadFunctions();
    }

    public static void SetCulling(CullingMode cullingMode) {
      if (cullingMode == CullingMode.SHOW_BOTH) {
        Gl.glDisable(Gl.GL_CULL_FACE);
        return;
      }

      Gl.glEnable(Gl.GL_CULL_FACE);
      Gl.glCullFace(cullingMode switch {
          CullingMode.SHOW_FRONT_ONLY => Gl.GL_BACK,
          CullingMode.SHOW_BACK_ONLY  => Gl.GL_FRONT,
          CullingMode.SHOW_NEITHER    => Gl.GL_FRONT_AND_BACK,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(cullingMode), cullingMode, null)
      });
    }

    public static void ResetBlending() {
      Gl.glDisable(Gl.GL_BLEND);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      Gl.glDisable(Gl.GL_COLOR_LOGIC_OP);
    }

    public static void SetBlending(
        BlendMode blendMode,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        LogicOp logicOp) {
      if (blendMode is BlendMode.NONE) {
        Gl.glDisable(Gl.GL_BLEND);
      } else {
        Gl.glEnable(Gl.GL_BLEND);
        Gl.glBlendEquation(GlUtil.ConvertFinBlendModeToGl_(blendMode));
        Gl.glBlendFunc(GlUtil.ConvertFinBlendFactorToGl_(srcFactor),
                       GlUtil.ConvertFinBlendFactorToGl_(dstFactor));
      }

      // TODO: Doesn't seem to work??
      /*Gl.glEnable(Gl.GL_COLOR_LOGIC_OP);
      Gl.glLogicOp(GlUtil.ConvertFinLogicOpToGl_(logicOp));*/
    }

    private static int ConvertFinBlendModeToGl_(BlendMode finBlendMode)
      => finBlendMode switch {
          BlendMode.ADD      => Gl.GL_FUNC_ADD,
          BlendMode.SUBTRACT => Gl.GL_FUNC_SUBTRACT,
          BlendMode.REVERSE_SUBTRACT => Gl.GL_FUNC_REVERSE_SUBTRACT,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finBlendMode), finBlendMode, null)
      };

    private static int ConvertFinBlendFactorToGl_(
        BlendFactor finBlendFactor)
      => finBlendFactor switch {
          BlendFactor.ZERO                => Gl.GL_ZERO,
          BlendFactor.ONE                 => Gl.GL_ONE,
          BlendFactor.SRC_COLOR           => Gl.GL_SRC_COLOR,
          BlendFactor.ONE_MINUS_SRC_COLOR => Gl.GL_ONE_MINUS_SRC_COLOR,
          BlendFactor.SRC_ALPHA           => Gl.GL_SRC_ALPHA,
          BlendFactor.ONE_MINUS_SRC_ALPHA => Gl.GL_ONE_MINUS_SRC_ALPHA,
          BlendFactor.DST_ALPHA           => Gl.GL_DST_ALPHA,
          BlendFactor.ONE_MINUS_DST_ALPHA => Gl.GL_ONE_MINUS_DST_ALPHA,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finBlendFactor), finBlendFactor, null)
      };

    private static int ConvertFinLogicOpToGl_(LogicOp finLogicOp)
      => finLogicOp switch {
          LogicOp.CLEAR         => Gl.GL_CLEAR,
          LogicOp.AND           => Gl.GL_AND,
          LogicOp.AND_REVERSE   => Gl.GL_AND_REVERSE,
          LogicOp.COPY          => Gl.GL_COPY,
          LogicOp.AND_INVERTED  => Gl.GL_AND_INVERTED,
          LogicOp.NOOP          => Gl.GL_NOOP,
          LogicOp.XOR           => Gl.GL_XOR,
          LogicOp.OR            => Gl.GL_OR,
          LogicOp.NOR           => Gl.GL_NOR,
          LogicOp.EQUIV         => Gl.GL_EQUIV,
          LogicOp.INVERT        => Gl.GL_INVERT,
          LogicOp.OR_REVERSE    => Gl.GL_OR_REVERSE,
          LogicOp.COPY_INVERTED => Gl.GL_COPY_INVERTED,
          LogicOp.OR_INVERTED   => Gl.GL_OR_INVERTED,
          LogicOp.NAND          => Gl.GL_NAND,
          LogicOp.SET           => Gl.GL_SET,
          _ => throw new ArgumentOutOfRangeException(
                   nameof(finLogicOp), finLogicOp, null)
      };
  }
}