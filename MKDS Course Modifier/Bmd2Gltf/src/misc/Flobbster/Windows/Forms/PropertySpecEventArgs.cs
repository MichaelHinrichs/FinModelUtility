// Decompiled with JetBrains decompiler
// Type: Flobbster.Windows.Forms.PropertySpecEventArgs
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;

namespace Flobbster.Windows.Forms
{
  public class PropertySpecEventArgs : EventArgs
  {
    private PropertySpec property;
    private object val;

    public PropertySpecEventArgs(PropertySpec property, object val)
    {
      this.property = property;
      this.val = val;
    }

    public PropertySpec Property
    {
      get
      {
        return this.property;
      }
    }

    public object Value
    {
      get
      {
        return this.val;
      }
      set
      {
        this.val = value;
      }
    }
  }
}
