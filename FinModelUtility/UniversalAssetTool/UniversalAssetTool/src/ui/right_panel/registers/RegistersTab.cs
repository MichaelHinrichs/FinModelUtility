using fin.language.equations.fixedFunction;
using fin.model;

using uni.ui.common;

namespace uni.ui.right_panel.registers {
  public partial class RegistersTab : UserControl {
    private IReadOnlyList<IColorRegister>? colorRegisters_;
    private IReadOnlyList<IScalarRegister>? scalarRegisters_;

    public RegistersTab() {
      InitializeComponent();
    }

    public IModel? Model {
      set {
        var registers = value?.MaterialManager.Registers;

        this.colorRegisters_ = registers?.ColorRegisters;
        this.scalarRegisters_ = registers?.ScalarRegisters;

        var controls = this.tableLayoutPanel_.Controls;
        controls.Clear();

        var row = 0;
        controls.Add(
            new Label { Text = "Color registers" },
            0,
            row++);
        if (this.colorRegisters_ != null) {
          foreach (var colorRegister in this.colorRegisters_) {
            controls.Add(
                new Label { Text = colorRegister.Name, Dock = DockStyle.Fill, },
                0,
                row);

            var finColorValue = colorRegister.Value;
            var sysColorValue = Color.FromArgb(
                finColorValue.Rb,
                finColorValue.Gb,
                finColorValue.Bb);
            controls.Add(
                new ColorPicker {
                    Value = sysColorValue, Dock = DockStyle.Fill,
                },
                1,
                row);

            row++;
          }
        }

        // controls.Add(new Label(), 0, row++);

        controls.Add(
            new Label { Text = "Scalar registers" },
            0,
            row++);
        if (this.scalarRegisters_ != null) {
          foreach (var scalarRegister in this.scalarRegisters_) {
            controls.Add(
                new Label { Text = scalarRegister.Name, Dock = DockStyle.Fill },
                0,
                row);

            controls.Add(
                new TrackBar {
                    Minimum = 0,
                    Maximum = 100,
                    Value = (int) (scalarRegister.Value * 100),
                    Dock = DockStyle.Fill,
                },
                1,
                row);

            row++;
          }
        }
      }
    }
  }
}