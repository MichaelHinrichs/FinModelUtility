// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.Triangle
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;

namespace MKDS_Course_Modifier.Converters.Colission
{
  public class Triangle
  {
    public Vector3 u;
    public Vector3 v;
    public Vector3 w;
    public Vector3 n;

    public Triangle(Vector3 u, Vector3 v, Vector3 w)
    {
      this.u = u;
      this.v = v;
      this.w = w;
      this.n = Helpers.unit(Vector3.Cross(v - u, w - u));
    }

    public Triangle(Vector3 u, Vector3 v, Vector3 w, Vector3 n)
    {
      this.u = u;
      this.v = v;
      this.w = w;
      this.n = n;
    }
  }
}
