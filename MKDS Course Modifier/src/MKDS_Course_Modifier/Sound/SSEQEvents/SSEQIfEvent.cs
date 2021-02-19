﻿// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Sound.SSEQEvents.SSEQIfEvent
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.Language;
using System.IO;

namespace MKDS_Course_Modifier.Sound.SSEQEvents
{
  public class SSEQIfEvent : SSEQEvent
  {
    public SSEQIfEvent(byte EventID, EndianBinaryReader er)
    {
      this.EventID = EventID;
    }

    public override void AddMidiEvents(ref SSEQMidiResult Result)
    {
      Result.If = true;
    }

    public override string ToString()
    {
      return LanguageHandler.GetString("sound.sseq.events.if");
    }
  }
}
