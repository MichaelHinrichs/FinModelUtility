// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.Cube
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System;

namespace MKDS_Course_Modifier.Converters.Colission
{
  public class Cube
  {
    public float hw;
    public Vector3 c;

    public bool axis_test(float a1, float a2, float b1, float b2, float c1, float c2, Cube c)
    {
      float val1 = (float) ((double) a1 * (double) b1 + (double) a2 * (double) b2);
      float val2 = (float) ((double) a1 * (double) c1 + (double) a2 * (double) c2);
      float num = c.hw * (Math.Abs(a1) + Math.Abs(a2));
      return (double) Math.Min(val1, val2) > (double) num || (double) Math.Max(val1, val2) < -(double) num;
    }

    public bool tricube_overlap(Triangle t, Cube c)
    {
      Vector3 right = t.u - c.c;
      Vector3 vector3_1 = t.v - c.c;
      Vector3 vector3_2 = t.w - c.c;
      int num1;
      if ((double) Helpers.min(right.X, vector3_1.X, vector3_2.X) <= (double) c.hw)
        num1 = (double) Helpers.max(right.X, vector3_1.X, vector3_2.X) >= -((double) c.hw + (double) c.hw / 4.0) ? 1 : 0;
      else
        num1 = 0;
      if (num1 == 0)
        return false;
      int num2;
      if ((double) Helpers.min(right.Y, vector3_1.Y, vector3_2.Y) <= (double) c.hw)
        num2 = (double) Helpers.max(right.Y, vector3_1.Y, vector3_2.Y) >= -((double) c.hw + (double) c.hw / 4.0) ? 1 : 0;
      else
        num2 = 0;
      if (num2 == 0)
        return false;
      int num3;
      if ((double) Helpers.min(right.Z, vector3_1.Z, vector3_2.Z) <= (double) c.hw)
        num3 = (double) Helpers.max(right.Z, vector3_1.Z, vector3_2.Z) >= -((double) c.hw + (double) c.hw / 4.0) ? 1 : 0;
      else
        num3 = 0;
      if (num3 == 0)
        return false;
      float num4 = Vector3.Dot(t.n, right);
      float num5 = (float) (((double) c.hw + (double) c.hw / 4.0) * ((double) Math.Abs(t.n.X) + (double) Math.Abs(t.n.Y) + (double) Math.Abs(t.n.Z)));
      if ((double) num4 > (double) num5 || (double) num4 < -(double) num5)
        return false;
      Vector3 vector3_3 = vector3_1 - right;
      if (this.axis_test(vector3_3.Z, -vector3_3.Y, right.Y, right.Z, vector3_2.Y, vector3_2.Z, c) || this.axis_test(-vector3_3.Z, vector3_3.X, right.X, right.Z, vector3_2.X, vector3_2.Z, c) || this.axis_test(vector3_3.Y, -vector3_3.X, vector3_1.X, vector3_1.Y, vector3_2.X, vector3_2.Y, c))
        return false;
      Vector3 vector3_4 = vector3_2 - vector3_1;
      if (this.axis_test(vector3_4.Z, -vector3_4.Y, right.Y, right.Z, vector3_2.Y, vector3_2.Z, c) || this.axis_test(-vector3_4.Z, vector3_4.X, right.X, right.Z, vector3_2.X, vector3_2.Z, c) || this.axis_test(vector3_4.Y, -vector3_4.X, right.X, right.Y, vector3_1.X, vector3_1.Y, c))
        return false;
      Vector3 vector3_5 = right - vector3_2;
      return !this.axis_test(vector3_5.Z, -vector3_5.Y, right.Y, right.Z, vector3_1.Y, vector3_1.Z, c) && !this.axis_test(-vector3_5.Z, vector3_5.X, right.X, right.Z, vector3_1.X, vector3_1.Z, c) && !this.axis_test(vector3_5.Y, -vector3_5.X, vector3_1.X, vector3_1.Y, vector3_2.X, vector3_2.Y, c);
    }
  }
}
