﻿// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Archive_Format.NARCFileSystem
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.IO;

namespace MKDS_Course_Modifier.Archive_Format
{
  public class NARCFileSystem : FileSystem
  {
    public NARCFileSystem(FileSystem.Directory Root)
    {
      this.Root = Root;
    }

    public override byte[] Write()
    {
      return base.Write();
    }
  }
}
