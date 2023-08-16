using System.Numerics;
using System.Runtime.CompilerServices;

using fin.model;

namespace fin.math.matrix.four {
  public static class ProjectionUtil {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectPosition(
        Matrix4x4 matrix,
        ref Position xyz) 
      => ProjectionUtil.ProjectPosition(
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
      => ProjectionUtil.ProjectNormal(
          matrix,
          ref Unsafe.As<Normal, Vector3>(ref xyz));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectNormal(Matrix4x4 matrix, ref Vector3 xyz)
      => xyz = Vector3.TransformNormal(xyz, matrix);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectTangent(Matrix4x4 matrix, ref Tangent xyzw)
      => ProjectionUtil.ProjectTangent(
          matrix,
          ref Unsafe.As<Tangent, Vector4>(ref xyzw));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ProjectTangent(Matrix4x4 matrix, ref Vector4 xyzw)
      // TODO: Might be wrong
      => xyzw = Vector4.Transform(xyzw, matrix);
  }
}