using System;
using System.Collections.Generic;
using System.Numerics;

using MathNet.Numerics.LinearAlgebra;

namespace fin.math {
  public class SoftwareModelViewMatrixTransformer {
    private Matrix<double> current_;
    private readonly LinkedList<MatrixNode> stack_ = new();

    private class MatrixNode {
      public Matrix<double>? Matrix { get; init; }
    }

    public SoftwareModelViewMatrixTransformer() {
      this.Push();

      // TODO: Assigned in push, possible to fix this?
      this.current_ = this.current_;
    }

    public void ProjectVertex(
        ref double x,
        ref double y,
        ref double z,
        bool correctPerspective = true)
      => GlMatrixUtil.Project(this.current_,
                              ref x,
                              ref y,
                              ref z,
                              1,
                              correctPerspective,
                              false);

    public void ProjectNormal(
        ref double normalX,
        ref double normalY,
        ref double normalZ,
        bool normalize = true)
      => GlMatrixUtil.Project(this.current_,
                              ref normalX,
                              ref normalY,
                              ref normalZ,
                              0,
                              false,
                              normalize);

    public SoftwareModelViewMatrixTransformer Push() {
      Matrix<double> newMatrix;
      if (this.current_ == null) {
        newMatrix = Matrix<double>.Build.DenseIdentity(4, 4);
      } else {
        newMatrix = this.current_.Clone();
      }

      this.stack_.AddLast(new MatrixNode {
          Matrix = newMatrix
      });
      this.UpdateCurrent_();

      return this;
    }

    public SoftwareModelViewMatrixTransformer Pop() {
      if (this.stack_.Count <= 1) {
        throw new Exception("Popped too far.");
      }

      this.stack_.RemoveLast();
      this.UpdateCurrent_();

      return this;
    }

    private void UpdateCurrent_()
      => this.current_ = this.stack_.Last.Value.Matrix!;

    private readonly Matrix<double> rhsBuffer_ =
        Matrix<double>.Build.DenseIdentity(4, 4);

    private readonly Matrix<double> resultBuffer_ =
        Matrix<double>.Build.DenseIdentity(4, 4);

    public SoftwareModelViewMatrixTransformer Identity() {
      this.current_.Clear();
      for (var i = 0; i < 4; ++i) {
        this.current_[i, i] = 1;
      }

      return this;
    }


    public SoftwareModelViewMatrixTransformer Translate(
        double x,
        double y,
        double z) {
      this.rhsBuffer_.Clear();
      for (var i = 0; i < 4; ++i) {
        this.rhsBuffer_[i, i] = 1;
      }
      this.rhsBuffer_[0, 3] = x;
      this.rhsBuffer_[1, 3] = y;
      this.rhsBuffer_[2, 3] = z;

      return this.MultMatrix(this.rhsBuffer_);
    }

    public SoftwareModelViewMatrixTransformer RotateAroundAxis(
        double angle,
        double x,
        double y,
        double z) {
      // From https://www.csee.umbc.edu/portal/help/C++/opengl/man_pages/html/gl/rotate.html
      var len = Math.Sqrt(x * x + y * y + z * z);
      x /= len;
      y /= len;
      z /= len;

      var rads = angle / 180 * Math.PI;
      var c = Math.Cos(rads);
      var s = Math.Sin(rads);

      this.rhsBuffer_.Clear();
      this.rhsBuffer_[0, 0] = x * x * (1 - c) + c;
      this.rhsBuffer_[0, 1] = x * y * (1 - c) - z * s;
      this.rhsBuffer_[0, 2] = x * z * (1 - c) + y * s;

      this.rhsBuffer_[1, 0] = y * x * (1 - c) + z * s;
      this.rhsBuffer_[1, 1] = y * y * (1 - c) + c;
      this.rhsBuffer_[1, 2] = y * z * (1 - c) - x * s;

      this.rhsBuffer_[2, 0] = x * z * (1 - c) - y * s;
      this.rhsBuffer_[2, 1] = y * z * (1 - c) + x * s;
      this.rhsBuffer_[2, 2] = z * z * (1 - c) + c;

      this.rhsBuffer_[3, 3] = 1;

      return this.MultMatrix(this.rhsBuffer_);
    }

    public SoftwareModelViewMatrixTransformer Rotate(Quaternion q)
      => this.Rotate(q.X, q.Y, q.Z, q.W);

