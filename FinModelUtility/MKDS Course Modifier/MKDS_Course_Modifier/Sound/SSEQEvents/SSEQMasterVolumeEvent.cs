// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQMasterVolumeEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using NAudio.Midi;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQMasterVolumeEvent : SSEQEvent
  {
    public byte MasterVolume { get; private set; }

    public SSEQMasterVolumeEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.MasterVolume = er.ReadByte();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.MidiTrack.Add((MidiEvent) new SequencerSpecificEvent(new byte[8]
      {
        (byte) 240,
        (byte) 127,
        (byte) 127,
        (byte) 4,
        (byte) 1,
        (byte) 0,
        this.MasterVolume,
        (byte) 247
      }, (long) Result.CurrentTime));
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 8, 8);
    }
  }
}
