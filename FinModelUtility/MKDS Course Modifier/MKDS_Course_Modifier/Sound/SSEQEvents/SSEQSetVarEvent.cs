// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQSetVarEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQSetVarEvent : SSEQEvent
  {
    public byte VarID { get; private set; }

    public short Value { get; private set; }

    public SSEQSetVarEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.VarID = er.ReadByte();
      this.Value = er.ReadInt16();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      if (this.VarID > (byte) 15)
        Result.GlobalVariables[(int) this.VarID - 16] = this.Value;
      else
        Result.LocalVariables[(int) this.VarID] = this.Value;
    }
  }
}
