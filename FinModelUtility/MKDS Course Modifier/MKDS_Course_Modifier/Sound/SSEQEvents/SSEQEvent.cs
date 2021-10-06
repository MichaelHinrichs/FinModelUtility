// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQEvent
  {
    public byte EventID { get; protected set; }

    public virtual void AddMidiEvents(ref SSEQMidiResult Result)
    {
    }

    public override string ToString()
    {
      return this.GetType().Name;
    }

    public virtual TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 9, 9);
    }
  }
}
