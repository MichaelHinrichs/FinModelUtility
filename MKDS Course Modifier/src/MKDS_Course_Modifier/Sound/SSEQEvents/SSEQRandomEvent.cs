// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQRandomEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQRandomEvent : SSEQEvent
  {
    public new byte EventID { get; private set; }

    public short Min { get; private set; }

    public short Max { get; private set; }

    public SSEQRandomEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      EventID = er.ReadByte();
      this.Min = er.ReadInt16();
      this.Max = er.ReadInt16();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
    }
  }
}
