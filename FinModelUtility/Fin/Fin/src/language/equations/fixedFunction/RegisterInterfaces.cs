using System.Collections.Generic;

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

  public interface IColorRegister : IColorNamedValue { }

  public interface IScalarRegister : IScalarNamedValue { }
}