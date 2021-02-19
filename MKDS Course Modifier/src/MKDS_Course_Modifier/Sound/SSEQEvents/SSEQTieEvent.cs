// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQTieEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQTieEvent : SSEQEvent
  {
    public byte Tie { get; private set; }

    public SSEQTieEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Tie = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.Tie = this.Tie == (byte) 1;
    }
  }
}
