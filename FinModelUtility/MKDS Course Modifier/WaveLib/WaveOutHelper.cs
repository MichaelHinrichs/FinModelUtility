// Decompiled with JetBrains decompiler
// Type: WaveLib.WaveOutHelper
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;

namespace WaveLib
{
  internal class WaveOutHelper
  {
    public static void Try(int err)
    {
      if (err != 0)
        throw new Exception(err.ToString());
    }
  }
}
