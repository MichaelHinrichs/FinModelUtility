// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQDecayRateEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQDecayRateEvent : SSEQEvent
  {
    public byte DecayRate { get; private set; }

    public SSEQDecayRateEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.DecayRate = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      MidiEvent midiEvent = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(75, 64 + (int) this.DecayRate / 2, Result.TrackID + 1).RawData);
      midiEvent.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent);
    }
  }
}
