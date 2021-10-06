// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQCallEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Language;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQCallEvent : SSEQEvent
  {
    public uint Offset { get; private set; }

    public SSEQCallEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Offset = (uint) Bytes.Read3BytesAsInt24(er.ReadBytes(3), 0);
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.Goto = true;
      Result.GotoOffset = this.Offset;
    }

    public override string ToString()
    {
      return LanguageHandler.GetString("sound.sseq.events.call");
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 6, 6);
    }
  }
}
