using fin.math.matrix;
using fin.model;

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

      this.stack_.AddLast(new MatrixNode { Matrix = newMatrix });
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


    public SoftwareModelViewMatrixTransformer Translate(Position position)
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

    public SoftwareModelViewMatrixTransformer Scale(Scale scale)
      => this.MultMatrix(MatrixTransformUtil.FromScale(scale));


    public SoftwareModelViewMatrixTransformer Trs(
        Position? position,
        IRotation? rotation,
        Scale? scale)
      => this.MultMatrix(
          MatrixTransformUtil.FromTrs(position, rotation, scale));

    public SoftwareModelViewMatrixTransformer Trs(
        Position? position,
        Quaternion? rotation,
        Scale? scale)
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
        ref Position xyz) 
      => GlMatrixUtil.ProjectPosition(
          matrix,
          ref Unsafe.As<Position, Vector3>(ref xyz));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectPosition(
        Matrix4x4 matrix,
        ref Vector3 xyz)
      => xyz = Vector3.Transform(xyz, matrix);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectNormal(
        Matrix4x4 matrix,
        ref Normal xyz)
      => GlMatrixUtil.ProjectNormal(
          matrix,
          ref Unsafe.As<Normal, Vector3>(ref xyz));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectNormal(Matrix4x4 matrix, ref Vector3 xyz)
      => xyz = Vector3.TransformNormal(xyz, matrix);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectTangent(Matrix4x4 matrix, ref Tangent xyzw)
      => GlMatrixUtil.ProjectTangent(
          matrix,
          ref Unsafe.As<Tangent, Vector4>(ref xyzw));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectTangent(Matrix4x4 matrix, ref Vector4 xyzw)
      // TODO: Might be wrong
      => xyzw = Vector4.Transform(xyzw, matrix);
  }
}