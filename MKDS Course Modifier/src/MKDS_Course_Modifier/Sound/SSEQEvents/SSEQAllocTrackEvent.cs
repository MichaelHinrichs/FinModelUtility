// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQAllocTrackEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQAllocTrackEvent : SSEQEvent
  {
    public bool[] TrackUsed;

    public ushort TracksUsed { get; private set; }

    public SSEQAllocTrackEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.TracksUsed = er.ReadUInt16();
      this.TrackUsed = new bool[16];
      for (int index = 0; index < 16; ++index)
        this.TrackUsed[index] = ((int) this.TracksUsed >> index & 1) == 1;
    }

    public SSEQAllocTrackEvent(int NrTracks)
    {
      this.EventID = (byte) 254;
      this.TracksUsed = (ushort) 0;
      this.TrackUsed = new bool[16];
      for (int index = 0; index < 16; ++index)
      {
        if (index < NrTracks)
        {
          this.TrackUsed[index] = true;
          this.TracksUsed |= (ushort) (1 << index);
        }
        else
          this.TrackUsed[index] = false;
      }
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
    }

    public override string ToString()
    {
      return LanguageHandler.GetString("sound.sseq.events.alloc");
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 2, 2);
    }
  }
}
