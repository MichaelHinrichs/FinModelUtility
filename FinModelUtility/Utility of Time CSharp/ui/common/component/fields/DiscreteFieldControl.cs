using System.Linq;
using System.Windows.Forms;

using UoT.hacks.fields;

namespace UoT.ui.common.component.fields {
  public partial class DiscreteFieldControl : UserControl {
    private readonly IDiscreteField field_;

    public DiscreteFieldControl(IDiscreteField field) {
      this.field_ = field;
      this.group_.Text = field.Name;

      this.dropdown_.DataSource =
          field.PossibleValues.Select(value => value.Name);
      this.dropdown_.SelectedIndex = field.SelectedValueIndex;

      this.InitializeComponent();
    }

    private void dropdown_SelectedIndexChanged_(
        object sender,
        System.EventArgs e) {
      var selectedValue =
          this.field_.PossibleValues[this.dropdown_.SelectedIndex];
      this.field_.Set(selectedValue);
    }
  }
}