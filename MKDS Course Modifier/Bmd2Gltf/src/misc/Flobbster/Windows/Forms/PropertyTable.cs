// Decompiled with JetBrains decompiler
// Type: Flobbster.Windows.Forms.PropertyTable
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections;

namespace Flobbster.Windows.Forms
{
  public class PropertyTable : PropertyBag
  {
    private Hashtable propValues;

    public PropertyTable()
    {
      this.propValues = new Hashtable();
    }

    public object this[string key]
    {
      get
      {
        return this.propValues[(object) key];
      }
      set
      {
        this.propValues[(object) key] = value;
      }
    }

    protected override void OnGetValue(PropertySpecEventArgs e)
    {
      e.Value = this.propValues[(object) e.Property.Name];
      base.OnGetValue(e);
    }

    protected override void OnSetValue(PropertySpecEventArgs e)
    {
      this.propValues[(object) e.Property.Name] = e.Value;
      base.OnSetValue(e);
    }
  }
}
