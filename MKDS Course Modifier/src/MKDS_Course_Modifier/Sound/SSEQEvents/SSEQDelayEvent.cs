// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQDelayEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQDelayEvent : SSEQEvent
  {
    public int Delay { get; private set; }

    public SSEQDelayEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Delay = er.ReadVariableLength();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.CurrentTime += this.Delay;
    }

    public override string ToString()
    {
      return LanguageHandler.GetString("sound.sseq.events.delay") + " (" + (object) this.Delay + ")";
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 3, 3);
    }
  }
}
