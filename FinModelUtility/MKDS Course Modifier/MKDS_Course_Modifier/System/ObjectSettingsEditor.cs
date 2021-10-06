// Decompiled with JetBrains decompiler
// Type: System.ObjectSettingsEditor
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using MKDS_Course_Modifier.MKDS;
using MKDS_Course_Modifier.UI.MKDS;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System
{
  public class ObjectSettingsEditor : UITypeEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(
      ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.Modal;
    }

    public override object EditValue(
      ITypeDescriptorContext context,
      IServiceProvider provider,
      object value) {
      throw new Exception("Shouldn't be here");
      /*IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
      if (service != null)
      {
        ObjectDbObjectSettingsEditor objectSettingsEditor = new ObjectDbObjectSettingsEditor((List<ObjectDb.Object.Setting.SettingData>) value);
        int num = (int) service.ShowDialog((Form) objectSettingsEditor);
        value = (object) objectSettingsEditor.Setting;
      }*/
      return value;
    }
  }
}
