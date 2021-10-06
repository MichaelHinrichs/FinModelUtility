// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQTempoEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using NAudio.Midi;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQTempoEvent : SSEQEvent
  {
    public short Tempo { get; private set; }

    public SSEQTempoEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Tempo = er.ReadInt16();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.MidiTrack.Add((MidiEvent) new TempoEvent((int) (60000000.0 / (double) this.Tempo), (long) Result.CurrentTime));
    }

    public override string ToString()
    {
      return LanguageHandler.GetString("sound.sseq.events.tempo") + " (" + (object) this.Tempo + " bpm)";
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 0, 0);
    }
  }
}
