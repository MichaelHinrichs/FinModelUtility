// Decompiled with JetBrains decompiler
// Type: System.ObjectSelector
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System
{
  public class ObjectSelector : UITypeEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(
      ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.Modal;
    }

    public override object EditValue(
      ITypeDescriptorContext context,
      IServiceProvider provider,
      object value)
    {
      throw new Exception("Shouldn't be here");
      /*IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
      if (service != null)
      {
        MKDS_Course_Modifier.UI.MKDS.ObjectSelector objectSelector = new MKDS_Course_Modifier.UI.MKDS.ObjectSelector(BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes((ushort) value)).Reverse<byte>().ToArray<byte>(), 0));
        if (service.ShowDialog((Form) objectSelector) == DialogResult.OK)
          value = (object) BitConverter.ToUInt16(((IEnumerable<byte>) BitConverter.GetBytes(objectSelector.ObjectID)).Reverse<byte>().ToArray<byte>(), 0);
      }*/
      return value;
    }
  }
}
