using fin.model;

using System.Numerics;
using System.Runtime.CompilerServices;


namespace fin.math.matrix {
  public static class GlMatrixUtil {
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