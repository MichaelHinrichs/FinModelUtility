// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQMidiResult
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.Collections.Generic;

namespace MKDS_Course_Modifier.Sound
{
  public class SSEQMidiResult
  {
    public bool NoteWait = true;
    public bool Tie = false;
    public IList<MidiEvent> MidiTrack = (IList<MidiEvent>) new List<MidiEvent>();
    public int CurrentTime = 0;
    public bool Goto = false;
    public bool Return = false;
    public uint GotoOffset = 0;
    public int ReturnOffset = -1;
    public short[] LocalVariables = new short[16];
    public short[] GlobalVariables = new short[16];
    public bool ComparisonResult = true;
    public bool If = false;

    public SSEQMidiResult(int TrackID, short[] GlobalVariables)
    {
      this.GlobalVariables = GlobalVariables;
      this.TrackID = TrackID;
    }

    public int TrackID { get; set; }

    public bool GotDrums { get; private set; }
  }
}
