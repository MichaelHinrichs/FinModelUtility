// Decompiled with JetBrains decompiler
// Type: SevenZip.ICodeProgress
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

namespace SevenZip
{
  public interface ICodeProgress
  {
    void SetProgress(long inSize, long outSize);
  }
}
