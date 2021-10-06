using System;
using System.Collections.Generic;

using MathNet.Numerics.LinearAlgebra;

using Tao.OpenGl;

using UoT.util;

namespace UoT {
  public interface IModelViewMatrixTransformer {
    void ProjectVertex(ref double x, ref double y, ref double z);

    void ProjectNormal(
        ref double normalX,
        ref double normalY,
        ref double normalZ);

    IModelViewMatrixTransformer Push();
    IModelViewMatrixTransformer Pop();

    IModelViewMatrixTransformer Identity();

    IModelViewMatrixTransformer Translate(double x, double y, double z);

    IModelViewMatrixTransformer Rotate(
        double angle,
        double x,
        double y,
        double z);

    IModelViewMatrixTransformer MultMatrix(Matrix<double> m);

    void Get(Matrix<double> m);
    void Set(Matrix<double> m);
  }

  public static class ModelViewMatrixTransformer {
    private static IModelViewMatrixTransformer INSTANCE =
        new SoftwareModelViewMatrixTransformer();

    public static void ProjectVertex(ref double x, ref double y, ref double z)
      => ModelViewMatrixTransformer.INSTANCE.ProjectVertex(ref x, ref y, ref z);

    public static void ProjectNormal(
        ref double normalX,
        ref double normalY,
        ref double normalZ)
      => ModelViewMatrixTransformer.INSTANCE.ProjectNormal(
          ref normalX,
          ref normalY,
          ref normalZ);

    public static IModelViewMatrixTransformer Push()
      => ModelViewMatrixTransformer.INSTANCE.Push();

    public static IModelViewMatrixTransformer Pop()
      => ModelViewMatrixTransformer.INSTANCE.Pop();

    public static IModelViewMatrixTransformer Identity()
      => ModelViewMatrixTransformer.INSTANCE.Identity();

    public static IModelViewMatrixTransformer Translate(
        double x,
        double y,
        double z) => ModelViewMatrixTransformer.INSTANCE.Translate(x, y, z);

    public static IModelViewMatrixTransformer Rotate(
        double angle,
        double x,
        double y,
        double z) => ModelViewMatrixTransformer.INSTANCE.Rotate(angle, x, y, z);

    public static IModelViewMatrixTransformer MultMatrix(Matrix<double> m)
      => ModelViewMatrixTransformer.INSTANCE.MultMatrix(m);

    public static void Get(Matrix<double> m)
      => ModelViewMatrixTransformer.INSTANCE.Get(m);

    public static void Set(Matrix<double> m)
      => ModelViewMatrixTransformer.INSTANCE.Set(m);
  }


  public class GlModelViewMatrixTransformer : IModelViewMatrixTransformer {
    private readonly Matrix<double> buffer_ =
        Matrix<double>.Build.DenseIdentity(4, 4);

    public void ProjectVertex(ref double x, ref double y, ref double z) {
      GlMatrixUtil.Get(this.buffer_);
      GlMatrixUtil.Project(this.buffer_, ref x, ref y, ref z, 1);
    }

    public void ProjectNormal(
        ref double normalX,
        ref double normalY,
        ref double normalZ) {
      GlMatrixUtil.Get(this.buffer_);
      GlMatrixUtil.Project(this.buffer_,
                           ref normalX,
                           ref normalY,
                           ref normalZ,
                           0);
    }

    public IModelViewMatrixTransformer Push() {
      Gl.glPushMatrix();
      return this;
    }

    public IModelViewMatrixTransformer Pop() {
      Gl.glPopMatrix();
      return this;
    }

    public IModelViewMatrixTransformer Identity() {
      Gl.glLoadIdentity();
      return this;
    }

    public IModelViewMatrixTransformer Translate(double x, double y, double z) {
      Gl.glTranslated(x, y, z);
      return this;
    }

    public IModelViewMatrixTransformer Rotate(
        double angle,
        double x,
        double y,
        double z) {
      Gl.glRotated(angle, x, y, z);
      return this;
    }

    public IModelViewMatrixTransformer MultMatrix(Matrix<double> m) {
      Gl.glMultMatrixd(m.ToColumnMajorArray());
      return this;
    }

    public void Get(Matrix<double> m)
      => GlMatrixUtil.Get(m);

    public void Set(Matrix<double> m)
      => GlMatrixUtil.Set(m);
  }

