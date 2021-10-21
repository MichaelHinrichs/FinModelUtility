// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.Polygon
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;

using System.Drawing;

namespace bmd._3D_Formats {
  public class Polygon {
    public PolygonType PolyType;
    public Vector3[] Normals;
    public Vector2[] TexCoords;
    public Vector2[] TexCoords2;
    public Vector2[] TexCoords3;
    public Vector3[] Vertex;
    public Color[] Colors;

    public Polygon() {}

    public Polygon(
        PolygonType PolyType,
        Vector3[] Normals,
        Vector2[] TexCoords,
        Vector3[] Vertex,
        Color[] Colors = null) {
      this.PolyType = PolyType;
      this.Normals = Normals;
      this.TexCoords = TexCoords;
      this.Vertex = Vertex;
      this.Colors = Colors;
    }
  }
}