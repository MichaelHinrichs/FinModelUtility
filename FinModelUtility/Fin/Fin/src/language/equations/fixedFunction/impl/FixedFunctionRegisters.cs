using System.Collections.Generic;

namespace fin.language.equations.fixedFunction {
  public class FixedFunctionRegisters : IFixedFunctionRegisters {
    private readonly Dictionary<string, IColorRegister> colorRegistersMap_ =
        new();

    private readonly Dictionary<string, IScalarRegister> scalarRegistersMap_ =
        new();

    private readonly List<IColorRegister> colorRegisters_ = new();
    private readonly List<IScalarRegister> scalarRegisters_ = new();

    public IReadOnlyList<IColorRegister> ColorRegisters
      => this.colorRegisters_;

    public IReadOnlyList<IScalarRegister> ScalarRegisters
      => this.scalarRegisters_;

    public IColorRegister GetOrCreateColorRegister(
        string name,
        IColorConstant defaultValue) {
      if (!this.colorRegistersMap_.TryGetValue(name, out var colorRegister)) {
        colorRegister = new ColorRegister {
            Name = name, ColorValue = defaultValue,
        };

        this.colorRegistersMap_[name] = colorRegister;
        this.colorRegisters_.Add(colorRegister);
      }

      return colorRegister;
    }

    public IScalarRegister GetOrCreateScalarRegister(
        string name,
        IScalarConstant defaultValue) {
      if (!this.scalarRegistersMap_.TryGetValue(name, out var scalarRegister)) {
        scalarRegister = new ScalarRegisterImpl {
            Name = name, ScalarValue = defaultValue,
        };

        this.scalarRegistersMap_[name] = scalarRegister;
        this.scalarRegisters_.Add(scalarRegister);
      }

      return scalarRegister;
    }

    private class ColorRegister : BColorValue, IColorRegister {
      public required string Name { get; init; }
      public required IColorValue ColorValue { get; init; }

      public override IScalarValue? Intensity => this.ColorValue.Intensity;
      public override IScalarValue R => this.ColorValue.R;
      public override IScalarValue G => this.ColorValue.G;
      public override IScalarValue B => this.ColorValue.B;

      public override string ToString() => $"{Name} : {ColorValue}";
    }

    private class ScalarRegisterImpl : BScalarValue, IScalarRegister {
      public required string Name { get; init; }
      public required IScalarValue ScalarValue { get; init; }

      public override string ToString() => $"{Name} : {ScalarValue}";
    }
  }
}