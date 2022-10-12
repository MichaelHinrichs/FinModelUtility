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

      GL.Enable(EnableCap.ColorLogicOp);
      GL.LogicOp(GlUtil.ConvertFinLogicOpToGl_(logicOp));
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

      GL.MultMatrix(matrix);
    }

    public static void Ortho2d(int left, int right, int bottom, int top)
      => GL.Ortho(left, right, bottom, top, -1, 1);

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

      GL.MultMatrix(matrix);
      GL.Translate(-eyeX, -eyeY, -eyeZ);
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