using System;

using fin.math;
using fin.math.matrix;
using fin.model;
using fin.util.asserts;

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
      Gl.glBlendEquation(Gl.GL_FUNC_ADD);
      Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      Gl.glDisable(Gl.GL_LOGIC_OP);
    }

    public static void SetBlending(
        BlendMode blendMode,
        BlendFactor srcFactor,
        BlendFactor dstFactor,
        LogicOp logicOp) {
      if (blendMode is BlendMode.NONE) {
        Gl.glDisable(Gl.GL_BLEND);
        Gl.glBlendEquation(Gl.GL_FUNC_ADD);
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
      } else {
        Gl.glEnable(Gl.GL_BLEND);
        Gl.glBlendEquation(GlUtil.ConvertFinBlendModeToGl_(blendMode));
        Gl.glBlendFunc(GlUtil.ConvertFinBlendFactorToGl_(srcFactor),
                       GlUtil.ConvertFinBlendFactorToGl_(dstFactor));
      }

      Gl.glEnable(Gl.GL_LOGIC_OP);
      Gl.glLogicOp(GlUtil.ConvertFinLogicOpToGl_(logicOp));
    }

    private static int ConvertFinBlendModeToGl_(BlendMode finBlendMode)
      => finBlendMode switch {
          BlendMode.ADD              => Gl.GL_FUNC_ADD,
          BlendMode.SUBTRACT         => Gl.GL_FUNC_SUBTRACT,
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

    public static void Perspective(double fovYDegrees,
                                   double aspectRatio,
                                   double zNear,
                                   double zFar) {
      var matrix = new double[16];

      var f = 1.0 / Math.Tan(fovYDegrees / 180 * Math.PI / 2);

      SetInMatrix(matrix, 0, 0, f / aspectRatio);
      SetInMatrix(matrix, 1, 1, f);
      SetInMatrix(matrix, 2, 2, (zNear + zFar) / (zNear - zFar));
      SetInMatrix(matrix, 3, 2, 2 * zNear * zFar / (zNear - zFar));
      SetInMatrix(matrix, 2, 3, -1);

      Gl.glMultMatrixd(matrix);
    }

    public static void Ortho2d(int left, int right, int bottom, int top)
      => Gl.glOrtho(left, right, bottom, top, -1, 1);

    public static void LookAt(
        double eyeX,
        double eyeY,
        double eyeZ,
        double centerX,
        double centerY,
        double centerZ,
        double upX,
        double upY,
        double upZ) {
      var lookX = centerX - eyeX;
      var lookY = centerY - eyeY;
      var lookZ = centerZ - eyeZ;
      Normalize3(ref lookX, ref lookY, ref lookZ);

      CrossProduct3(
          lookX, lookY, lookZ,
          upX, upY, upZ,
          out var sideX, out var sideY, out var sideZ);
      Normalize3(ref sideX, ref sideY, ref sideZ);

      CrossProduct3(
          sideX, sideY, sideZ,
          lookX, lookY, lookZ,
          out upX, out upY, out upZ);

      var matrix = new double[16];

      SetInMatrix(matrix, 0, 0, sideX);
      SetInMatrix(matrix, 1, 0, sideY);
      SetInMatrix(matrix, 2, 0, sideZ);

      SetInMatrix(matrix, 0, 1, upX);
      SetInMatrix(matrix, 1, 1, upY);
      SetInMatrix(matrix, 2, 1, upZ);

      SetInMatrix(matrix, 0, 2, -lookX);
      SetInMatrix(matrix, 1, 2, -lookY);
      SetInMatrix(matrix, 2, 2, -lookZ);

      SetInMatrix(matrix, 3, 3, 1);

      Gl.glMultMatrixd(matrix);
      Gl.glTranslated(-eyeX, -eyeY, -eyeZ);
    }

    public static int ConvertMatrixCoordToIndex(int r, int c) => 4 * r + c;

    public static void SetInMatrix(double[] matrix, int r, int c, double value)
      => matrix[ConvertMatrixCoordToIndex(r, c)] = value;

    public static void CrossProduct3(
        double x1,
        double y1,
        double z1,
        double x2,
        double y2,
        double z2,
        out double outX,
        out double outY,
        out double outZ) {
      outX = y1 * z2 - z1 * y2;
      outY = z1 * x2 - x1 * z2;
      outZ = x1 * y2 - y1 * x2;
    }

    public static void Normalize3(ref double x, ref double y, ref double z) {
      var length = Math.Sqrt(x * x + y * y + z * z);
      x /= length;
      y /= length;
      z /= length;
    }
  }
}