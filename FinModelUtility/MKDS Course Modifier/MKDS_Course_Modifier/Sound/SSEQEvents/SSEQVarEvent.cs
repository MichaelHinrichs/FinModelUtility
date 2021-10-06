// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQVarEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQVarEvent : SSEQEvent
  {
    public new byte EventID { get; private set; }

    public byte VariableID1 { get; private set; }

    public byte VariableID2 { get; private set; }

    public SSEQVarEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      EventID = er.ReadByte();
      this.VariableID1 = er.ReadByte();
      if (EventID < (byte) 176 || EventID > (byte) 189)
        return;
      this.VariableID2 = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
    }
  }
}
