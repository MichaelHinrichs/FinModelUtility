// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.Helpers
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;
using System.Collections.Generic;

namespace MKDS_Course_Modifier.Converters.Colission
{
  internal class Helpers
  {
    public static int containsVector3(Vector3 a, List<Vector3> b)
    {
      for (int index = 0; index < b.Count; ++index)
      {
        if ((double) b[index].X == (double) a.X && (double) b[index].Y == (double) a.Y && (double) b[index].Z == (double) a.Z)
          return index;
      }
      return -1;
    }

    public static int next_exponent(float value, int bas)
    {
      return (double) value <= 1.0 ? 0 : (int) Math.Ceiling(Math.Log((double) value, (double) bas));
    }

    public static float norm_sq(Vector3 a)
    {
      return (float) ((double) a.X * (double) a.X + (double) a.Y * (double) a.Y + (double) a.Z * (double) a.Z);
    }

    public static float norm(Vector3 a)
    {
      return (float) Math.Sqrt((double) Helpers.norm_sq(a));
    }

    public static Vector3 unit(Vector3 a)
    {
      Vector3 vector3 = a;
      return Vector3.Divide(vector3, Helpers.norm(vector3));
    }

    public static float min(params float[] v)
    {
      float num1 = float.MaxValue;
      foreach (float num2 in v)
      {
        if ((double) num2 < (double) num1)
          num1 = num2;
      }
      return num1;
    }

    public static float max(params float[] v)
    {
      float num1 = float.MinValue;
      foreach (float num2 in v)
      {
        if ((double) num2 > (double) num1)
          num1 = num2;
      }
      return num1;
    }

    public static Vector3 MinMax(Vector3 min, Vector3 max, Vector3 input)
    {
      Vector3 vector3 = input;
      if ((double) min.X > (double) vector3.X)
        min.X = vector3.X;
      if ((double) min.Y > (double) vector3.Y)
        min.Y = vector3.Y;
      if ((double) min.Z > (double) vector3.Z)
        min.Z = vector3.Z;
      if ((double) max.X < (double) vector3.X)
        max.X = vector3.X;
      if ((double) max.Y < (double) vector3.Y)
        max.Y = vector3.Y;
      if ((double) max.Z < (double) vector3.Z)
        max.Z = vector3.Z;
      return vector3;
    }
  }
}
