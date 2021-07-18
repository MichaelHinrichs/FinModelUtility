using System.Collections.Generic;
using System.Windows.Forms;

using UoT.hacks.fields;

namespace UoT.ui.common.component.fields {
  public partial class FieldControlList : UserControl {
    public FieldControlList() {
      this.InitializeComponent();
    }

    public IReadOnlyList<IField> Fields {
      set {
        this.fieldControlListContainer_.Controls.Clear();

        foreach (var field in value) {
          if (field is IDiscreteField discreteField) {
            this.fieldControlListContainer_.Controls.Add(
                new DiscreteFieldControl(discreteField));
          }
        }
      }
    }
  }
}