﻿using System.Numerics;
using System.Runtime.CompilerServices;

using fin.math.rotations;
using fin.model;

namespace fin.math.matrix.four {
  public static class FinMatrix4x4Util {
    public static IReadOnlyFinMatrix4x4 IDENTITY { get; } =
      FinMatrix4x4Util.FromIdentity();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromIdentity()
      => new FinMatrix4x4().SetIdentity();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTranslation(Position translation)
      => FinMatrix4x4Util.FromTranslation(
          translation.X,
          translation.Y,
          translation.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTranslation(float x, float y, float z)
      => new FinMatrix4x4(SystemMatrix4x4Util.FromTranslation(x, y, z));


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromRotation(IRotation rotation)
      => FinMatrix4x4Util.FromRotation(QuaternionUtil.Create(rotation));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromRotation(Quaternion rotation)
      => new FinMatrix4x4(SystemMatrix4x4Util.FromRotation(rotation));



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
      => new FinMatrix4x4(SystemMatrix4x4Util.FromScale(scaleX, scaleY, scaleZ));


    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFinMatrix4x4 FromTrs(
        Position? translation,
        IRotation? rotation,
        Scale? scale)
      => FinMatrix4x4Util.FromTrs(
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
      dst.CopyFrom(SystemMatrix4x4Util.FromTrs(translation, rotation, scale));
      return dst;
    }
  }
}