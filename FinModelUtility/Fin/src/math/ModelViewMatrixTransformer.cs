using System;
using System.Collections.Generic;
using System.Numerics;

using fin.math.matrix;
using fin.model;


namespace fin.math {
  public class SoftwareModelViewMatrixTransformer {
    private IFinMatrix4x4 current_;
    private readonly LinkedList<MatrixNode> stack_ = new();

    private class MatrixNode {
      public IFinMatrix4x4 Matrix { get; init; }
    }

    public SoftwareModelViewMatrixTransformer() {
      this.Push();
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
      IFinMatrix4x4 newMatrix;
      if (this.current_ == null) {
        newMatrix = new FinMatrix4x4().SetIdentity();
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

    public SoftwareModelViewMatrixTransformer Identity() {
      this.current_.SetIdentity();
      return this;
    }


    public SoftwareModelViewMatrixTransformer Translate(IPosition position)
      => this.MultMatrix(MatrixTransformUtil.FromTranslation(position));

    public SoftwareModelViewMatrixTransformer Translate(
        float x,
        float y,
        float z)
      => this.MultMatrix(MatrixTransformUtil.FromTranslation(x, y, z));


    public SoftwareModelViewMatrixTransformer Rotate(IRotation rotation)
      => this.MultMatrix(MatrixTransformUtil.FromRotation(rotation));

    public SoftwareModelViewMatrixTransformer Rotate(Quaternion rotation)
      => this.MultMatrix(MatrixTransformUtil.FromRotation(rotation));

    public SoftwareModelViewMatrixTransformer Scale(IScale scale)
      => this.MultMatrix(MatrixTransformUtil.FromScale(scale));


    public SoftwareModelViewMatrixTransformer Trs(
        IPosition? position,
        IRotation? rotation,
        IScale? scale)
      => this.MultMatrix(
          MatrixTransformUtil.FromTrs(position, rotation, scale));

    public SoftwareModelViewMatrixTransformer Trs(
        IPosition? position,
        Quaternion? rotation,
        IScale? scale)
      => this.MultMatrix(
          MatrixTransformUtil.FromTrs(position, rotation, scale));


    public SoftwareModelViewMatrixTransformer MultMatrix(
        IReadOnlyFinMatrix4x4 m) {
      this.current_.MultiplyInPlace(m);
      return this;
    }

    public void Get(IFinMatrix4x4 m) => m.CopyFrom(current_);
    public IFinMatrix4x4 Get() => this.current_.Clone();

    public void Set(IReadOnlyFinMatrix4x4 m) => this.current_.CopyFrom(m);
  }

  // TODO: Move this somewhere else.
  public class GlMatrixUtil {
    private static readonly FinVector4 SHARED_VECTOR = new();


    public static void ProjectVertex(
        IReadOnlyFinMatrix4x4 matrix,
        ref double x,
        ref double y,
        ref double z)
      => GlMatrixUtil.Project(matrix,
                              ref x,
                              ref y,
                              ref z,
                              1,
                              true,
                              false);

    public static void ProjectNormal(
        IReadOnlyFinMatrix4x4 matrix,
        ref double x,
        ref double y,
        ref double z,
        bool normalize = true)
      => GlMatrixUtil.Project(matrix,
                              ref x,
                              ref y,
                              ref z,
                              0,
                              false,
                              normalize);

    public static void Project(
        IReadOnlyFinMatrix4x4 matrix,
        ref double x,
        ref double y,
        ref double z,
        int inW,
        bool correctPerspective = true,
        bool normalize = true) {
      SHARED_VECTOR.X = x;
      SHARED_VECTOR.Y = y;
      SHARED_VECTOR.Z = z;
      SHARED_VECTOR.W = inW;

      SHARED_VECTOR.MultiplyInPlace(matrix);

      if (inW == 0) {
        if (normalize) {
          GlMatrixUtil.SHARED_VECTOR.NormalizeInPlace();
        }
      } else if (correctPerspective) {
        GlMatrixUtil.SHARED_VECTOR.MultiplyInPlace(1 / SHARED_VECTOR.W);
      }

      x = SHARED_VECTOR.X;
      y = SHARED_VECTOR.Y;
      z = SHARED_VECTOR.Z;
    }
  }
}