// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQBankChangeEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQBankChangeEvent : SSEQEvent
  {
    public int RealProgram { get; private set; }

    public SSEQBankChangeEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.RealProgram = er.ReadVariableLength();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      int patchNumber = this.RealProgram % 128;
      int num = this.RealProgram / 128 & 15;
      MidiEvent midiEvent1 = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(0, this.RealProgram / 128 / 128 & 15, Result.TrackID + 1).RawData);
      midiEvent1.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent1);
      MidiEvent midiEvent2 = MidiEvent.FromRawMessage(MidiMessage.ChangeControl(32, num, Result.TrackID + 1).RawData);
      midiEvent2.AbsoluteTime = (long) Result.CurrentTime;
      Result.MidiTrack.Add(midiEvent2);
      Result.MidiTrack.Add((MidiEvent) new PatchChangeEvent((long) Result.CurrentTime, Result.TrackID + 1, patchNumber));
      if (patchNumber != (int) sbyte.MaxValue)
        return;
      Result.TrackID = 9;
      foreach (MidiEvent midiEvent3 in (IEnumerable<MidiEvent>) Result.MidiTrack)
        midiEvent3.Channel = 10;
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 5, 5);
    }
  }
}
