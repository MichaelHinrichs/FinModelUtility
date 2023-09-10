using fin.color;
using fin.language.equations.fixedFunction;
using fin.model;
using fin.util.linq;

using gx;

using uni.ui.common;

namespace uni.ui.right_panel.registers {
  public partial class RegistersTab : UserControl {
    public RegistersTab() {
      InitializeComponent();
    }

    public IModel? Model {
      set {
        var controls = this.tableLayoutPanel_.Controls;
        controls.Clear();

        var materialManager = value?.MaterialManager;
        var registers = materialManager?.Registers;
        if (materialManager == null || registers == null) {
          return;
        }

        var allEquations =
            materialManager
                .All
                .WhereIs<IMaterial, IFixedFunctionMaterial>()
                .Select(mat => mat.Equations)
                .ToArray();

        var colorRegisters = registers.ColorRegisters;
        var scalarRegisters = registers.ScalarRegisters;

        var outputIdentifiers = new[] {
            FixedFunctionSource.OUTPUT_COLOR, FixedFunctionSource.OUTPUT_ALPHA
        };

        var row = 0;
        controls.Add(
            new Label { Text = "Color registers" },
            0,
            row++);
        foreach (var colorRegister in colorRegisters) {
          if (!allEquations.Any(equations => equations.DoOutputsDependOn(
                                    outputIdentifiers,
                                    colorRegister))) {
            continue;
          }

          controls.Add(
              new Label { Text = colorRegister.Name, Dock = DockStyle.Fill, },
              0,
              row);

          var finColorValue = colorRegister.Value;
          var sysColorValue = Color.FromArgb(
              finColorValue.Rb,
              finColorValue.Gb,
              finColorValue.Bb);
          var colorPicker = new ColorPicker {
              Value = sysColorValue, Dock = DockStyle.Fill,
          };
          colorPicker.ValueChanged += newColor => {
            colorRegister.Value = FinColor.FromSystemColor(newColor);
          };

          controls.Add(colorPicker, 1, row);

          row++;
        }

        controls.Add(
            new Label { Text = "Scalar registers" },
            0,
            row++);
        foreach (var scalarRegister in scalarRegisters) {
          if (!allEquations.Any(equations => equations.DoOutputsDependOn(
                                    outputIdentifiers,
                                    scalarRegister))) {
            continue;
          }

          controls.Add(
              new Label { Text = scalarRegister.Name, Dock = DockStyle.Fill },
              0,
              row);

          var trackBar = new TrackBar {
              Minimum = 0,
              Maximum = 100,
              Value = (int) (scalarRegister.Value * 100),
              Dock = DockStyle.Fill,
          };
          trackBar.ValueChanged += (_, _) => {
            scalarRegister.Value = trackBar.Value;
          };

          controls.Add(trackBar, 1, row);

          row++;
        }
      }
    }
  }
}