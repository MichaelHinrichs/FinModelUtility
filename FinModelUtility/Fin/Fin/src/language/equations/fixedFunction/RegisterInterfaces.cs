using System.Collections.Generic;

using fin.color;

namespace fin.language.equations.fixedFunction {
  public interface IFixedFunctionRegisters {
    IReadOnlyList<IColorRegister> ColorRegisters { get; }
    IReadOnlyList<IScalarRegister> ScalarRegisters { get; }

    IColorRegister GetOrCreateColorRegister(
        string name,
        IColorConstant defaultValue);

    IScalarRegister GetOrCreateScalarRegister(
        string name,
        IScalarConstant defaultValue);
  }

  public interface IColorRegister : IColorNamedValue {
    IColorConstant DefaultValue { get; }
    IColor Value { get; }
  }

  public interface IScalarRegister : IScalarNamedValue {
    IScalarConstant DefaultValue { get; }
    float Value { get; }
  }
}