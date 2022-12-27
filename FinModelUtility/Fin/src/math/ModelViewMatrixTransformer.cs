using fin.math.matrix;
using fin.model;
using fin.util.asserts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;


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

    public void ProjectVertex(ref Vector3 xyz)
      => GlMatrixUtil.ProjectPosition(this.current_.Impl, ref xyz);

    public void ProjectNormal(ref Vector3 xyz)
      => GlMatrixUtil.ProjectNormal(this.current_.Impl, ref xyz);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectPosition(
        Matrix4x4 matrix,
        ref Vector3 xyz)
      => xyz = Vector3.Transform(xyz, matrix);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectNormal(
        Matrix4x4 matrix,
        ref Vector3 xyz)
      => xyz = Vector3.TransformNormal(xyz, matrix);
  }
}