    public SoftwareModelViewMatrixTransformer Rotate(
        float qx,
        float qy,
        float qz,
        float qw) {
      this.rhsBuffer_.Clear();
      this.rhsBuffer_[0, 0] = 1.0 - 2.0 * qy * qy - 2.0 * qz * qz;
      this.rhsBuffer_[0, 1] = 2.0 * qx * qy - 2.0 * qz * qw;
      this.rhsBuffer_[0, 2] = 2.0 * qx * qz + 2.0 * qy * qw;
      this.rhsBuffer_[0, 3] = 0.0;

      this.rhsBuffer_[1, 0] = 2.0 * qx * qy + 2.0 * qz * qw;
      this.rhsBuffer_[1, 1] = 1.0 - 2.0 * qx * qx - 2.0 * qz * qz;
      this.rhsBuffer_[1, 2] = 2.0 * qy * qz - 2.0 * qx * qw;
      this.rhsBuffer_[1, 3] = 0.0;

      this.rhsBuffer_[2, 0] = 2.0 * qx * qz - 2.0 * qy * qw;
      this.rhsBuffer_[2, 1] = 2.0 * qy * qz + 2.0 * qx * qw;
      this.rhsBuffer_[2, 2] = 1.0 - 2.0 * qx * qx - 2.0 * qy * qy;
      this.rhsBuffer_[2, 3] = 0.0;

      this.rhsBuffer_[3, 0] = 0;
      this.rhsBuffer_[3, 1] = 0;
      this.rhsBuffer_[3, 2] = 0;
      this.rhsBuffer_[3, 3] = 1;

      return this.MultMatrix(this.rhsBuffer_);
    }

    public SoftwareModelViewMatrixTransformer Scale(
        double x,
        double y,
        double z) {
      this.rhsBuffer_.Clear();
      this.rhsBuffer_[0, 0] = x;
      this.rhsBuffer_[1, 1] = y;
      this.rhsBuffer_[2, 2] = z;
      this.rhsBuffer_[3, 3] = 1;

      return this.MultMatrix(this.rhsBuffer_);
    }

    public SoftwareModelViewMatrixTransformer MultMatrix(Matrix<double> m) {
      this.current_.Multiply(m, this.resultBuffer_);
      this.resultBuffer_.CopyTo(this.current_);

      return this;
    }

    public void Get(Matrix<double> m) {
      this.current_.CopyTo(m);
    }

    public void Set(Matrix<double> m) {
      m.CopyTo(this.current_);
    }

    public void Set(Matrix4x4 m) {
      /*this.current_[0, 0] = m.M11;
      this.current_[0, 1] = m.M12;
      this.current_[0, 2] = m.M13;
      this.current_[0, 3] = m.M14;

      this.current_[1, 0] = m.M21;
      this.current_[1, 1] = m.M22;
      this.current_[1, 2] = m.M23;
      this.current_[1, 3] = m.M24;

      this.current_[2, 0] = m.M31;
      this.current_[2, 1] = m.M32;
      this.current_[2, 2] = m.M33;
      this.current_[2, 3] = m.M34;

      this.current_[3, 0] = m.M41;
      this.current_[3, 1] = m.M42;
      this.current_[3, 2] = m.M43;
      this.current_[3, 3] = m.M44;*/
    }
  }

  // TODO: Move this somewhere else.
  public class GlMatrixUtil {
    public static Matrix<double> CsToMn(Matrix4x4 cs) {
      var mn = Matrix<double>.Build.Dense(4, 4);

      mn[0, 0] = cs.M11;
      mn[1, 0] = cs.M12;
      mn[2, 0] = cs.M13;
      mn[3, 0] = cs.M14;

      mn[0, 1] = cs.M21;
      mn[1, 1] = cs.M22;
      mn[2, 1] = cs.M23;
      mn[3, 1] = cs.M24;

      mn[0, 2] = cs.M31;
      mn[1, 2] = cs.M32;
      mn[2, 2] = cs.M33;
      mn[3, 2] = cs.M34;

      mn[0, 3] = cs.M41;
      mn[1, 3] = cs.M42;
      mn[2, 3] = cs.M43;
      mn[3, 3] = cs.M44;

      return mn;
    }

    public static void Project(
        Matrix<double> m,
        ref double x,
        ref double y,
        ref double z,
        int inW,
        bool correctPerspective = true,
        bool normalize = true) {
      var vector = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(4);
      vector[0] = x;
      vector[1] = y;
      vector[2] = z;
      vector[3] = inW;

      var result = m.Multiply(vector);

      if (inW == 0) {
        x = result[0];
        y = result[1];
        z = result[2];

        if (normalize) {
          var len = Math.Sqrt(x * x + y * y + z * z);
          x /= len;
          y /= len;
          z /= len;
        }
      } else {
        var w = result[3];

        if (!correctPerspective) {
          w = 1;
        }

        x = result[0] / w;
        y = result[1] / w;
        z = result[2] / w;
      }
    }
  }
}