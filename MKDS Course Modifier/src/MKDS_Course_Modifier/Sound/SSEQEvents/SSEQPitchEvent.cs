// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQPitchEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQPitchEvent : SSEQEvent
  {
    public sbyte Pitch { get; private set; }

    public SSEQPitchEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Pitch = er.ReadSByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      short num = (short) (((int) this.Pitch + 128) * 64);
      if (num < (short) 0 || num > (short) 16384)
        throw new Exception("Pitch isn't in a range between 0 and 0x4000.");
      Result.MidiTrack.Add((MidiEvent) new PitchWheelChangeEvent((long) Result.CurrentTime, Result.TrackID + 1, (int) num));
    }
  }
}
