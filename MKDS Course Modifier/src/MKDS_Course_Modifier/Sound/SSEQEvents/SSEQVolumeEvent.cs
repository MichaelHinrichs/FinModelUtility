// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQVolumeEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQVolumeEvent : SSEQEvent
  {
    public byte Volume { get; private set; }

    public SSEQVolumeEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Volume = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.MidiTrack.Add((MidiEvent) new ControlChangeEvent((long) Result.CurrentTime, Result.TrackID + 1, MidiController.MainVolume, (int) this.Volume));
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 8, 8);
    }
  }
}
