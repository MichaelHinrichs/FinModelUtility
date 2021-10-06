// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Language.MLDisplayNameAttribute
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.ComponentModel;

namespace MKDS_Course_Modifier.Language
{
  [AttributeUsage(AttributeTargets.All)]
  internal class MLDisplayNameAttribute : DisplayNameAttribute
  {
    private bool ps = false;

    public MLDisplayNameAttribute(string displayname)
      : base(displayname)
    {
    }

    public override string DisplayName
    {
      get
      {
        if (!this.ps)
        {
          this.ps = true;
          this.DisplayNameValue = LanguageHandler.GetString(base.DisplayName);
        }
        return base.DisplayName;
      }
    }
  }
}
