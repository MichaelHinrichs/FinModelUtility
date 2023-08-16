using System.Collections.Generic;

namespace fin.language.equations.fixedFunction {
  public interface IFixedFunctionRegisters {
    IReadOnlyList<IColorRegister> ColorRegisters { get; }
    IReadOnlyList<IScalarRegister> ScalarRegisters { get; }

    IColorRegister CreateColorRegister(
        string name,
        IColorConstant defaultValue);

    IScalarRegister CreateScalarRegister(
        string name,
        IScalarConstant defaultValue);
  }

  public interface IColorRegister : IColorNamedValue { }

  public interface IScalarRegister : IScalarNamedValue { }
}