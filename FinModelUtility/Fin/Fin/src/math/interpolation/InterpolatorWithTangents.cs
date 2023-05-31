using System.Numerics;
using System.Runtime.CompilerServices;

using MathNet.Numerics.Interpolation;


namespace fin.math.interpolation {
  public static class InterpolatorWithTangents {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InterpolateFloats(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time)
      => InterpolateFloatsGithub(
          fromTime, fromValue, fromTangent,
          toTime, toValue, toTangent,
          time);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InterpolateFloatsGithub(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      // https://answers.unity.com/questions/464782/t-is-the-math-behind-animationcurveevaluate.html
      var dt = toTime - fromTime;

      var m0 = fromTangent * dt;
      var m1 = toTangent * dt;

      var t1 = (time - fromTime) / (toTime - fromTime);
      var t2 = t1 * t1;
      var t3 = t2 * t1;

      var a = 2 * t3 - 3 * t2 + 1;
      var b = t3 - 2 * t2 + t1;
      var c = t3 - t2;
      var d = -2 * t3 + 3 * t2;

      return a * fromValue + b * m0 + c * m1 + d * toValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InterpolateFloatsWithMathNet(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var spline = CubicSpline
          .InterpolateHermiteSorted(new double[] { fromTime, toTime },
                                    new double[] { fromValue, toValue },
                                    new double[] { fromTangent, toTangent });

      return (float)spline.Interpolate(time);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InterpolateFloatsWithMkds(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var progress = (time - fromTime) / (toTime - fromTime);

      var v1 = fromValue;
      var v2 = toValue;
      var d1 = fromTangent;
      var d2 = toTangent;
      var t = progress;

      var num1 = 2.0 * (v1 - v2) + d1 + d2;
      var num2 = -3.0 * v1 + 3.0 * v2 - 2.0 * d1 - d2;
      var num3 = d1;
      var num4 = v1;
      return (float)(((num1 * t + num2) * t + num3) * t + num4);
    }

    public static float InterpolateFloatsWithRandom(float fromTime,
                                                    float fromValue,
                                                    float fromTangent,
                                                    float toTime,
                                                    float toValue,
                                                    float toTangent,
                                                    float time) {
      var progress = (time - fromTime) / (toTime - fromTime);

      var s = 1 / 30f;

      var p0 = fromValue;
      var p1 = toValue;
      var m0 = fromTangent * s;
      var m1 = toTangent * s;
      var t = progress;

      float tt = t * t;
      float ttt = t * t * t;

      return
          (2.0f * ttt - 3.0f * tt + 1.0f) * p0 +
          (ttt - 2.0f * tt + t) * m0 +
          (-2.0f * ttt + 3.0f * tt) * p1 +
          (ttt - tt) * m1;
    }

    private static Matrix4x4 m_hermiteMatrix =
        new(2, -2, 1, 1, -3, 3, -2, -1, 0, 0, 1, 0, 1, 0, 0, 0);

    public static float InterpolateFloatsWithJ3dHermite(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      // https://github.com/LordNed/JStudio/blob/93c5c4479ffb1babefe829cfc9794694a1cb93e6/JStudio/J3D/Animation/BaseJ3DAnimation.cs
      float numFramesBetweenKeys = toTime - fromTime;

      var t = (time - fromTime) / (toTime - fromTime);

      Vector4 s = new Vector4(t * t * t, t * t, t, 1);
      Vector4 c = new Vector4(fromValue, toValue,
                              fromTangent * numFramesBetweenKeys,
                              toTangent * numFramesBetweenKeys);
      Vector4 result = Vector4.Transform(s, m_hermiteMatrix);
      result = Vector4.Multiply(result, c);

      return result.X + result.Y + result.Z + result.W;
    }

    public static float InterpolateFloatsWithNoclipHermite(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var length = toTime - fromTime;

      var p0 = fromValue;
      var p1 = toValue;
      var s0 = fromTangent * length;
      var s1 = toTangent * length;

      var t = (time - fromTime) / length;

      var cf0 = (p0 * 2) + (p1 * -2) + (s0 * 1) + (s1 * 1);
      var cf1 = (p0 * -3) + (p1 * 3) + (s0 * -2) + (s1 * -1);
      var cf2 = (p0 * 0) + (p1 * 0) + (s0 * 1) + (s1 * 0);
      var cf3 = (p0 * 1) + (p1 * 0) + (s0 * 0) + (s1 * 0);

      return (((cf0 * t + cf1) * t + cf2) * t + cf3);
    }

    public static float InterpolateFloatsWithNoclipBezier(
        float fromTime,
        float fromValue,
        float fromTangent,
        float toTime,
        float toValue,
        float toTangent,
        float time) {
      var length = toTime - fromTime;

      var p0 = fromValue;
      var p1 = toValue;
      var p2 = fromTangent;
      var p3 = toTangent;

      var t = (time - fromTime) / length;

      var cf0 = (p0 * -1) + (p1 * 3) + (p2 * -3) + (p3 * 1);
      var cf1 = (p0 * 3) + (p1 * -6) + (p2 * 3) + (p3 * 0);
      var cf2 = (p0 * -3) + (p1 * 3) + (p2 * 0) + (p3 * 0);
      var cf3 = (p0 * 1) + (p1 * 0) + (p2 * 0) + (p3 * 0);

      return (((cf0 * t + cf1) * t + cf2) * t + cf3);
    }
  }
}