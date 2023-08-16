using System.Numerics;
using System.Runtime.CompilerServices;

using fin.math.rotations;
using fin.model;

namespace fin.math.matrix {
  public static class FinMatrixUtil {
    public static IReadOnlyFinMatrix4x4 IDENTITY { get; } =
      FinMatrixUtil.FromIdentity();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromIdentity()
      => new FinMatrix4x4().SetIdentity();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTranslation(Position translation)
      => FinMatrixUtil.FromTranslation(
          translation.X,
          translation.Y,
          translation.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTranslation(float x, float y, float z)
      => new FinMatrix4x4(SystemMatrixUtil.FromTranslation(x, y, z));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromRotation(IRotation rotation)
      => FinMatrixUtil.FromRotation(QuaternionUtil.Create(rotation));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromRotation(Quaternion rotation)
      => new FinMatrix4x4(SystemMatrixUtil.FromRotation(rotation));



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromScale(Scale scale)
      => FromScale(scale.X, scale.Y, scale.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromScale(float scale)
      => FromScale(scale, scale, scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromScale(float scaleX,
                                          float scaleY,
                                          float scaleZ)
      => new FinMatrix4x4(SystemMatrixUtil.FromScale(scaleX, scaleY, scaleZ));


    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTrs(
        Position? translation,
        IRotation? rotation,
        Scale? scale)
      => FinMatrixUtil.FromTrs(
          translation,
          rotation != null ? QuaternionUtil.Create(rotation) : null,
          scale);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTrs(
      Position? translation,
      Quaternion? rotation,
      Scale? scale)
      => FromTrs(translation, rotation, scale, new FinMatrix4x4());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTrs(
      Position? translation,
      Quaternion? rotation,
      Scale? scale,
      IFinMatrix4x4 dst) {
      dst.CopyFrom(SystemMatrixUtil.FromTrs(translation, rotation, scale));
      return dst;
    }
  }
}