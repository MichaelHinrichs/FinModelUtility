// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQJumpEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Converters;
using MKDS_Course_Modifier.Language;
using NAudio.Midi;
using System.IO;
using System.Windows.Forms;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQJumpEvent : SSEQEvent
  {
    public uint Offset { get; private set; }

    public SSEQJumpEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.Offset = (uint) Bytes.Read3BytesAsInt24(er.ReadBytes(3), 0);
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.MidiTrack.Add((MidiEvent) new TextEvent("loopEnd", MetaEventType.Marker, (long) Result.CurrentTime));
    }

    public override string ToString()
    {
      return LanguageHandler.GetString("sound.sseq.events.jump");
    }

    public override TreeNode GetTreeNode()
    {
      return new TreeNode(this.ToString(), 7, 7);
    }
  }
}
