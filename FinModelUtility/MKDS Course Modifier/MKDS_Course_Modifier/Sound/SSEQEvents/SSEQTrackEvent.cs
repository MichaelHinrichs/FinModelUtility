// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQTrackEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQTrackEvent : SSEQEvent
  {
    public uint Offset { get; private set; }

    public byte TrackNr { get; private set; }

    public SSEQTrackEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.TrackNr = er.ReadByte();
      this.Offset = (uint) Bytes.Read3BytesAsInt24(er.ReadBytes(3), 0);
    }

    public SSEQTrackEvent(int TrackNr)
    {
      this.EventID = (byte) 147;
      this.TrackNr = (byte) TrackNr;
    }

    public void SetOffset(uint Offset)
    {
      this.Offset = Offset;
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
    }
  }
}
