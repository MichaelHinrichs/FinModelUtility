// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQPitchRangeEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQPitchRangeEvent : SSEQEvent
  {
    public byte PitchRange { get; private set; }

    public SSEQPitchRangeEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.PitchRange = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      MidiEvent midiEvent1 = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(101, 0, Result.TrackID + 1).RawData);
      midiEvent1.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent1);
      MidiEvent midiEvent2 = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(100, 0, Result.TrackID + 1).RawData);
      midiEvent2.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent2);
      MidiEvent midiEvent3 = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(6, (int) this.PitchRange, Result.TrackID + 1).RawData);
      midiEvent3.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent3);
    }
  }
}
