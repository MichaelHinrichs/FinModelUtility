// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQPortamentoOnOffEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQPortamentoOnOffEvent : SSEQEvent
  {
    public byte Mode { get; private set; }

    public SSEQPortamentoOnOffEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Mode = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      MidiEvent midiEvent = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(65, (int) this.Mode, Result.TrackID + 1).RawData);
      midiEvent.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent);
    }
  }
}
