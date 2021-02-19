﻿// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQShiftVarEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQShiftVarEvent : SSEQEvent
  {
    public byte VarID { get; private set; }

    public short Value { get; private set; }

    public SSEQShiftVarEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
      this.VarID = er.ReadByte();
      this.Value = er.ReadInt16();
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      if (this.Value < (short) 0)
      {
        if (this.VarID > (byte) 15)
          Result.GlobalVariables[(int) this.VarID - 16] >>= (int) -this.Value;
        else
          Result.LocalVariables[(int) this.VarID] >>= (int) -this.Value;
      }
      else if (this.VarID > (byte) 15)
        Result.GlobalVariables[(int) this.VarID - 16] <<= (int) this.Value;
      else
        Result.LocalVariables[(int) this.VarID] <<= (int) this.Value;
    }
  }
}
