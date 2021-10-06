// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQModulationDepthEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQModulationDepthEvent : SSEQEvent
  {
    public byte Depth { get; private set; }

    public SSEQModulationDepthEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Depth = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.MidiTrack.Add((MidiEvent) new ControlChangeEvent((long) Result.CurrentTime, Result.TrackID + 1, MidiController.Modulation, (int) this.Depth));
    }
  }
}
