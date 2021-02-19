// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEncoder
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Sound.SSEQEvents;
using NAudio.Midi;
using System.Collections.Generic;

namespace MKDS_Course_Modifier.Sound
{
  public class SSEQEncoder
  {
    private List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent> Events;

    public SSEQEncoder(string MidiPath)
    {
      MidiFile midiFile = new MidiFile(MidiPath);
      this.Events = new List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent>();
      this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQAllocTrackEvent(midiFile.Tracks));
      for (int index = 0; index < midiFile.Tracks - 1; ++index)
        this.Events.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQTrackEvent(index + 1));
      foreach (MidiEventCollection midiEventCollection in midiFile.Events)
      {
        List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent> sseqEventList = new List<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent>();
        foreach (MidiEvent midiEvent in midiEventCollection)
        {
          if (midiEvent is NoteOnEvent)
            sseqEventList.Add((MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent) new SSEQNoteEvent((NoteOnEvent) midiEvent));
        }
        this.Events.AddRange((IEnumerable<MKDS_Course_Modifier.Sound.SSEQEvents.SSEQEvent>) sseqEventList);
      }
    }
  }
}
