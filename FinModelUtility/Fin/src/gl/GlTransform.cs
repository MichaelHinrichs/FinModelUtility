using fin.math;
using fin.math.matrix;
using System;
using OpenTK.Graphics.OpenGL;


namespace fin.gl {
  public static class GlTransform {
    public static void MatrixMode(MatrixMode mode) {
      GL.MatrixMode(mode);
    }

    public static void LoadIdentity() {
      GL.LoadIdentity();
    }

    public static void MultMatrix(float[] matrix) {
      GL.MultMatrix(matrix);
    }

    public static void MultMatrix(double[] matrix) {
      GL.MultMatrix(matrix);
    }

    public static void Translate(float x, float y, float z) {
      GL.Translate(x, y, z);
    }

    public static void Translate(double x, double y, double z) {
      GL.Translate(x, y, z);
    }

    public static void Scale(float x, float y, float z) {
      GL.Scale(x, y, z);
    }

    public static void Scale(double x, double y, double z) {
      GL.Scale(x, y, z);
    }

    public static void Rotate(float angle, float x, float y, float z) {
      GL.Rotate(angle, x, y, z);
    }

    public static void Rotate(double angle, double x, double y, double z) {
      GL.Rotate(angle, x, y, z);
    }


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

      MultMatrix(matrix);
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

      MultMatrix(matrix);
      Translate(-eyeX, -eyeY, -eyeZ);
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

    public static unsafe void MultMatrix(IReadOnlyFinMatrix4x4 matrix) {
      if (matrix is FinMatrix4x4 matrixImpl) {
        fixed (float* ptr = &matrixImpl.impl_.M11) {
          GL.MultMatrix(ptr);
        }
        return;
      }

      float[] buffer = new float[16];
      for (var y = 0; y < 4; ++y) {
        for (var x = 0; x < 4; ++x) {
          buffer[y * 4 + x] = matrix[x, y];
        }
      }
      MultMatrix(buffer);
    }
  }
}