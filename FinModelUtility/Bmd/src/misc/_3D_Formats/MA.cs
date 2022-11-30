// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier._3D_Formats.MA
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe


namespace bmd._3D_Formats {
  public class MA {
    public class Node {
      public Node(string Name, string Parent = null) {
        this.Name = Name;
        this.Parent = Parent;
      }

      public string Name { get; set; }

      public string Parent { get; set; }

      public override string ToString() {
        return this.Name;
      }
    }
  }
}