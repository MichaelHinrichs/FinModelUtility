// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Converters.Colission.OctreeNode
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using OpenTK;
using System.Collections.Generic;

namespace MKDS_Course_Modifier.Converters.Colission
{
  public class OctreeNode : Cube
  {
    public List<int> indices = new List<int>();
    public List<OctreeNode> branches = new List<OctreeNode>();
    public bool is_leaf;

    public OctreeNode(
      Vector3 bas,
      float width,
      List<Triangle> triangles,
      List<int> indices,
      int max_triangles,
      int min_width)
    {
      this.hw = width / 2f;
      this.c = bas + new Vector3(this.hw, this.hw, this.hw);
      this.is_leaf = true;
      foreach (int index in indices)
      {
        if (this.tricube_overlap(triangles[index], (Cube) this))
          this.indices.Add(index);
      }
      if (this.indices.Count <= max_triangles || (double) this.hw <= (double) min_width)
        return;
      this.is_leaf = false;
      for (int index1 = 0; index1 < 2; ++index1)
      {
        for (int index2 = 0; index2 < 2; ++index2)
        {
          for (int index3 = 0; index3 < 2; ++index3)
            this.branches.Add(new OctreeNode(bas + this.hw * new Vector3((float) index3, (float) index2, (float) index1), this.hw, triangles, this.indices, max_triangles, min_width));
        }
      }
      this.indices.Clear();
    }
  }
}
