// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Language.MLCategoryAttribute
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;

namespace MKDS_Course_Modifier.Language
{
  [AttributeUsage(AttributeTargets.All)]
  public sealed class MLCategoryAttribute : CategoryAttribute
  {
    public MLCategoryAttribute(string category)
      : base(category)
    {
    }

    protected override string GetLocalizedString(string value)
    {
      return LanguageHandler.GetString(value);
    }
  }
}
