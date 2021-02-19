// Decompiled with JetBrains decompiler
// Type: System.RotationSelector
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using AngleAltitudeControls;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System
{
  public class RotationSelector : UITypeEditor
  {
    public override UITypeEditorEditStyle GetEditStyle(
      ITypeDescriptorContext context)
    {
      return UITypeEditorEditStyle.DropDown;
    }

    public override object EditValue(
      ITypeDescriptorContext context,
      IServiceProvider provider,
      object value)
    {
      IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService(typeof (IWindowsFormsEditorService));
      if (service != null)
      {
        AngleSelector angleSelector = new AngleSelector();
        angleSelector.Height = 100;
        angleSelector.Width = 100;
        angleSelector.Angle = (int) ((double) (float) value - 90.0);
        service.DropDownControl((Control) angleSelector);
        value = (object) (angleSelector.Angle + 90);
      }
      return value;
    }
  }
}