  public class
      SoftwareModelViewMatrixTransformer : IModelViewMatrixTransformer {
    private Matrix<double>? current_;
    private LinkedList<MatrixNode> stack_ = new LinkedList<MatrixNode>();

    private class MatrixNode {
      public Matrix<double>? Matrix { get; set; }
    }

    public SoftwareModelViewMatrixTransformer() {
      this.Push();

      // TODO: Assigned in push, possible to fix this?
      //this.current_ = this.current_;
    }

    public void ProjectVertex(ref double x, ref double y, ref double z)
      => GlMatrixUtil.Project(Asserts.Assert(this.current_),
                              ref x,
                              ref y,
                              ref z,
                              1);

    public void ProjectNormal(
        ref double normalX,
        ref double normalY,
        ref double normalZ)
      => GlMatrixUtil.Project(Asserts.Assert(this.current_),
                              ref normalX,
                              ref normalY,
                              ref normalZ,
                              0);

    public IModelViewMatrixTransformer Push() {
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
      this.UpdateGl_();

      return this;
    }

    public IModelViewMatrixTransformer Pop() {
      if (this.stack_.Count <= 1) {
        throw new Exception("Popped too far.");
      }

      this.stack_.RemoveLast();
      this.UpdateCurrent_();
      this.UpdateGl_();

      return this;
    }

    private void UpdateCurrent_()
      => this.current_ = Asserts.Assert(this.stack_.Last.Value.Matrix);

    private readonly Matrix<double> rhsBuffer_ =
        Matrix<double>.Build.DenseIdentity(4, 4);

    private readonly Matrix<double> resultBuffer_ =
        Matrix<double>.Build.DenseIdentity(4, 4);

    public IModelViewMatrixTransformer Identity() {
      this.current_!.Clear();
      for (var i = 0; i < 4; ++i) {
        this.current_[i, i] = 1;
      }

      this.UpdateGl_();

      return this;
    }


    public IModelViewMatrixTransformer Translate(double x, double y, double z) {
      this.rhsBuffer_.Clear();
      for (var i = 0; i < 4; ++i) {
        this.rhsBuffer_[i, i] = 1;
      }
      this.rhsBuffer_[0, 3] = x;
      this.rhsBuffer_[1, 3] = y;
      this.rhsBuffer_[2, 3] = z;

      this.current_!.Multiply(this.rhsBuffer_, this.resultBuffer_);

      this.resultBuffer_.CopyTo(this.current_);
      this.UpdateGl_();

      return this;
    }

    public IModelViewMatrixTransformer Rotate(
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


      this.current_!.Multiply(this.rhsBuffer_, this.resultBuffer_);

      this.resultBuffer_.CopyTo(this.current_!);
      this.UpdateGl_();

      return this;
    }

    public IModelViewMatrixTransformer MultMatrix(Matrix<double> m) {
      this.current_!.Multiply(m, this.resultBuffer_);

      this.resultBuffer_.CopyTo(this.current_);
      this.UpdateGl_();

      return this;
    }

    public void Get(Matrix<double> m) {
      this.current_!.CopyTo(m);
    }

    public void Set(Matrix<double> m) {
      m.CopyTo(this.current_);
      this.UpdateGl_();
    }

    private void UpdateGl_() {
      //GlMatrixUtil.Set(this.current_);
    }
  }

  // TODO: Move this somewhere else.
  public class GlMatrixUtil {
    private static readonly double[] glBuffer_ = new double[16];

    public static void Set(Matrix<double> m) {
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          GlMatrixUtil.glBuffer_[4 * c + r] = m[r, c];
        }
      }
      Gl.glLoadMatrixd(GlMatrixUtil.glBuffer_);
    }

    public static void Get(Matrix<double> m) {
      Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, GlMatrixUtil.glBuffer_);
      for (var r = 0; r < 4; ++r) {
        for (var c = 0; c < 4; ++c) {
          m[r, c] = GlMatrixUtil.glBuffer_[4 * c + r];
        }
      }
    }

    public static void Project(
        Matrix<double> m,
        ref double x,
        ref double y,
        ref double z,
        int inW) {
      var vector = Vector<double>.Build.Dense(4);
      vector[0] = x;
      vector[1] = y;
      vector[2] = z;
      vector[3] = inW;

      var result = m.Multiply(vector);

      if (inW == 0) {
        x = result[0];
        y = result[1];
        z = result[2];

        var len = Math.Sqrt(x * x + y * y + z * z);
        x /= len;
        y /= len;
        z /= len;
      } else {
        var w = result[3];
        x = result[0] / w;
        y = result[1] / w;
        z = result[2] / w;
      }
    }
  }
}