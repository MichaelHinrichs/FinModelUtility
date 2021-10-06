// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQPrintVariableEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQPrintVariableEvent : SSEQEvent
  {
    public byte PrintVariable { get; private set; }

    public SSEQPrintVariableEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.PrintVariable = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
    }
  }
}
