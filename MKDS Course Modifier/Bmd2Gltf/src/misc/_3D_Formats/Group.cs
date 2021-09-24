// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.Group
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;

namespace MKDS_Course_Modifier._3D_Formats
{
  public class Group
  {
    private List<Polygon> PolygonList = new List<Polygon>();

    public Polygon[] Polygons
    {
      get
      {
        return this.PolygonList.ToArray();
      }
    }

    public void Add(Polygon g)
    {
      this.PolygonList.Add(g);
    }

    public Polygon this[int i]
    {
      get
      {
        return this.PolygonList[i];
      }
      set
      {
        this.PolygonList[i] = value;
      }
    }

    public IEnumerator<Polygon> GetEnumerator()
    {
      return (IEnumerator<Polygon>) this.PolygonList.GetEnumerator();
    }
  }
}
