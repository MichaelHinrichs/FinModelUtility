// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQNoteEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using NAudio.Midi;
using System;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQNoteEvent : SSEQEvent
  {
    private string[] noteString = new string[12]
    {
      "C",
      "C#",
      "D",
      "D#",
      "E",
      "F",
      "F#",
      "G",
      "G#",
      "A",
      "A#",
      "B"
    };

    public byte Velocity { get; private set; }

    public int Duration { get; private set; }

    public SSEQNoteEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Velocity = er.ReadByte();
      this.Duration = er.ReadVariableLength();
    }

    public SSEQNoteEvent(NoteOnEvent Event)
    {
      this.EventID = (byte) Event.NoteNumber;
      this.Duration = Event.NoteLength;
      this.Velocity = (byte) Event.Velocity;
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.MidiTrack.Add((MidiEvent) new NoteEvent((long) Result.CurrentTime, Result.TrackID + 1, MidiCommandCode.NoteOn, (int) this.EventID, this.Clamp((int) this.Velocity, 0, (int) sbyte.MaxValue)));
      if (this.Duration != 0)
        Result.MidiTrack.Add((MidiEvent) new NoteEvent((long) (Result.CurrentTime + this.Duration), Result.TrackID + 1, MidiCommandCode.NoteOff, (int) this.EventID, 64));
      else
        Result.MidiTrack.Add((MidiEvent) new NoteEvent((long) (Result.CurrentTime + 5000), Result.TrackID + 1, MidiCommandCode.NoteOff, (int) this.EventID, 64));
      if (!Result.NoteWait)
        return;
      Result.CurrentTime += this.Duration;
    }

    private int Clamp(int value, int min, int max)
    {
      return value < min ? min : (value > max ? max : value);
    }

    public override string ToString()
    {
      int num = (int) this.EventID / 12 - 1;
      int index = (int) this.EventID % 12;
      return LanguageHandler.GetString("sound.sseq.events.note") + " (" + this.noteString[index] + (object) index + ")";
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 1, 1);
    }
  }
}
