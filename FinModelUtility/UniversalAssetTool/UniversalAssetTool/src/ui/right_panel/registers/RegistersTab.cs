using fin.language.equations.fixedFunction;
using fin.model;

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

        ;
      }
    }
  }
}